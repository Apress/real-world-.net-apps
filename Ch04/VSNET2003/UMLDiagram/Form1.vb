Option Explicit On 
Option Strict On

Imports System
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing
Imports System.Collections
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters
Imports System.Runtime.Serialization.Formatters.Binary
Imports Microsoft.VisualBasic


Public Class Form1 : Inherits Form

  Private imageList As New ImageList()
  Private imageFolder As String = "Images" & _
    Path.DirectorySeparatorChar.ToString()

  ' ----------  menu and menu items variable declarations 
  Private mainMenu As New mainMenu()
  Private fileMenuItem As New MenuItem("&File")
  Private fileNewMenuItem As New MenuItem("&New", _
    New EventHandler(AddressOf fileNewMenuItem_Click), Shortcut.CtrlN)
  Private fileOpenMenuItem As New MenuItem("&Open", _
    New EventHandler(AddressOf fileOpenMenuItem_Click), Shortcut.CtrlO)
  Private fileSaveMenuItem As New MenuItem("&Save", _
    New EventHandler(AddressOf fileSaveMenuItem_Click), Shortcut.CtrlS)
  Private fileSaveAsMenuItem As New MenuItem("Save &As", _
    New EventHandler(AddressOf fileSaveAsMenuItem_Click))
  Private fileExitMenuItem As New MenuItem("E&xit", _
    New EventHandler(AddressOf fileExitMenuItem_Click), Shortcut.CtrlX)

  Private viewMenuItem As New MenuItem("&View")
  Private viewPropertyWindowMenuItem As New MenuItem("&Property Window", _
    New EventHandler(AddressOf viewPropertyWindowMenuItem_Click), Shortcut.F4)
  ' ----------  end of menu and menu items variable declarations 

  ' -------------- Images ------------------------------------
  Private newFileImage As New Bitmap(imageFolder & "newFile.bmp")
  Private openFileImage As New Bitmap(imageFolder & "openFile.gif")
  Private saveFileImage As New Bitmap(imageFolder & "saveFile.bmp")
  Private classImage As New Bitmap(imageFolder & "class.bmp")
  Private interfaceImage As New Bitmap(imageFolder & "interface.bmp")
  Private generalizationImage As New Bitmap(imageFolder & "generalization.bmp")
  Private dependencyImage As New Bitmap(imageFolder & "dependency.bmp")
  Private associationImage As New Bitmap(imageFolder & "association.bmp")
  Private aggregationImage As New Bitmap(imageFolder & "aggregation.bmp")
  Private selectImage As New Bitmap(imageFolder & "select.bmp")
  ' -------------- End of Images ------------------------------------

  ' -------------- Toolbar ------------------------------------
  Private toolBar As New toolBar()
  Private separatorToolBarButton As New ToolBarButton()
  Private newToolBarButton As New ToolBarButton()
  Private openToolBarButton As New ToolBarButton()
  Private saveToolBarButton As New ToolBarButton()
  Private classToolBarButton As New ToolBarButton()
  Private interfaceToolBarButton As New ToolBarButton()
  Private generalizationToolBarButton As New ToolBarButton()
  Private dependencyToolBarButton As New ToolBarButton()
  Private associationToolBarButton As New ToolBarButton()
  Private aggregationToolBarButton As New ToolBarButton()
  Private selectToolBarButton As New ToolBarButton()
  ' -------------- End of Toolbar -----------------------------

  ' -------------- StatusBar ------------------------------------
  Private statusBar As New StatusBar()
  Private statusBarPanel1 As New StatusBarPanel()

  ' -------------- End of StatusBar ------------------------------------

  Private docFileName As String

  Private propertyForm As New propertyForm()

