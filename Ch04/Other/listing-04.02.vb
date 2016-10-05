    Dim g As Graphics = Me.CreateGraphics()
    Dim pen As New Pen(Color.Black, 1)

    Dim gp As New GraphicsPath()
    gp.AddLine(New Point(-20, 0), New Point(20, 0))
    gp.AddLine(New Point(20, 0), New Point(0, 20))
    gp.AddLine(New Point(0, 20), New Point(-20, 0))

    Dim customCap As New CustomLineCap(Nothing, gp)
    pen.CustomEndCap = customCap
    g.DrawLine(pen, 50, 100, 50, 35)
    g.DrawLine(pen, 50, 150, 150, 100)
    g.DrawLine(pen, 50, 170, 150, 170)
    g.DrawLine(pen, 50, 180, 50, 250)
