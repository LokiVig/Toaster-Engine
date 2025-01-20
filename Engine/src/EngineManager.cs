using System;
using System.Runtime.InteropServices;

using Veldrid;

using Toast.Engine.Resources;
using Toast.Engine.Rendering;
using System.Diagnostics;

namespace Toast.Engine;

public class EngineManager
{
    //---------------------------------------//
    //			    Constants				 //
    //---------------------------------------//

    private const string ENGINE_VERSION = "0.0.1";

    //---------------------------------------//
    //               Publics                 //
    //---------------------------------------//

    public static event Action OnUpdate; // Whenever we should update things, this event gets called

    public static WTF currentFile; // The currently loaded WTF file / map
    public static Scene currentScene; // The currently running scene, initialized from the current file

    public static OSPlatform activeOS; // The operating system the engine's actively running on

    public static float deltaTime; // Helps stopping you from using FPS-dependant calculations

    //---------------------------------------//
    //				 Privates                //
    //---------------------------------------//

    private static float lastFrameTime; // Time of the last frame in seconds
    private static bool debugUIOpen; // Used to determine whether or not we should display the debug UI

    //---------------------------------------//
    //				Functions				 //
    //---------------------------------------//

    public static void Initialize( string title = null )
    {
        // Find out which OS we're running on
        // Used for things like the audio manager, etc.

        // Initialize file logging
        Log.OpenLogFile();

        try
        {
            // Initialize the renderer
            Renderer.Initialize( $"Toaster Engine - (v.{ENGINE_VERSION}){( title != null ? $" - {title}" : "" )}" );
            Log.Success( "Successfully initialized renderer." );
        }
        catch ( Exception exc )
        {
            Log.Error( $"Exception caught while initializing renderer!\n{exc.Message}\n", exc );
        }

        // Connect our InputManager's events to the Renderer's events for simplicity's sakes
        Renderer.window.KeyDown += InputManager.OnKeyDown;
        Renderer.window.KeyUp += InputManager.OnKeyUp;
    }

    public static void Update()
    {
        Stopwatch watch = Stopwatch.StartNew();

        while ( !Renderer.ShuttingDown() )
        {
            // Calculate the time elapsed since the last frame
            float currentTime = (float)watch.Elapsed.TotalSeconds;
            deltaTime = currentTime - lastFrameTime;
            lastFrameTime = currentTime;

            // Ensure deltaTime is non-negative
            deltaTime = Math.Max( 0, deltaTime );

            // Update the static audio manager
            AudioManager.UpdateAllPlayingFiles();

            // Open the debug UI if the F12 key is pressed
            if ( InputManager.IsKeyDown( Key.F12 ) )
            {
                debugUIOpen = true;
            }

            // Handle ImGui things
            HandleUI();

            // Call the OnUpdate event
            // This makes it so everything subscribed to the event will call their own,
            // subsidiary update method
            OnUpdate?.Invoke();

            // Call the renderer's function that'll render stuff
            Renderer.RenderFrame( currentScene );
        }
    }

    private static void HandleUI()
    {
        if ( debugUIOpen )
        {
            DebugUI.Open( ref debugUIOpen );
        }
    }

    public static void Shutdown()
    {
        // End file logging
        Log.CloseLogFile();

        // Clear everything from the renderer
        Renderer.Shutdown();
    }
}