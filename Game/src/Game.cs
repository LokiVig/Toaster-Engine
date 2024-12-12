using System;
using System.Diagnostics;
using DoomNET.Entities;
using DoomNET.Rendering;
using DoomNET.Resources;

namespace DoomNET;

/// <summary>
/// The game itself.<br/>
/// This class handles initializing and running everything game-wise, meaning it holds the Update function,
/// important variables such as deltaTime, the current file, and the current scene.
/// </summary>
public class Game
{
    public static event Action OnUpdate; // Whenever we should update things, this event gets called
    
    public static WTF currentFile; // The currently loaded WTF file / map
    public static Scene currentScene; // The currently running scene, initialized from the current file

    public static GameState currentState; // The state the game currently is in
    
    public static float deltaTime; // Helps stopping you from using FPS-dependant calculations
    
    public const int windowWidth = 1280; // The base width of the rendered window
    public const int windowHeight = 720; // The base height of the rendered window

    private IntPtr renderer;
    
    /// <summary>
    /// Initialize the game
    /// </summary>
    public void Initialize()
    {
        // DEBUG! Load a test map
        // currentFile = WTF.LoadFile("maps/test.wtf");

        currentFile = new WTF();
        
        EntitySpawner<Player> playerSpawner = new EntitySpawner<Player>();
        EntitySpawner<TestNPC> npcSpawner = new EntitySpawner<TestNPC>(new Vector3(0, 5, 0));
        
        TriggerBrush trigger = new TriggerBrush();
        trigger.SetBBox( new BBox( new Vector3( -15.0f ), new Vector3( 15.0f ) ) );
        trigger.triggerType = TriggerType.Multiple;
        trigger.triggerBy = TriggerBy.Player;
        trigger.triggerOn = TriggerOn.Trigger;
        trigger.targetEvent = EntityEvent.TakeDamage;
        trigger.targetEntity = "entity 4";
        trigger.fValue = 100.0f;
        trigger.Spawn();

        Brush floor = new Brush(new BBox(new Vector3(-15), new Vector3(15)));
        
        currentFile.AddEntity( playerSpawner );
        currentFile.AddEntity( npcSpawner );
        currentFile.AddEntity( trigger );
        currentFile.AddBrush(floor);
        
        WTF.SaveFile( "maps/test.wtf", currentFile );

        // Load everything necessary from the current file
        currentScene = Scene.LoadFromWTFFile(currentFile);

        // EntitySpawner<Player> playerSpawner = currentScene.FindEntity<EntitySpawner<Player>>("entity 0");
        // EntitySpawner<TestNPC> npcSpawner = currentScene.FindEntity<EntitySpawner<TestNPC>>("entity 1");
        // TriggerBrush trigger = currentScene.FindEntity<TriggerBrush>("entity 2");
        
        Player player = playerSpawner.SpawnEntity();
        TestNPC npc = npcSpawner.SpawnEntity();
        
        Ray.Trace( player, new Vector3(0, 500, 250), out object hitObject, RayIgnore.None, [playerSpawner, npcSpawner, trigger] );
        
        (hitObject as Entity)?.TakeDamage(5, player);
        
        // Initialize the renderer
        renderer = External.CreateRenderer();

        // If it isn't null
        if (renderer != IntPtr.Zero)
        {
            // Start it proper!
            External.StartRenderer(renderer, IntPtr.Zero, 1);
        }

        // Call the update function to start the game loop
        Update();
        
        // Destroy the renderer after we're finished
        External.DestroyRenderer( renderer );
    }
    
    private Stopwatch watch = Stopwatch.StartNew();

    /// <summary>
    /// Defines things to do every frame the game is run.
    /// </summary>
    private void Update()
    {
        while (renderer != IntPtr.Zero)
        {
            // Process Windows messages to handle input, window events, etc.
            if (External.PeekMessage(out External.MSG msg, IntPtr.Zero, 0, 0, 0x0001)) // WM_QUIT message
            {
                if (msg.message == 0x0012) // WM_QUIT
                {
                    break; // Exit the loop if the quit message is received
                }

                External.TranslateMessage(ref msg);
                External.DispatchMessage(ref msg);
            }
            
            // Calculate deltaTime
            deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
            watch.Restart();
            
            // Things to do when there is a loaded scene
            if (currentScene != null)
            {

            }

            // Call the OnUpdate event, so everything else that should update also updates with us
            OnUpdate?.Invoke();
            
            // Call the rendering function
            External.RenderFrame(renderer);
        }
    }
}

/// <summary>
/// This enum defines the different states the game can be in.
/// </summary>
public enum GameState
{
    Menu = 1, // Used when accessing any sort of menu
    Active = 2, // Rendering, updating, doing everything it should
    Paused = 4, // Paused for any reason, possibly maskable with menu?
}
