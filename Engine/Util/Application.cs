using Striped.Engine.Rendering.Core;

namespace Striped.Engine.Util;

public static class Application
{
    public static EngineWindow? Window { get; private set; } = null;

    public static void SetWindow(EngineWindow win)
    {
        if (Window == null) Window = win;
        else throw new Exception("You are not allowed to overwrite the current window!");
    }
    public static void Quit()
    {
        Window?.Close();
    }
}