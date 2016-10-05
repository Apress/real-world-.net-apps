Option Explicit On 
Option Strict On

Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections
Imports Microsoft.VisualBasic

Public Class InterfaceRect : Inherits Rect

  Public Sub New(ByVal startPoint As Point, ByVal endPoint As Point, _
    ByVal index As Integer)
    MyBase.New(startPoint, endPoint, index)
  End Sub

  Public Sub New(ByVal memento As InterfaceRectMemento)
    MyBase.New(memento.StartPoint, memento.EndPoint, memento.index)
    Me.Name = memento.name
    Me.operations = memento.operations
    Me.selected = memento.selected
  End Sub

  Protected Overrides Sub DrawName(ByRef g As Graphics)
    yPos = textTopMargin
    'draw prefix here
    Dim s As String = "<<Interface>>"
    Dim x As Integer = _
      CInt((Me.Width - g.MeasureString(s, Font).Width) / 2)
    If x > xpos Then
      g.DrawString(s, Font, textBrush, x, yPos)
    Else
      g.DrawString(s, Font, textBrush, xPos, yPos)
    End If

    yPos += fontHeight
    'draw name
    ' center text
    x = CInt((Me.Width - g.MeasureString(Me.Name, Font).Width) / 2)
    If x > xpos Then
      g.DrawString(Name, Font, textBrush, x, yPos)
    Else
      g.DrawString(Name, Font, textBrush, xPos, yPos)
    End If
    yPos += fontHeight
  End Sub

  Protected Overrides Sub DrawMembers(ByRef g As Graphics)
    'draw operations here
    'draw operations
    Dim opsEnum As IEnumerator = operations.GetEnumerator
    While opsEnum.MoveNext
      Dim operation As String = CType(opsEnum.Current, String)
      g.DrawString(operation, Font, textBrush, xpos, ypos)
      yPos += fontHeight
    End While
  End Sub

  Public Function GetMemento() As InterfaceRectMemento
    Dim memento As New InterfaceRectMemento()
    memento.index = Me.index
    memento.name = Me.Name
    memento.operations = Me.operations
    memento.StartPoint = Me.StartPoint
    memento.EndPoint = Me.EndPoint
    memento.selected = Me.selected
    Return memento
  End Function

End Class
