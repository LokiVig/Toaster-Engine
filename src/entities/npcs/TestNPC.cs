using DoomNET.Resources;

namespace DoomNET.Entities;

public class TestNPC : Entity
{
    public override EntityType type => EntityType.NPC; // This entity is of type NPC
    
    protected float _health = 100.0f;

    public TestNPC() : base() { }

    public TestNPC( Vector3 position ) : base( position ) { }

    public TestNPC( Vector3 position, BBox bbox ) : base( position, bbox ) { }

    protected override void Update()
    {
        base.Update();

        // Handle movements
        HandleMovement();
    }
}