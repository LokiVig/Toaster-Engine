using System;

using AudioPlayer = NetCoreAudio.Player;

namespace Toast.Engine.Resources;

public class AudioFile : IDisposable
{
    public string filepath;
    public byte volume;
    public bool repeats;

    public AudioPlayer audioPlayer;

    public void Dispose()
    {
        filepath = null;
        audioPlayer = null;
    }
}