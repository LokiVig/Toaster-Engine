﻿using Toast.Engine.Math;
using Toast.Engine.Resources;

namespace Toast.Engine.Entities.Brushes;

/// <summary>
/// A brush defining stuff to do when e.g. an entity or player enters it.
/// </summary>
public class TriggerBrush : BrushEntity
{
    // List of value constants
    private static readonly int IVALUE_DEFAULT = 0;
    private static readonly float FVALUE_DEFAULT = 0.0f;
    private static readonly int BVALUE_DEFAULT = -1;
    private static readonly Vector3 VVALUE_DEFAULT = Vector3.Zero;
    private static readonly Quaternion QVALUE_DEFAULT = Quaternion.Identity;
    private static readonly Entity EVALUE_DEFAULT = null;
    private static readonly BBox BBVALUE_DEFAULT = BBox.One;

    public int iValue { get; set; } = IVALUE_DEFAULT; // Event int value
    public float fValue { get; set; } = FVALUE_DEFAULT; // Event float value
    public int bValue { get; set; } = BVALUE_DEFAULT; // Event bool value (<=-1 -> none, =0 -> false, >=1 -> true)
    public Vector3 vValue { get; set; } = VVALUE_DEFAULT; // Event Vector3 value
    public Quaternion qValue { get; set; } = QVALUE_DEFAULT; // Event Quaternion value
    public Entity eValue { get; set; } = EVALUE_DEFAULT; // Event Entity value
    public BBox bbValue { get; set; } = BBVALUE_DEFAULT; // Event BBox value

    public Entity targetEntity { get; set; } // The entity we wish to target
    public EntityEvent targetEvent { get; set; } // The desired event

    public TriggerOn triggerOn { get; set; } // When should this trigger, trigger?
    public TriggerType triggerType { get; set; } // Which type of trigger is this?
    public TriggerBy triggerBy { get; set; } // What should this trigger, trigger from?
    public int triggerCount { get; set; } // The max amount of times this trigger should be triggered

    private int triggeredCount; // The amount of times this trigger has been triggered
    private bool hasTriggered; // Determines whether this trigger has already been triggered
    private Entity previousTriggerEntity; // The entity who last interacted with this trigger

    public TriggerBrush()
    {
    }

    public TriggerBrush( Vector3 position ) : base( position )
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
            if ( bbox.IntersectingWith( entity.GetBBox() ) )
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
        }
    }

    /// <summary>
    /// Things to do when this trigger has triggered.
    /// </summary>
    /// <param name="triggerEntity">The entity that triggered this trigger.</param>
    public bool OnTrigger( Entity triggerEntity )
    {
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
            default:
                break;
        }

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

        switch ( triggerBy )
        {
            case TriggerBy.All: // Any entity should be able to trigger this trigger
            default: // If there has been nothing set, anything should still trigger this
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

            case TriggerBy.NPCs: // Only NPCs should be able to trigger this
                if ( triggerEntity is NPCEntity )
                {
                    break;
                }
                else
                {
                    return false;
                }
        }

        // Get the entity we're supposed to affect
        // Entity foundTarget = EngineProgram.currentScene.FindEntity(targetEntity.GetID());

        Log.Info( $"TriggerBrush {this} has been triggered.\n" +
                          $"\tTarget: {targetEntity}\n" +
                          $"\tEvent: {targetEvent}\n" +
                          $"\tValues:\n" +
                          $"\t\tiValue: {iValue}\n" +
                          $"\t\tfValue: {fValue}\n" +
                          $"\t\tbValue: {( bValue > BVALUE_DEFAULT ? ( bValue == 0 ? "False" : "True" ) : "N/A" )}\n" +
                          $"\t\tvValue: {vValue}\n" +
                          $"\t\tqValue: {qValue}\n" +
                          $"\t\teValue: {( eValue == EVALUE_DEFAULT ? "N/A" : eValue )}\n" +
                          $"\t\tbbValue: {( bbValue )}\n" +
                          $"\tTrigger type: {triggerType}\n" +
                          $"\tTrigger by: {triggerBy}\n" +
                          $"\tTrigger on: {triggerOn}" );

        if ( iValue != IVALUE_DEFAULT ) // Int value event
        {
            targetEntity.OnEvent( targetEvent, iValue, this );
        }

        if ( fValue != FVALUE_DEFAULT ) // Float value event
        {
            targetEntity.OnEvent( targetEvent, fValue, this );
        }

        if ( bValue != BVALUE_DEFAULT ) // Bool value event
        {
            targetEntity.OnEvent( targetEvent, bValue == 1, this );
        }

        if ( vValue != VVALUE_DEFAULT ) // Vector3 value event
        {
            targetEntity.OnEvent( targetEvent, vValue, this );
        }

        if ( qValue != QVALUE_DEFAULT ) // Quaternion value event
        {
            targetEntity.OnEvent( targetEvent, qValue, this );
        }

        if ( eValue != EVALUE_DEFAULT ) // Entity value event
        {
            targetEntity.OnEvent( targetEvent, eValue, this );
        }

        if ( bbValue != BBVALUE_DEFAULT ) // BBox value event
        {
            targetEntity.OnEvent( targetEvent, bbValue, this );
        }

        // Regular event, not taking any special inputs
        targetEntity.OnEvent( targetEvent, this );

        // We've triggered this trigger successfully!
        triggeredCount++; // Triggered count goes up
        hasTriggered = true; // We have triggered
        previousTriggerEntity = triggerEntity; // Previous trigger entity is set to the one who last interacted with us

        // Return true, we've successfully been triggered!
        return true;
    }
}

public enum TriggerOn
{
    Trigger, // Either on enter or exit
    Enter, // Only when an entity has entered
    Exit // Only when an entity has left
}

public enum TriggerBy
{
    All, // Trigger by all things
    Player, // Trigger only by the player
    NPCs, // Trigger only by NPCs
}

public enum TriggerType
{
    Once, // Triggers once, then removes itself
    Count, // Triggers X amount of times before being removed
    Multiple // Can trigger multiple times
}