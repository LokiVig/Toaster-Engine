using System;
using System.IO;
using System.Collections.Generic;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using Toast.Engine.Entities;
using Toast.Engine.Attributes;

namespace Toast.Engine.Resources.Audio;

/// <summary>
/// Class that should manage playing, stopping, and updating all manners of audio files.
/// </summary>
public static class AudioManager
{
    // Constant paths to engine-important audio files
    private const string PATH_AUDIO_SUCCESS = "resources/audio/engine/success.mp3";
    private const string PATH_AUDIO_WARNING = "resources/audio/engine/warning.mp3";
    private const string PATH_AUDIO_ERROR = "resources/audio/engine/error.mp3";

    // Our list of actively playing sound files
    private static List<AudioFile> playingFiles = new List<AudioFile>();

    /// <summary>
    /// Plays a sound from the console.
    /// </summary>
    [ConsoleCommand( "playsound", "Plays a sound from a specified path (should be something like \"resources/audio/engine/error.mp3\".)" )]
    public static void PlaySound( List<object> args )
    {
        // The amount of arguments (-1 cause the first one is always the command itself)
        int argCount = args.Count - 1;

        string filepath = args[1].ToString().ToLower(); // Get the filepath
        float volume = 1.0f; // Default volume
        bool repeats = false; // Default repeat status

        // If we have enough arguments for it...
        if ( argCount >= 2 )
        {
            // Get the volume
            if ( !float.TryParse( args[2].ToString().Replace( ".", "," ), out volume ) )
            {
                Log.Warning( "Second argument is an invalid float!" );
                return;
            }
        }

        // If we have enough arguments for it...
        if ( argCount >= 3 )
        {
            // Do we repeat?
            if ( !bool.TryParse( (string)args[3], out repeats ) )
            {
                Log.Warning( "Third argument is an invalid bool!" );
                return;
            }
        }

        // Call the regular play sound method with our parsed arguments
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

            // Create a volume provider for independent volume control
            VolumeSampleProvider volumeProvider = new VolumeSampleProvider( fileReader.ToSampleProvider() )
            {
                Volume = volume // Set the volume
            };

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
            file.waveEvent.Init( volumeProvider );
            file.waveEvent.Play();

            // Add the new file to our list of playing files
            playingFiles.Add( file );

            // Return our newly made file
            return file;
        }
        catch ( Exception exc ) // Problem caught!
        {
            // If we couldn't find the file...
            if ( exc is FileNotFoundException || exc is DirectoryNotFoundException )
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
    /// Plays a sound from a specified entity, to another specified entity.
    /// </summary>
    /// <param name="source">The source of this audio.</param>
    /// <param name="listener">A listener to this audio.</param>
    /// <param name="filepath">The path to the specific sound we wish to play.</param>
    /// <param name="volume">Determines the volume of which this sound should play at. (Scale of 0.0f - 1.0f)</param>
    /// <param name="repeats">Determines whether or not this sound should repeat (loop) or not.</param>
    public static AudioFile PlaySound( Entity source, Entity listener, string filepath, float volume = 1.0f, bool repeats = false )
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Automatic method which allows an audio file to repeat.
    /// </summary>
    private static void RepeatSound( AudioFile file )
    {
        //
        // Do things that will effectively make the file repeat.
        //

        file.waveEvent.Pause(); // Pause the playback
        playingFiles.Remove( file ); // Remove the file from our list of playing files

        file.fileReader.Position = 0; // Reset the position back to the start
        file.waveEvent.Play(); // Play the audio
        playingFiles.Add( file ); // Add the file back to our list of playing files
    }

    /// <summary>
    /// Method to update all actively playing audio files.
    /// </summary>
    public static void Update()
    {
        // Check every actively playing file...
        for ( int i = 0; i < playingFiles.Count; i++ )
        {
            // Get the current file
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
                }
                else // Otherwise...
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
    public static void StopSound( AudioFile file )
    {
        file.waveEvent.Stop(); // Stop the sound
        file.Dispose(); // Call the file's dispose method

        playingFiles.Remove( file ); // Remove the file from our list
    }

    /// <summary>
    /// Method to stop all currently playing sounds.
    /// </summary>
    [ConsoleCommand( "stopsounds", "Stops all actively playing sounds." )]
    public static void StopAllSounds()
    {
        // For every playing file...
        for ( int i = 0; i < playingFiles.Count; i++ )
        {
            // Get the current file
            AudioFile file = playingFiles[i];

            // Do the same things as the 
            file.waveEvent.Stop();
            file.Dispose();
        }

        playingFiles.Clear();
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
    /// <returns><see langword="true"/> if <see cref="playingFiles"/> contains the given file, <see langword="false"/> otherwise.</returns>
    public static bool FileIsPlaying( AudioFile file )
    {
        return playingFiles.Contains( file );
    }

    /// <summary>
    /// Displays the list of actively playing files.
    /// </summary>
    [ConsoleCommand( "displaysounds", "Displays all currently playing audio files." )]
    public static void DisplayPlayingFiles()
    {
        Log.Info( "Actively playing files:" );

        foreach ( AudioFile file in playingFiles )
        {
            Log.Info( $"\t\"{file.filepath}\" - {file.volume} - {( file.repeats ? "Repeats" : "Does not repeat" )}" );
        }
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