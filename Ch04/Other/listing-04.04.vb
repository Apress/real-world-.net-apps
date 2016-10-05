    Dim g As Graphics = Me.CreateGraphics()
    Dim pen As New Pen(Color.Black, 1)
    Dim font As New Font("Courier", 12)
    Dim x As Integer = 120
    Dim y As Integer = 20
    g.DrawString("Advanced .NET Drawing", font, Brushes.Black, x, y)
    g.DrawLine(pen, x, y + 20, x + 180, y + 20)

    g.RotateTransform(30)
    g.DrawString("Advanced .NET Drawing", font, Brushes.Black, x, y)
    g.DrawLine(pen, x, y + 20, x + 180, y + 20)

    ‘ Rotate another 30 degrees
    g.RotateTransform(30)
    g.DrawString("Advanced .NET Drawing", font, Brushes.Black, x, y)
    g.DrawLine(pen, x, y + 20, x + 180, y + 20)
