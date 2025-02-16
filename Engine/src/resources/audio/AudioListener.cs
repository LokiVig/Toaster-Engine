namespace Toast.Engine.Resources.Audio;

/// <summary>
/// Placing this on an entity means that they should be affected by 3D audio calculations.
/// </summary>
public class AudioListener
{
    public float volumeOffset = 1.0f; // Offset of the audio played back (multiplicative)
}