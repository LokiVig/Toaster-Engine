using System;

using NAudio.Wave;

namespace Toast.Engine.Resources.Audio;

public class AudioFile : IDisposable
{
    public string filepath;
    public float volume;
    public bool repeats;

    public AudioFileReader fileReader;
    public WaveOutEvent waveEvent;

    public void Dispose()
    {
        // Dispose of everything...
        fileReader.Dispose();
        waveEvent.Dispose();

        // Nullify everything...
        filepath = null;
        fileReader = null;
        waveEvent = null;
    }
}