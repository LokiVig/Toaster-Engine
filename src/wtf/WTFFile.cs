﻿using System.Linq;
using System.Collections.Generic;

using DoomNET.Entities;
using DoomNET.Resources;

namespace DoomNET.WTF;

/// <summary>
/// WTF file, acronym for Where's The Files, describes all of the entities and brushes of a map
/// </summary>
public class WTFFile
{
    public List<Entity> entities { get; set; } = new(); // List of entities in this WTF file
    public List<Brush> brushes { get; set; } = new(); // All of the brushes in this WTF file

    public string directory { get; set; } // The directory of which this WTF file is saved at

    public WTFFile() { }

    public WTFFile( string directory )
    {
        this.directory = directory;
    }

    /// <summary>
    /// Add an entity to the entities list
    /// </summary>
    /// <param name="entity">Input entity</param>
    public void AddEntity( Entity entity )
    {
        entities.Add( entity );
    }

    /// <summary>
    /// Add a brush to the list of this WTF file
    /// </summary>
    /// <param name="brush">The brush that we're to add to this file</param>
    public void AddBrush( Brush brush )
    {
        // Add the brush to the list
        brushes.Add( brush );
    }

    /// <summary>
    /// Remove a brush from the list of this WTF file
    /// </summary>
    /// <param name="brush">The brush that we're to remove from this file</param>
    public void RemoveBrush( Brush brush )
    {
        // Remove the brush from the list
        brushes.Remove( brush );
    }

    /// <summary>
    /// Remove a brush from the list of this WTF using its ID
    /// </summary>
    /// <param name="id">The ID of the brush we're to remove from this file</param>
    public void RemoveBrush( string id )
    {
        foreach (Brush brush in brushes)
        {
            if (brush.GetID() == id)
            {
                brushes.Remove( brush );
            }
        }
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
    /// Called when saving the file
    /// </summary>
    public void OnSave()
    {
        // For every entity, set their ID to i, 0 should always be the player
        for (int i = 0; i < entities.Count; i++)
        {
            entities[ i ].SetID( $"entity {i}" );
        }

        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[ i ].SetID( $"brush {i}" );
        }
    }

    /// <summary>
    /// Finds an entity according to their ID
    /// </summary>
    /// <param name="id">A specific ID of an entity</param>
    /// <returns>The desired entity appropriate to the argument ID</returns>
    public Entity FindEntity( string id )
    {
        // Get the id of every entity
        for (int i = 0; i < entities.Count; i++)
        {
            // If entities[i]'s ID fits with the input ID, return that entity
            if (entities[ i ].GetID() == id)
            {
                return entities[ i ];
            }
        }

        // We didn't find an entity with that ID! Return null
        return null;
    }
}