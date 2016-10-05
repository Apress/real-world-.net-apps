Option Explicit Off
Option Strict On

Imports System
Imports System.IO
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections
Imports System.Diagnostics
Imports Microsoft.VisualBasic

Public Class Form1
    Inherits System.Windows.Forms.Form

    Private localCurrentDir As String = Directory.GetCurrentDirectory()
    Private remoteCurrentDir As String
    Private server, userName, password As String
    Private ftp As New ftp()
    'the size of the file being downloaded/uploaded
    Private fileSize As Integer

    Private Structure DirectoryItem
        Public name As String
        Public modifiedDate As String
        Public size As String
    End Structure

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        InitializeControls()

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If Disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(Disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Private hSplitter As System.Windows.Forms.Splitter
    Private vSplitter As System.Windows.Forms.Splitter
    Private leftPanel As System.Windows.Forms.Panel
    Private localDir As System.Windows.Forms.ComboBox
    Private localButtonsPanel As System.Windows.Forms.Panel
    Private uploadButton As System.Windows.Forms.Button
    Private remoteDir As System.Windows.Forms.ComboBox
    Private rightPanel As System.Windows.Forms.Panel
    Private localDirList As System.Windows.Forms.ListView
    Private remoteDirList As System.Windows.Forms.ListView
    Private remoteButtonsPanel As System.Windows.Forms.Panel
    Private downloadButton As System.Windows.Forms.Button
    Private ImageList As System.Windows.Forms.ImageList
    Private messageTextBox As System.Windows.Forms.TextBox
    Private mainMenu As System.Windows.Forms.MainMenu
    Private fileMenuItem As System.Windows.Forms.MenuItem
    Private remoteRenameButton As System.Windows.Forms.Button
    Private remoteMakeDirButton As System.Windows.Forms.Button
    Private localDeleteButton As System.Windows.Forms.Button
    Private localRenameButton As System.Windows.Forms.Button
    Private localMakeDirButton As System.Windows.Forms.Button
    Private connectFileMenuItem As System.Windows.Forms.MenuItem
    Private exitFileMenuItem As System.Windows.Forms.MenuItem
    Private remoteDeleteButton As System.Windows.Forms.Button
    Private progressBar As System.Windows.Forms.ProgressBar
    Private label1 As System.Windows.Forms.Label
    Private label2 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.leftPanel = New System.Windows.Forms.Panel()
        Me.localDirList = New System.Windows.Forms.ListView()
        Me.ImageList = New System.Windows.Forms.ImageList(Me.components)
        Me.localButtonsPanel = New System.Windows.Forms.Panel()
        Me.localMakeDirButton = New System.Windows.Forms.Button()
        Me.localDeleteButton = New System.Windows.Forms.Button()
        Me.uploadButton = New System.Windows.Forms.Button()
        Me.localRenameButton = New System.Windows.Forms.Button()
        Me.localDir = New System.Windows.Forms.ComboBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.progressBar = New System.Windows.Forms.ProgressBar()
        Me.rightPanel = New System.Windows.Forms.Panel()
        Me.remoteButtonsPanel = New System.Windows.Forms.Panel()
        Me.remoteMakeDirButton = New System.Windows.Forms.Button()
        Me.remoteRenameButton = New System.Windows.Forms.Button()
        Me.remoteDeleteButton = New System.Windows.Forms.Button()
        Me.downloadButton = New System.Windows.Forms.Button()
        Me.remoteDirList = New System.Windows.Forms.ListView()
        Me.remoteDir = New System.Windows.Forms.ComboBox()
        Me.label2 = New System.Windows.Forms.Label()
        Me.messageTextBox = New System.Windows.Forms.TextBox()
        Me.hSplitter = New System.Windows.Forms.Splitter()
        Me.vSplitter = New System.Windows.Forms.Splitter()
        Me.mainMenu = New System.Windows.Forms.MainMenu()
        Me.fileMenuItem = New System.Windows.Forms.MenuItem()
        Me.connectFileMenuItem = New System.Windows.Forms.MenuItem()
        Me.exitFileMenuItem = New System.Windows.Forms.MenuItem()
        Me.leftPanel.SuspendLayout()
        Me.localButtonsPanel.SuspendLayout()
        Me.rightPanel.SuspendLayout()
        Me.remoteButtonsPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'leftPanel
        '
        Me.leftPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.leftPanel.Controls.AddRange(New System.Windows.Forms.Control() {Me.localDirList, Me.localButtonsPanel, Me.localDir, Me.label1})
        Me.leftPanel.Dock = System.Windows.Forms.DockStyle.Left
        Me.leftPanel.Name = "leftPanel"
        Me.leftPanel.Size = New System.Drawing.Size(350, 349)
        Me.leftPanel.TabIndex = 2
        Me.leftPanel.Text = "Local Computer"
        '
        'localDirList
        '
        Me.localDirList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.localDirList.Location = New System.Drawing.Point(0, 37)
        Me.localDirList.Name = "localDirList"
        Me.localDirList.Size = New System.Drawing.Size(282, 308)
        Me.localDirList.SmallImageList = Me.ImageList
        Me.localDirList.TabIndex = 2
        Me.localDirList.View = System.Windows.Forms.View.Details
        '
        'ImageList
        '
        Me.ImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.ImageList.ImageSize = New System.Drawing.Size(16, 16)
        Me.ImageList.TransparentColor = System.Drawing.Color.Transparent
        '
        'localButtonsPanel
        '
        Me.localButtonsPanel.Controls.AddRange(New System.Windows.Forms.Control() {Me.localMakeDirButton, Me.localDeleteButton, Me.uploadButton, Me.localRenameButton})
        Me.localButtonsPanel.Dock = System.Windows.Forms.DockStyle.Right
        Me.localButtonsPanel.Location = New System.Drawing.Point(282, 37)
        Me.localButtonsPanel.Name = "localButtonsPanel"
        Me.localButtonsPanel.Size = New System.Drawing.Size(64, 308)
        Me.localButtonsPanel.TabIndex = 3
        '
        'localMakeDirButton
        '
        Me.localMakeDirButton.Location = New System.Drawing.Point(0, 128)
        Me.localMakeDirButton.Name = "localMakeDirButton"
        Me.localMakeDirButton.Size = New System.Drawing.Size(64, 32)
        Me.localMakeDirButton.TabIndex = 5
        Me.localMakeDirButton.Text = "MakeDir"
        '
        'localDeleteButton
        '
        Me.localDeleteButton.Location = New System.Drawing.Point(0, 64)
        Me.localDeleteButton.Name = "localDeleteButton"
        Me.localDeleteButton.Size = New System.Drawing.Size(64, 32)
        Me.localDeleteButton.TabIndex = 2
        Me.localDeleteButton.Text = "Delete"
        '
        'uploadButton
        '
        Me.uploadButton.Dock = System.Windows.Forms.DockStyle.Top
        Me.uploadButton.Name = "uploadButton"
        Me.uploadButton.Size = New System.Drawing.Size(64, 64)
        Me.uploadButton.TabIndex = 0
        Me.uploadButton.Text = "Upload"
        '
        'localRenameButton
        '
        Me.localRenameButton.Location = New System.Drawing.Point(0, 96)
        Me.localRenameButton.Name = "localRenameButton"
        Me.localRenameButton.Size = New System.Drawing.Size(64, 32)
        Me.localRenameButton.TabIndex = 4
        Me.localRenameButton.Text = "Rename"
        '
        'localDir
        '
        Me.localDir.Dock = System.Windows.Forms.DockStyle.Top
        Me.localDir.Location = New System.Drawing.Point(0, 16)
        Me.localDir.Name = "localDir"
        Me.localDir.Size = New System.Drawing.Size(346, 21)
        Me.localDir.TabIndex = 1
        '
        'label1
        '
        Me.label1.Dock = System.Windows.Forms.DockStyle.Top
        Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(346, 16)
        Me.label1.TabIndex = 0
        Me.label1.Text = "Local Computer"
        '
        'progressBar
        '
        Me.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.progressBar.Location = New System.Drawing.Point(0, 357)
        Me.progressBar.Name = "progressBar"
        Me.progressBar.Size = New System.Drawing.Size(728, 20)
        Me.progressBar.TabIndex = 7
        '
        'rightPanel
        '
        Me.rightPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.rightPanel.Controls.AddRange(New System.Windows.Forms.Control() {Me.remoteButtonsPanel, Me.remoteDirList, Me.remoteDir, Me.label2})
        Me.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rightPanel.Location = New System.Drawing.Point(358, 0)
        Me.rightPanel.Name = "rightPanel"
        Me.rightPanel.Size = New System.Drawing.Size(370, 349)
        Me.rightPanel.TabIndex = 3
        Me.rightPanel.Text = "Remote Server"
        '
        'remoteButtonsPanel
        '
        Me.remoteButtonsPanel.Controls.AddRange(New System.Windows.Forms.Control() {Me.remoteMakeDirButton, Me.remoteRenameButton, Me.remoteDeleteButton, Me.downloadButton})
        Me.remoteButtonsPanel.Dock = System.Windows.Forms.DockStyle.Right
        Me.remoteButtonsPanel.Location = New System.Drawing.Point(302, 37)
        Me.remoteButtonsPanel.Name = "remoteButtonsPanel"
        Me.remoteButtonsPanel.Size = New System.Drawing.Size(64, 308)
        Me.remoteButtonsPanel.TabIndex = 3
        '
        'remoteMakeDirButton
        '
        Me.remoteMakeDirButton.Location = New System.Drawing.Point(0, 128)
        Me.remoteMakeDirButton.Name = "remoteMakeDirButton"
        Me.remoteMakeDirButton.Size = New System.Drawing.Size(64, 32)
        Me.remoteMakeDirButton.TabIndex = 3
        Me.remoteMakeDirButton.Text = "MakeDir"
        '
        'remoteRenameButton
        '
        Me.remoteRenameButton.Location = New System.Drawing.Point(0, 96)
        Me.remoteRenameButton.Name = "remoteRenameButton"
        Me.remoteRenameButton.Size = New System.Drawing.Size(64, 32)
        Me.remoteRenameButton.TabIndex = 2
        Me.remoteRenameButton.Text = "Rename"
        '
        'remoteDeleteButton
        '
        Me.remoteDeleteButton.Location = New System.Drawing.Point(0, 64)
        Me.remoteDeleteButton.Name = "remoteDeleteButton"
        Me.remoteDeleteButton.Size = New System.Drawing.Size(64, 32)
        Me.remoteDeleteButton.TabIndex = 1
        Me.remoteDeleteButton.Text = "Delete"
        '
        'downloadButton
        '
        Me.downloadButton.Dock = System.Windows.Forms.DockStyle.Top
        Me.downloadButton.Name = "downloadButton"
        Me.downloadButton.Size = New System.Drawing.Size(64, 64)
        Me.downloadButton.TabIndex = 0
        Me.downloadButton.Text = "Download"
        '
        'remoteDirList
        '
        Me.remoteDirList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.remoteDirList.Location = New System.Drawing.Point(0, 37)
        Me.remoteDirList.Name = "remoteDirList"
        Me.remoteDirList.Size = New System.Drawing.Size(366, 308)
        Me.remoteDirList.SmallImageList = Me.ImageList
        Me.remoteDirList.TabIndex = 2
        Me.remoteDirList.View = System.Windows.Forms.View.Details
        '
        'remoteDir
        '
        Me.remoteDir.Dock = System.Windows.Forms.DockStyle.Top
        Me.remoteDir.Location = New System.Drawing.Point(0, 16)
        Me.remoteDir.Name = "remoteDir"
        Me.remoteDir.Size = New System.Drawing.Size(366, 21)
        Me.remoteDir.TabIndex = 1
        '
        'label2
        '
        Me.label2.Dock = System.Windows.Forms.DockStyle.Top
        Me.label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(366, 16)
        Me.label2.TabIndex = 0
        Me.label2.Text = "Remote Server"
        '
        'messageTextBox
        '
        Me.messageTextBox.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.messageTextBox.Location = New System.Drawing.Point(0, 377)
        Me.messageTextBox.Multiline = True
        Me.messageTextBox.Name = "messageTextBox"
        Me.messageTextBox.ReadOnly = True
        Me.messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.messageTextBox.Size = New System.Drawing.Size(728, 116)
        Me.messageTextBox.TabIndex = 4
        Me.messageTextBox.Text = ""
        '
        'hSplitter
        '
        Me.hSplitter.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.hSplitter.Location = New System.Drawing.Point(0, 349)
        Me.hSplitter.Name = "hSplitter"
        Me.hSplitter.Size = New System.Drawing.Size(728, 8)
        Me.hSplitter.TabIndex = 5
        Me.hSplitter.TabStop = False
        '
        'vSplitter
        '
        Me.vSplitter.Location = New System.Drawing.Point(350, 0)
        Me.vSplitter.Name = "vSplitter"
        Me.vSplitter.Size = New System.Drawing.Size(8, 349)
        Me.vSplitter.TabIndex = 6
        Me.vSplitter.TabStop = False
        '
        'mainMenu
        '
        Me.mainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.fileMenuItem})
        '
        'fileMenuItem
        '
        Me.fileMenuItem.Index = 0
        Me.fileMenuItem.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.connectFileMenuItem, Me.exitFileMenuItem})
        Me.fileMenuItem.Text = "&File"
        '
        'connectFileMenuItem
        '
        Me.connectFileMenuItem.Index = 0
        Me.connectFileMenuItem.Shortcut = System.Windows.Forms.Shortcut.F3
        Me.connectFileMenuItem.Text = "&Connect"
        '
        'exitFileMenuItem
        '
        Me.exitFileMenuItem.Index = 1
        Me.exitFileMenuItem.Text = "E&xit"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(728, 493)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.rightPanel, Me.vSplitter, Me.leftPanel, Me.hSplitter, Me.progressBar, Me.messageTextBox})
        Me.Menu = Me.mainMenu
        Me.Name = "Form1"
        Me.Text = "NET FTP"
        Me.leftPanel.ResumeLayout(False)
        Me.localButtonsPanel.ResumeLayout(False)
        Me.rightPanel.ResumeLayout(False)
        Me.remoteButtonsPanel.ResumeLayout(False)
        Me.ResumeLayout(False)


    End Sub

