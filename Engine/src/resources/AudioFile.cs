using System;

using NAudio.Wave;

namespace Toast.Engine.Resources;

public class AudioFile : IDisposable
{
    public string filepath;
    public float volume;
    public bool repeats;

    public Mp3FileReader mp3FileReader;
    public WaveOutEvent waveOutEvent;

    public void Dispose()
    {
        filepath = null;

        mp3FileReader.Dispose();
        waveOutEvent.Dispose();
    }
}