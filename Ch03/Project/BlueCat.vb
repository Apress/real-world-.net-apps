Option Explicit On 
Option Strict On

Imports System.Drawing

Class BlueCat : Inherits Cat

  Public Sub New(ByRef d As Dog)
    MyBase.New(d)
    normalImages(0) = New Bitmap("images/cat/blue/blue1.gif")
    normalimages(1) = New Bitmap("images/cat/blue/blue2.gif")
    normalimages(2) = New Bitmap("images/cat/blue/blue3.gif")
    normalimages(3) = New Bitmap("images/cat/blue/blue4.gif")
  End Sub

  Public Overrides Sub Init()
    SetPos(11, 15)
    walkDirection = Direction.Left
    image = normalImages(0)
    Timer.Interval = 25000
    Timer.Start()
    attack = False
  End Sub

End Class
