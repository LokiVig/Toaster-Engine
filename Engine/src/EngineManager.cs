using System;
using System.Diagnostics;
using System.Collections.Generic;

using Veldrid;

using Toast.Engine.Resources;
using Toast.Engine.Rendering;

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
        // Initialize file logging
        Log.OpenLogFile();

        // Initialize default console commands
        ConsoleManager.AddCommand( new ConsoleCommand() { alias = "clear", onCall = ConsoleUI.Clear } );
        ConsoleManager.AddCommand( new ConsoleCommand() { alias = "shutdown", onCall = Shutdown } ); // TODO: This causes an exception! Fix!
        ConsoleManager.AddCommand( new ConsoleCommand() { alias = "playsound", onArgsCall = ( List<object> args ) => { AudioManager.PlaySound( (string)args[0] ); } } );

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

            // Update the audio manager
            AudioManager.UpdatePlayingFiles();

            // Open the debug UI if the F12 key is pressed
            if ( InputManager.IsKeyDown( Key.F12 ) )
            {
                debugUIOpen = true;
            }

            // Manage UI elements (mainly ImGui related)
            ManageUI();

            // Call the OnUpdate event
            // This makes it so everything subscribed to the event will call their own,
            // subsidiary update method
            OnUpdate?.Invoke();

            // Call the renderer's function that'll render stuff
            Renderer.RenderFrame( currentScene );
        }
    }

    private static void ManageUI()
    {
        if ( debugUIOpen )
        {
            DebugUI.Open( ref debugUIOpen );
            ConsoleUI.Open( ref debugUIOpen );
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