using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

using DoomNET.Entities;

namespace DoomNET.Resources;

/// <summary>
/// A WTF, acronym for World Tracking File, describes all of the entities and brushes of a map
/// </summary>
public class WTF
{
    public List<Entity> entities { get; set; } = new(); // List of entities in this WTF file
    public List<Brush> brushes { get; set; } = new(); // All of the brushes in this WTF file

    public string directory { get; set; } // The directory of which this WTF file is saved at

    public WTF() { }

    public WTF( string directory )
    {
        this.directory = directory;
    }

    /// <summary>
    /// Add an entity to the entities list
    /// </summary>
    /// <param name="entity">Input entity</param>
    public void AddEntity<T>( T entity ) where T : Entity
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
    /// Finds an entity according to their ID and argument type
    /// </summary>
    /// <param name="id">A specific ID of an entity</param>
    /// <returns>The desired entity appropriate to the argument ID and type</returns>
    public T FindEntity<T>( string id ) where T : Entity
    {
        // Get the id of every entity
        for (int i = 0; i < entities.Count; i++)
        {
            // If entities[i]'s ID fits with the input ID, and is of the type we requested, return that entity
            if (entities[ i ].GetID() == id && entities[i] is T entity)
            {
                return entity;
            }
        }

        // We didn't find an entity with that ID! Return null
        return null;
    }

    /// <summary>
    /// Finds an entity according to their ID
    /// </summary>
    /// <param name="id">A specific ID of an entity</param>
    /// <returns>The desired entity appropriate to the argument ID</returns>
    public Entity FindEntity( string id )
    {
        // Get the ID of every entity
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
        // For every entity, set their ID appropriately
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
     /// Find a WTF by a specific path
     /// </summary>
     /// <param name="directory">The specified path to the WTF</param>
     /// <param name="outFile">An output file, for external uses</param>
     /// <exception cref="FileNotFoundException"></exception>
    public static void LoadFile( string directory, out WTF outFile )
    {
        // Couldn't find file from the input path! Throw an exception
        if ( !File.Exists( directory ) )
        {
            throw new FileNotFoundException( $"Couldn't find WTF file at \"{directory}\"." );
        }

        // Deserialize the file through JSON
        outFile = JsonSerializer.Deserialize<WTF>( File.OpenRead( directory ), Program.serializerOptions );
    }

    /// <summary>
    /// Find a WTF by a specific path
    /// </summary>
    /// <param name="directory">The specified path to the WTF</param>
    /// <returns>The desired file as a variable</returns>
    public static WTF LoadFile( string directory )
    {
        LoadFile( directory, out WTF file );
        return file;
    }

    /// <summary>
    /// Save a WTF to a specified path
    /// </summary>
    /// <param name="path">The path of the WTF</param>
    public static void SaveFile( string path, WTF inFile )
    {
        // A local variable for storing the file
        WTF file;

        // Ensure that the "maps/" directory actually exists
        if (!Directory.Exists( "maps/" ))
        {
            Directory.CreateDirectory( "maps/ ");
        }

        // If we already have a file open, set the path to the current file
        if ( DoomNET.currentFile != null && !string.IsNullOrEmpty( DoomNET.currentFile.directory ) )
        {
            file = DoomNET.currentFile;
            path = file.directory;
        }
        else if ( inFile != null ) // If the input file wasn't null, set it accordingly
        {
            file = inFile;
        }
        else // We couldn't find a file to save, error!
        {
            throw new NullReferenceException( "Error saving file, SaveFile().inFile == null & DoomNET.currentFile == null!" );
        }

        // Call the file's OnSave function
        file.OnSave();

        // Set the file's filepath if it wasn't already sourced from the file itself
        if ( file.directory != path )
        {
            file.directory = path;
        }

        // Write the WTF file to the path
        File.WriteAllText( path, JsonSerializer.Serialize( file, Program.serializerOptions ) );
    }
}