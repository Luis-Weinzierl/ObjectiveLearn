using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shapes;
using ObjectiveLearn.Shared;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using TankLite.Values;
using Shared.Localisation;
using Timer = System.Threading.Timer;

namespace ObjectiveLearn.Components;

public class Canvas : Drawable
{
    public event EventHandler<SelectShapeEventArgs> SelectShape;

    public new Point Location { get; set; }
    public List<Shape> Shapes { get; set; } = new();
    public Shape SelectedShape { get; set; }

    private PointF? _lastMouseDownPos;
    private PointF? _currentMousePos;
    private Color _drawColor = Color.FromArgb(255, 0, 0);
    private bool _selectedShapeContainsLastMouseDownPos;
    private bool _showTooltip;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly Color _canvasBackground = ConfigManager.GetColor(Config.CanvasColor);
    private readonly double _dragThreshold = ConfigManager.GetDouble(Config.DragThreshold);
    private readonly Color _textColor = Color.FromArgb(0, 0, 0);
    private readonly Color _tooltipColor =  Color.FromArgb(255, 255, 255);
    private readonly Brush _textBrush;

    public Canvas()
    {
        SelectShape += OnSelectShape;
        _textBrush = new SolidBrush(_textColor);
        _cancellationTokenSource = new();
    }

