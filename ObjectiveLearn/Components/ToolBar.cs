using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using System;
using System.Text.Json;
using System.IO;
using ObjectiveLearn.Shared;
using Shared.Localization;
using System.Linq;

namespace ObjectiveLearn.Components;

public class ToolBar : Drawable
{
    private static Dialog _confirmActionDialog;
    public CustomButton DeleteButton;
    public CustomNumericStepper RotationStepper;
    public CustomColorPicker ColorPicker;

    public ToolBar()
    {
        Draw();
    }

	public void Draw()
    {
        var rectangleButton = new ImageButton
        {
            Image = new Bitmap(Path.Combine(App.Directory, "Resources/rectangle.png")),
            Text = LanguageManager.Get(LanguageName.TopBarRectangle),
            Font = App.TextFont,
            Color = ConfigManager.GetColor(Config.UiTextColor),
            ForceHeight = 130
        };

        var triangleButton = new ImageButton
        {
            Image = new Bitmap(Path.Combine(App.Directory, "Resources/triangle.png")),
            Text = LanguageManager.Get(LanguageName.TopBarTriangle),
            Font = App.TextFont,
            Color = ConfigManager.GetColor(Config.UiTextColor),
            ForceHeight = 130
        };

        var ellipseButton = new ImageButton
        {
            Image = new Bitmap(Path.Combine(App.Directory, "Resources/ellipse.png")),
            Text = LanguageManager.Get(LanguageName.TopBarEllipse),
            Font = App.TextFont,
            Color = ConfigManager.GetColor(Config.UiTextColor),
            ForceHeight = 130
        };

        var saveButton = new CustomButton
        {
            Text = LanguageManager.Get(LanguageName.TopBarSave),
            Font = App.TextFont,
            Color = ConfigManager.GetColor(Config.UiTextColor)
        };

        var loadButton = new CustomButton
        {
            Text = LanguageManager.Get(LanguageName.TopBarOpen),
            Font = App.TextFont,
            Color = ConfigManager.GetColor(Config.UiTextColor)
        };

        var clearButton = new CustomButton
        {
            Text = LanguageManager.Get(LanguageName.TopBarClear),
            Font = App.TextFont,
            Color = ConfigManager.GetColor(Config.UiTextColor)
        };

        DeleteButton = new CustomButton
        {
            Text = LanguageManager.Get(LanguageName.TopBarDelete),
            Font = App.TextFont,
            Color = ConfigManager.GetColor(Config.UiTextColor),
            DisabledColor = ConfigManager.GetColor(Config.UiDisabledTextColor),
            Enabled = false
        };

        var label = new Label
        {
            Text = App.Task,
            Width = -1,
            Height = -1,
            Wrap = WrapMode.Word,
            TextColor = ConfigManager.GetColor(Config.CanvasColor),
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };

        ColorPicker = new CustomColorPicker
        {
            Width = 100,
            SelectedColor = Color.FromArgb(239, 35, 60)
        };

        RotationStepper = new CustomNumericStepper
        {
            Formatter = s => $"{s}°",
            Font = App.TextFont,
            Color = ConfigManager.GetColor(Config.UiTextColor),
            DisabledColor = ConfigManager.GetColor(Config.UiDisabledTextColor),
            Enabled = false
        };

        rectangleButton.Clicked += RectangleButtonOnClick;
        triangleButton.Clicked += TriangleButtonOnClick;
        ellipseButton.Clicked += CircleButtonOnClick;
        saveButton.Clicked += SaveButtonOnClick;
        loadButton.Clicked += LoadButtonOnClick;
        clearButton.Clicked += ClearButtonOnClick;
        DeleteButton.Clicked += DeleteButtonOnClick;
        ColorPicker.ValueChanged += ColorPickerOnValueChanged;
        RotationStepper.ValueChanged += RotationStepperOnValueChanged;

        var layout = new DynamicLayout
        {
            Padding = new Padding(8),
            DefaultSpacing = new Size(5, 5)
        };

        layout.BeginHorizontal(false);

        if (!App.TeacherMode)
        {
            layout.Add(rectangleButton);
            layout.Add(triangleButton);
            layout.Add(ellipseButton);    
        }

        layout.BeginVertical(null, null, false, false);

        layout.Add(saveButton, false, true);
        layout.Add(loadButton, false, true);

        layout.EndBeginVertical(null, null, false, false);

        layout.Add(clearButton, false, true);

        if (!App.TeacherMode) layout.Add(DeleteButton, false, true);

        layout.EndVertical();

        if (!App.TeacherMode) {
            layout.BeginVertical(null, null, false, false);

            layout.Add(ColorPicker, false, true);
            layout.Add(RotationStepper, false, true);

            layout.EndVertical();
        }
        layout.BeginVertical(Padding.Empty, Size.Empty, true, true);

        layout.Add(label);

        layout.EndVertical();

        layout.EndHorizontal();

        Content = layout;
    }

