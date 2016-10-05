Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Public Class Form1 : Inherits Form
    Friend WithEvents textArea As StyledTextArea
    Friend WithEvents findPattern As TextBox
    Friend WithEvents findButton As Button
    Friend WithEvents copyButton As Button
    Friend WithEvents cutButton As Button
    Friend WithEvents pasteButton As Button
    Friend WithEvents selectButton As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents caseCB As CheckBox
    Friend WithEvents wholeWordCB As CheckBox
    Friend WithEvents goUpCB As CheckBox

    Private x1 As Integer = 1
    Private y1 As Integer = 1
    Private column As Integer = 1
    Private line As Integer = 1

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub textArea_ColumnChanged(ByVal sender As Object, _
      ByVal e As ColumnEventArgs) Handles textArea.ColumnChanged
        column = e.NewColumn
        UpdateLabel()
    End Sub

    Private Sub textArea_LineChanged(ByVal sender As Object, _
      ByVal e As LineEventArgs) Handles textArea.LineChanged
        line = e.NewLine
        UpdateLabel()
    End Sub

    Private Sub InitializeComponent()
        Me.textArea = New StyledTextAreaProject.StyledTextArea()
        Me.findPattern = New System.Windows.Forms.TextBox()
        Me.findButton = New System.Windows.Forms.Button()
        Me.caseCB = New System.Windows.Forms.CheckBox()
        Me.wholeWordCB = New System.Windows.Forms.CheckBox()
        Me.goUpCB = New System.Windows.Forms.CheckBox()
        Me.copyButton = New System.Windows.Forms.Button()
        Me.cutButton = New System.Windows.Forms.Button()
        Me.pasteButton = New System.Windows.Forms.Button()
        Me.selectButton = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'textArea
        '
        Me.textArea.CaretColor = System.Drawing.Color.Red
        Me.textArea.Edited = False
        Me.textArea.HighlightBackColor = System.Drawing.Color.Black
        Me.textArea.HighlightForeColor = System.Drawing.Color.White
        Me.textArea.LineNumberBackColor = System.Drawing.Color.FromArgb(CType(240, Byte), CType(240, Byte), CType(240, Byte))
        Me.textArea.LineNumberForeColor = System.Drawing.Color.DarkCyan
        Me.textArea.Location = New System.Drawing.Point(5, 5)
        Me.textArea.Name = "textArea"
        Me.textArea.Size = New System.Drawing.Size(680, 345)
        Me.textArea.TabIndex = 0
        Me.textArea.TextBackColor = System.Drawing.Color.White
        Me.textArea.TextForeColor = System.Drawing.Color.Black
        '
        'findPattern
        '
        Me.findPattern.Location = New System.Drawing.Point(392, 400)
        Me.findPattern.Name = "findPattern"
        Me.findPattern.Size = New System.Drawing.Size(128, 20)
        Me.findPattern.TabIndex = 1
        Me.findPattern.Text = ""
        '
        'findButton
        '
        Me.findButton.Location = New System.Drawing.Point(536, 392)
        Me.findButton.Name = "findButton"
        Me.findButton.Size = New System.Drawing.Size(128, 32)
        Me.findButton.TabIndex = 2
        Me.findButton.Text = "Find"
        '
        'caseCB
        '
        Me.caseCB.Location = New System.Drawing.Point(390, 368)
        Me.caseCB.Name = "caseCB"
        Me.caseCB.Size = New System.Drawing.Size(123, 14)
        Me.caseCB.TabIndex = 3
        Me.caseCB.Text = "Case sensitive"
        '
        'wholeWordCB
        '
        Me.wholeWordCB.Location = New System.Drawing.Point(505, 368)
        Me.wholeWordCB.Name = "wholeWordCB"
        Me.wholeWordCB.Size = New System.Drawing.Size(84, 14)
        Me.wholeWordCB.TabIndex = 4
        Me.wholeWordCB.Text = "WholeWord"
        '
        'goUpCB
        '
        Me.goUpCB.Location = New System.Drawing.Point(600, 368)
        Me.goUpCB.Name = "goUpCB"
        Me.goUpCB.Size = New System.Drawing.Size(96, 14)
        Me.goUpCB.TabIndex = 5
        Me.goUpCB.Text = "Go Up"
        '
        'copyButton
        '
        Me.copyButton.Location = New System.Drawing.Point(4, 392)
        Me.copyButton.Name = "copyButton"
        Me.copyButton.Size = New System.Drawing.Size(80, 32)
        Me.copyButton.TabIndex = 6
        Me.copyButton.Text = "Copy"
        '
        'cutButton
        '
        Me.cutButton.Location = New System.Drawing.Point(90, 392)
        Me.cutButton.Name = "cutButton"
        Me.cutButton.Size = New System.Drawing.Size(80, 32)
        Me.cutButton.TabIndex = 7
        Me.cutButton.Text = "Cut"
        '
        'pasteButton
        '
        Me.pasteButton.Location = New System.Drawing.Point(176, 392)
        Me.pasteButton.Name = "pasteButton"
        Me.pasteButton.Size = New System.Drawing.Size(80, 32)
        Me.pasteButton.TabIndex = 8
        Me.pasteButton.Text = "Paste"
        '
        'selectButton
        '
        Me.selectButton.Location = New System.Drawing.Point(280, 392)
        Me.selectButton.Name = "selectButton"
        Me.selectButton.Size = New System.Drawing.Size(80, 32)
        Me.selectButton.TabIndex = 9
        Me.selectButton.Text = "Select All"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(6, 367)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(160, 24)
        Me.Label1.TabIndex = 0
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(692, 433)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.Label1, Me.selectButton, Me.pasteButton, Me.cutButton, Me.copyButton, Me.goUpCB, Me.textArea, Me.wholeWordCB, Me.caseCB, Me.findButton, Me.findPattern})
        Me.Name = "Form1"
        Me.Text = "Using StyledTextArea"
        Me.ResumeLayout(False)

    End Sub

    Private Sub goUpCB_Click(ByVal sender As Object, ByVal e As EventArgs) _
      Handles goUpCB.CheckedChanged
        If goUpCB.Checked Then
            y1 = textArea.LineCount
            x1 = textArea.GetLine(y1).Length + 1
        Else
            x1 = 1
            y1 = 1
        End If

    End Sub

    Private Sub findButton_Click(ByVal sender As Object, ByVal e As EventArgs) _
      Handles findButton.Click
        Dim pattern As String = findPattern.Text
        Dim p As ColumnLine = textArea.Find(pattern, x1, y1, caseCB.Checked, _
          wholeWordCB.Checked, goUpCB.Checked)

        If p.Equals(New ColumnLine(0, 0)) Then
            'search not found
            If goUpCB.Checked Then
                y1 = textArea.LineCount
                x1 = textArea.GetLine(y1).Length + 1
            Else
                y1 = 1
                x1 = 1
            End If
        Else
            If Not goUpCB.Checked Then
                x1 = p.Column + 1
                y1 = p.Line
            Else
                x1 = p.Column - findPattern.Text.Length - 1
                If x1 <= 1 Then
                    x1 = 1
                    y1 = p.Line - 1
                Else
                    y1 = p.Line
                End If
            End If
        End If
    End Sub

    Private Sub copyButton_Click(ByVal sender As Object, ByVal e As EventArgs) _
      Handles copyButton.Click
        textArea.Copy()
        textArea.Focus()
    End Sub

    Private Sub cutButton_Click(ByVal sender As Object, ByVal e As EventArgs) _
      Handles cutButton.Click
        textArea.Cut()
        textArea.Focus()

    End Sub

    Private Sub pasteButton_Click(ByVal sender As Object, ByVal e As EventArgs) _
      Handles pasteButton.Click
        textArea.Paste()
        textArea.Focus()
    End Sub

    Private Sub selectButton_Click(ByVal sender As Object, ByVal e As EventArgs) _
      Handles selectButton.Click
        textArea.SelectAll()
        textArea.Focus()
    End Sub

    Private Sub UpdateLabel()
        Label1.Text = "Ln: " & line & "    Col: " & column
    End Sub

    <STAThread()> Shared Sub Main()
        System.Windows.Forms.Application.Run(New Form1())
    End Sub

End Class



