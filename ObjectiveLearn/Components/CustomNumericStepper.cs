using System;
using System.Threading;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Shared;

namespace ObjectiveLearn.Components;

public class CustomNumericStepper : KeyboardDrawable
{
    private const string AddContent = "+";
    private const string RemoveContent = "-";
    private const int PrimaryKeyDelay = 500;
    private const int SecondaryKeyDelay = 50;
    private const int Gap = 1;
    private int _value;

    public int Value
    {
        get => _value;
        set {
            _value = value;
            Invalidate();
        }
    }

    public Func<int, string> Formatter = s => s.ToString();

    public Font Font { get; set; }

    public Color Color { get; set; }

    public Color DisabledColor { get; set; }

    public Color BackdropColor { get; set; } = Color.FromArgb(255, 255, 255, 10);

    public Color HoverColor { get; set; } = Color.FromArgb(255, 255, 255, 25);

    public SolidBrush TextBrush { get; private set; }
    
    public SolidBrush DisabledTextBrush { get; private set; }

    public event EventHandler ValueChanged;

    private const float TextPadding = 5;

    private RectangleF _addButtonRect;
    private RectangleF _removeButtonRect;
    
    private CancellationTokenSource _recursionSource = new();

    private int _areaHovered = -1;

    public void Init() 
    {
        TextBrush = new SolidBrush(Color);
        DisabledTextBrush = new SolidBrush(DisabledColor);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (TextBrush is null) Init();

        var width = e.ClipRectangle.Width;
        var height = e.ClipRectangle.Height;

        var addTextSize = e.Graphics.MeasureString(Font, AddContent);
        var removeTextSize = e.Graphics.MeasureString(Font, RemoveContent);

        var addButtonWidth = 2 * TextPadding + addTextSize.Width;
        var removeButtonWidth = 2 * TextPadding + removeTextSize.Width + 3;
        var textBoxWidth = width - addButtonWidth - removeButtonWidth - 3 * Gap;

        var textBoxRect = new RectangleF
        {
            Size = new SizeF(textBoxWidth, height),
            Location = new PointF(0, 0)
        };

        _addButtonRect = new RectangleF
        {
            Size = new SizeF(addButtonWidth, height),
            Location = new PointF(textBoxWidth + Gap, 0)
        };

        _removeButtonRect = new RectangleF
        {
            Size = new SizeF(removeButtonWidth, height),
            Location = new PointF(textBoxWidth + addButtonWidth + 3 * Gap, 0)
        };

        // e.Graphics.FillRectangle(BackdropColor, textBoxRect);
        CustomShapes.DrawRoundedRectangleL(e.Graphics, (int)textBoxRect.X, (int)textBoxRect.Y, (int)textBoxRect.Width, (int)textBoxRect.Height, 10, BackdropColor);
        e.Graphics.FillRectangle(BackdropColor, _addButtonRect);
        // e.Graphics.FillRectangle(BackdropColor, _removeButtonRect);
        CustomShapes.DrawRoundedRectangleR(e.Graphics, (int)_removeButtonRect.X, (int)_removeButtonRect.Y, (int)_removeButtonRect.Width, (int)_removeButtonRect.Height, 10, BackdropColor);

        switch (_areaHovered)
        {
            case 0:
                e.Graphics.FillRectangle(HoverColor, _addButtonRect);
                break;

            case 1:
                CustomShapes.DrawRoundedRectangleR(e.Graphics, (int)_removeButtonRect.X, (int)_removeButtonRect.Y, (int)_removeButtonRect.Width, (int)_removeButtonRect.Height, 10, HoverColor);
                break;
        }

        var distanceLeft = addTextSize.Width >= width - 20
            ? width - addTextSize.Width - 10
            : 10
            ;
        var distanceTop = (height - addTextSize.Height) / 2;

        e.Graphics.DrawText(Font, Enabled ? Color : DisabledColor, distanceLeft, distanceTop, Formatter(_value));
        e.Graphics.DrawText(Font, Enabled ? Color : DisabledColor, textBoxWidth + Gap + TextPadding, distanceTop, AddContent);
        e.Graphics.DrawText(Font, Enabled ? Color : DisabledColor, textBoxWidth + 2 * Gap + addButtonWidth + TextPadding, distanceTop, RemoveContent);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        KeyHandler.FocusedComponent = this;
        _recursionSource.Cancel();
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_addButtonRect.Contains(e.Location))
        {
            Cursor = Cursors.Pointer;
            _areaHovered = 0;
            Invalidate();
        }
        else if (_removeButtonRect.Contains(e.Location))
        {
            Cursor = Cursors.Pointer;
            _areaHovered = 1;
            Invalidate();
        }
        else
        {
            Cursor = Cursors.Default;
            _areaHovered = -1;
            Invalidate();
        }
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        Cursor = Cursors.Default;
        _areaHovered = -1;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (_addButtonRect.Contains(e.Location)) RecursiveAddOne();
        if (_removeButtonRect.Contains(e.Location)) RecursiveRemoveOne();
    }

    public override void HandleKeyDown(KeyEventArgs e)
    {
        
    }

    public override void HandleKeyUp(KeyEventArgs e)
    {
        // ReSharper disable All
        switch (e.Key)
        // ReSharper restore All
        {
            case Keys.Up:
                AddOne();
                break;

            case Keys.Down:
                AddOne();
                break;
        }
    }

    public override void Deactivate()
    {
        _recursionSource.Cancel();
    }

    private void AddOne() 
    {
        _value++;
        Invalidate();
    }

    private void RecursiveAddOne()
    {
        _recursionSource.Cancel();
        _recursionSource = new CancellationTokenSource();
        RecursiveAddOne(_recursionSource.Token);
    }

    private async void RecursiveAddOne(CancellationToken cancellationToken)
    {
        AddOne();
        await Task.Delay(PrimaryKeyDelay, CancellationToken.None);
        while (!cancellationToken.IsCancellationRequested)
        {
            AddOne();
            await Task.Delay(SecondaryKeyDelay, CancellationToken.None);
        }
    }

    private void RemoveOne() 
    {
        _value--;
        Invalidate();
    }

    private void RecursiveRemoveOne()
    {
        _recursionSource.Cancel();
        _recursionSource = new CancellationTokenSource();
        RecursiveRemoveOne(_recursionSource.Token);
    }

    private async void RecursiveRemoveOne(CancellationToken cancellationToken)
    {
        RemoveOne();
        await Task.Delay(PrimaryKeyDelay, CancellationToken.None);
        while (!cancellationToken.IsCancellationRequested)
        {
            RemoveOne();
            await Task.Delay(SecondaryKeyDelay, CancellationToken.None);
        }
    }
}