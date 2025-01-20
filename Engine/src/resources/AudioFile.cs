using System;

namespace Toast.Engine.Resources;

public class AudioFile : IDisposable
{
    public string filepath;
    public string alias;
    public float volume;
    public bool repeats;

    public void Dispose()
    {
        filepath = null;
        alias = null;
    }
}