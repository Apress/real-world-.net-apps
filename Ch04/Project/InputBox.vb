Option Explicit On 
Option Strict On

Imports System
Imports System.Windows.Forms
Imports System.Collections

Public Class InputBox : Inherits TextBox
  'the interface or class that is being updated
  Public rect As Rect
  Public Property LineArrayList() As ArrayList
    Get
      Dim str() As String = Me.Lines
      Dim al As New ArrayList()
      Dim count As Integer = str.Length
      Dim i As Integer
      For i = 0 To count - 1
        al.Add(str(i))
      Next
      Return al
    End Get
    Set(ByVal al As ArrayList)
      Dim itemCount As Integer = al.Count
      If itemCount = 0 Then
        Me.Lines = Nothing
      Else
        Dim str(itemCount - 1) As String
        Dim i As Integer
        For i = 0 To itemCount - 1
          str(i) = CType(al.Item(i), String)
        Next
        Me.Lines = str
      End If
    End Set
  End Property

End Class
