using Eto.Forms;
using ObjectiveLearn.Shared;
using System;
using System.Collections.Generic;
using Eto.Drawing;
using TankLite.Values;
using Shared.Localization;
using ObjectiveLearn.Models;

namespace ObjectiveLearn.Components;

public class ConsoleBar : Drawable
{
    public event EventHandler UpdateShapes;

    private TextBox _textBox;
    private readonly List<string> _errors = new();

    public ConsoleBar()
    {
        TlError.ErrorOccurred += OnErrorOccurred;

        Draw();
    }

    private void ExecuteButtonOnClick(object sender, EventArgs e)
    {
        ExecuteCommand();
    }

    private void ClearErrorsButtonOnClick(object sender, EventArgs e)
    {
        _errors.Clear();
        Draw();
    }

    private void TextBoxOnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Keys.Enter)
        {
            ExecuteCommand();
        }
    }

    private void OnErrorOccurred(object sender, string message)
    {
        _errors.Add(message);

        Draw();
    }

    private void Draw()
    {
        var executeButton = new Button
        {
            Text = LanguageManager.Get(LanguageName.ConsoleBarExecute)
        };

        var clearErrorsButton = new Button
        {
            Text = LanguageManager.Get(LanguageName.ConsoleBarClearErrors)
        };

        _textBox = new TextBox();

        _textBox.KeyUp += TextBoxOnKeyUp;

        executeButton.Click += ExecuteButtonOnClick;
        clearErrorsButton.Click += ClearErrorsButtonOnClick;

        var errorLabel = new Label
        {
            Text = string.Join('\n', _errors),
            TextColor = ConfigManager.GetColor(Config.ErrorTextColor),
            Width = -1
        };

        var layout = new DynamicLayout();

        layout.BeginVertical(new Padding(8, 8, 0, 8), new Size(5, 5));
        layout.BeginHorizontal();

        layout.Add(_textBox, true, false);
        layout.Add(executeButton, false, false);
        layout.Add(clearErrorsButton, false, false);

        layout.EndBeginHorizontal();

        layout.Add(errorLabel);

        layout.EndVertical();

        Content = layout;
    }

    private void ExecuteCommand()
    {
        if (UpdateShapes is null)
        {
            return;
        }

        App.TankVm.Execute(_textBox.Text);
        UpdateShapes.Invoke(this, EventArgs.Empty);

        _textBox.Text = string.Empty;
    }
}
