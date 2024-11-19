#pragma warning disable CA1416

using System;
using System.Diagnostics;

using DoomNET.Entities;
using DoomNET.Resources;

namespace DoomNET;

public class DoomNET
{
    public static event Action OnUpdate;
    public static WTF currentFile;
    public static Scene currentScene;
    public static bool active;
    public static float deltaTime;
    public static int windowWidth = 1280;
    public static int windowHeight = 720;

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
        
        // Call the update function, so the game will run once every frame!
        Update();
    }

    private void Update()
    {
        // We're now active! We should therefore activate the update loop
        active = true;
        
        // Stopwatch to calculate delta time with
        Stopwatch watch = Stopwatch.StartNew();
        
        while (active)
        {
            // Calculate deltaTime
            deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
            watch.Restart();

            // Call the OnUpdate event, so everything else that should update also updates with us
            OnUpdate?.Invoke();
        }
    }
}