﻿using System.Collections.Generic;

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
            Log.Info( $"\t{command.alias} - {command.description} {(command.enabled ? "" : "(*DISABLED*)")}" );
        }
    }

    /// <summary>
    /// Log the information about a specific command.
    /// </summary>
    /// <param name="commandAlias">The alias of the <see cref="ConsoleCommand"/> we wish to display info of.</param>
    public static void DisplayCommand( string commandAlias )
    {
        // Find the command
        ConsoleCommand command = FindCommand( commandAlias );

        // If we have found command...
        if ( command != null )
        {
            // Display its info!
            Log.Info( $"\t{commandAlias} - {command.description} {(command.enabled ? "" : "(*DISABLED*)")}" );
        }
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