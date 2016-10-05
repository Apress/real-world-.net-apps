Option Strict On
Option Explicit On 

Imports System
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

Public Delegate Sub PropertyEventHandler(ByVal sender As Object, _
 ByVal e As EventArgs)

Public Class PropertyForm
  Inherits System.Windows.Forms.Form
  Private lineField As Line
  Public Event PropertyChanged As PropertyEventHandler

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
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents Label2 As System.Windows.Forms.Label
  Friend WithEvents Label3 As System.Windows.Forms.Label
  Friend WithEvents leftTextBox As System.Windows.Forms.TextBox
  Friend WithEvents centerTextBox As System.Windows.Forms.TextBox
  Friend WithEvents rightTextBox As System.Windows.Forms.TextBox

  Private Sub InitializeComponent()
    Me.Label1 = New System.Windows.Forms.Label()
    Me.Label2 = New System.Windows.Forms.Label()
    Me.Label3 = New System.Windows.Forms.Label()
    Me.leftTextBox = New System.Windows.Forms.TextBox()
    Me.centerTextBox = New System.Windows.Forms.TextBox()
    Me.rightTextBox = New System.Windows.Forms.TextBox()
    Me.SuspendLayout()
    '
    'Label1
    '
    Me.Label1.Location = New System.Drawing.Point(8, 8)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(40, 16)
    Me.Label1.TabIndex = 0
    Me.Label1.Text = "Left"
    '
    'Label2
    '
    Me.Label2.Location = New System.Drawing.Point(8, 32)
    Me.Label2.Name = "Label2"
    Me.Label2.Size = New System.Drawing.Size(40, 16)
    Me.Label2.TabIndex = 1
    Me.Label2.Text = "Center"
    '
    'Label3
    '
    Me.Label3.Location = New System.Drawing.Point(8, 56)
    Me.Label3.Name = "Label3"
    Me.Label3.Size = New System.Drawing.Size(40, 16)
    Me.Label3.TabIndex = 2
    Me.Label3.Text = "Right"
    '
    'leftTextBox
    '
    Me.leftTextBox.Location = New System.Drawing.Point(72, 8)
    Me.leftTextBox.Name = "leftTextBox"
    Me.leftTextBox.Size = New System.Drawing.Size(120, 20)
    Me.leftTextBox.TabIndex = 3
    Me.leftTextBox.Text = ""
    '
    'centerTextBox
    '
    Me.centerTextBox.Location = New System.Drawing.Point(72, 32)
    Me.centerTextBox.Name = "centerTextBox"
    Me.centerTextBox.Size = New System.Drawing.Size(120, 20)
    Me.centerTextBox.TabIndex = 4
    Me.centerTextBox.Text = ""
    '
    'rightTextBox
    '
    Me.rightTextBox.Location = New System.Drawing.Point(72, 56)
    Me.rightTextBox.Name = "rightTextBox"
    Me.rightTextBox.Size = New System.Drawing.Size(120, 20)
    Me.rightTextBox.TabIndex = 5
    Me.rightTextBox.Text = ""
    '
    'PropertyForm
    '
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.ClientSize = New System.Drawing.Size(200, 85)
    Me.Controls.AddRange(New Control() _
      {rightTextBox, centerTextBox, leftTextBox, Label3, Label2, Label1})
    Me.MaximizeBox = False
    Me.MinimizeBox = False
    Me.Name = "PropertyForm"
    Me.Text = "Properties"
    Me.ResumeLayout(False)

  End Sub

#End Region

  Public Property Line() As Line
    Get
      Return lineField
    End Get
    Set(ByVal line As Line)

      lineField = line
      RefreshTextBoxes()
    End Set
  End Property

  Private Sub RefreshTextBoxes()
    If lineField Is Nothing Then
      leftTextBox.Enabled = False
      centerTextBox.Enabled = False
      rightTextBox.Enabled = False
    Else
      leftTextBox.Enabled = True
      centerTextBox.Enabled = True
      rightTextBox.Enabled = True
      leftTextBox.Text = lineField.leftText
      centerTextBox.Text = lineField.centerText
      rightTextBox.Text = lineField.rightText

    End If
  End Sub

  Private Sub UpdateProperties(ByVal sender As Object, ByVal e As EventArgs) _
    Handles leftTextBox.LostFocus, centerTextBox.LostFocus, rightTextBox.LostFocus
    If Not lineField Is Nothing Then
      lineField.leftText = leftTextBox.Text
      lineField.centerText = centerTextBox.Text
      lineField.rightText = rightTextBox.Text
      OnPropertyChanged(New EventArgs())
    End If
  End Sub

  Protected Overridable Sub OnPropertyChanged(ByVal e As EventArgs)
    RaiseEvent PropertyChanged(Me, e)
  End Sub

End Class
