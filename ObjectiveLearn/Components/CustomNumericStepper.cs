using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
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
    private int _value = 0;

    public int Value { 
        get {
            return _value;
        }
        set {
            _value = value;
            Invalidate();
        }
    }

    public Font Font { get; set; } = new Font(SystemFont.Default, 12);

    public Color Color { get; set; }

    public Color DisabledColor { get; set; }

    public Color BackdropColor { get; set; } = Color.FromArgb(255, 255, 255, 10);

    public Color HoverColor { get; set; } = Color.FromArgb(255, 255, 255, 25);

    public SolidBrush TextBrush { get; private set; }
    
    public SolidBrush DisabledTextBrush { get; private set; }

    public event EventHandler Submitted;
    public event EventHandler ValueChanged;

    private const float TextPadding = 5;

    private RectangleF _addButtonRect;
    private RectangleF _removeButtonRect;
    
    private CancellationTokenSource _recursionSource = new();

    public void Init() 
    {
        Cursor = Cursors.Pointer;
        TextBrush = new(Color);
        DisabledTextBrush = new(DisabledColor);
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
        var removeButtonWidth = 2 * TextPadding + removeTextSize.Width;
        var textBoxWidth = width - addButtonWidth - removeButtonWidth - 2 * Gap;

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
            Location = new PointF(textBoxWidth + addButtonWidth + 2 * Gap, 0)
        };

        e.Graphics.FillRectangle(BackdropColor, textBoxRect);
        e.Graphics.FillRectangle(BackdropColor, _addButtonRect);
        e.Graphics.FillRectangle(BackdropColor, _removeButtonRect);

        var distanceLeft = addTextSize.Width >= width - 20
            ? width - addTextSize.Width - 10
            : 10
            ;
        var distanceTop = (height - addTextSize.Height) / 2;

        e.Graphics.DrawText(Font, Enabled ? Color : DisabledColor, distanceLeft, distanceTop, _value.ToString());
        e.Graphics.DrawText(Font, Enabled ? Color : DisabledColor, textBoxWidth + Gap + TextPadding, distanceTop, AddContent);
        e.Graphics.DrawText(Font, Enabled ? Color : DisabledColor, textBoxWidth + 2 * Gap + addButtonWidth + TextPadding, distanceTop, RemoveContent);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        KeyHandler.FocusedComponent = this;
        _recursionSource.Cancel();
        ValueChanged.Invoke(this, EventArgs.Empty);
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
        switch (e.Key)
        {
            case Keys.Up:
                AddOne();
                break;

            case Keys.Down:
                AddOne();
                break;
        }
    }
    
    private void AddOne() 
    {
        _value++;
        Invalidate();
    }

    private void RecursiveAddOne()
    {
        _recursionSource.Cancel();
        _recursionSource = new();
        Task.Run(async () => await RecursiveAddOne(_recursionSource.Token));
    }

    private async Task RecursiveAddOne(CancellationToken cancellationToken)
    {
        AddOne();
        await Task.Delay(PrimaryKeyDelay);
        while (!cancellationToken.IsCancellationRequested)
        {
            AddOne();
            await Task.Delay(SecondaryKeyDelay);
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
        _recursionSource = new();
        Task.Run(async () => await RecursiveRemoveOne(_recursionSource.Token));
    }

    private async Task RecursiveRemoveOne(CancellationToken cancellationToken)
    {
        RemoveOne();
        await Task.Delay(PrimaryKeyDelay);
        while (!cancellationToken.IsCancellationRequested)
        {
            RemoveOne();
            await Task.Delay(SecondaryKeyDelay);
        }
    }
}