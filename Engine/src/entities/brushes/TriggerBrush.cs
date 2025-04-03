using System;
using System.Numerics;

using Toast.Engine.Resources;

namespace Toast.Engine.Entities.Brushes;

/// <summary>
/// A brush defining stuff to do when e.g. an entity or player enters it.
/// </summary>
public class TriggerBrush : BrushEntity
{
    // List of value constants
    private static readonly int DefaultIValue = 0;
    private static readonly float DefaultFValue = 0.0f;
    private static readonly int DefaultBValue = -1;
    private static readonly Vector3 DefaultV3Value = Vector3.Zero;
    private static readonly Quaternion DefaultQValue = Quaternion.Identity;
    private static readonly Entity DefaultEValue = null;
    private static readonly BoundingBox DefaultBBValue = BoundingBox.One;

    public int iValue = DefaultIValue; // Event int value
    public float fValue = DefaultFValue; // Event float value
    public int bValue = DefaultBValue; // Event bool value (<=-1 -> none, =0 -> false, >=1 -> true)
    public Vector3 v3Value = DefaultV3Value; // Event Vector3 value
    public Quaternion qValue = DefaultQValue; // Event Quaternion value
    public Entity eValue = DefaultEValue; // Event Entity value
    public BoundingBox bbValue = DefaultBBValue; // Event BBox value

    public string targetEntity; // The entity we wish to target (decided by an entity's ID)
    public EntityEvent targetEvent; // The desired event

    public TriggerOn triggerOn; // When should this trigger, trigger?
    public TriggerType triggerType; // Which type of trigger is this?
    public TriggerBy triggerBy; // What should this trigger, trigger from?
    public int triggerCount; // The max amount of times this trigger should be triggered

    public Entity previousTriggerEntity; // The entity who last interacted with this trigger

    private int triggeredCount; // The amount of times this trigger has been triggered
    private bool hasTriggered; // Determines whether this trigger has already been triggered

    public TriggerBrush() : base()
    {
    }

    public TriggerBrush( Vector3 position ) : base( position )
    {
    }

    public TriggerBrush( Entity parent ) : base( parent )
    {
    }

    public TriggerBrush( Entity parent, Vector3 position ) : base( parent, position )
    {
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();

        // Reset standard values
        triggerCount = 0;
        hasTriggered = false;
    }

    protected override void Update()
    {
        base.Update();

        // Check if any entity is intersecting with us
        foreach ( Entity entity in EngineManager.currentFile?.entities )
        {
            // Skip over intersection maths if we're disabled
            // Check our count over our max count
            if ( triggerType == TriggerType.Count && triggeredCount > triggerCount )
            {
                break;
            }

            // Check if we trigger only once, and if we have been triggered
            if ( triggerType == TriggerType.Once && hasTriggered )
            {
                break;
            }

            // Do the actual intersection math
            if ( transform.boundingBox.IntersectingWith( entity.GetBoundingBox() ) )
            {
                // If we do intersect, but the trigger fails for some reason...
                if ( !OnTrigger( entity ) )
                {
                    // Try with the next entity!
                    continue;
                }

                // Otherwise, we've surely successfully been triggered, break out of the loop
                break;
            }
            else
            {
                // Otherwise, if we're not interesecting with an entity, and it was our previous trigger entity...
                if ( entity == previousTriggerEntity )
                {
                    // Clear the previous trigger entity!
                    previousTriggerEntity = null;
                }
            }
        }
    }

    /// <summary>
    /// Has this TriggerBrush been triggered?<br/>
    /// Main use for this is the Debug UI, determining whether or not this TriggerBrush has already been<br/>
    /// triggered when our <see cref="triggerType"/> is <see cref="TriggerType.Once"/>.
    /// </summary>
    public bool HasBeenTriggered()
    {
        return hasTriggered;
    }

    /// <summary>
    /// How many times has this TriggerBrush been triggered?<br/>
    /// Main use for this is the Debug UI, displaying how many times this TriggerBrush has been<br/>
    /// triggered when our <see cref="triggerType"/> is <see cref="TriggerType.Count"/>.
    /// </summary>
    public int TriggeredCount()
    {
        return triggeredCount;
    }

