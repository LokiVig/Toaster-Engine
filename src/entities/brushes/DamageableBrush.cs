using DoomNET.WTF;

namespace DoomNET.Entities;

public class DamageableBrush : Entity
{
    public override EntityTypes type => EntityTypes.DamageableBrush; // This entity is of type DamageableBrush

    protected override void OnSpawn()
    {
        base.OnSpawn();

        health = DoomNET.file?.FindEntity(GetID()) == null ? 0.0f : DoomNET.file.FindEntity(GetID()).GetHealth();
    }
}