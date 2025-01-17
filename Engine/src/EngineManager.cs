using System;
using System.Text.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization.Metadata;

using Toast.Engine.Resources;

namespace Toast.Engine;

public class EngineManager
{
    //---------------------------------------//
    //			    Constants				 //
    //---------------------------------------//

    public static readonly JsonSerializerOptions serializerOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    private const string ENGINE_VERSION = "0.0.1";

    //---------------------------------------//
    //               Publics                 //
    //---------------------------------------//

    public static event Action OnUpdate; // Whenever we should update things, this event gets called

    public static WTF currentFile; // The currently loaded WTF file / map
    public static Scene currentScene; // The currently running scene, initialized from the current file

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

        // Initialize file logging
        Log.OpenLogFile();

        // Initialize the renderer
        try
        {
            renderer = External.Renderer_Initialize( $"Toaster Engine (v.{ENGINE_VERSION}){( title != null ? $" - {title}" : "" )}" );
            Log.Success( "Successfully initialized renderer." );
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
            External.Renderer_RenderFrame( renderer );

            // The renderer is shutting down!
            // Null the renderer value so we don't continue running without it
            if ( External.Renderer_ShuttingDown( renderer ) )
            {
                renderer = IntPtr.Zero;
            }

            External.Renderer_DrawText( renderer, "Test text!", 1280/2, 720/2, 24 );

            // Update the static audio manager
            AudioManager.UpdateAllPlayingFiles();

            // Call the OnUpdate event
            // This makes it so everything subscribed to the event will call their own,
            // subsidiary update method
            OnUpdate?.Invoke();
        }
    }

    public static void Shutdown()
    {
        // End file logging
        Log.CloseLogFile();

        // Shut down the renderer
        External.Renderer_Shutdown( renderer );
    }
}