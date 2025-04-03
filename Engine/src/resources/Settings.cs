﻿using System;
using System.IO;

using Newtonsoft.Json;

using Toast.Engine.Attributes;
using Toast.Engine.Rendering;
using Toast.Engine.Resources.Audio;

using Veldrid;

namespace Toast.Engine.Resources;

/// <summary>
/// The settings of the engine.
/// </summary>
public class Settings
{
    /// <summary>
    /// Path to the settings.<br/>
    /// The settings are saved in a JSON format.
    /// </summary>
    public const string PATH_SETTINGS = "settings.json";

    //---------------------------------------//
    //             Audio Settings            //
    //---------------------------------------//

    /// <summary>
    /// The current audio backend of the engine.
    /// </summary>
    public AudioBackend AudioBackend { get; set; }

    //---------------------------------------//
    //           Renderer Settings           //
    //---------------------------------------//

    /// <summary>
    /// The current window state of the engine.
    /// </summary>
    public WindowState WindowState { get; set; }

    /// <summary>
    /// Determines whether or not VSync is enabled.
    /// </summary>
    public bool VSyncEnabled { get; set; }

    /// <summary>
    /// The resolution of the window.
    /// </summary>
    public (int width, int height) WindowResolution { get; set; }

    /// <summary>
    /// The JSON serializer for the settings.
    /// </summary>
    private static JsonSerializer serializer = new JsonSerializer()
    {
        Formatting = Formatting.Indented,
    };

    /// <summary>
    /// Saves settings as a JSON file at the path of <see cref="PATH_SETTINGS"/>.
    /// </summary>
    public void Save()
    {
        // Automatically gather all audio settings
        // ... There are none here, yet!

        // Automatically gather all renderer settings
        WindowState = Renderer.GetWindowState();
        VSyncEnabled = Renderer.IsVSyncEnabled();
        WindowResolution = (Renderer.GetWindowWidth(), Renderer.GetWindowHeight());

        // Save the settings to a file using JSON
        using ( StreamWriter sw = new StreamWriter( PATH_SETTINGS ) )
        {
            using ( JsonWriter writer = new JsonTextWriter( sw ) )
            {
                serializer.Serialize( writer, this );
            }
        }
    }

    /// <summary>
    /// Console command that saves the engine's current settings.
    /// </summary>
    [ConsoleCommand("savesettings", "Saves the engine's current settings.")]
    public static void SaveEngineSettings()
    {
        // Simply call the engine's setting's save method
        EngineManager.settings.Save();
    }

    /// <summary>
    /// Loads settings from a JSON file at the path of <see cref="PATH_SETTINGS"/>.
    /// </summary>
    public static Settings Load()
    {
        try
        {
            // Return value
            Settings settings = null;

            // Load the settings from our JSON file
            using ( StreamReader sr = new StreamReader( PATH_SETTINGS ) )
            {
                using ( JsonReader reader = new JsonTextReader( sr ) )
                {
                    settings = serializer.Deserialize<Settings>( reader );
                }
            }

            // If it resulted in null...
            if ( settings == null )
            {
                // Log our error and return null!
                Log.Error( "Error loading settings, serializer failed deserializing / file isn't encoded properly!" );
                return null;
            }

            // Log our success and return the successfully deserialized settings!
            Log.Success( "Successfully loaded settings!" );
            return settings;
        }
        catch ( Exception exc )
        {
            // If the file doesn't exist...
            if ( exc is FileNotFoundException || exc is DirectoryNotFoundException )
            {
                // Log a standard error explaining what went wrong
                Log.Error( "Error loading settings, there is no existing settings file!" );
                return null;
            }

            // We've hit an unmanaged exception, throw it!
            Log.Error( "Error loading settings!", exc );
            return null;
        }
    }
}