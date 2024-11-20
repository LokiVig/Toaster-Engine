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

        EntitySpawner playerSpawner = new EntitySpawner(new Player());
        EntitySpawner npcSpawner = new EntitySpawner(new TestNPC());

        TriggerBrush trigger = new TriggerBrush();
        trigger.SetBBox( new BBox( new Vector3( -15.0f, -15.0f, -15.0f ), new Vector3( 15.0f, 15.0f, 15.0f ) ) );
        trigger.triggerType = TriggerType.Once;
        trigger.triggerBy = TriggerBy.Players;
        trigger.triggerOn = TriggerOn.Trigger;
        trigger.targetEvent = EntityEvent.TakeDamage;
        trigger.targetEntity = "entity 4";
        trigger.fValue = 100.0f;
        
        currentFile.AddEntity( playerSpawner );
        currentFile.AddEntity( npcSpawner );
        currentFile.AddEntity( trigger );

        WTF.SaveFile( "maps/test.wtf", currentFile );

        // Load everything necessary from the current file
        currentScene = Scene.LoadFromWTFFile(currentFile);

        Player player = playerSpawner.SpawnEntity<Player>();
        TestNPC npc = npcSpawner.SpawnEntity<TestNPC>();
        
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

        // Check every entity in the current scene
        foreach (Entity ent in currentScene?.GetEntities())
        {
            // If this entity doesn't have an ID...
            if (string.IsNullOrEmpty(ent.GetID()))
            {
                // Create an ID for the entity!
                ent.CreateID();
            }
            
            // DEBUG: Log every entity, their position, velocity, BBox, and ID
            // Console.WriteLine($"Entity {ent} (\"{ent.GetID()}\")\n" +
            //                     $"\tPosition: {ent.GetPosition()}\n" +
            //                     $"\tVelocity: {ent.GetVelocity()}\n" +
            //                     $"\tBBox: {ent.GetBBox()}\n");
        }
        
        // Call the OnUpdate event, so everything else that should update also updates with us
        OnUpdate?.Invoke();
    }
}