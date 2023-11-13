using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using System;
using System.Text.Json;
using System.IO;
using ObjectiveLearn.Shared;
using Shared.Localisation;
using TankLite.Values;
using System.Linq;

namespace ObjectiveLearn.Components;

public class TopBar : Drawable
{
    private static Dialog _confirmActionDialog;
    public Button DeleteButton;
    public NumericStepper RotationStepper;
    public ColorPicker ColorPicker;

	public new Point Location { get; set; }

    public TopBar()
    {
        Draw();
    }

	public void Draw()
    {
        var buttonSize = new Size(80, 100);
        var rectangleButton = new Button()
        {
            Image = new Bitmap(Path.Combine(App.Directory, "Resources/RectangleIcon.png")),
            ImagePosition = ButtonImagePosition.Above,
            Text = LanguageManager.Get(LanguageName.TopBarRectangle),
            Size = new Size(80, 100),
        };

        var triangleButton = new Button()
        {
            Image = new Bitmap(Path.Combine(App.Directory, "Resources/TriangleIcon.png")),
            ImagePosition = ButtonImagePosition.Above,
            Text = LanguageManager.Get(LanguageName.TopBarTriangle),
            Size = new Size(80, 100),
        };

        var ellipseButton = new Button()
        {
            Image = new Bitmap(Path.Combine(App.Directory, "Resources/CircleIcon.png")),
            ImagePosition = ButtonImagePosition.Above,
            Text = LanguageManager.Get(LanguageName.TopBarEllipse),
            Size = new Size(80, 100),
        };

        var saveButton = new Button()
        {
            Text = LanguageManager.Get(LanguageName.TopBarSave),
            Width = 75
        };

        var loadButton = new Button()
        {
            Text = LanguageManager.Get(LanguageName.TopBarOpen),
            Width = 75
        };

        var clearButton = new Button()
        {
            Text = LanguageManager.Get(LanguageName.TopBarClear),
            Width = 75
        };

        DeleteButton = new Button()
        {
            Text = LanguageManager.Get(LanguageName.TopBarDelete),
            Width = 75,
            Enabled = false
        };

        var label = new Label()
        {
            Text = App.Task,
            Width = -1,
            Height = -1,
            Wrap = WrapMode.Word,
            TextColor = ConfigManager.GetColor(Config.CanvasColor),
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };

        ColorPicker = new ColorPicker {
            Width = 100,
            AllowAlpha = true,
            Value = Color.FromArgb(255, 0, 0)
        };

        RotationStepper = new NumericStepper() {
            DecimalPlaces = 0,
            Enabled = false
        };

        rectangleButton.Click += RectangleButtonOnClick;
        triangleButton.Click += TriangleButtonOnClick;
        ellipseButton.Click += CircleButtonOnClick;
        saveButton.Click += SaveButtonOnClick;
        loadButton.Click += LoadButtonOnClick;
        clearButton.Click += ClearButtonOnClick;
        DeleteButton.Click += DeleteButtonOnClick;
        ColorPicker.ValueChanged += ColorPickerOnValueChanged;
        RotationStepper.ValueChanged += RotationStepperOnValueChanged;

        var layout = new DynamicLayout()
        {
            Padding = new(8),
            DefaultSpacing = new(5, 5)
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

    private void RectangleButtonOnClick(object sender, EventArgs e)
    {
        App.Tool = ShapeTool.Rectangle; 
        App.SideBar.ShowClassCard();
    }

    private void TriangleButtonOnClick(object sender, EventArgs e)
    {
        App.Tool = ShapeTool.Triangle;
        App.SideBar.ShowClassCard();
    }

    private void CircleButtonOnClick(object sender, EventArgs e)
    {
        App.Tool = ShapeTool.Ellipse;
        App.SideBar.ShowClassCard();
    }

    private void SaveButtonOnClick(object sender, EventArgs e)
    {
        var dialog = new SaveFileDialog()
        {
            FileName = "Checkpoint.olcp",
            Filters = {
                new FileFilter("O:L Checkpoint", new[] {".olcp"})
            }
        };

        if (dialog.ShowDialog(Application.Instance.MainForm) == DialogResult.Ok)
        {
            if (!dialog.FileName.EndsWith(".olcp")) {
                dialog.FileName += ".olcp";
            }

            var jsonString = JsonSerializer.Serialize(App.Serialize());
            App.CurrentFile = dialog.FileName;
            File.WriteAllText(dialog.FileName, jsonString);
        }
    }

    private void LoadButtonOnClick(object sender, EventArgs e)
    {
        var dialog = new OpenFileDialog()
        {
            Filters = {
                new FileFilter("O:L Checkpoint", new[] {".olcp"})
            }
        };
        
        if (dialog.ShowDialog(Application.Instance.MainForm) == DialogResult.Ok)
        {
            var program = JsonSerializer.Deserialize<SerializeableOLProgram>(File.ReadAllText(dialog.FileName));
            App.CurrentFile = dialog.FileName;
            App.Deserialize(program);
        }

        Draw();
    }

    private void ClearButtonOnClick(object sender, EventArgs e) {
        var random = new Random();
        var confirmButton = new Button {
            Text = random.Next(0, 1000000) == 1 ? "Jawui oida" : LanguageManager.Get(LanguageName.ConfirmClearDialogAccept),
        };

        confirmButton.Click += ConfirmActionDialogOnConfirmed;

        var abortButton = new Button {
            Text = LanguageManager.Get(LanguageName.ConfirmClearDialogDecline),
        };

        abortButton.Click += ConfirmActionDialogOnAborted;

        var content = new DynamicLayout()
        {
            Padding = new(8),
            DefaultSpacing = new(5, 5)
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
            Title = LanguageManager.Get(LanguageName.ConfirmClearDialogTitle),
        };

        _confirmActionDialog.NegativeButtons.Add(confirmButton);
        _confirmActionDialog.PositiveButtons.Add(abortButton);

        _confirmActionDialog.ShowModal(this);
    }

    private void ConfirmActionDialogOnConfirmed(object sender, EventArgs e) {
        App.TankVM.Visitor.Variables = VMVariables.DefaultVariables
            .ToDictionary(x => x.Key, x => x.Value); // = Clone
        App.Canvas.UpdateShapes();
        App.TeacherMode = false;
        App.SideBar.Reset();
        App.TopBar.Draw();
        App.CurrentFile = LanguageManager.Get(LanguageName.UiNoFileSelected);
        _confirmActionDialog.Close();
    }

    private void ConfirmActionDialogOnAborted(object sender, EventArgs e) {
        _confirmActionDialog.Close();
    }

    private void DeleteButtonOnClick(object sender, EventArgs e) {
        DeleteButton.Enabled = false;
        RotationStepper.Enabled = false;
        Invalidate();
        App.SideBar.Reset();
        App.TankVM.Visitor.Variables.Remove(App.Canvas.SelectedShape.ReferencedShape.VariableName);
        App.Canvas.SelectedShape = null;
        App.Canvas.UpdateShapes();
    }

    private void ColorPickerOnValueChanged(object sender, EventArgs e) {
        App.Canvas.SetColor(ColorPicker.Value);
    }

    private void RotationStepperOnValueChanged(object sender, EventArgs e) {
        App.Canvas.RotateSelectedShape((int)RotationStepper.Value);
    }
}
