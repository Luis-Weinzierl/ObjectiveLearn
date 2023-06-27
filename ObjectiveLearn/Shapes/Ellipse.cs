using Eto.Drawing;
using System;

namespace ObjectiveLearn.Shapes;

public class Ellipse : Shape
{
    public Ellipse(
        Size size,
        Point location,
        int rotation,
        Color color
    ) {
        Size = size;
        Location = location;
        Rotation = rotation;
        Color = color;

        var widthOver2 = Size.Width / 2;
        var widthTwoThirds = Size.Width * 2 / 3;
        var heightOverTwo = Size.Height / 2;

        var x2 = Location.X + widthOver2;
        var y2 = Location.Y + heightOverTwo;

        Points = new Point[]
        {
            new Point(x2, y2 - heightOverTwo),
            new Point(x2 + widthTwoThirds, Location.Y),
            new Point(x2 + widthTwoThirds, y2 + heightOverTwo),
            new Point(x2, y2 + heightOverTwo),
            new Point(x2, y2 + heightOverTwo),
            new Point(x2 - widthTwoThirds, y2 + heightOverTwo),
            new Point(x2 - widthTwoThirds, Location.Y),
            new Point(x2, y2 - heightOverTwo)
        };



        Rotate(Rotation);
    }

    public override void UpdatePath()
    {
        Path = new GraphicsPath();

        Path.AddBezier(
            Points[0],
            Points[1],
            Points[2],
            Points[3]
        );

        Path.AddBezier(
            Points[4],
            Points[5],
            Points[6],
            Points[7]
        );

        Path.CloseFigure();
    }

    public override bool Contains(Point p) {
        return IsInEllipse(
            new Point(
                Location.X + Size.Width / 2,
                Location.Y + Size.Height / 2
            ),
            Size.Height / 2f,
            Size.Width / 2f,
            Rotation,
            p
        );
    }

    private bool IsInEllipse(Point center, float a, float b, int angle, Point p)
    {
        var cos = Math.Cos(angle);
        var sin = Math.Sin(angle);

        return (cos * (p.X - center.X) + sin * (p.Y - center.Y)) * (cos * (p.X - center.X) + sin * (p.Y - center.Y))
        /
        (a * a)
        +
        (sin * (p.X - center.X) - cos * (p.Y - center.Y)) * (sin * (p.X - center.X) - cos * (p.Y - center.Y))
        /
        (b * b)
        <= 1.0;
    }
}
