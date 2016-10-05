Option Explicit On 
Option Strict On

Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections
Imports Microsoft.VisualBasic

Public Class ClassRect : Inherits Rect
  Public attributes As New ArrayList()

  Public Sub New(ByVal startPoint As Point, ByVal endPoint As Point, _
    ByVal index As Integer)
    MyBase.new(startPoint, endPoint, index)
  End Sub

  Public Sub New(ByVal memento As ClassRectMemento)
    MyBase.New(memento.StartPoint, memento.EndPoint, memento.index)
    Me.Name = memento.name
    Me.attributes = memento.attributes
    Me.operations = memento.operations
    Me.selected = memento.selected
  End Sub

  Public ReadOnly Property AttributeCount() As Integer
    Get
      Return attributes.Count
    End Get
  End Property

  Protected Overrides Sub DrawName(ByRef g As Graphics)
    yPos = textTopMargin
    ' center text
    Dim x As Integer = _
      CInt((Me.Width - g.MeasureString(Me.Name, Font).Width) / 2)
    If x > xpos Then
      g.DrawString(Name, Font, textBrush, x, yPos)
    Else
      g.DrawString(Name, Font, textBrush, xPos, yPos)
    End If
    yPos += fontHeight
  End Sub

  Protected Overrides Sub DrawMembers(ByRef g As Graphics)
    'draw attributes here
    Dim attrEnum As IEnumerator = attributes.GetEnumerator
    While attrEnum.MoveNext
      Dim attribute As String = CType(attrEnum.Current, String)
      g.DrawString(attribute, Font, textBrush, xpos, ypos)
      yPos += fontHeight
    End While
    'draw the line that partition the attributes and operations
    yPos += fontHeight
    If selected Then
      Dim x1 As Integer = CInt(handleWidth / 2)
      Dim x2 As Integer = x1 + Me.Width - handleWidth
      g.DrawLine(foregroundPen, x1, yPos, x2, yPos)
    Else
      Dim x1 As Integer = 0
      Dim x2 As Integer = x1 + Me.Width
      g.DrawLine(foregroundPen, x1, yPos, x2, yPos)
    End If
    'draw operations
    Dim opsEnum As IEnumerator = operations.GetEnumerator
    While opsEnum.MoveNext
      Dim operation As String = CType(opsEnum.Current, String)
      g.DrawString(operation, Font, textBrush, xpos, ypos)
      yPos += fontHeight
    End While

  End Sub

  Public Function GetMemento() As ClassRectMemento
    Dim memento As New ClassRectMemento()
    memento.index = Me.index
    memento.name = Me.Name
    memento.attributes = Me.attributes
    memento.operations = Me.operations
    memento.StartPoint = Me.StartPoint
    memento.EndPoint = Me.EndPoint
    memento.selected = Me.selected
    Return memento
  End Function
End Class
