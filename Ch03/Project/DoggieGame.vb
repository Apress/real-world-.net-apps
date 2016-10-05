Option Explicit On 
Option Strict On
Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Threading
Imports System.Timers
Imports Microsoft.VisualBasic

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

Public Class Constants
  Public Const catCount As Integer = 4 'don't change this
  Public Const lives As Integer = 3 ' the number of Dog's
End Class

Public Enum GameState As Integer
  Init
  [New]
  Begin
  Run
  Win
  Lose
  GameOver
  [Stop]
End Enum

Public Enum Direction As Integer
  Up = 0
  Left = 1
  Down = 2
  Right = 3
  Invalid = -1
End Enum

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


Public Class Form1
  Inherits System.Windows.Forms.Form

  Private game As Thread

  Public Sub New()
    InitializeComponent()
  End Sub

  Public Sub InitializeComponent()
    Dim mainMenu As New MainMenu()
    Dim gameMenuItem As New MenuItem("&Game")

    Dim gameNewGameMenuItem As New MenuItem("&New Game", _
      New EventHandler(AddressOf gameNewGameMenuItem_Click))
    Dim gameExitMenuItem As New MenuItem("E&xit", _
      New EventHandler(AddressOf gameExitMenuItem_Click))

    gameMenuItem.MenuItems.Add(gameNewGameMenuItem)
    gameMenuItem.MenuItems.Add(gameExitMenuItem)

    Dim aboutMenuItem As New MenuItem("&About", _
      New EventHandler(AddressOf aboutMenuItem_Click))

    mainMenu.MenuItems.Add(gameMenuItem)
    mainMenu.MenuItems.Add(aboutMenuItem)

    Me.Menu = mainMenu
    Me.MaximizeBox = False
    Me.FormBorderStyle = FormBorderStyle.Fixed3D
    Me.Text = "Doggie"
    GameManager.SetState(GameState.Init)
    Me.ClientSize = New Size(GameManager.Width, GameManager.Height)
    game = New Thread(New ThreadStart(AddressOf Run))
    AddHandler Me.KeyDown, AddressOf DetectKeyDown

    game.Start()
  End Sub

  Public Sub aboutMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
    Dim aboutBox As New Form()
    Dim okButton As New Button()
    Dim imageLabel As New Label()
    Dim textLabel As New Label()

    aboutBox.ClientSize = New Size(320, 150)
    aboutBox.FormBorderStyle = FormBorderStyle.FixedDialog
    aboutBox.Text = "About This Game"
    aboutBox.MaximizeBox = False
    aboutBox.MinimizeBox = False
    aboutBox.CancelButton = okButton
    aboutBox.StartPosition = FormStartPosition.CenterScreen
    Dim img As Image = Image.FromFile("images/about.gif")
    imageLabel.Image = img
    imageLabel.Size = New Size(img.Width, img.Height)
    imageLabel.Location = New Point(25, 20)
    okButton.Text = "OK"
    textLabel.Text = "Doggie is a Pacman clone." 

    textLabel.Width = aboutBox.ClientSize.Width
    textLabel.Height = aboutBox.ClientSize.Height - okButton.Height - 10
    textLabel.Location = New Point(50 + img.Width, 20)
    okButton.Location = New Point(aboutBox.ClientSize.Width - _
      okButton.Width - 1, aboutBox.ClientSize.Height - okButton.Height - 10)

    aboutBox.Controls.Add(okButton)
    aboutBox.Controls.Add(textLabel)
    aboutBox.Controls.Add(imageLabel)
    aboutBox.ShowDialog()
  End Sub

  Public Sub gameNewGameMenuItem_Click(ByVal sender As Object, _
    ByVal e As EventArgs)
    GameManager.SetState(GameState.[New])
  End Sub

  Public Sub gameExitMenuItem_Click(ByVal sender As Object, _
    ByVal e As EventArgs)
    Me.Close()
  End Sub

  Protected Overrides Sub OnClosed(ByVal e As EventArgs)
    GameManager.SetState(GameState.Stop)
    If game.ThreadState = ThreadState.Suspended Then
      game.Resume()
    End If
    game.Abort()
    game.Join()
    MyBase.OnClosed(e)
  End Sub

  Protected Sub DetectKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
    If e.KeyCode = Keys.Space Then
      If game.ThreadState = ThreadState.Suspended Then
        game.Resume()
      ElseIf game.ThreadState = ThreadState.Running Then
        game.Suspend()
      End If
    Else
      GameManager.KeyDown(sender, e)
    End If
  End Sub

  Public Sub Run()
    Thread.Sleep(100)
    While True
      ' the use of t1 and t2 below is to make the transition between
      ' frames smooth
      Dim t1 As Integer = Environment.TickCount
      Dim g As Graphics = Me.CreateGraphics()
      GameManager.Draw(g)
      Dim t2 As Integer = Environment.TickCount
      g.Dispose()
      Thread.Sleep(Math.Max(0, 30 - (t2 - t1)))
    End While
  End Sub

  <STAThread()> Shared Sub Main()
    Application.Run(New Form1())
  End Sub

