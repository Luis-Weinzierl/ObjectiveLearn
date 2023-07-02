using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.IO;

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
            Image = new Bitmap(System.IO.Path.Combine(MainForm.AppPath, "Resources/RectangleIcon.png")),
            ImagePosition = ButtonImagePosition.Above,
            Text = "Rechteck",
            Size = new Size(80, 100),
            BackgroundColor = ConfigurationManager.GetColor(Config.WindowColor),
            TextColor = ConfigurationManager.GetColor(Config.CanvasColor)
        };

        var triangleButton = new Button()
        {
            Image = new Bitmap(System.IO.Path.Combine(MainForm.AppPath, "Resources/TriangleIcon.png")),
            ImagePosition = ButtonImagePosition.Above,
            Text = "Dreieck",
            Size = new Size(80, 100),
            BackgroundColor = ConfigurationManager.GetColor(Config.WindowColor),
            TextColor = ConfigurationManager.GetColor(Config.CanvasColor)
        };

        var ellipseButton = new Button()
        {
            Image = new Bitmap(System.IO.Path.Combine(MainForm.AppPath, "Resources/CircleIcon.png")),
            ImagePosition = ButtonImagePosition.Above,
            Text = "Ellipse",
            Size = new Size(80, 100),
            BackgroundColor = ConfigurationManager.GetColor(Config.WindowColor),
            TextColor = ConfigurationManager.GetColor(Config.CanvasColor),
        };

        var saveButton = new Button()
        {
            Text = "Speichern"
        };

        var loadButton = new Button()
        {
            Text = "Öffnen"
        };

        rectangleButton.Click += RectangleButtonOnClick;
        triangleButton.Click += TriangleButtonOnClick;
        ellipseButton.Click += CircleButtonOnClick;
        saveButton.Click += SaveButtonOnClick;
        loadButton.Click += LoadButtonOnClick;

        var layout = new StackLayout()
        {
            Padding = new(8),
            Spacing = 5,
            Orientation = Orientation.Horizontal
        };

        if (!MainForm.TeacherMode)
        {
            layout.Items.Add(rectangleButton);
            layout.Items.Add(triangleButton);
            layout.Items.Add(ellipseButton);    
        }

        layout.Items.Add(saveButton);
        layout.Items.Add(loadButton);

        Content = layout;
    }

    private void RectangleButtonOnClick(object sender, EventArgs e)
    {
        Canvas.Tool = ShapeTool.Rectangle;
    }

    private void TriangleButtonOnClick(object sender, EventArgs e)
    {
        Canvas.Tool = ShapeTool.Triangle;
    }

    private void CircleButtonOnClick(object sender, EventArgs e)
    {
        Canvas.Tool = ShapeTool.Ellipse;
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
            var jsonString = JsonSerializer.Serialize(MainForm.Serialize());
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
            MainForm.Deserialize(program);
        }

        Draw();
    }
}
