using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Shared;

namespace ObjectiveLearn.Components;

public class CustomLink : Drawable
{
    public string Text { get; set; } = string.Empty;

    public Font Font { get; set; }
    public Font HoverFont { get; set; }

    public Color Color { get; set; }

    public SolidBrush TextBrush { get; private set; }
    
    public event EventHandler Clicked;

    private bool _isMouseOver;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var textSize = Font.MeasureString(Text);

        Height = (int)textSize.Height;
        Width = (int)textSize.Width;

        Cursor = Cursors.Pointer;
        TextBrush = new(Color);

        if (HoverFont is not { })
        {
            HoverFont = new Font(Font.FamilyName, Font.Size, Font.FontStyle, FontDecoration.Underline);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.DrawText(_isMouseOver ? HoverFont : Font, TextBrush, 0, 0, Text);
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