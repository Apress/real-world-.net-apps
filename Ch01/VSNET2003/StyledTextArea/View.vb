Option Strict On
Option Explicit On 

Imports System.Threading

Public Class View : Inherits UserControl

    Public controller As StyledTextArea
    Public lineSpace As Integer = 2  ' number of pixels between 2 lines
    Public fontFace As String = "Courier New"
    Public characterWidth As Integer = 8
    Public model As Model
    Private caretThread As Thread
    Private caretVisible As Boolean = True
    Private penWidth As Integer = 2
    Public highlightBackColor As Color = Color.DarkBlue
    Public highlightForeColor As Color = Color.White
    Public LeftInvisibleCharCount As Integer
    Private pen As New Pen(Color.Black, penWidth)

    Public Property CaretColor() As Color
        Get
            Return pen.Color
        End Get
        Set(ByVal Value As Color)
            pen.Color = Value
        End Set
    End Property

    Public Sub New(ByRef Model As Model)
        Me.model = Model
        fontHeight = 10
        Font = New Font(fontFace, fontHeight)

        caretThread = New Thread(New ThreadStart(AddressOf DisplayCaret))
        caretThread.Start()

    End Sub

    Public Function GetFontHeight() As Integer
        Return fontHeight
    End Function

    Public ReadOnly Property VisibleCharCount() As Integer
        Get
            Return CInt(Math.Floor(Me.Width / characterWidth)) - 1
        End Get
    End Property

    Public ReadOnly Property VisibleLineCount() As Integer
        Get
            Return CInt(Me.Height / (lineSpace + GetFontHeight()))
        End Get
    End Property

    Public Sub Scroll(ByVal increment As Integer)
        controller.TopInvisibleLineCount = _
          controller.TopInvisibleLineCount + increment
        If controller.TopInvisibleLineCount < 0 Then
            controller.TopInvisibleLineCount = 0
        End If
        RedrawAll()
    End Sub

    Public Sub MoveScreen(ByVal increment As Integer)
        'move screen horizontally by x character
        LeftInvisibleCharCount = Math.Max(LeftInvisibleCharCount + increment, 0)
        RedrawAll()
    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        caretThread.Abort()
        caretThread.Join()
        MyBase.Dispose(disposing)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

        Dim graphics As Graphics = e.Graphics
        Dim textBrush As SolidBrush = New SolidBrush(ForeColor)
        Dim highlightTextBrush As SolidBrush = New SolidBrush(highlightForeColor)

        If controller.HasSelection() Then
            PaintSelectionArea(graphics)
        End If

        Dim i, visibleLine As Integer
        Dim maxCharCount As Integer
        For i = 1 To model.LineCount
            Dim thisLine As String = model.GetLine(i)
            If i > controller.TopInvisibleLineCount Then
                Dim y As Integer = visibleLine * (lineSpace + fontHeight)
                Dim j As Integer
                For j = 1 To thisLine.Length
                    If j > LeftInvisibleCharCount And _
                      j <= LeftInvisibleCharCount + VisibleCharCount + 1 Then
                        Dim x As Integer = GetStringWidth(thisLine.Substring(0, j - 1)) - _
                          LeftInvisibleCharCount * characterWidth
                        If controller.IsInSelection(j, i) Then
                            graphics.DrawString(thisLine.Substring(j - 1, 1), Font, _
                              highlightTextBrush, x, y)
                        Else
                            graphics.DrawString(thisLine.Substring(j - 1, 1), Font, _
                              textBrush, x, y)
                        End If
                    End If
                Next j
                visibleLine = visibleLine + 1
            End If
        Next i

        'draw caret here
        DrawCaret(graphics)
    End Sub

    Protected Sub DrawCaret(ByRef graphics As Graphics)
        'it's protected so that it can be overriden by subclass
        If caretVisible And Me.Focused Then
            'Measure string
            Dim caretsLine As String = model.GetLine(controller.CaretLineNo)
            Dim x As Integer = GetStringWidth(caretsLine.Substring(0, _
              controller.CaretColumnNo - 1)) + _
              penWidth - (LeftInvisibleCharCount * characterWidth)
            Dim y As Integer = (controller.CaretLineNo - 1 - _
              controller.TopInvisibleLineCount) * (lineSpace + fontHeight)
            graphics.DrawLine(pen, x, y, x, y + lineSpace + fontHeight)
        End If
    End Sub

    Private Sub PaintSelectionArea(ByRef graphics As Graphics)
        Dim brush As New SolidBrush(highlightBackColor)
        ' representing start and end coordinates of selected text
        Dim x1, y1, x2, y2 As Integer
        Dim p1, p2 As ColumnLine
        p1 = controller.selectionStartLocation
        p2 = controller.selectionEndLocation
        x1 = p1.Column : y1 = p1.Line
        x2 = p2.Column : y2 = p2.Line

        If y1 > y2 Or (y1 = y2 And x1 > x2) Then
            'swap
            Dim t As Integer
            t = y1 : y1 = y2 : y2 = t
            t = x1 : x1 = x2 : x2 = t
        End If

        Dim i As Integer
        Dim beginLine As Integer = Math.Max(y1, 1)
        Dim endLine As Integer = Math.Min(y2, model.LineCount)

        If beginLine = endLine Then
            If x1 > x2 Then
                Dim t As Integer
                t = x1 : x1 = x2 : x2 = t
            End If
            Dim thisLine As String = model.GetLine(beginLine)
            graphics.FillRectangle(brush, _
              2 + GetStringWidth(thisLine.Substring(0, x1 - 1)) - _
              (LeftInvisibleCharCount * characterWidth), _
              (beginLine - 1 - controller.TopInvisibleLineCount) * _
              (lineSpace + fontHeight), _
              GetStringWidth(thisLine.Substring(x1 - 1, x2 - x1)), _
              (lineSpace + fontHeight))
        Else
            For i = beginLine To endLine
                Dim thisLine As String = model.GetLine(i)
                If i = beginLine Then
                    ' first line may not be the whole line, 
                    ' but from initial position of selection to end of string
                    graphics.FillRectangle(brush, _
                      2 + GetStringWidth(thisLine.Substring(0, x1 - 1)) - _
                      LeftInvisibleCharCount * characterWidth, _
                      (i - 1 - controller.TopInvisibleLineCount) * _
                      (lineSpace + fontHeight), GetStringWidth(thisLine) - _
                      GetStringWidth(thisLine.Substring(0, x1 - 1)), _
                      (lineSpace + fontHeight))

                ElseIf i = endLine Then
                    graphics.FillRectangle(brush, _
                      2 - LeftInvisibleCharCount * characterWidth, _
                      (i - 1 - controller.TopInvisibleLineCount) * _
                      (lineSpace + fontHeight), _
                      GetStringWidth(thisLine.Substring(0, x2 - 1)), _
                      (lineSpace + fontHeight))
                Else
                    ' last line may not be the whole line, 
                    ' but from first column to initial position of selection
                    graphics.FillRectangle(brush, _
                      2 - LeftInvisibleCharCount * characterWidth, _
                      (i - 1 - controller.TopInvisibleLineCount) * _
                      (lineSpace + fontHeight), GetStringWidth(thisLine), _
                      (lineSpace + fontHeight))
                End If
            Next i
        End If
        'don't dispose graphics!!    
    End Sub

    Private Function GetStringWidth(ByRef s As String) As Integer
        If Not s Is Nothing Then
            Return s.Length * characterWidth
        Else
            Return 0
        End If
    End Function


    Public Sub RedrawAll()
        'before redraw correct invisible line count
        controller.TopInvisibleLineCount = _
          Math.Min(controller.TopInvisibleLineCount, _
          model.LineCount - VisibleLineCount)
        If controller.TopInvisibleLineCount < 0 Then
            controller.TopInvisibleLineCount = 0
        End If
        LeftInvisibleCharCount = Math.Min(LeftInvisibleCharCount, _
          model.LongestLineCharCount - VisibleCharCount)
        If LeftInvisibleCharCount < 0 Then LeftInvisibleCharCount = 0
        Me.Invalidate()
        Me.Update()
    End Sub

    Private Sub DisplayCaret()
        Try
            While True
                ' call DrawCaret here
                Dim caretsLine As String = model.GetLine(controller.CaretLineNo)
                Dim x As Integer = GetStringWidth(caretsLine.Substring(0, _
                  controller.CaretColumnNo - 1)) + _
                  penWidth - (LeftInvisibleCharCount * characterWidth)
                Dim y As Integer = (controller.CaretLineNo - 1 - _
                  controller.TopInvisibleLineCount) * (lineSpace + fontHeight)
                Dim caretRectangle As New Rectangle( _
                  x - penWidth, y, 2 * penWidth, lineSpace + fontheight)

                Me.Invalidate(caretRectangle)
                Me.Update()
                If Not caretVisible Then
                    Thread.Sleep(150)
                Else
                    Thread.Sleep(350)
                End If
                caretVisible = Not caretVisible
            End While
        Catch
        End Try
    End Sub

    Public Function TranslateIntoCaretLocation( _
      ByVal x1 As Integer, ByVal y1 As Integer) As ColumnLine
        Dim column, line As Integer ' the coordinate for the returned Point

        'set lowest value for y1 in case the use keeps dragging above the control
        If y1 < 1 Then
            y1 = 1
        End If

        'Get the visible line number
        line = Math.Min(1 + controller.TopInvisibleLineCount + _
          CInt(y1 / (fontHeight + lineSpace)), model.LineCount)
        'Now calculate the closest position of the character in the current line
        Dim thisLine As String = model.GetLine(line)
        Dim i As Integer, minDistance As Single = Me.Width 'the width of this control
        Dim j As Integer = 0
        For i = 0 To thisLine.Length
            Dim distance As Single = _
              Math.Abs(x1 + LeftInvisibleCharCount * characterWidth - _
              GetStringWidth(thisLine.Substring(0, i)))
            If distance < minDistance Then
                minDistance = distance
                j = i
            End If
        Next i
        column = j + 1
        Return New ColumnLine(column, line)
    End Function

    Public Sub RepositionCaret(ByVal x As Integer, ByVal y As Integer)
        'Get the (visible) line number
        Dim lineNumber As Integer = 1 + CInt(y / (fontHeight + lineSpace))
        controller.CaretLineNo = Math.Min(lineNumber + _
          controller.TopInvisibleLineCount, model.LineCount)
        'Now calculate the closest position of the character in the current line
        Dim thisLine As String = controller.GetCurrentLine()
        Dim i As Integer, minDistance As Single = Width ' the width of this control
        Dim j As Integer = 0

        For i = 0 To thisLine.Length
            Dim distance As Integer = _
              Math.Abs(x + LeftInvisibleCharCount * characterWidth - _
              GetStringWidth(thisLine.Substring(0, i)))
            If distance < minDistance Then
                minDistance = distance
                j = i
            End If
        Next i
        controller.CaretColumnNo = j + 1
        RedrawAll()
    End Sub

    Public Function GetCaretXAbsolutePosition() As Integer
        Dim caretsLine As String = controller.GetCurrentLine()
        If Not caretsLine Is Nothing Then
            Return _
              GetStringWidth(caretsLine.Substring(0, controller.CaretColumnNo - 1))
        Else
            Return 0
        End If
    End Function

    Public Function IsCaretVisible() As Boolean
        Dim xPosition As Integer = GetCaretXAbsolutePosition()
        Dim leftInvisibleWidth As Integer = LeftInvisibleCharCount * characterWidth
        If xPosition < leftInvisibleWidth Or _
          xPosition > leftInvisibleWidth + Me.Width - 5 Or _
          controller.CaretLineNo > _
          controller.TopInvisibleLineCount + VisibleLineCount Then
            Return False
        Else
            Return True
        End If
    End Function
End Class

