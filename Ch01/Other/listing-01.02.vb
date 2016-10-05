Imports System
Public Class MyForm : Inherits System.Windows.Forms.Form
  Private myButton As New RoundButton()
  Public Sub New()
    myButton.Location = New System.Drawing.Point(24, 24)
    myButton.Size = New System.Drawing.Size(100, 100)
    myButton.Text = "Show Magic"
    Me.Controls.Add(myButton)
    Me.Text = "Demonstrating Round Button"
  End Sub

  <STAThread()> Shared Sub Main()
    System.Windows.Forms.Application.Run(New MyForm())
  End Sub

End Class
