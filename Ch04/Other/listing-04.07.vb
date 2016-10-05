Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class Form1 : Inherits Form
  Private drawArea As New DrawArea()

  Public Sub New()
    Me.ClientSize = New System.Drawing.Size(740, 585)
    drawArea.Dock = DockStyle.Fill
    Me.Controls.Add(drawArea)
    drawArea.BackColor = Color.White
  End Sub

  <STAThread()> Shared Sub Main()
    Dim f As New Form1()
    Application.Run(f)
  End Sub

End Class
