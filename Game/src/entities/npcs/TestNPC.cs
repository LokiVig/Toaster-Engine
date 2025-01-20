using System.Numerics;

using Toast.Engine.Entities;
using Toast.Engine.Resources;

namespace Toast.Game.Entities.NPC;

public class TestNPC : NPCEntity
{
	public override float maxHealth { get; set; } = 100.0f;

	protected override Type[] hateList => [];
	protected override Type[] adoreList => [typeof(Player)];

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
		ApplyVelocity();
	}

	protected override void OnDamage()
	{
		base.OnDamage();
		
		// We should target the thing that last attacked us!
		SetTarget(lastAttacker);
	}
}