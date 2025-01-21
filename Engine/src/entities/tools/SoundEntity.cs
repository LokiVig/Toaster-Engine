using System.Numerics;

using Toast.Engine.Resources;

namespace Toast.Engine.Entities.Tools;

public class SoundEntity : ToolEntity
{
    public string audioPath = "resources/audio/engine/error.mp3"; // The actual audio we should play
    public float audioVolume = 1.0f; // The volume of this audio, this should be a range between 0.0<->1.0!
    public bool audioRepeats; // Toggles whether or not this audio should repeat

    private bool playing; // Determines whether or not this entity's already playing an audio
    private AudioFile localAudioFile; // Our local audio file, generated when we play our sound

    public SoundEntity()
    {
        SetBBox( new BBox( new Vector3( -8 ), new Vector3( 8 ) ) );
    }

    public SoundEntity( Vector3 position ) : base( position )
    {
        SetBBox( new BBox( new Vector3( -8 ), new Vector3( 8 ) ) );
    }

    public void PlaySound()
    {
        // If we're not already playing our sound...
        if ( !playing )
        {
            // Try to play our sound!
            localAudioFile = AudioManager.PlaySound( audioPath, audioVolume, audioRepeats );
        }
        else // Otherwise, if we are, we should get a warning!
        {
            Log.Warning( $"Can't play sound \"{audioPath}\" as we're already playing a sound!" );
            return;
        }
    }

    protected override void Update()
    {
        base.Update();

        playing = AudioManager.FileIsPlaying( localAudioFile );
    }

    public void StopSound()
    {
        // If we're playing our audio...
        if ( playing )
        {
            AudioManager.StopSound( localAudioFile );
            playing = false;
        }
    }
}