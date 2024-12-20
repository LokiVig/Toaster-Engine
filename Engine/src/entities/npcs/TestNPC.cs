using Toast.Engine.Resources;

namespace Toast.Engine.Entities;

public class TestNPC : NPCEntity
{
	public override float health { get; set; } = 100.0f;

	public TestNPC()
	{
		SetBBox(new BBox(new Vector3(-32, -32, 0), new Vector3(32, 32, 64)));
	}

	public TestNPC(Vector3 position) : base(position)
	{
		SetBBox(new BBox(new Vector3(-32, -32, 0), new Vector3(32, 32, 64)));
	}

	protected override void Update()
	{
		base.Update();

		// Handle movements
		HandleMovement();
	}
}