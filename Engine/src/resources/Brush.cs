using System.Numerics;

using Newtonsoft.Json;

using Toast.Engine.Entities;
using Toast.Engine.Rendering;

namespace Toast.Engine.Resources;

/// <summary>
/// A brush, a collection of 6 solid faces, stretching from one point to another, defined with a bounding box
/// </summary>
public struct Brush
{
    public string id; // The id of this brush, can be set by the mapper
    public BoundingBox bbox = BoundingBox.One; // The extents of this brush

    [JsonIgnore] public Vertex[] vertices; // The vertices of this brush
    [JsonIgnore] public uint[] indices; // The indices of this brush

    /// <summary>
    /// Create a brush with specified mins and maxs
    /// </summary>
    /// <param name="mins">The minimum values, e.g. bottom of the brush</param>
    /// <param name="maxs">The maximum values, e.g. top of the brush</param>
    public Brush( Vector3 mins, Vector3 maxs )
    {
        bbox.mins = mins;
        bbox.maxs = maxs;

        InitializeVertices();
    }

    /// <summary>
    /// Create a brush with a specified <see cref="BoundingBox"/>
    /// </summary>
    public Brush( BoundingBox bbox )
    {
        this.bbox = bbox;
        InitializeVertices();
    }

    /// <summary>
    /// Initializes all of the necessary vertices based on the bounding box of the brush
    /// </summary>
    private void InitializeVertices()
    {
        // Initialize vertices based on the bbox
        vertices =
        [
            // Comments are organized like this:
            // {position on face} - {face}, {position on face} - {face}, {position on face} - {face}
            // This is so you don't have to guess which is which, it's about as much as one can do when it comes to
            // making this have any semblence of being readable
            new Vertex(new Vector3(bbox.maxs.X, bbox.maxs.Y, bbox.maxs.Z), Vector2.One), // Top left     - front, Bottom left  - top,    Top right    - left
            new Vertex(new Vector3(bbox.mins.X, bbox.maxs.Y, bbox.maxs.Z), Vector2.One), // Top right    - front, Bottom right - top,    Top left     - right
            new Vertex(new Vector3(bbox.maxs.X, bbox.maxs.Y, bbox.mins.Z), Vector2.One), // Bottom left  - front, Top left     - bottom, Bottom right - left
            new Vertex(new Vector3(bbox.mins.X, bbox.maxs.Y, bbox.mins.Z), Vector2.One), // Bottom right - front, Top right    - bottom, Bottom left  - right
            new Vertex(new Vector3(bbox.maxs.X, bbox.mins.Y, bbox.maxs.Z), Vector2.One), // Top left     - back,  Top left     - top,    Top left     - left
            new Vertex(new Vector3(bbox.mins.X, bbox.mins.Y, bbox.maxs.Z), Vector2.One), // Top right    - back,  Top right    - top,    Top right    - right
            new Vertex(new Vector3(bbox.maxs.X, bbox.mins.Y, bbox.mins.Z), Vector2.One), // Bottom left  - back,  Bottom left  - bottom, Bottom left  - left
            new Vertex(new Vector3(bbox.mins.X, bbox.mins.Y, bbox.mins.Z), Vector2.One), // Bottom right - back,  Bottom right - bottom, Bottom right - right
        ];

        // Initialize this brush's indices
        indices =
        [
            0, 1, 3, 3, 1, 2,
            1, 5, 2, 2, 5, 6,
            5, 4, 6, 6, 4, 7,
            4, 0, 7, 7, 0, 3,
            3, 2, 7, 7, 2, 6,
            4, 5, 0, 0, 5, 1,
        ];

#if DEBUG
        // Log the vertices for information's sake
        Log.Info( "Vertices initialized. Their values are:" );
        foreach ( Vertex vertice in vertices )
        {
            Log.Info( $"\t{vertice.ToString()}" );
        }
#endif // DEBUG
    }

    /// <summary>
    /// Set the ID of this brush
    /// </summary>
    public void SetID( string id )
    {
        this.id = id;
    }

    /// <summary>
    /// Set the BBox of this brush
    /// </summary>
    public void SetBBox( BoundingBox bbox )
    {
        this.bbox = bbox;
    }

    /// <summary>
    /// Get the ID of this brush
    /// </summary>
    public string GetID()
    {
        return id;
    }

    /// <summary>
    /// Get the BBox of this brush
    /// </summary>
    public BoundingBox GetBBox()
    {
        return bbox;
    }

    /// <summary>
    /// Turn this brush into an entity
    /// </summary>
    public void TurnIntoEntity<T>( T desiredEntity ) where T : Entity, new()
    {
        desiredEntity = new T();

        desiredEntity.SetBoundingBox( bbox );
        desiredEntity.SetPosition( bbox.GetCenter() );

        EngineManager.currentFile.RemoveBrush( this );
        EngineManager.currentFile.AddEntity( desiredEntity );
    }
}