#Region " Windows Form Designer generated code "

  Public Sub New()
    MyBase.New()
    'This call is required by the Windows Form Designer.
    InitializeComponent()
    'Add any initialization after the InitializeComponent() call
  End Sub

  'Form overrides dispose to clean up the component list.
  Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
    If disposing Then
      If Not (components Is Nothing) Then
        components.Dispose()
      End If
    End If
    MyBase.Dispose(disposing)
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  Private drawArea As drawArea

  Private Sub InitializeComponent()
    fileMenuItem.MenuItems.Add(fileNewMenuItem)
    fileMenuItem.MenuItems.Add(fileOpenMenuItem)
    fileMenuItem.MenuItems.Add("-")
    fileMenuItem.MenuItems.Add(fileSaveMenuItem)
    fileMenuItem.MenuItems.Add(fileSaveAsMenuItem)
    fileMenuItem.MenuItems.Add("-")
    fileMenuItem.MenuItems.Add(fileExitMenuItem)
    viewMenuItem.MenuItems.Add(viewPropertyWindowMenuItem)
    mainMenu.MenuItems.Add(fileMenuItem)
    mainMenu.MenuItems.Add(viewMenuItem)
    Me.Menu = mainMenu

    ' images'
    imageList.Images.Add(newFileImage)
    imageList.Images.Add(openFileImage)
    imageList.Images.Add(saveFileImage)
    imageList.Images.Add(classImage)
    imageList.Images.Add(interfaceImage)
    imageList.Images.Add(generalizationImage)
    imageList.Images.Add(dependencyImage)
    imageList.Images.Add(associationImage)
    imageList.Images.Add(aggregationImage)
    imageList.Images.Add(selectImage)

    ' toolbar and toolbar buttons
    toolBar.ImageList = imageList
    toolBar.Appearance = ToolBarAppearance.Flat
    toolBar.BorderStyle = BorderStyle.FixedSingle
    toolBar.ButtonSize = New Size(14, 6)

    separatorToolBarButton.Style = ToolBarButtonStyle.Separator
    newToolBarButton.ImageIndex = 0
    newToolBarButton.ToolTipText = "New Document"
    openToolBarButton.ImageIndex = 1
    openToolBarButton.ToolTipText = "Open Document"
    saveToolBarButton.ImageIndex = 2
    saveToolBarButton.ToolTipText = "Save Document"
    classToolBarButton.ImageIndex = 3
    classToolBarButton.ToolTipText = "Class"
    interfaceToolBarButton.ImageIndex = 4
    interfaceToolBarButton.ToolTipText = "Interface"
    generalizationToolBarButton.ImageIndex = 5
    generalizationToolBarButton.ToolTipText = "Generalization"
    dependencyToolBarButton.ImageIndex = 6
    dependencyToolBarButton.ToolTipText = "Dependency"
    associationToolBarButton.ImageIndex = 7
    associationToolBarButton.ToolTipText = "Association"
    aggregationToolBarButton.ImageIndex = 8
    aggregationToolBarButton.ToolTipText = "Aggregation"
    selectToolBarButton.ImageIndex = 9
    selectToolBarButton.ToolTipText = "Select"

    toolBar.Buttons.AddRange(New ToolBarButton() {newToolBarButton, _
      openToolBarButton, saveToolBarButton, separatorToolBarButton, _
      classToolBarButton, interfaceToolBarButton, generalizationToolBarButton, _
      dependencyToolBarButton, associationToolBarButton, _
      aggregationToolBarButton, separatorToolBarButton, selectToolBarButton})

    Me.Controls.Add(toolBar)

    'status bar
    statusBar.Panels.Add(statusBarPanel1)
    statusBarPanel1.BorderStyle = StatusBarPanelBorderStyle.Sunken
    statusBarPanel1.AutoSize = StatusBarPanelAutoSize.Spring
    statusBar.ShowPanels = True

    Me.Controls.Add(statusBar)
    Me.drawArea = New DrawArea()
    WriteToPanel("Selecting a shape")
    Me.SuspendLayout()
    '
    'drawArea
    '
    Me.drawArea.AutoScroll = True
    Me.drawArea.BackColor = System.Drawing.Color.White
    Me.drawArea.Location = New System.Drawing.Point(96, 16)
    Me.drawArea.Name = "drawArea"
    Me.drawArea.Size = New System.Drawing.Size(600, 500)
    Me.drawArea.TabIndex = 2
    drawArea.Dock = DockStyle.Fill
    '
    'Form1
    '
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.ClientSize = New System.Drawing.Size(724, 565)
    Me.Controls.Add(drawArea)
    Me.Text = "UML Class Diagram Editor"
    Me.ResumeLayout(False)
    propertyForm.Owner = Me
    AddHandler propertyForm.PropertyChanged, AddressOf RefreshDrawArea
    AddHandler drawArea.LineSelected, AddressOf me_LineSelected
    AddHandler drawArea.RectSelected, AddressOf me_RectSelected
    AddHandler toolBar.ButtonClick, AddressOf toolBar_ButtonClick
  End Sub

