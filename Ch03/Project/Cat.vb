Option Explicit On 
Option Strict On

Imports System
Imports System.Timers
Imports System.Drawing
Imports Microsoft.VisualBasic

Public MustInherit Class Cat : Inherits GameActor
  Private map() As String
  Private delay As Integer
  Public Shared scared As Boolean 'True if situation is reversed

  Shared random As New random()
  Private dog As Dog

  Private scaredImages(2) As Bitmap
  Protected normalImages(4) As Bitmap
  Private normalImageSequence As Integer

  Private deadCat(4) As Bitmap

  Protected state As Integer
  Protected attack As Boolean
  Protected timer As System.Timers.Timer

  Public Sub New(ByVal d As dog)
    scaredImages(0) = New Bitmap("images/cat/scared/1.gif")
    scaredImages(1) = New Bitmap("images/cat/scared/2.gif")

    ' there are four version of images for representing
    ' a dead cat. At this moment all are the same, but
    ' if you want you can create different ones to create
    ' animiation effects
    deadCat(0) = New Bitmap("images/cat/deadCat/dead.gif")
    deadCat(1) = New Bitmap("images/cat/deadCat/dead.gif")
    deadCat(2) = New Bitmap("images/cat/deadCat/dead.gif")
    deadCat(3) = New Bitmap("images/cat/deadCat/dead.gif")

    [step] = 2
    dog = d

    timer = New System.Timers.Timer()
    AddHandler timer.Elapsed, AddressOf SwapAttack
    timer.AutoReset = True

  End Sub

  Public Sub SwapAttack(ByVal o As Object, ByVal e As ElapsedEventArgs)
    attack = Not attack
  End Sub

  Public Sub SetMap(ByVal m() As String)
    map = m
  End Sub

  Public Overrides Sub Draw(ByVal g As Graphics)
    If currentState <> GameState.Lose Then
      g.DrawImage(image, xScreen - 4, yScreen - 4)
    End If
  End Sub

  Public Overrides Sub Update()
    If currentState <> GameState.Run Then
      Return
    End If
    If delay = 1 Then
      state += 1
      state = state Mod 2
    End If
    delay += 1
    delay = delay Mod 3

    If dead Then
      image = deadCat(walkDirection)
      MoveEye()
    Else
      If scared Then
        image = scaredImages(state)
      Else
        image = normalImages(normalImageSequence)
        normalImageSequence = (normalImageSequence + 1) Mod 4

      End If
      MoveCat()
    End If


  End Sub

  Sub MoveEye()
    If map(yScreen \ Maze.square).Chars(xScreen \ Maze.square) = "%"c _
      And (yScreen Mod Maze.square = 0) _
      And (xScreen Mod Maze.square = 0) Then
      'the cat is in the cat house
      dead = False
      RandomWalk()
    ElseIf map(yScreen \ Maze.square). _
      Chars(CInt(xScreen \ Maze.square)) = "J" Then
      WalkToTarget(12 * Maze.square, 13 * Maze.square)
    Else
      If GameManager.MoveRequest(Me, walkDirection, walkDirection) Then
        Return
      End If
      If GameManager.MoveRequest(Me, walkDirection, _
        (walkDirection + 1) Mod 4) Then
        walkDirection = (walkDirection + 1) Mod 4
        Return
      End If
      If GameManager.MoveRequest(Me, walkDirection, _
        (walkDirection + 3) Mod 4) Then
        walkDirection = (walkDirection + 3) Mod 4
        Return
      End If
    End If
  End Sub

  Public Sub MoveCat()
    Dim y As Integer = yScreen \ Maze.square
    If map(y).Chars(xScreen \ Maze.square) = "%"c And _
      yScreen Mod Maze.square = 0 And xScreen Mod Maze.square = 0 Then
      ' cat in cat house
      If (scared) Then
        RandomWalk()
      Else
        'walk to the cat house door (to get out)
        WalkToTarget(12 * Maze.square, 11 * Maze.square)
      End If
    ElseIf map(y).Chars(xScreen \ Maze.square) = "J"c Then
      ' a junction
      If (scared) Then
        RandomWalk()
      ElseIf (attack) Then
        WalkToTarget(dog.xScreen, dog.yScreen)
      Else
        RandomWalk()
      End If
    Else
      'Can I walk straight?
      If (GameManager.MoveRequest(Me, walkDirection, walkDirection)) Then
        Return
      End If

      If (GameManager.MoveRequest(Me, walkDirection, _
        (walkDirection + 1) Mod 4)) Then
        ' make an L-turn is okay
        walkDirection = (walkDirection + 1) Mod 4
        Return
      End If
      If (GameManager.MoveRequest(Me, walkDirection, _
        (walkDirection + 3) Mod 4)) Then
        ' make an L-turn is okay
        walkDirection = (walkDirection + 3) Mod 4
        Return
      End If
    End If
  End Sub

  Public Sub RandomWalk()
    Dim rand As Integer = random.Next(4)
    Dim dirs(4) As Integer
    dirs(0) = rand
    rand = random.Next(1)
    If rand = 0 Then
      dirs(1) = (dirs(0) + 1) Mod 4
      dirs(2) = (dirs(1) + 1) Mod 4
      dirs(3) = (dirs(2) + 1) Mod 4
    Else
      dirs(1) = ((dirs(0) - 1) + 4) Mod 4
      dirs(2) = ((dirs(1) - 1) + 4) Mod 4
      dirs(3) = ((dirs(2) - 1) + 4) Mod 4
    End If
    Walk(dirs)
  End Sub

  Public Sub WalkToTarget(ByVal tx As Integer, ByVal ty As Integer)
    Dim dirs(4) As Integer
    RequestDirection(dirs, tx, ty)
    Walk(dirs)
  End Sub

  Public Sub Walk(ByRef dirs() As Integer)
    'this sub sets the walkDirection value
    Dim ok As Boolean
    If dirs(0) <> ((walkDirection - 2) + 4) Mod 4 Then
      ok = GameManager.MoveRequest(Me, walkDirection, dirs(0))
      If ok Then
        walkDirection = dirs(0)
        Return
      End If
    End If
    If dirs(1) <> ((walkDirection - 2) + 4) Mod 4 Then
      ok = GameManager.MoveRequest(Me, walkDirection, dirs(1))
      If ok Then
        walkDirection = dirs(1)
        Return
      End If
    End If

    If dirs(2) <> ((walkDirection - 2) + 4) Mod 4 Then
      ok = GameManager.MoveRequest(Me, walkDirection, dirs(2))
      If ok Then
        walkDirection = dirs(2)
        Return
      End If
    End If
    If dirs(3) <> ((walkDirection - 2) + 4) Mod 4 Then
      ok = GameManager.MoveRequest(Me, walkDirection, dirs(3))
      If ok Then
        walkDirection = dirs(3)
        Return
      End If
    End If
  End Sub

  Public Sub RequestDirection(ByVal dirs() As Integer, _
    ByVal targetx As Integer, ByVal targety As Integer)
    Dim difx As Integer = targetx - xScreen
    Dim dify As Integer = targety - yScreen

    If Math.Abs(difx) > Math.Abs(dify) Then
      ' CInt is needed because Options Strict is On
      dirs(0) = CInt(IIf(difx > 0, 3, 1))
      dirs(1) = CInt(IIf(dify > 0, 2, 0))
      dirs(2) = CInt(IIf(dify > 0, 0, 2))
      dirs(3) = CInt(IIf(difx > 0, 1, 3))
    Else
      dirs(0) = CInt(IIf(dify > 0, 2, 0))
      dirs(1) = CInt(IIf(difx > 0, 3, 1))
      dirs(2) = CInt(IIf(difx > 0, 1, 3))
      dirs(3) = CInt(IIf(dify > 0, 0, 2))
    End If
  End Sub
End Class
