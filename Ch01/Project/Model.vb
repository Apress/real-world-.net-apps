Option Strict On
Option Explicit On 

Imports System
Imports System.Collections

Public Class Model
    Private list As New ArrayList(1024)  ' number of initial lines
    Private longestLineCharCountField As Integer
    Public Event LineCountChanged As LineCountEventHandler
    Public Event LongestLineCharCountChanged As LongestLineEventHandler

    Public Sub New()
        list.Add("")
    End Sub

    Protected Overridable Sub OnLineCountChanged(ByVal e As LineCountEventArgs)
        RaiseEvent LineCountChanged(Me, e)
    End Sub

    Protected Overridable Sub OnLongestLineCharCountChanged( _
      ByVal e As LongestLineEventArgs)
        RaiseEvent LongestLineCharCountChanged(Me, e)
    End Sub

    Public ReadOnly Property LongestLineCharCount() As Integer
        Get
            Dim lineCount As Integer = list.Count
            If lineCount = 0 Then
                Return 0
            Else
                Dim i, max As Integer
                For i = 0 To lineCount - 1
                    Dim thisLineCharCount As Integer = CType(list.Item(i), String).Length
                    If thisLineCharCount > max Then
                        max = thisLineCharCount
                    End If
                Next
                Return max
            End If
        End Get
    End Property

    Public ReadOnly Property CharCount() As Integer
        ' the total of lengths of all lines, therefore newline is not counted
        Get
            Dim i, total As Integer
            For i = 0 To LineCount - 1
                total = total + list.Item(i).ToString().Length
            Next i
            Return total
        End Get
    End Property


    Public ReadOnly Property LineCount() As Integer
        Get
            Return list.Count
        End Get
    End Property

    Public Function GetLine(ByVal lineNo As Integer) As String 'lineNo is 1-based
        If lineNo > 0 And lineNo <= list.Count Then
            Return CType(list.Item(lineNo - 1), String)
        Else
            Throw New ArgumentOutOfRangeException()
        End If
    End Function

    Public Function InsertData(ByVal data As String, _
      ByVal insertLocation As ColumnLine) As ColumnLine

        'delete vbLf character
        data = data.Replace(Microsoft.VisualBasic.Constants.vbLf.ToString(), "")

        Dim initialLongestLineCharCount As Integer = LongestLineCharCount
        Dim x As Integer = insertLocation.Column
        Dim y As Integer = insertLocation.Line

        Dim returnInserted As Boolean
        ' data may contain carriage return character
        Dim thisLine As String = GetLine(y)
        Dim head As String = thisLine.Substring(0, x - 1)
        Dim tail As String = thisLine.Substring(x - 1)
        list.RemoveAt(y - 1)
        Dim startIndex As Integer
        Do While (startIndex >= 0)
            Dim endIndex As Integer = _
              data.IndexOf(Microsoft.VisualBasic.Constants.vbCr, startIndex)
            Dim line As String
            If endIndex = -1 Then
                line = data.Substring(startIndex)
                'don't use SetLine bec it can raise event
                Dim newLine As String = head & line & tail
                list.Insert(y - 1, newLine)
                x = head.Length + line.Length + 1
                startIndex = endIndex
            Else
                line = data.Substring(startIndex, endIndex - startIndex)
                list.Insert(y - 1, head & line)
                returnInserted = True
                y = y + 1
                x = 1
                head = ""
                startIndex = endIndex + 1 'without carriage return
            End If

        Loop

        Dim currentCharCount As Integer = LongestLineCharCount
        If initialLongestLineCharCount <> currentCharCount Then
            OnLongestLineCharCountChanged(New LongestLineEventArgs(currentCharCount))
        End If
        If returnInserted Then
            OnLineCountChanged(New LineCountEventArgs(LineCount))
        End If
        Return New ColumnLine(x, y)
    End Function

    Public Sub InsertLine(ByVal lineNo As Integer, ByVal line As String)
        If lineNo > 0 And lineNo <= list.Count + 1 Then
            Dim oldLongestLineCharCount As Integer = LongestLineCharCount
            list.Insert(lineNo - 1, line)
            OnLineCountChanged(New LineCountEventArgs(LineCount))
            Dim newLongestLineCharCount As Integer = LongestLineCharCount
            If oldLongestLineCharCount <> newLongestLineCharCount Then
                OnLongestLineCharCountChanged(New _
                  LongestLineEventArgs(newLongestLineCharCount))
            End If
        End If
    End Sub

    Public Sub InsertChar(ByVal c As Char, ByVal insertLocation As ColumnLine)
        Dim oldLongestLineCharCount As Integer = LongestLineCharCount
        Dim oldLine As String = list.Item(insertLocation.Line - 1).ToString()
        Dim newLine As String = _
          oldLine.Insert(insertLocation.Column - 1, c.ToString())
        list.Item(insertLocation.Line - 1) = newLine
        Dim newLongestLineCharCount As Integer = LongestLineCharCount
        If oldLongestLineCharCount <> newLongestLineCharCount Then
            OnLongestLineCharCountChanged(New _
              LongestLineEventArgs(newLongestLineCharCount))
        End If
    End Sub

    Public Sub RemoveLine(ByVal lineNo As Integer, ByVal triggerEvent As Boolean)
        If lineNo > 0 And lineNo <= list.Count Then
            Dim oldLongestLineCharCount As Integer = LongestLineCharCount
            list.RemoveAt(lineNo - 1)
            If triggerEvent Then
                Dim newLongestLineCharCount As Integer = LongestLineCharCount
                If oldLongestLineCharCount <> newLongestLineCharCount Then
                    OnLongestLineCharCountChanged(New _
                      LongestLineEventArgs(newLongestLineCharCount))
                End If
                OnLineCountChanged(New LineCountEventArgs(LineCount))
            End If
        End If
    End Sub

    Public Sub SetLine(ByVal lineNo As Integer, ByVal line As String)
        Dim oldLongestLineCharCount As Integer = LongestLineCharCount
        If (lineNo > 0 And lineNo <= list.Count) Then
            list.Item(lineNo - 1) = line
        End If
        Dim newLongestLineCharCount As Integer = LongestLineCharCount
        If oldLongestLineCharCount <> newLongestLineCharCount Then
            OnLongestLineCharCountChanged(New _
              LongestLineEventArgs(newLongestLineCharCount))
        End If
    End Sub

    Public Sub DeleteChar(ByVal deleteLocation As ColumnLine)
        Dim oldLongestLineCharCount As Integer = LongestLineCharCount
        list.Item(deleteLocation.Line - 1) = _
          GetLine(deleteLocation.Line).Remove(deleteLocation.Column - 1, 1)
        Dim newLongestLineCharCount As Integer = LongestLineCharCount
        If oldLongestLineCharCount <> newLongestLineCharCount Then
            OnLongestLineCharCountChanged(New _
              LongestLineEventArgs(newLongestLineCharCount))
        End If
    End Sub

End Class

