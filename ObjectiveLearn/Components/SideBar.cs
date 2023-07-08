using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shared;
using TankLite.Values;

namespace ObjectiveLearn.Components;

public class SideBar : Drawable
{
    private const int _padding = 16;

    private string _title = string.Empty;
    private string _text = string.Empty;
    private string _text2 = string.Empty;

    private Color _textColor;
    private Color _backgroundColor;

    private SolidBrush _textBrush;
    private Font _textFont;

    public SideBar()
    {
        _textColor = ConfigManager.GetColor(Config.SidebarTextColor);
        _backgroundColor = ConfigManager.GetColor(Config.SidebarBackground);

        _textBrush = new(_textColor);
        _textFont = new("Default", 12);

        MinimumSize = new(300, 300);

        Width = 300;

        Padding = new(16);
    }

    public void SelectObject(SelectShapeEventArgs e)
    {
        _title = $"{e.VariableName} : {e.ShapeType}";
        GetAllProperties(e.Shape);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (_text.Length == 0)
        {
            return;
        }

        var totalHeight = _padding * 6 + (_text.Split('\n').Length + _text2.Split('\n').Length + 4) * _textFont.LineHeight;

        var rect = new RectangleF(e.ClipRectangle.X + _padding, e.ClipRectangle.Y + _padding, e.ClipRectangle.Width - 2 * _padding, totalHeight);

        var path = GraphicsPath.GetRoundRect(rect, 8);

        var height = rect.Y + _padding;

        e.Graphics.FillPath(_backgroundColor, path);

        e.Graphics.DrawText(_textFont, _textBrush, rect.X + _padding, height, _title);

        height += _textFont.LineHeight + _padding;

        e.Graphics.DrawLine(_textColor, rect.X, height, rect.X + rect.Width, height);

        height += _padding;

        e.Graphics.DrawText(_textFont, _textBrush, rect.X + _padding, height, _text);

        height += _textFont.LineHeight * (_text.Split("\n").Length + 1) + 2 * _padding;

        e.Graphics.DrawLine(_textColor, rect.X, height, rect.X + rect.Width, height);

        height += _padding;

        e.Graphics.DrawText(_textFont, _textBrush, rect.X + _padding, height, _text2);
    }

    private (string, string) GetAllProperties(TLObj obj, string prefix = "")
    {
        var propOutput = string.Empty;
        var funcOutput = string.Empty;

        foreach (var property in obj.Value)
        {
            if (property.Key.StartsWith('.'))
            {
                continue;
            }

            if (property.Value.Type.StartsWith(TLName.Func))
            {
                funcOutput += $"{property.Value.Type} {prefix}{property.Key}()\n";
                continue;
            }

            if (property.Value.Type == TLName.Object)
            {
                var props = GetAllProperties((TLObj)property.Value, $"{prefix}{property.Key}.");
                propOutput += props.Item1;
                funcOutput += props.Item2;
                continue;
            }

            propOutput += $"{property.Value.Type} {prefix}{property.Key} = {property.Value}\n";
        }

        return (propOutput, funcOutput);
    }

    private void GetAllProperties(TLObj obj)
    {
        (_text, _text2) = GetAllProperties(obj, "");
    }
}
