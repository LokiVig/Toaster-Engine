using System;
using System.IO;
using System.Collections.Generic;

using Toast.Engine;
using Toast.Engine.Rendering;
using Toast.Engine.Resources;
using Toast.Engine.Attributes;
using Toast.Engine.Resources.Input;
using Toast.Engine.Resources.Console;

using Toast.HTF.Rendering;

namespace Toast.HTF;

public class Program
{
    [STAThread]
    public static void Main()
    {
        HTFManager htf = new HTFManager();
        htf.Initialize();
    }
}

/// <summary>
/// Toaster Engine's map editor.<br/>
/// HTF stands for "Here's The Files!".
/// </summary>
public class HTFManager
{
    /// <summary>
    /// Is the object properties UI open?
    /// </summary>
    private static bool entityEditOpen;
    
    /// <summary>
    /// Determines whether or not the currently opened file has had any recent edits<br/>
    /// since it was last saved.
    /// </summary>
    private bool isDirty = false;

    /// <summary>
    /// Initialize the WTFEdit program
    /// </summary>
    public void Initialize()
    {
        // Create HTF keybinds
        CreateKeybinds();

        // Initialize the engine
        EngineManager.OnUpdate += Update;
        EngineManager.OnUIUpdate += UIUpdate;
        EngineManager.Initialize( "HTF" );
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

        InputManager.AddKeybind( new Keybind()
        {
            alias = "open_entity_edit",
            key = Veldrid.Key.Enter,
            comboKey = Veldrid.Key.AltLeft,
            commandAlias = "htf_entity_edit"
        } );
    }

    /// <summary>
    /// Things to do every frame
    /// </summary>
    private void Update()
    {
        Renderer.SetWindowTitle( $"HTF{( EngineManager.currentFile == null ? "" : $" - \"{EngineManager.currentFile.path}\"{( isDirty ? "*" : "" )}" )}" );
    }

    /// <summary>
    /// Updates UI elements, such as ImGui.
    /// </summary>
    private void UIUpdate()
    {
        if ( entityEditOpen )
        {
            EntityEditUI.Display( ref entityEditOpen, null );
        }
    }

    /// <summary>
    /// Toggles whether or not the entity edit UI should be visible.
    /// </summary>
    [ConsoleCommand("htf_entity_edit", "Toggles the entity edit UI.")]
    private static void ToggleEntityEdit()
    {
        entityEditOpen = !entityEditOpen;
    }

    /// <summary>
    /// Loads a map through the console.
    /// </summary>
    /// <param name="args"></param>
    [ConsoleCommand( "htf_load_map", "Loads a map from a specified path (e.g. \"maps/test.wtf\")." )]
    private static void LoadMap( List<object> args )
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
    public static void LoadMap( string path )
    {
        EngineManager.currentFile = WTF.LoadFile( path );
        Log.Info( $"Successfully loaded map \"{path}\"!", true );
    }

    /// <summary>
    /// Opens a file dialog to let the user specify the path to a map to load.
    /// </summary>
    [ConsoleCommand( "htf_load_map_dialog", "Loads a map from a specified path given from an opened file dialog." )]
    private static void LoadMapThroughDialog()
    {
        Log.Error( "This feature is not yet implemented! Go fuck yourself!" );
    }

    /// <summary>
    /// Unloads the currently loaded map.
    /// </summary>
    public static void UnloadMap()
    {
        EngineManager.currentFile = null;
    }

    [ConsoleCommand( "htf_save_map", "Saves the currently loaded map." )]
    public static void SaveMap()
    {
        if ( EngineManager.currentFile == null )
        {
            Log.Error( "Can't save map, because no map is loaded!" );
            return;
        }

        WTF.SaveFile( EngineManager.currentFile?.path, EngineManager.currentFile );
        Log.Info( $"Successfully saved map \"{EngineManager.currentFile.path}\"!", true );
    }

    /// <summary>
    /// Saves the current map to a specified path.
    /// </summary>
    [ConsoleCommand( "htf_save_map", "Saves the currently loaded map." )]
    private static void SaveMap( List<object> args )
    {
        int argCount = args.Count - 1;

        if ( argCount > 1 || argCount < 1 )
        {
            Log.Error( "Invalid argument count! You need at least one and at most 1 argument specifying the path to which you want to save the map to." );
            return;
        }

        if ( EngineManager.currentFile == null )
        {
            Log.Error( "Can't save map, because no map is loaded!" );
            return;
        }

        WTF.SaveFile( (string)args[1], EngineManager.currentFile );
        Log.Info( $"Successfully saved map \"{EngineManager.currentFile.path}\"!", true );
    }
}