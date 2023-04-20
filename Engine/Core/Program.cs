using System.Reflection;
using Striped.Engine.Core;
using Striped.Engine.Rendering.Core;
using Striped.Engine.Rendering.TemplateRenderers;

namespace Striped.Engine.Bootstrapper;

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
        new GameSession(game);
        using (EngineWindow mainEngineWindow = new EngineWindow(new OpenGLRenderer(),game.Width, game.Height, game.Title))
        {
            game?.Init();
            mainEngineWindow.Run();
            mainEngineWindow.Clean();
        }
    }
}