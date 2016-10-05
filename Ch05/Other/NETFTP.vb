Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports Microsoft.VisualBasic

Public Class NETFTP

  Private port As Integer = 21
  Private controlSocket As _
    New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
  Private dataSocket As Socket
  Private serverAddress As String

  Public replyMessage As String
  Public replyCode As String

  Public Sub Connect(ByVal server As String)
    Try
      controlSocket.Connect(New IPEndPoint(Dns.Resolve(server).AddressList(0), port))
    Catch e As Exception
      Console.WriteLine(e.ToString())
      Return
    End Try
    If controlSocket.Connected Then
      Console.WriteLine("Connected. Waiting for reply...")
      GetResponse()
    Else
      Console.WriteLine("Couldn't connect.")
    End If
  End Sub

  Private Sub PassiveDataConnection()
    SendCommand("PASV" & ControlChars.CrLf)
    GetResponse()
    Dim addr As String = replyMessage

    addr = addr.Substring(addr.IndexOf("("c) + 1, addr.IndexOf(")"c) - addr.IndexOf("("c) - 1)
    Dim address As String() = addr.Split(","c)

    Dim ip As String = address(0) & "." & address(1) & "." & address(2) & "." & address(3)
    Dim port As Integer = Convert.ToInt32(address(4)) * 256 + Convert.ToInt32(address(5))

    dataSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
    dataSocket.Connect(New IPEndPoint(IPAddress.Parse(ip), port))
  End Sub

  Private Sub GetResponse()
    ' this method listens for the server response and receives all bytes
    ' sent by the server
    ' A server response can be single line or multiline.
    ' If the fourth byte of the first line is a hyphen, then it is 
    ' multiline. If multiline, waits until the line that starts with the 
    ' response code (the first three bytes of the first line).

    Dim bytes(511) As Byte ' an array of 512 bytes
    Dim receivedByteCount As Integer
    Dim response As String = ""

    ' get the first line
    receivedByteCount = controlSocket.Receive(bytes)
    response = Encoding.ASCII.GetString(bytes, 0, receivedByteCount)
    Dim multiline As Boolean = (response.Chars(3) = "-"c)

    If multiline Then
      If response.Length > 3 Then
        replyCode = response.Substring(0, 3)
      End If
      Dim line As String = ""
      Dim lastLineReached As Boolean = False
      While Not lastLineReached
        receivedByteCount = controlSocket.Receive(bytes)
        line = Encoding.ASCII.GetString(bytes, 0, receivedByteCount)
        response += line

        If line.IndexOf(ControlChars.CrLf & replyCode & " ") <> -1 Then
          lastLineReached = True
        End If
        If lastLineReached Then
          'just wait until CRLF is reached
          While Not line.EndsWith(ControlChars.CrLf)
            receivedByteCount = controlSocket.Receive(bytes)
            line = Encoding.ASCII.GetString(bytes, 0, receivedByteCount)
            response += line
          End While
        End If
      End While
    Else
      While receivedByteCount = bytes.Length And Not response.EndsWith(ControlChars.CrLf)
        receivedByteCount = controlSocket.Receive(bytes)
        response += Encoding.ASCII.GetString(bytes, 0, receivedByteCount)
      End While
    End If

    Console.WriteLine()
    Console.Write(response)

    If response.Length > 3 Then
      replyCode = response.Substring(0, 3)
      replyMessage = response.Substring(3, response.Length - 3)
    Else
      replyCode = ""
      replyMessage = "Unexpected Error has occurred."
    End If

  End Sub

  Public Function Login(ByVal userName As String, ByVal password As String) As String
    If controlSocket.Connected Then

      ' Sending user name
      Dim command As String
      command = "USER " & userName & ControlChars.CrLf
      Console.WriteLine(command)
      SendCommand(command)
      GetResponse()

      ' Sending password
      command = "PASS " & password & ControlChars.CrLf
      Console.Write("PASS")  'do not display password
      SendCommand(command)
      GetResponse()
      Return replyCode
    Else
      Console.Write("Login failed because no connection is available")
    End If
    Return ""
  End Function

  Private Sub SendCommand(ByVal command As String)
    Try
      controlSocket.Send(Encoding.ASCII.GetBytes(command), command.Length, 0)
    Catch
    End Try
  End Sub

  Public Sub SendCWDCommand(ByVal path As String)
    SendCommand("CWD " & path & ControlChars.CrLf)
    GetResponse()
  End Sub

  Public Sub SendDELECommand(ByVal filename As String)
    SendCommand("DELE " & filename & ControlChars.CrLf)
    GetResponse()
  End Sub

  Public Sub SendLISTCommand()

    PassiveDataConnection()
    SendCommand("LIST" & ControlChars.CrLf)
    GetResponse()

    Dim byteReceivedCount As Integer
    Dim msg As New StringBuilder(2048)
    Dim bytes(511) As Byte
    Do
      byteReceivedCount = dataSocket.Receive(bytes, bytes.Length, SocketFlags.None)
      msg.Append(Encoding.ASCII.GetString(bytes, 0, byteReceivedCount))
    Loop Until byteReceivedCount = 0

    Console.WriteLine(msg.ToString())

    'because the 226 response might be sent 
    'before the data connection finishes, only try to get "completion message"
    'if it's not yet sent
    If replyMessage.IndexOf("226 ") = -1 Then
      GetResponse()
    End If
  End Sub

  Public Sub SendMKDCommand(ByVal dir As String)
    SendCommand("MKD " & dir & ControlChars.CrLf)
    GetResponse()
  End Sub

  Public Sub SendNOOPCommand()
    SendCommand("NOOP" & ControlChars.CrLf)
    GetResponse()
  End Sub

  Public Sub SendPWDCommand()
    SendCommand("PWD" & ControlChars.CrLf)
    GetResponse()
  End Sub

  Public Sub SendRMDCommand(ByVal dir As String)
    SendCommand("RMD " & dir & ControlChars.CrLf)
    GetResponse()
  End Sub

  Public Sub SendQUITCommand()
    SendCommand("QUIT" & ControlChars.CrLf)
    GetResponse()
    controlSocket.Shutdown(SocketShutdown.Both)
    controlSocket.Close()
  End Sub

  Public Sub SendRETRCommand(ByVal filename As String)

    Dim f As FileStream = File.Create(filename)

    SendTYPECommand("I")
    PassiveDataConnection()

    SendCommand("RETR " & filename & ControlChars.CrLf)
    GetResponse()

    Dim byteReceivedCount As Integer
    Dim totalByteReceived As Integer = 0
    Dim bytes(511) As Byte
    Do
      byteReceivedCount = _
        dataSocket.Receive(bytes, bytes.Length, SocketFlags.None)
      totalByteReceived += byteReceivedCount
      f.Write(bytes, 0, byteReceivedCount)
    Loop Until byteReceivedCount = 0

    f.Close()

    'because the 226 response might be sent 
    'before the data connection finishes, only try to get "completion message"
    'if it's not yet sent
    If replyMessage.IndexOf("226 ") = -1 Then
      GetResponse()
    End If

    SendTYPECommand("A")
  End Sub

  Public Sub SendSTORCommand(ByVal filename As String)

    Dim f As FileStream = File.Open(filename, FileMode.Open)
    SendTYPECommand("I")
    PassiveDataConnection()

    SendCommand("STOR " & filename & ControlChars.CrLf)
    GetResponse()

    Dim byteReadCount As Integer
    Dim totalByteSent As Integer
    Dim bytes(511) As Byte

    Do
      byteReadCount = f.Read(bytes, 0, bytes.Length)
      If byteReadCount <> 0 Then
        dataSocket.Send(bytes, byteReadCount, SocketFlags.None)
        totalByteSent += byteReadCount
      End If
    Loop Until byteReadCount = 0

    dataSocket.Shutdown(SocketShutdown.Both)
    dataSocket.Close()

    f.Close()
    GetResponse()

    SendTYPECommand("A")
  End Sub

  Public Sub SendSYSTCommand()
    SendCommand("SYST" & ControlChars.CrLf)
    GetResponse()
  End Sub

  Public Sub SendTYPECommand(ByVal type As String)
    SendCommand("TYPE " & type & ControlChars.CrLf)
    GetResponse()
  End Sub


  Public Shared Sub Main(ByVal args As String())
    If args.Length <> 3 Then
      Console.WriteLine("usage: NETFTP server username password")
    Else
      Dim ftp As New NETFTP()
      ftp.Connect(args(0))
      Dim replyCode As String = ftp.Login(args(1), args(2))

      If replyCode.Equals("230") Then
        'login successful, allow user to type in commands
        ftp.SendSYSTCommand()
        ftp.SendTYPECommand("A")

        Dim command As String = ""
        Try
          While Not command.ToUpper.Equals("QUIT")
            Console.Write("NETFTP>")
            command = Console.ReadLine().Trim()
            If command.ToUpper.Equals("PWD") Then
              ftp.SendPWDCommand()
            ElseIf command.ToUpper.StartsWith("CWD") Then
              If command.Length > 3 Then
                Dim path As String = command.Substring(4).Trim()
                If path.Equals("") Then
                  Console.WriteLine("Please specify the directory to change to")
                Else
                  ftp.SendCWDCommand(path)
                End If
              End If
            ElseIf command.ToUpper.StartsWith("DELE") Then
              If command.Length > 4 Then
                Dim path As String = command.Substring(5).Trim()
                If path.Equals("") Then
                  Console.WriteLine("Please specify the file to delete")
                Else
                  ftp.SendDELECommand(path)
                End If
              End If
            ElseIf command.ToUpper.Equals("LIST") Then
              ftp.SendLISTCommand()
            ElseIf command.ToUpper.StartsWith("MKD") Then
              If command.Length > 3 Then
                Dim dir As String = command.Substring(4).Trim()
                If dir.Equals("") Then
                  Console.WriteLine("Please specify the name of the directory to create")
                Else
                  ftp.SendMKDCommand(dir)
                End If
              End If
            ElseIf command.ToUpper.Equals("QUIT") Then
            ElseIf command.ToUpper.StartsWith("RMD") Then
              If command.Length > 3 Then
                Dim dir As String = command.Substring(4).Trim()
                If dir.Equals("") Then
                  Console.WriteLine("Please specify the name of the directory to delete")
                Else
                  ftp.SendRMDCommand(dir)
                End If
              End If
            ElseIf command.ToUpper.StartsWith("RETR") Then
              If command.Length > 4 Then
                Dim filename As String = command.Substring(5).Trim()
                If filename.Equals("") Then
                  Console.WriteLine("Please specify a file to retrieve")
                Else
                  ftp.SendRETRCommand(filename)
                End If
              End If
            ElseIf command.ToUpper.StartsWith("STOR") Then
              If command.Length > 4 Then
                Dim filename As String = command.Substring(5).Trim()
                If filename.Equals("") Then
                  Console.WriteLine("Please specify a file to store")
                Else
                  ftp.SendSTORCommand(filename)
                End If
              End If
            Else
              Console.WriteLine("Invalid command.")
            End If
          End While
          ftp.SendQUITCommand()
        Catch e As Exception
          Console.WriteLine(e.ToString())
        End Try

        Console.WriteLine("Thank you for using NETFTP.")
      Else
        ftp.SendQUITCommand()
        Console.WriteLine("Login failed. Please try again.")
      End If
    End If
  End Sub

End Class
