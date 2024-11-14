using DoomNET.WTF;

namespace DoomNET.Entities;

public class DamageableBrush : Entity
{
    public override EntityType type => EntityType.DamageableBrush; // This entity is of type DamageableBrush

    protected override void OnSpawn()
    {
        base.OnSpawn();

        health = DoomNET.currentFile?.FindEntity( GetID() ) == null ? 0.0f : DoomNET.currentFile.FindEntity( GetID() ).GetHealth();
    }
}