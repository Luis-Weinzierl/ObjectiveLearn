using ObjectiveLearn.Components;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shapes;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Shared.Localization;
using TankLite;
using TankLite.Values;

namespace ObjectiveLearn.Shared;

public static class App
{
    public static TankLiteRuntimeEnvironment TankVm { get; set; }

    public static string Task { get; set; } = string.Empty;

    public static bool TeacherMode { get; set; }

    public static ShapeTool Tool { get; set; } = ShapeTool.Triangle;

    public static string Directory { get; set; }
    public static string CurrentFile { get; set; }

    public static TopBar TopBar { get; set; }
    public static Canvas Canvas { get; set; }
    public static ConsoleBar ConsoleBar { get; set; }
    public static SideBar SideBar { get; set; }
    public static Font TextFont { get; set; }
    public static Font SmallTextFont { get; set; }


    public static void Initialize()
    {
        Directory   = System.AppContext.BaseDirectory;

        TopBar          = new TopBar();
        Canvas          = new Canvas();
        SideBar         = new SideBar();
        ConsoleBar      = new ConsoleBar();
        TankVm          = new TankLiteRuntimeEnvironment(VmVariables.DefaultVariables);
        TextFont        = new Font(SystemFont.Default, 12);
        SmallTextFont   = new Font(SystemFont.Default, 8);

        CurrentFile = LanguageManager.Get(LanguageName.UiNoFileSelected);

        ConsoleBar.UpdateShapes += (_, _) => Canvas.UpdateShapes();
        Canvas.SelectShape      += (_, e) => SideBar.SelectObject(e);
    }

    public static SerializableObjectiveLearnProgram Serialize()
    {
        TopBar.DeleteButton.Enabled = false;
        TopBar.RotationStepper.Enabled = false;
        var shapes = new Dictionary<string, SerializableShape>();

        foreach (var kv in TankVm.Visitor.Variables)
        {
            if (kv.Value.Type != TlName.Object || !((TlObj)kv.Value).Value.ContainsKey(TlName.Type))
            {
                continue;
            }

            var obj = ((TlObj)kv.Value).Value;
            var color = ((TlObj)obj[TlName.Color]).Value;

            shapes[kv.Key] = new SerializableShape
            {
                Type = ((TlString)obj[TlName.Type]).Value,
                X = ((TlInt)obj[TlName.XPos]).Value,
                Y = ((TlInt)obj[TlName.YPos]).Value,
                Width = ((TlInt)obj[TlName.Width]).Value,
                Height = ((TlInt)obj[TlName.Height]).Value,
                Rotation = ((TlInt)obj[TlName.Rotation]).Value,
                R = ((TlInt)color[TlName.Red]).Value,
                G = ((TlInt)color[TlName.Green]).Value,
                B = ((TlInt)color[TlName.Blue]).Value,
                A = ((TlInt)color[TlName.Alpha]).Value
            };
        }

        SideBar.Reset();

        return new SerializableObjectiveLearnProgram
        {
            FormCounter = Shape.IdCounter,
            TeacherMode = TeacherMode,
            Task = Task,
            Shapes = shapes
        };
    }

    public static void Deserialize(SerializableObjectiveLearnProgram program)
    {
        TopBar.DeleteButton.Enabled = false;
        TopBar.RotationStepper.Enabled = false;
        var dict = new Dictionary<string, TlValue>();

        foreach (var kv in program.Shapes)
        {
            dict[kv.Key] = ShapeHelpers.CreateShape(kv.Value);
        }

        TankVm.Visitor.Variables = dict
            .Concat(VmVariables.DefaultVariables)
            .ToDictionary(x => x.Key, x => x.Value);
        TeacherMode = program.TeacherMode;
        Task = program.Task;
        Shape.IdCounter = program.FormCounter;

        TopBar.ColorPicker.Enabled = !TeacherMode;

        Canvas.UpdateShapes();
    }
}
