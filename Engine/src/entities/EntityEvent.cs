namespace Toast.Engine.Entities;

public enum EntityEvent
{
    None, // Do nothing
    Kill, // Kill the entity
    Delete, // Delete the entity as a whole
    SetHealth, // Set the health of the entity
    TakeDamage, // Make this entity take damage
    SetPosition, // Set this entity's position (Vector3)
    SetBoundingBox, // Set this entity's BBox
    SpawnEntity, // Tell an EntitySpawner to spawn their entity
    SetRotation, // Set this entity's rotation (Quaternion)
    PlaySound, // Plays an AudioEntity's audio
    StopSound, // Stops an AudioEntity's audio
}