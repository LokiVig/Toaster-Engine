using System.Numerics;

using Toast.Engine.Resources;

namespace Toast.Engine.Entities.Tools;

public class SoundEntity : ToolEntity
{
    public string audioPath = "resources/audio/engine/error.mp3"; // The actual audio we should play
    public string audioAlias = "soundentitysfx"; // The Audio Manager would prefer to have a special alias for different audios, specified here
    public float audioVolume = 1.0f; // The volume of this audio // TODO: Decide if volume's between 0.0<->1.0, or 0.0<->100.0!
    public bool audioRepeats; // Toggles whether or not this audio should repeat

    private bool playing; // Determines whether or not this entity's already playing an audio

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
            if ( AudioManager.PlaySound( audioPath, audioAlias, audioVolume, audioRepeats ) )
            {
                Log.Info( $"Playing sound: \"{audioPath}\"...", true );
            }
            else // If it failed for some reason...
            {
                // We should return!
                // We shouldn't need to log cause of the fact that the failed AudioManager.PlaySound oughta log
                // the issue for us
                return;
            }
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

        playing = AudioManager.FileIsPlaying( AudioManager.FindPlayingFile( audioAlias ) );
    }

    public void StopSound()
    {
        // If we're playing our audio...
        if ( playing )
        {
            AudioManager.StopSound(AudioManager.FindPlayingFile(audioAlias).alias);
            playing = false;
        }
    }
}