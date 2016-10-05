Imports System
Imports System.Windows.Forms

Public Class Form1 : Inherits Form
  Private theTimer As New System.Timers.Timer()
  Private splash As New Form()
  Private splashShown As Boolean = False

  Public Sub New()
    AddHandler theTimer.Elapsed, AddressOf theTimer_Elapsed
    theTimer.Interval = 2000
    theTimer.Start()
  End Sub

  Private Sub theTimer_Elapsed(ByVal sender As Object, _
    ByVal e As System.Timers.ElapsedEventArgs)
    If Not splashShown Then
      splash.Text = "Splash screen"
      splash.Show()
      ' show the splash screen for 5 seconds
      theTimer.Interval = 5000
      splashShown = True
    Else
      'close the splash screen
      splash.Close()
      theTimer.AutoReset = False
    End If
  End Sub

  <STAThread()> Shared Sub Main()
    Application.Run(New Form1())
  End Sub
End Class
