using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using Toast.Engine.Entities;

namespace Toast.Engine.Resources;

/// <summary>
/// A WTF, acronym for "Where's The Files?!", describes all the entities and brushes of a map
/// </summary>
public class WTF
{
    public List<Entity> entities { get; set; } = new(); // List of entities in this WTF file
    public List<Brush> brushes { get; set; } = new(); // All the brushes in this WTF file
    public string path { get; set; } // The directory of which this WTF file is saved at

    private static JsonSerializer serializer = new()
    {
        Formatting = Formatting.Indented,
    };

    /// <summary>
    /// WTF file creation with a specific path.
    /// </summary>
    /// <param name="path">The path (mainly filename) of the WTF file.</param>
    public WTF( string path )
    {
        if ( !path.Contains( "maps/" ) )
        {
            path = Path.Combine( "maps/", path );
        }

        this.path = path;
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
        foreach ( Brush brush in brushes.ToList() )
        {
            if ( brush.GetID() == id )
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
        foreach ( Entity entity in entities.ToList() )
        {
            if ( entity.GetID() == id )
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
        for ( int i = 0; i < entities.Count; i++ )
        {
            // If entities[i]'s ID fits with the input ID, and is of the type we requested, return that entity
            if ( entities[i].GetID() == id && entities[i] is T entity )
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
    /// Returns this file's list of entities
    /// </summary>
    public List<Entity> GetEntities()
    {
        return entities;
    }

    /// <summary>
    /// Gets the player entity from the map's list of entities
    /// </summary>
    public PlayerEntity GetPlayer()
    {
        // Check every entity in our entities list...
        foreach ( Entity ent in entities )
        {
            // If the entity is a player entity...
            if ( ent is PlayerEntity )
            {
                // Return that entity, for it must be our player!
                return ent as PlayerEntity;
            }
        }

        // If there is no player
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
        for ( int i = 0; i < entities.Count; i++ )
        {
            entities[i].SetID( $"entity {i}" );
        }

        for ( int i = 0; i < brushes.Count; i++ )
        {
            brushes[i].SetID( $"brush {i}" );
        }
    }

    /// <summary>
    /// Save this specific file.<br/>
    /// Optionally takes a specific <paramref name="directory"/> to save it to.
    /// </summary>
    /// <param name="directory">The optional directory to save the file to.</param>
    public void Save( string directory = null )
    {
        if ( !string.IsNullOrEmpty( directory ) )
        {
            if ( !directory.Contains( "maps/" ) )
            {
                directory = Path.Combine( "maps/", directory );
            }
        }

        SaveFile( directory ?? path, this );
    }

    /// <summary>
    /// Find a WTF by a specific path.
    /// </summary>
    /// <param name="directory">The specified path to the WTF.</param>
    /// <param name="outFile">The resulting file we've loaded.</param>
    public static void LoadFile( string directory, out WTF outFile )
    {
        // Couldn't find file from the input path! Throw an exception
        if ( !File.Exists( directory ) )
        {
            Log.Error<FileNotFoundException>( $"Couldn't find WTF file at \"{directory}\"." );
        }

        // Get the json reader from our directory
        using ( StreamReader sr = File.OpenText( directory ) )
        {
            using ( JsonReader reader = new JsonTextReader( sr ) )
            {
                // Deserialize the file from our reader
                outFile = serializer.Deserialize<WTF>( reader );
            }
        }
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
    /// <param name="inFile">The desired file to save</param>
    public static void SaveFile( string path, WTF inFile )
    {
        WTF file = null; // A local variable for storing the file

        // If we already have a file open, set the path to the current file
        if ( EngineManager.currentFile != null && !string.IsNullOrEmpty( EngineManager.currentFile.path ) )
        {
            file = EngineManager.currentFile;
            path = file.path;
        }
        else if ( inFile != null ) // If the input file wasn't null, set it accordingly
        {
            file = inFile;
        }
        else // We couldn't find a file to save, error!
        {
            Log.Error<NullReferenceException>( "Error saving file, inFile == null && EngineProgram.currentFile == null!" );
        }

        // Call the file's OnSave function
        file.OnSave();

        // Set the file's filepath if it wasn't already sourced from the file itself
        if ( file.path != path )
        {
            file.path = path;
        }

        // Write the WTF file to the path
        using ( StreamWriter sw = new StreamWriter( path ) )
        {
            using ( JsonWriter writer = new JsonTextWriter( sw ) )
            {
                serializer.Serialize( writer, file );
            }
        }
    }
}