#End Region

    Private Sub InitializeControls()
        AddHandler downloadButton.Click, AddressOf downloadButton_Click
        AddHandler uploadButton.Click, AddressOf uploadButton_Click

        AddHandler localDeleteButton.Click, AddressOf localDeleteButton_Click
        AddHandler localRenameButton.Click, AddressOf localRenameButton_Click
        AddHandler localMakeDirButton.Click, AddressOf localMakeDirButton_Click

        AddHandler remoteDeleteButton.Click, AddressOf remoteDeleteButton_Click
        AddHandler remoteRenameButton.Click, AddressOf remoteRenameButton_Click
        AddHandler remoteMakeDirButton.Click, AddressOf remoteMakeDirButton_Click

        AddHandler connectFileMenuItem.Click, AddressOf connectFileMenuItem_Click
        AddHandler exitFileMenuItem.Click, AddressOf exitFileMenuItem_Click

        AddHandler ftp.BeginDownload, AddressOf ftp_BeginDownload
        AddHandler ftp.EndDownload, AddressOf ftp_EndDownload
        AddHandler ftp.BeginUpload, AddressOf ftp_BeginUpload
        AddHandler ftp.EndUpload, AddressOf ftp_EndUpload
        AddHandler ftp.TransferProgressChanged, AddressOf ftp_TransferProgressChanged

        ImageList.Images.Add(Bitmap.FromFile("./images/Up.gif"))
        ImageList.Images.Add(Bitmap.FromFile("./images/Folder.gif"))
        ImageList.Images.Add(Bitmap.FromFile("./images/File.gif"))

        localDirList.View = View.Details
        localDirList.Columns.Add("Name", -2, HorizontalAlignment.Left)
        localDirList.Columns.Add("Size", -2, HorizontalAlignment.Right)
        localDirList.Columns.Add("Modified", -2, HorizontalAlignment.Left)
        localDirList.SmallImageList = ImageList
        localDirList.Activation = ItemActivation.Standard
        localDirList.MultiSelect = False
        AddHandler localDirList.ItemActivate, AddressOf localDirList_ItemActivate

        remoteDirList.View = View.Details
        remoteDirList.Columns.Add("Name", 160, HorizontalAlignment.Left)
        remoteDirList.Columns.Add("Size", 60, HorizontalAlignment.Right)
        remoteDirList.Columns.Add("Modified", 80, HorizontalAlignment.Left)
        remoteDirList.SmallImageList = ImageList
        remoteDirList.MultiSelect = False
        AddHandler remoteDirList.ItemActivate, AddressOf remoteDirList_ItemActivate

        AddHandler remoteDir.SelectedIndexChanged, _
          AddressOf remoteDir_SelectedIndexChanged
        AddHandler localDir.SelectedIndexChanged, _
          AddressOf localDir_SelectedIndexChanged
        SelectLocalDirectory(localCurrentDir)
        Log("Welcome. Press F3 for quick login.")

    End Sub

    ' ---------------------- Event handlers ----------------------
    Private Sub connectFileMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        Connect()
    End Sub

    Private Sub downloadButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        DownloadFile()
    End Sub

    Private Sub exitFileMenuItem_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        Me.Close()
    End Sub

    Private Sub ftp_BeginDownload(ByVal sender As Object, _
      ByVal e As EventArgs)
        InitializeProgressBar()
    End Sub

    Private Sub ftp_BeginUpload(ByVal sender As Object, _
      ByVal e As EventArgs)
        InitializeProgressBar()
    End Sub

    Private Sub ftp_EndDownload(ByVal sender As Object, _
      ByVal e As EndDownloadEventArgs)
        Log(e.Message)
        LoadLocalDirList()
    End Sub

    Private Sub ftp_EndUpload(ByVal sender As Object, _
      ByVal e As EndUploadEventArgs)
        Log(e.Message)
        LoadRemoteDirList()
    End Sub

    Private Sub ftp_TransferProgressChanged(ByVal sender As Object, _
    ByVal e As TransferProgressChangedEventArgs)
        UpdateProgressBar(e.TransferredByteCount)
    End Sub

    Private Sub localDeleteButton_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        DeleteLocalFile()
    End Sub

    Private Sub localDir_SelectedIndexChanged(ByVal sender As Object, _
      ByVal e As EventArgs)
        UpdateLocalDir()
    End Sub

    Private Sub localDirList_ItemActivate(ByVal sender As Object, _
      ByVal e As EventArgs)
        ChangeLocalDir()
    End Sub

    Private Sub localMakeDirButton_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        MakeLocalDir()
    End Sub

    Private Sub localRenameButton_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        RenameLocalFile()
    End Sub

    Private Sub remoteDeleteButton_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        DeleteRemoteFile()
    End Sub

    Private Sub remoteDir_SelectedIndexChanged(ByVal sender As Object, _
      ByVal e As EventArgs)
        UpdateRemoteDir()
    End Sub

    Private Sub remoteDirList_ItemActivate(ByVal sender As Object, _
      ByVal e As EventArgs)
        ChangeRemoteDir()
    End Sub

    Private Sub remoteMakeDirButton_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        MakeRemoteDir()
    End Sub

    Private Sub remoteRenameButton_Click(ByVal sender As Object, _
      ByVal e As EventArgs)
        RenameRemoteFile()
    End Sub

    Private Sub uploadButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        UploadFile()
    End Sub
    ' ---------------------- end of Event handlers ----------------------

    Private Sub ChangeLocalDir()
        'get activated item (the items that was double-clicked
        Dim item As ListViewItem = localDirList.SelectedItems(0)
        If item.Text.Equals("..") Then
            Dim parentDir As DirectoryInfo = Directory.GetParent(localCurrentDir)
            If Not parentDir Is Nothing Then
                localCurrentDir = parentDir.FullName
                SelectLocalDirectory(localCurrentDir)
            End If
        Else
            Directory.SetCurrentDirectory(localCurrentDir)
            Dim fullPath As String = Path.GetFullPath(item.Text)
            If Helper.IsDirectory(fullPath) Then
                localCurrentDir = fullPath
                SelectLocalDirectory(localCurrentDir)
            Else
                UploadFile()
            End If
        End If
    End Sub

    Private Sub ChangeRemoteDir()
        If ftp.Connected Then

            'get activated item (the item that was double-clicked)
            Dim item As ListViewItem = remoteDirList.SelectedItems(0)

            If item.Text.Equals("..") Then
                Dim index As Integer 'get the last index of "/"
                index = remoteCurrentDir.LastIndexOf("/")
                If index = 0 Then
                    remoteCurrentDir = "/"
                Else
                    remoteCurrentDir = remoteCurrentDir.Substring(0, index)
                End If
                ftp.ChangeDir(remoteCurrentDir)
                If ftp.replyCode.StartsWith("2") Then 'successful
                    SelectRemoteDirectory(remoteCurrentDir)
                End If
                Log(ftp.replyMessage)
            ElseIf Helper.IsDirectoryItem(remoteDirList.SelectedItems(0)) Then
                If remoteCurrentDir.Equals("/") Then
                    remoteCurrentDir += item.Text
                Else
                    remoteCurrentDir += "/" & item.Text
                End If
                ftp.ChangeDir(remoteCurrentDir)
                If ftp.replyCode.StartsWith("2") Then 'successful
                    SelectRemoteDirectory(remoteCurrentDir)
                End If
                Log(ftp.replyMessage)
            Else
                DownloadFile()
            End If

        Else
            NotConnected()
        End If
    End Sub

    Private Sub Connect()
        'connect and disconnect
        If connectFileMenuItem.Text.Equals("&Disconnect") Then
            'disconnect
            If MessageBox.Show("Disconnect from remote server?", "Disconnect", _
              MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = _
              DialogResult.OK Then
                If ftp.Connected Then
                    ftp.Disconnect()
                    Log("Disconnected.")
                    connectFileMenuItem.Text = "&Connect"
                    'clearing the ListView
                    'don't use the remoteDirList.Clear because it removes the columns too,
                    'instead use remoteDirList.Items.Clear()
                    remoteDirList.Items.Clear()
                    'clearing the combo box
                    remoteDir.Items.Clear()
                    remoteDir.Text = ""
                End If
            End If
        Else
            'connect
            Dim loginForm As New LoginForm()
            Dim loggedIn As Boolean = False
            While Not loggedIn AndAlso loginForm.ShowDialog() = DialogResult.OK
                server = loginForm.Server
                userName = loginForm.UserName
                password = loginForm.Password
                Log("Connecting " & server)
                Try
                    ftp.Connect(server)
                    If ftp.Connected Then
                        Log(server & " connected. Try to login.")
                        If ftp.Login(userName, password) Then
                            connectFileMenuItem.Text = "&Disconnect"
                            Log("Login successful.")
                            loggedIn = True
                            ' try to get the remote list
                            ftp.ChangeToAsciiMode()
                            remoteCurrentDir = ftp.GetCurrentRemoteDir()
                            If Not remoteCurrentDir Is Nothing Then
                                SelectRemoteDirectory(remoteCurrentDir)
                            End If
                        Else
                            Log("Login failed.")
                        End If
                    Else
                        Log("Connection failed")
                    End If
                Catch e As Exception
                    Log(e.ToString())
                End Try
            End While
            If Not loggedIn AndAlso _
              Not ftp Is Nothing AndAlso _
              ftp.Connected Then
                ftp.Disconnect()
            End If
        End If
    End Sub

    Private Sub DeleteLocalFile()
        Dim selectedItemCount As Integer = localDirList.SelectedItems.Count
        If selectedItemCount = 0 Then
            MessageBox.Show("Please select a file/directory to delete.", _
              "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            If MessageBox.Show("Delete the selected file/directory?", _
              "Delete Confirmation", _
              MessageBoxButtons.OKCancel, MessageBoxIcon.Question) _
              = DialogResult.OK Then
                Dim completePath As String = _
                  Path.Combine(localCurrentDir, localDirList.SelectedItems(0).Text)
                Try
                    If Helper.IsDirectory(completePath) Then
                        Directory.Delete(completePath)
                    Else
                        File.Delete(completePath)
                    End If
                    LoadLocalDirList()
                Catch ex As Exception
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, _
                      MessageBoxIcon.Error)
                End Try
            End If
        End If

    End Sub

    Private Sub DeleteRemoteFile()
        If ftp.Connected Then
            Dim selectedItemCount As Integer = remoteDirList.SelectedItems.Count
            If selectedItemCount = 0 Then
                MessageBox.Show("Please select a file/directory to delete.", _
                  "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                If MessageBox.Show("Delete the selected file/directory?", _
                  "Delete Confirmation", _
                  MessageBoxButtons.OKCancel, MessageBoxIcon.Question) _
                  = DialogResult.OK Then
                    Try
                        Dim selectedItem As ListViewItem = remoteDirList.SelectedItems(0)
                        If Helper.IsDirectoryItem(selectedItem) Then
                            If ftp.DeleteDir(selectedItem.Text) Then
                                LoadRemoteDirList()
                            Else
                                Log(ftp.replyMessage)
                            End If
                        Else
                            If ftp.DeleteFile(selectedItem.Text) Then
                                LoadRemoteDirList()
                            Else
                                Log(ftp.replyMessage)
                            End If
                        End If
                    Catch ex As Exception
                        MessageBox.Show(ex.ToString, "Error", MessageBoxButtons.OK, _
                          MessageBoxIcon.Error)
                    End Try
                End If
            End If
        Else
            NotConnected()
        End If
    End Sub

    Private Sub DownloadFile()
        If ftp.Connected Then
            Dim selectedItemCount As Integer = remoteDirList.SelectedItems.Count

            If selectedItemCount = 0 Then
                MessageBox.Show("Please select a file to download.", _
                  "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                Dim item As ListViewItem = remoteDirList.SelectedItems(0)
                If Helper.IsDirectoryItem(item) Then
                    MessageBox.Show("You cannot download a directory.", _
                      "Error downloading file", MessageBoxButtons.OK, _
                      MessageBoxIcon.Error)
                Else
                    Try
                        fileSize = Convert.ToInt32(item.SubItems(1).Text)
                    Catch
                    End Try
                    ftp.Download(item.Text, localCurrentDir)
                End If
            End If
        Else
            NotConnected()
        End If
    End Sub

    Private Function GetDirectoryItem(ByVal s As String) As DirectoryItem
        's is in the following format
        '-rwxrwxrwx   1 owner    group           11801 Jul 23 10:52 NETFTP.vb
        '
        'or
        '
        'drwxrwxrwx   1 owner    group               0 Jul 26 20:11 New Folder

        Dim dirItem As New DirectoryItem()
        If Not s Is Nothing Then
            Dim index As Integer
            index = s.IndexOf(" "c)
            If index <> -1 Then
                s = s.Substring(index).TrimStart() 'removing "drwxrwxrwx" part 
                'now s is in the following format
                '1 owner    group           11801 Jul 23 10:52 NETFTP.vb
                '
                'or
                '
                '1 owner    group               0 Jul 26 20:11 New Folder
                index = s.IndexOf(" "c)
                If index <> -1 Then
                    s = s.Substring(index).TrimStart() 'removing the '1' part
                    'now s is in the following format
                    'owner    group           11801 Jul 23 10:52 NETFTP.vb
                    '
                    'or
                    '
                    'owner    group               0 Jul 26 20:11 New Folder
                    index = s.IndexOf(" "c)
                    If index <> -1 Then
                        s = s.Substring(index).TrimStart() 'removing the 'owner' part
                        'now s is in the following format
                        'group           11801 Jul 23 10:52 NETFTP.vb
                        '
                        'or
                        '
                        'group               0 Jul 26 20:11 New Folder
                        index = s.IndexOf(" "c)
                        If index <> -1 Then
                            s = s.Substring(index).TrimStart() 'removing the 'group' part
                            'now s is in the following format
                            '11801 Jul 23 10:52 NETFTP.vb
                            '
                            'or
                            '
                            '0 Jul 26 20:11 New Folder

                            'now get the size.
                            index = s.IndexOf(" "c)
                            If index > 0 Then
                                dirItem.size = s.Substring(0, index)
                                s = s.Substring(index).TrimStart() 'removing the size
                                'now s is in the following format
                                'Jul 23 10:52 NETFTP.vb
                                '
                                'or
                                '
                                'Jul 26 20:11 New Folder
                                'now, get the 3 elements of the date part
                                Dim date1, date2, date3 As String
                                index = s.IndexOf(" "c)
                                If index <> -1 Then
                                    date1 = s.Substring(0, index)
                                    s = s.Substring(index).TrimStart()
                                    index = s.IndexOf(" "c)
                                    If index <> -1 Then
                                        date2 = s.Substring(0, index)
                                        s = s.Substring(index).TrimStart()
                                        index = s.IndexOf(" "c)
                                        If index <> -1 Then
                                            date3 = s.Substring(0, index)
                                            dirItem.modifiedDate = date1 & " " & date2 & " " & date3
                                            ' get the name
                                            dirItem.name = s.Substring(index).Trim()
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
        Return dirItem
    End Function

    Private Sub InitializeProgressBar()
        progressBar.Value = 0
        progressBar.Maximum = fileSize
    End Sub

    Private Sub LoadLocalDirList()
        localDirList.Items.Clear()
        Dim item As ListViewItem

        ' if current directory is not root, add pointer to parent dir
        If Not Directory.GetParent(localCurrentDir) Is Nothing Then
            item = New ListViewItem("..", 1)
            item.ImageIndex = 0
            localDirList.Items.Add(item)
        End If

        ' list of directories
        Dim directories As String() = Directory.GetDirectories(localCurrentDir)
        Dim length As Integer = directories.Length
        Dim dirName As String
        For Each dirName In directories
            item = New ListViewItem(Path.GetFileName(dirName), 1)
            item.SubItems.Add("")
            item.SubItems.Add(Directory.GetLastAccessTime(dirName).ToString())
            item.ImageIndex = 1
            localDirList.Items.Add(item)
        Next

        'list of files
        Dim files As String() = Directory.GetFiles(localCurrentDir)
        length = files.Length
        Dim fileName As String
        For Each fileName In files
            item = New ListViewItem(Path.GetFileName(fileName), 1)
            Dim fi As New FileInfo(fileName)
            item.SubItems.Add(Convert.ToString(fi.Length))
            item.SubItems.Add(File.GetLastWriteTime(fileName).ToString())
            item.ImageIndex = 2
            localDirList.Items.Add(item)
        Next

    End Sub

    Private Sub LoadRemoteDirList()
        If ftp.Connected Then
            remoteDirList.Items.Clear()
            Dim item As ListViewItem

            If Not remoteCurrentDir.Equals("/") Then
                item = New ListViewItem("..", 1)
                item.ImageIndex = 0
                remoteDirList.Items.Add(item)
            End If

            Try
                ftp.ChangeDir(remoteCurrentDir)
                ftp.GetDirList()
                Dim lines As String() = _
                  ftp.DirectoryList.Split(Convert.ToChar(ControlChars.Cr))
                Dim line As String
                Dim fileList As New ArrayList()
                Dim dirList As New ArrayList()

                For Each line In lines
                    If line.Trim().StartsWith("-") Then ' a file
                        fileList.Add(line)
                    ElseIf line.Trim().StartsWith("d") Then ' a directory
                        dirList.Add(line)
                    End If
                Next

                ' now load subdirectories to DirListView
                Dim enumerator As IEnumerator = dirList.GetEnumerator
                While enumerator.MoveNext
                    Dim dirItem As DirectoryItem = _
                      GetDirectoryItem(CType(enumerator.Current, String))
                    If Not dirItem.name Is Nothing Then
                        item = New ListViewItem(dirItem.name, 1)
                        item.SubItems.Add("")
                        item.SubItems.Add(dirItem.modifiedDate)
                        remoteDirList.Items.Add(item)
                    End If
                End While

                enumerator = fileList.GetEnumerator
                While enumerator.MoveNext
                    Dim dirItem As DirectoryItem = _
                      GetDirectoryItem(CType(enumerator.Current, String))
                    If Not dirItem.name Is Nothing Then
                        item = New ListViewItem(dirItem.name, 2)
                        item.SubItems.Add(diritem.size)
                        item.SubItems.Add(dirItem.modifiedDate)
                        remoteDirList.Items.Add(item)
                    End If
                End While
            Catch e As Exception
                Debug.WriteLine(e.ToString())
            End Try
        Else
            NotConnected()
        End If
    End Sub

    Private Sub Log(ByVal message As String)
        messageTextBox.Text += message & ControlChars.CrLf
        'forces the TextBox to scroll 
        messageTextBox.SelectionStart = messageTextBox.Text.Length
        messageTextBox.ScrollToCaret()
    End Sub

    Private Sub MakeLocalDir()
        Dim dirName As String = InputBox( _
          "Enter the name of the directory to create in the local computer", _
          "Make New Directory").Trim()
        If Not dirName.Equals("") Then
            Dim fullPath As String = Path.Combine(localCurrentDir, dirName)
            If Directory.Exists(fullPath) Then
                MessageBox.Show("Directory already exists.", _
                  "Error creating directory", MessageBoxButtons.OK, _
                  MessageBoxIcon.Error)
            Else
                If File.Exists(fullPath) Then
                    MessageBox.Show("Directory name is the same as the name of a file.", _
                      "Error creating directory", MessageBoxButtons.OK, _
                      MessageBoxIcon.Error)
                Else
                    Try
                        Directory.CreateDirectory(fullPath)
                        LoadLocalDirList()
                    Catch e As Exception
                        MessageBox.Show(e.ToString, _
                          "Error creating directory", MessageBoxButtons.OK, _
                          MessageBoxIcon.Error)
                    End Try
                End If
            End If
        End If
    End Sub

    Private Sub MakeRemoteDir()
        If ftp.Connected Then
            Dim dirName As String = InputBox( _
              "Enter the name of the directory to create in the remote server", _
              "Make New Directory").Trim()
            If Not dirName.Equals("") Then
                ftp.MakeDir(dirName)
                Log(ftp.replyMessage)
                If ftp.replyCode.StartsWith("2") Then
                    LoadRemoteDirList()
                    'Dim item As New ListViewItem(dirName, 1)
                    'If remoteCurrentDir.Equals("/") Then
                    '  remoteDirList.Items.Insert(1, item)
                    'Else
                    '  remoteDirList.Items.Insert(0, item)
                    'End If
                End If
            End If
        Else
            NotConnected()
        End If
    End Sub

    Private Sub NotConnected()
        Log("Not connected")
        connectFileMenuItem.Text = "&Connect"
        'clearing the ListView
        'don't use the remoteDirList.Clear because it removes the columns too,
        'instead use remoteDirList.Items.Clear()
        remoteDirList.Items.Clear()
        'clearing the combo box
        remoteDir.Items.Clear()
        remoteDir.Text = ""
    End Sub

    Private Sub RenameLocalFile()
        Dim selectedItemCount As Integer = localDirList.SelectedItems.Count
        If selectedItemCount = 0 Then
            MessageBox.Show("Please select a file/directory to rename.", _
              "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Dim newName As String = InputBox("Enter the new name", "Rename").Trim()
            If Not newName.Equals("") Then
                Dim item As ListViewItem = localDirList.SelectedItems(0)
                If newName.Equals(item.Text) Then
                    MessageBox.Show("Please enter a different name from the " & _
                      "file/directory you are trying to rename.", _
                      "Error renaming file/directory", MessageBoxButtons.OK, _
                      MessageBoxIcon.Error)
                Else
                    Dim fullPath As String = Path.Combine(localCurrentDir, item.Text)
                    If Helper.IsDirectory(fullPath) Then
                        Directory.Move(fullPath, Path.Combine(localCurrentDir, newName))
                    Else
                        Dim fi As New FileInfo(fullPath)
                        fi.MoveTo(Path.Combine(localCurrentDir, newName))
                    End If
                    LoadLocalDirList()
                End If
            End If
        End If
    End Sub

    Private Sub RenameRemoteFile()
        If ftp.Connected Then
            Dim selectedItemCount As Integer = remoteDirList.SelectedItems.Count
            If selectedItemCount = 0 Then
                MessageBox.Show("Please select a file/directory to rename.", _
                  "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                Dim dirName As String = InputBox( _
                  "Enter the new name", "Rename").Trim()
                If Not dirName.Equals("") Then
                    Dim item As ListViewItem = remoteDirList.SelectedItems(0)
                    If dirName.Equals(item.Text) Then
                        MessageBox.Show("Please enter a different name from the " & _
                          "file/directory you are trying to rename.", _
                          "Error renaming file/directory", MessageBoxButtons.OK, _
                          MessageBoxIcon.Error)
                    Else
                        ftp.Rename(item.Text, dirName)
                        If ftp.replyCode.StartsWith("2") Then
                            item.Text = dirName
                        End If
                        Log(ftp.replyCode & " " & ftp.replyMessage)
                    End If
                End If
            End If
        Else
            NotConnected()
        End If
    End Sub

    Private Sub SelectLocalDirectory(ByVal path As String)
        ' add current dir to the list
        localDir.Items.Remove(path)
        localDir.Items.Insert(0, path)
        'this will trigger the localDir ComboBox's SelectedIndexChanged event
        localDir.SelectedIndex = 0
    End Sub

    Private Sub SelectRemoteDirectory(ByVal path As String)
        If ftp.Connected Then
            ' add current dir to thel ist
            remoteDir.Items.Remove(path)
            remoteDir.Items.Insert(0, path)
            'this will trigger the remoteDir ComboBox's SelectedIndexChanged event
            remoteDir.SelectedIndex = 0
        Else
            NotConnected()
        End If
    End Sub

    Private Sub UpdateLocalDir()
        Dim selectedIndex As Integer = localDir.SelectedIndex
        If localDir.SelectedIndex <> -1 Then
            localCurrentDir = CType(localDir.Items(selectedIndex), String)
            LoadLocalDirList()
        End If
    End Sub

    Private Sub UpdateProgressBar(ByVal count As Integer)
        progressBar.Value = count
    End Sub

    Private Sub UpdateRemoteDir()
        If ftp.Connected Then
            Dim selectedIndex As Integer = remoteDir.SelectedIndex
            If remoteDir.SelectedIndex <> -1 Then
                remoteCurrentDir = CType(remoteDir.Items(selectedIndex), String)
                LoadRemoteDirList()
            End If
        Else
            NotConnected()
        End If
    End Sub

    Private Sub UploadFile()
        If ftp.Connected Then
            Dim selectedItemCount As Integer = localDirList.SelectedItems.Count
            If selectedItemCount = 0 Then
                MessageBox.Show("Please select a file to upload.", _
                  "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                Dim item As ListViewItem = localDirList.SelectedItems(0)
                If Helper.IsDirectoryItem(item) Then
                    MessageBox.Show("You cannot upload a directory.", _
                      "Error uploading file", MessageBoxButtons.OK, _
                      MessageBoxIcon.Error)
                Else
                    Try
                        fileSize = Convert.ToInt32(item.SubItems(1).Text)
                    Catch
                    End Try
                    ftp.Upload(item.Text, localCurrentDir)
                End If
            End If
        Else
            NotConnected()
        End If
    End Sub

End Class
