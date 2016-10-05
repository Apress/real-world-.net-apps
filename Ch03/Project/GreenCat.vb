Option Explicit On 
Option Strict On

Imports System.Drawing

Public Class GreenCat : Inherits Cat

  Public Sub New(ByRef d As Dog)
    MyBase.New(d)
    normalimages(0) = New Bitmap("images/cat/green/green1.gif")
    normalimages(1) = New Bitmap("images/cat/green/green2.gif")
    normalimages(2) = New Bitmap("images/cat/green/green3.gif")
    normalimages(3) = New Bitmap("images/cat/green/green4.gif")
  End Sub

  Public Overrides Sub Init()
    SetPos(14, 15)
    walkdirection = Direction.Right
    image = normalimages(3)
    Timer.Interval = 8000
    Timer.Start()
    attack = False
  End Sub

End Class
