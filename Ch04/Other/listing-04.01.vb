    Dim font As New Font("Courier New", 10)
    Dim brush As New SolidBrush(Color.Black)

    pen.EndCap = LineCap.AnchorMask
    g.DrawLine(pen, 5, 10, 40, 10)
    g.DrawString("AnchorMask", font, brush, 75, 5)

    pen.EndCap = LineCap.ArrowAnchor
    g.DrawLine(pen, 5, 30, 40, 30)
    g.DrawString("ArrowAnchor", font, brush, 75, 25)

    pen.EndCap = LineCap.DiamondAnchor
    g.DrawLine(pen, 5, 50, 40, 50)
    g.DrawString("DiamondAnchor", font, brush, 75, 45)

    pen.EndCap = LineCap.Flat
    g.DrawLine(pen, 5, 70, 40, 70)
    g.DrawString("Flat", font, brush, 75, 65)

    pen.EndCap = LineCap.NoAnchor
    g.DrawLine(pen, 5, 90, 40, 90)
    g.DrawString("NoAnchor", font, brush, 75, 85)

    pen.EndCap = LineCap.Round
    g.DrawLine(pen, 5, 110, 40, 110)
    g.DrawString("Round", font, brush, 75, 105)

    pen.EndCap = LineCap.RoundAnchor
    g.DrawLine(pen, 5, 130, 40, 130)
    g.DrawString("RoundAnchor", font, brush, 75, 125)

    pen.EndCap = LineCap.Square
    g.DrawLine(pen, 5, 150, 40, 150)
    g.DrawString("Square", font, brush, 75, 145)

    pen.EndCap = LineCap.SquareAnchor
    g.DrawLine(pen, 5, 170, 40, 170)
    g.DrawString("SquareAnchor", font, brush, 75, 165)

    pen.EndCap = LineCap.Triangle
    g.DrawLine(pen, 5, 190, 40, 190)
    g.DrawString("Triangle", font, brush, 75, 185)
