using DoomNET.Resources;

namespace DoomNET.Entities;

public class TestNPC : Entity
{
    public override EntityType type => EntityType.NPC; // This entity is of type NPC
    
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