    private void OnSelectShape(object sender, SelectShapeEventArgs e) {
        SelectedShape = (Shape)sender;
        App.TopBar.DeleteButton.Enabled = true;
        App.TopBar.RotationStepper.Enabled = true;
        App.TopBar.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        var now = DateTime.Now;
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

        if (_showTooltip && _currentMousePos is {} cp)
        {
            Shape hoveredShape = null;

            foreach (var shape in Shapes)
            {
                if (shape.Contains((Point)cp))
                {
                    hoveredShape = shape;
                    break;
                }
            }

            if (hoveredShape is {} h)
            {
                var text = h.ReferencedShape.VariableName;
                var textSize = App.SmallTextFont.MeasureString(text);
                var bgWidth = textSize.Width + 6;
                var bgHeight = textSize.Height + 6;
                pe.Graphics.FillRectangle(_tooltipColor, cp.X - bgWidth, cp.Y - bgHeight, bgWidth, bgHeight);
                pe.Graphics.DrawRectangle(_textColor, cp.X - bgWidth, cp.Y - bgHeight, bgWidth, bgHeight);
                pe.Graphics.DrawText(App.SmallTextFont, _textBrush, cp.X - bgWidth + 3, cp.Y - bgHeight + 3, text);
            }
        }

        if (App.TeacherMode) return;

        if (_lastMouseDownPos is { } l && _currentMousePos is { } c)
        {
            if (SelectedShape is {} && _selectedShapeContainsLastMouseDownPos) {
                var location = (Point)(SelectedShape.Location - _lastMouseDownPos + _currentMousePos);
                var previewSize = SelectedShape.Size;
                var rotation = SelectedShape.Rotation;

                if (SelectedShape is Shapes.Rectangle) {
                    pe.Graphics.DrawRectangle(location, previewSize, rotation);
                }
                else if (SelectedShape is Ellipse) {
                    pe.Graphics.DrawEllipse(location, previewSize, rotation);
                }
                else if (SelectedShape is Triangle) {
                    pe.Graphics.DrawTriangle(location, previewSize, rotation);
                }
                return;
            }
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
        if (SelectedShape is {})
            _selectedShapeContainsLastMouseDownPos = SelectedShape.Contains((Point)_lastMouseDownPos);
    }

    protected override async void OnMouseMove(MouseEventArgs e)
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();
        base.OnMouseMove(e);

        if (_showTooltip)
        {
            _showTooltip = false;
            Invalidate();
        }

        _currentMousePos = e.Location;

        if (_lastMouseDownPos is not { })
        {
            var cancellationToken = _cancellationTokenSource.Token;
            try
            {
                await Task.Delay(500, cancellationToken);
                _showTooltip = true;
            }
            catch (OperationCanceledException)
            { }
        }

        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (SelectedShape is {} && 
            _lastMouseDownPos is {} lp && 
            _selectedShapeContainsLastMouseDownPos &&
            !App.TeacherMode
        ) {
            MoveSelectedShape(SelectedShape.Location - lp + e.Location);
            _lastMouseDownPos = null;
            _currentMousePos = null;
            return;
        }

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
                    if (shape.HandleClick(startPoint)) {
                        App.TopBar.ColorPicker.Value = shape.Color;
                        App.TopBar.RotationStepper.Value = shape.Rotation;
                        Invalidate();
                        return;
                    }
                }
            }

            SelectedShape = null;
            App.SideBar.Reset();
            App.TopBar.DeleteButton.Enabled = false;
            App.TopBar.RotationStepper.Enabled = false;
            App.TopBar.ColorPicker.Value = _drawColor;
            App.TopBar.RotationStepper.Value = 0;
            App.TopBar.Invalidate();

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
            _drawColor.Rb,
            _drawColor.Gb,
            _drawColor.Bb,
            _drawColor.Ab
            );

        Shape.IdCounter++;
        UpdateShapes();
        SelectedShape = null;
        App.TopBar.DeleteButton.Enabled = false;
        App.TopBar.RotationStepper.Enabled = false;
    }

    public void UpdateShapes()
    {
        var previousSelectedShapeRefName = SelectedShape is {}
            ? SelectedShape.ReferencedShape.VariableName
            : string.Empty
            ;

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

            if (variable.Key == previousSelectedShapeRefName) {
                SelectedShape = shape;
            }
        }

        Invalidate();
    }

    public void SetColor(Color color) {
        if (SelectedShape is {} s) {
            var name = s.ReferencedShape.VariableName;
            var colorName = LanguageManager.Get(LanguageName.TLNameColor);
            var r = LanguageManager.Get(LanguageName.TLNameRed);
            var g = LanguageManager.Get(LanguageName.TLNameGreen);
            var b = LanguageManager.Get(LanguageName.TLNameBlue);
            var a = LanguageManager.Get(LanguageName.TLNameAlpha);
            var shapeObj = (TLObj)App.TankVM.Visitor.Variables[name];
            var colorObj = (TLObj)shapeObj.Value[colorName];
            colorObj.Value[r] = new TLInt(color.Rb);
            colorObj.Value[g] = new TLInt(color.Gb);
            colorObj.Value[b] = new TLInt(color.Bb);
            colorObj.Value[a] = new TLInt(color.Ab);
            App.SideBar.SelectObject(SelectedShape.ReferencedShape);
            UpdateShapes();
        }
        else {
            _drawColor = color;
        }
    }

    public void RotateSelectedShape(int newRotation) {
        if (SelectedShape is not {}) return;

        var name = SelectedShape.ReferencedShape.VariableName;
        var rot = LanguageManager.Get(LanguageName.TLNameRotation);
        var shapeObj = (TLObj)App.TankVM.Visitor.Variables[name];
        shapeObj.Value[rot] = new TLInt(newRotation);
        App.SideBar.SelectObject(SelectedShape.ReferencedShape);
        UpdateShapes();
    }

    public void MoveSelectedShape(PointF newPos) {
        if (SelectedShape is not {}) return;

        var name = SelectedShape.ReferencedShape.VariableName;
        var x = LanguageManager.Get(LanguageName.TLNameXPos);
        var y = LanguageManager.Get(LanguageName.TLNameYPos);
        var shapeObj = (TLObj)App.TankVM.Visitor.Variables[name];
        shapeObj.Value[x] = new TLInt((int)newPos.X);
        shapeObj.Value[y] = new TLInt((int)newPos.Y);
        App.SideBar.SelectObject(SelectedShape.ReferencedShape);
        UpdateShapes();
    }
}
