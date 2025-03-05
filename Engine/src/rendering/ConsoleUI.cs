using System;
using System.Numerics;
using System.Collections.Generic;

using ImGuiNET;

using Toast.Engine.Attributes;
using Toast.Engine.Resources.Console;

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

            // If the window's focused...
            if ( ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) && !ImGui.IsAnyItemActive() && !ImGui.IsMouseClicked( 0 ) )
            {
                // Set they keyboard focus on our input
                ImGui.SetKeyboardFocusHere( 0 );
            }

            // Text input handler
            if ( ImGui.InputText( "##", ref input, 2048, ImGuiInputTextFlags.NoUndoRedo | ImGuiInputTextFlags.EnterReturnsTrue ) )
            {
                ConsoleManager.TryCommand( input );
                input = string.Empty;
            }

            ImGui.SameLine();

            // Button to console the current text input
            if ( ImGui.Button( "Input" ) )
            {
                ConsoleManager.TryCommand( input );
                input = string.Empty;
            }

            ImGui.End();
        }
    }

    /// <summary>
    /// Clear the console of all logs
    /// </summary>
    [ConsoleCommand("clear", "Clears the console's logs. (Does NOT clear the log file!)")]
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
        logs.Add( $"({DateTime.Now.ToLongTimeString()}) : {message}\n" );
    }
}