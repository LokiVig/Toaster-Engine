using System;

using NAudio.Wave;

namespace Toast.Engine.Resources;

public class AudioFile : IDisposable
{
    public string filepath;
    public float volume;
    public bool repeats;

    public AudioFileReader fileReader;
    public WaveOutEvent waveEvent;

    public void Dispose()
    {
        filepath = null;
        waveEvent.Dispose();
    }
}