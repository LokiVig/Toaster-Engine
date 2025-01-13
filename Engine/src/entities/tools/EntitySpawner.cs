using System;

using Toast.Engine.Math;
using Toast.Engine.Resources;

namespace Toast.Engine.Entities.Tools;

public class EntitySpawner<T> : ToolEntity where T : Entity, new()
{
    public T entityToSpawn { get; } = new T();

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
        T ent = new T();

        // Make sure the desired entity is not a tool entity!
        if ( ent is ToolEntity )
        {
            EngineProgram.DoError( "Can't spawn tool entity!" );
            return null;
        }

        // The entity should spawn!
        ent.Spawn();
        // Set the entity's position to our position
        ent.SetPosition( position );
        // Generate the entity's ID
        ent.CreateID();

        // Add the newly spawned entity to the current scene
        EngineProgram.currentScene?.AddEntity( ent );

        Console.WriteLine( $"Spawned entity {ent}.\n" +
                          $"\tSource: {this}\n" +
                          $"\tPosition: {ent.GetPosition()}\n" +
                          $"\tRotation: {ent.GetRotation()}\n" +
                          $"\tBBox: {ent.GetBBox()}\n" );

        // And return the entity we just spawned
        return ent;
    }

    public override string ToString()
    {
        return $"EntitySpawner<{typeof( T )}> (\"{GetID()}\")";
    }
}

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
            EngineProgram.DoError( "Can't spawn tool entity!" );
            return null;
        }

        // The entity should spawn!
        ent.Spawn();
        // Set the entity's position to our position
        ent.SetPosition( position );
        // Generate the entity's ID
        ent.CreateID();

        // Add the newly spawned entity to the current scene
        EngineProgram.currentScene?.AddEntity( ent );

        Console.WriteLine( $"Spawned entity {ent}.\n" +
                          $"\tSource: {this}\n" +
                          $"\tPosition: {ent.GetPosition()}\n" +
                          $"\tRotation: {ent.GetRotation()}\n" +
                          $"\tBBox: {ent.GetBBox()}\n" );

        // And return the entity we just spawned
        return ent;
    }
}