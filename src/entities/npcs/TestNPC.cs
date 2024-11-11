namespace DoomNET.Entities;

public class TestNPC : Entity
{
    public override EntityTypes type => EntityTypes.NPC; // This entity is of type NPC

    public override float health => 100.0f;
}