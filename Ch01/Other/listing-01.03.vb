Imports System
Public Class Form1 : Inherits System.Windows.Forms.Form
  Friend WithEvents myTextBox As System.Windows.Forms.TextBox
  Public Sub New()
    myTextBox = New System.Windows.Forms.TextBox()
    myTextBox.Location = New System.Drawing.Point(24, 24)
    myTextBox.Size = New System.Drawing.Size(248, 20)
    AddHandler myTextBox.TextChanged, AddressOf myTextBox_TextChanged
    Me.Controls.Add(myTextBox)
  End Sub

  Private Sub myTextBox_TextChanged(ByVal sender As System.Object, _
    ByVal e As System.EventArgs)
    Me.Text = myTextBox.Text
  End Sub

  <STAThread()> Shared Sub Main()
    System.Windows.Forms.Application.Run(New Form1())
  End Sub

End Class
