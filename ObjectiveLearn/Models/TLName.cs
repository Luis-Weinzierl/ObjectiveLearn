using TankLite.Values;

namespace ObjectiveLearn.Models;

public static class TLName
{
    public const string Rectangle = "RECHTECK";
    public const string Triangle = "DREIECK";
    public const string Ellipse = "ELLIPSE";

    public static readonly TLString RectangleType = new(Rectangle);
    public static readonly TLString TriangleType = new(Triangle);
    public static readonly TLString EllipseType = new(Ellipse);

    public const string Constructor = ".ctor";
    public const string Type = ".type";
    public const string XPos = "X";
    public const string YPos = "Y";
    public const string Width = "Breite";
    public const string Height= "Hoehe";
    public const string Rotation = "Rotation";

    public const string Color = "Farbe";

    public const string Red = "R";
    public const string Green = "G";
    public const string Blue = "B";
    public const string Alpha = "A";

    public const string Int = "int";
    public const string Object = "object";
}
