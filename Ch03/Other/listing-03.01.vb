Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO
Imports System.Threading

Public Class Form1 : Inherits Form

  Private label1, label2 As New Label()
  Private newsItems() As String = _
    {"Safest Aerobic Machine Launched", _
      "First Dog Cloning Is Only Days Away", _
      "Reviving the Extinct Tasmanian Tiger"}

  Private businessItems() As String = _
    {"FirstMeasure Software to Go Nasdaq", _
      "MFMF Directors To Meet For The First Time", _
      "First Sign of Economic Recovery Finally At Sight", _
      "Euro Hits Record Low (Again)"}

  Private thread1, thread2 As Thread

  Public Sub New()
    label1.Width = 280
    label1.Height = 30
    label1.Location = New Point(1, 10)
    label1.TextAlign = ContentAlignment.MiddleRight

    label2.Width = 280
    label2.Height = 30
    label2.Location = New Point(1, 40)

    Me.Controls.Add(label1)
    Me.Controls.Add(label2)

    thread1 = New Thread(New ThreadStart(AddressOf MoveLeft))
    thread1.Start()

    thread2 = New Thread(New ThreadStart(AddressOf MoveRight))
    thread2.Start()

  End Sub

  Private Sub MoveLeft()
    Dim counter As Integer = 0
    Dim max As Integer = newsItems.Length

    While (True)
      ' get news headline
      Dim headline As String = newsItems(counter)
      counter += 1
      If counter = max Then
        counter = 0
      End If
      Dim i As Integer
      For i = 0 To headline.Length
        label1.Text = headline.Substring(0, i)
        Thread.Sleep(60)
      Next
      Thread.Sleep(100)
    End While
  End Sub

  Private Sub MoveRight()
    Dim counter As Integer = 0
    Dim max As Integer = businessItems.Length

    While (True)
      ' get news headline
      Dim headline As String = businessItems(counter)
      counter += 1
      If counter = max Then
        counter = 0
      End If
      Dim i As Integer
      For i = 0 To headline.Length
        label2.Text = headline.Substring(0, i)
        Thread.Sleep(100)
      Next

      Thread.Sleep(100)
    End While
  End Sub

  Protected Overrides Sub OnClosed(ByVal e As EventArgs)
    thread1.Join(0)
    thread2.Join(0)
    Environment.Exit(0)
  End Sub

  <STAThread()> Shared Sub Main()
    Application.Run(New Form1())
  End Sub

End Class
