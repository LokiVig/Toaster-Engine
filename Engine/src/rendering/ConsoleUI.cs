using System;
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

        if ( ImGui.Begin( "Console", ref open, ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoDocking ) )
        {
            if ( ImGui.TreeNodeEx( "Logs", ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.DefaultOpen ) )
            {
                for ( int i = 0; i < logs.Count; i++ )
                {
                    if ( ImGui.Selectable( $"{logs[i]}" ) )
                    {
                        ImGui.SetClipboardText( logs[i] );
                    }
                }

                ImGui.TreePop();
            }

            ImGui.Separator();

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
        // Log the input
        Log.Info( input, true );

        // Make sure the input isn't actually empty
        if ( input != string.Empty )
        {
            // All of the collective arguments
            // Null by default, allows for commands without arguments to be called too
            string[] args = null;

            // If our input has spaces...
            if ( input.Contains( " " ) )
            {
                // We should split our arguments on these spaces!
                args = input.Split(" ");
            }

            // Find our command, either from the first variable of our args list, or directly from our input
            ConsoleCommand command = ConsoleManager.FindCommand( args != null ? args[0] : input );

            // Make sure our command actually is found...
            if ( command == null )
            {
                // If not, warning!!!
                Log.Warning($"Couldn't find console command with the alias of \"{input}\"!");
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

            // Make the input empty, helps clean things up both visually and functionally
            // Don't want our recently entered command to still be in there :p
            input = string.Empty;
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