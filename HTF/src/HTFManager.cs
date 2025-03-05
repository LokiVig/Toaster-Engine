using System.IO;
using System.Collections.Generic;

using Toast.Engine;
using Toast.Engine.Rendering;
using Toast.Engine.Resources;
using Toast.Engine.Attributes;
using Toast.Engine.Resources.Input;
using Toast.Engine.Resources.Console;

namespace Toast.WTFEdit;

public class Program
{
    public static void Main()
    {
        HTFManager htf = new HTFManager();
        htf.Initialize();
    }
}

public class HTFManager
{
    private WTF currentFile;
    private bool isDirty;

    /// <summary>
    /// Initialize the WTFEdit program
    /// </summary>
    public void Initialize()
    {
        // Initialize the engine
        EngineManager.Initialize();
        EngineManager.OnUpdate += Update;

        // Create HTF commands
        CreateCommands();

        // Create HTF keybinds
        CreateKeybinds();

        // !! DEBUG !! \\
        SaveMap( new WTF( "maps/test.wtf" ) );
        LoadMap( "maps/test.wtf" );

        // Start the engine's update function
        EngineManager.Update();

        // After having run the update for however many times, shut down the engine
        EngineManager.Shutdown();
    }

    private void CreateCommands()
    {
        // HTF Load Map
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "htf_load_map",
            description = "Loads a map from a specified path (e.g. \"maps/test.wtf\")",

            onArgsCall = LoadMap
        } );

        // HTF Load Map (File Dialog)
        ConsoleManager.AddCommand( new ConsoleCommand
        {
            alias = "htf_load_map_dialog",
            description = "Loads a map from a specified path given from an opened file dialog.",

            onCall = LoadMapThroughDialog
        } );
    }

    private void CreateKeybinds()
    {
        // File browser for opening map files
        InputManager.AddKeybind( new Keybind
        {
            alias = "open_file",
            key = Veldrid.Key.O,
            comboKey = Veldrid.Key.ControlLeft,
            commandAlias = "htf_load_map_dialog"
        } );

        InputManager.AddKeybind( new Keybind
        {
            alias = "save_file",
            key = Veldrid.Key.S,
            comboKey = Veldrid.Key.ControlLeft,
            commandAlias = "htf_save_map"
        } );
    }

    /// <summary>
    /// Things to do every frame
    /// </summary>
    private void Update()
    {
        Renderer.SetWindowTitle( $"HTF{( currentFile == null ? "" : $" - \"{currentFile.path}\"{( isDirty ? "*" : "" )}" )}" );
    }

    /// <summary>
    /// Loads a map through the console.
    /// </summary>
    /// <param name="args"></param>
    private void LoadMap( List<object> args )
    {
        // Get the current argument count
        int argCount = args.Count - 1;

        // Make sure we have the right argument count
        if ( argCount > 1 || argCount < 1 )
        {
            Log.Error( "Invalid argument count! You need at least and at most 1 argument specifying the path to the map you want to load." );
            return;
        }

        // If we say we're switching to map "null"...
        if ( args[1].ToString() == "null" )
        {
            // Unload the map
            UnloadMap();
            return;
        }

        // If the specified file doesn't exist...
        if ( !File.Exists( args[1].ToString() ) || !Path.Exists( args[1].ToString() ) )
        {
            // We've encountered an error!
            Log.Error( $"File \"{args[1]}\" not found!" );
            return;
        }

        // Load the map with the specified path
        LoadMap( args[1].ToString() );
    }

    /// <summary>
    /// Loads a map from a specified path.
    /// </summary>
    /// <param name="path">The path to the map we wish to load.</param>
    private void LoadMap( string path )
    {
        currentFile = WTF.LoadFile( path );
        Log.Info( $"Successfully loaded map \"{path}\"!", true );
    }

    /// <summary>
    /// Opens a file dialog to let the user specify the path to a map to load.
    /// </summary>
    private void LoadMapThroughDialog()
    {
        Log.Error( "This feature is not yet implemented! Go fuck yourself!" );
    }

    /// <summary>
    /// Unloads the currently loaded map.
    /// </summary>
    private void UnloadMap()
    {
        currentFile = null;
    }

    [ConsoleCommand("htf_save_map", "Saves the currently loaded map.")]
    private void SaveMap()
    {
        if ( currentFile == null )
        {
            Log.Error( "Can't save map, because no map is loaded!" );
            return;
        }

        WTF.SaveFile( currentFile?.path, currentFile );
        Log.Info( $"Successfully saved map \"{currentFile.path}\"!", true );
    }

    /// <summary>
    /// Saves the current map to a specified path.
    /// </summary>
    private void SaveMap(List<object> args)
    {
        int argCount = args.Count - 1;

        if ( argCount > 1 || argCount < 1 )
        {
            Log.Error( "Invalid argument count! You need at least one and at most 1 argument specifying the path to which you want to save the map to." );
            return;
        }

        if ( currentFile == null )
        {
            Log.Error( "Can't save map, because no map is loaded!" );
            return;
        }

        WTF.SaveFile( args[1].ToString(), currentFile );
        Log.Info( $"Successfully saved map \"{currentFile.path}\"!", true );
    }

    private void SaveMap( WTF inFile )
    {
        WTF.SaveFile( inFile.path, inFile );
        Log.Info( $"Successfully saved map \"{inFile.path}\"!", true );
    }
}