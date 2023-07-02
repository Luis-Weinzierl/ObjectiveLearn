using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TankLite;
using TankLite.Models;
using TankLite.Values;
using System.Text.Json;

namespace ObjectiveLearn.Components;

public class ConsoleBar : Drawable
{
    public event EventHandler UpdateShapes;

    private TextBox _textBox;
    private string _error = string.Empty;

    private readonly Dictionary<string, TLValue> _variables = new()
    {
        { "println", new TLFunc(TestFunc, "void") },
        {
            TLName.Rectangle,
            new TLObj()
            {
                Value = new()
                {
                    { TLName.Constructor, new TLFunc(RectangleHelpers.Constructor, TLName.Rectangle) }
                }
            }
        },
        {
            TLName.Triangle,
            new TLObj()
            {
                Value = new()
                {
                    { TLName.Constructor, new TLFunc(TriangleHelpers.Constructor, TLName.Triangle) }
                }
            }
        },
        {
            TLName.Ellipse,
            new TLObj()
            {
                Value = new()
                {
                    { TLName.Constructor, new TLFunc(EllipseHelpers.Constructor, TLName.Ellipse) }
                }
            }
        }
    };

    public ConsoleBar()
    {
        MainForm.TLEnv = new TankLiteRuntimeEnvironment(_variables);

        TLError.ErrorOccurred += OnErrorOccured;

        Draw();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
    }

    private static TLValue TestFunc(TLFuncArgs args)
    {
        Debug.WriteLine(args.Args[0].ToString());
        
        return new TLVoid();
    }

    private void ExecuteButtonOnClick(object sender, EventArgs e)
    {
        MainForm.TLEnv.Execute(_textBox.Text);
        UpdateShapes.Invoke(this, EventArgs.Empty);

        _textBox.Text = string.Empty;
    }

    private void OnErrorOccured(object sender, string message)
    {
        _error = message;

        Draw();
    }

    private void Draw()
    {
        var button = new Button()
        {
            Text = "Ausführen"
        };

        _textBox = new TextBox();

        button.Click += ExecuteButtonOnClick;

        var label = new Label()
        {
            Text = _error,
            TextColor = Color.FromArgb(255, 0, 0)
        };

        var layout = new DynamicLayout()
        {
            Padding = new(8),
            Spacing = new(5, 20),
        };

        layout.BeginVertical();
        layout.BeginHorizontal();

        layout.Add(_textBox, true, false);
        layout.Add(button, false, false);

        layout.EndBeginHorizontal();

        layout.Add(label);

        layout.EndVertical();

        Content = layout;
    }
}
