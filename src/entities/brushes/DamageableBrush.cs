namespace DoomNET.Entities;

public class DamageableBrush : Entity
{
    public override EntityType type => EntityType.DamageableBrush; // This entity is of type DamageableBrush
    public override float health { get; set; } = 100.0f;
}