using DoomNET.Resources;

namespace DoomNET.Entities;

public class TestNPC : Entity
{
    public override EntityTypes type => EntityTypes.NPC; // This entity is of type NPC

    public override float health => 100.0f;

    public TestNPC() : base() { }

    public TestNPC(Vector3 position) : base(position) { }

    public TestNPC(Vector3 position, BBox bbox) : base(position, bbox) { }
}