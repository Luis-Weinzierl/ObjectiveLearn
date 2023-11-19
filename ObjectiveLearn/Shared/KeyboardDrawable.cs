using System;
using Eto.Forms;

namespace ObjectiveLearn.Shared;

public abstract class KeyboardDrawable : Drawable
{
    public abstract void HandleKeyDown(KeyEventArgs e);
    public abstract void HandleKeyUp(KeyEventArgs e);
}