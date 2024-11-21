using System;

using DoomNET.Resources;

namespace DoomNET.Entities;

public class EntitySpawner : Entity
{
    private Entity desiredEntity;

    public EntitySpawner() : base()
    {
        SetBBox(new BBox(new Vector3(-8, -8, -8), new Vector3(8, 8, 8)));
    }

    public EntitySpawner(Vector3 position) : base(position)
    {
        SetBBox(new BBox(new Vector3(-8, -8, -8), new Vector3(8, 8, 8)));
    }
    
    public EntitySpawner(Entity desiredEntity)
    {
        this.desiredEntity = desiredEntity;
        SetBBox(new BBox(new Vector3(-8, -8, -8), new Vector3(8, 8, 8)));
    }

    public EntitySpawner(Vector3 position, Entity desiredEntity) : base(position)
    {
        this.desiredEntity = desiredEntity;
        SetBBox(new BBox(new Vector3(-8, -8, -8), new Vector3(8, 8, 8)));
    }

    /// <summary>
    /// Spawn an entity, based on our local <see cref="desiredEntity"/> value.
    /// </summary>
    /// <returns>The recently spawned entity.</returns>
    public T SpawnEntity<T>() where T : Entity
    {
        return (T)SpawnEntity(desiredEntity);
    }

    /// <summary>
    /// Spawn an entity, based on the given argument.
    /// </summary>
    /// <param name="ent">The entity we wish to spawn.</param>
    /// <returns>The recently spawned entity.</returns>
    public T SpawnEntity<T>(T ent) where T : Entity
    {
        // Make sure the desired entity is not another spawner!
        if (ent is EntitySpawner)
        {
            Console.WriteLine("Can't spawn another spawner!\n");
            return null;
        }

        // The entity should spawn!
        ent.Spawn();
        // Set the entity's position to our position
        ent.SetPosition(position);

        // Add the newly spawned entity to the current scene
        DoomNET.currentScene?.AddEntity(ent);

        // And return the entity we just spawned
        return ent;
    }
}