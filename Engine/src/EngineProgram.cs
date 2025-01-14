using System;
using System.Text.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Toast.Engine.Resources;

namespace Toast.Engine;

public class EngineProgram
{
    //---------------------------------------//
    //			    Constants				 //
    //---------------------------------------//

    public static readonly JsonSerializerOptions serializerOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true
    };

    private const string ENGINE_VERSION = "0.0.1";

    //---------------------------------------//
    //               Publics                 //
    //---------------------------------------//

    public static event Action OnUpdate; // Whenever we should update things, this event gets called

    public static WTF currentFile; // The currently loaded WTF file / map
    public static Scene currentScene; // The currently running scene, initialized from the current file

    public static AudioManager globalAudioManager; // Global audio manager

    public static OSPlatform activeOS; // The operating system the engine's actively running on

    public static float deltaTime; // Helps stopping you from using FPS-dependant calculations

    public static IntPtr renderer; // The C++ initialized renderer as a whole

    //---------------------------------------//
    //				 Privates                //
    //---------------------------------------//

    private static Stopwatch watch = Stopwatch.StartNew(); // Used to calculate delta time

    //---------------------------------------//
    //				Functions				 //
    //---------------------------------------//

    public static void Initialize( string title = null )
    {
        // Find out which OS we're running on
        // Used for things like the audio manager, etc.


        // Initialize the global audio manager
        globalAudioManager = new AudioManager();

        // Initialize the renderer
        try
        {
            renderer = External.CreateRenderer( $"Toaster Engine (v.{ENGINE_VERSION}){( title != null ? $" - {title}" : "" )}" );
            Log.Info("Successfully initialized renderer.", true);
        }
        catch ( Exception exc )
        {
            Log.Error( $"Exception caught while initializing renderer!\n{exc.Message}\n", exc );
        }
    }

    public static void Update()
    {
        while ( renderer != IntPtr.Zero )
        {
            // Calculate deltaTime
            deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
            watch.Restart();

            // Call the rendering function
            External.RenderFrame( renderer );

            // The renderer is shutting down!
            // Null the renderer value so we don't continue running without it
            if ( External.RendererShuttingDown( renderer ) )
            {
                renderer = IntPtr.Zero;
            }

            // Update the global audio manager
            globalAudioManager.UpdateAllPlayingFiles();

            //External.RenderText(renderer, "FUCK", 1280/2, 720/2, 25.0f);

            // Call the OnUpdate event
            // This makes it so everything subscribed to the event will call their own,
            // subsidiary update method
            OnUpdate?.Invoke();
        }
    }

    public static void Shutdown()
    {
        External.ShutdownRenderer( renderer );
    }
}