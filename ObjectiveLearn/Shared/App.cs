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
    public static Font SmallTextFontUnderline { get; set; }


    public static void Initialize()
    {
        Directory   = System.AppContext.BaseDirectory;

        TextFont                 = new Font(SystemFont.Default, 12);
        SmallTextFont            = new Font(SystemFont.Default, 8);
        SmallTextFontUnderline   = new Font(SystemFont.Default, 8, FontDecoration.Underline); 

        TopBar                   = new TopBar();
        Canvas                   = new Canvas();
        SideBar                  = new SideBar();
        ConsoleBar               = new ConsoleBar();
        TankVm                   = new TankLiteRuntimeEnvironment(VmVariables.DefaultVariables); 

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
            if (kv.Value.Type != TankLiteName.Object || !((TankLiteObj)kv.Value).Value.ContainsKey(TankLiteName.Type))
            {
                continue;
            }

            var obj = ((TankLiteObj)kv.Value).Value;
            var color = ((TankLiteObj)obj[TankLiteName.Color]).Value;

            shapes[kv.Key] = new SerializableShape
            {
                Type = ((TankLiteString)obj[TankLiteName.Type]).Value,
                X = ((TankLiteInt)obj[TankLiteName.XPos]).Value,
                Y = ((TankLiteInt)obj[TankLiteName.YPos]).Value,
                Width = ((TankLiteInt)obj[TankLiteName.Width]).Value,
                Height = ((TankLiteInt)obj[TankLiteName.Height]).Value,
                Rotation = ((TankLiteInt)obj[TankLiteName.Rotation]).Value,
                R = ((TankLiteInt)color[TankLiteName.Red]).Value,
                G = ((TankLiteInt)color[TankLiteName.Green]).Value,
                B = ((TankLiteInt)color[TankLiteName.Blue]).Value,
                A = ((TankLiteInt)color[TankLiteName.Alpha]).Value
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
        var dict = new Dictionary<string, TankLiteValue>();

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
