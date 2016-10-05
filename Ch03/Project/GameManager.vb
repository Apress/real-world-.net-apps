Option Explicit On 
Option Strict On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Timers

Class GameManager
  Private lives As Integer = Constants.lives  ' the number of Dog's
  Private level As Integer ' game level
  Private currentState As GameState

  Private f18, f16, f14 As Font
  Private white, green, red As SolidBrush

  Private theMaze As Maze
  Private dog As Dog
  Private cats(Constants.catCount - 1) As Cat

  Private boardImage, topImage, bottomImage, dogImage As Bitmap
  Private timer As System.Timers.Timer
  Private Shared gameMgr As GameManager
  Public Shared Height, Width As Integer

  Public Sub New()
    dog = New Dog()
    cats(0) = New RedCat(dog)
    cats(1) = New BlueCat(dog)
    cats(2) = New BlackCat(dog)
    cats(3) = New GreenCat(dog)

    theMaze = New Maze(dog, cats)

    Const topImageHeight As Integer = 25
    Const bottomImageHeight As Integer = 30

    topImage = New Bitmap(theMaze.Width, topImageHeight)
    boardImage = New Bitmap(theMaze.Width, theMaze.Height)
    bottomImage = New Bitmap(theMaze.Width, bottomImageHeight)

    Height = theMaze.Height + topImageHeight + bottomImageHeight
    Width = theMaze.Width

    timer = New System.Timers.Timer()
    AddHandler timer.Elapsed, AddressOf OnTimedEvent
    timer.AutoReset = False

    f14 = New Font("Arial", 14)
    f16 = New Font("Arial", 16)
    f18 = New Font("Arial", 18)

    green = New SolidBrush(Color.Green)
    white = New SolidBrush(Color.White)
    red = New SolidBrush(Color.Red)

    dogImage = New Bitmap("images/dog/right2.gif")
  End Sub

  Public Shared Sub SetState(ByVal state As GameState)
    If state = GameState.Init Then
      gameMgr = New GameManager()
    End If

    gameMgr.currentState = state
    gameMgr.theMaze.SetState(state)
    gameMgr.dog.SetState(state)
    Dim i As Integer
    For i = 0 To Constants.catCount - 1
      gameMgr.cats(i).SetState(state)
    Next

    Select Case state
      Case GameState.Init
        SetState(GameState.[New])
      Case GameState.[New]
        gameMgr.level = 1
        gameMgr.lives -= 1
      Case GameState.Begin
        gameMgr.timer.Interval = 2000
        gameMgr.timer.Start()
      Case GameState.Win
        gameMgr.timer.Interval = 2000
        gameMgr.timer.Start()
      Case GameState.Lose
        gameMgr.timer.Interval = 2000
        gameMgr.timer.Start()
      Case GameState.GameOver
        gameMgr.timer.Interval = 2000
        gameMgr.timer.Start()
    End Select
  End Sub

  Public Shared Sub Draw(ByRef g As Graphics)
    ' --- Drawing the top part ---
    Dim gTemp As Graphics = Graphics.FromImage(gameMgr.topImage)
    gTemp.Clear(Color.Black)
    gTemp.DrawString("Score: " & gameMgr.theMaze.score * 10, _
      gameMgr.f14, gameMgr.white, 10, 0)

    Select Case gameMgr.currentState
      Case GameState.[New]
        gTemp.DrawString("Please press 'Enter' to play.", _
          gameMgr.f14, gameMgr.white, 150, 0)
      Case GameState.Begin
        gTemp.DrawString("Ready!", gameMgr.f16, gameMgr.white, 150, 0)
      Case GameState.Run
        gameMgr.theMaze.CheckCollision()
        If gameMgr.dog.dead Then
          SetState(GameState.Lose)
        ElseIf gameMgr.theMaze.food = 0 Then
          SetState(GameState.Win)
        End If
      Case GameState.Win
        gTemp.DrawString("YOU WIN!!!", gameMgr.f16, gameMgr.green, 150, 0)
      Case GameState.Lose
        gTemp.DrawString("YOU LOSE!!!", gameMgr.f16, gameMgr.red, 150, 0)
      Case GameState.GameOver
        gTemp.DrawString("GAME OVER", gameMgr.f18, gameMgr.red, 150, 0)
    End Select

    gTemp.Dispose()

    ' --- Drawing the maze ---
    gTemp = Graphics.FromImage(gameMgr.boardImage)
    gameMgr.theMaze.Draw(gTemp)
    gTemp.Dispose()

    ' --- Drawing the bottom part ---
    gTemp = Graphics.FromImage(gameMgr.bottomImage)
    gTemp.Clear(Color.Black)

    gTemp.DrawString("Level - " & gameMgr.level, gameMgr.f14, _
      gameMgr.white, 10, 0)

    Dim i As Integer
    For i = 0 To gameMgr.lives - 1
      gTemp.DrawImage(gameMgr.dogImage, (Width - 10) - (i + 1) * 24, 0, 24, 24)
    Next
    gTemp.Dispose()

    ' putting all three parts together
    g.DrawImage(gameMgr.topImage, 0, 0)
    g.DrawImage(gameMgr.boardImage, 0, 25)
    g.DrawImage(gameMgr.bottomImage, 0, gameMgr.theMaze.Height + 25)
  End Sub

  Public Shared Sub OnTimedEvent(ByVal o As Object, ByVal e As ElapsedEventArgs)
    Select Case gameMgr.currentState
      Case GameState.Begin
        SetState(GameState.Run)
      Case GameState.Win
        gameMgr.level += 1
        SetState(GameState.Begin)
      Case GameState.Lose
        If gameMgr.lives = 0 Then
          SetState(GameState.GameOver)
        Else
          gameMgr.lives -= 1
          SetState(GameState.Begin)
        End If
      Case GameState.GameOver
        SetState(GameState.[New])
    End Select
  End Sub

  Public Shared Sub KeyDown(ByVal o As Object, ByVal e As KeyEventArgs)
    If Not gameMgr Is Nothing Then
      If gameMgr.currentState = GameState.Run Then
        gameMgr.dog.KeyDown(o, e)
      ElseIf gameMgr.currentState = GameState.[New] And _
        e.KeyCode = Keys.Enter Then
        SetState(GameState.Begin)
      End If
    End If
  End Sub

  Public Shared Function MoveRequest(ByRef actor As GameActor, _
    ByVal oldDirection As Integer, ByVal dir As Integer) As Boolean
    Return gameMgr.theMaze.MoveRequest(actor, oldDirection, dir)
  End Function

End Class