End Class



Public MustInherit Class GameActor

  Public frameReady As Boolean = True
  Public dead As Boolean

  Public image As Bitmap

  ' the thread that moves the actor
  Private thread As thread

  Public xScreen, yScreen As Integer
  Public oldXScreen, oldYScreen As Integer
  Protected walkDirection As Integer


  Public [step] As Integer

  Protected currentState As GameState

  Public MustOverride Sub Draw(ByVal g As Graphics)
  Public MustOverride Sub Update()
  Public MustOverride Sub Init()

  Public Sub New()
    thread = New Thread(New ThreadStart(AddressOf Run))
  End Sub

  Public Sub SetState(ByVal state As GameState)
    Select Case state
      Case GameState.Init
        thread.Start()
        Init()
      Case GameState.Begin
        Init()
      Case GameState.[Stop]
        thread.Abort()
    End Select
    currentState = state

  End Sub

  Private Sub Run()
    While True
      SyncLock Me
        If frameReady Then
          Try
            Monitor.Wait(Me)
          Catch e As SynchronizationLockException
          Catch e As ThreadInterruptedException
          End Try
        End If

        Update()
        frameReady = True
        Monitor.Pulse(Me)
      End SyncLock
    End While
  End Sub

  Public Sub Move(ByVal xoff As Integer, ByVal yoff As Integer)
    oldXScreen = xScreen
    oldYScreen = yScreen

    xScreen += xoff
    yScreen += yoff
  End Sub

  Public Sub SetPos(ByVal x As Integer, ByVal y As Integer)
    xScreen = x * Maze.square
    yScreen = y * Maze.square

  End Sub

End Class



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




