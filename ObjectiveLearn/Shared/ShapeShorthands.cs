﻿using System;
using Eto.Drawing;
using ObjectiveLearn.Models;
using Shared.Localisation;

namespace ObjectiveLearn.Shared;

public static class ShapeShorthands
{
    private static readonly Pen _pen = new(
        ConfigManager.GetColor(Config.PreviewColor),
        ConfigManager.GetFloat(Config.PreviewStrength)
        );

    public static void DrawRectangle(this Graphics graphics, Point location, Size size, int rotation = 0)
    {
        var points = new Point[]
        {
            new Point(location.X, location.Y),
            new Point(location.X + size.Width, location.Y),
            new Point(location.X + size.Width, location.Y + size.Height),
            new Point(location.X, location.Y + size.Height),
        };

        if (rotation != 0) {
            points = Rotate(points, rotation);
        }

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

    public static void DrawTriangle(this Graphics graphics, Point location, Size size, int rotation = 0)
    {
        var points = new Point[]
        {
            new Point(location.X + size.Width / 2, location.Y),
            new Point(location.X + size.Width, location.Y + size.Height),
            new Point(location.X, location.Y + size.Height),
        };

        if (rotation != 0) {
            points = Rotate(points, rotation);
        }

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

    public static void DrawEllipse(this Graphics graphics, Point location, Size size, int rotation = 0)
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

        if (rotation != 0) {
            points = Rotate(points, rotation);
        }

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

    private static Point[] Rotate(Point[] points, int rotation) {
        var angleRad = rotation / 57.2958;

        var sumX = 0;
        var sumY = 0;

        for (int i = 0; i < points.Length; i++)
        {
            sumX += points[i].X;
            sumY += points[i].Y;
        }

        var center = new Point(
            sumX / points.Length,
            sumY / points.Length
        );

        var sin = Math.Sin(angleRad);
        var cos = Math.Cos(angleRad);

        for (int i = 0; i < points.Length; i++)
        {
            var new_coord = new Point(
                (int)Math.Round(center.X + (points[i].X - center.X) * cos - (points[i].Y - center.Y) * sin),
                (int)Math.Round(center.Y + (points[i].X - center.X) * sin + (points[i].Y - center.Y) * cos)
            );

            (points[i].X, points[i].Y) = (new_coord.X, new_coord.Y);
        }

        return points;
    }
}
