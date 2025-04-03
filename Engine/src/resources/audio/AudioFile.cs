using System;

using Silk.NET.OpenAL;

namespace Toast.Engine.Resources.Audio;

public class AudioFile : IDisposable
{
    public string filepath;
    public float volume;
    public bool repeats;

    public AL alApi;
    public ALContext alContext;

    public unsafe Device* alDevice;
    public unsafe Context* alCtx;

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
        if ( state != (int)SourceState.Stopped || state != 0 )
        {
            // Stop it!
            alApi.SourceStop( src );
        }

        // Handle smaller variables
        alApi.DeleteSource( src );
        alApi.DeleteBuffer( buf );

        unsafe
        {
            alContext.DestroyContext( alCtx );
            alContext.CloseDevice( alDevice );
        }

        // Dispose of the API and context
        alApi.Dispose();
        alContext.Dispose();
    }
}