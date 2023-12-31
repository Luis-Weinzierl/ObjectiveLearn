﻿using Eto.Drawing;
using ObjectiveLearn.Models;
using System;

namespace ObjectiveLearn.Shapes;

public abstract class Shape
{
    public event EventHandler<SelectShapeEventArgs> ShapeSelected;

    public static int IdCounter { get; set; } = 0;

    public SelectShapeEventArgs ReferencedShape { get; set; }
    public Size Size { get; set; }
    public Point Location { get; set; }
    public int Rotation { get; set; }
    public Color Color { get; set; }
    public Point[] Points { get; internal set; }
    public GraphicsPath Path { get; internal set; }

    public void Draw(Graphics graphics)
    {
        graphics.FillPath(Color, Path);
    }

    public void Rotate(int deg)
    {
        Rotation = deg;

        var angleRad = deg / 57.2958;

        var sumX = 0;
        var sumY = 0;

        foreach (var point in Points)
        {
            sumX += point.X;
            sumY += point.Y;
        }

        var center = new Point(
            sumX / Points.Length,
            sumY / Points.Length
        );

        var sin = Math.Sin(angleRad);
        var cos = Math.Cos(angleRad);

        for (var i = 0; i < Points.Length; i++)
        {
            var newCoordinate = new Point(
                (int)Math.Round(center.X + (Points[i].X - center.X) * cos - (Points[i].Y - center.Y) * sin),
                (int)Math.Round(center.Y + (Points[i].X - center.X) * sin + (Points[i].Y - center.Y) * cos)
            );

            (Points[i].X, Points[i].Y) = (newCoordinate.X, newCoordinate.Y);
        }

        UpdatePath();
    }

    public abstract void UpdatePath();
    public abstract bool Contains(Point p);

    public bool HandleClick(Point clickPos)
    {
        if (!Contains(clickPos) || ShapeSelected is null)
        {
            return false;
        }

        ShapeSelected.Invoke(this, ReferencedShape);
        return true;
    }
}
