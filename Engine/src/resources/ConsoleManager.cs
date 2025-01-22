using System.Collections.Generic;

using Toast.Engine.Rendering;

namespace Toast.Engine.Resources;

public static class ConsoleManager
{
    private static List<ConsoleCommand> commands = new();

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
    /// Removes a <see cref="ConsoleCommand"/> from this console manager's list of commands.
    /// </summary>
    public static void RemoveCommand( ConsoleCommand command )
    {
        // Simpy remove the argument command from the list
        commands.Remove( command );
    }
}