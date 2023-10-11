using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using System;
using System.Text.Json;
using System.IO;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using ObjectiveLearn.Shared;
using Shared.Localisation;
using TankLite.Values;

namespace ObjectiveLearn.Components;

public class TopBar : Drawable
{
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
            BackgroundColor = ConfigManager.GetColor(Config.WindowColor),
            TextColor = ConfigManager.GetColor(Config.CanvasColor)
        };

        var triangleButton = new Button()
        {
            Image = new Bitmap(Path.Combine(App.Directory, "Resources/TriangleIcon.png")),
            ImagePosition = ButtonImagePosition.Above,
            Text = LanguageManager.Get(LanguageName.TopBarTriangle),
            Size = new Size(80, 100),
            BackgroundColor = ConfigManager.GetColor(Config.WindowColor),
            TextColor = ConfigManager.GetColor(Config.CanvasColor)
        };

        var ellipseButton = new Button()
        {
            Image = new Bitmap(Path.Combine(App.Directory, "Resources/CircleIcon.png")),
            ImagePosition = ButtonImagePosition.Above,
            Text = LanguageManager.Get(LanguageName.TopBarEllipse),
            Size = new Size(80, 100),
            BackgroundColor = ConfigManager.GetColor(Config.WindowColor),
            TextColor = ConfigManager.GetColor(Config.CanvasColor),
        };

        var saveButton = new Button()
        {
            Text = LanguageManager.Get(LanguageName.TopBarSave),
            Width = 50
        };

        var loadButton = new Button()
        {
            Text = LanguageManager.Get(LanguageName.TopBarOpen),
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

        rectangleButton.Click += RectangleButtonOnClick;
        triangleButton.Click += TriangleButtonOnClick;
        ellipseButton.Click += CircleButtonOnClick;
        saveButton.Click += SaveButtonOnClick;
        loadButton.Click += LoadButtonOnClick;

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

        layout.EndVertical();

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
            Filters = {
                new FileFilter("O:L Checkpoint", new[] {".olcp"})
            }
        };

        if (dialog.ShowDialog(Application.Instance.MainForm) == DialogResult.Ok)
        {
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
}
