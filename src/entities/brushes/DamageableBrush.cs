using DoomNET.Resources;

namespace DoomNET.Entities;

public class DamageableBrush : Entity
{
    public override EntityType type => EntityType.DamageableBrush; // This entity is of type DamageableBrush

    protected float _health = 100.0f;
}