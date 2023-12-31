using System;
using Eto.Drawing;
using Eto.Forms;

namespace ObjectiveLearn.Components;

public sealed class CustomColorPicker : Drawable
{
    private Color _selectedColor;

    public Color SelectedColor {
        get => _selectedColor;

        set {
            _selectedColor = value;
            Invalidate();
        }
    }

    public Color BackdropColor { get; set; } = Color.FromArgb(255, 255, 255, 10);

    public Color HoverColor { get; set; } = Color.FromArgb(255, 255, 255, 25);
   
    public event EventHandler ValueChanged;

    private bool _isMouseOver;

    private ColorDialog _dialog;

    private const float TextPadding = 20;

    public CustomColorPicker() 
    {
        Cursor = Cursors.Pointer;
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
            Location = new PointF((width - size.Width) / 2, TextPadding)
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

        _dialog = new ColorDialog
        {
            AllowAlpha = true,
            Color = _selectedColor
        };

        _dialog.ColorChanged += ColorDialogOnColorChanged;

        _dialog.ShowDialog(this);
    }

    private void ColorDialogOnColorChanged(object sender, EventArgs e)
    {
        _selectedColor = _dialog.Color;
        ValueChanged?.Invoke(this, EventArgs.Empty);
        Invalidate();
    }
}