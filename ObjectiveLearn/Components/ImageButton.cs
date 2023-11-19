using System;
using System.Security.Cryptography.X509Certificates;
using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Shared;

namespace ObjectiveLearn.Components;

public class ImageButton : Drawable
{
    public Image Image { get; set; }

    public string Text { get; set; }

    public Font Font { get; set; }

    public Color Color { get; set; }

    public Color BackdropColor { get; set; } = Color.FromArgb(255, 255, 255, 10);

    public Color HoverColor { get; set; } = Color.FromArgb(255, 255, 255, 25);

    public SolidBrush TextBrush { get; private set; }
    public SolidBrush DisabledTextBrush { get; private set; }

    public event EventHandler Clicked;

    private bool _isMouseOver = false;

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
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var height = (int)e.ClipRectangle.Height;
        var width = Image.Width;
        var location = e.ClipRectangle.Location;

        e.Graphics.FillRectangle(BackdropColor, 0, 0, width, height);

        if (_isMouseOver && Enabled)
        {
            e.Graphics.FillRectangle(HoverColor, 0, 0, width, height);
        }

        e.Graphics.DrawImage(Image, location);

        var heightLeft = height - Image.Height;

        var textSize = e.Graphics.MeasureString(Font, Text);

        var distanceTop = Image.Height + (heightLeft - textSize.Height) / 2;
        var distanceLeft = (Image.Width - textSize.Width) / 2;

        e.Graphics.DrawText(Font, TextBrush, distanceLeft, distanceTop, Text);

        if (!Enabled)
        {
            e.Graphics.FillRectangle(HoverColor, 0, 0, width, height);
        }

        Width = width;
        Height = height;
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