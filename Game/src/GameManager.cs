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
        // Initialize the engine
        EngineManager.Initialize( "Game" ); // Call the engine's initialize function, with "Game" as its title
        EngineManager.OnUpdate += Update; // After the engine's done updating, the game manager itself (us) should update
                                          // The engine's update function is a lot more focused on engine-wide prospects,
                                          // while the game manager should be focused more on the game-specific functionalities

        // Create our default keybinds
        CreateKeybinds();

        // Initialize everything necessary before the game is actually run
        // DEBUG: Setting up a basic scene to test out certain aspects of what's done
        EngineManager.currentFile = new WTF( "test.wtf" );

        // References to the player and NPC entity
        TestNPC npc = new TestNPC();
        Player player = new Player();

        // Entity spawners of the NPC and player which should spawn an instance of each
        EntitySpawner npcSpawner = new( npc, new Vector3( 0, 5.0f, 0 ) );
        EntitySpawner playerSpawner = new( player, Vector3.Zero );

        // Sound entity which should play some background music for us
        SoundEntity soundEntity = new SoundEntity( new Vector3( 0, 0, 15.0f ) );
        soundEntity.audioPath = "resources/audio/music/debugmusic.wav";
        soundEntity.audioVolume = 0.25f;
        soundEntity.audioRepeats = true;

        // A trigger brush entity, which should start the sound entity's playback
        TriggerBrush trigger = new TriggerBrush();
        trigger.SetBoundingBox( new BoundingBox( new Vector3( -15 ), new Vector3( 15 ) ) );
        trigger.triggerBy = TriggerBy.Player;
        trigger.triggerOn = TriggerOn.Trigger;
        trigger.triggerType = TriggerType.Once;
        trigger.targetEvent = EntityEvent.PlaySound;
        trigger.targetEntity = "entity 3"; // DEBUG!

        // Debug brush!
        Brush debugBrush = new Brush( new Vector3( -5.0f ), new Vector3( 5.0f ) );

        // Add all the entities to the active file
        EngineManager.currentFile.AddEntity( npcSpawner );
        EngineManager.currentFile.AddEntity( playerSpawner );
        EngineManager.currentFile.AddEntity( trigger );
        EngineManager.currentFile.AddEntity( soundEntity );

        // Add the debugging brush to the file aswell
        EngineManager.currentFile.AddBrush( debugBrush );

        // Save the file to its path
        EngineManager.currentFile.Save();

        // Load our scene from the newly made file
        EngineManager.currentScene = Scene.LoadFromFile( EngineManager.currentFile );

        // Spawn all of the entities
        npcSpawner.Spawn();
        playerSpawner.Spawn();
        trigger.Spawn();
        soundEntity.Spawn();

        // Tell the NPC and player spawner to spawn their respective entities
        player = mainPlayer = (Player)playerSpawner.SpawnEntity();
        TestNPC newNpc = (TestNPC)npcSpawner.SpawnEntity();

        // Trace from the main player towards the NPC that was just spawned
        Ray.Trace( mainPlayer, newNpc, out object hitObject, RayIgnore.Brushes | RayIgnore.BrushEntities );
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