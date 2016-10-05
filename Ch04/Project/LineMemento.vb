Imports System
Imports System.Drawing
<Serializable()> Public Class LineMemento
  Public index As Integer
  Public lineType As ShapeType
  Public fromRectIndex As Integer
  Public toRectIndex As Integer
  Public StartPoint As Point
  Public EndPoint As Point
  Public selected As Boolean
  Public leftText As String
  Public centerText As String
  Public rightText As String
  Public autoMove As Boolean
  Public fromRectXRelPos As Integer
  Public fromRectYRelPos As Integer
  Public toRectXRelPos As Integer
  Public toRectYRelPos As Integer
  Public fromRectWhichSide As Integer
  Public toRectWhichSide As Integer
End Class
