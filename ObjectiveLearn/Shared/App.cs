using ObjectiveLearn.Components;
using ObjectiveLearn.Models;
using ObjectiveLearn.Shapes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TankLite;
using TankLite.Values;

namespace ObjectiveLearn.Shared;

public static class App
{
    public static TankLiteRuntimeEnvironment TankVM { get; set; }

    public static bool TeacherMode { get; set; }

    public static ShapeTool Tool { get; set; } = ShapeTool.Triangle;

    public static string Directory { get; set; }

    public static TopBar TopBar { get; set; }
    public static Canvas Canvas { get; set; }
    public static ConsoleBar ConsoleBar { get; set; }
    public static SideBar SideBar { get; set; }

    public static void Initialize()
    {
        Directory   = System.AppContext.BaseDirectory;

        TopBar      = new();
        Canvas      = new();
        SideBar     = new();
        ConsoleBar  = new();
        TankVM      = new(VMVariables.DefaultVariables);

        ConsoleBar.UpdateShapes += (s, e) => Canvas.UpdateShapes();
        Canvas.SelectShape      += (s, e) => SideBar.SelectObject(e);
    }

    public static SerializeableOLProgram Serialize()
    {
        var shapes = new Dictionary<string, SerializeableShape>();

        foreach (var kv in App.TankVM.Visitor.Variables)
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
            FormCounter = Shape.IdCounter,
            TeacherMode = App.TeacherMode,
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

        TankVM.Visitor.Variables = dict
            .Concat(VMVariables.DefaultVariables)
            .ToDictionary(x => x.Key, x => x.Value);
        TeacherMode = program.TeacherMode;
        Shape.IdCounter = program.FormCounter;

        Canvas.UpdateShapes();
    }
}
