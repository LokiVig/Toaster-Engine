﻿using System;

using DoomNET.Resources;

namespace DoomNET.Entities;

/// <summary>
/// A brush defining stuff to do when e.g. an entity or the player interacts with a brush
/// </summary>
public class TriggerBrush : Entity
{
    public int iValue { get; set; } // Event int value
    public float fValue { get; set; } // Event float value
    public int bValue { get; set; } = -1; // Event bool value (-1 = none, 0 = false, 1 = true)
    public Vector3 vValue { get; set; } // Event Vector3 value
    public Quaternion qValue { get; set; } // Event Quaternion value
    public Entity eValue { get; set; } // Event Entity value
    public BBox bbValue { get; set; } // Event BBox value

    public string targetEntity { get; set; } // The entity we wish to target
    public EntityEvent targetEvent { get; set; } // The desired event

    public TriggerOn triggerOn { get; set; } // When should this trigger, trigger?
    public TriggerType triggerType { get; set; } // Which type of trigger is this?
    public TriggerBy triggerBy { get; set; } // What should this trigger, trigger from?
    public int triggerCount { get; set; } // The max amount of times this trigger should be triggered

    private int triggeredCount; // The amount of times this trigger has been triggered
    private bool hasTriggered; // Determines whether this trigger has already been triggered

    public override EntityType type => EntityType.TriggerBrush; // This entity is of type TriggerBrush

    protected override void OnSpawn()
    {
        base.OnSpawn();

        // Reset standard values
        triggerCount = 0;
        hasTriggered = false;

        switch ( triggerOn )
        {
            default: // !!! IMPLEMENT DIFFERENT TriggerOn EVENTS !!!
                break;
        }
    }

    protected override void Update()
    {
        // Check if any entity is intersecting with us
        foreach ( Entity entity in DoomNET.currentFile?.entities )
        {
            if ( bbox.IntersectingWith( entity.GetBBox() ) )
            {
                // If they are, we bbox.OnIntersect triggers!
                OnTrigger();
                break;
            }
        }
    }

    /// <summary>
    /// Things to do when this trigger has triggered
    /// </summary>
    public void OnTrigger()
    {
        switch ( triggerType )
        {
            case TriggerType.Once: // Only trigger once
                if ( hasTriggered )
                {
                    return;
                }
                break;

            case TriggerType.Count: // Trigger only a certain amount of times
                if ( triggeredCount >= triggerCount )
                {
                    return;
                }
                break;

            case TriggerType.Multiple: // Always trigger, there is no stopping it.
            default: // Also the default
                break;
        }

        switch ( triggerBy )
        {
            // Find a way to change which thing can trigger this trigger!
            case TriggerBy.All:
            case TriggerBy.Players:
            case TriggerBy.NPCs:
                break;
        }

        Console.WriteLine( $"TriggerBrush \"{GetID()}\" has been triggered.\n" +
                                $"\tTarget: {DoomNET.currentScene.FindEntity( targetEntity )} (\"{DoomNET.currentScene.FindEntity( targetEntity ).GetID()}\")\n" +
                                $"\tEvent: {targetEvent}\n" +
                                $"\tValues:\n" +
                                    $"\t\tiValue: {iValue}\n" +
                                    $"\t\tfValue: {fValue}\n" +
                                    $"\t\tbValue: {( bValue > -1 ? ( bValue == 0 ? "False" : "True" ) : "" )}\n" +
                                    $"\t\tvValue: {vValue}\n" +
                                    $"\t\tqValue: {qValue}\n" +
                                    $"\t\teValue: {(eValue == null ? "N/A" : $"{eValue} (\"{eValue.GetID()}\")")}\n" +
                                    $"\t\tbbValue: {bbValue}\n" +
                                $"\tTrigger type: {triggerType}\n" +
                                $"\tTrigger by: {triggerBy}\n" +
                                $"\tTrigger on: {triggerOn}\n" );

        if ( iValue != 0 ) // Int value event
        {
            DoomNET.currentScene.FindEntity(targetEntity).OnEvent( targetEvent, iValue, this );
        }

        if ( fValue != 0 ) // Float value event
        {
            DoomNET.currentScene.FindEntity( targetEntity ).OnEvent( targetEvent, fValue, this );
        }

        if ( bValue != -1 ) // Bool value event
        {
            DoomNET.currentScene.FindEntity( targetEntity ).OnEvent( targetEvent, bValue == 1, this );
        }

        if ( vValue != 0 ) // Vector3 value event
        {
            DoomNET.currentScene.FindEntity( targetEntity ).OnEvent( targetEvent, vValue, this );
        }

        if ( qValue != 0 ) // Quaternion value event
        {
            DoomNET.currentScene.FindEntity( targetEntity ).OnEvent( targetEvent, qValue, this );
        }

        if ( bbValue != null ) // BBox value event
        {
            DoomNET.currentScene.FindEntity( targetEntity ).OnEvent( targetEvent, bbValue, this );
        }

        // Regular event, not taking any special inputs
        DoomNET.currentScene.FindEntity( targetEntity ).OnEvent( targetEvent, this );

        // We've triggered this trigger, set the bool to true and increase the count
        triggeredCount++;
        hasTriggered = true;
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
    Players, // Trigger only by players
    NPCs, // Trigger only by NPCs
}

public enum TriggerType
{
    Once, // Triggers once, then removes itself
    Count, // Triggers X amount of times before being removed
    Multiple // Can trigger multiple times
}