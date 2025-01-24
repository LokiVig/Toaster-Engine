using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

using DynamicExpresso;

namespace Toast.Engine.Resources;

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
    /// Sets the status of a specific command, specified from the argument <paramref name="commandAlias"/>.
    /// </summary>
    public static void SetCommandStatus( string commandAlias, bool commandEnabled )
    {
        ConsoleCommand foundCommand = FindCommand( commandAlias ); // Find the command we want

        // If the command we tried to find isn't null...
        if ( foundCommand != null )
        {
            foundCommand.enabled = commandEnabled; // Set the command's status
        }
        else // Otherwise!
        {
            // We can't really do anything with a null command
            Log.Warning( "foundCommand was null! Can't set status of a null command, dingus." );
            return;
        }
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
        if ( !File.Exists( PATH_COMMANDS ) )
        {
            return false;
        }

        try
        {
            using ( StreamReader sr = new StreamReader( PATH_COMMANDS ) )
            {
                using ( JsonReader reader = new JsonTextReader( sr ) )
                {
                    if ( ( commands = serializer.Deserialize<List<ConsoleCommand>>( reader ) ) != null )
                    {
                        Interpreter interpreter = new Interpreter();
                        foreach ( ConsoleCommand command in commands )
                        {
                            command.onCall = interpreter.ParseAsDelegate<Action>( command.onCallAlias );
                            command.onArgsCall = interpreter.ParseAsDelegate<Action<List<object>>>( command.onArgsCallAlias );
                        }

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

        // Some other error happened
        return false;
    }

    public static void SaveCommands()
    {
        FileStream file = File.Open( PATH_COMMANDS, FileMode.Create );
        file.Close();

        using ( StreamWriter sw = new StreamWriter( PATH_COMMANDS ) )
        {
            using ( JsonWriter writer = new JsonTextWriter( sw ) )
            {
                foreach ( ConsoleCommand command in commands )
                {
                    command.onCallAlias = command.onCall.Method.Name;
                    command.onArgsCallAlias = command.onArgsCall.Method.Name;
                }

                serializer.Serialize( writer, commands );
            }
        }
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