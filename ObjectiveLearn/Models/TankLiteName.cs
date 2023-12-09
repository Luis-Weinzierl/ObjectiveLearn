using TankLite.Values;
using Shared.Localization;

namespace ObjectiveLearn.Models;

public static class TankLiteName
{
    public static string Rectangle { get; set; }
    public static string Triangle { get; set; }
    public static string Ellipse { get; set; }

    public static TankLiteString RectangleType { get; set; }
    public static TankLiteString TriangleType { get; set; }
    public static TankLiteString EllipseType { get; set; }
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
        Rectangle = LanguageManager.Get(LanguageName.TankLiteNameRectangle);
        Triangle = LanguageManager.Get(LanguageName.TankLiteNameTriangle);
        Ellipse = LanguageManager.Get(LanguageName.TankLiteNameEllipse);
        RectangleType = new TankLiteString(Rectangle);
        TriangleType = new TankLiteString(Triangle);
        EllipseType = new TankLiteString(Ellipse);
        Constructor = LanguageManager.Get(LanguageName.TankLiteNameConstructor);
        Type = LanguageManager.Get(LanguageName.TankLiteNameType);
        XPos = LanguageManager.Get(LanguageName.TankLiteNameXPos);
        YPos = LanguageManager.Get(LanguageName.TankLiteNameYPos);
        Width = LanguageManager.Get(LanguageName.TankLiteNameWidth);
        Height = LanguageManager.Get(LanguageName.TankLiteNameHeight);
        Rotation = LanguageManager.Get(LanguageName.TankLiteNameRotation);
        Color = LanguageManager.Get(LanguageName.TankLiteNameColor);
        Red = LanguageManager.Get(LanguageName.TankLiteNameRed);
        Green = LanguageManager.Get(LanguageName.TankLiteNameGreen);
        Blue = LanguageManager.Get(LanguageName.TankLiteNameBlue);
        Alpha = LanguageManager.Get(LanguageName.TankLiteNameAlpha);
        Int = LanguageManager.Get(LanguageName.TankLiteNameInt);
        Object = LanguageManager.Get(LanguageName.TankLiteNameObject);
        Func = LanguageManager.Get(LanguageName.TankLiteNameFunc);
        SetPosition = LanguageManager.Get(LanguageName.TankLiteNameSetPosition);
        SetSize = LanguageManager.Get(LanguageName.TankLiteNameSetSize);
        SetColor = LanguageManager.Get(LanguageName.TankLiteNameSetColor);
        SetRotation = LanguageManager.Get(LanguageName.TankLiteNameSetRotation);
        Move = LanguageManager.Get(LanguageName.TankLiteNameMove);
    }
}
