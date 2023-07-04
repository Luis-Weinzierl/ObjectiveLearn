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

    public ConsoleBar()
    {
        TLError.ErrorOccurred += OnErrorOccured;

        Draw();
    }

    private void ExecuteButtonOnClick(object sender, EventArgs e)
    {
        ExecuteCommand();
    }

    private void TextBoxOnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Keys.Enter)
        {
            ExecuteCommand();
        }
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

        _textBox.KeyUp += TextBoxOnKeyUp;

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

    private void ExecuteCommand()
    {
        App.TankVM.Execute(_textBox.Text);
        UpdateShapes.Invoke(this, EventArgs.Empty);

        _textBox.Text = string.Empty;
    }
}
