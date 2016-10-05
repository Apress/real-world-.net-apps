Option Explicit On 
Option Strict On

Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections
Imports Microsoft.VisualBasic

Public MustInherit Class Rect : Inherits UserControl
  Public StartPoint As Point
  Public EndPoint As Point
  Public selected As Boolean = True
  Private handleColor As Color = Color.Red
  Public Const handleWidth As Integer = 6
  Public minimumWidth As Integer = 50
  Public minimumHeight As Integer = 100
  Protected foregroundPen As New Pen(Color.Black)
  Private handleBrush As New SolidBrush(handleColor)
  Protected textBrush As New SolidBrush(Color.Black)
  Public Shared Shadows fontHeight As Integer = 12
  Public Shared textTopMargin As Integer = 5
  Public index As Integer

  Protected yPos As Integer
  Protected xPos As Integer = 5
  Public operations As New ArrayList()

  Public Sub New(ByVal startPoint As Point, ByVal endPoint As Point, _
    ByVal index As Integer)
    Me.StartPoint = startPoint
    Me.EndPoint = endPoint
    Me.index = index
    Dim r As Rectangle = MathUtil.GetRectangleFromPoints(startPoint, endPoint)
    Dim currentWidth As Integer = _
      CInt(IIf(r.Width > minimumWidth, r.Width, minimumWidth))
    Dim currentHeight As Integer = _
      CInt(IIf(r.Height > minimumHeight, r.Height, minimumHeight))
    Me.SetBounds(r.X, r.Y, currentWidth, currentHeight)
    Font = New Font("Times New Roman", 10)
  End Sub

  Public Sub Delete()
    ' call Dispose to remove me from parent's Controls collection
    Me.Dispose()
    Me.DestroyHandle() 'trigger the HandleDestroyed event
  End Sub

  Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    Dim g As Graphics = e.Graphics
    If selected Then
      g.DrawRectangle(foregroundPen, _
        CInt(handleWidth / 2), CInt(handleWidth / 2), Me.Width - handleWidth, _
        Me.Height - handleWidth)
      DrawHandles(g)
    Else
      g.DrawRectangle(foregroundPen, _
        0, 0, Me.Width - 1, Me.Height - 1)
    End If
    DrawName(g)
    'draw the line that partition the name part and the next part
    yPos += fontHeight
    If selected Then
      Dim x1 As Integer = CInt(handleWidth / 2)
      Dim x2 As Integer = x1 + Me.Width - handleWidth
      g.DrawLine(foregroundPen, x1, yPos, x2, yPos)
    Else
      Dim x1 As Integer = 0
      Dim x2 As Integer = x1 + Me.Width
      g.DrawLine(foregroundPen, x1, yPos, x2, yPos)
    End If
    DrawMembers(g)
  End Sub

  Private Sub DrawHandles(ByRef g As Graphics)
    'draw handles
    g.FillRectangle(handleBrush, _
      0, 0, handleWidth, handleWidth)
    g.FillRectangle(handleBrush, _
      Me.Width - handleWidth, 0, handleWidth, handleWidth)
    g.FillRectangle(handleBrush, _
      0, Me.Height - handleWidth, handleWidth, handleWidth)
    g.FillRectangle(handleBrush, _
      Me.Width - handleWidth, Me.Height - handleWidth, _
      handleWidth, handleWidth)
  End Sub

  Protected MustOverride Sub DrawName(ByRef g As Graphics)
  Protected MustOverride Sub DrawMembers(ByRef g As Graphics)

  Public Function OnWhichHandle(ByVal p As Point) As RectHandle
    If Not selected Then
      Return RectHandle.None
    End If
    Dim x As Integer = p.X
    Dim y As Integer = p.Y
    If x <= handleWidth And y <= handleWidth Then
      Return RectHandle.TopLeft
    ElseIf x >= Me.Width - handleWidth And y <= handleWidth Then
      Return RectHandle.TopRight
    ElseIf x <= handleWidth And y >= Me.Height - handleWidth Then
      Return RectHandle.BottomLeft
    ElseIf x >= Me.Width - handleWidth And y >= Me.Height - handleWidth Then
      Return RectHandle.BottomRight
    Else
      Return RectHandle.None
    End If
  End Function

  Public Sub SetTop(ByVal top As Integer)
    If top >= 0 Then
      Me.Top = top
    End If
    StartPoint.Y = Me.Top
    EndPoint.Y = StartPoint.Y + Me.Height
  End Sub

  Public Sub SetLeft(ByVal left As Integer)
    If left >= 0 Then
      Me.Left = left
    End If
    StartPoint.X = Me.Left
    EndPoint.X = StartPoint.X + Me.Width
  End Sub

  Public Sub SetWidth(ByVal width As Integer)
    If width > minimumWidth Then
      Me.Width = width
    Else
      Me.Width = minimumWidth
    End If
    EndPoint.X = StartPoint.X + Me.Width
  End Sub

  Public Sub SetHeight(ByVal height As Integer)
    If height > minimumHeight Then
      Me.Height = height
    Else
      Me.Height = minimumHeight
    End If
    EndPoint.Y = StartPoint.Y + Me.Height
  End Sub
End Class
