namespace ObjectiveLearn.Shared;

public static class KeyHandler
{
    private static KeyboardDrawable _focusedComponent;

    public static KeyboardDrawable FocusedComponent
    {
        get => _focusedComponent;
        set
        {
            _focusedComponent?.Deactivate();
            _focusedComponent = value;
        }
    }
}