using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Shared;

namespace ObjectiveLearn.Components;

public class CustomTextBox : KeyboardDrawable
{
    private const string ForbiddenChars = "\b\r\n\0\f\t\v";

    private const int PrimaryKeyDelay = 1000;
    private const int SecondaryKeyDelay = 100;
    private const int CursorBlinkInterval = 1000;
    private string _textContent = "";
    private float _cursorHeight;

    public string Text
    {
        get => _textContent;
        set {
            _textContent = value;
            _cursorIndex = value.Length;
            Invalidate();
        }
    }

    public Dictionary<int, Color> TextColorRanges = new();

    public Font Font { get; set; }

    public Color Color { get; set; }

    public Color DisabledColor { get; set; }

    public Color BackdropColor { get; set; } = Color.FromArgb(255, 255, 255, 10);

    public SolidBrush TextBrush { get; private set; }
    
    public SolidBrush DisabledTextBrush { get; private set; }

    public event EventHandler Submitted;

    private const float TextPadding = 20;
    private readonly Stack<string> _history = new();

    private int _cursorIndex;
    private bool _cursorVisible;
    private char? _currentChar;
    private int _historyPosition = -1;

    private CancellationTokenSource _recursionSource = new();
    private CancellationTokenSource _cursorBlinkSource = new();

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        _cursorHeight = Font.MeasureString("\0").Height;

        // Because WPF was a bitch about painting sth with Width = -1
        if (Width == 0)
        {
            Width = 100;
        }

