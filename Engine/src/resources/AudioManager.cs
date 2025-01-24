using System;
using System.IO;
using System.Collections.Generic;

using NAudio;
using NAudio.Wave;

namespace Toast.Engine.Resources;

/// <summary>
/// Class that should manage playing, stopping, and updating all manners of audio files.
/// </summary>
public static class AudioManager
{
    private const string PATH_AUDIO_SUCCESS = "resources/audio/engine/success.mp3";
    private const string PATH_AUDIO_WARNING = "resources/audio/engine/warning.mp3";
    private const string PATH_AUDIO_ERROR = "resources/audio/engine/error.mp3";

    private static List<AudioFile> playingFiles = new List<AudioFile>();

    /// <summary>
    /// Plays a sound from the console.
    /// </summary>
    public static void PlaySound( List<object> args )
    {
        // The amount of arguments (-1 cause the first one is always the command itself)
        int argCount = args.Count - 1;

        string filepath = (string)args[1]; // Get the filepath
        float volume = 1.0f; // Default volume
        bool repeats = false; // Default repeat status

        // If we have enough arguments for it...
        if ( argCount >= 1 )
        {
            // Get the volume
            if ( !float.TryParse( (string)args[2], out volume ) )
            {
                Log.Warning( "Second argument is an invalid float!" );
                return;
            }
        }

        // If we have enough arguments for it...
        if ( argCount >= 2 )
        {
            // Do we repeat?
            if ( !bool.TryParse( (string)args[3], out repeats ) )
            {
                Log.Warning( "Third argument is an invalid bool!" );
                return;
            }
        }

        PlaySound( filepath, volume, repeats );
    }

    /// <summary>
    /// Plays a sound effect from a specified path.
    /// </summary>
    /// <param name="filepath">The path to the specific sound we wish to play.</param>
    /// <param name="volume">Determines the volume of which this sound should play at. (Scale of 0.0f - 1.0f)</param>
    /// <param name="repeats">Determines whether or not this sound should repeat (loop) or not.</param>
    public static AudioFile PlaySound( string filepath, float volume = 1.0f, bool repeats = false )
    {
        // Try to...
        try
        {
            // Read the file
            AudioFileReader fileReader = new AudioFileReader( filepath );

            // Initialize a WaveOutEvent
            WaveOutEvent waveEvent = new WaveOutEvent();

            // Make a new file with these variables
            AudioFile file = new AudioFile()
            {
                filepath = filepath,
                volume = volume,
                repeats = repeats,

                fileReader = fileReader,
                waveEvent = waveEvent
            };

            // Actually play the sound
            file.waveEvent.Volume = volume;
            file.waveEvent.Init( fileReader );
            file.waveEvent.Play();

            // Add the new file to our list of playing files
            playingFiles.Add( file );

            // Return our newly made file
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
        // Do things that will effectively make the file repeat
        file.waveEvent.Pause();
        file.fileReader.Position = 0;
        file.waveEvent.Play();
    }

    /// <summary>
    /// Method to update all actively playing audio files.
    /// </summary>
    public static void Update()
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
            if ( file.waveEvent.PlaybackState == PlaybackState.Stopped )
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
                }
            }
        }
    }

    /// <summary>
    /// Stop a specified <see cref="AudioFile"/>.
    /// </summary>
    /// <param name="file">The <see cref="AudioFile"/> we wish to stop the sound of.</param>
    public static void StopSound( AudioFile file, bool dispose = true )
    {
        file.waveEvent.Stop(); // Stop the sound
        file.Dispose(); // Call the file's dispose method

        playingFiles.Remove( file ); // Remove the file from our list
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
        PlaySound( PATH_AUDIO_SUCCESS );
    }

    /// <summary>
    /// Plays the engine's warning sound.
    /// </summary>
    public static void PlayWarning()
    {
        PlaySound( PATH_AUDIO_WARNING );
    }

    /// <summary>
    /// Plays the engine's error sound.
    /// </summary>
    public static void PlayError()
    {
        PlaySound( PATH_AUDIO_ERROR );
    }
}