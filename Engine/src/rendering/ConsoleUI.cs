using System;
using System.Numerics;
using System.Collections.Generic;

using ImGuiNET;

using Toast.Engine.Resources;

namespace Toast.Engine.Rendering;

/// <summary>
/// The main UI element to display the console.
/// </summary>
public static class ConsoleUI
{
    private static List<string> logs = new(); // This console's log of messages
    private static string input = string.Empty; // The current user input, used for console commands, etc.

    /// <summary>
    /// Displays the console.
    /// </summary>
    public static void Display( ref bool open )
    {
        // The main console window
        if ( ImGui.Begin( "Console", ref open,
             ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize ) )
        {
            // We should only take inputs when the console isn't focused
            EngineManager.ToggleInput( !ImGui.IsWindowFocused() );

            // Set the console window's size to one that'd fit even the smallest of screens
            ImGui.SetWindowSize( new Vector2( 800f, 600f ) );

            // The actual logs!
            if ( ImGui.BeginChild( "Logs", new Vector2( 782.5f, 542.5f ),
                 ImGuiChildFlags.Borders | ImGuiChildFlags.AlwaysUseWindowPadding,
                 ImGuiWindowFlags.ChildWindow ) )
            {
                // Make a wrapped text for every log
                for ( int i = 0; i < logs.Count; i++ )
                {
                    ImGui.TextWrapped( logs[i] );
                    ImGui.SetScrollY( ImGui.GetScrollMaxY() );
                }

                ImGui.EndChild();
            }

            if ( ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows) && !ImGui.IsAnyItemActive() && !ImGui.IsMouseClicked(0) )
            {
                ImGui.SetKeyboardFocusHere( 0 );
            }

            // Text input handler
            if ( ImGui.InputText( "##", ref input, 2048, ImGuiInputTextFlags.NoUndoRedo | ImGuiInputTextFlags.EnterReturnsTrue ) )
            {
                TryCommand();
            }

            ImGui.SameLine();

            // Button to console the current text input
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
                args = localInput.Split( " " );
            }

            // Find our command, either from the first variable of our args list, or directly from our input
            ConsoleCommand command = ConsoleManager.FindCommand( args != null ? (string)args[0] : localInput );

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
        logs.Add( $"({DateTime.Now.ToLongTimeString()}) : {message}\n" ); // TODO: Implement some way to determine if this
                                                                          // log is from the engine, game, otherwise
    }
}