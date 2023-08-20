using TankLite.Values;
using Shared.Localisation;

namespace ObjectiveLearn.Models;

public static class TLName
{
    public static string Rectangle;
    public static string Triangle;
    public static string Ellipse;

    public static TLString RectangleType;
    public static TLString TriangleType;
    public static TLString EllipseType;

    public static string Constructor;
    public static string Type;
    public static string XPos;
    public static string YPos;
    public static string Width;
    public static string Height;
    public static string Rotation;

    public static string Color;

    public static string Red;
    public static string Green;
    public static string Blue;
    public static string Alpha;

    public static string Int;
    public static string Object;
    public static string Func;

    public static string SetPosition;
    public static string SetSize;
    public static string SetColor;
    public static string SetRotation;


    public static void Init()
    {
        Rectangle = LanguageManager.Get(LanguageName.TLNameRectangle);
        Triangle = LanguageManager.Get(LanguageName.TLNameTriangle);
        Ellipse = LanguageManager.Get(LanguageName.TLNameEllipse);
        RectangleType = new(Rectangle);
        TriangleType = new(Triangle);
        EllipseType = new(Ellipse);
        Constructor = LanguageManager.Get(LanguageName.TLNameConstructor);
        Type = LanguageManager.Get(LanguageName.TLNameType);
        XPos = LanguageManager.Get(LanguageName.TLNameXPos);
        YPos = LanguageManager.Get(LanguageName.TLNameYPos);
        Width = LanguageManager.Get(LanguageName.TLNameWidth);
        Height = LanguageManager.Get(LanguageName.TLNameHeight);
        Rotation = LanguageManager.Get(LanguageName.TLNameRotation);
        Color = LanguageManager.Get(LanguageName.TLNameColor);
        Red = LanguageManager.Get(LanguageName.TLNameRed);
        Green = LanguageManager.Get(LanguageName.TLNameGreen);
        Blue = LanguageManager.Get(LanguageName.TLNameBlue);
        Alpha = LanguageManager.Get(LanguageName.TLNameAlpha);
        Int = LanguageManager.Get(LanguageName.TLNameInt);
        Object = LanguageManager.Get(LanguageName.TLNameObject);
        Func = LanguageManager.Get(LanguageName.TLNameFunc);
        SetPosition = LanguageManager.Get(LanguageName.TLNameSetPosition);
        SetSize = LanguageManager.Get(LanguageName.TLNameSetSize);
        SetColor = LanguageManager.Get(LanguageName.TLNameSetColor);
        SetRotation = LanguageManager.Get(LanguageName.TLNameSetRotation);
    }
}
