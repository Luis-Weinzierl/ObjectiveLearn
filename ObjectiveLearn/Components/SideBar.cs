﻿using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shared;
using System;
using System.Diagnostics;
using System.IO;
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

    public SideBar()
    {
        _textColor = ConfigManager.GetColor(Config.SidebarTextColor);
        _backgroundColor = ConfigManager.GetColor(Config.SidebarBackground);

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
        var textBrush = new SolidBrush(_textColor);
        var textFont = new Font("Default", 12);

        var rect = new RectangleF(e.ClipRectangle.X + _padding, e.ClipRectangle.Y + _padding, e.ClipRectangle.Width - 2 * _padding, e.ClipRectangle.Height - 2 * _padding);

        var path = GraphicsPath.GetRoundRect(rect, 8);

        var height = rect.Y + _padding;

        e.Graphics.FillPath(_backgroundColor, path);

        e.Graphics.DrawText(textFont,textBrush, rect.X + _padding, height, _title);

        height += textFont.LineHeight + _padding;

        e.Graphics.DrawLine(_textColor, rect.X, height, rect.X + rect.Width, height);

        height += _padding;

        e.Graphics.DrawText(textFont, textBrush, rect.X + _padding, height, _text);

        height += textFont.LineHeight * (_text.Split("\n").Length + 1) + 2 * _padding;

        e.Graphics.DrawLine(_textColor, rect.X, height, rect.X + rect.Width, height);

        height += _padding;

        e.Graphics.DrawText(textFont, textBrush, rect.X + _padding, height, _text2);
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
