using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Shared;

namespace ObjectiveLearn.Components;

public class CustomButton : Drawable
{
    public string Text { get; set; } = string.Empty;

    public Font Font { get; set; }

    public Color Color { get; set; }

    public Color DisabledColor { get; set; }

    public Color BackdropColor { get; set; } = Color.FromArgb(255, 255, 255, 10);

    public Color HoverColor { get; set; } = Color.FromArgb(255, 255, 255, 25);

    public SolidBrush TextBrush { get; private set; }
    
    public SolidBrush DisabledTextBrush { get; private set; }

    public event EventHandler Clicked;

    private bool _isMouseOver = false;

    private const float TextPadding = 20;

    private float _width = -1;
    private float _height = -1;
    private SizeF _textSize;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Because WPF was a bitch about painting sth with Width = -1
        if (Width == 0)
        {
            Width = 100;
        }
    
        Cursor = Cursors.Pointer;
        TextBrush = new(Color);
        DisabledTextBrush = new(DisabledColor);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        _textSize = e.Graphics.MeasureString(Font, Text);

        var width = _textSize.Width + 2 * TextPadding;
        var height = _textSize.Height + 2 * TextPadding;

        if (Width > 1)
        {
            width = Width;
        }
        else if (e.ClipRectangle.Width > width)
        {
            width = e.ClipRectangle.Width;
        }

        if (Height > 0)
        {
            height = Height;
        }
        else if (e.ClipRectangle.Height > height)
        {
            height = e.ClipRectangle.Height;
        }

        _height = height;
        _width = width;

        Height = (int)height;
        Width = (int)width;

        e.Graphics.FillRectangle(BackdropColor, 0, 0, _width, _height);

        if (_isMouseOver && Enabled)
        {
            e.Graphics.FillRectangle(HoverColor, 0, 0, _width, _height);
        }

        var distanceLeft = (_width - _textSize.Width) / 2;
        var distanceTop = (_height - _textSize.Height) / 2;

        e.Graphics.DrawText(Font, Enabled ? TextBrush : DisabledTextBrush, distanceLeft, distanceTop, Text);
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