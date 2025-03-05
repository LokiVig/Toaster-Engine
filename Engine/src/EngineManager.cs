using System;
using System.Diagnostics;

using Veldrid;

using Toast.Engine.Rendering;
using Toast.Engine.Resources;
using Toast.Engine.Attributes;
using Toast.Engine.Resources.Input;
using Toast.Engine.Resources.Audio;
using Toast.Engine.Resources.Console;

namespace Toast.Engine;

public static class EngineManager
{
    //---------------------------------------//
    //			    Constants				 //
    //---------------------------------------//

    public const string VERSION = "0.0.2";

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
    public static void Initialize( string title = null, WindowState initialWindowState = WindowState.Normal )
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
            ConsoleManager.RegisterCommands();
        }

        // If we couldn't load our keybinds file...
        if ( !InputManager.LoadKeybinds() )
        {
            // Create default keybinds
            CreateKeybinds();
        }
#else
        // Register the default console commands
        ConsoleManager.RegisterCommands();

        // Create default keybinds
        CreateKeybinds();
#endif // !DEBUG

        // Try to...
        try
        {
            // Initialize the renderer
            Renderer.Initialize( $"Toaster Engine (v.{VERSION}){( title != null ? $" - {title}" : "" )}", initialWindowState );
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
    [ConsoleCommand( "console", "Toggles the display of the console." )]
    private static void ToggleConsole()
    {
        consoleOpen = !consoleOpen;
    }

    /// <summary>
    /// Toggles the debug UI. Mainly used with console commands.
    /// </summary>
    [ConsoleCommand( "debug", "Toggles the debug UI menu, displaying specific engine and game information.", CommandConditions.Cheats )]
    private static void ToggleDebug()
    {
        debugOpen = !debugOpen;
    }

    /// <summary>
    /// Toggles whether or not cheats are enabled. Mainly used with console commands.
    /// </summary>
    [ConsoleCommand( "cheats", "Toggles whether or not cheats are enabled." )]
    private static void ToggleCheats()
    {
        cheatsEnabled = !cheatsEnabled;
        Log.Info( $"{( cheatsEnabled ? "Enabled" : "Disabled" )} cheats!", true );
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
    [ConsoleCommand( "quit", "Shuts the engine down entirely." )]
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