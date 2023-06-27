using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using TankLite.Values;

namespace ObjectiveLearn.Components;

public class SideBar : Drawable
{
    private string _text = string.Empty;
    public SideBar()
    {
        MinimumSize = new(300, 300);

        Width = 300;

        Padding = new(16);

        Draw();
    }

    public void SelectObject(SelectShapeEventArgs e)
    {
        _text = $"{e.VariableName} : {e.ShapeType}\n{GetAllProperties(e.Shape)}";
        Draw();
        Invalidate();
    }

    private void Draw()
    {
        Content = new Label()
        {
            Text = _text,
            TextColor = Color.FromArgb(255, 255, 255),
            TextAlignment = TextAlignment.Center,
            Font = new("", 12)
        };
    }

    private string GetAllProperties(TLObj obj, string prefix = "")
    {
        var output = string.Empty;

        foreach (var property in obj.Value)
        {
            if (property.Key.StartsWith('.'))
            {
                continue;
            }

            if (property.Value.Type == TLName.Object)
            {
                output += GetAllProperties((TLObj)property.Value, $"{property.Key}.");
                continue;
            }

            output += $"{property.Value.Type} {prefix}{property.Key} = {property.Value}\n";
        }

        return output;
    }
}
