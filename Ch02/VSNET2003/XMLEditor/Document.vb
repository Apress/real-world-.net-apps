Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Windows.Forms
Imports System.IO
Imports System.Xml

Public Delegate Sub TitleEventHandler(ByVal sender As Object, _
  ByVal e As TitleEventArgs)

Public Class TitleEventArgs : Inherits EventArgs
    Private titleField As String
    Public Sub New(ByVal title As String)
        titleField = title
    End Sub
    Public ReadOnly Property Title() As String
        Get
            Return titleField
        End Get
    End Property
End Class

Public Class Document : Inherits Form

    Public ChildIndex As Integer
    Private line As Integer = 1
    Private column As Integer = 1
    Private textPrinter As TextPrinter
    Private findDialog As findDialog

    Private statusBarField As StatusBar
    Public Event TitleChanged As TitleEventHandler
    Private xmlViewer As xmlViewer

    Private textArea As StyledTextArea
    Private mainMenu As MainMenu

    Public FilePath As String

    Private fileMenuItem As New MenuItem
    Private fileSaveMenuItem As New MenuItem("&Save", _
      New EventHandler(AddressOf fileSaveMenuItem_Click), Shortcut.CtrlS)
    Private fileSaveAsMenuItem As New MenuItem("Save &As...", _
      New EventHandler(AddressOf fileSaveAsMenuItem_Click))
    Private filePageSetupMenuItem As New MenuItem("Page Set&up...", _
      New EventHandler(AddressOf filePageSetupMenuItem_Click))
    Private filePrintPreviewMenuItem As New MenuItem("Print Pre&view", _
      New EventHandler(AddressOf filePrintPreviewMenuItem_Click))
    Private filePrintMenuItem As New MenuItem("&Print...", _
      New EventHandler(AddressOf filePrintMenuItem_Click), Shortcut.CtrlP)
    Private fileSeparatorMenuItem As New MenuItem("-")

    Private editMenuItem As New MenuItem("&Edit")
    Private editCopyMenuItem As New MenuItem("&Copy", _
      New EventHandler(AddressOf editCopyMenuItem_Click), Shortcut.CtrlC)
    Private editCutMenuItem As New MenuItem("Cu&t", _
      New EventHandler(AddressOf editCutMenuItem_Click), Shortcut.CtrlX)
    Private editPasteMenuItem As New MenuItem("&Paste", _
      New EventHandler(AddressOf editPasteMenuItem_Click), Shortcut.CtrlV)
    Private editSelectAllMenuItem As New MenuItem("&Select All", _
      New EventHandler(AddressOf editSelectAllMenuItem_Click), Shortcut.CtrlA)
    Private editFindMenuItem As New MenuItem("&Find ...", _
      New EventHandler(AddressOf editFindMenuItem_Click), Shortcut.CtrlF)
    Private editReplaceMenuItem As New MenuItem("&Replace ...", _
      New EventHandler(AddressOf editReplaceMenuItem_Click), Shortcut.CtrlH)

    Private toolMenuItem As New MenuItem
    Private toolValidateMenuItem As New MenuItem("&Validate", _
      New EventHandler(AddressOf toolValidateMenuItem_Click), Shortcut.F5)
    Private toolValidateWithInlineSchemaMenuItem As New _
      MenuItem("Validate (Inline &Schema)", _
      New EventHandler(AddressOf toolValidateWithInlineSchemaMenuItem_Click), _
        Shortcut.F6)
    Private toolRebuildTreeMenuItem As New MenuItem("&Rebuild Tree", _
      New EventHandler(AddressOf toolRebuildTreeMenuItem_Click), Shortcut.F9)

    Public WriteOnly Property StatusBar() As StatusBar
        Set(ByVal statusBar As StatusBar)
            statusBarField = statusBar
        End Set
    End Property

    Public Sub New(ByVal childIndex As Integer, ByVal filePath As String, _
      ByRef xmlViewer As xmlViewer)
        Me.FilePath = filePath
        Me.ChildIndex = childIndex
        Me.Title = Path.GetFileName(filePath)
        Me.xmlViewer = xmlViewer
        InitializeComponent()
    End Sub

    Public Sub New(ByVal childIndex As Integer, ByRef xmlViewer As xmlViewer)
        Me.ChildIndex = childIndex
        Me.Title = "Document " & childIndex.ToString()
        Me.xmlViewer = xmlViewer
        InitializeComponent()
    End Sub

    Private Sub UpdateLineAndColumnNumbers()
        WriteToRightPanel("Line: " & line & "    Column: " & column)
    End Sub

    Private Sub InitializeComponent()
        textArea = New StyledTextArea
        mainMenu = New MainMenu
        Me.Menu = mainMenu
        textArea.Dock = System.Windows.Forms.DockStyle.Fill
        AddHandler textArea.ColumnChanged, AddressOf textArea_ColumnChanged
        AddHandler textArea.LineChanged, AddressOf textArea_LineChanged

        'Add File Menu
        fileMenuItem.Text = "&File"
        fileMenuItem.MergeType = MenuMerge.MergeItems
        fileMenuItem.MergeOrder = 0
        fileSaveMenuItem.MergeOrder = 113
        fileSaveAsMenuItem.MergeOrder = 114
        fileSeparatorMenuItem.MergeOrder = 115
        filePageSetupMenuItem.MergeOrder = 116
        filePrintPreviewMenuItem.MergeOrder = 117
        filePrintMenuItem.MergeOrder = 118

        fileMenuItem.MenuItems.Add(fileSaveMenuItem)
        fileMenuItem.MenuItems.Add(fileSaveAsMenuItem)
        fileMenuItem.MenuItems.Add(filePageSetupMenuItem)
        fileMenuItem.MenuItems.Add(filePrintPreviewMenuItem)
        fileMenuItem.MenuItems.Add(filePrintMenuItem)
        fileMenuItem.MenuItems.Add(fileSeparatorMenuItem)

        editMenuItem.MergeType = MenuMerge.Add
        editMenuItem.MergeOrder = 5
        AddHandler editMenuItem.Popup, AddressOf editMenuItem_Popup

        editMenuItem.MenuItems.Add("-")
        editMenuItem.MenuItems.Add(editCopyMenuItem)
        editMenuItem.MenuItems.Add(editCutMenuItem)
        editMenuItem.MenuItems.Add(editPasteMenuItem)
        editMenuItem.MenuItems.Add("-")
        editMenuItem.MenuItems.Add(editSelectAllMenuItem)
        editMenuItem.MenuItems.Add("-")
        editMenuItem.MenuItems.Add(editFindMenuItem)
        editMenuItem.MenuItems.Add(editReplaceMenuItem)

        If Clipboard.GetDataObject() Is Nothing Then
            editPasteMenuItem.Enabled = False
        End If

        toolMenuItem.Text = "&Tool"
        toolMenuItem.MergeType = MenuMerge.Add
        toolMenuItem.MergeOrder = 9
        toolMenuItem.MenuItems.Add(toolValidateMenuItem)
        toolMenuItem.MenuItems.Add(toolValidateWithInlineSchemaMenuItem)
        toolMenuItem.MenuItems.Add("-")
        toolMenuItem.MenuItems.Add(toolRebuildTreeMenuItem)

        mainMenu.MenuItems.Add(fileMenuItem)
        mainMenu.MenuItems.Add(editMenuItem)
        mainMenu.MenuItems.Add(toolMenuItem)
        Me.Controls.Add(textArea)
        textArea.Focus()

    End Sub

    ' ------------------ event handlers --------------------------------

    Private Sub textArea_ColumnChanged(ByVal sender As Object, _
      ByVal e As ColumnEventArgs)
        column = e.NewColumn
        UpdateLineAndColumnNumbers()
    End Sub

    Private Sub textArea_LineChanged(ByVal sender As Object, _
      ByVal e As LineEventArgs)
        line = e.NewLine
        UpdateLineAndColumnNumbers()
    End Sub

    Private Sub toolValidateMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        ValidateDocument(False) 'validation without schema
    End Sub

    Private Sub toolValidateWithInlineSchemaMenuItem_Click( _
      ByVal sender As Object, ByVal e As EventArgs)
        ValidateDocument(True) 'validation without schema
    End Sub

    Private Sub toolRebuildTreeMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        RebuildTree()
    End Sub

    Protected Sub fileSaveMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        Save()
    End Sub

    Protected Sub filePrintPreviewMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        PrintPreview()
    End Sub

    Private Sub fileSaveAsMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        SaveAs()
    End Sub 'fileSaveAsMenuItem_Click

    Private Sub filePageSetupMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        SetupPage()
    End Sub

    Private Sub filePrintMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        Print()
    End Sub

    Private Sub editMenuItem_Popup(ByVal sender As Object, ByVal e As EventArgs)
        If textArea.SelectedText.Equals("") Then
            editCopyMenuItem.Enabled = False
            editCutMenuItem.Enabled = False
        Else
            editCopyMenuItem.Enabled = True
            editCutMenuItem.Enabled = True
        End If

    End Sub

    Private Sub editCopyMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        textArea.Copy()
    End Sub

    Private Sub editCutMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        textArea.Cut()
    End Sub

    Private Sub editPasteMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        textArea.Paste()
    End Sub

    Private Sub editSelectAllMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        textArea.SelectAll()
    End Sub

    Private Sub editFindMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        Find()
    End Sub

    Private Sub editReplaceMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        Find()
    End Sub

    ' ------------------ end of event handlers --------------------------------
    Protected Overrides Sub OnClosing(ByVal e As CancelEventArgs)
        If textArea.Edited Then
            Dim dialogResult As DialogResult = MessageBox.Show( _
              "Do you want to save changes you made to " & Me.Title, _
              "Changes Not Saved", MessageBoxButtons.YesNoCancel, _
              MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, _
              MessageBoxOptions.DefaultDesktopOnly)

            If dialogResult = dialogResult.Cancel Then
                e.Cancel = True
            ElseIf dialogResult = dialogResult.Yes Then
                'save here
                If FilePath Is Nothing Then
                    SaveAs()
                Else
                    Save()
                End If
            End If

        End If
    End Sub

    Protected Overrides Sub OnGotFocus(ByVal e As EventArgs)
        findDialog = findDialog.GetInstance()
        findDialog.Owner = Me
        findDialog.SetTextArea(textArea)
    End Sub

    Protected Overridable Sub OnTitleChanged(ByVal e As TitleEventArgs)
        RaiseEvent TitleChanged(Me, e)
    End Sub

    Private Function Save() As Boolean
        If FilePath Is Nothing Then
            Return SaveAs()
        Else
            Try
                Dim sw As New StreamWriter(FilePath)
                sw.Write(Text)
                sw.Close()
                textArea.Edited = False
                WriteToLeftPanel("Document saved")
                Return True
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error Saving Document")
                Return False
            End Try
        End If
    End Function

    Private Function ValidateDocument(ByVal withInlineSchema As Boolean) As Boolean
        'set the current directory so the system knows how to find
        'external files (such as DTD files), if any is referenced
        'from the current document.
        'The current directory is the directory where the document resides.
        'For new document, force the user to save.
        If FilePath Is Nothing Then
            If SaveAs() Then
                Return ValidateDocument(withInlineSchema)
            Else
                Return False
            End If
        End If
        Environment.CurrentDirectory = Path.GetDirectoryName(FilePath)

        Dim xmlText As String = Me.Text
        If xmlText.Trim().Equals("") Then
            MessageBox.Show("Document is empty.", "Error")
            Return False
        End If
        Dim xmlReader As XmlTextReader
        Dim xmlValReader As XmlValidatingReader
        Try
            xmlReader = New XmlTextReader(New StringReader(xmlText))

            If withInlineSchema Then
                xmlValReader = New _
                  XmlValidatingReader(xmlText, XmlNodeType.Element, Nothing)
            Else
                xmlValReader = New XmlValidatingReader(xmlReader)
            End If

            While xmlValReader.Read()
            End While
            MessageBox.Show("Document validated.", "Validating Document")
            Return True
        Catch ex As Exception
            Dim errorMessage As String = ex.Message
            ' it ends with "Line x, position y" or "(x, y)"
            MessageBox.Show(errorMessage, "Error validating document")
            Dim index1, index2 As Integer
            Dim line As String
            index1 = errorMessage.LastIndexOf("Line")
            Try
                If index1 = -1 Then
                    index1 = errorMessage.LastIndexOf("(")
                    index2 = errorMessage.LastIndexOf(",")
                    line = errorMessage.Substring(index1 + 1, index2 - index1 - 1)
                Else
                    index2 = errorMessage.LastIndexOf(", position")
                    line = errorMessage.Substring(index1 + 5, index2 - index1 - 5)
                End If
                SelectLine(CInt(line))
            Catch
            End Try
            Return False
        End Try

    End Function

    Private Sub RebuildTree()
        xmlViewer.BuildTree(Me.Text)
    End Sub

    Private Function IsFilenameUsed(ByVal path As String) As Boolean
        'check if a particular path is already used as a name
        'of a file opened as another child document
        Dim childCount As Integer = Me.MdiParent.MdiChildren.Length
        Dim i As Integer
        For i = 0 To childCount - 1
            Dim doc As Document = CType(Me.MdiParent.MdiChildren(i), Document)
            If Not doc.FilePath Is Nothing Then
                If doc.FilePath.Equals(path) Then
                    Return True
                End If
            End If
        Next i
        Return False
    End Function

    Private Function SaveAs() As Boolean
        'returns True if the user saves 
        'returns False if the user cancels the saving

        Dim saveFileDialog As New SaveFileDialog
        saveFileDialog.Filter = "Xml Documents (*.xml)|*.xml|All files (*.*)|*.*"
        saveFileDialog.FilterIndex = 1
        If saveFileDialog.ShowDialog = DialogResult.OK Then
            'does not allow the doc to be saved as a name
            'of a file that is opened as another child document
            If IsFilenameUsed(saveFileDialog.FileName) Then
                MessageBox.Show( _
                  "The name is identical with one of the open documents. " & _
                  "Please use another name")
                Return False
            Else
                FilePath = saveFileDialog.FileName
                Me.Title = Path.GetFileName(FilePath)
                OnTitleChanged(New TitleEventArgs(Me.Title))
                Return Save()
            End If
        End If
    End Function

    Private Sub PrintPreview()
        GetTextPrinter().PrintPreview()
    End Sub

    Private Sub SetupPage()
        GetTextPrinter().SetupPage()
    End Sub

    Private Sub Print()
        GetTextPrinter().Print()
    End Sub

    Public Sub Find()
        If Not findDialog Is Nothing Then
            findDialog.Show()
        End If
    End Sub

    Public Function SelectLine(ByVal lineNo As Integer)
        textArea.SelectLine(lineNo)
    End Function

    Public Property Title() As String
        Get
            Return MyBase.Text
        End Get
        Set(ByVal title As String)
            MyBase.Text = title
        End Set
    End Property

    Public Overloads Property Text() As String
        Get
            Return textArea.Text
        End Get
        Set(ByVal newText As String)
            If Not textArea Is Nothing Then
                textArea.Text = newText
            End If
        End Set
    End Property

    Private Function GetTextPrinter() As textPrinter
        If textPrinter Is Nothing Then
            textPrinter = New TextPrinter
        End If
        textPrinter.Text = textArea.Text
        Return textPrinter
    End Function

    Private Sub WriteToLeftPanel(ByVal s As String)
        If Not statusBarField Is Nothing Then
            If statusBarField.Panels.Count > 0 Then
                statusBarField.Panels(0).Text = s
            End If
        End If
    End Sub

    Private Sub WriteToRightPanel(ByVal s As String)
        If Not statusBarField Is Nothing Then
            If statusBarField.Panels.Count > 1 Then
                statusBarField.Panels(1).Text = s
            End If
        End If
    End Sub

End Class

