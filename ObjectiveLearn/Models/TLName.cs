using TankLite.Values;
using Shared.Localization;

namespace ObjectiveLearn.Models;

public static class TlName
{
    public static string Rectangle { get; set; }
    public static string Triangle { get; set; }
    public static string Ellipse { get; set; }

    public static TlString RectangleType { get; set; }
    public static TlString TriangleType { get; set; }
    public static TlString EllipseType { get; set; }
    public static string Constructor { get; set; }

    public static string Type { get; set; }
    public static string XPos { get; set; }
    public static string YPos { get; set; }
    public static string Width { get; set; }
    public static string Height { get; set; }
    public static string Rotation { get; set; }
    public static string Color { get; set; }
    public static string Red { get; set; }
    public static string Green { get; set; }
    public static string Blue { get; set; }
    public static string Alpha { get; set; }
    public static string Int { get; set; }
    public static string Object { get; set; }
    public static string Func { get; set; }
    public static string SetPosition { get; set; }
    public static string SetSize { get; set; }
    public static string SetColor { get; set; }
    public static string SetRotation { get; set; }
    public static string Move { get; set; }


    public static void Init()
    {
        Rectangle = LanguageManager.Get(LanguageName.TlNameRectangle);
        Triangle = LanguageManager.Get(LanguageName.TlNameTriangle);
        Ellipse = LanguageManager.Get(LanguageName.TlNameEllipse);
        RectangleType = new TlString(Rectangle);
        TriangleType = new TlString(Triangle);
        EllipseType = new TlString(Ellipse);
        Constructor = LanguageManager.Get(LanguageName.TlNameConstructor);
        Type = LanguageManager.Get(LanguageName.TlNameType);
        XPos = LanguageManager.Get(LanguageName.TlNameXPos);
        YPos = LanguageManager.Get(LanguageName.TlNameYPos);
        Width = LanguageManager.Get(LanguageName.TlNameWidth);
        Height = LanguageManager.Get(LanguageName.TlNameHeight);
        Rotation = LanguageManager.Get(LanguageName.TlNameRotation);
        Color = LanguageManager.Get(LanguageName.TlNameColor);
        Red = LanguageManager.Get(LanguageName.TlNameRed);
        Green = LanguageManager.Get(LanguageName.TlNameGreen);
        Blue = LanguageManager.Get(LanguageName.TlNameBlue);
        Alpha = LanguageManager.Get(LanguageName.TlNameAlpha);
        Int = LanguageManager.Get(LanguageName.TlNameInt);
        Object = LanguageManager.Get(LanguageName.TlNameObject);
        Func = LanguageManager.Get(LanguageName.TlNameFunc);
        SetPosition = LanguageManager.Get(LanguageName.TlNameSetPosition);
        SetSize = LanguageManager.Get(LanguageName.TlNameSetSize);
        SetColor = LanguageManager.Get(LanguageName.TlNameSetColor);
        SetRotation = LanguageManager.Get(LanguageName.TlNameSetRotation);
        Move = LanguageManager.Get(LanguageName.TlNameMove);
    }
}
