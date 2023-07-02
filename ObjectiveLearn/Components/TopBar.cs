using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using System;
using System.Diagnostics;

namespace ObjectiveLearn.Components;

public class TopBar : Drawable
{
	public new Point Location { get; set; }
	public TopBar()
    {
        var buttonSize = new Size(80, 100);
        var rectangleButton = new Button()
        {
            Image = new Bitmap("Resources/RectangleIcon.png"),
            ImagePosition = ButtonImagePosition.Above,
            Text = "Rechteck",
            Size = new Size(80, 100),
            BackgroundColor = ConfigurationManager.GetColor(Config.WindowColor),
            TextColor = ConfigurationManager.GetColor(Config.CanvasColor)
        };

        var triangleButton = new Button()
        {
            Image = new Bitmap("Resources/TriangleIcon.png"),
            ImagePosition = ButtonImagePosition.Above,
            Text = "Dreieck",
            Size = new Size(80, 100),
            BackgroundColor = ConfigurationManager.GetColor(Config.WindowColor),
            TextColor = ConfigurationManager.GetColor(Config.CanvasColor)
        };

        var ellipseButton = new Button()
        {
            Image = new Bitmap("Resources/CircleIcon.png"),
            ImagePosition = ButtonImagePosition.Above,
            Text = "Ellipse",
            Size = new Size(80, 100),
            BackgroundColor = ConfigurationManager.GetColor(Config.WindowColor),
            TextColor = ConfigurationManager.GetColor(Config.CanvasColor),
        };

        rectangleButton.Click += RectangleButtonOnClick;
        triangleButton.Click += TriangleButtonOnClick;
        ellipseButton.Click += CircleButtonOnClick;

        var layout = new StackLayout()
        {
            Padding = new(8),
            Spacing = 5,
            Orientation = Orientation.Horizontal,
            Items =
            {
                rectangleButton,
                triangleButton,
                ellipseButton
            }
        };

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
}
