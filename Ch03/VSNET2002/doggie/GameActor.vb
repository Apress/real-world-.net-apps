Option Explicit On 
Option Strict On

Imports System.Drawing
Imports System.Threading

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

