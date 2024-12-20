namespace Toast.Engine.Entities;

/// <summary>
/// This enum dictates the different types of entities there exist in the game.<br/>
/// Remember! No entity should ever be of type <see cref="EntityType.None"/>, otherwise you have a problem on your hands.
/// </summary>
public enum EntityType
{
	None, // If an entity is of this type, we have a big issue!

	// Misc. entities
	Player, // Self-describing
	Item, // All items should be of this type
	Prop, // All static, dynamic, physics, etc. prop should be of this type
	NPC, // All Non-Playable-Characters / AI should be of this type (NPC is used as a general term)
	Tool, // Everything that helps make it easier to make a map should be of this type
	Brush, // Everything that's a brush entity (Damageable brushes, Trigger brushes, etc.)
}