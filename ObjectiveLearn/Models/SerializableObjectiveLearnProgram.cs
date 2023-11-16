using System.Collections.Generic;

namespace ObjectiveLearn.Models;

public class SerializableObjectiveLearnProgram
{
    public int FormCounter { get; set; }
    public bool TeacherMode { get; set; }
    public string Task { get; set; }
    public Dictionary<string, SerializableShape> Shapes { get; set; }
}