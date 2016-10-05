Public Class Dog : Inherits GameActor
  Private anim As Integer
  Private images(3, 4) As Bitmap
  Private movement, turnDirection As Integer

  Public Sub New()
    images(0, 0) = New Bitmap("images/dog/up1.gif")
    images(0, 1) = New Bitmap("images/dog/left1.gif")
    images(0, 2) = New Bitmap("images/dog/down1.gif")
    images(0, 3) = New Bitmap("images/dog/right1.gif")

    images(1, 0) = New Bitmap("images/dog/up2.gif")
    images(1, 1) = New Bitmap("images/dog/left2.gif")
    images(1, 2) = New Bitmap("images/dog/down2.gif")
    images(1, 3) = New Bitmap("images/dog/right2.gif")

    images(2, 0) = New Bitmap("images/dog/up3.gif")
    images(2, 1) = New Bitmap("images/dog/left3.gif")
    images(2, 2) = New Bitmap("images/dog/down3.gif")
    images(2, 3) = New Bitmap("images/dog/right3.gif")

    [step] = 2
  End Sub

  Public Overrides Sub Draw(ByVal g As Graphics)
    g.DrawImage(image, xScreen - 4, yScreen - 4)
  End Sub

  Public Overrides Sub Init()
    SetPos(13, 23)
    movement = 0
    turnDirection = Direction.Invalid
    image = images(0, 1)
    dead = False
  End Sub


  Public Overrides Sub Update()
    If currentState = GameState.Lose Then
      image = images(2, anim)
      anim += 1
      anim = anim Mod 4
      Return
    End If

    If currentState <> GameState.Run Then
      Return
    End If

    If turnDirection = Direction.Invalid Then
      Return
    End If

    Dim ok As Boolean = GameManager.MoveRequest(Me, _
      walkDirection, turnDirection)
    If ok Then
      walkDirection = turnDirection
    Else
      GameManager.MoveRequest(Me, walkDirection, walkDirection)
    End If

    'choose the image.
    image = images(movement, walkDirection)
    movement += 1
    movement = movement Mod 3
  End Sub

  Public Sub KeyDown(ByVal o As Object, ByVal e As KeyEventArgs)
    Select Case e.KeyCode
      Case Keys.Up
        turnDirection = Direction.Up
      Case Keys.Down
        turnDirection = Direction.Down
      Case Keys.Left
        turnDirection = Direction.Left
      Case Keys.Right
        turnDirection = Direction.Right
    End Select
  End Sub

End Class
