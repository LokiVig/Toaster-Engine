using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

using Veldrid;

using Toast.Engine.Attributes;
using Toast.Engine.Resources.Console;

namespace Toast.Engine.Resources.Input;

public static class InputManager
{
    private const string PATH_KEYBINDS = "resources/keybinds.json";

    private static Dictionary<Key, bool> keys = new();
    private static Dictionary<Key, bool> prevKeys = new();

    private static List<Keybind> keybinds = new();

    private static JsonSerializer serializer = new JsonSerializer
    {
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// Initializes the input manager, effectively just fills the keys dictionaries.
    /// </summary>
    public static void Initialize()
    {
        // Enumerate over every key and add it to our dictionary of keys
        // Source:
        //   - https://stackoverflow.com/questions/105372/how-to-enumerate-an-enum
        foreach ( Key key in Enum.GetValues( typeof( Key ) ) )
        {
            // Make sure we have no duplicate keys...
            if ( !keys.ContainsKey( key ) )
            {
                keys.Add( key, false );
            }
        }

        // Log our success!
        Log.Success( "Successfully initialized input manager!" );
    }

    /// <summary>
    /// Things to do when the Veldrid KeyDown event is invoked.
    /// </summary>
    public static void OnKeyDown( KeyEvent ev )
    {
        prevKeys = new Dictionary<Key, bool>( keys );
        keys[ev.Key] = true;
    }

    /// <summary>
    /// Things to do when the Veldrid KeyUp event is invoked.
    /// </summary>
    public static void OnKeyUp( KeyEvent ev )
    {
        prevKeys = new Dictionary<Key, bool>( keys );
        keys[ev.Key] = false;
    }

    /// <summary>
    /// Adds the argument keybind to the input manager's list of keybinds.
    /// </summary>
    public static void AddKeybind( Keybind keybind )
    {
        keybinds.Add( keybind );
    }

    /// <summary>
    /// Removes a keybind from our list of keybinds through a console command.
    /// </summary>
    [ConsoleCommand("removebind", "Removes a bind outright from our list of binds." )]
    public static void RemoveKeybind( List<object> args )
    {
        // Get the count of arguments
        int argCount = args.Count - 1;

        // Make sure we have a correct amount of arguments for this command
        if ( argCount < 1 || argCount > 1 )
        {
            Log.Warning( "Invalid amount of arguments! You need at least, and at most, one argument to specify which keybind you wish to remove!" );
            return;
        }

        // For every keybind...
        for ( int i = 0; i < keybinds.Count; i++ )
        {
            // Check if their alias matches our argument...
            if ( keybinds[i].alias == (string)args[1] )
            {
                // Remove the specified keybind!
                keybinds.RemoveAt( i );
                Log.Info( $"Successfully removed keybind \"{args[1]}\"!" );
                return;
            }
        }

        // Otherwise, we couldn't find the keybind we wanted
        Log.Warning( $"Couldn't find a keybind with the alias of \"{args[1]}\"!" );
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
                // Skip over keys with unknown binds!
                if ( keybind.key == Key.Unknown )
                {
                    continue;
                }

                // If the keybind supports being held down...
                if ( keybind.down )
                {
                    // Check if the key is actively being pressed down...
                    if ( IsKeyDown( keybind.key ) && ( keybind.comboKey != Key.Unknown ? IsKeyDown( keybind.comboKey ) : true ) )
                    {
                        // Find the console command appropriate to this keybind, and invoke it
                        ConsoleManager.GetCommand( keybind.commandAlias )?.onCall.Invoke();
                    }
                }
                else // Otherwise, if the keybind only supports being pressed...
                {
                    // Check if the key has just been pressed...
                    if ( IsKeyPressed( keybind.key ) && ( keybind.comboKey != Key.Unknown ? IsKeyDown( keybind.comboKey ) : true ) )
                    {
                        // Find the console command appropriate to this keybind, and invoke it
                        ConsoleManager.GetCommand( keybind.commandAlias )?.onCall.Invoke();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Finds a keybind from a specified alias.
    /// </summary>
    /// <param name="alias">The alias of the keybind we want to find.</param>
    /// <returns>Returns the found keybind, or <see langword="null"/> if no keybind was found.</returns>
    public static Keybind GetKeybind( string alias )
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
    /// Tries to find a keybind from a specified alias.
    /// </summary>
    /// <param name="alias">The alias of the keybind we want to find.</param>
    /// <param name="keybind">The resulting keybind.</param>
    /// <returns><see langword="true"/> if the keybind was found, <see langword="false"/> otherwise.</returns>
    public static bool TryGetKeybind( string alias, out Keybind keybind )
    {
        return ( keybind = GetKeybind( alias ) ) != null;
    }

    /// <summary>
    /// Changes a keybind through the console.
    /// </summary>
    [ConsoleCommand("bind", "Binds a key to an action defined by an alias." )]
    public static void EditKeybind( List<object> args )
    {
        int argCount = args.Count - 1; // The amount of provided arguments (-1 because the first argument is the command itself in this case)
        Keybind bindToEdit = null; // The bind we're going to edit

        Key key = Key.Unknown; // The forced, base key we should press to trigger this keybind
        Key comboKey = Key.Unknown; // The optional, additional key we should bind this keybind with

        // Make sure we have the right amount of arguments!
        if ( argCount < 2 || argCount > 5 )
        {
            Log.Warning( "Invalid amount of arguments! You need at least 2 arguments, and at most 5, one for the alias of the keybind you want to change, and the other for the key you want to change it to!" );
            return;
        }

        // If we're not making a new keybind...
        if ( argCount < 4 )
        {
            // If we can't find a bind from our first argument...
            if ( !TryGetKeybind( (string)args[1], out bindToEdit ) )
            {
                Log.Warning( $"Couldn't find keybind with the alias of \"{args[1]}\"!" );
                return;
            }
        }

        // If we're featuring a comboKey argument...
        if ( argCount >= 3 )
        {
            // If we didn't successfully find a key from our second argument (comboKey)...
            if ( !Enum.TryParse( (string)args[2], true, out comboKey ) )
            {
                // Log such!
                Log.Warning( $"\"{args[2]}\" is not a valid Key!" );
                return;
            }

            // If we didn't successfully find a key from our third argument (key)...
            if ( !Enum.TryParse( (string)args[3], true, out key ) )
            {
                // Log such!
                Log.Warning( $"\"{args[3]}\" is not a valid Key!" );
                return;
            }
        }
        else // Otherwise...
        {
            // If we didn't successfully find a key from our second argument...
            if ( !Enum.TryParse( (string)args[2], true, out key ) )
            {
                // Log such!
                Log.Warning( $"\"{args[2]}\" is not a valid Key!" );
                return;
            }
        }

        // If we have 4 or more arguments...
        if ( argCount >= 4 )
        {
            bool down = false; // Default down value

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
            if ( !ConsoleManager.TryGetCommand( (string)args[4], out _ ) )
            {
                Log.Warning( $"There's no command with the alias of \"{args[4]}\"!" );
                return;
            }

            // If we have the bool to determine if the keybind can be held...
            if ( argCount == 5 )
            {
                if ( !bool.TryParse( (string)args[5], out down ) )
                {
                    Log.Warning( $"Couldn't parse \"{args[5]}\" to bool!" );
                    return;
                }
            }

            // Make a new keybind
            Keybind kb = new Keybind();
            kb.alias = (string)args[1]; // The alias of the keybind should be the first argument
            kb.key = key; // The base key
            kb.comboKey = comboKey; // The optional, combo key
            kb.commandAlias = (string)args[3]; // The command alias is the third
            kb.down = down; // Even if we didn't have the 4th argument, we should set our newly made keybind's hold functionality to the default

            // Add the new keybind to our list of keybinds
            AddKeybind( kb );
            Log.Info( $"Successfully added new keybind \"{args[1]}\" (key: \"{( comboKey == Key.Unknown ? key : $"{comboKey}+{key}" )}\"), command: \"{args[3]}\", hold: {( argCount < 5 ? "False" : down )}" ); // Log our success
            return;
        }

        bindToEdit.key = key; // Change the bind's key
        bindToEdit.comboKey = comboKey; // Change the bind's combo key, too
        Log.Info( $"Successfully bound action \"{args[1]}\" to key \"{( comboKey == Key.Unknown ? key : $"{comboKey}+{key}" )}\"!", true ); // Log our success
    }

    /// <summary>
    /// Unbinds a keybind through the console.
    /// </summary>
    [ConsoleCommand("unbind", "Unbinds a key from an action. (Effectively just sets its key value to \"Unknown\".)" )]
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
        if ( !TryGetKeybind( (string)args[1], out bindToEdit ) )
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
    [ConsoleCommand("displaybinds", "Displays all bindings." )]
    public static void DisplayKeybinds()
    {
        // Introductory log
        Log.Info( "Available keybindings:" );

        // For every keybind...
        foreach ( Keybind keybind in keybinds )
        {
            // Log its information! (Alias, currently bound key and associated command)
            Log.Info( $"\tAlias: \"{keybind.alias}\" - Key: {( keybind.key == Key.Unknown ? "Unbound" : $"\"{( keybind.comboKey == Key.Unknown ? keybind.key : $"{keybind.comboKey}+{keybind.key}" )}\"" )} - Command: \"{keybind.commandAlias}\" - Supports Being Held Down? {keybind.down}" );
        }
    }

    /// <summary>
    /// Is the specified key currently pressed?
    /// </summary>
    /// <returns><see langword="true"/> if the specified key is defined as being down / pressed, <see langword="false"/> otherwise.</returns>
    public static bool IsKeyDown( Key key )
    {
        return keys[key];
    }

    /// <summary>
    /// Checks if the argument key was just pressed.
    /// </summary>
    /// <returns><see langword="true"/> if the key was just recently pressed, <see langword="false"/> otherwise.</returns>
    public static bool IsKeyPressed( Key key )
    {
        return keys[key] && !prevKeys[key];
    }

    /// <summary>
    /// Is the specified key currently not pressed?
    /// </summary>
    /// <returns><see langword="true"/> if the specified key is not defined as being down / pressed, <see langword="false"/> otherwise.</returns>
    public static bool IsKeyUp( Key key )
    {
        return !keys[key];
    }

    /// <summary>
    /// Checks if the argument key was just released.
    /// </summary>
    /// <returns><see langword="true"/> if the key was just recently released, <see langword="false"/> otherwise.</returns>
    public static bool IsKeyReleased( Key key )
    {
        return !keys[key] && prevKeys[key];
    }
}