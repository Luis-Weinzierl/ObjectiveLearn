﻿using TankLite.Values;

namespace ObjectiveLearn.Models;

public class SelectShapeEventArgs
{
    public string VariableName { get; set; }

    public string ShapeType { get; set; }

    public TankLiteObj Shape { get; set; }
}
