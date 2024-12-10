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
        
        currentFile.AddEntity( playerSpawner );
        currentFile.AddEntity( npcSpawner );
        currentFile.AddEntity( trigger );

        WTF.SaveFile( "maps/test.wtf", currentFile );

        // Load everything necessary from the current file
        currentScene = Scene.LoadFromWTFFile(currentFile);

        Player player = playerSpawner.SpawnEntity();
        TestNPC npc = npcSpawner.SpawnEntity();
        
        Ray.Trace( player, new Vector3(0, 500, 250), out object hitObject, RayIgnore.None, [playerSpawner, npcSpawner, trigger] );
        
        (hitObject as Entity)?.TakeDamage(5, player);
        
        renderer = new Renderer(windowWidth, windowHeight, "Doom.NET"); // Initialize a new window to render upon
        renderer.OnRender += Update; // Subscribe the update method, so everytime something is rendered, we update the game
        renderer.Run(); // Run the renderer
    }
    
    private Stopwatch watch = Stopwatch.StartNew();

    /// <summary>
    /// Defines things to do every frame the game is run.
    /// </summary>
    private void Update()
    {
        // Calculate deltaTime
        deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
        watch.Restart();

        // Things to do when there is a loaded scene
        if (currentScene != null)
        {
            // Check every entity in the current scene
            // foreach (Entity ent in currentScene.GetEntities())
            // {
                // // If this entity doesn't have an ID...
                // if (string.IsNullOrEmpty(ent.GetID()))
                // {
                //     // Create an ID for the entity!
                //     ent.CreateID();
                // }

                // DEBUG: Log every entity, their position, velocity, BBox, and ID
                // Console.WriteLine($"Entity {ent} (\"{ent.GetID()}\")\n" +
                //                     $"\tPosition: {ent.GetPosition()}\n" +
                //                     $"\tVelocity: {ent.GetVelocity()}\n" +
                //                     $"\tBBox: {ent.GetBBox()}\n");
            // }
        }

        // Call the OnUpdate event, so everything else that should update also updates with us
        OnUpdate?.Invoke();
    }
}
