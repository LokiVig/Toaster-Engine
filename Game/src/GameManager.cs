using System.Numerics;

using Veldrid;

using Toast.Engine;
using Toast.Engine.Entities;
using Toast.Engine.Entities.Tools;
using Toast.Engine.Entities.Brushes;
using Toast.Engine.Resources;
using Toast.Engine.Resources.Input;

using Toast.Game.Entities;
using Toast.Game.Entities.NPC;

namespace Toast.Game;

/// <summary>
/// Used to initialize the game.
/// </summary>
public class Program
{
    [STAThread]
    public static void Main()
    {
        // Makes a new instance of the game and calls its initialize function
        GameManager game = new GameManager();
        game.Initialize();
    }
}

/// <summary>
/// The game itself.<br/>
/// This class handles initializing and running everything game-wise, handling specificities that should<br/>
/// only pertain to the game.
/// </summary>
public class GameManager
{
    /// <summary>
    /// The state the game is currently in.
    /// </summary>
    public static GameState currentState = GameState.Active;

    private Player? mainPlayer;

    /// <summary>
    /// Initialize the game
    /// </summary>
    public void Initialize()
    {
        // Create our default keybinds
        CreateKeybinds();

        // Initialize the engine
        EngineManager.OnUpdate += Update; // After the engine's done updating, the game manager itself (us) should update
                                          // The engine's update function is a lot more focused on engine-wide prospects,
                                          // while the game manager should be focused more on the game-specific functionalities
        EngineManager.Initialize( "Game" ); // Call the engine's initialize function, with "Game" as its title
    }

    /// <summary>
    /// Initializes this game's default keybinds, and which console command they correspond to.
    /// </summary>
    private void CreateKeybinds()
    {
        // Add movement keybinds
        InputManager.AddKeybind( new Keybind { alias = "move_forward", key = Key.W, commandAlias = "+move_forward", down = true } ); // Forwards movement
        InputManager.AddKeybind( new Keybind { alias = "move_backward", key = Key.S, commandAlias = "+move_backward", down = true } ); // Backwards movement
        InputManager.AddKeybind( new Keybind { alias = "move_left", key = Key.A, commandAlias = "+move_left", down = true } ); // Leftwards movement
        InputManager.AddKeybind( new Keybind { alias = "move_right", key = Key.D, commandAlias = "+move_right", down = true } ); // Rightwards movement
    }

    /// <summary>
    /// Defines things to do every frame the game is run.
    /// </summary>
    private void Update()
    {
        // Things to do when there is a loaded scene
        if ( EngineManager.currentScene != null )
        {
        }

        if ( ( currentState & GameState.Active ) == 0 )
        {
            // We shouldn't do anything more after this!
            // Everything below this if-statement will be run only when the game is in an
            // active state
            return;
        }
    }
}

/// <summary>
/// This enum defines the different states the game can be in.
/// </summary>
[Flags]
public enum GameState
{
    Menu = 1 << 0, // Used when accessing any sort of menu
    Active = 1 << 1, // Rendering, updating, doing everything it should
    Paused = 1 << 2, // Paused for any reason, possibly maskable with menu?
}