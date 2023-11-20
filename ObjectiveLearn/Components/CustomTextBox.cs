using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using Antlr4.Runtime.Atn;
using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Shared;

namespace ObjectiveLearn.Components;

public class CustomTextBox : KeyboardDrawable
{
    private const string ForbiddenChars = "\b\r\n\0\f\t\v";

    private const int PrimaryKeyDelay = 1000;
    private const int SecondaryKeyDelay = 100;
    private string _textContent = "\0";

    public string Text
    {
        get => _textContent;
        set {
            _textContent = value;
            _cursorIndex = value.Length;
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

    public event EventHandler Submitted;

    private bool _isMouseOver = false;

    private const float TextPadding = 20;
    private int _cursorIndex = 0;
    private char? _currentChar;

    private CancellationTokenSource _recursionSource = new();

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

        var textSize = e.Graphics.MeasureString(Font, Text);

        var width = e.Graphics.ClipBounds.Width;
        var height = textSize.Height + 2 * TextPadding;

        if (Height > 1)
        {
            height = Height;
        }
        else if (e.ClipRectangle.Height > height)
        {
            height = e.ClipRectangle.Height;
        }

        if (Enabled)
        {
            e.Graphics.FillRectangle(BackdropColor, 0, 0, width, height);

            if (_isMouseOver)
            {
                e.Graphics.FillRectangle(HoverColor, 0, 0, width, height);
            }
        }

        var distanceLeft = textSize.Width >= width - 20
            ? width - textSize.Width - 10
            : 10
            ;
        var distanceTop = (height - textSize.Height) / 2;

        e.Graphics.DrawText(Font, Enabled ? TextBrush : DisabledTextBrush, distanceLeft, distanceTop, Text);

        var textBeforeCursor = _textContent[.._cursorIndex];

        var textSizeBeforeCursor = e.Graphics.MeasureString(Font, textBeforeCursor);

        e.Graphics.DrawLine(
            Color,
            new PointF(textSizeBeforeCursor.Width + distanceLeft, distanceTop),
            new PointF(textSizeBeforeCursor.Width + distanceLeft, Height - distanceTop)
        );

        Height = (int)height;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        KeyHandler.FocusedComponent = this;
    }

    public override void HandleKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Keys.Backspace:
                RenewRecursionSource();
                RecursiveBackspace(_recursionSource.Token);
                return;

            case Keys.Delete:
                RenewRecursionSource();
                RecursiveDelete(_recursionSource.Token);
                return;

            default:
                if (!e.IsChar) return;

                RenewRecursionSource();

                if (ForbiddenChars.Contains(e.KeyChar))
                {
                    return;
                }

                _currentChar = e.KeyChar;
                RecursiveTyping(_recursionSource.Token);

                return;
        }
    }

    public override void HandleKeyUp(KeyEventArgs e)
    {
        _recursionSource.Cancel();
        
        switch (e.Key) 
        {
            case Keys.Enter:
                Submitted?.Invoke(this, EventArgs.Empty);
                break;

            case Keys.Left:
                GoBackwards();
                break;

            case Keys.Right:
                GoForwards();
                break;

            case Keys.End:
                _cursorIndex = _textContent.Length;
                Invalidate();
                break;

            case Keys.Home:
                _cursorIndex = 0;
                Invalidate();
                break;

            default:
                if (e.IsChar)
                {
                    _currentChar = null;
                }

                break;
        }
    }

    private void Delete()
    {
        if (_textContent.Length <= 0 || _cursorIndex >= _textContent.Length) return;
        _textContent = _textContent[.._cursorIndex] + _textContent[(_cursorIndex + 1)..];
        Invalidate();
    }

    private void Backspace()
    {
        if (_textContent.Length <= 0 || _cursorIndex <= 0) return;
        _textContent = _textContent[..(_cursorIndex - 1)] + _textContent[_cursorIndex..];
        _cursorIndex--;
        Invalidate();
    }

    private void GoForwards()
    {
        if (_cursorIndex >= _textContent.Length) return;
        _cursorIndex++;
        Invalidate();
    }

    private void GoBackwards()
    {
        if (_cursorIndex <= 0) return;
        _cursorIndex--;
        Invalidate();
    }

    private void TypeChar() 
    {
        _textContent = _textContent[.._cursorIndex] + _currentChar + _textContent[_cursorIndex..];
        _cursorIndex++;
        Invalidate();
    }

    private async void RecursiveDelete(CancellationToken cancellationToken)
    {
        Delete();
        await Task.Delay(PrimaryKeyDelay, CancellationToken.None);
        while (!cancellationToken.IsCancellationRequested)
        {
            Delete();
            await Task.Delay(SecondaryKeyDelay, CancellationToken.None);
        }
    }

    private async void RecursiveBackspace(CancellationToken cancellationToken)
    {
        Backspace();
        await Task.Delay(PrimaryKeyDelay, CancellationToken.None);
        while (!cancellationToken.IsCancellationRequested)
        {
            Backspace();
            await Task.Delay(SecondaryKeyDelay, CancellationToken.None);
        }
    }

    private async void RecursiveTyping(CancellationToken cancellationToken)
    {
        var lastChar = _currentChar;
        TypeChar();
        await Task.Delay(PrimaryKeyDelay, CancellationToken.None);
        while (!cancellationToken.IsCancellationRequested)
        {
            TypeChar();
            await Task.Delay(SecondaryKeyDelay, CancellationToken.None);
        }
    }

    private void RenewRecursionSource()
    {
        _recursionSource.Cancel();
        _recursionSource = new();
    }
}