using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media.Media3D;
using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Shared;

namespace ObjectiveLearn.Components;

public class CustomButton : Drawable
{
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            Invalidate();
        }
    }

    public Font Font { get; set; }

    public Color Color { get; set; }

    public Color DisabledColor { get; set; }

    public Color BackdropColor { get; set; } = Color.FromArgb(255, 255, 255, 10);

    public Color HoverColor { get; set; } = Color.FromArgb(255, 255, 255, 25);

    public SolidBrush TextBrush { get; private set; }
    
    public SolidBrush DisabledTextBrush { get; private set; }

    public int ForceWidth { get; set; } = -1;

    public int ForceHeight { get; set; } = -1;

    public event EventHandler Clicked;

    private string _text = string.Empty;
    private bool _isMouseOver;
    private const float TextPadding = 20;
    private float _distanceLeft = -1;
    private float _distanceTop = -1;
    private int _width = -1;
    private int _height = -1;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var textSize = Font.MeasureString(Text);

        var width = textSize.Width + 2 * TextPadding;

        var height = textSize.Height + 2 * TextPadding;

        Resize((int)height, (int)width);

        Cursor = Cursors.Pointer;
        TextBrush = new SolidBrush(Color);
        DisabledTextBrush = new SolidBrush(DisabledColor);
    }

    private void Resize(int height, int width)
    {
        var textSize = Font.MeasureString(Text);

        if (ForceHeight > -1)
        {
            Height = ForceHeight;
            _height = ForceHeight;
        }

        if (ForceWidth > -1)
        {
            Width = ForceWidth;
            _width = ForceWidth;
        }

        if (ForceHeight <= -1)
        {
            Height = height;
            _height = height;
        }

        if (ForceWidth <= -1)
        {
            Width = width;
            _width = width;
        }

        _distanceLeft = (_width - textSize.Width) / 2;
        _distanceTop = (_height - textSize.Height) / 2;
        Invalidate();
    }

    private void CheckForResize(PaintEventArgs e)
    {
        var sizeChanged = false;
        var height = Height;
        var width = Width;

        if (e.ClipRectangle.Width != Width || Width != _width)
        {
            width = (int)e.ClipRectangle.Width;
            sizeChanged = true;
        }

        if (e.ClipRectangle.Height != Height || Height != _height)
        {
            height = (int)e.ClipRectangle.Height;
            sizeChanged = true;
        }

        if (sizeChanged) 
            Resize(height, width);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        CheckForResize(e);

        e.Graphics.FillRectangle(BackdropColor, 0, 0, Width, Height);

        if (_isMouseOver && Enabled)
        {
            e.Graphics.FillRectangle(HoverColor, 0, 0, Width, Height);
        }

        e.Graphics.DrawText(Font, Enabled ? TextBrush : DisabledTextBrush, _distanceLeft, _distanceTop, Text);
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);

        _isMouseOver = true;
        Invalidate();
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        _isMouseOver = false;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        Clicked.Invoke(this, EventArgs.Empty);
    }
}