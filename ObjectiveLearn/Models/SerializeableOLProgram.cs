using System.Collections.Generic;

namespace ObjectiveLearn.Models;

public class SerializeableOLProgram
{
    public int FormCounter { get; set; }
    public bool TeacherMode { get; set; }
    public Dictionary<string, SerializeableShape> Shapes { get; set; }
}