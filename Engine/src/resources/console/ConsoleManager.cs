using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

using DynamicExpresso;

namespace Toast.Engine.Resources.Console;

public static class ConsoleManager
{
    private const string PATH_COMMANDS = "resources/commands.txt";

    private static List<ConsoleCommand> commands = new();

    private static JsonSerializer serializer = new()
    {
        Formatting = Formatting.Indented,
    };

    /// <summary>
    /// Adds a <see cref="ConsoleCommand"/> to this console manager's list of commands.
    /// </summary>
    public static void AddCommand( ConsoleCommand command )
    {
        // Simply add the argument command to the list
        commands.Add( command );
    }

    /// <summary>
    /// Searches through this console manager's list of commands and returns one fitting with the argument <paramref name="commandAlias"/>.
    /// </summary>
    public static ConsoleCommand FindCommand( string commandAlias )
    {
        // Check every command...
        foreach ( ConsoleCommand command in commands )
        {
            // If this command's alias fits the argument alias...
            if ( command.alias == commandAlias.ToLower() )
            {
                // Return it!
                return command;
            }
        }

        // We couldn't find one, log a warning and return null
        Log.Warning( $"Couldn't find command with the alias of \"{commandAlias}\"!" );
        return null;
    }

    /// <summary>
    /// Try to load our list of commands from our saved path.
    /// </summary>
    public static bool LoadCommands()
    {
        // If we don't have a commands file...
        if ( !File.Exists( PATH_COMMANDS ) )
        {
            // Get outta dodge!
            return false;
        }

        try
        {
            // Open the file...
            using ( StreamReader sr = new StreamReader( PATH_COMMANDS ) )
            {
                // Read the JSON data...
                using ( JsonReader reader = new JsonTextReader( sr ) )
                {
                    // If we did find our list of commands...
                    if ( ( commands = serializer.Deserialize<List<ConsoleCommand>>( reader ) ) != null )
                    {
                        // We should interpret the command action that's defined in the file!
                        Interpreter interpreter = new Interpreter();
                        foreach ( ConsoleCommand command in commands )
                        {
                            command.onCall = interpreter.ParseAsDelegate<Action>( command.onCallAlias );
                            command.onArgsCall = interpreter.ParseAsDelegate<Action<List<object>>>( command.onArgsCallAlias );
                        }

                        // Successful command loading!
                        return true;
                    }
                }
            }
        }
        catch ( Exception exc ) // Unexpected exception has been caught!
        {
            Log.Error( "Unmanaged exception caught!", exc );
            return false;
        }

        // Some other error happened
        return false;
    }

    public static void SaveCommands()
    {
        // Create a new file at the default path
        FileStream file = File.Open( PATH_COMMANDS, FileMode.Create );
        file.Close();

        // Open the file...
        using ( StreamWriter sw = new StreamWriter( PATH_COMMANDS ) )
        {
            // Create a new JSON writer...
            using ( JsonWriter writer = new JsonTextWriter( sw ) )
            {
                // For every command...
                foreach ( ConsoleCommand command in commands )
                {
                    // Set their command's alias to the name of their connected method
                    command.onCallAlias = command.onCall.Method.Name;
                    command.onArgsCallAlias = command.onArgsCall.Method.Name;
                }

                // Serialize the JSON data to the file!
                serializer.Serialize( writer, commands );
            }
        }
    }

    /// <summary>
    /// Gets the total list of console commands.
    /// </summary>
    public static List<ConsoleCommand> GetConsoleCommands()
    {
        return commands;
    }

    /// <summary>
    /// Log every available command to the console, and their description
    /// </summary>
    public static void DisplayCommands()
    {
        // Display a header / introduction to what we just did
        Log.Info( "List of available commands and their status:" );

        // For every command...
        foreach ( ConsoleCommand command in commands )
        {
            // Display its information!
            Log.Info( $"\t{command.alias} - {command.description} {( command.enabled ? "" : "(*DISABLED*)" )}" );
        }
    }

    /// <summary>
    /// Log the information about a specific command.
    /// </summary>
    public static void DisplayCommand( List<object> args )
    {
        // Find the command
        ConsoleCommand command = FindCommand( (string)args[1] );

        // If we have found command...
        if ( command != null )
        {
            // Display its info!
            Log.Info( $"\t{args[1]} - {command.description} {( command.enabled ? "" : "(*DISABLED*)" )}" );
        }
    }

    /// <summary>
    /// Toggles a command through the console.
    /// </summary>
    public static void ToggleCommand( List<object> args )
    {
        // Find the command
        ConsoleCommand command = FindCommand( (string)args[1] );

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

    /// <summary>
    /// Try to call the command from our inputs.
    /// </summary>
    public static void TryCommand(string input)
    {
        // Log the input
        Log.Info( input );

        // Make sure the input isn't actually empty
        if ( input != string.Empty )
        {
            // All of the collective arguments
            // Null by default, allows for commands without arguments to be called too
            object[] args = null;

            // If our input has spaces...
            if ( input.Contains( " " ) )
            {
                // We should split our arguments on these spaces!
                args = input.Split( " " );
            }

            // Find our command, either from the first variable of our args list, or directly from our input
            ConsoleCommand command = FindCommand( args != null ? (string)args[0] : input );

            // Make sure our command actually is found...
            if ( command == null )
            {
                // If not, return!!!
                return;
            }

            // If this command is disabled...
            if ( !command.enabled )
            {
                // Log such to the console and get outta here!
                Log.Info( $"Found command \"{command.alias}\", but the command is disabled, therefore we cannot call it!", true );
                return;
            }

            // If the command requires cheats, but cheats are disabled...
            if ( command.requiresCheats && !EngineManager.cheatsEnabled )
            {
                // Log this revelating information to the console, then skedaddle!
                Log.Info( $"Command \"{command.alias}\" requires cheats, but cheats are disabled!", true );
                return;
            }

            // If we have arguments...
            if ( args != null )
            {
                // Call the argumented version of this command's function!
                command.onArgsCall?.Invoke( new List<object>( args ) );
            }
            else // Otherwise!
            {
                // Call the command's regular function!
                command.onCall?.Invoke();
            }
        }
    }

    /// <summary>
    /// An un-argumented invalid command.
    /// </summary>
    public static void InvalidCommand()
    {
        Log.Warning( "This command only supports arguments! Please try to run the command again, with fitting arguments!" );
    }

    /// <summary>
    /// An argumented invalid command.
    /// </summary>
    public static void InvalidCommand( List<object> args )
    {
        Log.Warning( "This command does not support arguments! Please try to run the command again, without any arguments!" );
    }

    /// <summary>
    /// Removes a <see cref="ConsoleCommand"/> from this console manager's list of commands.
    /// </summary>
    public static void RemoveCommand( ConsoleCommand command )
    {
        // Simpy remove the argument command from the list
        commands.Remove( command );
    }
}