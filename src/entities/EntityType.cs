namespace DoomNET.Entities;

public enum EntityType
{
    None,
    
    // Misc. entities
    Player,
    Item,
    Prop,
    NPC,

    // Brush types
    TriggerBrush,
    MoveableBrush,
    DamageableBrush,
}