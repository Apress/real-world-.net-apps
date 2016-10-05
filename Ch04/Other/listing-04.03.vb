    Dim g As Graphics = Me.CreateGraphics()
    Dim pen As New Pen(Color.Black, 1)

    Dim gp As New GraphicsPath()
    gp.AddLine(New Point(-20, 0), New Point(20, 0))
    gp.AddLine(New Point(20, 0), New Point(0, 20))
    gp.AddLine(New Point(0, 20), New Point(-20, 0))

    Dim customCap As New CustomLineCap(Nothing, gp)
    pen.CustomEndCap = customCap
    g.DrawLine(pen, 50, 100, 60, 35)
    g.DrawLine(pen, 50, 150, 150, 100)
    g.DrawLine(pen, 50, 170, 150, 190)
    g.DrawLine(pen, 50, 180, 70, 250)

    g.SmoothingMode = SmoothingMode.AntiAlias
    ‘ the following lines will be smoothed out
    g.DrawLine(pen, 250, 100, 260, 35)
    g.DrawLine(pen, 250, 150, 350, 100)
    g.DrawLine(pen, 250, 170, 350, 190)
    g.DrawLine(pen, 250, 180, 270, 250)
