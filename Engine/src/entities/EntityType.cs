namespace Toast.Engine.Entities;

/// <summary>
/// This enum dictates the different types of entities there exist in the game.<br/>
/// Remember! No entity should ever be of type <see cref="EntityType.None"/>, otherwise you have a problem on your hands.
/// </summary>
public enum EntityType
{
	None, // If an entity is of this type, we have a big issue!
	
	// All entities should be one of these types
	Player, // Self-describing
	Item, // All items should be of this type
	Prop, // All static, dynamic, physics, etc. props should be of this type
	NPC, // All Non-Playable-Characters / AI should be of this type (NPC is used as a general term)
	Tool, // All tool entities that are used when making a map should be of this type (sound entities, entity spawners, etc.)
	Brush, // Everything that's a brush entity should be of this type (damageable brushes, trigger brushes, etc.)
}