using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Components;
using System;
using TankLite;

namespace ObjectiveLearn
{
	public partial class MainForm : Form, IDisposable
	{
        public static TankLiteRuntimeEnvironment TLEnv { get; set; }

        public MainForm(string title = "Objective: Learn")
		{
			Title = title;
			MinimumSize = new Size(200, 200);
			Size = new Size(800, 500);

			Location = new Point(100, 100);

            BackgroundColor = ConfigurationManager.GetColor(Config.WindowColor);

            var topBar = new TopBar();
            var canvas = new Canvas()
            {
                Location = new(0, 0)
            };
            var sideBar = new SideBar();
            var console = new ConsoleBar();

            console.UpdateShapes += (s, e) => canvas.UpdateShapes();
            canvas.SelectShape += (s, e) => sideBar.SelectObject(e);

            var layout = new DynamicLayout();

            layout.BeginHorizontal(true);
            layout.BeginVertical(null, null, true, true);
            layout.Add(topBar, true, false);
            layout.Add(canvas, true, true);
            layout.Add(console, true, false);
            layout.EndBeginVertical();
            layout.Add(sideBar, false, true);
            layout.EndHorizontal();

            Content = layout;
        }

        public new void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
