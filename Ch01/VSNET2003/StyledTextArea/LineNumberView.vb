Option Strict On
Option Explicit On 

Public Class LineNumberView : Inherits UserControl
    Private model As model
    Public controller As StyledTextArea
    Public lineSpace As Integer = 2
    Public fontFace As String = "Courier New"

    Public Sub New(ByRef model As model)
        Me.model = model
        fontHeight = 10
        Font = New Font(fontFace, fontHeight)
    End Sub

    Public ReadOnly Property VisibleLineCount() As Integer
        Get
            Return CInt(Me.Height / (lineSpace + FontHeight))
        End Get
    End Property

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

        Dim graphics As Graphics = e.Graphics
        Dim textBrush As SolidBrush = New SolidBrush(ForeColor)
        Dim characterWidth As Integer = 8

        Dim i, visibleLine As Integer
        For i = 1 To Math.Min(model.LineCount, VisibleLineCount)
            Dim number As Integer = i + controller.TopInvisibleLineCount
            Dim x As Integer = Me.Width - characterWidth - _
              (number).ToString().Length * characterWidth
            Dim y As Integer = (i - 1) * (lineSpace + fontheight)
            graphics.DrawString(number.ToString(), Font, textBrush, x, y)
        Next i

    End Sub

    Public Sub RedrawAll()
        Me.Invalidate()
        Me.Update()
    End Sub
End Class
