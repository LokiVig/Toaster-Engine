using System;

using Silk.NET.OpenAL;

namespace Toast.Engine.Resources.Audio;

public unsafe class AudioFile : IDisposable
{
    public string filepath;
    public float volume;
    public bool repeats;

    public AL alApi;
    public Device* alDevice;
    public ALContext alContext;

    public Context* ctx;

    public uint src;
    public uint buf;

    private short numChannels;
    private int sampleRate;
    private int byteRate;
    private short blockAlign;
    private short bitsPerSample;
    private BufferFormat format;

    public AudioFile(
        short numChannels = -1,
        int sampleRate = -1,
        int byteRate = -1,
        short blockAlign = -1,
        short bitsPerSample = -1,
        BufferFormat format = 0 )
    {
        this.numChannels = numChannels;
        this.sampleRate = sampleRate;
        this.byteRate = byteRate;
        this.blockAlign = blockAlign;
        this.bitsPerSample = bitsPerSample;
        this.format = format;
    }

    public void Dispose()
    {
        //
        // Dispose of OpenAL-related variables
        //

        // Get the state of this source
        alApi.GetSourceProperty( src, GetSourceInteger.SourceState, out int state );

        // If the source isn't stopped...
        if ( state != (int)SourceState.Stopped )
        {
            // Stop it!
            alApi.SourceStop( src );
        }

        // Handle smaller variables
        alApi.DeleteSource( src );
        alApi.DeleteBuffer( buf );
        alContext.DestroyContext( ctx );
        alContext.CloseDevice( alDevice );
        
        // Dispose of the API and context
        alApi.Dispose();
        alContext.Dispose();
    }
}