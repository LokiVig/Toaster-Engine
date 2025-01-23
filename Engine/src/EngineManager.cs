using System;
using System.Diagnostics;
using System.Collections.Generic;

using Veldrid;

using Toast.Engine.Resources;
using Toast.Engine.Rendering;
using System.IO;

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

    /// <summary>
    /// Initialize a new Toaster Engine instance with an optional <paramref name="title"/> argument.
    /// </summary>
    /// <param name="title">The title this instance of Toaster Engine should run.</param>
    public static void Initialize( string title = null )
    {
        // Initialize file logging
        Log.OpenLogFile();

        // Initialize default console commands
        InitializeConsoleCommands();

        // Try to...
        try
        {
            // Initialize the renderer
            Renderer.Initialize( $"Toaster Engine - (v.{ENGINE_VERSION}){( title != null ? $" - {title}" : "" )}" );
            Log.Success( "Successfully initialized renderer." );
        }
        catch ( Exception exc ) // Handle any exceptions we encounter!
        {
            Log.Error( $"Exception caught while initializing renderer!", exc );
        }

        // Connect our InputManager's events to the Renderer's events for simplicity's sakes
        Renderer.window.KeyDown += InputManager.OnKeyDown;
        Renderer.window.KeyUp += InputManager.OnKeyUp;
    }

    /// <summary>
    /// Initializes the most basic of console commands.
    /// </summary>
    private static void InitializeConsoleCommands()
    {
        // Clear
        ConsoleManager.AddCommand( new ConsoleCommand()
        {
            alias = "clear",
            description = "Clears the console's logs. (Does NOT clear the log file!)",
            onCall = ConsoleUI.Clear,
            onArgsCall = ( List<object> args ) => { ConsoleUI.Clear(); }
        } );

        // Help
        ConsoleManager.AddCommand( new ConsoleCommand()
        {
            alias = "help",
            description = "Displays information about a command, or the list of available commands.",
            onCall = ConsoleManager.DisplayCommands,
            onArgsCall = ( List<object> args ) => { ConsoleManager.DisplayCommand( (string)args[1] ); }
        } );

        // Play sound
        ConsoleManager.AddCommand( new ConsoleCommand()
        {
            alias = "playsound",
            description = "Plays a sound from a specified path (should be something like \"resources/audio/engine/error.mp3\".)",
            onCall = () => { Log.Error( "Can't play a sound without a specified path to it!" ); },
            onArgsCall = ( List<object> args ) => 
            { 
                // Make sure the specified file exists
                if ( !File.Exists( (string)args[1] ) )
                {
                    Log.Error( $"Couldn't find file at path \"{args[1]}\"!" );
                    return;
                }
                
                // Play the sound!
                AudioManager.PlaySound( (string)args[1] ); 
                Log.Info( $"Playing sound \"{args[1]}\"..." );
            }
        } );

        // ToggleCommand
        ConsoleManager.AddCommand( new ConsoleCommand()
        {
            alias = "togglecommand",
            description = "Disables or enables a specific console command.",
            onCall = () => { Log.Error( "Cannot toggle a console command without specifying a command!" ); },
            onArgsCall = (List<object> args) =>
            {
                // Find the command
                ConsoleCommand command = ConsoleManager.FindCommand( (string)args[1] );

                // If we did actually find a command...
                if ( command != null )
                {
                    // If its alias is our own...
                    if ( command.alias == "togglecommand" )
                    {
                        // We can't toggle its status!
                        Log.Warning( "Can't toggle the toggle command, that'd be problematic!" );
                        return;
                    }

                    // Toggle its state!
                    command.enabled = !command.enabled;
                }
            }
        } );

        // Quit
        ConsoleManager.AddCommand( new ConsoleCommand()
        {
            alias = "quit",
            description = "Shuts the engine down entirely.",
            onCall = EnvironmentShutdown,
            onArgsCall = (List<object> args) => { EnvironmentShutdown(); }
        } );
    }

    /// <summary>
    /// Things to do for every frame.
    /// </summary>
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
            DebugUI.Display( ref debugUIOpen );
            ConsoleUI.Display( ref debugUIOpen );
        }
    }

    /// <summary>
    /// Used primarily for the "quit" console command, instantly closes the application with the code 0.
    /// </summary>
    public static void EnvironmentShutdown()
    {
        Shutdown(); // Call the regular shutdown function
        Environment.Exit( 0 ); // Exit the environment
    }

    /// <summary>
    /// Shut down the engine after a hard day of calling update functions and etc.
    /// </summary>
    public static void Shutdown()
    {
        // End file logging
        Log.CloseLogFile();

        // Clear everything from the renderer
        Renderer.Shutdown();
    }
}