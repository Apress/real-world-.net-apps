Option Explicit On 
Option Strict On

Imports System
Imports System.Drawing
Imports System.Collections
Imports System.Windows.Forms
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic

Public Delegate Sub LineEventHandler(ByVal sender As Object, _
  ByVal e As LineEventArgs)

Public Class LineEventArgs : Inherits EventArgs
  Public Line As Line
End Class

Public Delegate Sub RectEventHandler(ByVal sender As Object, _
  ByVal e As RectEventArgs)

Public Class RectEventArgs : Inherits EventArgs
  Public rect As rect
End Class

Public Class DrawArea : Inherits Panel

  Private resizing As Boolean ' user clicks on the handle of active Rect
  Private movingLine As Boolean ' user clicks on handle of the active line
  Private startPoint As Point
  Private movingEndPoint As Point
  Private inputBox As New inputBox()
  Private lineList As New ArrayList()
  Private whichRectHandle As RectHandle = RectHandle.None
  Private lineHandleDragged As LineHandle = LineHandle.None
  Public edited As Boolean 'whether or not there's been change
  Public index As Integer 'sequential index number for create Rect or Line objects

  Public Event LineSelected As LineEventHandler
  Public Event RectSelected As RectEventHandler

  Public Sub New()
    AddHandler Me.MouseDown, AddressOf me_MouseDown
    AddHandler Me.MouseMove, AddressOf me_MouseMove
    AddHandler Me.MouseUp, AddressOf me_MouseUp
    Me.CreateGraphics().SmoothingMode() = SmoothingMode.AntiAlias

    AddHandler inputBox.LostFocus, AddressOf inputBox_LostFocus

    inputBox.BorderStyle = BorderStyle.FixedSingle
    inputBox.BackColor = Color.FromArgb(240, 240, 240)
    inputBox.Visible = False
    Me.Controls.Add(inputBox)

  End Sub

  Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    DrawShapes()
  End Sub

  Protected Overridable Sub OnLineSelected(ByVal e As LineEventArgs)
    ' called from rect_MouseUp and me_MouseDown
    edited = True
    RaiseEvent LineSelected(Me, e)
  End Sub

  Protected Overridable Sub OnRectSelected(ByVal e As RectEventArgs)
    edited = True
    RaiseEvent RectSelected(Me, e)
  End Sub

  Private Sub HideInputBox()
    inputBox.Visible = False
  End Sub

  Public Function GetRectMementos() As ArrayList
    Dim arrayList As New ArrayList()
    'bec. the first member is InputBox
    Dim rectCount As Integer = Me.Controls.Count
    Dim i As Integer
    For i = 1 To rectCount - 1
      Dim ctl As Control = Me.Controls(i)
      If TypeName(ctl) = "ClassRect" Then
        arrayList.Add(CType(ctl, ClassRect).GetMemento())
      ElseIf TypeName(ctl) = "InterfaceRect" Then
        arrayList.Add(CType(ctl, InterfaceRect).GetMemento())
      End If
    Next
    Return arrayList
  End Function

  Public Function GetLineMementos() As ArrayList
    Dim arrayList As New ArrayList()
    Dim lineEnum As IEnumerator = lineList.GetEnumerator
    While lineEnum.MoveNext
      Dim line As Line = CType(lineEnum.Current, Line)
      Dim memento As LineMemento = line.GetMemento()
      arrayList.Add(memento)
    End While
    Return arrayList
  End Function

  Protected Overrides Function ProcessDialogKey(ByVal keyData As Keys) _
    As Boolean
    ' We could have used OnKeyDown, but according to the .NET Framework 1.0
    ' documentation, OnKeyDown should not be used from code
    If inputBox.Visible Then
      Return False
    End If

    If keyData = Keys.Delete Then
      ' look for the active line, if any, and remove it from ArrayList
      Dim lineEnum As IEnumerator = LineList.GetEnumerator
      While lineEnum.MoveNext
        Dim line As Line = CType(lineEnum.Current, Line)
        If line.Selected Then
          LineList.Remove(line)
          ClearDrawArea()
          Me.DrawShapes()
          edited = True
          Return True
        End If
      End While

      'check rect now
      Dim ctl As Control
      For Each ctl In Me.Controls
        If Not ctl Is inputBox Then
          Dim rect As Rect = CType(ctl, Rect)
          If rect.selected Then
            rect.Delete()
            edited = True
            Exit For
          End If
        End If
      Next
    End If
    'return false so the key will be processed by the active component
    Return False
  End Function

  Private Sub inputBox_LostFocus(ByVal sender As System.Object, _
    ByVal e As EventArgs)
    Dim rect As Rect = inputBox.rect
    If States.RectPart = RectPart.Name Then
      rect.Name = inputBox.Text.Trim()
    ElseIf States.RectPart = RectPart.Operations Then
      rect.operations = inputBox.LineArrayList
    Else
      CType(rect, ClassRect).attributes = inputBox.LineArrayList
    End If
    rect.Invalidate()
    rect.Update()
    edited = True
    inputBox.Visible = False
  End Sub

  Private Sub rect_MouseDown(ByVal sender As System.Object, _
    ByVal e As MouseEventArgs)

    Dim rect As Rect = CType(sender, Rect)
    If Not rect.selected Then
      DeselectAllShapes()
      rect.selected = True
      rect.Invalidate()
      rect.Update()
    End If

    Dim rea As New RectEventArgs()
    rea.rect = rect
    OnRectSelected(rea)

    If e.Button = MouseButtons.Left Then
      whichRectHandle = rect.OnWhichHandle(New Point(e.X, e.Y))
      If whichRectHandle <> RectHandle.None Then
        resizing = True
        Select Case whichRectHandle
          Case RectHandle.TopLeft
            rect.Cursor = Cursors.SizeNWSE
          Case RectHandle.TopRight
            rect.Cursor = Cursors.SizeNESW
          Case RectHandle.BottomLeft
            rect.Cursor = Cursors.SizeNESW
          Case RectHandle.BottomRight
            rect.Cursor = Cursors.SizeNWSE
          Case RectHandle.None
            rect.Cursor = Cursors.Arrow
        End Select

      End If
      startPoint = New Point(e.X + rect.Left, e.Y + rect.Top)
      movingEndPoint = startPoint
    ElseIf e.Button = MouseButtons.Right Then
      UpdateRect(e.X, e.Y, rect)
    End If
  End Sub

  Private Sub UpdateRect(ByVal x As Integer, ByVal y As Integer, _
    ByRef rect As Rect)
    inputBox.rect = rect
    ' (x, y) is the click point
    ' now check which part of the rect was clicked (i.e. name, attributes or
    ' operations
    If TypeName(rect) = "ClassRect" Then
      Dim classRect As ClassRect = CType(rect, ClassRect)
      If y <= rect.textTopMargin + 2 * rect.fontHeight Then
        'user clicked the name part
        States.RectPart = RectPart.Name
        inputBox.Multiline = False
        inputBox.TextAlign = HorizontalAlignment.Center
        inputBox.Top = rect.Top + rect.textTopMargin
        inputBox.Height = rect.fontHeight
        inputBox.Text = rect.Name

      ElseIf y <= rect.textTopMargin + _
        rect.fontHeight * (1 + 2 + classRect.AttributeCount) Then
        'user clicked the attribute part
        States.RectPart = RectPart.Attributes
        inputBox.Multiline = True
        inputBox.TextAlign = HorizontalAlignment.Left
        inputBox.Top = rect.Top + rect.textTopMargin + 2 * rect.fontHeight
        inputBox.Height = rect.Height - rect.textTopMargin - 2 * rect.fontHeight
        inputBox.LineArrayList = classRect.attributes
      Else
        'user clicked the operations part
        States.RectPart = RectPart.Operations
        inputBox.Multiline = True
        inputBox.TextAlign = HorizontalAlignment.Left
        inputBox.Top = rect.Top + rect.textTopMargin + _
          (1 + 2 + classRect.AttributeCount) * rect.fontHeight
        inputBox.Height = rect.Height - rect.textTopMargin - _
          (1 + 2 + classRect.AttributeCount) * rect.fontHeight
        inputBox.LineArrayList = rect.operations
      End If
    ElseIf TypeName(rect) = "InterfaceRect" Then
      If y <= rect.textTopMargin + 3 * rect.fontHeight Then
        'user clicked the name part
        States.RectPart = RectPart.Name
        inputBox.Multiline = False
        inputBox.TextAlign = HorizontalAlignment.Center
        inputBox.Top = rect.Top + rect.textTopMargin + _
          rect.fontHeight
        inputBox.Height = CInt(1.5 * rect.fontHeight)
        inputBox.Text = rect.Name
      Else
        'user clicked the operations part
        States.RectPart = RectPart.Operations
        inputBox.Multiline = True
        inputBox.TextAlign = HorizontalAlignment.Left
        inputBox.Top = rect.Top + rect.textTopMargin + 3 * rect.fontHeight
        inputBox.Height = rect.Height - rect.textTopMargin - 3 * rect.fontHeight
        inputBox.LineArrayList = rect.operations
      End If
    End If
    inputBox.Left = rect.Left + CInt(rect.handleWidth / 2)
    inputBox.Width = rect.Width - rect.handleWidth + 1
    inputBox.Visible = True
    inputBox.Focus()
  End Sub

  Private Sub rect_MouseMove(ByVal sender As System.Object, _
    ByVal e As MouseEventArgs)

    Dim rect As Rect = CType(sender, Rect)
    Dim whichHandle As RectHandle = rect.OnWhichHandle(New Point(e.X, e.Y))
    Select Case whichHandle
      Case RectHandle.TopLeft
        rect.Cursor = Cursors.SizeNWSE
      Case RectHandle.TopRight
        rect.Cursor = Cursors.SizeNESW
      Case RectHandle.BottomLeft
        rect.Cursor = Cursors.SizeNESW
      Case RectHandle.BottomRight
        rect.Cursor = Cursors.SizeNWSE
      Case RectHandle.None
        rect.Cursor = Cursors.Arrow
    End Select

    If e.Button = MouseButtons.Left Then
      Dim endPoint As New Point(e.X + rect.Left, e.Y + rect.Top)
      Dim graphics As Graphics = Me.CreateGraphics
      If resizing Then
        Select Case whichRectHandle
          Case RectHandle.TopLeft
            ' does not allow resizing past the minimum width 
            If movingEndPoint.X - endPoint.X + rect.Width >= _
              rect.minimumWidth Then
              Dim newLeft As Integer = rect.Left + endPoint.X - movingEndPoint.X
              rect.SetLeft(newLeft)
              If newLeft >= 0 Then
                'only change the width if the user does NOT drag past 
                'the left edge of DrawArea
                rect.SetWidth(rect.Width + movingEndPoint.X - endPoint.X)
                'update endPoint
              End If
            End If
            If movingEndPoint.Y - endPoint.Y + rect.Height > - _
              rect.minimumHeight Then
              Dim newTop As Integer = rect.Top + endPoint.Y - movingEndPoint.Y
              rect.SetTop(newTop)
              If newTop >= 0 Then
                'only change the height if the user does NOT drag past 
                'the top edge of DrawArea
                rect.SetHeight(rect.Height + movingEndPoint.Y - endPoint.Y)
              End If
            End If
          Case RectHandle.TopRight
            rect.SetWidth(rect.Width + endPoint.X - movingEndPoint.X)

            If movingEndPoint.Y - endPoint.Y + rect.Height > - _
              rect.minimumHeight Then
              Dim newTop As Integer = rect.Top + endPoint.Y - movingEndPoint.Y
              rect.SetTop(newTop)
              If newTop >= 0 Then
                'only change the height if the user does NOT drag past 
                'the top edge of DrawArea
                rect.SetHeight(rect.Height + movingEndPoint.Y - endPoint.Y)
              End If
            End If
          Case RectHandle.BottomLeft
            rect.SetHeight(rect.Height + endPoint.Y - movingEndPoint.Y)

            If movingEndPoint.X - endPoint.X + rect.Width >= _
              rect.minimumWidth Then
              Dim newLeft As Integer = rect.Left + endPoint.X - movingEndPoint.X
              rect.SetLeft(newLeft)
              If newLeft >= 0 Then
                'only change the width if the user does NOT drag past 
                'the left edge of DrawArea
                rect.SetWidth(rect.Width + movingEndPoint.X - endPoint.X)
              End If
            End If
          Case RectHandle.BottomRight
            rect.SetWidth(rect.Width + endPoint.X - movingEndPoint.X)
            rect.SetHeight(rect.Height + endPoint.Y - movingEndPoint.Y)
        End Select
        edited = True
        movingEndPoint = endPoint
        ClearDrawArea()
        rect.Refresh()
        DrawShapes()
      Else

        Select Case States.ShapeDrawn
          Case ShapeType.Class
          Case ShapeType.Interface
          Case Is = ShapeType.Aggregation, Is = ShapeType.Association, _
            Is = ShapeType.Dependency, Is = ShapeType.Generalization
            'remove the previous "temporary" line
            graphics.DrawLine(New Pen(Me.BackColor), startPoint, movingEndPoint)
            movingEndPoint = endPoint
            graphics.DrawLine(Pens.Black, startPoint, movingEndPoint)
            DrawShapes()
          Case ShapeType.None
            ' use SetLeft and SetTop so 
            ' that the Rect's internal states are updated
            rect.SetLeft(Math.Max(0, rect.Left + (endPoint.X - movingEndPoint.X)))
            rect.SetTop(Math.Max(0, rect.Top + (endPoint.Y - movingEndPoint.Y)))
            movingEndPoint = endPoint
            edited = True
            ClearDrawArea()
            DrawShapes()
        End Select
      End If
    End If
  End Sub

  Private Sub rect_MouseUp(ByVal sender As System.Object, _
    ByVal e As MouseEventArgs)
    If e.Button = MouseButtons.Left Then
      Dim rect As Rect = CType(sender, Rect)
      Dim endPoint As New Point(e.X + rect.Left, e.Y + rect.Top)
      Select Case States.ShapeDrawn
        Case ShapeType.Class
        Case ShapeType.Interface
        Case Is = ShapeType.Aggregation, Is = ShapeType.Association, _
          Is = ShapeType.Dependency, Is = ShapeType.Generalization
          'if it ends in one of the Rect
          Dim toRect As Rect = GetRectAtPoint(endPoint)
          If Not toRect Is Nothing AndAlso Not toRect.Equals(rect) Then
            Dim fromRect As Rect = CType(sender, Rect)
            Dim line As New Line(fromRect, toRect, index, States.ShapeDrawn)
            index += 1
            line.Selected = True
            AddHandler line.Removed, AddressOf line_Removed
            AddLine(line)
            edited = True
            rect.selected = False
            Dim lineEventArgs As New LineEventArgs()
            lineEventArgs.Line = line
            OnLineSelected(lineEventArgs)
            Me.Refresh()
            Me.Focus()
          End If
          Dim graphics As Graphics = Me.CreateGraphics
          graphics.DrawLine(New Pen(Me.BackColor), startPoint, movingEndPoint)
        Case ShapeType.None
      End Select
      DrawShapes()
      resizing = False
    End If
  End Sub

  Private Sub me_MouseDown(ByVal sender As System.Object, _
    ByVal e As MouseEventArgs)
    If e.Button = MouseButtons.Left Then
      HideInputBox()
      DeselectAllShapes()
      'drawing = True
      startPoint = New Point(e.X, e.Y)
      movingEndPoint = startPoint
      If States.ShapeDrawn = ShapeType.None Then
        ' deselect all lines
        Dim selectedLine As Line = GetSelectedLine()
        If Not selectedLine Is Nothing Then
          edited = True
          selectedLine.Selected = False
        End If
        Dim line As Line = GetNearestLine(startPoint)
        If Not line Is Nothing Then
          line.Selected = True
          line.Draw(Me.CreateGraphics)
          Dim lineEventArgs As New LineEventArgs()
          lineEventArgs.Line = line
          edited = True
          OnLineSelected(lineEventArgs)
        End If
        lineHandleDragged = IsOverLineHandle(New Point(e.X, e.Y))
        If lineHandleDragged <> LineHandle.None Then
          movingLine = True
        End If
      End If
    End If

  End Sub

  Private Sub me_MouseMove(ByVal sender As System.Object, _
    ByVal e As MouseEventArgs)
    If e.Button = MouseButtons.Left Then
      Dim endPoint As New Point(e.X, e.Y)
      Dim graphics As Graphics = Me.CreateGraphics
      Select Case States.ShapeDrawn
        Case Is = ShapeType.Class, Is = ShapeType.Interface
          graphics.DrawRectangle(Pens.White, _
            MathUtil.GetRectangleFromPoints(startPoint, movingEndPoint))
          movingEndPoint = endPoint
          graphics.DrawRectangle(Pens.Black, _
            MathUtil.GetRectangleFromPoints(startPoint, movingEndPoint))
          DrawShapes()
        Case Is = ShapeType.Aggregation, Is = ShapeType.Association, _
          Is = ShapeType.Dependency, Is = ShapeType.Generalization
        Case ShapeType.None
          If movingLine Then
            Dim line As Line = GetActiveLine()
            If Not line Is Nothing Then
              If (line.Move(lineHandleDragged, movingEndPoint, endPoint)) Then
                ClearDrawArea()
                DrawShapes()
              End If
              movingEndPoint = endPoint
            End If
          End If
      End Select
    End If
  End Sub

  Public Sub AddRect(ByVal rect As Rect)
    AddHandler rect.GotFocus, AddressOf rect_GotFocus
    AddHandler rect.MouseDown, AddressOf rect_MouseDown
    AddHandler rect.MouseMove, AddressOf rect_MouseMove
    AddHandler rect.MouseUp, AddressOf rect_MouseUp
    Me.Controls.Add(rect)
    edited = True
  End Sub

  Public Sub AddLine(ByVal line As Line)
    lineList.Add(line)
    edited = True
  End Sub

  Private Sub me_MouseUp(ByVal sender As System.Object, _
    ByVal e As MouseEventArgs)
    If e.Button = MouseButtons.Left Then
      Dim endPoint As New Point(e.X, e.Y)
      Select Case States.ShapeDrawn
        Case Is = ShapeType.Class, Is = ShapeType.Interface
          If Not startPoint.Equals(endPoint) Then
            Dim rect As Rect
            If States.ShapeDrawn = ShapeType.Class Then
              rect = New ClassRect(startPoint, endPoint, index)
            Else
              rect = New InterfaceRect(startPoint, endPoint, index)
            End If
            index += 1
            AddRect(rect)

            Me.CreateGraphics.DrawRectangle(Pens.White, _
              MathUtil.GetRectangleFromPoints(startPoint, movingEndPoint))
            'rect.Focus()
          End If
        Case ShapeType.None
      End Select
      movingLine = False
    End If
  End Sub

  Private Sub ClearDrawArea()
    Me.CreateGraphics().Clear(Me.BackColor)
  End Sub


  Private Sub rect_GotFocus(ByVal sender As Object, ByVal e As EventArgs)
    DeselectAllShapes()
    edited = True
  End Sub

  Private Sub line_Removed(ByVal sender As Object, ByVal e As EventArgs)
    Dim obj As Object
    For Each obj In lineList
      If obj.Equals(sender) Then
        lineList.Remove(obj)
        Me.Refresh()
        edited = True
        Return
      End If
    Next
  End Sub


  Private Function GetSelectedLine() As Line
    Dim lineEnum As IEnumerator = lineList.GetEnumerator()
    While lineEnum.MoveNext
      Dim line As Line = CType(lineEnum.Current, Line)
      If line.Selected Then
        Return line
      End If
    End While
    Return Nothing
  End Function

  Private Function GetNearestLine(ByVal p As Point) As Line
    Dim lineEnum As IEnumerator = lineList.GetEnumerator()
    Dim minDistance As Double = 1000
    Dim nearestLine As Line = Nothing
    While lineEnum.MoveNext
      Dim line As Line = CType(lineEnum.Current, Line)
      Dim startPoint, endPoint As Point
      startPoint = line.StartPoint
      endPoint = line.EndPoint
      Dim distance As Double = _
        MathUtil.GetSquareDistance(startPoint, endPoint, p)
      If minDistance > distance Then
        minDistance = distance
        nearestLine = line
      End If
    End While
    If minDistance < 100 Then
      'check if the point is really in the vicinity of the line
      If Not nearestLine Is Nothing Then
        Dim clickPointX As Integer = p.X
        Dim clickPointY As Integer = p.Y
        Dim startPointX As Integer = nearestLine.StartPoint.X
        Dim startPointY As Integer = nearestLine.StartPoint.Y
        Dim endPointX As Integer = nearestLine.EndPoint.X
        Dim endPointY As Integer = nearestLine.EndPoint.Y
        Dim margin As Integer = 10

        If startPointX < endPointX Then
          If clickPointX < startPointX - margin Or _
            clickPointX > endPointX + margin Then
            Return Nothing
          End If
        Else
          If clickPointX < endPointX - margin Or _
            clickPointX > startPointX + margin Then
            Return Nothing
          End If
        End If

        If startPointY < endPointY Then
          If clickPointY < startPointY - margin Or _
            clickPointY > endPointY + margin Then
            Return Nothing
          End If
        Else
          If clickPointY < endPointY - margin Or _
            clickPointY > startPointY + margin Then
            Return Nothing
          End If
        End If

        Return nearestLine
      End If
    Else
      Return Nothing
    End If
  End Function

  Private Sub DeselectAllShapes()
    Dim lineEnum As IEnumerator = lineList.GetEnumerator()
    While lineEnum.MoveNext
      CType(lineEnum.Current, Line).Selected = False
    End While
    Dim ctl As Control
    For Each ctl In Me.Controls
      If Not ctl Is inputBox Then
        Dim rect As Rect = CType(ctl, Rect)
        If rect.selected Then
          rect.selected = False
          rect.Invalidate() 'force repaint
          rect.Update()
          Exit For
        End If
      End If
    Next
    Me.Refresh()
    Me.Focus()
  End Sub

  Private Sub DrawShapes()
    Dim graphics As Graphics = Me.CreateGraphics
    Dim lineEnum As IEnumerator = lineList.GetEnumerator()
    While lineEnum.MoveNext
      Dim line As Line = CType(lineEnum.Current, Line)
      line.Draw(graphics)
    End While

  End Sub

  Private Function GetRectAtPoint(ByVal point As Point) As Rect
    Dim ctl As Control
    Dim x As Integer = point.X
    Dim y As Integer = point.Y

    For Each ctl In Me.Controls
      If Not ctl Is inputBox AndAlso _
        x > ctl.Left AndAlso x < ctl.Left + ctl.Width _
        AndAlso y > ctl.Top AndAlso y < ctl.Top + ctl.Height Then
        Return CType(ctl, Rect)
      End If
    Next
    Return Nothing
  End Function

  Private Function IsOverLineHandle(ByVal point As Point) As LineHandle
    Dim line As Line = GetActiveLine()
    If Not line Is Nothing Then
      Return line.IsOverHandle(point)
    Else
      Return LineHandle.None
    End If
  End Function

  Private Function GetActiveLine() As Line
    Dim lineEnum As IEnumerator = lineList.GetEnumerator
    While lineEnum.MoveNext
      Dim line As Line = CType(lineEnum.Current, Line)
      If line.Selected Then
        Return line
      End If
    End While
    Return Nothing
  End Function

  Public Sub RemoveShapes()
    ' remove lines
    lineList.Clear()
    ' remove Rects
    Me.Controls.Clear()
    ' re-add InputBox
    Me.Controls.Add(inputBox)
    Me.Refresh()
  End Sub

End Class
