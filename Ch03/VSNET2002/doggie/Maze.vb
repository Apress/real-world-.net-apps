Option Explicit On 
Option Strict On

Imports System.Timers
Imports System.Threading

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
