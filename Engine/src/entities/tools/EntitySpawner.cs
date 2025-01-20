using System.Numerics;

using Toast.Engine.Resources;

namespace Toast.Engine.Entities.Tools;

/// <summary>
/// Tool entity which allows the spawning of another entity in the current scene.
/// </summary>
/// <typeparam name="T">The <see cref="Entity"/> we wish to spawn.</typeparam>
public class EntitySpawner<T> : ToolEntity where T : Entity, new()
{
    public T entityToSpawn { get; private set; }

    public EntitySpawner()
    {
        SetBBox( new BBox( new Vector3( -8 ), new Vector3( 8 ) ) );
    }

    public EntitySpawner( Vector3 position ) : base( position )
    {
        SetBBox( new BBox( new Vector3( -8 ), new Vector3( 8 ) ) );
    }

    /// <summary>
    /// Spawn an entity, based on the given argument.
    /// </summary>
    /// <returns>The recently spawned entity.</returns>
    public T SpawnEntity()
    {
        entityToSpawn = new T();

        // Make sure the desired entity is not a tool entity!
        if ( entityToSpawn is ToolEntity )
        {
            Log.Error( "Can't spawn tool entity!" );
            return null;
        }

        // The entity should spawn!
        entityToSpawn.Spawn();
        // Set the entity's position to our position
        entityToSpawn.SetPosition( position );

        // Add the newly spawned entity to the current scene
        EngineManager.currentScene?.AddEntity( entityToSpawn );

        // Generate the entity's ID
        entityToSpawn.GenerateID();

        Log.Info( $"Spawned entity {entityToSpawn}.\n" +
                          $"\tSource: {this}\n" +
                          $"\tPosition: {entityToSpawn.GetPosition()}\n" +
                          $"\tRotation: {entityToSpawn.GetRotation()}\n" +
                          $"\tBBox: {entityToSpawn.GetBBox()}" );

        // And return the entity we just spawned
        return entityToSpawn;
    }

    public override string ToString()
    {
        return $"EntitySpawner<{typeof( T )}> (\"{GetID()}\")";
    }
}

/// <summary>
/// <inheritdoc cref="EntitySpawner{T}"/>
/// </summary>
public class EntitySpawner : ToolEntity
{
    public EntitySpawner()
    {
        SetBBox( new BBox( new Vector3( -8 ), new Vector3( 8 ) ) );
    }

    public EntitySpawner( Vector3 position ) : base( position )
    {
        SetBBox( new BBox( new Vector3( -8 ), new Vector3( 8 ) ) );
    }

    /// <summary>
    /// Spawn an entity, based on the given argument.
    /// </summary>
    /// <param name="ent">The entity we wish to spawn.</param>
    /// <returns>The recently spawned entity.</returns>
    public Entity SpawnEntity( Entity ent )
    {
        // Make sure the desired entity is not a tool entity!
        if ( ent is ToolEntity )
        {
            Log.Error( "Can't spawn tool entity!" );
            return null;
        }

        // The entity should spawn!
        ent.Spawn();
        // Set the entity's position to our position
        ent.SetPosition( position );
        // Generate the entity's ID
        ent.GenerateID();

        // Add the newly spawned entity to the current scene
        EngineManager.currentScene?.AddEntity( ent );

        Log.Info( $"Spawned entity {ent}.\n" +
                          $"\tSource: {this}\n" +
                          $"\tPosition: {ent.GetPosition()}\n" +
                          $"\tRotation: {ent.GetRotation()}\n" +
                          $"\tBBox: {ent.GetBBox()}" );

        // And return the entity we just spawned
        return ent;
    }
}