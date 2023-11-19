using System;
using System.Security.Cryptography.X509Certificates;
using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Shared;

namespace ObjectiveLearn.Components;

public class CustomColorPicker : Drawable
{
    public Color Color { get; set; }

    public Color SelectedColor {
        get {
            return _dialog.Color;
        }

        set {
            _dialog.Color = value;
            Invalidate();
        }
    }

    public Color DisabledColor { get; set; }

    public Color BackdropColor { get; set; } = Color.FromArgb(255, 255, 255, 10);

    public Color HoverColor { get; set; } = Color.FromArgb(255, 255, 255, 25);
   
    public event EventHandler ValueChanged;

    private bool _isMouseOver = false;

    private ColorDialog _dialog = new ColorDialog()
    {
        AllowAlpha = true
    };

    private const float TextPadding = 20;

    public CustomColorPicker() 
    {
        Cursor = Cursors.Pointer;
        _dialog.ColorChanged += ColorDialogOnColorChanged;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var width = e.ClipRectangle.Width;
        var height = e.ClipRectangle.Height;

        e.Graphics.FillRectangle(BackdropColor, 0, 0, width, height);

        if (_isMouseOver && Enabled)
        {
            e.Graphics.FillRectangle(HoverColor, 0, 0, width, height);
        }

        var size = new SizeF(height - 2 * TextPadding, height - 2 * TextPadding);

        var centeredRectangle = new RectangleF
        {
            Size = size,
            Location = new((width - size.Width) / 2, TextPadding)
        };

        e.Graphics.FillEllipse(SelectedColor, centeredRectangle);
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

        _dialog.ShowDialog(this);
    }

    private void ColorDialogOnColorChanged(object sender, EventArgs e)
    {
        Invalidate();
        ValueChanged.Invoke(this, EventArgs.Empty);
    }
}