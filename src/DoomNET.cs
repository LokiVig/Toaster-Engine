#pragma warning disable CA1416

using System;
using System.Diagnostics;

using DoomNET.Entities;
using DoomNET.Resources;
using DoomNET.Rendering;

namespace DoomNET;

public class DoomNET
{
    public static event Action OnUpdate;
    public static WTF currentFile;
    public static Scene currentScene;
    public static float deltaTime;
    public static int windowWidth = 1280;
    public static int windowHeight = 720;

    private Renderer renderer;
    
    /// <summary>
    /// Initialize the game
    /// </summary>
    public void Initialize()
    {
        // DEBUG! Load a test map
        //currentFile = WTFFile.LoadFile("maps/test.wtf");

        currentFile = new();

        TestNPC npc = new TestNPC( new Vector3( 0, 5, 0 ), new BBox( new Vector3( 16.0f, 16.0f, 64.0f ), new Vector3( -16.0f, -16.0f, 0.0f ) ) );
        npc.SetVelocity( new Vector3( 0f, 0f, 0f ) );

        TriggerBrush trigger = new TriggerBrush();
        trigger.SetBBox( new BBox( new Vector3( -15.0f, -15.0f, -15.0f ), new Vector3( 15.0f, 15.0f, 15.0f ) ) );
        trigger.triggerType = TriggerType.Once;
        trigger.triggerBy = TriggerBy.Players;
        trigger.triggerOn = TriggerOn.Trigger;
        trigger.targetEvent = EntityEvent.TakeDamage;
        trigger.targetEntity = "entity 1";
        trigger.fValue = 100.0f;

        Player player = new Player( new Vector3( 0, 0, 0 ), new BBox( new Vector3( 32.0f, 32.0f, 64.0f ), new Vector3( -32.0f, -32.0f, 0.0f ) ) );
        player.SetVelocity( new Vector3( 1.5f, 0.25f, 0f ) );

        currentFile.AddEntity( player );
        currentFile.AddEntity( npc );
        currentFile.AddEntity( trigger );

        WTF.SaveFile( "maps/test.wtf", currentFile );

        // Load everything necessary from the current file
        currentScene = Scene.LoadFromWTFFile( currentFile );

        //Player player = currentScene.FindEntity<Player>("player");
        //TestNPC npc = currentScene.FindEntity<TestNPC>("entity 1");
        //TriggerBrush trigger = currentScene.FindEntity<TriggerBrush>( "entity 2" );

        Ray.Trace( player, npc, out object hitObject, RayIgnore.None, [trigger] );
        
        renderer = new Renderer(windowWidth, windowHeight, "Doom.NET"); // Initialize a new window to render upon
        renderer.OnRender += Update; // Subscribe the update method, so everytime something is rendered, we update the game
        renderer.Run(); // Run the renderer
    }
    
    private Stopwatch watch = Stopwatch.StartNew();

    /// <summary>
    /// Update game functions.
    /// </summary>
    private void Update()
    {
        // Calculate deltaTime
        deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
        watch.Restart();

        // Call the OnUpdate event, so everything else that should update also updates with us
        OnUpdate?.Invoke();
    }
}