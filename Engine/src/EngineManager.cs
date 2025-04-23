using System;
using System.Diagnostics;

using Veldrid;

using Toast.Engine.Rendering;
using Toast.Engine.Resources;
using Toast.Engine.Attributes;
using Toast.Engine.Resources.API;
using Toast.Engine.Resources.Input;
using Toast.Engine.Resources.Audio;
using Toast.Engine.Resources.Console;

namespace Toast.Engine;

/// <summary>
/// The engine instance itself.
/// </summary>
public static class EngineManager
{
    //---------------------------------------//
    //			    Constants				 //
    //---------------------------------------//

    /// <summary>
    /// Defines which version of the engine we're on as a string.<br/>
    /// Primarily to be used on the window's title.
    /// </summary>
    public const string VERSION = "0.0.2";

    //---------------------------------------//
    //               Publics                 //
    //---------------------------------------//

    /// <summary>
    /// Whenever we update things, this event gets called.
    /// </summary>
    public static event Action OnUpdate;

    /// <summary>
    /// Whenever UI elements get updated, this event gets called.
    /// </summary>
    public static event Action OnUIUpdate;

    /// <summary>
    /// The constant title the engine should feature.
    /// </summary>
    public static readonly string EngineTitle = $"Toaster Engine [V.{VERSION}]";

#if DEBUG // If we're debugging, we should, by default, be cheating
    /// <summary>
    /// Determines whether or not cheats are enabled.
    /// </summary>
    public static bool cheatsEnabled = true;
#else // If we're in release mode, we should, by default, not be cheating
    /// <summary>
    /// Determines whether or not cheats are enabled.
    /// </summary>    
    public static bool cheatsEnabled = false;
#endif // DEBUG

    /// <summary>
    /// The currently loaded WTF file (map).
    /// </summary>
    public static WTF currentFile;

    /// <summary>
    /// The currently running scene, should be initialized from the current file.
    /// </summary>
    public static Scene currentScene;

    /// <summary>
    /// The engine's settings.
    /// </summary>
    public static Settings settings;

    /// <summary>
    /// Helps stopping you from using FPS-dependant calculations.<br/>
    /// Should be used for things that pass through time, e.g. physics.
    /// </summary>
    public static float deltaTime;

    //---------------------------------------//
    //				 Privates                //
    //---------------------------------------//

    private static float lastFrameTime; // Time of the last frame in seconds

    private static bool takesInput = true; // Determines whether or not we should take regular input

    private static bool consoleOpen; // Used to determine whether or not we should display the console
    private static bool debugOpen; // Used to determine whether or not we should display the debug UI

    /// <summary>
    /// Initialize a new Toaster Engine instance with an optional <paramref name="title"/> argument.
    /// </summary>
    /// <param name="title">The title this Toaster Engine instance should have.</param>
    public static void Initialize( string title = null )
    {
        // Try to...
        try
        {
            // Initialize file logging
            Log.OpenLogFile();
         
            // Load the settings
            if ( !Settings.Load( out settings ) )
            {
                settings = new Settings();
            }

            // Initialize our input manager
            InputManager.Initialize();
            
            // Register the default console commands
            ConsoleManager.RegisterCommands();

            DiscordManager.Initialize();

            // Create default keybinds
            CreateKeybinds();

            // Initialize the renderer
            Renderer.Initialize( $"{EngineTitle}{( title != null ? $" - {title}" : "" )}" );
        }
        catch ( Exception exc ) // Handle any exceptions we encounter!
        {
            Log.Error( $"Exception caught while initializing!", exc );
        }

        // Connect our InputManager's events to the Renderer's events for simplicity's sakes
        Renderer.window.KeyDown += InputManager.OnKeyDown;
        Renderer.window.KeyUp += InputManager.OnKeyUp;

        // Call our update function
        Update();
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

            // Call the OnUpdate event
            // This makes it so everything subscribed to the event will call their own,
            // subsidiary update method
            OnUpdate?.Invoke();

            // Call the RenderFrame method from the renderer
            Renderer.RenderFrame( currentScene ?? null );

            // Update UI elements
            UIUpdate();
        }

        // We should stop everything and call our shutdown!
        Shutdown();
    }

    /// <summary>
    /// Updates UI elements, such as ImGui.
    /// </summary>
    private static void UIUpdate()
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

        // Call the UI update event
        OnUIUpdate?.Invoke();
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
    [ConsoleCommand( "debug", "Toggles the debug UI menu, displaying specific engine and game information.", Conditions = CommandConditions.Cheats )]
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
    /// Shut down the engine after a hard day of calling update functions and etc.
    /// </summary>
    public static void Shutdown()
    {
        // Save the keybinds
        InputManager.SaveKeybinds();

        // Save our commands
        ConsoleManager.SaveCommands();

        // Save our settings
        settings.Save();

        // Shutdown the Discord RPC
        DiscordManager.Shutdown();

        // End file logging
        Log.CloseLogFile(); 

        // Clear everything from the renderer
        Renderer.Shutdown();
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
}