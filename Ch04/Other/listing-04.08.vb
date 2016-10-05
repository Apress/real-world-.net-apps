Imports System.Drawing
Imports System.Collections
Imports System.Windows.Forms

Public Class DrawArea : Inherits Panel
  Private shapes As New ArrayList()
  Private startPoint As Point
  Private movingEndPoint As Point

  Public Sub New()
    AddHandler Me.MouseDown, AddressOf me_MouseDown
    AddHandler Me.MouseMove, AddressOf me_MouseMove
    AddHandler Me.MouseUp, AddressOf me_MouseUp
  End Sub

  Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    Dim g As Graphics = e.Graphics
    Dim shapeEnum As IEnumerator = shapes.GetEnumerator
    Dim blackPen As Pen = Pens.Black
    While shapeEnum.MoveNext
      Dim rectangle As Rectangle = CType(shapeEnum.Current, Rectangle)
      g.DrawRectangle(blackPen, rectangle)
    End While
  End Sub

  Private Sub me_MouseDown(ByVal sender As System.Object, _
    ByVal e As MouseEventArgs)
    If e.Button = MouseButtons.Left Then
      startPoint = New Point(e.X, e.Y)
      movingEndPoint = startPoint
    End If

  End Sub

  Private Sub me_MouseMove(ByVal sender As System.Object, _
    ByVal e As MouseEventArgs)
    If e.Button = MouseButtons.Left Then
      Dim endPoint As New Point(e.X, e.Y)
      Dim graphics As Graphics = Me.CreateGraphics
      graphics.DrawRectangle(Pens.White, _
        GetRectangleFromPoints(startPoint, movingEndPoint))
      movingEndPoint = endPoint
      Me.Refresh()
      graphics.DrawRectangle(Pens.Black, _
        GetRectangleFromPoints(startPoint, movingEndPoint))
    End If
  End Sub

  Private Sub me_MouseUp(ByVal sender As System.Object, _
    ByVal e As MouseEventArgs)
    If e.Button = MouseButtons.Left Then
      Dim endPoint As New Point(e.X, e.Y)
      Dim r As Rectangle = GetRectangleFromPoints(startPoint, endPoint)
      shapes.Add(r)
      Me.Refresh()
    End If
  End Sub

  Public Shared Function GetRectangleFromPoints(ByVal p1 As Point, _
    ByVal p2 As Point) As Rectangle
    Dim x1, x2, y1, y2 As Integer
    If p1.X < p2.X Then
      x1 = p1.X
      x2 = p2.X
    Else
      x1 = p2.X
      x2 = p1.X
    End If

    If p1.Y < p2.Y Then
      y1 = p1.Y
      y2 = p2.Y
    Else
      y1 = p2.Y
      y2 = p1.Y
    End If
    ' x2 > x1 and y2 > y1
    Return New Rectangle(x1, y1, x2 - x1, y2 - y1)
  End Function

End Class
