using System;
using System.IO;
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

    private static bool takesInput = true; // Determines whether or not we should take regular input

    private static bool consoleOpen; // Used to determine whether or not we should display the console
    private static bool debugOpen; // Used to determine whether or not we should display the debug UI

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

        // Create default console commands
        CreateConsoleCommands();

        // If we couldn't load our keybinds file...
        if ( !InputManager.LoadKeybinds() )
        {
            // Create default keybinds
            CreateKeybinds();
        }

        // Try to...
        try
        {
            // Initialize the renderer
            Renderer.Initialize( $"Toaster Engine (v.{ENGINE_VERSION}){( title != null ? $" - {title}" : "" )}" );
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
    private static void CreateConsoleCommands()
    {
        // Clear
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "clear",
            description = "Clears the console's logs. (Does NOT clear the log file!)",

            onCall = ConsoleUI.Clear,
            onArgsCall = ConsoleManager.InvalidCommand
        } );

        // Console
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "console",
            description = "Toggles the display of the console.",

            onCall = ToggleConsole,
            onArgsCall = ConsoleManager.InvalidCommand
        } );

        // Help
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "help",
            description = "Displays information about a command, or the list of available commands.",

            onCall = ConsoleManager.DisplayCommands,
            onArgsCall = ConsoleManager.DisplayCommand
        } );

        // Open debug (UI)
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "open_debug",
            description = "Opens the debug UI menu, displaying special game information.",

            onCall = ToggleDebug,
            onArgsCall = ConsoleManager.InvalidCommand
        } );

        // Play sound
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "playsound",
            description = "Plays a sound from a specified path (should be something like \"resources/audio/engine/error.mp3\".)",

            onCall = ConsoleManager.InvalidCommand,
            onArgsCall = AudioManager.PlaySound
        } );

        // Bind
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "bind",
            description = "Binds a key to an action defined by an alias.",

            onCall = ConsoleManager.InvalidCommand,
            onArgsCall = InputManager.EditKeybind
        } );

        // Show bindings
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "showbindings",
            description = "Displays all bindings.",

            onCall = InputManager.DisplayKeybinds,
            onArgsCall = ConsoleManager.InvalidCommand
        } );

        // Toggle command
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "togglecommand",
            description = "Disables or enables a specific console command.",

            onCall = ConsoleManager.InvalidCommand,
            onArgsCall = ConsoleManager.ToggleCommand
        } );

        // Quit
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "quit",
            description = "Shuts the engine down entirely.",

            onCall = EnvironmentShutdown,
            onArgsCall = ConsoleManager.InvalidCommand
        } );
    }

    /// <summary>
    /// Initializes engine-wide keybinds.
    /// </summary>
    private static void CreateKeybinds()
    {
        // Open the console when F12 is pressed
        InputManager.AddKeybind( new Keybind { alias = "console", key = Key.F12, commandAlias = "console" } );

        // Unbound (by default) keybind to shutdown the engine
        InputManager.AddKeybind( new Keybind { alias = "quit", key = Key.Unknown, commandAlias = "quit" } );
    }

    /// <summary>
    /// Sets the engine's regular ability to take inputs.
    /// </summary>
    public static void ToggleInput(bool status)
    {
        // Set the status of the takesInput variable
        takesInput = status;
    }

    /// <summary>
    /// Toggles the console. Mainly used with console commands.
    /// </summary>
    private static void ToggleConsole()
    {
        consoleOpen = !consoleOpen;
    }

    /// <summary>
    /// Toggles the debug UI. Mainly used with console commands.
    /// </summary>
    private static void ToggleDebug()
    {
        debugOpen = !debugOpen;
    }

    /// <summary>
    /// Does the engine currently take inputs?
    /// </summary>
    public static bool TakesInput()
    {
        return takesInput;
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
            AudioManager.Update();

            // Update the input manager
            InputManager.Update();

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
        // Manage the console
        if ( consoleOpen )
        {
            ConsoleUI.Display( ref consoleOpen );
        }

        // Manage the debug UI
        if ( debugOpen )
        {
            DebugUI.Display( ref debugOpen );
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
        // Save the keybinds
        InputManager.SaveKeybinds();

        // End file logging
        Log.CloseLogFile();

        // Clear everything from the renderer
        Renderer.Shutdown();
    }
}