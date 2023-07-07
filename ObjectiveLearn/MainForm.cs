using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shared;
using System;

namespace ObjectiveLearn
{
    public partial class MainForm : Form, IDisposable
	{
        public MainForm(string title = "Objective: Learn")
		{
            App.Initialize();

            Icon = new Icon(System.IO.Path.Combine(App.Directory, "WinIcon.ico"));
			Title = title;
			MinimumSize = new Size(200, 200);
			Size = new Size(800, 500);

			Location = new Point(100, 100);

            BackgroundColor = ConfigManager.GetColor(Config.WindowColor);

            var layout = new DynamicLayout();

            layout.BeginHorizontal(true);
            layout.BeginVertical(null, null, true, true);
            layout.Add(App.TopBar, true, false);
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
	}
}
