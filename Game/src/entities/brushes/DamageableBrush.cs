namespace DoomNET.Entities;

public class DamageableBrush : BrushEntity
{
    public override EntityType type => EntityType.DamageableBrush; // This entity is of type DamageableBrush
    public override float health { get; set; } = 100.0f;
}