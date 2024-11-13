namespace DoomNET.Entities;

public enum EntityEvent
{
    None, // Do nothing
    Kill, // Kill the entity
    Delete, // Delete the entity as a whole
    SetHealth, // Set the health of the entity
    TakeDamage, // Make this entity take damage
    SetPosition, // Set this entity's position (Vector3)
    SetBBox, // Set this entity's BBox
    SetRotation, // Set this entity's rotation (Quaternion)
}