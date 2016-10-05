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
