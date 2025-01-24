using System.Numerics;

using Toast.Engine;
using Toast.Engine.Entities;
using Toast.Engine.Entities.Tools;
using Toast.Engine.Entities.Brushes;
using Toast.Engine.Resources;

using Toast.Game.Entities;
using Toast.Game.Entities.NPC;

using Veldrid;

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
/// This class handles initializing and running everything game-wise, meaning it holds the Update function,
/// important variables such as deltaTime, the current file, and the current scene.
/// </summary>
public class GameManager
{
    public static GameState currentState = GameState.Active; // The state the game currently is in

    private PlayerEntity mainPlayer;

    /// <summary>
    /// Initialize the game
    /// </summary>
    public void Initialize()
    {
        // Initialize the engine
        EngineManager.Initialize( "Game" ); // Call the engine's initialize function
        EngineManager.OnUpdate += Update; // After the engine's done updating, this class should update
                                          // The engine's update function is a lot more focused on, well, engine-wide
                                          // prospects, while this class's should be focused more on the game-specific
                                          // functionalities

        // If we couldn't load our keybinds...
        if ( !InputManager.LoadKeybinds() )
        {
            // Initialize our default keybinds
            CreateKeybinds();
        }

        // If we couldn't load our console commands...
        if ( !ConsoleManager.LoadCommands() )
        {
            // Initialize our console commands
            CreateCommands();
        }

        // Initialize everything necessary before the game is actually run
        // DEBUG: Setting up a basic scene to test out certain aspects of what's done
        EngineManager.currentFile = new WTF( "test.wtf" );

        EntitySpawner<TestNPC> npcSpawner = new( new Vector3( 0, 5.0f, 0 ) );
        EntitySpawner<Player> playerSpawner = new( Vector3.Zero );

        SoundEntity audioPlayer = new SoundEntity( new Vector3( 0, 0, 15.0f ) );
        audioPlayer.audioPath = "resources/audio/music/debugmusic.mp3";
        audioPlayer.audioVolume = 0.25f;
        audioPlayer.audioRepeats = true;

        TriggerBrush trigger = new TriggerBrush();
        trigger.SetBBox( new BBox( new Vector3( -15 ), new Vector3( 15 ) ) );
        trigger.triggerBy = TriggerBy.Player;
        trigger.triggerOn = TriggerOn.Trigger;
        trigger.triggerType = TriggerType.Once;
        trigger.targetEvent = EntityEvent.PlaySound;
        trigger.targetEntity = "entity 3"; // DEBUG!

        EngineManager.currentFile.AddEntity( npcSpawner );
        EngineManager.currentFile.AddEntity( playerSpawner );
        EngineManager.currentFile.AddEntity( trigger );
        EngineManager.currentFile.AddEntity( audioPlayer );

        EngineManager.currentFile.Save();

        EngineManager.currentScene = Scene.LoadFromFile( EngineManager.currentFile );

        npcSpawner.Spawn();
        playerSpawner.Spawn();
        trigger.Spawn();
        audioPlayer.Spawn();

        mainPlayer = playerSpawner.SpawnEntity();
        TestNPC npc = npcSpawner.SpawnEntity();

        Ray.Trace( mainPlayer, npc, out object hitObject, RayIgnore.Brushes | RayIgnore.BrushEntities );
        ( hitObject as Entity )?.TakeDamage( 25, mainPlayer );

        // Start updating the engine
        EngineManager.Update();

        // After of which, we should call the engine shutdown function
        EngineManager.OnUpdate -= Update; // Unsubscribe ourselves from the engine update function
        EngineManager.Shutdown(); // Actually call the engine shutdown function
    }

    /// <summary>
    /// Initializes this game's default keybinds, and which console command they correspond to.
    /// </summary>
    private void CreateKeybinds()
    {
        // Add movement keybinds
        InputManager.AddKeybind( new Keybind { alias = "move_forward",  key = Key.W, commandAlias = "+move_forward"  } ); // Forwards movement
        InputManager.AddKeybind( new Keybind { alias = "move_backward", key = Key.S, commandAlias = "+move_backward" } ); // Backwards movement
        InputManager.AddKeybind( new Keybind { alias = "move_left",     key = Key.A, commandAlias = "+move_left"     } ); // Leftwards movement
        InputManager.AddKeybind( new Keybind { alias = "move_right",    key = Key.D, commandAlias = "+move_right"    } ); // Rightwards movement
    }

    /// <summary>
    /// Initializes this game's console commands.
    /// </summary>
    private void CreateCommands()
    {
        // Move Forward
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "+move_forward",
            description = "Moves the main player entity forward.",

            onCall = () => { mainPlayer.AddForce( new Vector3( 0, 255, 0 ) ); },
            onArgsCall = (List<object> args) => { mainPlayer.AddForce( new Vector3( 0, 255, 0 ) ); }
        } );

        // Move Backward
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "+move_backward",
            description = "Moves the main player entity backward.",

            onCall = () => { mainPlayer.AddForce( new Vector3( 0, -255, 0 ) ); },
            onArgsCall = (List<object> args) => { mainPlayer.AddForce( new Vector3( 0, -255, 0 ) ); }
        } );

        // Move Left
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "+move_left",
            description = "Moves the main player entity left.",

            onCall = () => { mainPlayer.AddForce( new Vector3( -255, 0, 0 ) ); },
            onArgsCall = ( List<object> args ) => { mainPlayer.AddForce( new Vector3( -255, 0, 0 ) ); }
        } );

        // Move Right
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "+move_right",
            description = "Moves the main player entity right.",

            onCall = () => { mainPlayer.AddForce( new Vector3( 255, 0, 0 ) ); },
            onArgsCall = ( List<object> args ) => { mainPlayer.AddForce( new Vector3( 255, 0, 0 ) ); }
        } );
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