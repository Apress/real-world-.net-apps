Option Strict On
Option Explicit On 

Public Structure ColumnLine
    Public Column As Integer
    Public Line As Integer
    Public Sub New(ByVal column As Integer, ByVal line As Integer)
        Me.Column = column
        Me.Line = line
    End Sub
End Structure
