using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shapes;
using ObjectiveLearn.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TankLite.Values;
using Shared.Localization;
using Rectangle = ObjectiveLearn.Shapes.Rectangle;

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
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void OnSelectShape(object sender, SelectShapeEventArgs e) {
        SelectedShape = (Shape)sender;
        App.TopBar.DeleteButton.Enabled = true;
        App.TopBar.RotationStepper.Enabled = true;
        App.TopBar.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        var rect = new Eto.Drawing.Rectangle(Location, Size);
        pe.Graphics.FillRectangle(_canvasBackground, rect);

        var lineColor = ConfigManager.GetColor(Config.CanvasGridColor);

        for (var i = 1; i <= Size.Width / 10; i++)
        {
            var startingPoint = new Point(Location.X + 10 * i, Location.Y);
            var endingPoint = new Point(Location.X + 10 * i, Location.Y + Size.Height);

            pe.Graphics.DrawLine(lineColor, startingPoint, endingPoint);
        }

        for (var i = 1; i <= Size.Height / 10; i++)
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
            var hoveredShape = Shapes
                .FirstOrDefault(shape => shape.Contains((Point)cp));

            if (hoveredShape is not null)
            {
                var text = hoveredShape.ReferencedShape.VariableName;
                var textSize = App.SmallTextFont.MeasureString(text);
                var bgWidth = textSize.Width + 6;
                var bgHeight = textSize.Height + 6;
                pe.Graphics.FillRectangle(_tooltipColor, cp.X - bgWidth, cp.Y - bgHeight, bgWidth, bgHeight);
                pe.Graphics.DrawRectangle(_textColor, cp.X - bgWidth, cp.Y - bgHeight, bgWidth, bgHeight);
                pe.Graphics.DrawText(App.SmallTextFont, _textBrush, cp.X - bgWidth + 3, cp.Y - bgHeight + 3, text);
            }
        }

        if (App.TeacherMode) return;

        if (_lastMouseDownPos is null || _currentMousePos is null)
        {
            return;
        }

        var l = (PointF)_lastMouseDownPos!;
        var c = (PointF)_currentMousePos!;

        if (SelectedShape is not null && _selectedShapeContainsLastMouseDownPos) {
            var location = (Point)(SelectedShape.Location - l + c);
            var previewSize = SelectedShape.Size;
            var rotation = SelectedShape.Rotation;

            switch (SelectedShape)
            {
                case Rectangle:
                    break;
                case Ellipse:
                    pe.Graphics.DrawEllipse(location, previewSize, rotation);
                    break;
                case Triangle:
                    pe.Graphics.DrawTriangle(location, previewSize, rotation);
                    break;
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

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _lastMouseDownPos = e.Location;
        if (SelectedShape is not null)
        {
            _selectedShapeContainsLastMouseDownPos = SelectedShape.Contains((Point)_lastMouseDownPos);
        }
    }

    protected override async void OnMouseMove(MouseEventArgs e)
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        base.OnMouseMove(e);

        if (_showTooltip)
        {
            _showTooltip = false;
            Invalidate();
        }

        _currentMousePos = e.Location;

        if (_lastMouseDownPos is null)
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

        if (SelectedShape is not null && 
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
                var clickedShapes = Shapes
                    .Where(shape => shape.HandleClick(startPoint));

                var clickedShape = clickedShapes.FirstOrDefault();

                if (clickedShape is not null)
                {
                    App.TopBar.ColorPicker.Value = clickedShape.Color;
                    App.TopBar.RotationStepper.Value = clickedShape.Rotation;
                    Invalidate();
                    return;
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

        if (App.TeacherMode)
        {
            return;
        }

        App.TankVm.Visitor.Variables[$"form{Shape.IdCounter}"] = ShapeHelpers.CreateShape(
            App.Tool switch
            {
                ShapeTool.Rectangle => TlName.RectangleType,
                ShapeTool.Triangle => TlName.TriangleType,
                ShapeTool.Ellipse => TlName.EllipseType,
                _ => TlName.RectangleType
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
        var previousSelectedShapeRefName = SelectedShape is not null
                ? SelectedShape.ReferencedShape.VariableName
            : string.Empty
            ;

        Shapes.Clear();

        foreach (var variable in App.TankVm.Visitor.Variables)
        {
            if (variable.Value.Type != TlName.Object)
            {
                continue;
            }

            var obj = (TlObj)variable.Value;

            if (!obj.Value.ContainsKey(TlName.Type))
            {
                continue;
            }

            var type = (TlString)obj.Value[TlName.Type];
            var x = (TlInt)obj.Value[TlName.XPos];
            var y = (TlInt)obj.Value[TlName.YPos];
            var w = (TlInt)obj.Value[TlName.Width];
            var h = (TlInt)obj.Value[TlName.Height];
            var r = (TlInt)obj.Value[TlName.Rotation];

            var color = (TlObj)obj.Value[TlName.Color];

            var red     = (TlInt)color.Value[TlName.Red];
            var green   = (TlInt)color.Value[TlName.Green];
            var blue    = (TlInt)color.Value[TlName.Blue];
            var alpha   = (TlInt)color.Value[TlName.Alpha];

            Shape shape;

            if (type.Value == TlName.Rectangle)
            {
                shape = new Shapes.Rectangle(
                    new Size(w.Value, h.Value),
                    location: new Point(x.Value, y.Value),
                    r.Value,
                    Color.FromArgb(
                        red.Value,
                        green.Value,
                        blue.Value,
                        alpha.Value
                        )
                    );
            }
            else if (type.Value == TlName.Triangle)
            {
                shape = new Triangle(
                    
                    new Size(w.Value, h.Value),
                    new Point(x.Value, y.Value),
                    r.Value,
                    Color.FromArgb(
                        red.Value,
                        green.Value,
                        blue.Value,
                        alpha.Value
                        )
                    );
            }
            else if (type.Value == TlName.Ellipse)
            {
                shape = new Ellipse(
                    new Size(w.Value, h.Value),
                    new Point(x.Value, y.Value),
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

            shape.ReferencedShape = new SelectShapeEventArgs
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
            var colorName = LanguageManager.Get(LanguageName.TlNameColor);
            var r = LanguageManager.Get(LanguageName.TlNameRed);
            var g = LanguageManager.Get(LanguageName.TlNameGreen);
            var b = LanguageManager.Get(LanguageName.TlNameBlue);
            var a = LanguageManager.Get(LanguageName.TlNameAlpha);
            var shapeObj = (TlObj)App.TankVm.Visitor.Variables[name];
            var colorObj = (TlObj)shapeObj.Value[colorName];
            colorObj.Value[r] = new TlInt(color.Rb);
            colorObj.Value[g] = new TlInt(color.Gb);
            colorObj.Value[b] = new TlInt(color.Bb);
            colorObj.Value[a] = new TlInt(color.Ab);
            App.SideBar.SelectObject(SelectedShape.ReferencedShape);
            UpdateShapes();
        }
        else {
            _drawColor = color;
        }
    }

    public void RotateSelectedShape(int newRotation) {
        if (SelectedShape is null) return;

        var name = SelectedShape.ReferencedShape.VariableName;
        var rot = LanguageManager.Get(LanguageName.TlNameRotation);
        var shapeObj = (TlObj)App.TankVm.Visitor.Variables[name];
        shapeObj.Value[rot] = new TlInt(newRotation);
        App.SideBar.SelectObject(SelectedShape.ReferencedShape);
        UpdateShapes();
    }

    public void MoveSelectedShape(PointF newPos) {
        if (SelectedShape is null) return;

        var name = SelectedShape.ReferencedShape.VariableName;
        var x = LanguageManager.Get(LanguageName.TlNameXPos);
        var y = LanguageManager.Get(LanguageName.TlNameYPos);
        var shapeObj = (TlObj)App.TankVm.Visitor.Variables[name];
        shapeObj.Value[x] = new TlInt((int)newPos.X);
        shapeObj.Value[y] = new TlInt((int)newPos.Y);
        App.SideBar.SelectObject(SelectedShape.ReferencedShape);
        UpdateShapes();
    }
}
