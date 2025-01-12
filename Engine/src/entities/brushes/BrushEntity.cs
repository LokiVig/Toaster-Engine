using Toast.Engine.Math;

namespace Toast.Engine.Entities;

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