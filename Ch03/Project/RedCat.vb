Option Explicit On 
Option Strict On

Imports System.Drawing

Public Class RedCat : Inherits Cat

  Public Sub New(ByRef d As Dog)
    MyBase.New(d)
    normalimages(0) = New Bitmap("images/cat/red/red1.gif")
    normalimages(1) = New Bitmap("images/cat/red/red2.gif")
    normalimages(2) = New Bitmap("images/cat/red/red3.gif")
    normalimages(3) = New Bitmap("images/cat/red/red4.gif")
  End Sub

  Public Overrides Sub Init()
    SetPos(12, 13)
    walkdirection = Direction.Up
    image = normalimages(2)
    Timer.Interval = 20000
    Timer.Start()
    attack = True
  End Sub

End Class
