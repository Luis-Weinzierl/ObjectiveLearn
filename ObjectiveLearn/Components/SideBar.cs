using System;
using System.Diagnostics;
using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shared;
using Shared.Localisation;
using TankLite.Values;

namespace ObjectiveLearn.Components;

public class SideBar : Drawable
{
    private const int _padding = 16;
    private const int _smallPadding = 8;

    private string _title = string.Empty;
    private string _text = string.Empty;
    private string _text2 = string.Empty;

    private readonly Color _textColor;
    private readonly Color _backgroundColor;

    private readonly SolidBrush _textBrush;
    private readonly Font _textFont;
    private readonly Font _smallTextFont;

    public SideBar()
    {
        _textColor = ConfigManager.GetColor(Config.SidebarTextColor);
        _backgroundColor = ConfigManager.GetColor(Config.SidebarBackground);

        _textBrush = new(_textColor);
        _textFont = new(SystemFont.Default, 12);
        _smallTextFont = new(SystemFont.Default, 8);

        Width = 300;

        Padding = new(16);
    }

    public void SelectObject(SelectShapeEventArgs e)
    {
        _title = $"{e.VariableName} : {e.ShapeType}";
        _text2 = string.Empty;
        GetAllProperties(e.Shape);
        Invalidate();
    }

    public void Reset()
    {
        _title = string.Empty;
        _text = string.Empty;
        _text2 = string.Empty;
        Invalidate();
    }

    public void ShowClassCard()
    {
        _title = App.Tool switch
        {
            ShapeTool.Rectangle => TLName.Rectangle,
            ShapeTool.Triangle => TLName.Triangle,
            ShapeTool.Ellipse => TLName.Ellipse,
            _ => throw new NotImplementedException()
        };

        _text = LanguageManager.Get(LanguageName.UiClassCardVariables);
        _text2 = LanguageManager.Get(LanguageName.UiClassCardMethods);

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Debug.WriteLine("Redrawing SideBar");

        var fullRect = e.ClipRectangle;

        var pathLabelSize = e.Graphics.MeasureString(_smallTextFont, App.CurrentFile);
        var textX = fullRect.X + fullRect.Width - pathLabelSize.Width - _smallPadding;
        var textY = fullRect.Y + fullRect.Height - pathLabelSize.Height - _smallPadding;

        e.Graphics.DrawText(_smallTextFont, _textBrush, textX, textY, App.CurrentFile);

        if (_text2.Length > 0)
        {
            DrawClassCard(e);
        }
        else
        {
            DrawObjectCard(e);
        }
    }

    private void DrawObjectCard(PaintEventArgs e)
    {
        if (_text.Length == 0)
        {
            return;
        }

        var totalHeight = _textFont.MeasureString(_text).Height + _textFont.MeasureString(_title).Height + 4 * _padding;

        var rect = new RectangleF(e.ClipRectangle.X + _padding, e.ClipRectangle.Y + _padding, e.ClipRectangle.Width - 2 * _padding, totalHeight);

        var path = GraphicsPath.GetRoundRect(rect, 8);

        var height = rect.Y + _padding;

        e.Graphics.FillPath(_backgroundColor, path);

        e.Graphics.DrawText(_textFont, _textBrush, rect.X + _padding, height, _title);

        height += _textFont.LineHeight + _padding;

        e.Graphics.DrawLine(_textColor, rect.X, height, rect.X + rect.Width, height);

        height += _padding;

        e.Graphics.DrawText(_textFont, _textBrush, rect.X + _padding, height, _text);
    }

    private void DrawClassCard(PaintEventArgs e)
    {
        if (_text.Length == 0)
        {
            return;
        }

        var textHeight = _textFont.MeasureString(_text).Height;
        var text2Height = _textFont.MeasureString(_text2).Height;

        var totalHeight = textHeight + _textFont.MeasureString(_title).Height + text2Height + 6 * _padding;

        var rect = new RectangleF(e.ClipRectangle.X + _padding, e.ClipRectangle.Y + _padding, e.ClipRectangle.Width - 2 * _padding, totalHeight);

        var height = rect.Y + _padding;

        e.Graphics.FillRectangle(_backgroundColor, rect);

        e.Graphics.DrawText(_textFont, _textBrush, rect.X + _padding, height, _title);

        height += _textFont.LineHeight + _padding;

        e.Graphics.DrawLine(_textColor, rect.X, height, rect.X + rect.Width, height);

        height += _padding;

        e.Graphics.DrawText(_textFont, _textBrush, rect.X + _padding, height, _text);

        height += textHeight + _padding;

        e.Graphics.DrawLine(_textColor, rect.X, height, rect.X + rect.Width, height);

        height += _padding;

        e.Graphics.DrawText(_textFont, _textBrush, rect.X + _padding, height, _text2);
    }

    private string GetAllProperties(TLObj obj, string prefix = "")
    {
        var propOutput = string.Empty;

        foreach (var property in obj.Value)
        {
            if (property.Key.StartsWith('.') || property.Value.Type.StartsWith(TLName.Func))
            {
                continue;
            }

            if (property.Value.Type == TLName.Object)
            {
                var props = GetAllProperties((TLObj)property.Value, $"{prefix}{property.Key}.");
                propOutput += props;
                continue;
            }

            propOutput += $"{property.Value.Type} {prefix}{property.Key} = {property.Value}\n";
        }

        return propOutput;
    }

    private void GetAllProperties(TLObj obj)
    {
        _text = GetAllProperties(obj, "");
    }
}
