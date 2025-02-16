using System;
using System.Diagnostics;

using Veldrid;

using Toast.Engine.Rendering;
using Toast.Engine.Resources;
using Toast.Engine.Resources.Input;
using Toast.Engine.Resources.Audio;
using Toast.Engine.Resources.Console;

namespace Toast.Engine;

public static class EngineManager
{
    //---------------------------------------//
    //			    Constants				 //
    //---------------------------------------//

    private const string ENGINE_VERSION = "0.0.2";

    //---------------------------------------//
    //               Publics                 //
    //---------------------------------------//

    public static event Action OnUpdate; // Whenever we should update things, this event gets called

#if DEBUG // If we're debugging, we should, by default, be cheating
    public static bool cheatsEnabled = true; // Determines whether or not cheats are enabled
#else // If we're in release mode, we should, by default, not be cheating
    public static bool cheatsEnabled = false; // Determines whether or not cheats are enabled
#endif // DEBUG

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

        // Initialize our input manager
        InputManager.Initialize();

#if !DEBUG
        // If we can't load our commands...
        if ( !ConsoleManager.LoadCommands() )
        {
            // Create default console commands
            CreateConsoleCommands();
        }

        // If we couldn't load our keybinds file...
        if ( !InputManager.LoadKeybinds() )
        {
            // Create default keybinds
            CreateKeybinds();
        }
#else
        // Create default console commands
        CreateConsoleCommands();

        // Create default keybinds
        CreateKeybinds();
#endif // !DEBUG

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

            onCall = ConsoleUI.Clear
        } );

        // Console
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "console",
            description = "Toggles the display of the console.",

            onCall = ToggleConsole
        } );

        // Help
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "help",
            description = "Displays information about a command, or the list of available commands.",

            onCall = ConsoleManager.DisplayCommands,
            onArgsCall = ConsoleManager.DisplayCommand
        } );

        // Cheats
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "cheats",
            description = "Toggles whether or not cheats are enabled.",

            onCall = ToggleCheats
        } );

        // Open debug (UI)
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "debug",
            description = "Toggles the debug UI menu, displaying specific engine and game information.",

            onCall = ToggleDebug
        } );

        // Play sound
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "playsound",
            description = "Plays a sound from a specified path (should be something like \"resources/audio/engine/error.mp3\".)",

            onArgsCall = AudioManager.PlaySound
        } );

        // Display sounds
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "displaysounds",
            description = "Displays all currently playing audio files.",

            onCall = AudioManager.DisplayPlayingFiles
        } );

        // Stop sound(s)
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "stopsound",
            description = "Stops all actively playing sounds.",

            onCall = AudioManager.StopAllSounds
        } );

        // Bind
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "bind",
            description = "Binds a key to an action defined by an alias.",

            onArgsCall = InputManager.EditKeybind
        } );

        // Display binds
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "displaybinds",
            description = "Displays all bindings.",

            onCall = InputManager.DisplayKeybinds
        } );

        // Unbind
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "unbind",
            description = "Unbinds a key from an action. (Effectively just sets its key value to \"Unknown\".)",

            onArgsCall = InputManager.UnbindKeybind
        } );

        // Remove Bind
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "removebind",
            description = "Removes a bind outright from our list of binds.",

            onArgsCall = InputManager.RemoveKeybind
        } );

        // Toggle command
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "togglecommand",
            description = "Disables or enables a specific console command.",

            onArgsCall = ConsoleManager.ToggleCommand
        } );

        // Quit
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "quit",
            description = "Shuts the engine down entirely.",

            onCall = EngineShutdown
        } );
    }

    /// <summary>
    /// Initializes engine-wide keybinds.
    /// </summary>
    private static void CreateKeybinds()
    {
        // Open the console when '~' is pressed
        InputManager.AddKeybind( new Keybind { alias = "console", key = Key.Tilde, commandAlias = "console" } );

        // Unbound (by default) keybind to shutdown the engine
        InputManager.AddKeybind( new Keybind { alias = "quit", key = Key.Unknown, commandAlias = "quit" } );
    }

    /// <summary>
    /// Sets the engine's regular ability to take inputs.
    /// </summary>
    public static void ToggleInput( bool status )
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
    /// Toggles whether or not cheats are enabled. Mainly used with console commands.
    /// </summary>
    private static void ToggleCheats()
    {
        cheatsEnabled = !cheatsEnabled;
        Log.Info( $"{(cheatsEnabled ? "Enabled" : "Disabled")} cheats!", true );
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

        // While the renderer isn't shutting down...
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

            // Call the RenderFrame method from the renderer
            Renderer.RenderFrame( currentScene ?? null );
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
    public static void EngineShutdown()
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

        // Save our commands
        ConsoleManager.SaveCommands();

        // End file logging
        Log.CloseLogFile();

        // Clear everything from the renderer
        Renderer.Shutdown();
    }
}