using DoomNET.WTF;
using DoomNET.Resources;
using System;

namespace DoomNET.Entities;

/// <summary>
/// A brush defining stuff to do when e.g. an entity or the player interacts with a brush
/// </summary>
public class TriggerBrush : Entity
{
    public Entity targetEntity { get; set; } // The entity we wish to target
    public EntityEvent targetEvent { get; set; } // The desired event

    public TriggerOn triggerOn { get; set; } // When should this trigger, trigger?
    public TriggerType triggerType { get; set; } // Which type of trigger is this?
    public TriggerBy triggerBy { get; set; } // What should this trigger trigger from?

    private int triggeredCount; // The amount of times this trigger has been triggered
    public int triggerCount { get; set; } // The max amount of times this trigger should be triggered

    private bool hasTriggered; // Determines whether or not this trigger has already been triggered

    public int iValue { get; set; } // Event int value
    public float fValue { get; set; } // Event float value
    public Vector3 vValue { get; set; } // Event Vector3 value
    public BBox bValue { get; set; } // Event BBox value

    public override EntityTypes type => EntityTypes.TriggerBrush; // This entity is of type TriggerBrush

    protected override void OnSpawn()
    {
        base.OnSpawn();

        // Reset standard values
        triggerCount = 0;
        hasTriggered = false;

        switch (triggerOn)
        {
            default: // !!! IMPLEMENT DIFFERENT TriggerOn EVENTS !!!
                break;
        }
    }

    protected override void Update()
    {
        // Check if any entity is intersecting with us
        foreach (Entity entity in DoomNET.file?.entities)
        {
            if (bbox.IntersectingWith(entity.GetPosition()))
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
        switch (triggerType)
        {
            case TriggerType.Once: // Only trigger once
                if (hasTriggered)
                {
                    return;
                }
                break;

            case TriggerType.Count: // Trigger only a certain amount of times
                if (triggeredCount >= triggerCount)
                {
                    return;
                }
                break;

            case TriggerType.Multiple: // Always trigger, there is no stopping it.
            default: // Also the default
                break;
        }

        switch (triggerBy)
        {
            // Find a way to change which thing can trigger this trigger!
            case TriggerBy.All:
            case TriggerBy.Players:
            case TriggerBy.NPCs:
                break;
        }

        Console.WriteLine($"TriggerBrush \"{GetID()}\" has been triggered.\n" +
                                $"\tTarget: {targetEntity} (\"{targetEntity.GetID()}\")\n" +
                                $"\tEvent: {targetEvent}\n" +
                                $"\tValues:\n" +
                                    $"\t\tiValue: {iValue}\n" +
                                    $"\t\tfValue: {fValue}\n" +
                                    $"\t\tvValue: {vValue}\n" +
                                    $"\t\tbValue: {bValue}\n" +
                                $"\tTrigger type: {triggerType}\n" +
                                $"\tTrigger by: {triggerBy}\n" +
                                $"\tTrigger on: {triggerOn}\n");

        if (iValue != 0) // Int value event
        {
            targetEntity.OnEvent(targetEvent, iValue, this);
        }
        else if (fValue != 0) // Float value event
        {
            targetEntity.OnEvent(targetEvent, fValue, this);
        }
        else if (vValue != 0) // Vector3 value event
        {
            targetEntity.OnEvent(targetEvent, vValue, this);
        }
        else if (bValue != null) // BBox value event
        {
            targetEntity.OnEvent(targetEvent, bValue, this);
        }
        else // Regular event, not taking any inputs
        {
            targetEntity.OnEvent(targetEvent, this);
        }

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