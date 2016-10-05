Option Explicit On 
Option Strict On

Imports System.Drawing

Class BlackCat : Inherits Cat

  Public Sub New(ByRef d As Dog)
    MyBase.New(d)
    normalimages(0) = New Bitmap("images/cat/black/black1.gif")
    normalimages(1) = New Bitmap("images/cat/black/black2.gif")
    normalimages(2) = New Bitmap("images/cat/black/black3.gif")
    normalimages(3) = New Bitmap("images/cat/black/black4.gif")

  End Sub

  Public Overrides Sub Init()
    SetPos(14, 13)
    walkdirection = Direction.Down
    image = normalimages(1)
    Timer.Interval = 2000
    Timer.Start()
    attack = True
  End Sub

End Class
