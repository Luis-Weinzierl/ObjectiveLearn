using System;
using Eto.Drawing;
using Eto.Forms;

namespace ObjectiveLearn.Components;

public class ImageButton : Drawable
{
    public Image Image { get; set; }

    public string Text { get; set; }

    public Font Font { get; set; }

    public Color Color { get; set; }

    public Color BackdropColor { get; set; } = Color.FromArgb(255, 255, 255, 10);

    public Color HoverColor { get; set; } = Color.FromArgb(255, 255, 255, 25);

    public int ForceWidth { get; set; } = -1;

    public int ForceHeight { get; set; } = -1;

    public SolidBrush TextBrush { get; private set; }

    public event EventHandler Clicked;

    private bool _isMouseOver;
    private float _distanceLeft = -1;
    private float _distanceTop = -1;
    private int _width = -1;
    private int _height = -1;

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

        var heightLeft = _height - Image.Height;

        _distanceTop = Image.Height + (heightLeft - textSize.Height) / 2;
        _distanceLeft = (Image.Width - textSize.Width) / 2;
        Invalidate();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var height = Image.Height;
        var width = Image.Width;

        Resize(height, width);

        Cursor = Cursors.Pointer;
        TextBrush = new SolidBrush(Color);
    }

    private void CheckForResize(PaintEventArgs e)
    {
        var sizeChanged = false;
        var height = Height;
        var width = Width;

        if (Math.Abs(e.ClipRectangle.Width - Width) > 0 || Width != _width)
        {
            width = (int)e.ClipRectangle.Width;
            sizeChanged = true;
        }

        if (Math.Abs(e.ClipRectangle.Height - Height) > 0 || Height != _height)
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

        e.Graphics.FillRectangle(BackdropColor, 0, 0, _width, _height);

        if (_isMouseOver && Enabled)
        {
            e.Graphics.FillRectangle(HoverColor, 0, 0, _width, _height);
        }

        e.Graphics.DrawImage(Image, 0, 0);

        e.Graphics.DrawText(Font, TextBrush, _distanceLeft, _distanceTop, Text);

        if (!Enabled)
        {
            e.Graphics.FillRectangle(HoverColor, 0, 0, _width, _height);
        }
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

        Clicked?.Invoke(this, EventArgs.Empty);
    }
}