Class Maze

  Public frameReady As Boolean = True
  Public Width, Height As Integer
  Private currentState As GameState
  Private doorClosed As Boolean

  Private reverse As Boolean
  Public food As Integer
  Public score As Integer
  Private cats(Constants.catCount - 1) As Cat
  Private dog As Dog
  Private image As Bitmap
  Private images(8) As Bitmap
  Private foodBrush, emptyBrush, superBrush As SolidBrush
  Private timer As System.Timers.Timer
  Private rebuild As Boolean = False

  Public Shared square As Integer = 16
  Public Const mazeColumnCount As Integer = 26
  Public Const mazeRowCount As Integer = 31
  Private mazeData(mazeRowCount) As String

  Private maze() As String = { _
    "a-----------ba-----------b", _
    "|***********||***********|", _
    "|*a-------b*||*a--b*a--b*|", _
    "|*|||||||||*||*||||*||||*|", _
    "|*d-------c*dc*d--c*d--c*|", _
    "|************************|", _
    "|*a-b*ab*a------b*ab*a-b*|", _
    "|*d-c*||*d--ba--c*||*d-c*|", _
    "|$****||****||****||****$|", _
    "d---b*|d--b*||*a--c|*a---c", _
    "|||||*|a--c*dc*d--b|*|||||", _
    "|||||*||          ||*|||||", _
    "|||||*|| a--##--b ||*|||||", _
    "----c*dc |%%%%%%| dc*d----", _
    "<    *   |%%%%%%|   *    >", _
    "----b*ab |%%%%%%| ab*a----", _
    "|||||*|| d------c dc*|||||", _
    "|||||*||          ***|||||", _
    "|||||*|| a------b ab*|||||", _
    "a---c*dc d--ba--c dc*d---b", _
    "|***********||***********|", _
    "|*a-b*a---b*||*a---b*a-b*|", _
    "|$db|*d---c*dc*d---c*|ac$|", _
    "|**||****************||**|", _
    "db*||*ab*a------b*ab*||*ac", _
    "ac*dc*||*d--ba--c*||*dc*db", _
    "|*****||****||****||*****|", _
    "|*a---cd--b*||*a--cd---b*|", _
    "|*d-------c*dc*d-------c*|", _
    "|************************|", _
    "d------------------------c" _
  }

  Private map() As String = { _
    "+-----------++-----------+", _
    "|    J      ||     J     |", _
    "| a-------b || a--b a--b |", _
    "| | | |   | || |  | |  | |", _
    "| d-------c dc d--c d--c |", _
    "|J   J  J  J  J  J JJ   J|", _
    "| a-b ab a------b ab a-b |", _
    "| d-c || d--ba--c || d-c |", _
    "|    J||    ||    ||J    |", _
    "d---b |d--b || a--c| a---c", _
    "    | |a--c dc d--b| |    ", _
    "    | ||   JJJJ   || |    ", _
    "    | || a--##--b || |    ", _
    "----c dc |%%%%%%| dc d----", _
    "     J  J|%%%%%%|J  J     ", _
    "----b ab |%%%%%%| ab a----", _
    "    | || d------c || |    ", _
    "    | ||J        J  J|    ", _
    "    | || a------b || |    ", _
    "a---c dc d--ba--c dc d---b", _
    "|    J  J   ||   J  J    |", _
    "| a-b a---b || a---b a-b |", _
    "| db| d---c dc d---c |ac |", _
    "|  ||J  J  J  J  J  J||  |", _
    "db || ab a------b ab || ac", _
    "ac dc || d--ba--c || dc db", _
    "| J   ||    ||    ||   J |", _
    "| a---cd--b || a--cd---b |", _
    "| d-------c dc d-------c |", _
    "|          J  J          |", _
    "d------------------------c" _
  }


  Public Sub New(ByRef d As dog, ByRef g() As Cat)
    dog = d
    cats = g
    images(0) = New Bitmap("images/maze/horz.gif")
    images(1) = New Bitmap("images/maze/vert.gif")
    images(2) = New Bitmap("images/maze/topleft.gif")
    images(3) = New Bitmap("images/maze/topright.gif")
    images(4) = New Bitmap("images/maze/downright.gif")
    images(5) = New Bitmap("images/maze/downleft.gif")
    images(6) = New Bitmap("images/maze/door1.gif")
    images(7) = New Bitmap("images/maze/bone.gif")

    Width = mazeColumnCount * square
    Height = mazeRowCount * square

    image = New Bitmap(Width, Height)
    foodBrush = New SolidBrush(Color.Black)
    emptyBrush = New SolidBrush(Color.White)

    timer = New System.Timers.Timer()
    AddHandler timer.Elapsed, AddressOf ReverseExpired
    timer.AutoReset = False
  End Sub

  Public Sub SetState(ByVal state As GameState)
    Select Case state
      Case GameState.Begin
        InitActors()
        If currentState = GameState.Win Then
          rebuild = True
        End If
      Case GameState.[New]
        score = 0
        doorClosed = False
        InitActors()
        rebuild = True
    End Select
    currentState = state
  End Sub

  Public Sub ReverseExpired(ByVal o As Object, ByVal e As ElapsedEventArgs)
    doorClosed = False
    reverse = False
    dog.step = 2
    Cat.scared = False
  End Sub

  Public Sub InitActors()
    dog.Init()
    Dim i As Integer
    For i = 0 To Constants.catCount - 1
      cats(i).Init()
      cats(i).SetMap(map)
    Next
  End Sub

  Private Sub DrawMaze()
    food = 0
    Dim g As Graphics = Graphics.FromImage(image)
    Dim i As Integer
    For i = 0 To mazeRowCount - 1
      ' copy the ith string from maze to mazeData
      mazeData(i) = maze(i)
      Dim j As Integer
      For j = 0 To mazeColumnCount - 1
        If mazeData(i).Chars(j) = "*" Or _
          mazeData(i).Chars(j) = "$" Then
          food += 1
        End If
        DrawCell(j, i, g)
      Next
    Next
    g.Dispose()

  End Sub

  Public Sub Draw(ByRef g As Graphics)
    If rebuild Then
      reverse = False
      DrawMaze()
      rebuild = False
      g.Clear(Color.Black)
      g.DrawImage(image, 0, 0)
    End If

    RemoveActor(CType(dog, GameActor), g)
    Dim i As Integer
    For i = 0 To Constants.catCount - 1
      RemoveActor(CType(cats(i), GameActor), g)
    Next
    DrawActor(CType(dog, GameActor), g)
    For i = 0 To Constants.catCount - 1
      DrawActor(CType(cats(i), GameActor), g)
    Next
  End Sub

  Private Sub RemoveActor(ByRef a As GameActor, ByRef g As Graphics)
    Dim x As Integer = a.oldXScreen \ square
    Dim y As Integer = a.oldYScreen \ square

    If x = 0 Or y = 0 Then
      Return
    End If

    Dim actualX As Integer = a.oldXScreen - 4
    Dim actualY As Integer = a.oldYScreen - 4

    Dim i As Integer = actualX \ square
    Dim j As Integer = actualY \ square

    Dim upperBound As Integer = (actualX + 24) \ square

    While i <= upperBound
      For j = actualY \ square To (actualY + 24) \ square
        DrawCell(i, j, g)
      Next
      i += 1
    End While
  End Sub

  Sub DrawCell(ByVal x As Integer, ByVal y As Integer, ByRef g As Graphics)
    Dim value As Char = mazeData(y).Chars(x)

    Select Case value
      Case " "c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
      Case "*"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
        g.FillEllipse(foodBrush, x * square + 6, y * square + 6, 4, 4)
      Case "J"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
        g.FillEllipse(foodBrush, x * square + 6, y * square + 6, 4, 4)
      Case "$"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
        g.DrawImage(images(7), x * square, y * square, square, square)
      Case "-"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
        g.DrawImage(images(0), x * square, y * square, square, square)
      Case "|"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
        g.DrawImage(images(1), x * square, y * square, square, square)
      Case "a"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
        g.DrawImage(images(2), x * square, y * square, square, square)
      Case "b"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
        g.DrawImage(images(3), x * square, y * square, square, square)
      Case "c"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
        g.DrawImage(images(4), x * square, y * square, square, square)
      Case "d"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
        g.DrawImage(images(5), x * square, y * square, square, square)
      Case "#"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
        g.DrawImage(images(6), x * square, y * square, square, square)
      Case "<"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
      Case ">"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
      Case "%"c
        g.FillRectangle(emptyBrush, x * square, y * square, square, square)
    End Select
  End Sub

  Private Sub DrawActor(ByRef a As GameActor, ByRef g As Graphics)
    SyncLock (a)
      If Not a.frameReady Then
        Try
          Monitor.Wait(a)
        Catch e As SynchronizationLockException
        Catch e As ThreadInterruptedException
        End Try
      End If
      a.Draw(g)
      a.frameReady = False
      Monitor.Pulse(a)
    End SyncLock
  End Sub

  Public Function MoveRequest(ByVal actor As GameActor, _
      ByVal oldDir As Integer, ByVal dir As Integer) As Boolean
    Dim xMove As Integer = 0
    Dim yMove As Integer = 0

    Dim x As Integer = actor.xScreen
    Dim y As Integer = actor.yScreen
    Dim xFood As Integer = x
    Dim yFood As Integer = y

    If (x Mod square <> 0 Or y Mod square <> 0) And _
      Math.Abs(oldDir - dir) Mod 2 <> 0 Then
      Return False
    End If
    Select Case dir
      Case Direction.Up
        y -= actor.step
        yMove = -actor.step
        yFood += yMove
      Case Direction.Left
        x -= actor.step
        xMove = -actor.step
        xFood += xMove
      Case Direction.Down
        y += square + actor.step - 1
        yMove = actor.step
        yFood += yMove
      Case Direction.Right
        x += square + actor.step - 1
        xMove = actor.step
        xFood += xMove
    End Select

    Dim xOff As Integer = x \ square
    Dim yOff As Integer = y \ square

    Dim val As Char = mazeData(yOff).Chars(xOff)

    If val = "a"c Or val = "b"c Or val = "c"c Or val = "d"c Or _
      val = "-"c Or val = "|"c Then
      Return False
    End If

    If val = "#"c And dir = 2 And (Not actor.dead) Then
      Return False
    End If

    If val = "#"c And dir = 0 And doorClosed Then
      Return False
    End If

    If val = "<"c Then
      actor.SetPos(24, 14)
    Else
      If val = ">"c Then
        actor.SetPos(1, 14)
      Else
        actor.Move(xMove, yMove)
      End If
    End If
    If xFood Mod square <> 0 Or yFood Mod square <> 0 Then
      Return True
    End If

    xOff = xFood \ square
    yOff = yFood \ square

    If actor.GetType().ToString().EndsWith("Dog") And _
      (mazeData(yOff).Chars(xOff) = "*"c Or _
      mazeData(yOff).Chars(xOff) = "$") Then

      food -= 1
      score += 1
      If val = "$"c Then
        reverse = True
        Cat.scared = True
        actor.step = 4
        doorClosed = True
        timer.Interval = 10000
        timer.Start()
      End If

      mazeData(yOff) = mazeData(yOff).Remove(xOff, 1)
      mazeData(yOff) = mazeData(yOff).Insert(xOff, " ")
    End If

    Return True
  End Function

  Public Sub CheckCollision()
    Dim i As Integer
    For i = 0 To Constants.catCount - 1
      If ColissionDetected(dog.xScreen, dog.yScreen, _
        cats(i).xScreen, cats(i).yScreen) Then
        If Not cats(i).dead Then
          If reverse Then
            cats(i).dead = True
            Thread.Sleep(500)
            score += 10
          Else
            dog.dead = True
            Return
          End If
        End If
      End If
    Next
  End Sub

  Private Function ColissionDetected(ByVal x1 As Integer, ByVal y1 As Integer, _
    ByVal x2 As Integer, ByVal y2 As Integer) As Boolean

    Dim minX As Integer = Math.Min(x1, x2)
    Dim maxX As Integer = Math.Max(x1, x2)
    Dim minY As Integer = Math.Min(y1, y2)
    Dim maxY As Integer = Math.Max(y1, y2)

    Return (maxX <= (minX + square \ 2)) And _
      (maxY <= minY + square \ 2)
  End Function

End Class



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

