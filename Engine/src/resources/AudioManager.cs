using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Toast.Engine.Resources;

public static class AudioManager
{
    [DllImport( "winmm.dll", CharSet = CharSet.Auto )]
    private static extern int mciSendString( [In] string command, [Optional, In, Out] char[] returnValue, [Optional, In] int returnLength, [Optional, In] IntPtr callback );

    private const string AUDIO_SUCCESS_PATH = "resources/audio/engine/success.mp3";
    private const string AUDIO_WARNING_PATH = "resources/audio/engine/warning.mp3";
    private const string AUDIO_ERROR_PATH = "resources/audio/engine/error.mp3";

    private static List<AudioFile> playingFiles = new List<AudioFile>();

    /// <summary>
    /// Plays a sound effect from a specified path.
    /// </summary>
    /// <param name="path">The path to the specific sound we wish to play.</param>
    /// <param name="alias">Effectively which channel this sound is played on. This should be unique!</param>
    /// <param name="volume">Determines the volume of which this sound should play at.</param>
    /// <param name="repeats">Determines whether or not this sound should repeat (loop) or not.</param>
    public static bool PlaySound( string path, string alias = "sfx", float volume = 1.0f, bool repeats = false )
    {
        // Index of repeated alias files
        int i = 0;

        // If the file from the specified path doesn't actually exist...
        if ( !File.Exists( path ) )
        {
            // We've gotten an error!
            Log.Warning( $"File \"{path}\" doesn't exist!" );
            return false;
        }

        // Get the index of the sound that should now play
        for ( i = 0; i < playingFiles.Count; i++ ) ;

        // Apply the index to the alias
        alias = $"{alias}_{i}";

        mciSendString( $"open \"{path}\" type mpegvideo alias {alias}" ); // Load the file

        // If this sound effect should repeat (e.g. if the sound is ambience or music)...
        if ( repeats )
        {
            mciSendString( $"play {alias} repeat" ); // Play the sound repeatedly
        }
        else // Otherwise...
        {
            mciSendString( $"play {alias}" ); // Play the sound once
        }

        playingFiles.Add( new AudioFile { filepath = path, alias = alias, volume = volume, repeats = repeats } ); // Add a new playing file to the list

        // Successful audio playback! Returning true...
        return true;
    }

    /// <summary>
    /// Returns the AudioManager's list of currently playing <see cref="AudioFile"/>'s.
    /// </summary>
    public static List<AudioFile> GetPlayingFiles()
    {
        return playingFiles;
    }

    /// <summary>
    /// Checks whether or not the argument file is in our list of playing files,<br/>
    /// therefore is playing.
    /// </summary>
    public static bool FileIsPlaying( AudioFile file )
    {
        return playingFiles.Contains( file );
    }

    /// <summary>
    /// Find a currently playing file from an alias.
    /// </summary>
    public static AudioFile FindPlayingFile( string alias )
    {
        // For every file we're currently playing...
        foreach ( AudioFile file in playingFiles )
        {
            // Check if the current one contains the argument alias...
            if ( file.alias.Contains( alias ) )
            {
                // If so, return it!
                return file;
            }
        }

        // Couldn't find one! Returning null
        return null;
    }

    /// <summary>
    /// Updates every single audio file currently in our <see cref="playingFiles"/> list.
    /// </summary>
    public static void UpdateAllPlayingFiles()
    {
        for ( int i = 0; i < playingFiles.Count; i++ )
        {
            // Get the current file
            AudioFile file = playingFiles[i];

            // Get its status
            char[] status = new char[128];
            mciSendString( $"status {file.alias} mode", status, status.Length, IntPtr.Zero );
            string strStatus = new string( status );

            //Log.Info( $"Status of sound \"{file.filepath}\" (alias \"{file.alias}\"): \"{strStatus}\"" );

            // If its status is "stopped"...
            if ( strStatus.Contains( "stopped" ) || string.IsNullOrEmpty( strStatus ) )
            {
                StopSound( file.alias ); // Call the stop sound function, doing as what the name suggests
            }
        }
    }

    /// <summary>
    /// Stops a specific sound effect based on its alias.
    /// </summary>
    /// <param name="alias">The alias, effectively sound channel, of the sound we wish to stop.</param>
    public static void StopSound( string alias )
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
                file.Dispose(); // Dispose of the file

                return; // Get outta here! After this is the fail case
            }
        }

        // We couldn't find a file with the specified alias!
        Log.Warning( $"Couldn't find sound file with alias of \"{alias}\" in our list of playing sounds!" );
    }

    /// <summary>
    /// Plays the engine's success sound.
    /// </summary>
    public static void PlaySuccess()
    {
        PlaySound( AUDIO_SUCCESS_PATH, "success" );
    }

    /// <summary>
    /// Plays the engine's warning sound
    /// </summary>
    public static void PlayWarning()
    {
        PlaySound( AUDIO_WARNING_PATH, "warning" );
    }

    /// <summary>
    /// Plays the engine's error sound.
    /// </summary>
    public static void PlayError()
    {
        PlaySound( AUDIO_ERROR_PATH, "error" );
    }
}