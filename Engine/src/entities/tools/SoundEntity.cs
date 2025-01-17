using Toast.Engine.Math;
using Toast.Engine.Resources;

namespace Toast.Engine.Entities.Tools;

public class SoundEntity : ToolEntity
{
    public string audioPath { get; set; } = "resources/audio/engine/error"; // The actual audio we should play
    public string audioAlias { get; set; } = "soundentitysfx"; // The Audio Manager would prefer to have a special alias for different audios, specified here
    public float audioVolume { get; set; } = 1.0f; // The volume of this audio // TODO: Decide if volume's between 0.0-1.0, or 0.0-100.0!
    public bool audioRepeats { get; set; } // Toggles whether or not this audio should repeat

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
        // If we're not already playing our audio...
        if ( !playing )
        {
            // Try to do so!
            if ( AudioManager.PlaySound( audioPath, audioAlias, audioRepeats ) )
            {
                Log.Info( $"Playing sound: \"{audioPath}\"...", true );
                playing = true;
            }
            else // If it failed for some reason...
            {
                // We should return!
                // We shouldn't need to log cause of the fact that the failed AudioManager.PlaySound oughta log
                // the issue for us
                return;
            }
        }
        else // Otherwise...
        {
            // Don't do anything
            Log.Warning( $"{this} is already playing sound: \"{audioPath}\"!" );
            return;
        }
    }

    public void StopSound()
    {
        // If we're playing our audio...
        if ( playing )
        {
            // Stop it!
            AudioManager.StopSound( audioAlias );
            playing = false;
        }
        else // Otherwise...
        {
            // Don't do anything
            Log.Warning( $"{this} has been told to have its audio stop playing, but there's no audio playing!" );
            return;
        }
    }

    protected override void Update()
    {
        base.Update();

        if ( !playing )
        {
            // Remove ourselves from the current scene automatically
            EngineManager.currentScene.RemoveEntity( this );
        }
    }
}