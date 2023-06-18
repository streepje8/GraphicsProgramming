using System.Reflection;
using Striped.Engine.Core;
using Striped.Engine.Rendering.Core;
using Striped.Engine.Rendering.TemplateRenderers;
using Striped.Engine.Util;

namespace Striped.Engine.Bootstrapper;

public class Program
{
    public static void Main(string[] args)
    {
        try
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

            if (game == null)
                throw new Exception(
                    "No game class found in the current app domain! Get started by extending the 'Game' class.");
            GameSession session = new GameSession(game);
            using (EngineWindow mainEngineWindow =
                   new EngineWindow(Activator.CreateInstance(game.Renderer) as Renderer ?? new OpenGLRenderer(),
                       game.Width, game.Height, game.Title))
            {
                session.BindWindow(mainEngineWindow);
                game?.Init();
                mainEngineWindow.Run();
                mainEngineWindow.Clean();
            }
        }
        catch (Exception e)
        {
            Logger.Err(e.InnerException?.Message ?? e.Message);
            Logger.Err(e.InnerException?.StackTrace ?? e.StackTrace ?? "No stacktrace provided...");
        }
    }
}