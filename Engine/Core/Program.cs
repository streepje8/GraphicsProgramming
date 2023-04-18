using System.Reflection;
using GraphicsProgramming;
using GraphicsProgramming.Engine.Core;

public class Program
{
    public static void Main(string[] args)
    {
        Game? game = null;
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in a.GetTypes())
            {
                if (t.IsSubclassOf(typeof(Game)) && t != typeof(Game))
                {
                    game = Activator.CreateInstance(t) as Game;
                }
            }
        }
        if (game == null) throw new Exception("No game class found in the current app domain! Get started by extending the 'Game' class.");
        using (Window mainWindow = new Window(new OpenGLRenderer(),800, 400, "WOO"))
        {
            mainWindow.ActiveGame = game;
            mainWindow.Initialize();
            mainWindow.UpdateFrequency = 600;
            mainWindow.Run();
            mainWindow.Clean();
        }
    }
}