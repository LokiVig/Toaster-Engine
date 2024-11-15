using System.Collections.Generic;

using DoomNET.Entities;


namespace DoomNET.Resources;

/// <summary>
/// A scene with a list of entities and brushes.
/// </summary>
public class Scene
{
    private List<Entity> entities = new();
    private List<Brush> brushes = new();

    public Scene() { }

    public Scene( List<Entity> entities, List<Brush> brushes )
    {
        this.entities = entities;
        this.brushes = brushes;
    }

    /// <summary>
    /// Load variables from the argument WTF file
    /// </summary>
    public static Scene LoadFromWTFFile( WTF file )
    {
        return new Scene( file.GetEntities(), file.GetBrushes() );
    }

    /// <summary>
    /// Remove the entity from the entities list
    /// </summary>
    /// <param name="entity">Desired entity to remove</param>
    public void RemoveEntity( Entity entity )
    {
        entities.Remove( entity );
    }

    /// <summary>
    /// Remove the entity from the entities list
    /// </summary>
    /// <param name="id">Desired entity to remove</param>
    public void RemoveEntity( string id )
    {
        foreach (Entity entity in entities)
        {
            if (entity.GetID() == id)
            {
                entities.Remove( entity );
            }
        }
    }

    /// <summary>
    /// Returns this file's list of entities
    /// </summary>
    public List<Entity> GetEntities()
    {
        return entities;
    }

    /// <summary>
    /// Gets the player entity from the map's list of entities
    /// </summary>
    public Player GetPlayer()
    {
        // Check every entity in our entities list
        foreach (Entity ent in entities)
        {
            if (ent is Player)
            {
                return ent as Player;
            }
        }

        // If there is no player
        // Should this really be possible?
        return null;
    }

    /// <summary>
    /// Returns this file's list of brushes
    /// </summary>
    public List<Brush> GetBrushes()
    {
        return brushes;
    }

    /// <summary>
    /// Finds an entity according to their ID
    /// </summary>
    /// <param name="id">A specific ID of an entity</param>
    /// <returns>The desired entity appropriate to the argument ID</returns>
    public T FindEntity<T>( string id ) where T : Entity
    {
        // Get the id of every entity
        for (int i = 0; i < entities.Count; i++)
        {
            // If entities[i]'s ID fits with the input ID, return that entity
            if (entities[ i ].GetID() == id && entities[ i ] is T entity)
            {
                return entity;
            }
        }

        // We didn't find an entity with that ID! Return null
        return null;
    }

    public Entity FindEntity( string id )
    {
        // Get the id of every entity
        for (int i = 0; i < entities.Count; i++)
        {
            // If entities[i]'s ID fits with the input ID, return that entity
            if (entities[ i ].GetID() == id)
            {
                return entities[i];
            }
        }

        // We didn't find an entity with that ID! Return null
        return null;
    }
}