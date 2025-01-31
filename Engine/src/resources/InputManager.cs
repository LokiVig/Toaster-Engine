using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

using Veldrid;

namespace Toast.Engine.Resources;

public static class InputManager
{
    private const string PATH_KEYBINDS = "resources/keybinds.txt";

    private static List<Key> keysDown = new();
    private static List<Keybind> keybinds = new();

    private static JsonSerializer serializer = new JsonSerializer
    {
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// Things to do when the Veldrid KeyDown event is invoked.
    /// </summary>
    public static void OnKeyDown( KeyEvent ev )
    {
        // If our current list of keys down doesn't contain that key...
        if ( !keysDown.Contains( ev.Key ) )
        {
            // Add it to our list of keys down!
            keysDown.Add( ev.Key );
        }
    }

    /// <summary>
    /// Adds the argument keybind to the input manager's list of keybinds.
    /// </summary>
    public static void AddKeybind( Keybind keybind )
    {
        keybinds.Add( keybind );
    }

    /// <summary>
    /// Loads the list of keybinds from our default keybinds path.
    /// </summary>
    public static bool LoadKeybinds()
    {
        // If there's no keybinds file...
        if ( !File.Exists( PATH_KEYBINDS ) )
        {
            // Return outta here!
            return false;
        }

        // Try to...
        try
        {
            // Open the file...
            using ( StreamReader sr = File.OpenText( PATH_KEYBINDS ) )
            {
                // Read it using a JsonReader...
                using ( JsonReader reader = new JsonTextReader( sr ) )
                {
                    // If we could deserialize it...
                    if ( ( keybinds = serializer.Deserialize<List<Keybind>>( reader ) ) != null )
                    {
                        // Successful operation!
                        return true;
                    }
                }
            }
        }
        catch ( Exception exc )
        {
            Log.Error( "Unmanaged exception caught!", exc );
            return false;
        }

        // Couldn't load the file for any ordinary reason!
        return false;
    }

    /// <summary>
    /// Saves our list of keybinds to the default keybinds path.
    /// </summary>
    public static void SaveKeybinds()
    {
        // Create the file we want to write to!
        FileStream file = File.Open( PATH_KEYBINDS, FileMode.Create );
        file.Close();


        // Try to...
        try
        {
            // Open the file using a writer...
            using ( StreamWriter sw = new StreamWriter( PATH_KEYBINDS ) )
            {
                using ( JsonWriter writer = new JsonTextWriter( sw ) )
                {
                    // Serialize the keybinds list to the file!
                    serializer.Serialize( writer, keybinds );
                }
            }
        }
        catch ( Exception exc )
        {
            Log.Error( "Unmanaged exception caught!", exc );
        }
    }

    /// <summary>
    /// Updates the input manager.
    /// </summary>
    public static void Update()
    {
        // If we currently take inputs...
        if ( EngineManager.TakesInput() )
        {
            // Check every keybind...
            foreach ( Keybind keybind in keybinds )
            {
                // If their key is currently pressed down...
                if ( IsKeyDown( keybind.key ) )
                {
                    // Find the console command appropriate to this keybind, and invoke it
                    ConsoleManager.FindCommand( keybind.commandAlias ).onCall.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Finds a keybind from a specified alias.
    /// </summary>
    public static Keybind FindKeybind( string alias )
    {
        // Check every keybind...
        foreach ( Keybind keybind in keybinds )
        {
            // If their alias matches our search...
            if ( keybind.alias == alias )
            {
                // Return that keybind!
                return keybind;
            }
        }

        // Couldn't find the keybind we searched for, return null!
        return null;
    }

    /// <summary>
    /// Changes a keybind through the console.
    /// </summary>
    public static void EditKeybind( List<object> args )
    {
        int argCount = args.Count - 1; // The amount of provided arguments (-1 because the first argument is the command itself in this case)
        Keybind bindToEdit = null; // The bind we're going to edit

        // Make sure we have the right amount of arguments!
        if ( argCount < 2 || argCount > 3 )
        {
            Log.Warning( "Invalid amount of arguments! You need at least 2 arguments, and at most 3, one for the alias of the keybind you want to change, and the other for the key you want to change it to!" );
            return;
        }

        // If we can't find a bind from our first argument...
        if ( ( bindToEdit = FindKeybind( (string)args[1] ) ) == null )
        {
            Log.Warning( $"Couldn't find keybind with the alias of \"{args[1]}\"!" );
            return;
        }

        // If we didn't successfully find a key from our second argument...
        if ( !Enum.TryParse( (string)args[2], true, out Key key ) )
        {
            // Log such!
            Log.Warning( $"\"{args[2]}\" is not a valid Key!" );
            return;
        }

        // If we have three arguments...
        if ( argCount == 3 )
        {
            // It means we're making a new keybind! Ensure that its name doesn't override an already existing one
            foreach ( Keybind keybind in keybinds )
            {
                if ( args[1].ToString().ToLower() == keybind.alias )
                {
                    Log.Warning( $"There's already a keybind with the name of \"{args[1]}\"!" );
                    return;
                }
            }

            // Make sure the third argument corresponds to an actual command
            if ( ConsoleManager.FindCommand( (string)args[3] ) == null )
            {
                Log.Warning( $"There's no command with the alias of \"{args[3]}\"!" );
                return;
            }

            // Make a new keybind
            Keybind kb = new Keybind();
            kb.alias = (string)args[1]; // The alias of the keybind should be the first argument
            kb.key = key; // The key is the second argument
            kb.commandAlias = (string)args[3]; // The command alias is the third

            // Add the new keybind to our list of keybinds
            AddKeybind( kb );
            Log.Info( $"Successfully added new keybind \"{args[1]}\" (key: \"{args[2]}\"), command: \"{args[3]}\"" ); // Log our success
            return;
        }

        bindToEdit.key = key; // Change the bind's key!
        Log.Info( $"Successfully bound action \"{args[1]}\" to key \"{args[2]}\"!", true ); // Log our success
    }

    /// <summary>
    /// Unbinds a keybind through the console.
    /// </summary>
    public static void UnbindKeybind( List<object> args )
    {
        int argCount = args.Count - 1; // The amount of provided arguments (-1 because the first argument is the command itself in this case)
        Keybind bindToEdit = null; // The bind we're going to edit

        // Make sure we have the right amount of arguments
        if ( argCount < 1 || argCount > 1 )
        {
            Log.Warning( "Invalid amount of arguments! You need at least, and at most, one argument defining the keybind you wish to unbind." );
            return;
        }

        // If we can't find a keybind from our fist argument...
        if ( ( bindToEdit = FindKeybind( (string)args[1] ) ) == null )
        {
            Log.Warning( $"Couldn't find keybind with the alias of \"{args[1]}\"!" );
            return;
        }

        bindToEdit.key = Key.Unknown; // Set the found keybind's key to unknown!
        Log.Info( $"Successfully unbound keybind \"{args[1]}\"!", true ); // Log our success
    }

    /// <summary>
    /// Logs all keybinds to the console.
    /// </summary>
    public static void DisplayKeybinds()
    {
        // Introductory log
        Log.Info( "Available keybindings:" );

        // For every keybind...
        foreach ( Keybind keybind in keybinds )
        {
            // Log its information! (Alias and currently bound key)
            Log.Info( $"\t\"{keybind.alias}\" - \"{keybind.key}\"" );
        }
    }

    /// <summary>
    /// Things to do when the Veldrid KeyUp event is invoked.
    /// </summary>
    public static void OnKeyUp( KeyEvent ev )
    {
        // If our current keys down contains that key...
        if ( keysDown.Contains( ev.Key ) )
        {
            // Remove it from our list of current keys down!
            keysDown.Remove( ev.Key );
        }
    }

    /// <summary>
    /// Is the specified key currently pressed?
    /// </summary>
    /// <returns><see langword="true"/> if the specified key is found in the current list of keys down, <see langword="false"/> otherwise.</returns>
    public static bool IsKeyDown( Key key )
    {
        return keysDown.Contains( key );
    }
}