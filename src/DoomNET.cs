using System;
using System.Threading;
using System.Diagnostics;

using DoomNET.WTF;
using DoomNET.Entities;
using DoomNET.Resources;

namespace DoomNET;

public class DoomNET
{
    private bool active;

    public static event Action OnUpdate;
    public static WTFFile file;

    public static float deltaTime;

    /// <summary>
    /// Initialize the game
    /// </summary>
    public void Initialize()
    {
        // DEBUG! Load a test map
        //file = WTFLoader.LoadFile("maps/test.wtf");

        file = new();

        Player player = new();
        player.SetVelocity(new Vector3(1.5f, 0.25f, 0f));

        TestNPC npc = new();
        npc.SetVelocity(new Vector3(-0.5f, -25.0f, 0f));

        file.AddEntity(player);
        file.AddEntity(npc);
        file.AddBrush(new Brush(new Vector3(-21.5f, -15.5f, -31.5f), new Vector3(25.5f, 34.5f, 0.5f)));

        WTFSaver.SaveFile("maps/test.wtf", file);

        if (Ray.Trace(new Vector3(), file.GetBrushes()[0].GetCenter(), out object hit, RayIgnore.Entities))
        {
            Console.WriteLine($"Wuhyippe!!! We hit {hit}!!!");
        }
        else
        {
            Console.WriteLine("FUCK");
        }

        // The game is now active
        active = true;

        // Initialize the update function
        Update();
    }

    /// <summary>
    /// Things to do every frame, game-wise<br></br>
    /// This includes calling every other update function that needs it
    /// </summary>
    public void Update()
    {
        Stopwatch watch = Stopwatch.StartNew();

        while (active)
        {
            // Calculate deltaTime
            deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
            watch.Restart();

            // Call the OnUpdate event, so everything else that should update also updates with us
            OnUpdate?.Invoke();

            // Lock the FPS to 60
            float elapsedTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
            float timeToWait = (1.0f / 60.0f) - elapsedTime;

            for (int i = 0; i < file.GetEntities().Count; i++)
            {
                Console.SetCursorPosition(0, i + 1);
                Console.Write($"Entity {file.GetEntities()[i].GetID()} Position: {file.GetEntities()[i].GetPosition().ToString()}");

                Console.SetCursorPosition(0, i + 3);
                Console.Write($"Entity {file.GetEntities()[i].GetID()} Velocity: {file.GetEntities()[i].GetVelocity().ToString()}");
            }

            if (timeToWait > 0)
            {
                Thread.Sleep((int)(timeToWait * 1000));
            }
        }
    }
}