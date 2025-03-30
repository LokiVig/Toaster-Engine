using System;

namespace Toast.Engine.Resources.Audio;

public class AudioFile : IDisposable
{
    public string filepath;
    public float volume;
    public bool repeats;

    public void Dispose()
    {

    }
}