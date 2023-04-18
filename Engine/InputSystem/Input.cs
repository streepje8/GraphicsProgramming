using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Striped.Engine.InputSystem;

public static class Input
{
    private static KeyboardState lastKeyboardState;
    public static void UpdateKeyboardState(KeyboardState newState) => lastKeyboardState = newState;

    public static bool GeyKey(Keys key) => lastKeyboardState.IsKeyDown(key);
    public static bool GeyKeyDown(Keys key) => lastKeyboardState.IsKeyPressed(key);
    public static bool GeyKeyUp(Keys key) => lastKeyboardState.IsKeyReleased(key);
    public static bool AnyKey() => lastKeyboardState.IsAnyKeyDown;
    public static bool WasKeyDown(Keys key) => lastKeyboardState.WasKeyDown(key);
}