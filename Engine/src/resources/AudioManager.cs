using System;
using System.Collections.Generic;
using System.IO;

using NAudio.Wave;

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
    private static List<WaveOutEvent> waveOutEvents = new List<WaveOutEvent>();

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
            // Create a new event
            WaveOutEvent waveOut = new WaveOutEvent();

            // Read the MP3 file
            Mp3FileReader mp3Reader = new Mp3FileReader( filepath );

            // Initialize and play the MP3 file
            waveOut.Volume = volume;
            waveOut.Init( mp3Reader );
            waveOut.Play();

            // Add our event to our list of events
            waveOutEvents.Add( waveOut );

            // Create a new AudioFile for us to return
            AudioFile file = new AudioFile()
            {
                // Basic info
                filepath = filepath,
                volume = volume,
                repeats = repeats,

                // Audio related variables
                mp3FileReader = mp3Reader,
                waveOutEvent = waveOut
            };

            // Dispose our local audio related variables
            mp3Reader.Dispose();
            waveOut.Dispose();

            // Add our new file to the list of playing files
            playingFiles.Add( file );

            // Return our newly created audio file variable
            return file;
        }
        catch ( Exception exc ) // Problem caught!
        {
            if ( exc is FileNotFoundException )
            {
                Log.Warning( $"Couldn't find file at path \"{filepath}\"!" );
                return null;
            }

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
        //file.waveOutEvent.Stop(); // Stop it at its end
        //file.mp3FileReader.Position = 0; // Set its position to 0 (the start of the audio file)
        //file.waveOutEvent.Play(); // Play the sound again
    }

    /// <summary>
    /// Method to update all actively playing audio files.
    /// </summary>
    public static void UpdatePlayingFiles()
    {
        // Check every actively playing file...
        foreach ( AudioFile file in playingFiles )
        {
            // If its status is stopped...
            if ( file.waveOutEvent.PlaybackState == PlaybackState.Stopped )
            {
                // If the file should repeat...
                if ( file.repeats )
                {
                    // Do things that will make this file repeat itself
                    RepeatSound( file );
                }
                else // Otherwise!
                {
                    // Just stop the sound as per regular
                    StopSound( file );
                    continue;
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
        file.waveOutEvent.Stop(); // Stop the input file's sound
        file.Dispose(); // Also dispose of it
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