    private static void RectangleButtonOnClick(object sender, EventArgs e)
    {
        App.Tool = ShapeTool.Rectangle; 
        App.SideBar.ShowClassCard();
    }

    private static void TriangleButtonOnClick(object sender, EventArgs e)
    {
        App.Tool = ShapeTool.Triangle;
        App.SideBar.ShowClassCard();
    }

    private static void CircleButtonOnClick(object sender, EventArgs e)
    {
        App.Tool = ShapeTool.Ellipse;
        App.SideBar.ShowClassCard();
    }

    private static void SaveButtonOnClick(object sender, EventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            FileName = "Checkpoint.olcp",
            Filters = {
                new FileFilter("O:L Checkpoint", ".olcp")
            }
        };

        if (dialog.ShowDialog(Application.Instance.MainForm) != DialogResult.Ok)
        {
            return;
        }

        if (!dialog.FileName.EndsWith(".olcp")) 
        {
            dialog.FileName += ".olcp";
        }

        var jsonString = JsonSerializer.Serialize(App.Serialize());
        App.CurrentFile = dialog.FileName;
        File.WriteAllText(dialog.FileName, jsonString);
    }

    private void LoadButtonOnClick(object sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filters = {
                new FileFilter("O:L Checkpoint", ".olcp")
            }
        };
        
        if (dialog.ShowDialog(Application.Instance.MainForm) == DialogResult.Ok)
        {
            var program = JsonSerializer.Deserialize<SerializableObjectiveLearnProgram>(File.ReadAllText(dialog.FileName));
            App.CurrentFile = dialog.FileName;
            App.Deserialize(program);
        }

        Draw();
    }

    private void ClearButtonOnClick(object sender, EventArgs e) {
        var random = new Random();
        var confirmButton = new Button {
            Text = random.Next(0, 1000000) == 1 
                ? "Jawui oida" 
                : LanguageManager.Get(LanguageName.ConfirmClearDialogAccept)
        };

        confirmButton.Click += ConfirmActionDialogOnConfirmed;

        var abortButton = new Button {
            Text = LanguageManager.Get(LanguageName.ConfirmClearDialogDecline)
        };

        abortButton.Click += ConfirmActionDialogOnAborted;

        var content = new DynamicLayout
        {
            Padding = new Padding(8),
            DefaultSpacing = new Size(5, 5)
        };

        content.BeginHorizontal(true);

        content.Add(new Label 
        {
            TextAlignment = TextAlignment.Center,
            Text = LanguageManager.Get(LanguageName.ConfirmClearDialogContent)
        });

        content.EndHorizontal();

        _confirmActionDialog = new Dialog {
            Content = content,
            Title = LanguageManager.Get(LanguageName.ConfirmClearDialogTitle)
        };

        _confirmActionDialog.NegativeButtons.Add(confirmButton);
        _confirmActionDialog.PositiveButtons.Add(abortButton);

        _confirmActionDialog.ShowModal(this);
    }

    private static void ConfirmActionDialogOnConfirmed(object sender, EventArgs e) {
        App.TankVm.Visitor.Variables = VmVariables.DefaultVariables
            .ToDictionary(x => x.Key, x => x.Value); // = Clone
        App.Canvas.UpdateShapes();
        App.TeacherMode = false;
        App.SideBar.Reset();
        App.ToolBar.Draw();
        App.CurrentFile = LanguageManager.Get(LanguageName.UiNoFileSelected);
        _confirmActionDialog.Close();
    }

    private static void ConfirmActionDialogOnAborted(object sender, EventArgs e) {
        _confirmActionDialog.Close();
    }

    private void DeleteButtonOnClick(object sender, EventArgs e) {
        DeleteButton.Enabled = false;
        ColorPicker.SelectedColor = App.Canvas.DrawColor;
        RotationStepper.Enabled = false;
        RotationStepper.Value = 0;
        Invalidate();
        App.SideBar.Reset();
        App.TankVm.Visitor.Variables.Remove(App.Canvas.SelectedShape.ReferencedShape.VariableName);
        App.Canvas.SelectedShape = null;
        App.Canvas.UpdateShapes();
    }

    private void ColorPickerOnValueChanged(object sender, EventArgs e) {
        App.Canvas.SetColor(ColorPicker.SelectedColor);
    }

    private void RotationStepperOnValueChanged(object sender, EventArgs e) {
        App.Canvas.RotateSelectedShape(RotationStepper.Value);
    }
}
