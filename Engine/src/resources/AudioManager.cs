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
    /// <param name="alias">Effectively which channel this sound is played on. This should be unique!</param>
    /// <param name="repeats">Determines whether or not this sound should repeat (loop) or not.</param>
    public void PlaySound( string path, string alias = "sfx", bool repeats = false )
    {
        // Index of repeated alias files
        int i = 0;

        // If the file from the specified path doesn't actually exist...
        if ( !File.Exists( path ) )
        {
            // We've gotten an error!
            Log.Warning( $"File \"{path}\" doesn't exist!" );
            return;
        }

        // Check every other currently playing audio for if they have the same alias
        for ( i = 0; i < playingFiles.Count; i++ )
        {
            AudioFile currentFile = playingFiles[i];

            // If they do...
            if ( currentFile.alias == alias )
            {
                // Generate a more unique alias for it
                break;
            }
        }

        alias = $"{alias}{i}";

        mciSendString( $"open \"{path}\" type mpegvideo alias {alias}" ); // Load the file

        // If this sound effect should repeat (e.g. if the sound is ambience or music)...
        if ( repeats )
        {
            mciSendString( $"play {alias} wait repeat" ); // Play the sound repeatedly
        }
        else // Otherwise...
        {
            mciSendString( $"play {alias}" ); // Play the sound once
        }

        playingFiles.Add( new AudioFile { filepath = path, alias = alias, repeats = repeats } ); // Add a new playing file to the list
    }

    /// <summary>
    /// Updates every single audio file currently in our <see cref="playingFiles"/> list.
    /// </summary>
    public void UpdateAllPlayingFiles()
    {
        for (int i = 0; i < playingFiles.Count; i++ )
        {
            // Get the current file
            AudioFile file = playingFiles[i];

            // Get its status
            char[] status = new char[128];
            mciSendString( $"status {file.alias} mode", status, status.Length, IntPtr.Zero );
            string strStatus = new string( status );

            Log.Info( $"Status of sound \"{file.filepath}\" (alias \"{file.alias}\"): \"{strStatus}\"" );

            // If its status is "stopped"...
            if ( strStatus.Contains( "stopped" ) )
            {
                StopSound( file.alias ); // Call the stop sound function, doing as what the name suggests
            }
        }
    }

    /// <summary>
    /// Stops a specific sound effect based on its alias.
    /// </summary>
    /// <param name="alias">The alias, effectively sound channel, of the sound we wish to stop.</param>
    public void StopSound( string alias )
    {
        // Check every file...
        foreach ( AudioFile file in playingFiles )
        {
            // If we have a file with this alias...
            if ( file.alias == alias )
            {
                mciSendString( $"stop {alias}" ); // Stop the sound
                mciSendString( $"close {alias}" ); // Close the sound
                playingFiles.Remove( file ); // Remove it from our list of playing sounds

                return; // Get outta here! After this is the fail case
            }
        }

        // We couldn't find a file with the specified alias!
        Log.Warning( $"Couldn't find sound file with alias of \"{alias}\" in our list of playing sounds!" );
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