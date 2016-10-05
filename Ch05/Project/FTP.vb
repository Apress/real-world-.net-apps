Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic

Public Class EndDownloadEventArgs : Inherits EventArgs
  Public Message As String
End Class

Public Class EndUploadEventArgs : Inherits EventArgs
  Public Message As String
End Class

Public Class TransferProgressChangedEventArgs : Inherits EventArgs
  Public TransferredByteCount As Integer

  Public Sub New()
  End Sub

  Public Sub New(ByVal size As Integer)
    TransferredByteCount = size
  End Sub

End Class

Public Delegate Sub BeginDownloadEventHandler(ByVal sender As Object, _
  ByVal e As EventArgs)

Public Delegate Sub EndDownloadEventHandler(ByVal sender As Object, _
  ByVal e As EndDownloadEventArgs)

Public Delegate Sub BeginUploadEventHandler(ByVal sender As Object, _
  ByVal e As EventArgs)

Public Delegate Sub EndUploadEventHandler(ByVal sender As Object, _
  ByVal e As EndUploadEventArgs)

Public Delegate Sub TransferProgressChangedEventHandler(ByVal sender As Object, _
  ByVal e As TransferProgressChangedEventArgs)

Public Class FTP

  Public Event BeginDownload As BeginDownloadEventHandler
  Protected Overridable Sub OnBeginDownload(ByVal e As EventArgs)
    RaiseEvent BeginDownload(Me, e)
  End Sub

  Public Event EndDownload As EndDownloadEventHandler
  Protected Overridable Sub OnEndDownload(ByVal e As EndDownloadEventArgs)
    RaiseEvent EndDownload(Me, e)
  End Sub

  Public Event BeginUpload As BeginUploadEventHandler
  Protected Overridable Sub OnBeginUpload(ByVal e As EventArgs)
    RaiseEvent BeginUpload(Me, e)
  End Sub

  Public Event EndUpload As EndUploadEventHandler
  Protected Overridable Sub OnEndUpload(ByVal e As EndUploadEventArgs)
    RaiseEvent EndUpload(Me, e)
  End Sub

  Public Event TransferProgressChanged As TransferProgressChangedEventHandler
  Protected Overridable Sub OnTransferProgressChanged(ByVal e As TransferProgressChangedEventArgs)
    RaiseEvent TransferProgressChanged(Me, e)
  End Sub

  Private port As Integer = 21
  Private controlSocket, dataSocket As Socket
  Private serverAddress As String
  Private directoryListField As String
  Private dataTransferThread As Thread
  'indicates whether dataTransferThread is being used
  'if it is, do not allow another operation
  Private transferring As Boolean

  'for transferring filename and localDir when calling DoUpload and
  'DoDownload
  Private filename, localDir As String

  Public replyMessage As String
  Public replyCode As String

  Public ReadOnly Property Connected() As Boolean
    Get
      If Not controlSocket Is Nothing Then
        Return controlSocket.Connected
      Else
        Return False
      End If
    End Get
  End Property

  Public ReadOnly Property DirectoryList() As String
    Get
      Return directoryListField
    End Get
  End Property

  Public Sub ChangeDir(ByVal path As String)
    SendCommand("CWD " & path & ControlChars.CrLf)
    GetResponse()
  End Sub

  Public Sub ChangeToAsciiMode()
    SendTYPECommand("A")
  End Sub

  Public Sub Connect(ByVal server As String)
    Try
      controlSocket = New _
        Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
      controlSocket.Connect(New _
        IPEndPoint(Dns.Resolve(server).AddressList(0), port))
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

  Public Function DeleteDir(ByVal dir As String) As Boolean
    SendCommand("RMD " & dir & ControlChars.CrLf)
    GetResponse()
    If replyCode.StartsWith("2") Then
      Return True
    Else
      Return False
    End If
  End Function

  Public Function DeleteFile(ByVal filename As String) As Boolean
    SendCommand("DELE " & filename & ControlChars.CrLf)
    GetResponse()
    If replyCode.StartsWith("2") Then
      Return True
    Else
      Return False
    End If
  End Function

  Public Sub Disconnect()
    If controlSocket.Connected Then
      SendCommand("QUIT" & ControlChars.CrLf)
      GetResponse()
      controlSocket.Shutdown(SocketShutdown.Both)
      controlSocket.Close()
    End If
  End Sub

  Public Sub DoDownload()
    OnBeginDownload(New EventArgs())
    Dim completePath As String = Path.Combine(localDir, filename)
    Try

      Dim f As FileStream = File.Create(completePath)
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
        OnTransferProgressChanged(New _
          TransferProgressChangedEventArgs(totalByteReceived))
      Loop Until byteReceivedCount = 0

      f.Close()
      'because the 226 response might be sent 
      'before the data connection finishes, only try to get "completion message"
      'if it's not yet sent
      If replyMessage.IndexOf("226 ") = -1 Then
        GetResponse()
      End If

      SendTYPECommand("A")
    Catch
    End Try
    Dim e As New EndDownloadEventArgs()
    e.Message = "Finished downloading " & filename
    OnEndDownload(e)
    transferring = False
  End Sub

  Public Sub DoUpload()
    OnBeginUpload(New EventArgs())
    Dim completePath As String = Path.Combine(localDir, filename)
    Try
      Dim f As FileStream = _
        File.Open(completePath, FileMode.Open, FileAccess.Read)
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
          OnTransferProgressChanged(New TransferProgressChangedEventArgs(totalByteSent))
        End If
      Loop Until byteReadCount = 0

      dataSocket.Shutdown(SocketShutdown.Both)
      dataSocket.Close()
      f.Close()
      GetResponse()
      SendTYPECommand("A")
    Catch e As Exception
      replyMessage = e.ToString()
    End Try
    Dim ev As New EndUploadEventArgs()
    ev.Message = "Finished uploading " & filename
    OnEndUpload(ev)
    transferring = False
  End Sub

  Public Sub Download(ByVal filename As String, ByVal localdir As String)
    If Not transferring Then
      transferring = True
      Me.filename = filename
      Me.localDir = localdir
      dataTransferThread = _
        New Thread(New ThreadStart(AddressOf DoDownload))
      dataTransferThread.Start()
    End If
  End Sub

  Public Function GetCurrentRemoteDir() As String
    SendCommand("PWD" & ControlChars.CrLf)
    GetResponse()
    If replyCode.Equals("257") Then
      Return GetRemoteDirectory(replyMessage)
    End If
  End Function

  Public Sub GetDirList()
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

    directoryListField = msg.ToString
    'because the 226 response might be sent 
    'before the data connection finishes, only try to get "completion message"
    'if it's not yet sent
    If replyMessage.IndexOf("226 ") = -1 Then
      GetResponse()
    End If

  End Sub

  Private Function GetRemoteDirectory(ByVal message As String) As String
    'message is the server response upon sending the "PWD" command
    'its format is something like: "path" is current directory
    'this function obtains the string between the double quotes
    Dim path As String = ""
    Dim index As Integer = message.IndexOf("""")
    If index <> -1 Then
      Dim index2 As Integer = message.IndexOf("""", index + 1)
      If index2 <> -1 Then
        path = message.Substring(index + 1, index2 - index - 1)
      End If
    End If
    Return path
  End Function

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
      'single line
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
      replyMessage = "Unexpected Error."
    End If

  End Sub

  Public Function Login(ByVal userName As String, ByVal password As String) As Boolean
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
      If replyCode.Equals("230") Then
        Return True
      Else
        Return False
      End If
    End If
  End Function

  Public Sub MakeDir(ByVal dir As String)
    SendCommand("MKD " & dir & ControlChars.CrLf)
    GetResponse()
  End Sub

  Private Sub PassiveDataConnection()
    SendCommand("PASV" & ControlChars.CrLf)
    GetResponse()
    Dim addr As String = replyMessage

    addr = addr.Substring(addr.IndexOf("("c) + 1, addr.IndexOf(")"c) - addr.IndexOf("("c) - 1)
    Dim address As String() = addr.Split(","c)

    Dim ip As String = address(0) & "." & address(1) & "." & address(2) & "." & address(3)
    Dim port As Integer = Convert.ToInt32(address(4)) * 256 + Convert.ToInt32(address(5))

    dataSocket = New _
      Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
    dataSocket.Connect(New IPEndPoint(IPAddress.Parse(ip), port))
  End Sub

  Public Sub Rename(ByVal renameFrom As String, ByVal renameTo As String)
    SendCommand("RNFR " & renameFrom & ControlChars.CrLf)
    GetResponse()
    Console.WriteLine(replyCode & " " & replyMessage)
    SendCommand("RNTO " & renameTo & ControlChars.CrLf)
    GetResponse()
    Console.WriteLine(replyCode & " " & replyMessage)
  End Sub

  Private Sub SendCommand(ByVal command As String)
    Try
      controlSocket.Send(Encoding.ASCII.GetBytes(command), command.Length, 0)
    Catch
    End Try
  End Sub

  Private Sub SendTYPECommand(ByVal type As String)
    SendCommand("TYPE " & type & ControlChars.CrLf)
    GetResponse()
  End Sub

  Public Sub Upload(ByVal filename As String, ByVal localDir As String)
    If Not transferring Then
      transferring = True
      Me.filename = filename
      Me.localDir = localDir
      dataTransferThread = New Thread(New ThreadStart(AddressOf DoUpload))
      dataTransferThread.Start()
    End If
  End Sub

End Class