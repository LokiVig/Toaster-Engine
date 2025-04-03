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