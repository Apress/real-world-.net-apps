Option Explicit On 
Option Strict On

Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Collections
Imports Microsoft.VisualBasic

Public Class Line
  Private fromRect, toRect As Rect
  Public Event Removed As EventHandler
  Public HandleColor As Color = Color.Red
  Public HandleWidth As Integer = 6
  Public leftText As String
  Public rightText As String
  Public centerText As String
  Public index As Integer
  Public lineType As ShapeType

  Private pen As pen
  Private arrowHeadGraphicPath As GraphicsPath
  Private font As New font("Times New Roman", 10)
  Private textBrush As New SolidBrush(Color.Black)
  Private fromHandleRectangle, toHandleRectangle As Rectangle

  Public ForeColor As Color = Color.Black
  Public Selected As Boolean
  Private startPointField, endPointField As Point
  'whether line moves when one of the rect's is moved or resized
  Private autoMove As Boolean = True
  'relative position when autoMove = False
  Private fromRectXRelPos, fromRectYRelPos, _
    toRectXRelPos, toRectYRelPos As Integer
  'which side the points are when autoMove = False
  Private fromRectWhichSide, toRectWhichSide As whichSide
  Private Enum whichSide As Integer
    Top = 0
    Bottom = 1
    Left = 2
    Right = 3
  End Enum

  Public Property StartPoint() As Point
    Get
      Return startPointField
    End Get
    Set(ByVal p As Point)
      startPointField = p
    End Set
  End Property

  Public Property EndPoint() As Point
    Get
      Return endPointField
    End Get
    Set(ByVal p As Point)
      endPointField = p
    End Set
  End Property

  Public Sub New(ByVal fromRect As Rect, ByVal toRect As Rect, _
    ByVal index As Integer, ByVal lineType As ShapeType)
    Me.lineType = lineType
    Me.fromRect = fromRect
    Me.toRect = toRect
    Me.index = index
    AddHandler fromRect.LocationChanged, AddressOf rect_LocationChanged
    AddHandler toRect.LocationChanged, AddressOf rect_LocationChanged
    AddHandler fromRect.HandleDestroyed, AddressOf rect_HandleDestroyed
    AddHandler toRect.HandleDestroyed, AddressOf rect_HandleDestroyed
    AddHandler fromRect.Resize, AddressOf rect_Resize
    AddHandler toRect.Resize, AddressOf rect_Resize
    ConstructPen()
    ReadjustLocation()
  End Sub


  Public Sub New(ByVal fromRect As Rect, ByVal toRect As Rect, _
    ByVal memento As LineMemento)
    Me.New(fromRect, toRect, memento.index, memento.lineType)
    'set other values
    Me.autoMove = memento.autoMove
    Me.centerText = memento.centerText
    Me.leftText = memento.leftText
    Me.rightText = memento.rightText
    Me.StartPoint = memento.StartPoint
    Me.EndPoint = memento.EndPoint
    Me.Selected = memento.selected
    Me.fromRectWhichSide = CType(memento.fromRectWhichSide, whichSide)
    Me.toRectWhichSide = CType(memento.toRectWhichSide, whichSide)
    Me.fromRectXRelPos = memento.fromRectXRelPos
    Me.fromRectYRelPos = memento.fromRectYRelPos
    Me.toRectXRelPos = memento.toRectXRelPos
    Me.toRectYRelPos = memento.toRectYRelPos

  End Sub

  Private Sub ConstructPen()
    pen = New Pen(Me.ForeColor, 1)
    Select Case Me.lineType
      Case ShapeType.Generalization
        arrowHeadGraphicPath = New GraphicsPath()
        arrowHeadGraphicPath.AddLine(New Point(-5, -10), New Point(5, -10))
        arrowHeadGraphicPath.AddLine(New Point(5, -10), New Point(0, 0))
        arrowHeadGraphicPath.AddLine(New Point(0, 0), New Point(-5, -10))
        pen.CustomEndCap = New CustomLineCap(Nothing, arrowHeadGraphicPath)
      Case ShapeType.Dependency
        pen.DashStyle = DashStyle.Custom
        pen.DashPattern = New Single() {3, 3}
        arrowHeadGraphicPath = New GraphicsPath()
        arrowHeadGraphicPath.AddLine(New Point(5, -10), New Point(0, 0))
        arrowHeadGraphicPath.AddLine(New Point(0, 0), New Point(-5, -10))
        pen.CustomEndCap = New CustomLineCap(Nothing, arrowHeadGraphicPath)
      Case ShapeType.Association
      Case ShapeType.Aggregation
        arrowHeadGraphicPath = New GraphicsPath()
        arrowHeadGraphicPath.AddLine(New Point(0, -20), New Point(-5, -10))
        arrowHeadGraphicPath.AddLine(New Point(-5, -10), New Point(0, 0))
        arrowHeadGraphicPath.AddLine(New Point(0, 0), New Point(5, -10))
        arrowHeadGraphicPath.AddLine(New Point(5, -10), New Point(0, -20))
        pen.CustomEndCap = New CustomLineCap(Nothing, arrowHeadGraphicPath)
    End Select

  End Sub

  Protected Overridable Sub OnRemoved(ByVal e As EventArgs)
    RaiseEvent Removed(Me, e)
  End Sub

  Private Sub rect_HandleDestroyed(ByVal sender As System.Object, _
    ByVal e As EventArgs)
    'remove myself
    OnRemoved(New EventArgs())
  End Sub

  Private Sub rect_LocationChanged(ByVal sender As System.Object, _
    ByVal e As EventArgs)
    ReadjustLocation()
  End Sub

  Private Sub rect_Resize(ByVal sender As System.Object, _
    ByVal e As EventArgs)
    ReadjustLocation()
  End Sub

  Private Sub ReadjustLocation()

    If autoMove Then
      'for each Rect, define four points,
      'then set StartPoint and EndPoint based
      'on the relative positions of the two Rect objects

      Dim frRightMiddlePoint, frLeftMiddlePoint, _
        frTopMiddlePoint, frBottomMiddlePoint As Point
      Dim toRightMiddlePoint, toLeftMiddlePoint, _
        toTopMiddlePoint, toBottomMiddlePoint As Point

      frRightMiddlePoint.X = fromRect.Left + fromRect.Width
      frRightMiddlePoint.Y = CInt(fromRect.Top + fromRect.Height / 2)
      frLeftMiddlePoint.X = fromRect.Left
      frLeftMiddlePoint.Y = CInt(fromRect.Top + fromRect.Height / 2)
      frTopMiddlePoint.X = CInt(fromRect.Left + fromRect.Width / 2)
      frTopMiddlePoint.Y = fromRect.Top
      frBottomMiddlePoint.X = CInt(fromRect.Left + fromRect.Width / 2)
      frBottomMiddlePoint.Y = fromRect.Top + fromRect.Height

      toRightMiddlePoint.X = toRect.Left + toRect.Width
      toRightMiddlePoint.Y = CInt(toRect.Top + toRect.Height / 2)
      toLeftMiddlePoint.X = toRect.Left
      toLeftMiddlePoint.Y = CInt(toRect.Top + toRect.Height / 2)
      toTopMiddlePoint.X = CInt(toRect.Left + toRect.Width / 2)
      toTopMiddlePoint.Y = toRect.Top
      toBottomMiddlePoint.X = CInt(toRect.Left + toRect.Width / 2)
      toBottomMiddlePoint.Y = toRect.Top + toRect.Height

      If toBottomMiddlePoint.Y < frTopMiddlePoint.Y Then
        ' toRect above fromRect
        StartPoint = frTopMiddlePoint
        EndPoint = toBottomMiddlePoint
        fromRectWhichSide = whichSide.Top
        toRectWhichSide = whichSide.Bottom
        fromRectXRelPos = StartPoint.X - fromRect.Left
        toRectXRelPos = EndPoint.X - toRect.Left
      ElseIf frBottomMiddlePoint.Y < toTopMiddlePoint.Y Then
        StartPoint = frBottomMiddlePoint
        EndPoint = toTopMiddlePoint
        fromRectWhichSide = whichSide.Bottom
        toRectWhichSide = whichSide.Top
        fromRectXRelPos = StartPoint.X - fromRect.Left
        toRectXRelPos = EndPoint.X - toRect.Left
      Else
        If fromRect.Left > toRect.Left + toRect.Width Then
          StartPoint = frLeftMiddlePoint
          EndPoint = toRightMiddlePoint
          fromRectWhichSide = whichSide.Left
          toRectWhichSide = whichSide.Right
          fromRectYRelPos = StartPoint.Y - fromRect.Top
          toRectYRelPos = EndPoint.Y - toRect.Top
        Else
          StartPoint = frRightMiddlePoint
          EndPoint = toLeftMiddlePoint
          fromRectWhichSide = whichSide.Right
          toRectWhichSide = whichSide.Left
          fromRectYRelPos = StartPoint.Y - fromRect.Top
          toRectYRelPos = EndPoint.Y - toRect.Top
        End If
      End If
    Else
      'manual move based on the relative position

      'adjust the StartPoint
      Select Case fromRectWhichSide
        Case whichSide.Bottom
          startPointField.Y = fromRect.Top + fromRect.Height
          If fromRectXRelPos <= fromRect.Width Then
            startPointField.X = fromRect.Left + fromRectXRelPos
          Else
            startPointField.X = fromRect.Left + fromRect.Width
          End If
        Case whichSide.Top
          startPointField.Y = fromRect.Top
          If fromRectXRelPos <= fromRect.Width Then
            startPointField.X = fromRect.Left + fromRectXRelPos
          Else
            startPointField.X = fromRect.Left + fromRect.Width
          End If
        Case whichSide.Left
          startPointField.X = fromRect.Left
          If fromRectYRelPos <= fromRect.Height Then
            startPointField.Y = fromRect.Top + fromRectYRelPos
          Else
            startPointField.Y = fromRect.Top + fromRect.Height
          End If
        Case whichSide.Right
          startPointField.X = fromRect.Left + fromRect.Width
          If fromRectYRelPos <= fromRect.Height Then
            startPointField.Y = fromRect.Top + fromRectYRelPos
          Else
            startPointField.Y = fromRect.Top + fromRect.Height
          End If
      End Select

      'adjust the EndPoint
      Select Case toRectWhichSide
        Case whichSide.Bottom
          endPointField.Y = toRect.Top + toRect.Height
          If toRectXRelPos <= toRect.Width Then
            endPointField.X = toRect.Left + toRectXRelPos
          Else
            endPointField.X = toRect.Left + toRect.Width
          End If
        Case whichSide.Top
          endPointField.Y = toRect.Top
          If toRectXRelPos <= toRect.Width Then
            endPointField.X = toRect.Left + toRectXRelPos
          Else
            endPointField.X = toRect.Left + toRect.Width
          End If
        Case whichSide.Left
          endPointField.X = toRect.Left
          If toRectYRelPos <= toRect.Height Then
            endPointField.Y = toRect.Top + toRectYRelPos
          Else
            endPointField.Y = toRect.Top + toRect.Height
          End If
        Case whichSide.Right
          endPointField.X = toRect.Left + toRect.Width
          If toRectYRelPos <= toRect.Height Then
            endPointField.Y = toRect.Top + toRectYRelPos
          Else
            endPointField.Y = toRect.Top + toRect.Height
          End If
      End Select

    End If
  End Sub

  Private Function CanHandleMoveHorizontal(ByVal whichHandle As LineHandle) _
    As Boolean
    If whichHandle = LineHandle.None Then
      Return False
    End If

    Dim rect As Rect
    Dim point As Point
    If whichHandle = LineHandle.FromHandle Then
      rect = fromRect
      point = StartPoint
    ElseIf whichHandle = LineHandle.ToHandle Then
      rect = toRect
      point = EndPoint
    End If

    If point.Y = rect.Top Or point.Y = rect.Top + rect.Height Then
      Return True
    Else
      Return False
    End If
  End Function

  Private Function CanHandleMoveVertical(ByVal whichHandle As LineHandle) _
    As Boolean
    If whichHandle = LineHandle.None Then
      Return False
    End If

    Dim rect As Rect
    Dim point As Point
    If whichHandle = LineHandle.FromHandle Then
      rect = fromRect
      point = StartPoint
    ElseIf whichHandle = LineHandle.ToHandle Then
      rect = toRect
      point = EndPoint
    End If

    If point.X = rect.Left Or point.X = rect.Left + rect.Width Then
      Return True
    Else
      Return False
    End If
  End Function

  Public Overridable Sub Draw(ByRef g As Graphics)
    '    g.SmoothingMode = SmoothingMode.AntiAlias
    g.DrawLine(pen, StartPoint, EndPoint)

    If lineType = ShapeType.Aggregation Or _
      lineType = ShapeType.Generalization Then


      'create a new GraphicsPath so the following transform won't affect the
      'arrow head
      Dim gp As GraphicsPath = CType(arrowHeadGraphicPath.Clone, GraphicsPath)
      Dim rotAngle As Single = GetRotationAngle()

      g.TranslateTransform(EndPoint.X, EndPoint.Y)
      g.RotateTransform(rotAngle + 270)
      g.FillPath(Brushes.White, gp)
      'FillPath will erase some of the graphics path
      'redraw the arrow head
      g.DrawPath(Pens.Black, gp)
      g.ResetTransform()
    End If

    'draw left text
    g.DrawString(leftText, font, textBrush, StartPoint.X, StartPoint.Y)

    'draw center text
    Dim halfStringWidth As Integer = _
      CInt(g.MeasureString(centerText, font).Width / 2)
    Dim midX As Integer = StartPoint.X + CInt((EndPoint.X - StartPoint.X) / 2) _
      - halfStringWidth
    Dim midY As Integer = StartPoint.Y + CInt((EndPoint.Y - StartPoint.Y) / 2)
    g.DrawString(centerText, font, textBrush, midX, midY)

    'draw right text
    Dim stringWidth As Integer = CInt(g.MeasureString(rightText, font).Width)
    g.DrawString(rightText, font, textBrush, _
      EndPoint.X - stringWidth, EndPoint.Y)

    If Selected Then
      DrawHandle(g)
    End If
  End Sub

  Private Function GetRotationAngle() As Single
    'returns the angle btw the origin and the line passing StartPoint and EndPoint
    Dim x1 As Integer = StartPoint.X
    Dim y1 As Integer = StartPoint.Y
    Dim x2 As Integer = EndPoint.X
    Dim y2 As Integer = EndPoint.Y
    Dim dx As Integer = x2 - x1
    Dim dy As Integer = y2 - y1

    Dim quadrant As Integer 'value is either 1, 2, 3, or 4
    Dim angle As Double

    Try
      angle = Math.Atan(Math.Abs(dy / dx)) 'this is in radians
      'convert to degree
      angle = angle * 180 / Math.PI
    Catch
    End Try

    If dx >= 0 And dy >= 0 Then
      quadrant = 1
    ElseIf dx < 0 And dy >= 0 Then
      quadrant = 2
      angle = 180 - angle
    ElseIf dx < 0 And dy < 0 Then
      quadrant = 3
      angle = 180 + angle
    Else
      quadrant = 4
      angle = 360 - angle
    End If

    Return CSng(angle)
  End Function

  Private Sub DrawHandle(ByRef g As Graphics)
    '(x1, y1) the top-left corner of the handle on the start point
    '(x2, y2) the top-left corner of the handle on the end point
    Dim x1, y1, x2, y2 As Integer

    If StartPoint.X < EndPoint.X Then
      x1 = StartPoint.X
      x2 = EndPoint.X - HandleWidth
    Else
      x1 = StartPoint.X - HandleWidth
      x2 = EndPoint.X
    End If
    If StartPoint.Y < EndPoint.Y Then
      y1 = StartPoint.Y
      y2 = EndPoint.Y - HandleWidth
    Else
      y1 = StartPoint.Y - HandleWidth
      y2 = EndPoint.Y
    End If
    fromHandleRectangle = New Rectangle(x1, y1, HandleWidth, HandleWidth)
    toHandleRectangle = New Rectangle(x2, y2, HandleWidth, HandleWidth)
    g.FillRectangle(New SolidBrush(HandleColor), fromHandleRectangle)
    g.FillRectangle(New SolidBrush(HandleColor), toHandleRectangle)
  End Sub

  Public Function IsOverHandle(ByVal p As Point) As LineHandle
    Dim x As Integer = p.X
    Dim y As Integer = p.Y

    If x >= fromHandleRectangle.X AndAlso _
      x <= fromHandleRectangle.X + HandleWidth AndAlso _
      y >= fromHandleRectangle.Y AndAlso _
      y <= fromHandleRectangle.Y + HandleWidth Then
      Return LineHandle.FromHandle
    End If
    If x >= toHandleRectangle.X AndAlso _
      x <= toHandleRectangle.X + HandleWidth AndAlso _
      y >= toHandleRectangle.Y AndAlso _
      y <= toHandleRectangle.Y + HandleWidth Then
      Return LineHandle.ToHandle
    End If
    Return LineHandle.None

  End Function

  Public Function Move(ByVal whichHandle As LineHandle, _
    ByVal p1 As Point, ByVal p2 As Point) As Boolean
    ' return True if the line is moved, False otherwise

    If whichHandle = LineHandle.None Then
      Return False
    End If

    Dim dx As Integer = p2.X - p1.X
    Dim dy As Integer = p2.Y - p1.Y

    Dim rect As Rect
    If whichHandle = LineHandle.FromHandle Then
      rect = fromRect
    Else
      rect = toRect
    End If

    If Math.Abs(dy) > Math.Abs(dx) Then
      'move vertical
      If CanHandleMoveVertical(whichHandle) Then
        autoMove = False
        Dim newYPos As Integer
        If whichHandle = LineHandle.FromHandle Then
          newYPos = StartPoint.Y + dy
          If newYPos <= rect.Top Then
            startPointField.Y = rect.Top
          ElseIf newYPos >= rect.Top + rect.Height Then
            startPointField.Y = rect.Top + rect.Height
          Else
            startPointField.Y = newYPos
          End If
          fromRectYRelPos = startPointField.Y - fromRect.Top
          If startPointField.X = fromRect.Left Then
            fromRectWhichSide = whichSide.Left
          Else
            fromRectWhichSide = whichSide.Right
          End If
        Else
          'to Rect
          newYPos = EndPoint.Y + dy
          If newYPos <= rect.Top Then
            endPointField.Y = rect.Top
          ElseIf newYPos >= rect.Top + rect.Height Then
            endPointField.Y = rect.Top + rect.Height
          Else
            endPointField.Y = newYPos
          End If
          toRectYRelPos = endPointField.Y - toRect.Top
          If endPointField.X = toRect.Left Then
            toRectWhichSide = whichSide.Left
          Else
            toRectWhichSide = whichSide.Right
          End If
        End If
        Return True
      Else
        Return False
      End If
    Else
      'move horizontal
      If CanHandleMoveHorizontal(whichHandle) Then
        autoMove = False
        Dim newXPos As Integer
        If whichHandle = LineHandle.FromHandle Then
          'fromRect
          newXPos = StartPoint.X + dx
          If newXPos <= rect.Left Then
            startPointField.X = rect.Left
          ElseIf newXPos >= rect.Left + rect.Width Then
            startPointField.X = rect.Left + rect.Width
          Else
            startPointField.X = newXPos
          End If
          fromRectXRelPos = startPointField.X - fromRect.Left
          If startPointField.Y = fromRect.Top Then
            fromRectWhichSide = whichSide.Top
          Else
            fromRectWhichSide = whichSide.Bottom
          End If
        Else
          'toRect
          newXPos = EndPoint.X + dx
          If newXPos <= rect.Left Then
            endPointField.X = rect.Left
          ElseIf newXPos >= rect.Left + rect.Width Then
            endPointField.X = rect.Left + rect.Width
          Else
            endPointField.X = newXPos
          End If
          toRectXRelPos = endPointField.X - toRect.Left
          If endPointField.Y = toRect.Top Then
            toRectWhichSide = whichSide.Top
          Else
            toRectWhichSide = whichSide.Bottom
          End If
        End If
        Return True
      Else
        Return False
      End If
    End If
    Return False
  End Function

  Public Function GetMemento() As LineMemento
    Dim memento As New LineMemento()
    memento.index = Me.index
    memento.lineType = Me.lineType
    memento.fromRectIndex = fromRect.index
    memento.toRectIndex = toRect.index
    memento.StartPoint = Me.StartPoint
    memento.EndPoint = Me.EndPoint
    memento.selected = Me.Selected
    memento.leftText = Me.leftText
    memento.centerText = Me.centerText
    memento.rightText = Me.rightText
    memento.autoMove = Me.autoMove
    memento.fromRectXRelPos = Me.fromRectXRelPos
    memento.fromRectYRelPos = Me.fromRectYRelPos
    memento.toRectXRelPos = Me.toRectXRelPos
    memento.toRectYRelPos = Me.toRectYRelPos
    memento.fromRectWhichSide = Me.fromRectWhichSide
    memento.toRectWhichSide = Me.toRectWhichSide
    Return memento
  End Function

End Class
