using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Toast.Engine.Resources;

public class AudioManager
{
    [DllImport( "winmm.dll", CharSet = CharSet.Auto )]
    private static extern int mciSendString( [In] string command, [Optional, In, Out] char[] returnValue, [Optional, In] int returnLength, [Optional, In] IntPtr callback );

    private const string AUDIO_SUCCESS_PATH = "resources/audio/engine/success.mp3";
    private const string AUDIO_WARNING_PATH = "resources/audio/engine/warning.mp3";
    private const string AUDIO_ERROR_PATH = "resources/audio/engine/error.mp3";

    private List<AudioFile> playingFiles = new List<AudioFile>();

    /// <summary>
    /// Plays a sound effect from a specified path.
    /// </summary>
    /// <param name="path">The path to the specific sound we wish to play.</param>
    public void PlaySound( string path, string alias = "sfx" )
    {
        // If the file from the specified path doesn't actually exist...
        if ( !File.Exists( path ) )
        {
            // We've gotten an error!
            EngineProgram.DoError( $"File \"{path}\" doesn't exist!" );
            return;
        }

        mciSendString( $"open \"{path}\" type mpegvideo alias {alias}" ); // Load the file
        mciSendString( $"play {alias}" ); // Play the sound
        playingFiles.Add( new AudioFile { filepath = path, alias = alias } ); // Add a new playing file to the list
    }

    /// <summary>
    /// Updates every single audio file currently in our <see cref="playingFiles"/> list.
    /// </summary>
    public void UpdateAllPlayingFiles()
    {
        // Check every playing audio file...
        for (int i = playingFiles.Count - 1; i >= 0; i--)
        {
            // The current file
            AudioFile file = playingFiles[i];

            // Get its status
            char[] status = new char[128];
            mciSendString( $"status {file.alias} mode", status, status.Length, IntPtr.Zero );

            // If its status is "stopped"...
            if (status.ToString() == "stopped" )
            {
                // Send the command to close it, and remove it from our list.
                mciSendString( $"close {file.alias}" );
                playingFiles.Remove( file );
            }
        }
    }

    /// <summary>
    /// Plays a looping sound, for e.g. music or ambience.
    /// </summary>
    /// <param name="path">The path to the specific, looping sound we wish to play.</param>
    public static void PlayLooping( string path )
    {
        //    if ( !File.Exists( path ) )
        //    {
        //        PlayError();
        //        Console.WriteLine( $"AudioManager.PlayLooping(string): ERROR; File \"{path}\" doesn't exist!" );
        //        return;
        //    }

        //    player = new SoundPlayer( path );
        //    player.Load();
        //    player.PlayLooping();
    }

    /// <summary>
    /// Plays the engine's success sound.
    /// </summary>
    public void PlaySuccess()
    {
        PlaySound( AUDIO_SUCCESS_PATH, "success" );
    }

    /// <summary>
    /// Plays the engine's warning sound
    /// </summary>
    public void PlayWarning()
    {
        PlaySound( AUDIO_WARNING_PATH, "warning" );
    }

    /// <summary>
    /// Plays the engine's error sound.
    /// </summary>
    public void PlayError()
    {
        PlaySound( AUDIO_ERROR_PATH, "error" );
    }
}