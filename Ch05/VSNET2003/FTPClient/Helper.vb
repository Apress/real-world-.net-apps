Option Strict On
Option Explicit On 

Imports System.Windows.Forms
Imports System.IO

Public Class Helper

  Public Shared Function IsDirectory(ByVal path As String) As Boolean
    If File.Exists(path) Or Directory.Exists(path) Then
      Dim attr As FileAttributes = File.GetAttributes(path)
      If (attr And FileAttributes.Directory) = FileAttributes.Directory Then
        Return True
      End If
    End If
  End Function

  Public Shared Function IsDirectoryItem(ByVal item As ListViewItem) As Boolean
    If item.ImageIndex = 1 Then
      Return True
    Else
      Return False
    End If
  End Function
End Class
