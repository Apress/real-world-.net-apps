Imports System
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing

Public Class FindDialog : Inherits Form

    Private Shared findDialog As FindDialog
    Private textArea As StyledTextArea

    Private downRadioButton As New RadioButton
    Private upRadioButton As New RadioButton
    Private groupBox1 As New GroupBox
    Private wholeWordCheckBox As New CheckBox
    Private caseCheckBox As New CheckBox
    Private findButton As New Button
    Private replaceButton As New Button
    Private replaceAllButton As New Button
    Private closeButton As New Button
    Private findTextBox As New TextBox
    Private replaceTextBox As New TextBox
    Private label1 As New Label
    Private label2 As New Label
    Private index As Integer = 0
    Private x1 As Integer = 1
    Private y1 As Integer = 1

    Private Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Size = New Size(470, 166)
        Me.Text = "Find And Replace"
        Me.AutoScaleBaseSize = New Size(5, 13)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Icon = Nothing
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog

        Dim buttonSize As New Size(96, 24)

        findButton.Location = New Point(352, 8)
        findButton.Size = New Size(96, 24)
        findButton.TabIndex = 2
        findButton.Text = "&Find Next"
        AddHandler findButton.Click, AddressOf findButton_Click

        replaceButton.Location = New Point(352, 38)
        replaceButton.Size = buttonSize
        replaceButton.TabIndex = 3
        replaceButton.Text = "&Replace"
        AddHandler replaceButton.Click, AddressOf replaceButton_Click

        replaceAllButton.Location = New Point(352, 68)
        replaceAllButton.Size = buttonSize
        replaceAllButton.TabIndex = 4
        replaceAllButton.Text = "Replace &All"
        AddHandler replaceAllButton.Click, AddressOf replaceAllButton_Click

        closeButton.Location = New Point(352, 108)
        closeButton.Size = buttonSize
        closeButton.TabIndex = 5
        closeButton.Text = "&Close"
        AddHandler closeButton.Click, AddressOf closeButton_Click

        upRadioButton.Location = New System.Drawing.Point(8, 16)
        upRadioButton.Size = New System.Drawing.Size(64 + 30, 16)
        upRadioButton.TabIndex = 0
        upRadioButton.Text = "Up"

        groupBox1.Location = New Point(140, 72)
        groupBox1.TabIndex = 11
        groupBox1.TabStop = False
        groupBox1.Text = "Direction"
        groupBox1.Size = New Size(134 + 63, 40)

        wholeWordCheckBox.Location = New Point(8, 92)
        wholeWordCheckBox.Text = "Whole Word Only"
        wholeWordCheckBox.Size = New Size(112, 25)
        wholeWordCheckBox.TabIndex = 7
        wholeWordCheckBox.TextAlign = ContentAlignment.MiddleLeft

        caseCheckBox.Location = New Point(8, 68)
        caseCheckBox.Text = "Case Sensitive"
        caseCheckBox.Size = New Size(112, 25)
        caseCheckBox.TabIndex = 6
        caseCheckBox.TextAlign = ContentAlignment.MiddleLeft

        findTextBox.Location = New Point(74, 12)
        findTextBox.Size = New Size(264, 21)
        findTextBox.TabIndex = 1

        replaceTextBox.Location = New Point(74, 40)
        replaceTextBox.Size = New Size(264, 21)
        replaceTextBox.TabIndex = 1

        label1.Location = New Point(2, 16)
        label1.Text = "Find What"
        label1.Size = New Size(80, 24)
        label1.TabIndex = 0

        label2.Location = New Point(2, 42)
        label2.Text = "Replace With"
        label2.Size = New Size(80, 24)
        label2.TabIndex = 0

        downRadioButton.Location = New Point(64 + 30, 16)
        downRadioButton.Size = New Size(64 + 20, 16)
        downRadioButton.TabIndex = 1
        downRadioButton.Text = "Down"
        downRadioButton.Checked = True

        Me.Controls.Add(groupBox1)
        Me.Controls.Add(wholeWordCheckBox)
        Me.Controls.Add(caseCheckBox)
        Me.Controls.Add(findButton)
        Me.Controls.Add(replaceButton)
        Me.Controls.Add(replaceAllButton)
        Me.Controls.Add(closeButton)
        Me.Controls.Add(findTextBox)
        Me.Controls.Add(replaceTextBox)
        Me.Controls.Add(label1)
        Me.Controls.Add(label2)

        groupBox1.Controls.Add(downRadioButton)
        groupBox1.Controls.Add(upRadioButton)

    End Sub

    ' ------------------ event handlers --------------------------------

    Protected Sub findButton_Click(ByVal sender As Object, _
      ByVal e As System.EventArgs)
        Find()
    End Sub

    Protected Sub replaceButton_Click(ByVal sender As Object, _
      ByVal e As System.EventArgs)
        Replace()
    End Sub

    Protected Sub replaceAllButton_Click(ByVal sender As Object, _
      ByVal e As System.EventArgs)
        ReplaceAll()
    End Sub

    Protected Sub closeButton_Click(ByVal sender As Object, _
      ByVal e As System.EventArgs)
        Me.Hide()
    End Sub
    ' ------------------ end of event handlers --------------------------------

    Private Function Find() As Boolean
        If Not textArea Is Nothing Then
            Dim pattern As String = findTextBox.Text
            If pattern.Length > 0 Then
                Dim p As ColumnLine = textArea.Find(pattern, x1, y1, _
                  caseCheckBox.Checked, wholeWordCheckBox.Checked, upRadioButton.Checked)

                If p.Equals(New ColumnLine(0, 0)) Then
                    'search not found
                    If upRadioButton.Checked Then
                        y1 = textArea.LineCount
                        x1 = textArea.GetLine(y1).Length + 1
                    Else
                        y1 = 1
                        x1 = 1
                    End If
                    MessageBox.Show("Search Pattern Not Found")
                    Return False
                Else
                    If Not upRadioButton.Checked Then
                        x1 = p.Column + 1
                        y1 = p.Line
                    Else
                        x1 = p.Column - pattern.Length - 1
                        If x1 <= 1 Then
                            x1 = 1
                            y1 = p.Line - 1
                        Else
                            y1 = p.Line
                        End If
                    End If
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Private Sub ReplaceAll()
        While Replace()
        End While
    End Sub

    Private Function Replace() As Boolean
        If Not textArea Is Nothing Then
            If textArea.SelectedText.Equals("") Then
                Return Find()
            Else
                Dim replacePattern As String = replaceTextBox.Text
                Dim buffer As IDataObject = Clipboard.GetDataObject()
                textArea.Cut()
                Clipboard.SetDataObject(replacePattern)
                textArea.Paste()
                Clipboard.SetDataObject(buffer)
                Return Find()
            End If
        End If
    End Function

    Public Sub SetTextArea(ByVal textArea As StyledTextArea)
        Me.textArea = textArea
    End Sub

    Public Shared Function GetInstance()
        If findDialog Is Nothing Then
            findDialog = New FindDialog
        End If
        Return findDialog
    End Function

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        findDialog = Nothing
        MyBase.Dispose(disposing)
    End Sub

    Protected Overrides Sub OnClosing(ByVal e As CancelEventArgs)
        e.Cancel = True 'doesn't allow the user to close
        Me.Hide()
    End Sub

End Class