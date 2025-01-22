using System;
using System.Numerics;
using System.Collections.Generic;

using ImGuiNET;

using Toast.Engine.Resources;

namespace Toast.Engine.Rendering;

public static class ConsoleUI
{
    private static List<string> logs = new();
    private static string input = string.Empty;

    public static void Open( ref bool open )
    {
        open = true;

        ImGui.SetNextWindowSize( new Vector2( 1000f, 500f ) );

        if ( ImGui.Begin( "Console", ref open, ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoDocking ) )
        {
            if ( ImGui.BeginChild( "Logs", ImGui.GetWindowSize() / 1.15f, ImGuiChildFlags.Borders ) )
            {
                for ( int i = 0; i < logs.Count; i++ )
                {
                    if ( ImGui.Selectable( $"{logs[i]}" ) )
                    {
                        ImGui.SetClipboardText( logs[i] );
                    }
                }

                ImGui.EndChild();
            }

            if ( ImGui.InputText( " ", ref input, 2048, ImGuiInputTextFlags.NoUndoRedo | ImGuiInputTextFlags.EnterReturnsTrue ) )
            {
                TryCommand();
            }

            ImGui.SameLine();

            if ( ImGui.Button( "Input" ) )
            {
                TryCommand();
            }

            ImGui.End();
        }
    }

    /// <summary>
    /// Try to call the command from our inputs.
    /// </summary>
    private static void TryCommand()
    {
        // Localize the input value
        string localInput = input;

        // Make the input empty, helps clean things up both visually and functionally
        // Don't want our recently entered command to still be in there :p
        input = string.Empty;

        // Log the input
        Log.Info( localInput );

        // Make sure the input isn't actually empty
        if ( localInput != string.Empty )
        {
            // All of the collective arguments
            // Null by default, allows for commands without arguments to be called too
            object[] args = null;

            // If our input has spaces...
            if ( localInput.Contains( " " ) )
            {
                // We should split our arguments on these spaces!
                args = localInput.Split(" ");
            }

            // Find our command, either from the first variable of our args list, or directly from our input
            ConsoleCommand command = ConsoleManager.FindCommand( args != null ? (string)args[0] : localInput );

            // Make sure our command actually is found...
            if ( command == null )
            {
                // If not, return!!!
                return;
            }

            // If we have arguments...
            if ( args != null )
            {
                // Call the argumented version of this command's function!
                command.onArgsCall?.Invoke( new List<object>(args) );
            }
            else // Otherwise!
            {
                // Call the command's regular function!
                command.onCall?.Invoke();
            }
        }
    }

    /// <summary>
    /// Clear the console of all logs
    /// </summary>
    public static void Clear()
    {
        // Simply call the log list's clear function
        logs.Clear();
    }

    /// <summary>
    /// Writes a string to the console, with a newline character affixed to it.
    /// </summary>
    public static void WriteLine( string message )
    {
        // Add the argument message to our list of logs
        logs.Add( $"({DateTime.Now.ToLongTimeString()}) : <ENGINE> {message}\n" ); // TODO: Implement some way to determine if this
                                                                                   // log is from the engine, game, otherwise
    }
}