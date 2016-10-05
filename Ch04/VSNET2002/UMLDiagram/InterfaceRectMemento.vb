Imports System
Imports System.Drawing
Imports System.Collections

<Serializable()> Public Class InterfaceRectMemento
  Public index As Integer
  Public name As String
  Public StartPoint As Point
  Public EndPoint As Point
  Public selected As Boolean = True
  Public operations As ArrayList
End Class
