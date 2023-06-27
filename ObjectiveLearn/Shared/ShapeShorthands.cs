using Eto.Drawing;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TankLite.Values;

namespace ObjectiveLearn.Shared;

public static class ShapeShorthands
{
    private static readonly Pen _pen = new(
        ConfigurationManager.GetColor(Config.PreviewColor),
        ConfigurationManager.GetFloat(Config.PreviewStrength)
        );

    public static void DrawRectangle(this Graphics graphics, Point location, Size size)
    {
        var points = new Point[]
        {
            new Point(location.X, location.Y),
            new Point(location.X + size.Width, location.Y),
            new Point(location.X + size.Width, location.Y + size.Height),
            new Point(location.X, location.Y + size.Height),
        };

        var path = new GraphicsPath();

        path.AddLine(
            points[0],
            points[1]
            );

        path.AddLine(
            points[1],
            points[2]
            );

        path.AddLine(
            points[2],
            points[3]
            );

        path.CloseFigure();

        graphics.DrawPath(_pen, path);
    }

    public static void DrawTriangle(this Graphics graphics, Point location, Size size)
    {
        var points = new Point[]
        {
            new Point(location.X + size.Width / 2, location.Y),
            new Point(location.X + size.Width, location.Y + size.Height),
            new Point(location.X, location.Y + size.Height),
        };

        var path = new GraphicsPath();

        path.AddLine(
            points[0],
            points[1]
            );

        path.AddLine(
            points[1],
            points[2]
            );

        path.CloseFigure();

        graphics.DrawPath(_pen, path);
    }

    public static void DrawEllipse(this Graphics graphics, Point location, Size size)
    {
        var widthOver2 = size.Width / 2;
        var widthTwoThirds = size.Width * 2 / 3;
        var heightOverTwo = size.Height / 2;

        var x2 = location.X + widthOver2;
        var y2 = location.Y + heightOverTwo;

        var points = new Point[]
        {
            new Point(x2, y2 - heightOverTwo),
            new Point(x2 + widthTwoThirds, location.Y),
            new Point(x2 + widthTwoThirds, y2 + heightOverTwo),
            new Point(x2, y2 + heightOverTwo),
            new Point(x2, y2 + heightOverTwo),
            new Point(x2 - widthTwoThirds, y2 + heightOverTwo),
            new Point(x2 - widthTwoThirds, location.Y),
            new Point(x2, y2 - heightOverTwo)
        };

        var path = new GraphicsPath();

        path.AddBezier(
            points[0],
            points[1],
            points[2],
            points[3]
        );

        path.AddBezier(
            points[4],
            points[5],
            points[6],
            points[7]
        );

        path.CloseFigure();

        graphics.DrawPath(_pen, path);
    }
}
