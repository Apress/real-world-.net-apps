Option Explicit On 
Option Strict On

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

Public Class LoginForm
  Inherits Form

  Private WithEvents okButton As Button
  Private userNameField, passwordField, serverField As String
  Private cnlButton As System.Windows.Forms.Button

  Private label1 As New Label()
  Private label2 As New Label()
  Private label3 As New Label()

  Public ReadOnly Property Server() As String
    Get
      Return serverField
    End Get
  End Property

  Public ReadOnly Property UserName() As String
    Get
      Return userNameField
    End Get
  End Property

  Public ReadOnly Property Password() As String
    Get
      Return passwordField
    End Get
  End Property


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
  Private userTextBox As System.Windows.Forms.TextBox
  Private passwordTextBox As System.Windows.Forms.TextBox
  Private serverTextBox As System.Windows.Forms.TextBox

  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Me.userTextBox = New System.Windows.Forms.TextBox()
    Me.passwordTextBox = New System.Windows.Forms.TextBox()
    Me.serverTextBox = New System.Windows.Forms.TextBox()
    Me.label1 = New System.Windows.Forms.Label()
    Me.label2 = New System.Windows.Forms.Label()
    Me.label3 = New System.Windows.Forms.Label()
    Me.okButton = New System.Windows.Forms.Button()
    Me.cnlButton = New System.Windows.Forms.Button()
    Me.SuspendLayout()
    '
    'userTextBox
    '
    Me.userTextBox.Location = New System.Drawing.Point(112, 48)
    Me.userTextBox.Name = "userTextBox"
    Me.userTextBox.Size = New System.Drawing.Size(240, 20)
    Me.userTextBox.TabIndex = 1
    Me.userTextBox.Text = ""
    '
    'passwordTextBox
    '
    Me.passwordTextBox.Location = New System.Drawing.Point(112, 80)
    Me.passwordTextBox.Name = "passwordTextBox"
    Me.passwordTextBox.PasswordChar = Microsoft.VisualBasic.ChrW(42)
    Me.passwordTextBox.Size = New System.Drawing.Size(240, 20)
    Me.passwordTextBox.TabIndex = 2
    Me.passwordTextBox.Text = ""
    '
    'serverTextBox
    '
    Me.serverTextBox.Location = New System.Drawing.Point(112, 16)
    Me.serverTextBox.Name = "serverTextBox"
    Me.serverTextBox.Size = New System.Drawing.Size(240, 20)
    Me.serverTextBox.TabIndex = 0
    Me.serverTextBox.Text = ""
    '
    'label1
    '
    Me.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
    Me.label1.Location = New System.Drawing.Point(8, 48)
    Me.label1.Name = "label1"
    Me.label1.Size = New System.Drawing.Size(88, 16)
    Me.label1.TabIndex = 4
    Me.label1.Text = "User Name"
    '
    'label2
    '
    Me.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
    Me.label2.Location = New System.Drawing.Point(8, 80)
    Me.label2.Name = "label2"
    Me.label2.Size = New System.Drawing.Size(88, 16)
    Me.label2.TabIndex = 5
    Me.label2.Text = "Password"
    '
    'label3
    '
    Me.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
    Me.label3.Location = New System.Drawing.Point(8, 16)
    Me.label3.Name = "label3"
    Me.label3.Size = New System.Drawing.Size(88, 16)
    Me.label3.TabIndex = 6
    Me.label3.Text = "Server"
    '
    'okButton
    '
    Me.okButton.DialogResult = System.Windows.Forms.DialogResult.OK
    Me.okButton.Location = New System.Drawing.Point(168, 112)
    Me.okButton.Name = "okButton"
    Me.okButton.Size = New System.Drawing.Size(88, 24)
    Me.okButton.TabIndex = 8
    Me.okButton.Text = "OK"
    '
    'cnlButton
    '
    Me.cnlButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.cnlButton.Location = New System.Drawing.Point(264, 112)
    Me.cnlButton.Name = "cnlButton"
    Me.cnlButton.Size = New System.Drawing.Size(88, 24)
    Me.cnlButton.TabIndex = 9
    Me.cnlButton.Text = "Cancel"
    '
    'LoginForm
    '
    Me.AcceptButton = Me.okButton
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.CancelButton = Me.cnlButton
    Me.ClientSize = New System.Drawing.Size(360, 141)
    Me.ControlBox = False
    Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.cnlButton, Me.okButton, Me.label3, Me.label2, Me.label1, Me.serverTextBox, Me.passwordTextBox, Me.userTextBox})
    Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
    Me.MaximizeBox = False
    Me.MinimizeBox = False
    Me.Name = "LoginForm"
    Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
    Me.Text = "Login Form"
    Me.ResumeLayout(False)

  End Sub

#End Region

  '
  ' event handlers
  '
  Private Sub okButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles okButton.Click
    userNameField = userTextBox.Text
    passwordField = passwordTextBox.Text
    serverField = serverTextBox.Text
    Me.Close()
  End Sub

End Class