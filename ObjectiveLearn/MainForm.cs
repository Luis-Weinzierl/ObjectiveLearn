using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Components;
using System;
using System.IO;
using TankLite;

namespace ObjectiveLearn
{
	public partial class MainForm : Form, IDisposable
	{
        public static TankLiteRuntimeEnvironment TLEnv { get; set; }

        private TopBar TopBar { get; set; }
        private Canvas Canvas { get; set; }
        private ConsoleBar Console { get; set; }
        private SideBar SideBar { get; set; }

        public MainForm(string title = "Objective: Learn")
		{
			Title = title;
			MinimumSize = new Size(200, 200);
			Size = new Size(
				(int)Screen.Bounds.Width - 200,
				(int)Screen.Bounds.Height - 200);

			Location = new Point(100, 100);

            BackgroundColor = ConfigurationManager.GetColor(Config.WindowColor);

            TopBar = new();
            Canvas = new()
            {
                Location = new(0, 0)
            };
            SideBar = new();
            Console = new();

            Console.UpdateShapes += (s, e) => Canvas.UpdateShapes();
            Canvas.SelectShape += (s, e) => SideBar.SelectObject(e);

            var layout = new DynamicLayout();

            layout.BeginHorizontal(true);
            layout.BeginVertical(null, null, true, true);
            layout.Add(TopBar, true, false);
            layout.Add(Canvas, true, true);
            layout.Add(Console, true, false);
            layout.EndBeginVertical();
            layout.Add(SideBar, false, true);
            layout.EndHorizontal();

            Content = layout;
        }

        public new void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
