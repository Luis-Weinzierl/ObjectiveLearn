using Eto.Drawing;

namespace ObjectiveLearn.Shapes;

public class Triangle : Shape
{
    public Triangle(
        Size size,
        Point location,
        int rotation,
        Color color
    ) {
        Size = size;
        Location = location;
        Rotation = rotation;
        Color = color;

        Points = new Point[]
        {
            new(Location.X + Size.Width / 2, Location.Y),
            new(Location.X + Size.Width, Location.Y + Size.Height),
            new(Location.X, Location.Y + Size.Height)
        };

        Rotate(Rotation);
    }

    public override void UpdatePath()
    {
        var path = new GraphicsPath();

        path.AddLine(
            Points[0],
            Points[1]
            );

        path.AddLine(
            Points[1],
            Points[2]
            );

        path.CloseFigure();

        Path = path;
    }

    public override bool Contains(Point p)
    {
        return IsPointInTriangle(Points[0], Points[1], Points[2], p);
    }

    public static float Sign(Point p1, Point p2, Point p3)
    {
        return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
    }

    public static bool IsPointInTriangle(Point p1, Point p2, Point p3, Point p)
    {
        var d1 = Sign(p, p1, p2);
        var d2 = Sign(p, p2, p3);
        var d3 = Sign(p, p3, p1);

        var hasNegative = d1 < 0.0 || d2 < 0.0 || d3 < 0.0;
        var hasPositive = d1 > 0.0 || d2 > 0.0 || d3 > 0.0;

        return !(hasNegative && hasPositive);
    }
}
