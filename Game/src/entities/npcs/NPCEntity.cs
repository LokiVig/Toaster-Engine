using DoomNET.Resources;

namespace DoomNET.Entities;

public class NPCEntity : Entity
{
	public override EntityType type => EntityType.NPC; // This entity is of type NPC

	public NPCEntity()
	{
	}

	public NPCEntity(Vector3 position) : base(position)
	{
	}
}