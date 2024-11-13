using System;
using System.Diagnostics;

using DoomNET.WTF;
using DoomNET.Entities;
using DoomNET.Resources;
using DoomNET.Rendering;

namespace DoomNET;

public class DoomNET
{
    public static event Action OnUpdate;
    public static WTFFile file;

    public static bool active;

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
        //file = WTFLoader.LoadFile("maps/test.wtf");

        file = new();

        TestNPC npc = new TestNPC( new Vector3( 50, 55, 50 ), new BBox( new Vector3( 16.0f, 16.0f, 64.0f ), new Vector3( -16.0f, -16.0f, 0.0f ) ) );
        npc.SetVelocity( new Vector3( 0f, 0f, 0f ) );

        TriggerBrush trigger = new TriggerBrush();
        trigger.SetBBox( new BBox( new Vector3( -15.0f, -15.0f, -15.0f ), new Vector3( 15.0f, 15.0f, 15.0f ) ) );
        trigger.triggerType = TriggerType.Once;
        trigger.triggerBy = TriggerBy.Players;
        trigger.triggerOn = TriggerOn.Trigger;
        trigger.targetEvent = EntityEvent.TakeDamage;
        trigger.targetEntity = npc;
        trigger.fValue = 100.0f;

        Player player = new Player( new Vector3( 50.0f, 50.0f, 50.0f ), new BBox( new Vector3( 32.0f, 32.0f, 64.0f ), new Vector3( -32.0f, -32.0f, 0.0f ) ) );
        player.SetVelocity( new Vector3( 1.5f, 0.25f, 0f ) );

        file.AddEntity( player );
        file.AddEntity( npc );
        file.AddEntity( trigger );

        WTFFile.SaveFile( "maps/test.wtf", file );

        Ray.Trace( player, npc, out object hitObject, RayIgnore.None, trigger );

        // Initialize an SDL window
        renderer = new Renderer();

        // Now we can run the game every frame
        Update();

        // Clean up everything SDL-wise
        renderer.CleanUp();
    }

    /// <summary>
    /// Things to do every frame, game-wise<br/>
    /// This includes calling every other update function that updates every frame
    /// </summary>
    private void Update()
    {
        // Start the loop
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

            // Poll SDL events and render everything necessary on the window
            renderer.PollEvents();
            renderer.Render();
        }
    }
}