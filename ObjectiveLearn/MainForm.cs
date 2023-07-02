using Eto.Drawing;
using Eto.Forms;
using ObjectiveLearn.Components;
using ObjectiveLearn.Shared;
using System;
using System.Collections.Generic;
using TankLite;
using TankLite.Values;
using ObjectiveLearn.Models;

namespace ObjectiveLearn
{
	public partial class MainForm : Form, IDisposable
	{
        public static TankLiteRuntimeEnvironment TLEnv { get; set; }
        public static bool TeacherMode = false;
        public static string AppPath { get; private set; }

        private static Canvas _canvas;

        public MainForm(string title = "Objective: Learn")
		{
            var exeFullName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            AppPath = System.IO.Path.GetDirectoryName(exeFullName);

            Icon = new Icon(System.IO.Path.Combine(AppPath, "WinIcon.ico"));
			Title = title;
			MinimumSize = new Size(200, 200);
			Size = new Size(800, 500);

			Location = new Point(100, 100);

            BackgroundColor = ConfigurationManager.GetColor(Config.WindowColor);

            var topBar = new TopBar();
            _canvas = new Canvas();
            var sideBar = new SideBar();
            var console = new ConsoleBar();

            console.UpdateShapes += (s, e) => _canvas.UpdateShapes();
            _canvas.SelectShape += (s, e) => sideBar.SelectObject(e);

            var layout = new DynamicLayout();

            layout.BeginHorizontal(true);
            layout.BeginVertical(null, null, true, true);
            layout.Add(topBar, true, false);
            layout.Add(_canvas, true, true);
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

        public static SerializeableOLProgram Serialize()
        {
            var shapes = new Dictionary<string, SerializeableShape>();

            foreach (var kv in TLEnv.Visitor.Variables)
            {
                if (kv.Value.Type != TLName.Object || !((TLObj)kv.Value).Value.ContainsKey(TLName.Type))
                {
                    continue;
                }

                var obj = ((TLObj)kv.Value).Value;
                var color = ((TLObj)obj[TLName.Color]).Value;

                shapes[kv.Key] = new()
                {
                    Type = ((TLString)obj[TLName.Type]).Value,                    
                    X = ((TLInt)obj[TLName.XPos]).Value,                    
                    Y = ((TLInt)obj[TLName.YPos]).Value,                    
                    Width = ((TLInt)obj[TLName.Width]).Value,                    
                    Height = ((TLInt)obj[TLName.Height]).Value,
                    Rotation = ((TLInt)obj[TLName.Rotation]).Value,
                    R = ((TLInt)color[TLName.Red]).Value,
                    G = ((TLInt)color[TLName.Green]).Value,
                    B = ((TLInt)color[TLName.Blue]).Value,
                    A = ((TLInt)color[TLName.Alpha]).Value,
                };
            }

            return new()
            {
                TeacherMode = TeacherMode,
                Shapes = shapes
            };
        }

        public static void Deserialize(SerializeableOLProgram program)
        {
            var dict = new Dictionary<string, TLValue>();

            foreach (var kv in program.Shapes)
            {
                dict[kv.Key] = ShapeHelpers.CreateShape(kv.Value);
            }

            TLEnv.Visitor.Variables = dict;
            TeacherMode = program.TeacherMode;

            _canvas.UpdateShapes();
        }
	}
}
