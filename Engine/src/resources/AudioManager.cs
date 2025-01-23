using System;
using System.IO;
using System.Collections.Generic;

using AudioPlayer = NetCoreAudio.Player;

namespace Toast.Engine.Resources;

/// <summary>
/// Class that should manage playing, stopping, and updating all manners of audio files.
/// </summary>
public static class AudioManager
{
    private const string AUDIO_SUCCESS_PATH = "resources/audio/engine/success.mp3";
    private const string AUDIO_WARNING_PATH = "resources/audio/engine/warning.mp3";
    private const string AUDIO_ERROR_PATH = "resources/audio/engine/error.mp3";

    private static List<AudioFile> playingFiles = new List<AudioFile>();

    /// <summary>
    /// Plays a sound effect from a specified path.
    /// </summary>
    /// <param name="filepath">The path to the specific sound we wish to play.</param>
    /// <param name="volume">Determines the volume of which this sound should play at. (Scale of 0.0f - 1.0f)</param>
    /// <param name="repeats">Determines whether or not this sound should repeat (loop) or not.</param>
    public static AudioFile PlaySound( string filepath, byte volume = 100, bool repeats = false )
    {
        // Try to...
        try
        {
            AudioPlayer audioPlayer = new AudioPlayer(); // Initialize an audio player
            audioPlayer.SetVolume( volume ); // Set its volume
            audioPlayer.Play( filepath ); // And play the sound from the argument file path!

            // Make a new file
            AudioFile file = new AudioFile()
            {
                filepath = filepath,
                volume = volume,
                repeats = repeats,

                audioPlayer = audioPlayer
            };

            // Add it to our list of playing files
            playingFiles.Add( file );

            // And return it!
            return file;
        }
        catch ( Exception exc ) // Problem caught!
        {
            // If we couldn't find the file...
            if ( exc is FileNotFoundException )
            {
                // We shouldn't throw the exception, just log a warning that we couldn't find it!
                Log.Warning( $"Couldn't find file at path \"{filepath}\"!" );
                return null;
            }

            // Log the error and throw the provided exception
            Log.Error( $"Exception caught when trying to play sound \"{filepath}\"", exc );
            return null;
        }
    }

    /// <summary>
    /// Automatic method which allows an audio file to repeat.
    /// </summary>
    private static void RepeatSound( AudioFile file )
    {
        // Do things that will effectively make it repeat
        file.audioPlayer.Pause(); // Pause the audio
        PlaySound( file.filepath, file.volume, file.repeats ); // Play the same file again
        file.Dispose(); // Then dispose of the file
    }

    /// <summary>
    /// Method to update all actively playing audio files.
    /// </summary>
    public static void UpdatePlayingFiles()
    {
        // Check every actively playing file...
        for ( int i = 0; i < playingFiles.Count; i++ )
        {
            AudioFile file = playingFiles[i];

            // If we encountered a null file...
            if ( file == null )
            {
                playingFiles.Remove( file ); // Remove it from the list!
                continue; // Continue to the next file
            }

            // If its status is stopped...
            if ( !file.audioPlayer.Playing )
            {
                // If the file should repeat...
                if ( file.repeats )
                {
                    RepeatSound( file ); // Do things that will make this file repeat itself
                    playingFiles[i] = null; // Remove the file from the list
                }
                else // Otherwise!
                {
                    StopSound( file ); // Just stop the sound as per regular
                    file.Dispose(); // Call the file's dispose method
                    playingFiles[i] = null; // Also remove it from the list
                }
            }
        }
    }

    /// <summary>
    /// Stop a specified <see cref="AudioFile"/>.
    /// </summary>
    /// <param name="file">The <see cref="AudioFile"/> we wish to stop the sound of.</param>
    public static void StopSound( AudioFile file )
    {
        file.audioPlayer.Stop(); // Stop the input file's sound
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
    /// Plays the engine's success sound.
    /// </summary>
    public static void PlaySuccess()
    {
        PlaySound( AUDIO_SUCCESS_PATH );
    }

    /// <summary>
    /// Plays the engine's warning sound.
    /// </summary>
    public static void PlayWarning()
    {
        PlaySound( AUDIO_WARNING_PATH );
    }

    /// <summary>
    /// Plays the engine's error sound.
    /// </summary>
    public static void PlayError()
    {
        PlaySound( AUDIO_ERROR_PATH );
    }
}