        Cursor = Cursors.Pointer;
        TextBrush = new SolidBrush(Color);
        DisabledTextBrush = new SolidBrush(DisabledColor);
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
            CustomShapes.DrawRoundedRectangle(e.Graphics, 0, 0, Width, Height, 10, BackdropColor);
        }

        var distanceLeft = textSize.Width >= width - 20
            ? width - textSize.Width - 10
            : 10
            ;
        var distanceTop = (height - _cursorHeight) / 2;

        DrawText(e.Graphics, distanceLeft, distanceTop);

        var textBeforeCursor = _textContent[.._cursorIndex];

        var textSizeBeforeCursor = e.Graphics.MeasureString(Font, textBeforeCursor);

        if (_cursorVisible)
            e.Graphics.DrawLine(
                Color,
                new PointF(textSizeBeforeCursor.Width + distanceLeft, distanceTop),
                new PointF(textSizeBeforeCursor.Width + distanceLeft, _cursorHeight + distanceTop)
            );

        Height = (int)height;
    }

    private void DrawText(Graphics g, float distanceLeft, float distanceTop)
    {
        var currentColor = Color;
        var buffer = string.Empty;

        for (var index = 0; index < _textContent.Length; index++)
        {
            var textChar = _textContent[index];

            if (TextColorRanges.TryGetValue(index, out var range))
            {
                Debug.WriteLine(string.Join(", ", TextColorRanges.Select(model => $"{model.Key} : {model.Value}")));
                Debug.WriteLine($"Setting Color to {range} after drawing {buffer} in {currentColor} with idx {index}");
                Draw();
                currentColor = range;
            }

            buffer += textChar;
        }

        Draw();
        return;

        void Draw()
        {
            if (buffer.Length <= 0) return;

            g.DrawText(Font, Enabled ? new SolidBrush(currentColor) : DisabledTextBrush, distanceLeft, distanceTop, buffer);
            distanceLeft += Font.MeasureString(buffer).Width;
            buffer = string.Empty;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        KeyHandler.FocusedComponent = this;
        ShowCursorAndBlink();

        var lowestDistanceIndex = 0;
        var lowestDistance = float.MaxValue;

        for (int i = 0; i < _textContent.Length; i++)
        {
            var substr = _textContent[..(i + 1)];
            var textSize = Font.MeasureString(substr);

            var zeroedX = Location.X + 3;
            var distance = Math.Abs(e.Location.X - (zeroedX + textSize.Width));

            if (distance > lowestDistance)
            {
                break;
            }

            lowestDistance = distance;
            lowestDistanceIndex = i + 1;
        }

        _cursorIndex = lowestDistanceIndex;
    }

    public override void HandleKeyDown(KeyEventArgs e)
    {
        Console.WriteLine($"Key down: {e.Key}");

        if (e.IsChar) _currentChar = e.KeyChar;

        RenewRecursionSource();
        Recurse(e.Key switch
        {
            Keys.Backspace => Backspace,
            Keys.Delete => Delete,
            Keys.Left => GoBackwards,
            Keys.Right => GoForwards,
            _ => TypeChar,
        }, _recursionSource.Token);

        /*
        // ReSharper disable All
        switch (e.Key)
        // ReSharper restore All
        {
            case Keys.Backspace:
                RenewRecursionSource();
                Recurse(Backspace, _recursionSource.Token);
                return;

            case Keys.Delete:
                RenewRecursionSource();
                Recurse(Delete, _recursionSource.Token);
                return;

            case Keys.Left:
                RenewRecursionSource();
                Recurse(GoBackwards, _recursionSource.Token);
                return;

            case Keys.Right:
                RenewRecursionSource();
                Recurse(GoForwards, _recursionSource.Token);
                return;

            default:
                
                return;
        }
        */
    }

    public override void HandleKeyUp(KeyEventArgs e)
    {
        _recursionSource.Cancel();
        _currentChar = null;

        // ReSharper disable All
        switch (e.Key)
        // ReSharper restore All
        {
            case Keys.Enter:
                Submitted?.Invoke(this, EventArgs.Empty);
                break;

            case Keys.Up:
                GoBackHistory();
                break;

            case Keys.Down:
                GoForwardsHistory();
                break;

            case Keys.End:
                _cursorIndex = _textContent.Length;
                Invalidate();
                break;

            case Keys.Home:
                _cursorIndex = 0;
                Invalidate();
                break;
        }
    }

    public override void Deactivate()
    {
        _recursionSource.Cancel();
        _cursorBlinkSource.Cancel();
        _cursorVisible = false;
        TextHasChanged();
    }

    private void Delete()
    {
        if (_textContent.Length <= 0 || _cursorIndex >= _textContent.Length) return;
        _textContent = _textContent[.._cursorIndex] + _textContent[(_cursorIndex + 1)..];
        _cursorVisible = true;
        TextHasChanged();
    }

    private void Backspace()
    {
        if (_textContent.Length <= 0 || _cursorIndex <= 0) return;
        _textContent = _textContent[..(_cursorIndex - 1)] + _textContent[_cursorIndex..];
        _cursorIndex--;
        _cursorVisible = true;
        TextHasChanged();
    }

    private void GoForwards()
    {
        if (_cursorIndex >= _textContent.Length) return;
        _cursorIndex++;
        _cursorVisible = true;
        TextHasChanged();
    }

    private void GoBackwards()
    {
        if (_cursorIndex <= 0) return;
        _cursorIndex--;
        _cursorVisible = true;
        TextHasChanged();
    }

    private void GoForwardsHistory()
    {
        if (_history.Count == 0 || _historyPosition <= -1)
        {
            return;
        }

        _historyPosition--;
        switch (_historyPosition)
        {
            case > -1:
                UpdateHistory();
                break;

            default:
                ClearTextField();
                break;
        }
    }

    private void GoBackHistory()
    {
        if (_history.Count == 0 || _historyPosition + 1 >= _history.Count)
        {
            return;
        }

        _historyPosition++;
        UpdateHistory();
    }

    private void UpdateHistory()
    {
        _textContent = _history.ElementAt(_historyPosition);
        _cursorIndex = _textContent.Length;
        _cursorVisible = true;
        TextHasChanged();
    }

    private void ClearTextField()
    {
        _textContent = "\0";
        _cursorIndex = 0;
        _cursorVisible = true;
        TextHasChanged();
    }

    private void TypeChar() 
    {
        Debug.WriteLine($"called {_currentChar}");
        if (_currentChar is null || ForbiddenChars.Contains((char)_currentChar)) return;

        _textContent = _textContent[.._cursorIndex] + _currentChar + _textContent[_cursorIndex..];
        _cursorIndex++;
        _cursorVisible = true;
        TextHasChanged();
    }

    private async void Recurse(Action action, CancellationToken cancellationToken)
    {
        action();
        await Task.Delay(PrimaryKeyDelay, CancellationToken.None);
        while (!cancellationToken.IsCancellationRequested)
        {
            action();
            await Task.Delay(SecondaryKeyDelay, CancellationToken.None);
        }
    }

    private void RenewRecursionSource()
    {
        _recursionSource.Cancel();
        _recursionSource = new CancellationTokenSource();
    }

    public void Clear()
    {
        _history.Push(_textContent);
        _historyPosition = -1;
        ClearTextField();
    }

    private void ShowCursorAndBlink()
    {
        _cursorVisible = true;
        Invalidate();
        _cursorBlinkSource.Cancel();
        _cursorBlinkSource = new CancellationTokenSource();
        StartBlinkingCursor(_cursorBlinkSource.Token);
    }

    private async void TextHasChanged()
    {
        Invalidate();
        TextColorRanges = await Task.Run(ProcessTextColors);
        Invalidate();
    }

    private Dictionary<int, Color> ProcessTextColors()
    {
        var dict = new Dictionary<int, Color>();

        foreach (var regexMatch in ColorPatterns)
        {
            MatchRegex(regexMatch);
        }

        return dict;

        void MatchRegex(ColoredRegexMatch regexMatch)
        {
            var matches = regexMatch.Regex.Matches(Text);

            foreach (Match match in matches)
            {
                var group = match.Groups[regexMatch.Group];
                dict[group.Index] = regexMatch.Color;
                for (int i = 1; i < group.Length; i++)
                {
                    var j = group.Index + i;
                    if (dict.ContainsKey(j))
                        dict.Remove(j);
                }
                if (dict.ContainsKey(group.Index + group.Length))
                    continue;
                dict[group.Index + group.Length] = Color;
            }
        }
    }

    private async void StartBlinkingCursor(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(CursorBlinkInterval, CancellationToken.None);
            _cursorVisible = !_cursorVisible;
            Invalidate();
        }
    }

    private static readonly ColoredRegexMatch[] ColorPatterns = {
        new() // Methods
        {
            Group = 1,
            Regex = new Regex(@"([a-zA-Z_][a-zA-Z0-9_]*)(\((.^[\(\);])*\))"), 
            Color = Color.FromArgb(255, 209, 102)
        },
        new() // Constructors
        {
            Group = 1,
            Regex = new Regex(@"new ([a-zA-Z_][a-zA-Z0-9_]*)(\(([^\(\);])*\))"),
            Color = Color.FromArgb(6, 214, 160)
        },
        new() // Keywords
        {
            Group = 1,
            Regex = new Regex("(?<![a-zA-Z0-9_.])(var|new)(?![a-zA-Z0-9_.])"),
            Color = Color.FromArgb(46, 134, 171)
        },
        new() // Strings
        {
            Group = 0,
            Regex = new Regex(@"""[^""]*"""),
            Color = Color.FromArgb(244, 211, 94)
        },
        new() // Numbers
        {
            Group = 1,
            Regex = new Regex(@"(?<![a-zA-Z0-9_.])([0-9]+(\.[0-9]+)?)"),
            Color = Color.FromArgb(244, 240, 187)
        },
        new() // Symbols
        {
            Group = 0,
            Regex = new Regex(@"\(|\)|=|;|,|\."),
            Color = Color.FromArgb(101, 107, 123)
        }
    };

    private struct ColoredRegexMatch
    {
        public int Group { get; init; }
        public Regex Regex { get; init; }
        public Color Color { get; init; }

    }
}