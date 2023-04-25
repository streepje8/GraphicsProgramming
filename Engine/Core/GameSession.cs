using Striped.Engine.Rendering.Core;
using Striped.Engine.Util;

namespace Striped.Engine.Core;

public class GameSession
{
    private static GameSession? activeSession = null;
    public static GameSession? ActiveSession
    {
        get => activeSession;
        set
        {
            if (activeSession == null) activeSession = value;
            else Logger.Except(new Exception("Close the currently active session first!"));
        }
    }
    public Game Game { get; private set; }
    public EngineWindow Window { get; private set; }
    
    public List<InteractiveEnvironment?> LoadedEnvironments { get; } = new List<InteractiveEnvironment?>();

    public InteractiveEnvironment? CreateEnvironment()
    {
        InteractiveEnvironment? environment = new InteractiveEnvironment();
        LoadedEnvironments.Add(environment);
        return environment;
    }
    
    public void UnloadEnvironment(InteractiveEnvironment? environment)
    {
        environment.Destroy();
        LoadedEnvironments.Remove(environment);
    }
    
    public GameSession(Game game)
    {
        Game = game;
        ActiveSession = this;
    }

    public void CloseSession()
    {
        activeSession = null;
    }

    public void BindWindow(EngineWindow window)
    {
        Window = window;
    }

    public void UnloadAllEnvironments()
    {
        foreach (var interactiveEnvironment in LoadedEnvironments)
        {
            interactiveEnvironment.Destroy();
        }
    }
}