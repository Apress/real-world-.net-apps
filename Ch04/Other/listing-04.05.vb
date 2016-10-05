    Dim g As Graphics = Me.CreateGraphics()
    Dim pen As New Pen(Color.Black, 1)
    Dim font As New Font("Courier", 8)

    ' draw a circle at the center of rotation
    g.FillEllipse(Brushes.Red, 200, 200, 9, 9)
    g.TranslateTransform(200, 200)

    Dim i As Integer
    For i = 0 To 11
      g.DrawString("Advanced .NET Drawing", font, Brushes.Black, 20, 20)
      g.DrawLine(pen, 20, 35, 150, 35)
      g.RotateTransform(30)
    Next
