using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Striped.Engine.InputSystem;

public static class Input
{
    private static KeyboardState? LastKeyboardState;
    public static void UpdateKeyboardState(KeyboardState? newState) => LastKeyboardState = newState;

    public static bool GetKey(Keys key) => LastKeyboardState != null && LastKeyboardState.IsKeyDown(key);
    public static bool GetKeyDown(Keys key) => LastKeyboardState != null && LastKeyboardState.IsKeyPressed(key);
    public static bool GetKeyUp(Keys key) => LastKeyboardState != null && LastKeyboardState.IsKeyReleased(key);
    public static bool AnyKey() => LastKeyboardState != null && LastKeyboardState.IsAnyKeyDown;
    public static bool WasKeyDown(Keys key) => LastKeyboardState != null && LastKeyboardState.WasKeyDown(key);
}