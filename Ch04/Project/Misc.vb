Public Enum ShapeType As Integer
  [Class]
  [Interface]
  Generalization
  Dependency
  Association
  Aggregation
  None
End Enum

Public Enum RectHandle As Integer
  TopLeft
  TopRight
  BottomLeft
  BottomRight
  None
End Enum

Public Enum LineHandle As Integer
  FromHandle
  ToHandle
  None
End Enum

Public Enum RectPart As Integer
  Name
  Attributes
  Operations
End Enum

Public Class States
  Public Shared ShapeDrawn As ShapeType = ShapeType.None
  Public Shared RectPart As RectPart
End Class

