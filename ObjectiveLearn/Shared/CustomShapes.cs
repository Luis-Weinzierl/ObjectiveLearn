using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveLearn.Shared;

public class CustomShapes
{
    public static void DrawRoundedRectangle(Graphics g, int x, int y, int w, int h, int r, Color c)
    {
        var path = new GraphicsPath();

        path.AddArc(x, y, r, r, -90, -90);
        path.AddLine(x, y + r, x, y + h - r);
        path.AddArc(x, y + h - r, r, r, 180, -90);
        path.AddLine(x + r, y + h, x + w - r, y + h);
        path.AddArc(x + w - r, y + h - r, r, r, -270, -90);
        path.AddLine(x + w, y + h - r, x + w, y + r);
        path.AddArc(x + w - r, y, r, r, 0, -90);
        path.CloseFigure();

        g.FillPath(c, path);
    }

    public static void DrawRoundedRectangleL(Graphics g, int x, int y, int w, int h, int r, Color c)
    {
        var path = new GraphicsPath();

        path.AddArc(x, y, r, r, -90, -90);
        path.AddLine(x, y + r, x, y + h - r);
        path.AddArc(x, y + h - r, r, r, 180, -90);
        path.AddLine(x, y + h, x + w, y + h);
        path.AddLine(x + w, y + h, x + w, y);
        path.CloseFigure();

        g.FillPath(c, path);
    }

    public static void DrawRoundedRectangleR(Graphics g, int x, int y, int w, int h, int r, Color c)
    {
        var path = new GraphicsPath();

        path.AddLine(x, y, x, y + h);
        path.AddLine(x, y + h, x + w, y + h);
        path.AddArc(x + w - r, y + h - r, r, r, -270, -90);
        path.AddLine(x + w, y + h - r, x + w, y + r);
        path.AddArc(x + w - r, y, r, r, 0, -90);
        path.CloseFigure();

        g.FillPath(c, path);
    }
}
