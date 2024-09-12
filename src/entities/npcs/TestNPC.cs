namespace DoomNET.Entities;

public class TestNPC : Entity
{
    protected override EntityTypes type => EntityTypes.NPC; // This entity is of type NPC

    protected override float health => 100.0f;
}