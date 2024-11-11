using DoomNET.WTF;
using DoomNET.Resources;

namespace DoomNET.Entities;

/// <summary>
/// A brush defining stuff to do when e.g. an entity or the player interacts with a brush
/// </summary>
public class TriggerBrush : Entity
{
    private Entity targetEnt; // The entity we wish to target
    private EntityEvent desiredEvent; // The desired event

    private TriggerOn triggerOn; // When should this trigger, trigger?
    private TriggerType triggerType; // Which type of trigger is this?

    private int triggerCount; // The amount of times this trigger has been triggered
    private int maxTriggerCount; // The max amount of thimes this trigger should be triggered

    private bool hasTriggered; // Determines whether or not this trigger has already been triggered

    private int iValue; // Event int value
    private float fValue; // Event float value
    private Vector3 vValue; // Event Vector3 value
    private BBox bValue; // Event BBox value

    public override EntityTypes type => EntityTypes.TriggerBrush; // This entity is of type TriggerBrush

    protected override void OnSpawn()
    {
        base.OnSpawn();

        GetBBox().OnIntersect += OnTrigger;

        // Reset standard values
        triggerCount = 0;
        hasTriggered = false;

        switch (triggerOn)
        {
            default: // !!! IMPLEMENT DIFFERENT TriggerOn EVENTS !!!
                break;
        }

        switch (triggerType)
        {
            case TriggerType.Once: // Only trigger once
                if (hasTriggered)
                {
                    GetBBox().OnIntersect -= OnTrigger;
                }
                break;

            case TriggerType.Count: // Trigger only a certain amount of times
                if (triggerCount >= maxTriggerCount)
                {
                    GetBBox().OnIntersect -= OnTrigger;
                }
                break;

            case TriggerType.Multiple: // Always trigger, there is no stopping it.
                break;
        }
    }

    /// <summary>
    /// Things to do when this trigger has triggered
    /// </summary>
    public void OnTrigger()
    {
        if (iValue != 0) // Int value event
        {
            targetEnt.OnEvent(desiredEvent, iValue);
        }
        else if (fValue != 0) // Float value event
        {
            targetEnt.OnEvent(desiredEvent, fValue);
        }
        else if (vValue != 0) // Vector3 value event
        {
            targetEnt.OnEvent(desiredEvent, vValue);
        }
        else if (bValue != null) // BBox value event
        {
            targetEnt.OnEvent(desiredEvent, bValue);
        }
        else // Regular event, not taking any inputs
        {
            targetEnt.OnEvent(desiredEvent);
        }

        // We've triggered this trigger, set the bool to true and increase the count
        triggerCount++;
        hasTriggered = true;
    }
}

public enum TriggerOn
{
    Trigger, // Either on enter or exit
    Enter, // Only when an ent has entered
    Exit // Only when an ent has left
}

public enum TriggerType
{
    Once, // Triggers once, then removes itself
    Count, // Triggers x amount of times before being removed
    Multiple // Can trigger multiple times
}