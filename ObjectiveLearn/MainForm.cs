using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shared;
using System;

namespace ObjectiveLearn;

public class MainForm : Form, IDisposable
{
    public MainForm(string title = "Objective: Learn")
	{
        App.Initialize();

        Icon = new Icon(System.IO.Path.Combine(App.Directory, "WinIcon.ico"));
		Title = title;
		MinimumSize = new Size(1020, 580);
		Handler.Size = new Size(1020, 600);

		Location = new Point(100, 100);

        BackgroundColor = ConfigManager.GetColor(Config.WindowColor);

        var layout = new DynamicLayout();

        layout.BeginHorizontal(true);
        layout.BeginVertical(null, null, true, true);
        layout.Add(App.ToolBar, true, false);
        layout.Add(App.Canvas, true, true);
        layout.Add(App.ConsoleBar, true, false);
        layout.EndVertical();
        layout.Add(App.SideBar, false, true);
        layout.EndHorizontal();

        Content = layout;

    }

    public new void Dispose()
	{
		GC.SuppressFinalize(this);
	}

    protected override void OnKeyDown(KeyEventArgs e)
    {
        KeyHandler.FocusedComponent?.HandleKeyDown(e);

        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        KeyHandler.FocusedComponent?.HandleKeyUp(e);

        base.OnKeyUp(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        KeyHandler.FocusedComponent?.Deactivate();
    }
}