#End Region

  Private Sub toolBar_ButtonClick(ByVal sender As Object, _
    ByVal e As ToolBarButtonClickEventArgs)
    If e.Button.Equals(newToolBarButton) Then
    ElseIf e.Button.Equals(openToolBarButton) Then
    ElseIf e.Button.Equals(saveToolBarButton) Then
    ElseIf e.Button.Equals(classToolBarButton) Then
      States.ShapeDrawn = ShapeType.Class
      WriteToPanel("Drawing Class")
    ElseIf e.Button.Equals(interfaceToolBarButton) Then
      States.ShapeDrawn = ShapeType.Interface
      WriteToPanel("Drawing Interface")
    ElseIf e.Button.Equals(generalizationToolBarButton) Then
      States.ShapeDrawn = ShapeType.Generalization
      WriteToPanel("Drawing Generalization")
    ElseIf e.Button.Equals(dependencyToolBarButton) Then
      States.ShapeDrawn = ShapeType.Dependency
      WriteToPanel("Drawing Dependency")
    ElseIf e.Button.Equals(associationToolBarButton) Then
      States.ShapeDrawn = ShapeType.Association
      WriteToPanel("Drawing Association")
    ElseIf e.Button.Equals(aggregationToolBarButton) Then
      States.ShapeDrawn = ShapeType.Aggregation
      WriteToPanel("Drawing Aggregation")
    ElseIf e.Button.Equals(selectToolBarButton) Then
      States.ShapeDrawn = ShapeType.None
      WriteToPanel("Selecting a shape")
    End If
  End Sub

  Private Sub fileNewMenuItem_Click(ByVal sender As Object, _
    ByVal e As EventArgs)
    NewDocument()
  End Sub

  Private Sub fileOpenMenuItem_Click(ByVal sender As Object, _
    ByVal e As EventArgs)
    OpenDocument()
  End Sub

  Private Sub fileSaveMenuItem_Click(ByVal sender As Object, _
    ByVal e As EventArgs)
    Save()
  End Sub

  Private Sub fileSaveAsMenuItem_Click(ByVal sender As Object, _
    ByVal e As EventArgs)
    SaveAs()
  End Sub

  Private Sub fileExitMenuItem_Click(ByVal sender As Object, _
    ByVal e As EventArgs)
    If drawArea.edited Then
      'prompt for user to save first
    End If
    Me.Close()
  End Sub

  Private Sub viewPropertyWindowMenuItem_Click(ByVal sender As Object, _
  ByVal e As EventArgs)

    propertyForm.Show()

  End Sub

  Private Sub NewDocument()
    If drawArea.edited Then
      Select Case PromptSave()
        Case DialogResult.Cancel
          Return
        Case DialogResult.Yes
          If Save() Then
            docFileName = Nothing
          Else
            Return
          End If
        Case DialogResult.No
          docFileName = Nothing
      End Select
    End If
    drawArea.RemoveShapes()
  End Sub

  Private Sub OpenDocument()
    Dim openFileDialog As New OpenFileDialog()

    'you can set an initial directory if you want, such as this
    'openFileDialog.InitialDirectory = "c:\"
    openFileDialog.Filter = "UML Documents (*.uml)|*.uml|All files (*.*)|*.*"
    openFileDialog.FilterIndex = 1

    If openFileDialog.ShowDialog() = DialogResult.OK Then
      If drawArea.edited Then
        'prompt to save first
        Select Case PromptSave()
          Case DialogResult.Cancel
            Return
          Case DialogResult.Yes
            If Save() Then
              docFileName = Nothing
            End If
          Case DialogResult.No
        End Select
      End If

      drawArea.RemoveShapes()
      drawArea.edited = False
      Dim myStream As Stream = openFileDialog.OpenFile()
      If Not myStream Is Nothing Then
        Dim formatter As IFormatter
        formatter = CType(New BinaryFormatter(), IFormatter)
        'get DrawArea's index
        drawArea.index = CType(formatter.Deserialize(myStream), Int32)

        'to be used when obtaining line mementos
        Dim rectArrayList As New ArrayList()
        Dim rectMementos As ArrayList = _
          CType(formatter.Deserialize(myStream), ArrayList)
        'each element of the array list is either a ClassRectMemento object or a
        'InterfaceRectMemento object
        Dim rectEnum As IEnumerator = rectMementos.GetEnumerator()
        While rectEnum.MoveNext
          Dim obj As Object = rectEnum.Current
          If TypeName(obj) = "ClassRectMemento" Then
            Dim memento As ClassRectMemento = CType(obj, ClassRectMemento)
            Dim rect As New ClassRect(memento)
            drawArea.AddRect(rect)
            rectArrayList.Add(rect)
          ElseIf TypeName(obj) = "InterfaceRectMemento" Then
            Dim memento As InterfaceRectMemento = CType(obj, InterfaceRectMemento)
            Dim rect As New InterfaceRect(memento)
            drawArea.AddRect(rect)
            rectArrayList.Add(rect)
          End If
        End While

        ' get line mementos
        Dim lineMementos As ArrayList = _
          CType(formatter.Deserialize(myStream), ArrayList)
        Dim lineEnum As IEnumerator = lineMementos.GetEnumerator()
        While lineEnum.MoveNext
          Dim memento As LineMemento = CType(lineEnum.Current, LineMemento)
          Dim fromRect As Rect = GetRect(memento.fromRectIndex, rectArrayList)
          Dim toRect As Rect = GetRect(memento.toRectIndex, rectArrayList)
          If (Not fromRect Is Nothing) And (Not toRect Is Nothing) Then
            Dim line As Line = New Line(fromRect, toRect, memento)
            drawArea.AddLine(line)
          End If
        End While
        myStream.Close()
        drawArea.Refresh()
      End If
    End If
  End Sub

  Private Function PromptSave() As DialogResult
    Return MessageBox.Show("Do you want to save changes you have made?", _
      "UML Class Diagram Editor", MessageBoxButtons.YesNoCancel, _
      MessageBoxIcon.Warning)
  End Function

  Private Function GetRect(ByVal index As Integer, ByVal arrayList As ArrayList) _
    As Rect
    'called from the Open method only
    Dim rectEnum As IEnumerator = arrayList.GetEnumerator()
    While rectEnum.MoveNext
      Dim rect As Rect = CType(rectEnum.Current, Rect)
      If Not rect Is Nothing Then
        If rect.index = index Then
          rectEnum = Nothing
          Return rect
        End If
      End If
    End While
    rectEnum = Nothing
    Return Nothing
  End Function

  Private Function Save() As Boolean
    If docFileName Is Nothing Then
      Return SaveAs()
    Else
      Dim myStream As Stream
      myStream = File.OpenWrite(docFileName)
      If Not myStream Is Nothing Then
        Dim formatter As IFormatter
        formatter = CType(New BinaryFormatter(), IFormatter)
        ' serialize DrawArea's index
        formatter.Serialize(myStream, drawArea.index)
        Dim rectMementos As ArrayList = drawArea.GetRectMementos()
        formatter.Serialize(myStream, rectMementos)
        Dim lineMementos As ArrayList = drawArea.GetLineMementos()
        formatter.Serialize(myStream, lineMementos)
        myStream.Close()
        drawArea.edited = False
        Return True
      End If
      Return False
    End If
  End Function

  Private Function SaveAs() As Boolean
    Dim saveFileDialog As New SaveFileDialog()
    saveFileDialog.Filter = "UML Documents (*.uml)|*.uml|All files (*.*)|*.*"
    saveFileDialog.FilterIndex = 1
    If saveFileDialog.ShowDialog = DialogResult.OK Then
      docFileName = saveFileDialog.FileName
      Return Save()
    End If
    Return False
  End Function


  Private Sub RefreshDrawArea(ByVal sender As Object, _
    ByVal e As EventArgs)
    drawArea.Refresh()
  End Sub

  Private Sub me_LineSelected(ByVal sender As Object, ByVal e As LineEventArgs)
    propertyForm.Line = e.Line
  End Sub

  Private Sub me_RectSelected(ByVal sender As Object, ByVal e As RectEventArgs)
    propertyForm.Line = Nothing
  End Sub

  Private Sub WriteToPanel(ByVal s As String)
    statusBar.Panels(0).Text = s
  End Sub

  Public Shared Sub Main()
    Application.Run(New Form1())
  End Sub

End Class
