using DoomNET.Resources;

namespace DoomNET.Entities;

public class BrushEntity : Entity
{
	public override EntityType type => EntityType.Brush;

	public BrushEntity()
	{
	}

	public BrushEntity(Vector3 position) : base(position)
	{
	}
}