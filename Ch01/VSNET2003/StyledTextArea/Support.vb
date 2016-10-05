Option Strict On
Option Explicit On 

Public Delegate Sub ColumnEventHandler(ByVal sender As Object, _
  ByVal e As ColumnEventArgs)

Public Class ColumnEventArgs : Inherits EventArgs
    Private oldColumnField, newColumnField As Integer

    Public Sub New(ByVal oldColumn As Integer, ByVal newColumn As Integer)
        oldColumnField = oldColumn
        newColumnField = newColumn
    End Sub

    Public ReadOnly Property OldColumn() As Integer
        Get
            Return oldColumnField
        End Get
    End Property

    Public ReadOnly Property NewColumn() As Integer
        Get
            Return newColumnField
        End Get
    End Property
End Class

Public Delegate Sub LineEventHandler(ByVal sender As Object, _
  ByVal e As LineEventArgs)

Public Class LineEventArgs : Inherits EventArgs
    Private oldLineField, newLineField As Integer

    Public Sub New(ByVal oldLine As Integer, ByVal newLine As Integer)
        oldLineField = oldLine
        newLineField = newLine
    End Sub

    Public ReadOnly Property OldLine() As Integer
        Get
            Return oldLineField
        End Get
    End Property

    Public ReadOnly Property NewLine() As Integer
        Get
            Return newLineField
        End Get
    End Property
End Class


Public Delegate Sub LineCountEventHandler(ByVal sender As Object, _
  ByVal e As LineCountEventArgs)

Public Class LineCountEventArgs : Inherits EventArgs
    Private lineCountField As Integer ' the current line count

    Public Sub New(ByVal lineCount As Integer)
        lineCountField = lineCount
    End Sub

    Public ReadOnly Property LineCount() As Integer
        Get
            Return lineCountField
        End Get
    End Property
End Class

Public Delegate Sub LongestLineEventHandler(ByVal sender As Object, _
  ByVal e As LongestLineEventArgs)

Public Class LongestLineEventArgs : Inherits EventArgs
    Private charCountField As Integer

    Public Sub New(ByVal charCount As Integer)
        charCountField = charCount
    End Sub

    Public ReadOnly Property LongestLineCharCount() As Integer
        Get
            Return charCountField
        End Get
    End Property
End Class
