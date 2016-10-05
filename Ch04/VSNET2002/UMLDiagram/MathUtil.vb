Option Explicit On 
Option Strict On

Imports System
Imports System.Drawing

Public Class MathUtil
  Public Shared Function GetSquareDistance(ByVal p1 As Point, _
    ByVal p2 As Point, ByVal p3 As Point) As Double
    ' calculate the distance between P3 and the line passing P1 and P2
    Dim aSquare, bSquare, cSquare, d, eSquare As Double
    aSquare = GetSquareDistance(p1, p3)
    bSquare = GetSquareDistance(p2, p3)
    cSquare = GetSquareDistance(p1, p2)
    ' c should not be zero, otherwise it means p1 = p2
    ' if it is so, return the distance of p3 and p1
    If cSquare = 0 Then
      Return Math.Sqrt(aSquare)
    End If

    d = (bSquare - aSquare + cSquare) / (2 * Math.Sqrt(cSquare))
    eSquare = bSquare - d ^ 2
    Return Math.Abs(eSquare)
  End Function

  Private Shared Function GetSquareDistance(ByVal p1 As Point, _
    ByVal p2 As Point) As Double
    Return ((p2.X - p1.X) ^ 2 + (p2.Y - p1.Y) ^ 2)
  End Function

  Public Shared Function GetRectangleFromPoints(ByVal p1 As Point, _
  ByVal p2 As Point) As Rectangle
    Dim x1, x2, y1, y2 As Integer
    If p1.X < p2.X Then
      x1 = p1.X
      x2 = p2.X
    Else
      x1 = p2.X
      x2 = p1.X
    End If

    If p1.Y < p2.Y Then
      y1 = p1.Y
      y2 = p2.Y
    Else
      y1 = p2.Y
      y2 = p1.Y
    End If
    ' x2 > x1 and y2 > y1
    Return New Rectangle(x1, y1, x2 - x1, y2 - y1)

  End Function

End Class
