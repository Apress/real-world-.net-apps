Imports System
Imports System.Windows.Forms

Public Class Form1 : Inherits Form
  Private theTimer As New Timer()
  Private splash As New Form()
  Private splashShown As Boolean = False

  Public Sub New()
    AddHandler theTimer.Tick, AddressOf theTimer_Tick
    theTimer.Interval = 2000
    theTimer.Start()
  End Sub

  Private Sub theTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
    If Not splashShown Then
      splash.Text = "Splash screen"
      splash.Show()
      ' show the splash screen for 5 seconds
      theTimer.Interval = 5000
      splashShown = True
    Else
      'close the splash screen
      splash.Close()
      theTimer.Enabled = False
    End If
  End Sub

  <STAThread()> Shared Sub Main()
    Application.Run(New Form1())
  End Sub

End Class