    /// <summary>
    /// Things to do when this trigger has triggered.
    /// </summary>
    /// <param name="triggerEntity">The entity that triggered this trigger.</param>
    public bool OnTrigger( Entity triggerEntity )
    {
        // Depending on when we should be triggered...
        switch ( triggerOn )
        {
            case TriggerOn.Enter: // Trigger only when first entered
                // If the previous trigger entity is not the new one, or the previous one was just null, someone has just entered the trigger
                if ( previousTriggerEntity != triggerEntity || previousTriggerEntity == null )
                {
                    break;
                }

                // Otherwise, stop trying to be triggered
                return false;

            case TriggerOn.Exit: // Trigger only when an entity has just exited
                // When the currently triggering entity is null, but the previous one isn't, someone has just left the trigger
                if ( triggerEntity == null && previousTriggerEntity != null )
                {
                    break;
                }

                // Otherwise, stop trying to be triggered
                return false;

            case TriggerOn.Trigger: // Trigger no matter what happens
            default: // Also the default
                break;
        }

        // Depending on which trigger type we are...
        switch ( triggerType )
        {
            case TriggerType.Once: // Only trigger once
                if ( hasTriggered )
                {
                    return false;
                }

                break;

            case TriggerType.Count: // Trigger only a certain amount of times
                if ( triggeredCount >= triggerCount )
                {
                    return false;
                }

                break;

            case TriggerType.Multiple: // Always trigger, there is no stopping it.
            default: // Also the default

                // To make sure we don't continuosly intersect with the same entity every frame,
                // do a check to make sure the entity has left the trigger before re-triggering
                if ( previousTriggerEntity == triggerEntity )
                {
                    return false;
                }

                break;
        }

        // By default we should ignore other brush entities and all tool entities
        if ( triggerEntity is ToolEntity || triggerEntity is BrushEntity )
        {
            return false;
        }

        // Depending on what we should be triggered by...
        switch ( triggerBy )
        {
            case TriggerBy.All: // Any entity should be able to trigger this trigger
            default: // Also the default
                break;

            case TriggerBy.Player: // Only players should be able to trigger this
                if ( triggerEntity is PlayerEntity )
                {
                    break;
                }
                else
                {
                    return false;
                }

            case TriggerBy.NPC: // Only NPCs should be able to trigger this
                if ( triggerEntity is NPCEntity )
                {
                    break;
                }
                else
                {
                    return false;
                }
        }

        // Get the entity we're supposed to effect
        Entity foundTarget = EngineManager.currentScene.FindEntity( targetEntity );

        // Make sure our found target isn't null
        if ( foundTarget == null )
        {
            // Though, if it is...
            // We've got a null reference error!
            Log.Error<NullReferenceException>( $"Trigger brush {this} found entity returned null!" );
            return false;
        }

        // Log all information
        Log.Info( $"TriggerBrush {this} has been triggered." );
        Log.Info( $"\tTriggerer: {triggerEntity}" );
        Log.Info( $"\tTarget: {foundTarget}" );
        Log.Info( $"\tEvent: {targetEvent}" );
        Log.Info( $"\tValues:" );
        Log.Info( $"\t\tiValue: {iValue}" );
        Log.Info( $"\t\tfValue: {fValue}" );
        Log.Info( $"\t\tbValue: {( bValue == DefaultBValue ? "N/A" : ( bValue >= 1 ? "False" : "True" ) )}" );
        Log.Info( $"\t\tv3Value: {v3Value}" );
        Log.Info( $"\t\tqValue: {qValue}" );
        Log.Info( $"\t\teValue: {(eValue == DefaultEValue ? "N/A" : eValue)}" );
        Log.Info( $"\t\tbbValue: {bbValue}" );
        Log.Info( $"\tTrigger type: {triggerType}" );
        Log.Info( $"\tTrigger by: {triggerBy}" );
        Log.Info( $"\tTrigger on: {triggerOn}" );

        if ( iValue != DefaultIValue ) // Int value event
        {
            foundTarget.OnEvent( targetEvent, iValue, this );
        }

        if ( fValue != DefaultFValue ) // Float value event
        {
            foundTarget.OnEvent( targetEvent, fValue, this );
        }

        if ( bValue != DefaultBValue ) // Bool value event
        {
            foundTarget.OnEvent( targetEvent, bValue >= 1, this );
        }

        if ( v3Value != DefaultV3Value ) // Vector3 value event
        {
            foundTarget.OnEvent( targetEvent, v3Value, this );
        }

        if ( qValue != DefaultQValue ) // Quaternion value event
        {
            foundTarget.OnEvent( targetEvent, qValue, this );
        }

        if ( eValue != DefaultEValue ) // Entity value event
        {
            foundTarget.OnEvent( targetEvent, eValue, this );
        }

        if ( bbValue != DefaultBBValue ) // BBox value event
        {
            foundTarget.OnEvent( targetEvent, bbValue, this );
        }

        // Regular event, not taking any special inputs
        foundTarget.OnEvent( targetEvent, this );

        // We've triggered this trigger successfully!
        triggeredCount++; // Triggered count goes up
        hasTriggered = true; // We have triggered
        previousTriggerEntity = triggerEntity; // Previous trigger entity is set to the one who last interacted with us

        // If we're only supposed to trigger once...
        if ( triggerType == TriggerType.Once )
        {
            // We should delete ourself after a triggering!
            //Remove();
        }

        // Return true, we've successfully been triggered!
        return true;
    }
}

/// <summary>
/// Determines when a trigger should be triggered.
/// </summary>
public enum TriggerOn
{
    Trigger, // Either on enter or exit
    Enter, // Only when an entity has entered
    Exit // Only when an entity has left
}

/// <summary>
/// Determines what this trigger can be triggered by.
/// </summary>
public enum TriggerBy
{
    All, // Trigger by all things
    Player, // Trigger only by the player
    NPC, // Trigger only by NPCs
}

/// <summary>
/// Effectively determines how many times this trigger can be triggered.
/// </summary>
public enum TriggerType
{
    Once, // Triggers once, then removes itself
    Count, // Triggers X amount of times before being removed
    Multiple // Can trigger multiple times
}