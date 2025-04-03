using System.Numerics;

using Toast.Engine.Resources;

namespace Toast.Engine.Entities.Tools;

/// <summary>
/// Tool entity which allows the spawning of another entity in the current scene.
/// </summary>
public class EntitySpawner : ToolEntity
{
    public Entity toSpawn;

    public EntitySpawner() : base()
    {
        SetBoundingBox( BoundingBox.SmallTool );
    }

    public EntitySpawner( Entity toSpawn ) : this()
    {
        this.toSpawn = toSpawn;
    }

    public EntitySpawner( Vector3 position ) : base( position )
    {
        SetBoundingBox( BoundingBox.SmallTool );
    }

    public EntitySpawner( Entity toSpawn, Vector3 position ) : this( position )
    {
        this.toSpawn = toSpawn;
    }

    /// <summary>
    /// Spawns an entity specified by a parameter.
    /// </summary>
    /// <returns>The recently spawned entity.</returns>
    public Entity SpawnEntity()
    {
        // Make sure the entity is not a tool entity!
        if ( toSpawn is ToolEntity )
        {
            Log.Error( "Can't spawn a tool entity" );
            return null;
        }

        // Add the entity to the scene
        EngineManager.currentScene?.AddEntity( toSpawn );

        // The entity should spawn
        toSpawn.Spawn();

        // Set the entity's parent to be us
        toSpawn.SetParent( this );

        // Set the entity's position to our position
        toSpawn.SetTransform( GetTransform() );

        // Log the successfully spawned entity
        Log.Info( $"Spawned entity {toSpawn}." );
        Log.Info( $"\tSource: {this}" );
        Log.Info( $"\tPosition: {toSpawn.GetPosition()}" );
        Log.Info( $"\tRotation: {toSpawn.GetRotation()}" );
        Log.Info( $"\tBounding box: {toSpawn.GetBoundingBox()}" );

        // Return the spawned entity
        return toSpawn;
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
            Log.Error( "Can't spawn a tool entity!" );
            return null;
        }

        // The entity should spawn!
        ent.Spawn();
        // Set the entity's position to our position
        ent.SetPosition( transform.worldPosition );
        // Generate the entity's ID
        ent.GenerateID();

        // Add the newly spawned entity to the current scene
        EngineManager.currentScene?.AddEntity( ent );

        Log.Info( $"Spawned entity {ent}." );
        Log.Info( $"\tSource: {this}" );
        Log.Info( $"\tPosition: {ent.GetPosition()}" );
        Log.Info( $"\tRotation: {ent.GetRotation()}" );
        Log.Info( $"\tBounding box: {ent.GetBoundingBox()}" );

        // And return the entity we just spawned
        return ent;
    }
}