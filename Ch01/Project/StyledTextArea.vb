Option Strict On
Option Explicit On 

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Collections
Imports System.Text

Public Class StyledTextArea : Inherits UserControl

    Private view As View          ' the view in the MVC pattern
    Private lineNumberView As LineNumberView
    Private model As model  ' the model in the MVC pattern
    Private selecting As Boolean  ' user selecting text
    Private panel As New Panel()  ' we use panel so we can set the borderstyle
    Private hScrollBar As New HScrollBar()
    Private vScrollBar As New VScrollBar()
    Private Const hScrollBarHeight As Integer = 15
    Private Const vScrollBarWidth As Integer = 15
    Public LineNumberWidth As Integer = 50
    Public Event ColumnChanged As ColumnEventHandler
    Public Event LineChanged As LineEventHandler


    ' indicates that the key press has been processed by ProcessDialogKey, 
    ' so KeyPressed does not have to process this.
    Private keyProcessed As Boolean
    Private caretLocation As New ColumnLine(1, 1)
    ' indicates whether the text has been edited
    Private editedField As Boolean

    Friend selectionStartLocation As New ColumnLine(0, 0)
    Friend selectionEndLocation As New ColumnLine(0, 0)
    ' represents the number of lines not displayed 
    ' because the screen was scrolled down
    Friend TopInvisibleLineCount As Integer

    Public Sub New()
        InitializeComponents()
    End Sub

    Public Property Edited() As Boolean
        Get
            Return editedField
        End Get
        Set(ByVal value As Boolean)
            'allows it to be reset/set, 
            'for example if the document using this control is saved
            editedField = value
        End Set
    End Property

    Public Property CaretColor() As Color
        Get
            Return view.CaretColor
        End Get
        Set(ByVal caretColor As Color)
            view.CaretColor = caretColor
        End Set
    End Property

    Public Property LineNumberForeColor() As Color
        Get
            Return lineNumberView.ForeColor
        End Get
        Set(ByVal color As Color)
            lineNumberView.ForeColor = color
        End Set
    End Property

    Public Property LineNumberBackColor() As Color
        Get
            Return lineNumberView.BackColor
        End Get
        Set(ByVal color As Color)
            lineNumberView.BackColor = color
        End Set
    End Property

    Public Property TextForeColor() As Color
        Get
            Return view.ForeColor
        End Get
        Set(ByVal color As Color)
            view.ForeColor = color
        End Set
    End Property

    Public Property TextBackColor() As Color
        Get
            Return view.BackColor
        End Get
        Set(ByVal color As Color)
            view.BackColor = color
        End Set
    End Property

    Public Property HighlightForeColor() As Color
        Get
            Return view.highlightForeColor
        End Get
        Set(ByVal color As Color)
            view.highlightForeColor = color
        End Set
    End Property

    Public Property HighlightBackColor() As Color
        Get
            Return view.highlightBackColor
        End Get
        Set(ByVal color As Color)
            view.highlightBackColor = color
        End Set
    End Property

    Public ReadOnly Property LineCount() As Integer
        Get
            Return model.LineCount
        End Get
    End Property

    Public Function GetLine(ByVal lineNo As Integer) As String
        Return model.GetLine(lineNo)
    End Function

    Friend Function HasSelection() As Boolean
        If selectionStartLocation.Equals(selectionEndLocation) Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub InitializeComponents()
        model = New Model()
        view = New View(model)
        view.controller = Me
        view.ForeColor = Color.Black
        view.BackColor = Color.White
        view.Cursor = Cursors.IBeam

        lineNumberView = New LineNumberView(model)
        lineNumberView.controller = Me
        lineNumberView.BackColor = Color.AntiqueWhite
        lineNumberView.ForeColor = Color.Black

        lineNumberView.TabStop = False
        vScrollBar.TabStop = False
        hScrollBar.TabStop = False

        ResizeComponents()

        panel.Dock = DockStyle.Fill
        panel.Controls.Add(lineNumberView)
        panel.Controls.Add(view)
        panel.Controls.Add(hScrollBar)
        panel.Controls.Add(vScrollBar)
        panel.BorderStyle = BorderStyle.Fixed3D


        Me.Controls.Add(panel)
        AddHandler view.KeyPress, AddressOf view_KeyPress
        AddHandler view.MouseDown, AddressOf view_MouseDown
        AddHandler view.MouseUp, AddressOf view_MouseUp
        AddHandler view.MouseMove, AddressOf view_MouseMove

        AddHandler model.LineCountChanged, AddressOf Model_LineCountChanged
        AddHandler model.LongestLineCharCountChanged, _
          AddressOf model_LongestLineCharCountChanged
        AddHandler vScrollBar.Scroll, AddressOf vScrollBar_Scroll
        AddHandler hScrollBar.Scroll, AddressOf hScrollBar_Scroll
        AddHandler panel.Resize, AddressOf panel_Resize

        hScrollBar.Enabled = False
        hScrollBar.SmallChange = 1 ' 1 character
        hScrollBar.LargeChange = 2
        vScrollBar.Enabled = False
        vScrollBar.SmallChange = 1
        vScrollBar.LargeChange = 2
    End Sub

    Friend Property CaretLineNo() As Integer
        Get
            Return caretLocation.Line
        End Get
        Set(ByVal newLineNo As Integer)
            If newLineNo > 0 And newLineNo <= model.LineCount Then
                Dim oldLineNo As Integer = caretLocation.Line
                caretLocation.Line = newLineNo
                If oldLineNo <> newLineNo Then
                    OnLineChanged(New LineEventArgs(oldLineNo, newLineNo))
                End If
            End If
        End Set
    End Property

    Friend Property CaretColumnNo() As Integer
        Get
            Return caretLocation.Column
        End Get
        Set(ByVal newColumnNo As Integer)
            If newColumnNo > 0 And newColumnNo <= GetCurrentLine().Length + 1 Then
                Dim oldColumnNo As Integer = caretLocation.Column
                caretLocation.Column = newColumnNo
                If oldColumnNo <> newColumnNo Then
                    OnColumnChanged(New ColumnEventArgs(oldColumnNo, newColumnNo))
                End If
            End If
        End Set
    End Property

    Protected Overridable Sub OnColumnChanged(ByVal e As ColumnEventArgs)
        RaiseEvent ColumnChanged(Me, e)
    End Sub

    Protected Overridable Sub OnLineChanged(ByVal e As LineEventArgs)
        RaiseEvent LineChanged(Me, e)
    End Sub

    Public Function Find(ByVal userPattern As String, _
      ByVal startColumn As Integer, ByVal startLine As Integer, _
      ByVal caseSensitive As Boolean, _
      ByVal wholeWord As Boolean, ByVal goUp As Boolean) As ColumnLine

        'make sure startColumn and startLine is greater than 0
        startColumn = Math.Max(startColumn, 1)
        Dim pattern As String = userPattern.Trim()
        Dim patternLength As Integer = pattern.Length
        Dim lineCount As Integer = model.LineCount
        Dim direction As Integer = 1
        If goUp Then direction = -1

        startLine = Math.Max(startLine, 1)
        startLine = Math.Min(startLine, lineCount)
        Dim lineNo As Integer = startLine
        If Not caseSensitive Then
            pattern = pattern.ToUpper()
        End If
        While lineNo <= lineCount And lineNo > 0
            Dim thisLine As String = model.GetLine(lineNo)

            If Not caseSensitive Then
                thisLine = thisLine.ToUpper()
            End If

            Dim searchResult As Integer = -1
            If lineNo = startLine Then
                If startColumn - 1 < thisLine.Length Then
                    If goUp Then
                        searchResult = thisLine.LastIndexOf(pattern, startColumn - 1)
                    Else
                        searchResult = thisLine.IndexOf(pattern, startColumn - 1)
                    End If
                End If
            Else
                If goUp Then
                    searchResult = thisLine.LastIndexOf(pattern)
                Else
                    searchResult = thisLine.IndexOf(pattern)
                End If
            End If

            If searchResult <> -1 And wholeWord Then
                'search successful but now test if the found pattern is a
                'whole word by checking the characters after and before
                'the match
                If searchResult > 0 Then
                    'test the character before the match
                    If Char.IsLetterOrDigit( _
                      Convert.ToChar(thisLine.Substring(searchResult - 1, 1))) Then
                        searchResult = -1
                    End If
                End If
                If searchResult <> -1 And _
                  thisLine.Length > searchResult + patternLength Then
                    'test the character after the match
                    If Char.IsLetterOrDigit( _
                      Convert.ToChar(thisLine.Substring(searchResult + _
                      patternLength, 1))) Then
                        searchResult = -1
                    End If
                End If
            End If
            If searchResult <> -1 Then    'successful
                'move caret to new position
                CaretLineNo = lineNo
                CaretColumnNo = searchResult + patternLength + 1
                Highlight(searchResult + 1, lineNo, _
                  searchResult + patternLength + 1, lineNo)
                RedrawAll()
                Return New ColumnLine(searchResult + 1, lineNo)
            End If
            lineNo = lineNo + direction
        End While
        Return New ColumnLine(0, 0)
    End Function

    Private Sub ResizeComponents()
        lineNumberView.Size = New Size(LineNumberWidth, panel.Height - _
          hScrollBarHeight - 4)
        lineNumberView.Location = New Point(0, 0)
        view.Size = New Size(panel.Width - lineNumberView.Width - _
          vScrollBarWidth - 4, panel.Height - hScrollBarHeight - 4)
        view.Location = New Point(lineNumberView.Width, 0)
        vScrollBar.Location = New Point(view.Width + lineNumberView.Width, 0)
        vScrollBar.Size = New Size(vScrollBarWidth, view.Height)
        hScrollBar.Location = New Point(0, view.Height)
        hScrollBar.Size = _
          New Size(view.Width + lineNumberView.Width, hScrollBarHeight)
        AdjustVScrollBar()
        AdjustHScrollBar()

    End Sub

    Public Sub SelectLine(ByVal lineNo As Integer)
        If lineNo <= model.LineCount Then
            Dim thisLine As String = model.GetLine(lineNo)
            Dim length As Integer = thisLine.Length
            selectionStartLocation = New ColumnLine(1, lineNo)
            selectionEndLocation = New ColumnLine(length + 1, lineNo)
            caretLocation = selectionStartLocation
            RedrawAll()
        End If
    End Sub
    Private Sub panel_Resize(ByVal sender As Object, ByVal e As EventArgs)
        ResizeComponents()
    End Sub

    Private Sub hScrollBar_Scroll(ByVal sender As Object, _
      ByVal e As ScrollEventArgs)
        Select Case e.Type
            Case ScrollEventType.SmallIncrement
                If view.LeftInvisibleCharCount < _
                  model.LongestLineCharCount - view.VisibleCharCount + 1 Then
                    view.MoveScreen(hScrollBar.SmallChange)
                End If
            Case ScrollEventType.LargeIncrement
                If view.LeftInvisibleCharCount < _
                  model.LongestLineCharCount - view.VisibleCharCount Then
                    Dim maxIncrement As Integer = _
                      Math.Min(hScrollBar.LargeChange, model.LongestLineCharCount - _
                      view.VisibleCharCount - view.LeftInvisibleCharCount)
                    view.MoveScreen(maxIncrement)
                End If
            Case ScrollEventType.SmallDecrement
                view.MoveScreen(-hScrollBar.SmallChange)
            Case ScrollEventType.LargeDecrement
                view.MoveScreen(-hScrollBar.LargeChange)
            Case ScrollEventType.ThumbTrack
                view.LeftInvisibleCharCount = e.NewValue
                RedrawAll()
            Case ScrollEventType.ThumbPosition
                view.LeftInvisibleCharCount = e.NewValue
                RedrawAll()
        End Select
    End Sub

    Private Sub vScrollBar_Scroll(ByVal sender As Object, _
      ByVal e As ScrollEventArgs)
        Select Case e.Type
            Case ScrollEventType.SmallIncrement
                If TopInvisibleLineCount < model.LineCount - view.VisibleLineCount Then
                    view.Scroll(vScrollBar.SmallChange)
                    lineNumberView.RedrawAll()
                End If
            Case ScrollEventType.LargeIncrement
                If TopInvisibleLineCount < model.LineCount - view.VisibleLineCount Then
                    Dim maxIncrement As Integer = _
                      Math.Min(vScrollBar.LargeChange, _
                        model.LineCount - view.VisibleLineCount - TopInvisibleLineCount)
                    view.Scroll(maxIncrement)
                    lineNumberView.RedrawAll()
                End If
            Case ScrollEventType.SmallDecrement
                view.Scroll(-vScrollBar.SmallChange)
                lineNumberView.RedrawAll()
            Case ScrollEventType.LargeDecrement
                view.Scroll(-vScrollBar.LargeChange)
                lineNumberView.RedrawAll()
            Case ScrollEventType.ThumbTrack
                TopInvisibleLineCount = e.NewValue
                RedrawAll()
            Case ScrollEventType.ThumbPosition
                TopInvisibleLineCount = e.NewValue
                RedrawAll()
        End Select

    End Sub

    Private Sub Model_LineCountChanged(ByVal sender As Object, _
      ByVal e As LineCountEventArgs)
        AdjustVScrollBar()
    End Sub

    Private Sub AdjustHScrollBar()
        If view.Width < model.LongestLineCharCount * view.characterWidth + 1 Then
            hScrollBar.Enabled = True
            hScrollBar.Maximum = model.LongestLineCharCount - _
                view.VisibleCharCount + 1  '10 is the margin
            hScrollBar.Value = Math.Min(hScrollBar.Maximum, _
              view.LeftInvisibleCharCount)
        Else
            hScrollBar.Enabled = False
        End If
    End Sub

    Private Sub AdjustVScrollBar()
        'adjust vScrollBar
        If view.VisibleLineCount < model.LineCount Then
            vScrollBar.Enabled = True
            vScrollBar.Maximum = model.LineCount - view.VisibleLineCount + 1
            vScrollBar.Value = Math.Min(TopInvisibleLineCount, vScrollBar.Maximum)
        Else
            vScrollBar.Enabled = False
        End If

    End Sub

    Private Sub model_LongestLineCharCountChanged(ByVal sender As Object, _
      ByVal e As LongestLineEventArgs)
        If e.LongestLineCharCount < view.VisibleCharCount Then
            view.LeftInvisibleCharCount = 0
        End If
        AdjustHScrollBar()
    End Sub

    Private Sub view_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        'move the caret
        If selecting And e.Button = MouseButtons.Left Then
            selecting = False 'reset selecting
            view.RepositionCaret(e.X, e.Y)
            RedrawAll()
        End If
    End Sub

    Private Sub view_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
        'move the caret
        If e.Button = MouseButtons.Left Then
            view.RepositionCaret(e.X, e.Y)
            selecting = True
            Dim cl As ColumnLine = view.TranslateIntoCaretLocation(e.X, e.Y)
            selectionStartLocation = cl
            selectionEndLocation = cl
            RedrawAll()
        End If
    End Sub

    Private Sub view_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
        'only respond when selecting, i.e. when the left button is pressed
        If selecting Then
            Dim cl As ColumnLine
            cl = view.TranslateIntoCaretLocation(e.X, e.Y)
            selectionEndLocation = cl
            RedrawAll()
        End If
    End Sub

    Private Sub view_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)
        If Not keyProcessed Then
            Dim c As Char = e.KeyChar
            Dim convertedChar As Integer = Convert.ToInt32(c)
            RemoveSelection()
            ResetSelection()

            Select Case convertedChar
                Case Keys.Back   'backspace 
                    If Not (IsCaretOnFirstColumn() And IsCaretOnFirstLine()) Then
                        'not at beginning of Model
                        If IsCaretOnFirstColumn() Then
                            Dim oldLine As String = model.GetLine(CaretLineNo)
                            Dim prevLine As String = model.GetLine(CaretLineNo - 1)
                            Dim newLine As String = prevLine & oldLine
                            model.SetLine(CaretLineNo - 1, newLine)
                            model.RemoveLine(CaretLineNo, True)
                            CaretLineNo = CaretLineNo - 1
                            CaretColumnNo = prevLine.Length + 1
                        Else
                            Dim oldLine As String = model.GetLine(CaretLineNo)
                            Dim newLine As String = oldLine.Remove(CaretColumnNo - 2, 1)
                            model.SetLine(CaretLineNo, newline)
                            CaretColumnNo = CaretColumnNo - 1
                        End If
                        editedField = True
                    End If
                Case Keys.Return ' return key
                    Dim oldLine As String = GetCurrentLine()
                    Dim newLine As String = oldLine.Substring(0, CaretColumnNo - 1)
                    Dim nextLine As String = oldLine.Substring(CaretColumnNo - 1)
                    model.SetLine(CaretLineNo, newline)
                    model.InsertLine(CaretLineNo + 1, nextLine)
                    Dim needToScroll As Boolean = IsCaretOnLastVisibleLine()
                    CaretColumnNo = 1
                    CaretLineNo = CaretLineNo + 1
                    If needToScroll Then
                        view.Scroll(1)
                    End If
                    editedField = True

                Case Keys.Escape 'Escape
                    'escape key, do nothing
                Case Else
                    model.InsertChar(c, caretLocation)
                    CaretColumnNo = CaretColumnNo + 1
                    editedField = True
            End Select

            ScrollToShowCaret()
            RedrawAll()
            e.Handled = True
        End If
    End Sub

    Public Sub ResetSelection()
        selectionStartLocation.Column = 0
        selectionStartLocation.Line = 0
        selectionEndLocation.Column = 0
        selectionEndLocation.Line = 0
    End Sub

    Public Sub RemoveSelection()
        If Not HasSelection() Then
            Return
        End If

        Dim initialCaretLocation As ColumnLine = caretLocation
        ' after selection is removed, adjust CaretX position.
        Dim x1, y1, x2, y2 As Integer
        x1 = selectionStartLocation.Column
        y1 = selectionStartLocation.Line
        x2 = selectionEndLocation.Column
        y2 = selectionEndLocation.Line

        If y1 > y2 Or (y1 = y2 And x1 > x2) Then
            'swap (x1, y1) and (x2, y2)
            Dim t As Integer
            t = x1 : x1 = x2 : x2 = t
            t = y1 : y1 = y2 : y2 = t
        End If
        If y1 = y2 Then
            Dim thisLine As String = model.GetLine(y1)
            model.SetLine(y1, thisLine.Substring(0, x1 - 1) & _
              thisLine.Substring(x2 - 1, thisLine.Length - x2 + 1))
            'it's okay if event is raised when CaretColumnNo is set
            CaretColumnNo = x1
        Else
            'delete lines between y1 and y2
            Dim j As Integer
            For j = 1 To (y2 - y1 - 1)
                model.RemoveLine(y1 + 1, False) 'false means "do not raise event"
            Next
            'merge line y1 with line y2 and delete the original line y2
            Dim thisLine As String = model.GetLine(y1)
            Dim nextLine As String = model.GetLine(y1 + 1)
            model.SetLine(y1, thisLine.Substring(0, x1 - 1) & _
              nextLine.Substring(x2 - 1, nextLine.Length - x2 + 1))
            model.RemoveLine(y1 + 1, True)
            ' CaretLineNo must be adjusted before CaretColumnNo because
            ' CaretColumnNo property will use CaretLineNo. Therefore, it
            ' is important that CaretLineNo contains the correct value
            CaretLineNo = y1
            CaretColumnNo = x1
        End If
    End Sub

    Protected Overrides Function ProcessDialogKey(ByVal keyData As Keys) As Boolean
        keyProcessed = True
        Select Case keyData
            Case Keys.Down
                ResetSelection()
                If Not IsCaretOnLastLine() Then
                    If IsCaretOnLastVisibleLine() Then
                        view.Scroll(1)
                    End If
                    CaretLineNo = CaretLineNo + 1
                    If CaretColumnNo > GetCurrentLine().Length + 1 Then
                        CaretColumnNo = GetCurrentLine().Length + 1
                    End If
                    ScrollToShowCaret()
                    RedrawAll()
                End If
                Return True
            Case Keys.Up
                ResetSelection()
                If Not IsCaretOnFirstLine() Then
                    If IsCaretOnFirstVisibleLine() Then
                        view.Scroll(-1)
                    End If
                    CaretLineNo = CaretLineNo - 1
                    If CaretColumnNo > GetCurrentLine().Length + 1 Then
                        CaretColumnNo = GetCurrentLine().Length + 1
                    End If
                    ScrollToShowCaret()
                    RedrawAll()
                End If
                Return True
            Case Keys.Right
                ResetSelection()
                If IsCaretOnLastColumn() Then
                    If Not IsCaretOnLastLine() Then
                        If IsCaretOnLastVisibleLine() Then
                            view.Scroll(1)
                        End If
                        CaretLineNo = CaretLineNo + 1
                        CaretColumnNo = 1
                    End If
                Else
                    CaretColumnNo = CaretColumnNo + 1
                End If
                ScrollToShowCaret()
                RedrawAll()
                Return True
            Case Keys.Left
                ResetSelection()
                If IsCaretOnFirstColumn() Then
                    If Not IsCaretOnFirstLine() Then
                        If IsCaretOnFirstVisibleLine() Then
                            view.Scroll(-1)
                        End If
                        CaretLineNo = CaretLineNo - 1
                        CaretColumnNo = GetCurrentLine().Length + 1
                    End If
                Else
                    CaretColumnNo = CaretColumnNo - 1
                End If
                ScrollToShowCaret()
                RedrawAll()
                Return True
            Case Keys.Delete
                'Deleting character does not change caret position but
                'may change the longest line char count
                If HasSelection() Then
                    RemoveSelection()
                    ResetSelection()
                    ' then don't delete anything
                Else
                    If CaretColumnNo = GetCurrentLine().Length + 1 Then
                        ' at the end of line
                        If CaretLineNo < model.LineCount Then
                            'concatenate current line and next line
                            'and delete next line
                            Dim nextLine As String = model.GetLine(CaretLineNo + 1)
                            model.SetLine(CaretLineNo, GetCurrentLine() & nextLine)
                            model.RemoveLine(CaretLineNo + 1, True)
                        End If
                    Else
                        'delete one character
                        model.DeleteChar(caretLocation)
                    End If
                End If

                RedrawAll()
                Return True
            Case Else
                If CInt(Keys.Control And keyData) = 0 And _
                  CInt(Keys.Alt And keyData) = 0 Then
                    ' let KeyPress process the key
                    keyProcessed = False
                End If
                Return MyBase.ProcessDialogKey(keyData)
        End Select
    End Function

    Private Sub ScrollToShowCaret()
        If Not view.IsCaretVisible() Then
            If model.GetLine(CaretLineNo).Length > view.VisibleCharCount Then
                view.LeftInvisibleCharCount = GetCurrentLine().Length - _
                view.VisibleCharCount
            Else
                view.LeftInvisibleCharCount = 0
            End If
            If CaretLineNo > TopInvisibleLineCount + view.VisibleLineCount Then
                TopInvisibleLineCount = CaretLineNo - view.VisibleLineCount + 1
            End If
        End If
    End Sub

    Private Function IsCaretOnFirstColumn() As Boolean
        Return (CaretColumnNo = 1)
    End Function

    Private Function IsCaretOnLastColumn() As Boolean
        Return (CaretColumnNo = GetCurrentLine().Length + 1)
    End Function

    Private Function IsCaretOnLastVisibleLine() As Boolean
        Return (CaretLineNo = view.VisibleLineCount + TopInvisibleLineCount)
    End Function

    Private Function IsCaretOnLastLine() As Boolean
        Return (CaretLineNo = model.LineCount)
    End Function

    Private Function IsCaretOnFirstLine() As Boolean
        Return (CaretLineNo = 1)
    End Function

    Private Function IsCaretOnFirstVisibleLine() As Boolean
        Return (CaretLineNo = TopInvisibleLineCount + 1)
    End Function

    Private Sub RedrawAll()
        view.RedrawAll()
        lineNumberView.RedrawAll()
        AdjustVScrollBar()
        AdjustHScrollBar()
    End Sub

    Private Sub Highlight(ByVal x1 As Integer, ByVal y1 As Integer, _
      ByVal x2 As Integer, ByVal y2 As Integer)
        '(x1, y1) is the starting column,line of highlight
        '(x2, y2) is the end column,line of hightlight
        'swap (x1,y1) and (x2,y2) if necessary
        If y1 > y2 Or (y1 = y2 And x1 > x2) Then
            Dim t As Integer
            t = x1 : x1 = x2 : x2 = t
            t = y1 : y1 = y2 : y2 = t
        End If
        selectionStartLocation.Column = x1
        selectionStartLocation.Line = y1
        selectionEndLocation.Column = x2
        selectionEndLocation.Line = y2
    End Sub

    Public ReadOnly Property SelectedText() As String
        Get
            If HasSelection() Then
                Dim x1, y1, x2, y2 As Integer
                x1 = selectionStartLocation.Column
                y1 = selectionStartLocation.Line
                x2 = selectionEndLocation.Column
                y2 = selectionEndLocation.Line

                'swap if necessary
                If y1 > y2 Or (y1 = y2 And x1 > x2) Then
                    Dim t As Integer
                    t = x1 : x1 = x2 : x2 = t
                    t = y1 : y1 = y2 : y2 = t
                End If

                If y1 = y2 Then
                    Return model.GetLine(y1).Substring(x1 - 1, x2 - x1)
                Else
                    Dim sb As New StringBuilder(model.CharCount + 2 * model.LineCount)
                    Dim lineCount As Integer = model.LineCount
                    Dim i, lineNo As Integer
                    For i = y1 To y2
                        Dim thisLine As String = model.GetLine(i)
                        If i = y1 Then
                            sb.Append(thisLine.Substring(x1 - 1))
                        ElseIf i = y2 Then
                            sb.Append(Microsoft.VisualBasic.Constants.vbCrLf)
                            sb.Append(thisLine.Substring(0, x2 - 1))
                        Else
                            sb.Append(Microsoft.VisualBasic.Constants.vbCrLf)
                            sb.Append(thisLine)
                        End If
                    Next
                    Return sb.ToString()
                End If
            Else
                Return ""
            End If

        End Get
    End Property

    Public Overrides Property Text() As String
        Get
            Dim sb As New StringBuilder(model.CharCount + 2 * model.LineCount)
            Dim lineCount As Integer = model.LineCount
            Dim i As Integer
            For i = 1 To lineCount
                Dim thisLine As String = model.GetLine(i)
                sb.Append(thisLine)
                If i < lineCount Then
                    sb.Append(Microsoft.VisualBasic.Constants.vbCrLf)
                End If
            Next
            Return sb.ToString()
        End Get
        Set(ByVal s As String)
            If Not s Is Nothing Then
                Dim initialColumn As Integer = caretLocation.Column
                Dim initialLine As Integer = caretLocation.Line

                'remove all lines
                Dim i As Integer
                Dim lineCount As Integer = model.LineCount
                For i = 2 To lineCount
                    model.RemoveLine(2, False)
                Next
                model.SetLine(1, "") ' don't remove the first line
                caretLocation.Column = 1
                caretLocation.Line = 1
                caretLocation = model.InsertData(s, caretLocation)

                If caretLocation.Column <> initialColumn Then
                    OnColumnChanged(New _
                      ColumnEventArgs(initialColumn, caretLocation.Column))
                End If
                If caretLocation.Line <> initialLine Then
                    OnLineChanged(New LineEventArgs(initialLine, caretLocation.Line))
                End If
                ResetSelection()
                If Not view.IsCaretVisible() Then
                    ScrollToShowCaret()
                End If
                RedrawAll()
            End If
        End Set
    End Property

    Public Function IsInSelection(ByVal column As Integer, _
      ByVal line As Integer) As Boolean
        'indicate that the character at (column, line) is selected
        If Not HasSelection() Then
            Return False
        Else
            Dim x1, y1, x2, y2 As Integer
            x1 = selectionStartLocation.Column
            y1 = selectionStartLocation.Line
            x2 = selectionEndLocation.Column
            y2 = selectionEndLocation.Line

            'swap if necessary to make (x2, y2) below (x1, y1)
            If (y1 > y2) Or (y1 = y2 And x1 > x2) Then
                Dim t As Integer
                t = x2 : x2 = x1 : x1 = t
                t = y2 : y2 = y1 : y1 = t
            End If

            If y2 > model.LineCount Then
                y2 = model.LineCount
                If y1 = y2 And x1 > x2 Then
                    Dim t As Integer
                    t = x2 : x2 = x1 : x1 = t
                End If
            End If
            If line < y2 And line > y1 Then
                Return True
            ElseIf y1 = y2 And line = y1 And column >= x1 And column < x2 Then
                'selection in one line
                Return True
            ElseIf line = y1 And line <> y2 And column >= x1 Then
                Return True
            ElseIf line = y2 And line <> y1 And column < x2 Then
                Return True
            Else
                Return False
            End If
        End If
    End Function

    Public Sub Copy()
        'copy selected text to clipboard
        If HasSelection() Then
            Clipboard.SetDataObject(SelectedText)
        End If
    End Sub

    Public Sub Cut()
        If HasSelection() Then
            Copy()
            RemoveSelection()
            ResetSelection()
            RedrawAll()
        End If
    End Sub

    Public Sub Paste()
        ' In this method, CaretLocation's Line and Column fields
        ' are accessed without going through the CaretLineNo and CaretColumnNo
        ' properties so as not to raise the OnLineChanged and OnColumnChanged
        ' events repeatedly.
        Dim buffer As IDataObject = Clipboard.GetDataObject()
        If buffer.GetDataPresent(DataFormats.Text) Then

            Dim initialColumn As Integer = caretLocation.Column
            Dim initialLine As Integer = caretLocation.Line

            If HasSelection() Then
                RemoveSelection()
            End If

            Dim s As String = buffer.GetData(DataFormats.Text).ToString()
            caretLocation = model.InsertData(s, caretLocation)
            If caretLocation.Column <> initialColumn Then
                OnColumnChanged(New ColumnEventArgs(initialColumn, caretLocation.Column))
            End If
            If caretLocation.Line <> initialLine Then
                OnLineChanged(New LineEventArgs(initialLine, caretLocation.Line))
            End If
            If HasSelection() Then
                ResetSelection()
            End If
            If Not view.IsCaretVisible() Then
                ScrollToShowCaret()
            End If
            RedrawAll()
        Else
            'MsgBox("Incompatible data format")
        End If
    End Sub

    Public Sub SelectAll()
        selectionStartLocation.Column = 1
        selectionStartLocation.Line = 1
        selectionEndLocation.Column = model.GetLine(model.LineCount).Length + 1
        selectionEndLocation.Line = model.LineCount
        RedrawAll()
    End Sub

    Public Function GetCurrentLine() As String
        'return the line where the caret is on
        Return model.GetLine(CaretLineNo)
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class


