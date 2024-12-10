using System;

using DoomNET.Resources;

namespace DoomNET.Entities;

/// <summary>
/// A brush defining stuff to do when e.g. an entity or player enters it.
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
	private Entity previousTriggerEntity; // The entity who last interacted with this trigger

	public override EntityType type => EntityType.TriggerBrush; // This entity is of type TriggerBrush

	public TriggerBrush()
	{
	}

	public TriggerBrush(Vector3 position) : base(position)
	{
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();

		// Reset standard values
		triggerCount = 0;
		hasTriggered = false;

		// switch ( triggerOn )
		// {
		//     default: // !!! IMPLEMENT DIFFERENT TriggerOn EVENTS !!!
		//         break;
		// }
	}

	protected override void Update()
	{
		// Check if any entity is intersecting with us
		foreach (Entity entity in Game.currentFile?.entities!)
		{
			if (bbox.IntersectingWith(entity.GetBBox()))
			{
				// If we do intersect, but the trigger fails for some reason...
				if (!OnTrigger(entity))
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
	public bool OnTrigger(Entity triggerEntity)
	{
		switch (triggerType)
		{
			case TriggerType.Once: // Only trigger once
				if (hasTriggered)
				{
					return false;
				}
				break;

			case TriggerType.Count: // Trigger only a certain amount of times
				if (triggeredCount >= triggerCount)
				{
					return false;
				}
				break;

			case TriggerType.Multiple: // Always trigger, there is no stopping it.
			default: // Also the default

				// To make sure we don't continuosly intersect with the same entity every frame,
				// do a check to make sure the entity has left the trigger before re-triggering
				if (previousTriggerEntity == triggerEntity)
				{
					return false;
				}
				break;
		}

		switch (triggerBy)
		{
			case TriggerBy.All: // Any entity should be able to trigger this trigger
			default: // If there has been nothing set, anything should still trigger this
				break;

			case TriggerBy.Player: // Only players should be able to trigger this
				if (triggerEntity.type == EntityType.Player)
				{
					break;
				}
				else
				{
					return false;
				}

			case TriggerBy.NPCs: // Only NPCs should be able to trigger this
				if (triggerEntity.type == EntityType.NPC)
				{
					break;
				}
				else
				{
					return false;
				}
		}

		Console.WriteLine($"TriggerBrush {this} has been triggered.\n" +
			                  $"\tTarget: {Game.currentScene.FindEntity(targetEntity)}\n" +
			                  $"\tEvent: {targetEvent}\n" +
			                  $"\tValues:\n" +
				                  $"\t\tiValue: {iValue}\n" +
				                  $"\t\tfValue: {fValue}\n" +
				                  $"\t\tbValue: {(bValue > -1 ? bValue == 0 ? "False" : "True" : "N/A")}\n" +
				                  $"\t\tvValue: {vValue}\n" +
				                  $"\t\tqValue: {qValue}\n" +
				                  $"\t\teValue: {(eValue == null ? "N/A" : eValue)}\n" +
				                  $"\t\tbbValue: {(bbValue == null ? BBox.Zero : bbValue)}\n" +
			                  $"\tTrigger type: {triggerType}\n" +
			                  $"\tTrigger by: {triggerBy}\n" +
			                  $"\tTrigger on: {triggerOn}\n");

		if (iValue != 0) // Int value event
		{
			Game.currentScene.FindEntity(targetEntity).OnEvent(targetEvent, iValue, this);
		}

		if (fValue != 0) // Float value event
		{
			Game.currentScene.FindEntity(targetEntity).OnEvent(targetEvent, fValue, this);
		}

		if (bValue != -1) // Bool value event
		{
			Game.currentScene.FindEntity(targetEntity).OnEvent(targetEvent, bValue == 1, this);
		}

		if (vValue != 0) // Vector3 value event
		{
			Game.currentScene.FindEntity(targetEntity).OnEvent(targetEvent, vValue, this);
		}

		if (qValue != 0) // Quaternion value event
		{
			Game.currentScene.FindEntity(targetEntity).OnEvent(targetEvent, qValue, this);
		}

		if (eValue != null) // Entity value event
		{
			Game.currentScene.FindEntity(targetEntity).OnEvent(targetEvent, eValue, this);
		}

		if (bbValue != null) // BBox value event
		{
			Game.currentScene.FindEntity(targetEntity).OnEvent(targetEvent, bbValue, this);
		}

		// Regular event, not taking any special inputs
		Game.currentScene.FindEntity(targetEntity).OnEvent(targetEvent, this);

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