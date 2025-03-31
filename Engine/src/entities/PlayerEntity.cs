using System.Numerics;

using Toast.Engine.Resources;

namespace Toast.Engine.Entities;

public partial class PlayerEntity : Entity
{
    public override EntityType type => EntityType.Player; // This entity is of type Player
    public override float maxHealth => 100.0f;

    public PlayerEntity() : base()
    {
        SetBoundingBox( BoundingBox.LargeEntity );
    }

    public PlayerEntity( Vector3 position ) : base( position )
    {
        SetBoundingBox( BoundingBox.LargeEntity );
    }

    protected override void Update()
    {
        base.Update();

        // Handle movements
        ApplyVelocity();
    }
}