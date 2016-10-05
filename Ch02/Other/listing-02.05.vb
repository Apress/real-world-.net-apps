Imports System
Imports System.Windows.Forms
Imports System.ComponentModel

Public Class SingletonForm : Inherits Form
  Private Shared myInstance As SingletonForm
  Private Sub New()
    Me.Text = "Singleton Form"
  End Sub

  Protected Overrides Sub OnCLosing(ByVal e As CancelEventArgs)
    e.Cancel = True
    Me.Hide()
  End Sub

  Public Shared Function GetInstance() As SingletonForm
    If myInstance Is Nothing Then
      myInstance = New SingletonForm()
    End If
    Return myInstance
  End Function
End Class
