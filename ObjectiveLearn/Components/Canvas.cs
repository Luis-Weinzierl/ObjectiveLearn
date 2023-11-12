using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shapes;
using ObjectiveLearn.Shared;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TankLite.Values;

namespace ObjectiveLearn.Components;

public class Canvas : Drawable
{
    public event EventHandler<SelectShapeEventArgs> SelectShape;

    public new Point Location { get; set; }
    public List<Shape> Shapes { get; set; } = new();
    public SelectShapeEventArgs SelectedShape { get; set; }

    private PointF? _lastMouseDownPos;
    private PointF? _currentMousePos;
    private readonly Color _canvasBackground = ConfigManager.GetColor(Config.CanvasColor);
    private readonly double _dragThreshold = ConfigManager.GetDouble(Config.DragThreshold);

    public Canvas() {
        SelectShape += OnSelectShape;
    }

    private void OnSelectShape(object sender, SelectShapeEventArgs e) {
        SelectedShape = e;
        App.TopBar.DeleteButton.Enabled = true;
        App.TopBar.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        var rect = new Eto.Drawing.Rectangle(Location, Size);
        pe.Graphics.FillRectangle(_canvasBackground, rect);

        var lineColor = ConfigManager.GetColor(Config.CanvasGridColor);

        for (int i = 1; i <= Size.Width / 10; i++)
        {
            var startingPoint = new Point(Location.X + 10 * i, Location.Y);
            var endingPoint = new Point(Location.X + 10 * i, Location.Y + Size.Height);

            pe.Graphics.DrawLine(lineColor, startingPoint, endingPoint);
        }

        for (int i = 1; i <= Size.Height / 10; i++)
        {
            var startingPoint = new Point(Location.X , Location.Y + 10 * i);
            var endingPoint = new Point(Location.X + Size.Width, Location.Y + 10 * i);

            pe.Graphics.DrawLine(lineColor, startingPoint, endingPoint);
        }

        foreach (var shape in Shapes)
        {
            shape.Draw(pe.Graphics);
        }

        if (App.TeacherMode) return;

        if (_lastMouseDownPos is { } l && _currentMousePos is { } c)
        {
            var width = l.X < c.X
                            ? c.X - l.X
                            : l.X - c.X
                            ;

            var height = l.Y < c.Y
                    ? c.Y - l.Y
                    : l.Y - c.Y
                    ;

            var size = new Size
            {
                Width = (int)width,
                Height = (int)height
            };

            var startPoint = new Point(
                l.X < c.X
                    ? (int)l.X
                    : (int)c.X
                    ,
                l.Y < c.Y
                    ? (int)l.Y
                    : (int)c.Y
            );

            switch (App.Tool)
            {
                case ShapeTool.Rectangle:
                    pe.Graphics.DrawRectangle(startPoint, size);
                    break;

                case ShapeTool.Triangle:
                    pe.Graphics.DrawTriangle(startPoint, size);
                    break;

                case ShapeTool.Ellipse:
                    pe.Graphics.DrawEllipse(startPoint, size);
                    break;
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _lastMouseDownPos = e.Location;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_lastMouseDownPos is not { } l)
        {
            return;
        }

        _currentMousePos = e.Location;

        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (_lastMouseDownPos is not { } l)
        {
            Invalidate();
            return;
        }

        var width = l.X < e.Location.X
                ? e.Location.X - l.X
                : l.X - e.Location.X
                ;

        var height = l.Y < e.Location.Y
                ? e.Location.Y - l.Y
                : l.Y - e.Location.Y
                ;

        var startPoint = new Point(
            l.X < e.Location.X
                ? (int)l.X
                : (int)e.Location.X
                ,
            l.Y < e.Location.Y
                ? (int)l.Y
                : (int)e.Location.Y
        );

        _lastMouseDownPos = null;

        var distance = Math.Sqrt(width * width + height * height);

        if (distance < _dragThreshold)
        {
            if (distance == 0)
            {
                foreach (var shape in Shapes)
                {
                    shape.HandleClick(startPoint);
                }
            }

            Invalidate();
            return;
        }

        if (App.Tool == ShapeTool.Eraser || App.TeacherMode)
        {
            return;
        }

        App.TankVM.Visitor.Variables[$"form{Shape.IdCounter}"] = ShapeHelpers.CreateShape(
            App.Tool switch
            {
                ShapeTool.Rectangle => TLName.RectangleType,
                ShapeTool.Triangle => TLName.TriangleType,
                ShapeTool.Ellipse => TLName.EllipseType,
                _ => TLName.RectangleType
            },
            startPoint,
            new Size((int)width, (int)height),
            0,
            255,
            0,
            0,
            255
            );

        Shape.IdCounter++;
        UpdateShapes();
    }

    public void UpdateShapes()
    {
        Shapes.Clear();

        foreach (var variable in App.TankVM.Visitor.Variables)
        {
            if (variable.Value.Type != TLName.Object)
            {
                continue;
            }

            var obj = (TLObj)variable.Value;

            if (!obj.Value.ContainsKey(TLName.Type))
            {
                continue;
            }

            var type = (TLString)obj.Value[TLName.Type];
            var x = (TLInt)obj.Value[TLName.XPos];
            var y = (TLInt)obj.Value[TLName.YPos];
            var w = (TLInt)obj.Value[TLName.Width];
            var h = (TLInt)obj.Value[TLName.Height];
            var r = (TLInt)obj.Value[TLName.Rotation];

            var color = (TLObj)obj.Value[TLName.Color];

            var red     = (TLInt)color.Value[TLName.Red];
            var green   = (TLInt)color.Value[TLName.Green];
            var blue    = (TLInt)color.Value[TLName.Blue];
            var alpha   = (TLInt)color.Value[TLName.Alpha];

            SelectShape.Invoke(this, new()
            {
                VariableName = variable.Key,
                ShapeType = type.Value,
                Shape = obj
            });

            Shape shape;

            if (type.Value == TLName.Rectangle)
            {
                shape = new Shapes.Rectangle(
                    new(w.Value, h.Value),
                    new(x.Value, y.Value),
                    r.Value,
                    Color.FromArgb(
                        red.Value,
                        green.Value,
                        blue.Value,
                        alpha.Value
                        )
                    );
            }
            else if (type.Value == TLName.Triangle)
            {
                shape = new Triangle(
                    new(w.Value, h.Value),
                    new(x.Value, y.Value),
                    r.Value,
                    Color.FromArgb(
                        red.Value,
                        green.Value,
                        blue.Value,
                        alpha.Value
                        )
                    );
            }
            else if (type.Value == TLName.Ellipse)
            {
                shape = new Ellipse(
                    new(w.Value, h.Value),
                    new(x.Value, y.Value),
                    r.Value,
                    Color.FromArgb(
                        red.Value,
                        green.Value,
                        blue.Value,
                        alpha.Value
                        )
                    );
            }
            else {
                throw new NotImplementedException(type.Value);
            }

            shape.ReferencedShape = new()
            {
                VariableName = variable.Key,
                ShapeType = type.Value,
                Shape = obj
            };

            shape.ShapeSelected += SelectShape;

            Shapes.Add(shape);
        }

        Invalidate();
    }
}
