using System.Collections.Generic;

using Toast.Engine.Entities;

namespace Toast.Engine.Resources;

/// <summary>
/// A scene with a list of entities and brushes.
/// </summary>
public class Scene
{
    private List<Entity> entities;
    private List<Brush> brushes;

    public Scene( List<Entity> entities, List<Brush> brushes )
    {
        this.entities = entities;
        this.brushes = brushes;
    }

    /// <summary>
    /// Load variables from the argument WTF file
    /// </summary>
    public static Scene LoadFromFile( WTF file )
    {
        EngineManager.currentFile = file; // Set the engine's current file
        return new Scene( file.GetEntities(), file.GetBrushes() ); // Return a new scene
    }

    /// <summary>
    /// Load variables from a WTF file loaded from the argument path
    /// </summary>
    public static Scene LoadFromFile( string path )
    {
        WTF file = WTF.LoadFile( path ); // Load the WTF file
        EngineManager.currentFile = file; // Set the engine's current file appropriately
        return new Scene( file.GetEntities(), file.GetBrushes() ); // Return a new scene
    }

    /// <summary>
    /// Remove an entity from the entities list.
    /// </summary>
    /// <param name="entity">Desired entity to remove.</param>
    public void RemoveEntity( Entity entity )
    {
        entities.Remove( entity );
    }

    /// <summary>
    /// Remove an entity from the entities list.
    /// </summary>
    /// <param name="id">The ID of the entity we wish to remove.</param>
    public void RemoveEntity( string id )
    {
        // Remove the entity we find from the specified ID argument
        entities.Remove( FindEntity( id ) );
    }

    /// <summary>
    /// Returns this scene's list of entities.
    /// </summary>
    public List<Entity> GetEntities()
    {
        return entities;
    }

    /// <summary>
    /// Gets the player entity from the scene's list of entities.
    /// </summary>
    public PlayerEntity GetPlayer()
    {
        // Check every entity in our entities list
        foreach ( Entity ent in entities )
        {
            if ( ent is PlayerEntity )
            {
                return ent as PlayerEntity;
            }
        }

        // If there is no player
        // Should this really be possible?
        return null;
    }

    /// <summary>
    /// Returns this scenes list of brushes
    /// </summary>
    public List<Brush> GetBrushes()
    {
        return brushes;
    }

    /// <summary>
    /// Add an entity to the scene.
    /// </summary>
    /// <param name="ent">A specific entity to add to this scene.</param>
    /// <typeparam name="T">Anything that is an entity should be added to the scene.</typeparam>
    public void AddEntity<T>( T ent ) where T : Entity
    {
        entities.Add( ent );
    }

    /// <summary>
    /// Finds an entity according to their ID
    /// </summary>
    /// <param name="id">A specific ID of an entity</param>
    /// <returns>The desired entity appropriate to the argument ID</returns>
    public T FindEntity<T>( string id ) where T : Entity
    {
        // Get the id of every entity
        for ( int i = 0; i < entities.Count; i++ )
        {
            // If entities[i]'s ID fits with the input ID, return that entity
            if ( entities[i].GetID() == id && entities[i] is T entity )
            {
                return entity;
            }
        }

        // We didn't find an entity with that ID! Return null
        return null;
    }

    /// <inheritdoc cref="FindEntity{T}(string)"/>
    public Entity FindEntity( string id )
    {
        // Get the id of every entity
        for ( int i = 0; i < entities.Count; i++ )
        {
            // If entities[i]'s ID fits with the input ID, return that entity
            if ( entities[i].GetID() == id )
            {
                return entities[i];
            }
        }

        // We didn't find an entity with that ID! Return null
        return null;
    }

    /// <summary>
    /// Tries to find an entity according to their ID.
    /// </summary>
    /// <typeparam name="T">The type of the entity we want.</typeparam>
    /// <param name="id">The ID of the entity we're trying to find.</param>
    /// <param name="entity">The resulting entity we'll, hopefully, find. Will be <see langword="null"/> if none was found.</param>
    /// <returns><see langword="true"/> if entity was found, <see langword="false"/> otherwise.</returns>
    public bool TryFindEntity<T>( string id, out T entity ) where T : Entity
    {
        return ( entity = FindEntity<T>( id ) ) != null;
    }

    /// <inheritdoc cref="TryFindEntity{T}(string, out T)"/>
    public bool TryFindEntity( string id, out Entity entity )
    {
        return ( entity = FindEntity( id ) ) != null;
    }
}