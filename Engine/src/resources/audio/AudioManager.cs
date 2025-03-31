using System;
using System.IO;
using System.Collections.Generic;

using Silk.NET.OpenAL;

using Toast.Engine.Entities;
using Toast.Engine.Attributes;
using System.Buffers.Binary;
using System.Reflection;
using System.Text;

namespace Toast.Engine.Resources.Audio;

/// <summary>
/// Class that should manage playing, stopping, and updating all manners of audio files.
/// </summary>
public static class AudioManager
{
    // Constant paths to engine-important audio files
    private const string PATH_AUDIO_SUCCESS = "resources/audio/engine/success.wav";
    private const string PATH_AUDIO_WARNING = "resources/audio/engine/warning.wav";
    private const string PATH_AUDIO_ERROR = "resources/audio/engine/error.wav";

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
        //
        // Most of this is from the wave format sample from Silk.NET's OpenAL bindings!
        // If possible, find out a cleaner, if not easier way to handle this... Shouldn't be all too encompassing
        // for this engine's purposes after all, I don't wanna limit ourselves to just wave, either.
        //
        // Source:
        //  - https://github.com/dotnet/Silk.NET/blob/main/examples/CSharp/OpenAL%20Demos/WavePlayer/Program.cs
        //

        try
        {
            // Read the file
            ReadOnlySpan<byte> file = File.ReadAllBytes( filepath );
            int fileIndex = 0;

            // Check if the file is of RIFF format...
            if ( file[fileIndex++] != 'R' || file[fileIndex++] != 'I' || file[fileIndex++] != 'F' || file[fileIndex++] != 'F' )
            {
                Log.Info( "File is not in RIFF format!", true );
                return null;
            }

            // Go back to the start to read other formats
            int chunkSize = BinaryPrimitives.ReadInt32LittleEndian( file.Slice( fileIndex, 4 ) );
            fileIndex += 4;

            // Check if the file is of WAVE format...
            if ( file[fileIndex++] != 'W' || file[fileIndex++] != 'A' || file[fileIndex++] != 'V' || file[fileIndex++] != 'E' )
            {
                Log.Info( "File is not in WAVE format!", true );
                return null;
            }

            short numChannels = -1;
            int sampleRate = -1;
            int byteRate = -1;
            short blockAlign = -1;
            short bitsPerSample = -1;
            BufferFormat format = 0;

            AudioFile audio = null;

            unsafe
            {
                // Get the ALContext and AL API itself
                ALContext alc = ALContext.GetApi();
                AL al = AL.GetApi();

                // Get the device
                Device* device = alc.OpenDevice( $"sound{playingFiles.Count + 1}" );

                // Make sure the device isn't null
                if ( device == null )
                {
                    Log.Info( "Could not create device!", true );
                    return null;
                }

                // Create a context
                Context* ctx = alc.CreateContext( device, null );
                alc.MakeContextCurrent( ctx );

                // Get any errors that might've occurred
                al.GetError();

                uint src = al.GenSource();
                uint buf = al.GenBuffer();

                // If the sound should repeat...
                if ( repeats )
                {
                    // Set its property to reflect that
                    al.SetSourceProperty( src, SourceBoolean.Looping, true );
                }

                // Read the file
                while ( fileIndex + 4 < file.Length )
                {
                    // Read the section identifier
                    string identifier = "" + (char)file[fileIndex++] + (char)file[fileIndex++] + (char)file[fileIndex++] + (char)file[fileIndex++];
                    int size = BinaryPrimitives.ReadInt32LittleEndian( file.Slice( fileIndex, 4 ) );
                    fileIndex += 4;

                    // If we're reading the format section of the file...
                    if ( identifier == "fmt " )
                    {
                        // If the size of the section doesn't fit...
                        if ( size != 16 )
                        {
                            // Log the faulty file!
                            Log.Info( $"Unknown audio format with subchunk1 size: {size}", true );
                            return null;
                        }
                        else // Otherwise, if we have a perfectly fine sized file...
                        {
                            // Get the audio format of the file...
                            short audioFormat = BinaryPrimitives.ReadInt16LittleEndian( file.Slice( fileIndex, 2 ) );
                            fileIndex += 2;

                            // If we have a faulty audio format...
                            if ( audioFormat != 1 )
                            {
                                // Log the faulty file!
                                Log.Info( $"Unknown audio format with ID: {audioFormat}", true );
                                return null;
                            }
                            else // Otherwise, if we have a perfectly fine audio format...
                            {
                                // Get information about the audio...
                                // Get the number of channels
                                numChannels = BinaryPrimitives.ReadInt16LittleEndian( file.Slice( fileIndex, 2 ) );
                                fileIndex += 2;

                                // Get the sample rate
                                sampleRate = BinaryPrimitives.ReadInt32LittleEndian( file.Slice( fileIndex, 4 ) );
                                fileIndex += 4;

                                // Get the byte rate
                                byteRate = BinaryPrimitives.ReadInt32LittleEndian( file.Slice( fileIndex, 4 ) );
                                fileIndex += 4;

                                // Get the block align
                                blockAlign = BinaryPrimitives.ReadInt16LittleEndian( file.Slice( fileIndex, 2 ) );
                                fileIndex += 2;

                                // Get the bits per sample
                                bitsPerSample = BinaryPrimitives.ReadInt16LittleEndian( file.Slice( fileIndex, 2 ) );
                                fileIndex += 2;

                                // If we have mono sound...
                                if ( numChannels == 1 )
                                {
                                    // If we have only 8 bits per sample...
                                    if ( bitsPerSample == 8 )
                                    {
                                        // Format is mono 8-bits-per-sample!
                                        format = BufferFormat.Mono8;
                                    }
                                    else if ( bitsPerSample == 16 ) // If we have a whopping 16 bits per sample...
                                    {
                                        // Format is mono 16-bits-per-sample!
                                        format = BufferFormat.Mono16;
                                    }
                                    else // If we have an unknown bits per sample count...
                                    {
                                        // Log the faulty file!
                                        Log.Info( $"Can't play mono {bitsPerSample} sound!", true );
                                        return null;
                                    }
                                }
                                else if ( numChannels == 2 ) // If we have stereo sound...
                                {
                                    // If we only have 8 bits per sample...
                                    if ( bitsPerSample == 8 )
                                    {
                                        // Format is stereo 8-bits-per-sample!
                                        format = BufferFormat.Stereo8;
                                    }
                                    else if ( bitsPerSample == 16 ) // If we have a whopping 16 bits per sample...
                                    {
                                        // Format is stereo 16-bits-per-sample!
                                        format = BufferFormat.Stereo16;
                                    }
                                    else // If we have an unknown bits per sample count...
                                    {
                                        // Log the faulty file!
                                        Log.Info( $"Can't play stereo {bitsPerSample} sound!", true );
                                        return null;
                                    }
                                }
                                else // If we have an unknown amount of channels...
                                {
                                    // Log the faulty file!
                                    Log.Info( $"Can't play audio with {numChannels} channels!", true );
                                    return null;
                                }
                            }
                        }
                    }
                    else if ( identifier == "data" ) // If this section contains general data...
                    {
                        // Read the data
                        ReadOnlySpan<byte> data = file.Slice( fileIndex, size );
                        fileIndex += size;

                        // Buffer the data to the AL API
                        fixed ( byte* pData = data )
                        {
                            al.BufferData( buf, format, pData, size, sampleRate );
                        }

                        // Log the reading of data
                        Log.Info( $"Read {size} bytes data.", true );
                    }
                    else if ( identifier == "JUNK" ) // If this section is junk...
                    {
                        // This exists to align things
                        fileIndex += size;
                    }
                    else if ( identifier == "iXML" ) // If this section is XML content..
                    {
                        // Read the section
                        ReadOnlySpan<byte> v = file.Slice( fileIndex, size );
                        string str = Encoding.ASCII.GetString( v );

                        // Log the XML information
                        Log.Info( $"iXML chunk: {str}", true );
                        fileIndex += size;
                    }
                    else // Otherwise we've encountered an unknown section...
                    {
                        // Log the unknown section and skip over it!
                        Log.Info( $"Unknown section: {identifier}", true );
                        fileIndex += size;
                    }

                    // Log the successful loading of the file
                    Log.Info( "Detected RIFF-WAVE audio file, PCM encoding.", true );

                    // Create a new audio file with our specifications
                    audio = new AudioFile( numChannels, sampleRate, byteRate, blockAlign, bitsPerSample, format );
                    audio.alContext = alc; // Set the ALContext
                    audio.alApi = al; // Set the AL API
                    audio.alDevice = device; // Set its device
                    audio.ctx = ctx; // Set its local context

                    audio.src = src; // Set its source
                    audio.buf = buf; // Set its buffer

                    // Set the properties of the audio
                    audio.alApi.SetSourceProperty( audio.src, SourceFloat.Gain, volume );
                    audio.alApi.SetSourceProperty( audio.src, SourceInteger.Buffer, audio.buf );

                    // Start playback!
                    audio.alApi.SourcePlay( audio.src );
                }

                // Add the audio file to the list of playing files, and return the newly made playing file!
                playingFiles.Add( audio );
                return audio;
            }
        }
        catch ( Exception exc )
        {
            if ( exc is FileNotFoundException || exc is DirectoryNotFoundException )
            {
                Log.Warning( $"Couldn't find file at \"{filepath}\"!" );
                return null;
            }

            Log.Error( "Exception caught playing sound!", exc );
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

            // Get the state of the file
            file.alApi.GetSourceProperty( file.src, GetSourceInteger.SourceState, out int state );

            // If the file is stopped...
            if ( state == (int)SourceState.Stopped )
            {
                // Remove the file!
                playingFiles.Remove( file );
                continue;
            }
        }
    }

    /// <summary>
    /// Stop a specified <see cref="AudioFile"/>.
    /// </summary>
    /// <param name="file">The <see cref="AudioFile"/> we wish to stop the sound of.</param>
    public static void StopSound( AudioFile file )
    {
        file.alApi.SourceStop( file.src ); // Stop the source
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

            // Dispose of the file
            file.Dispose();
        }

        // Clear the list of playing files
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
        // Header for the information about to be displayed
        Log.Info( "Actively playing files:" );

        // For every file...
        foreach ( AudioFile file in playingFiles )
        {
            // Get the state of the file
            file.alApi.GetSourceProperty( file.src, GetSourceInteger.SourceState, out int state );

            // Log the file's information
            Log.Info( $"\t\"{file.filepath}\" - {file.volume} - {( file.repeats ? "Repeats" : "Does not repeat" )} - {(SourceState)state}" );
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