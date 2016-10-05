Imports System.Windows.Forms
Imports System.Drawing

Public Class RoundButton : Inherits UserControl
  Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    Dim graphics As Graphics = e.Graphics
    Dim pen As New Pen(Color.BlueViolet)
    Dim area As New Rectangle(0, 0, 90, 90)
    graphics.DrawEllipse(pen, area)
    ' draw the value of the Text property
    Dim font As New Font("Times New Roman", 10)
    Dim brush As Brush = New SolidBrush(Color.Black)
    graphics.DrawString(Me.Text, font, brush, 10, 35)
  End Sub
End Class
