
Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO
Imports System.ComponentModel
Imports System.Xml

Public Class XMLEditor : Inherits Form

    Class IndexedTabPage : Inherits TabPage
        Private childIndexField As Integer
        Public Property ChildIndex() As Integer
            Get
                Return childIndexField
            End Get
            Set(ByVal index As Integer)
                childIndexField = index
            End Set
        End Property
    End Class

    Private childIndex As Integer = 1 ' one-based index for MDI child and tab pages

    Private xmlViewer As New XmlViewer()
    Private imageList As New ImageList()
    Private imageFolder As String = "Images" & _
      Path.DirectorySeparatorChar.ToString()
    Private xmlDocumentFilePath As String
    Private leftSplitter = New Splitter()
    Private childFormActivated As Boolean


    ' -------------- Images ------------------------------------
    Private newFileImage As New Bitmap(imageFolder & "newFile.bmp")
    Private openFileImage As New Bitmap(imageFolder & "openFile.gif")
    Private saveFileImage As New Bitmap(imageFolder & "saveFile.bmp")
    Private printImage As New Bitmap(imageFolder & "print.gif")
    Private cutImage As New Bitmap(imageFolder & "cut.bmp")
    Private copyImage As New Bitmap(imageFolder & "copy.bmp")
    Private pasteImage As New Bitmap(imageFolder & "paste.bmp")
    Private xmlDocumentImage As New Bitmap(imageFolder & "xmlDocument.bmp")
    Private rootImage As New Bitmap(imageFolder & "root.bmp")
    Private selectedRootImage As New Bitmap(imageFolder & "selectedRoot.bmp")
    Private branchImage As New Bitmap(imageFolder & "branch.bmp")
    Private selectedBranchImage As New Bitmap(imageFolder & "selectedBranch.bmp")
    Private leafImage As New Bitmap(imageFolder & "leaf.bmp")
    Private selectedLeafImage As New Bitmap(imageFolder & "selectedLeaf.bmp")

    ' -------------- End of Images ------------------------------------

    ' -------------- Menu ------------------------------------
    Private fileMenuItem As New MenuItem()

    ' the following constructor is the same as:
    '   Private fileNewMenuItem As New MenuItem()
    '   fileNewMenuItem.Text = "&New"
    '   fileNewMenuItem.Shortcut = Shortcut.CtrlN
    '   AddHandler fileNewMenuItem.Click, AddressOf Me.fileNewMenuItem_Click
    Private fileNewMenuItem As New MenuItem("&New", _
      New System.EventHandler(AddressOf Me.fileNewMenuItem_Click), Shortcut.CtrlN)

    Private fileOpenMenuItem As New MenuItem("&Open", _
      New EventHandler(AddressOf fileOpenMenuItem_Click), Shortcut.CtrlO)

    Private fileExitMenuItem As New MenuItem("E&xit", _
      New System.EventHandler(AddressOf Me.fileExitMenuItem_Click))

    Private mainMenu As New MainMenu()
    Private windowMenuItem As New MenuItem()

    ' -------------- End of Menu ------------------------------------

    ' -------------- Toolbar ------------------------------------
    Private toolBar As New ToolBar()
    Private separatorToolBarButton As New ToolBarButton()
    Private newToolBarButton As New ToolBarButton()
    Private openToolBarButton As New ToolBarButton()
    Private saveToolBarButton As New ToolBarButton()
    Private printToolBarButton As New ToolBarButton()
    Private cutToolBarButton As New ToolBarButton()
    Private copyToolBarButton As New ToolBarButton()
    Private pasteToolBarButton As New ToolBarButton()

    ' -------------- End of Toolbar ------------------------------------

    ' -------------- TabControl and TabPage -------------------------------
    Private tabControl As New TabControl()
    ' -------------- End of TabControl and TabPage -------------------

    ' -------------- StatusBar ------------------------------------
    Private statusBar As New StatusBar()
    Private statusBarPanel1 As New StatusBarPanel()
    Private statusBarPanel2 As New StatusBarPanel()

    ' -------------- End of StatusBar ------------------------------------

    Public Sub New()
        InitializeComponents()
    End Sub

    Public Sub InitializeComponents()
        Me.IsMdiContainer = True
        Me.Text = "New South .NET XML Editor"
        Me.Icon = New Icon(imageFolder & "applicationLogo.ico")
        Me.Width = 800
        Me.Height = 600
        Me.StartPosition = FormStartPosition.CenterScreen

        imageList.Images.Add(newFileImage)
        imageList.Images.Add(openFileImage)
        imageList.Images.Add(saveFileImage)
        imageList.Images.Add(printImage)
        imageList.Images.Add(cutImage)
        imageList.Images.Add(copyImage)
        imageList.Images.Add(pasteImage)
        imageList.Images.Add(xmlDocumentImage)
        imageList.Images.Add(rootImage)
        imageList.Images.Add(selectedRootImage)
        imageList.Images.Add(branchImage)
        imageList.Images.Add(selectedBranchImage)
        imageList.Images.Add(leafImage)
        imageList.Images.Add(selectedLeafImage)

        ' menu
        fileMenuItem.Text = "&File"
        fileMenuItem.MergeType = MenuMerge.MergeItems
        fileMenuItem.MergeOrder = 0

        mainMenu.MenuItems.Add(fileMenuItem)
        fileOpenMenuItem.MergeOrder = 101
        fileNewMenuItem.MergeOrder = 100
        fileExitMenuItem.MergeOrder = 120

        fileMenuItem.MenuItems.Add(fileNewMenuItem)
        fileMenuItem.MenuItems.Add(fileOpenMenuItem)

        Dim separatorFileMenuItem As MenuItem = _
          fileMenuItem.MenuItems.Add("-")
        separatorFileMenuItem.MergeOrder = 119

        fileMenuItem.MenuItems.Add(fileExitMenuItem)

        windowMenuItem.Text = "&Window"
        windowMenuItem.MergeOrder = 10
        windowMenuItem.MenuItems.Add("&Cascade", _
          New EventHandler(AddressOf windowCascadeMenuItem_Clicked))
        windowMenuItem.MenuItems.Add("Tile &Horizontal", _
          New EventHandler(AddressOf windowTileHMenuItem_Clicked))
        windowMenuItem.MenuItems.Add("Tile &Vertical", _
          New EventHandler(AddressOf windowTileVMenuItem_Clicked))
        windowMenuItem.MdiList = True
        'Adds the MDI Window List to the bottom of the menu
        mainMenu.MenuItems.Add(windowMenuItem)

        toolBar.Appearance = ToolBarAppearance.Flat
        'an alternative is
        'toolBar.Appearance = ToolBarAppearance.Normal
        toolBar.BorderStyle = BorderStyle.FixedSingle
        toolBar.ImageList = imageList
        toolBar.ButtonSize = New Size(14, 6)

        separatorToolBarButton.Style = ToolBarButtonStyle.Separator
        newToolBarButton.ToolTipText = "New Document"
        newToolBarButton.ImageIndex = 0
        openToolBarButton.ToolTipText = "Open Document"
        openToolBarButton.ImageIndex = 1
        saveToolBarButton.ToolTipText = "Save"
        saveToolBarButton.ImageIndex = 2
        printToolBarButton.ToolTipText = "Print"
        printToolBarButton.ImageIndex = 3
        cutToolBarButton.ToolTipText = "Cut"
        cutToolBarButton.ImageIndex = 4
        copyToolBarButton.ToolTipText = "Copy"
        copyToolBarButton.ImageIndex = 5
        pasteToolBarButton.ToolTipText = "Paste"
        pasteToolBarButton.ImageIndex = 6

        AddHandler toolBar.ButtonClick, AddressOf Me.toolBar_ButtonClick

        toolBar.Buttons.Add(newToolBarButton)
        toolBar.Buttons.Add(openToolBarButton)
        toolBar.Buttons.Add(saveToolBarButton)
        toolBar.Buttons.Add(printToolBarButton)
        toolBar.Buttons.Add(separatorToolBarButton)
        toolBar.Buttons.Add(cutToolBarButton)
        toolBar.Buttons.Add(copyToolBarButton)
        toolBar.Buttons.Add(pasteToolBarButton)

        xmlViewer.ImageList = imageList
        xmlViewer.RootImageIndex = 8
        xmlViewer.RootSelectedImageIndex = 9
        xmlViewer.BranchImageIndex = 10
        xmlViewer.BranchSelectedImageIndex = 11
        xmlViewer.LeafImageIndex = 12
        xmlViewer.LeafSelectedImageIndex = 13

        xmlViewer.Dock = DockStyle.Left

        leftSplitter.Dock = DockStyle.Left

        tabControl.Size = New Size(60, 24)
        tabControl.ImageList = imageList
        tabControl.Dock = DockStyle.Top
        AddHandler tabControl.SelectedIndexChanged, _
          AddressOf tabControl_SelectedIndexChanged

        statusBarPanel1.BorderStyle = StatusBarPanelBorderStyle.Sunken
        statusBarPanel1.Text = "Ready to validate XML document"
        statusBarPanel1.AutoSize = StatusBarPanelAutoSize.Spring
        statusBarPanel2.BorderStyle = StatusBarPanelBorderStyle.Sunken
        statusBarPanel2.ToolTipText = System.DateTime.Now.ToShortTimeString()
        statusBarPanel2.AutoSize = StatusBarPanelAutoSize.Contents
        statusBar.ShowPanels = True
        statusBar.Panels.Add(statusBarPanel1)
        statusBar.Panels.Add(statusBarPanel2)

        Me.Menu = mainMenu
        Me.Controls.Add(tabControl)
        Me.Controls.Add(leftSplitter)
        Me.Controls.Add(xmlViewer)
        Me.Controls.Add(toolBar)
        Me.Controls.Add(statusBar)
    End Sub


    ' -------------- Event Handlers --------------------------
    Private Sub fileNewMenuItem_Click(ByVal sender As Object, _
      ByVal e As System.EventArgs)
        NewDocument()
    End Sub

    Private Sub fileOpenMenuItem_Click(ByVal sender As Object, _
      ByVal e As System.EventArgs)
        OpenDocument()
    End Sub

    Private Sub fileExitMenuItem_Click(ByVal sender As Object, _
      ByVal e As System.EventArgs)
        Me.Close()
    End Sub

    Private Sub windowCascadeMenuItem_Clicked(ByVal sender As Object, _
      ByVal e As EventArgs)
        Me.LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub windowTileHMenuItem_Clicked(ByVal sender As Object, _
      ByVal e As EventArgs)
        Me.LayoutMdi(MdiLayout.TileHorizontal)
    End Sub

    Private Sub windowTileVMenuItem_Clicked(ByVal sender As Object, _
      ByVal e As EventArgs)
        Me.LayoutMdi(MdiLayout.TileVertical)
    End Sub

    Protected Sub toolBar_ButtonClick(ByVal sender As Object, _
      ByVal e As ToolBarButtonClickEventArgs)

        ' Evaluate the Button property to determine which button was clicked.
        If e.Button.Equals(newToolBarButton) Then
            fileNewMenuItem.PerformClick()
        ElseIf e.Button.Equals(openToolBarButton) Then
            fileOpenMenuItem.PerformClick()
        ElseIf e.Button.Equals(saveToolBarButton) Then
            If Me.MdiChildren.Length > 0 Then
                SendKeys.Send("%FS")
            End If
        ElseIf e.Button.Equals(printToolBarButton) Then
            If Me.MdiChildren.Length > 0 Then
                SendKeys.Send("%FP")
            End If
        ElseIf e.Button.Equals(cutToolBarButton) Then
            If Me.MdiChildren.Length > 0 Then
                SendKeys.Send("%ET")
            End If
        ElseIf e.Button.Equals(copyToolBarButton) Then
            If Me.MdiChildren.Length > 0 Then
                SendKeys.Send("%EC")
            End If
        ElseIf e.Button.Equals(pasteToolBarButton) Then
            If Me.MdiChildren.Length > 0 Then
                SendKeys.Send("%EP")
            End If
        End If
    End Sub

    Private Sub child_TitleChanged(ByVal sender As Object, _
      ByVal e As TitleEventArgs)
        Dim doc As Document = CType(sender, Document)
        Dim childIndex As Integer = doc.ChildIndex
        Dim tabPage As IndexedTabPage
        For Each tabPage In tabControl.TabPages
            If tabPage.ChildIndex = childIndex Then
                tabPage.Text = e.Title
                Exit For
            End If
        Next
    End Sub

    Protected Overrides Sub OnMdiChildActivate(ByVal e As EventArgs)
        ' the call to MyBase is important, otherwise the menus cannot be merged!!!
        MyBase.OnMdiChildActivate(e)
        Try
            childFormActivated = True
            Dim activeChildDocument As Document = CType(Me.ActiveMdiChild, Document)
            'Get the ChildIndex of the active MdiChild
            Dim activeChildIndex As Integer = activeChildDocument.ChildIndex
            activeChildDocument.Focus()
            'Now activate the tab page with the same ChildIndex
            Dim tabPage As IndexedTabPage
            Dim i As Integer
            Dim tabPageCount As Integer = tabControl.TabCount
            For i = 0 To tabPageCount - 1
                tabPage = tabControl.TabPages(i)
                If tabPage.ChildIndex = activeChildIndex Then
                    tabControl.SelectedIndex = i
                    Exit For
                End If
            Next i

            xmlViewer.BuildTree(CType(Me.ActiveMdiChild, Document).Text)

        Catch
        End Try
    End Sub

    Protected Sub tabControl_SelectedIndexChanged(ByVal sender As Object, _
      ByVal e As EventArgs)
        If Not childFormActivated Then ' this is to avoid infinite loop
            'get the ChildIndex of the active tab page
            If tabControl.SelectedIndex <> -1 Then '-1 when if is no tabpage
                Dim tabPage As IndexedTabPage = _
                  CType(tabControl.TabPages(tabControl.SelectedIndex), IndexedTabPage)
                Dim activeChildIndex As Integer = tabPage.ChildIndex '

                ' Now activate the MdiChild with the same ChildIndex
                Dim mdiChild As Document
                For Each mdiChild In Me.MdiChildren
                    If mdiChild.ChildIndex = activeChildIndex Then
                        mdiChild.Activate()
                        mdiChild.Focus()
                        Exit For
                    End If
                Next
            End If
        End If
        childFormActivated = False
    End Sub
    ' -------------- End of Event Handlers -------------------

    Private Sub NewDocument()
        Dim doc As Document = New Document(childIndex, xmlViewer)
        AddHandler doc.Closed, AddressOf child_Closed
        AddHandler doc.TitleChanged, AddressOf child_TitleChanged

        doc.MdiParent = Me
        doc.StatusBar = Me.statusBar
        doc.Show()
        AddTabPage("Document " & childIndex.ToString())
    End Sub

    Private Function GetDocumentByFilepath(ByVal path As String) As Document
        Dim childCount As Integer = Me.MdiChildren.Length
        Dim i As Integer
        For i = 0 To childCount - 1
            Dim doc As Document = CType(Me.MdiChildren(i), Document)
            Dim docPath As String = doc.FilePath
            If Not docPath Is Nothing Then
                'docPath could be null in the case of a new document
                If docPath.Equals(path) Then
                    Return doc
                End If
            End If
        Next
        Return Nothing
    End Function

    Private Sub OpenDocument()
        Dim openFileDialog As New OpenFileDialog()

        'you can set an initial directory if you want, such as this
        'openFileDialog.InitialDirectory = "c:\"
        openFileDialog.Filter = "Xml Documents (*.xml)|*.xml|All files (*.*)|*.*"
        openFileDialog.FilterIndex = 1

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            'check if the document already opened by this app.
            Dim doc As Document = GetDocumentByFilepath(openFileDialog.FileName)
            If Not doc Is Nothing Then
                doc.Activate()
            Else
                Dim stream As Stream ' the current XML document
                xmlDocumentFilePath = openFileDialog.FileName
                stream = openFileDialog.OpenFile()
                Dim sr As New StreamReader(stream)

                doc = New Document(childIndex, xmlDocumentFilePath, xmlViewer)
                AddHandler doc.Closed, AddressOf child_Closed
                AddHandler doc.TitleChanged, AddressOf child_TitleChanged
                doc.Text = sr.ReadToEnd()
                sr.Close()

                doc.MdiParent = Me
                doc.StatusBar = Me.statusBar
                doc.Show()

                AddTabPage(xmlDocumentFilePath)
            End If
        End If
    End Sub

    Private Sub child_Closed(ByVal sender As Object, ByVal e As EventArgs)
        Dim doc As Document = CType(sender, Document)
        Dim childIndex As Integer = doc.ChildIndex
        'now remove tabpage with the same childIndex
        Dim tabPage As IndexedTabPage
        For Each tabPage In tabControl.TabPages
            If tabPage.ChildIndex = childIndex Then
                tabControl.TabPages.Remove(tabPage)
                Exit For
            End If
        Next

    End Sub

    Private Sub DisplayMessageBox(ByVal message As String, ByVal title As String)
        MessageBox.Show(message, title)
    End Sub

    Private Sub DisplayErrorMessageBox(ByVal message As String, _
      ByVal title As String)
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub AddTabPage(ByVal text As String)
        Dim tabPage As New IndexedTabPage()
        tabPage.Text = Path.GetFileName(text)
        tabPage.ImageIndex = 7
        tabPage.ChildIndex = childIndex
        childIndex += 1
        tabControl.Controls.Add(tabPage)
        'Activate the new tabPage
        tabControl.SelectedIndex = tabControl.TabCount - 1
    End Sub

    <STAThread()> Shared Sub Main()
        Dim form As New XMLEditor()
        Application.Run(form)
    End Sub

End Class
