using Eto.Drawing;

namespace ObjectiveLearn.Shapes;

public class Rectangle : Shape
{
    public Rectangle(
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
            new(Location.X, Location.Y),
            new(Location.X + Size.Width, Location.Y),
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

        path.AddLine(
            Points[2],
            Points[3]
            );

        path.CloseFigure();

        Path = path;
    }

    public override bool Contains(Point p)
    {
        return IsInRectangle(Points[0], Points[1], Points[2], Points[3], p);
    }

    private static float IsLeft(Point p0, Point p1, Point p2) {
        return (p1.X - p0.X) *(p2.Y - p0.Y) - (p2.X - p0.X) * (p1.Y - p0.Y);
    }

    private static bool IsInRectangle(Point x, Point y, Point z, Point w, Point p)
    {
        return IsLeft(x, y, p) > 0.0 && IsLeft(y, z, p) > 0.0 && IsLeft(z, w, p) > 0.0 && IsLeft(w, x, p) > 0.0;
    }
}
