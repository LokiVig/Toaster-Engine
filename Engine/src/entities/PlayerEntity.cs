using System.Numerics;

using Toast.Engine.Resources;

namespace Toast.Engine.Entities;

public partial class PlayerEntity : Entity
{
    public override EntityType type => EntityType.Player; // This entity is of type Player
    public override float maxHealth => 100.0f;

    public PlayerEntity()
    {
        SetBBox(new BBox(new Vector3(-32, -32, 0), new Vector3(32, 32, 64)));
    }

    public PlayerEntity(Vector3 position) : base(position)
    {
        SetBBox(new BBox(new Vector3(-32, -32, 0), new Vector3(32, 32, 64)));
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
    }

    protected override void Update()
    {
        base.Update();

        // Handle movements
        ApplyVelocity();
    }
}