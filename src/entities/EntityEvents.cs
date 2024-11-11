namespace DoomNET.Entities;

public enum EntityEvent
{
    None, // Do nothing
    Kill, // Kill the entity
    Delete, // Delete the entity as a whole
    TakeDamage, // Make this entity take damage
    SetPosition, // Set this entity's position
    SetBBox, // Set this entity's BBox
}