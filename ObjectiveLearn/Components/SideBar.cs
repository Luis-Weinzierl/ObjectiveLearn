using System;
using System.Diagnostics;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shared;
using Shared.Localization;
using TankLite.Values;

namespace ObjectiveLearn.Components;

public class SideBar : Drawable
{
    protected const int CardPadding = 16;
    private const int SmallPadding = 8;

    private string _title = string.Empty;
    private string _text = string.Empty;
    private string _text2 = string.Empty;

    private readonly Color _textColor;
    private readonly Color _backgroundColor;

    private readonly SolidBrush _textBrush;
    private readonly SolidBrush _uiTextBrush;

    public SideBar()
    {
        _textColor = ConfigManager.GetColor(Config.SidebarTextColor);
        var uiTextColor = ConfigManager.GetColor(Config.UiTextColor);
        _backgroundColor = ConfigManager.GetColor(Config.SidebarBackground);

        _textBrush = new SolidBrush(_textColor);
        _uiTextBrush = new SolidBrush(uiTextColor);

        Handler.Width = 300;
        Padding = new Padding(16);
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
            ShapeTool.Rectangle => TankLiteName.Rectangle,
            ShapeTool.Triangle => TankLiteName.Triangle,
            ShapeTool.Ellipse => TankLiteName.Ellipse,
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

        var pathLabelSize = App.SmallTextFont.MeasureString(App.CurrentFile);
        var textX = fullRect.X + fullRect.Width - pathLabelSize.Width - SmallPadding;
        var textY = fullRect.Y + fullRect.Height - pathLabelSize.Height - SmallPadding;

        e.Graphics.DrawText(App.SmallTextFont, _uiTextBrush, textX, textY, App.CurrentFile);

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

        var totalHeight = App.TextFont.MeasureString(_text).Height + App.TextFont.MeasureString(_title).Height + 4 * CardPadding;

        var rect = new RectangleF(e.ClipRectangle.X + CardPadding, e.ClipRectangle.Y + CardPadding, e.ClipRectangle.Width - 2 * CardPadding, totalHeight);

        var path = GraphicsPath.GetRoundRect(rect, 8);

        var height = rect.Y + CardPadding;

        e.Graphics.FillPath(_backgroundColor, path);

        e.Graphics.DrawText(App.TextFont, _textBrush, rect.X + CardPadding, height, _title);

        height += App.TextFont.LineHeight + CardPadding;

        e.Graphics.DrawLine(_textColor, rect.X, height, rect.X + rect.Width, height);

        height += CardPadding;

        e.Graphics.DrawText(App.TextFont, _textBrush, rect.X + CardPadding, height, _text);
    }

    private void DrawClassCard(PaintEventArgs e)
    {
        if (_text.Length == 0)
        {
            return;
        }

        var textHeight = App.TextFont.MeasureString(_text).Height;
        var text2Height = App.TextFont.MeasureString(_text2).Height;

        var totalHeight = textHeight + App.TextFont.MeasureString(_title).Height + text2Height + 6 * CardPadding;

        var rect = new RectangleF(e.ClipRectangle.X + CardPadding, e.ClipRectangle.Y + CardPadding, e.ClipRectangle.Width - 2 * CardPadding, totalHeight);

        var height = rect.Y + CardPadding;

        e.Graphics.FillRectangle(_backgroundColor, rect);

        e.Graphics.DrawText(App.TextFont, _textBrush, rect.X + CardPadding, height, _title);

        height += App.TextFont.LineHeight + CardPadding;

        e.Graphics.DrawLine(_textColor, rect.X, height, rect.X + rect.Width, height);

        height += CardPadding;

        e.Graphics.DrawText(App.TextFont, _textBrush, rect.X + CardPadding, height, _text);

        height += textHeight + CardPadding;

        e.Graphics.DrawLine(_textColor, rect.X, height, rect.X + rect.Width, height);

        height += CardPadding;

        e.Graphics.DrawText(App.TextFont, _textBrush, rect.X + CardPadding, height, _text2);
    }

    public static string GetAllProperties(TankLiteObj tankLiteObj, string prefix)
    {
        var propOutput = string.Empty;

        var properties = tankLiteObj.Value
            .Where(property =>
                !property.Key.StartsWith('.') && 
                !property.Value.Type.StartsWith(TankLiteName.Func));

        foreach (var property in properties)
        {
            if (property.Value.Type == TankLiteName.Object)
            {
                var props = GetAllProperties((TankLiteObj)property.Value, $"{prefix}{property.Key}.");
                propOutput += props;
                continue;
            }

            propOutput += $"{property.Value.Type} {prefix}{property.Key} = {property.Value}\n";
        }

        return propOutput;
    }

    private void GetAllProperties(TankLiteObj tankLiteObj)
    {
        _text = GetAllProperties(tankLiteObj, "");
    }
}
