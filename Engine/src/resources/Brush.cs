using System.Numerics;

using Newtonsoft.Json;

using Toast.Engine.Entities.Brushes;
using Toast.Engine.Rendering;

namespace Toast.Engine.Resources;

/// <summary>
/// A brush, a collection of 6 solid faces, stretching from one point to another, defined with a bounding box
/// </summary>
public struct Brush
{
    public string id; // The id of this brush, can be set by the mapper
    public BoundingBox boundingBox = BoundingBox.One; // The extents of this brush

    [JsonIgnore] public Vertex[] vertices; // The vertices of this brush
    [JsonIgnore] public uint[] indices; // The indices of this brush

    /// <summary>
    /// Create a brush with specified mins and maxs
    /// </summary>
    /// <param name="mins">The minimum values, e.g. bottom of the brush</param>
    /// <param name="maxs">The maximum values, e.g. top of the brush</param>
    public Brush( Vector3 mins, Vector3 maxs )
    {
        boundingBox.mins = mins;
        boundingBox.maxs = maxs;

        InitializeVertices();
    }

    /// <summary>
    /// Create a brush with a specified <see cref="BoundingBox"/>.<br/>
    /// Can be used to convert a brush entity back into a brush.
    /// </summary>
    public Brush( BoundingBox boundingBox )
    {
        this.boundingBox = boundingBox;
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
            new Vertex(new Vector3(boundingBox.maxs.X, boundingBox.maxs.Y, boundingBox.maxs.Z), Vector2.One), // Top left     - front, Bottom left  - top,    Top right    - left
            new Vertex(new Vector3(boundingBox.mins.X, boundingBox.maxs.Y, boundingBox.maxs.Z), Vector2.One), // Top right    - front, Bottom right - top,    Top left     - right
            new Vertex(new Vector3(boundingBox.maxs.X, boundingBox.maxs.Y, boundingBox.mins.Z), Vector2.One), // Bottom left  - front, Top left     - bottom, Bottom right - left
            new Vertex(new Vector3(boundingBox.mins.X, boundingBox.maxs.Y, boundingBox.mins.Z), Vector2.One), // Bottom right - front, Top right    - bottom, Bottom left  - right
            new Vertex(new Vector3(boundingBox.maxs.X, boundingBox.mins.Y, boundingBox.maxs.Z), Vector2.One), // Top left     - back,  Top left     - top,    Top left     - left
            new Vertex(new Vector3(boundingBox.mins.X, boundingBox.mins.Y, boundingBox.maxs.Z), Vector2.One), // Top right    - back,  Top right    - top,    Top right    - right
            new Vertex(new Vector3(boundingBox.maxs.X, boundingBox.mins.Y, boundingBox.mins.Z), Vector2.One), // Bottom left  - back,  Bottom left  - bottom, Bottom left  - left
            new Vertex(new Vector3(boundingBox.mins.X, boundingBox.mins.Y, boundingBox.mins.Z), Vector2.One), // Bottom right - back,  Bottom right - bottom, Bottom right - right
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
        this.boundingBox = bbox;
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
        return boundingBox;
    }

    /// <summary>
    /// Turn this brush into a specified entity.
    /// </summary>
    /// <typeparam name="T">The specific brush entity we wish to turn this brush into.</typeparam>
    /// <returns>A new entity of type <typeparamref name="T"/>, with the same bounding box and position as this brush.</returns>
    public T TurnIntoEntity<T>() where T : BrushEntity, new()
    {
        // Create a new instance of the entity
        T resultingEntity = new T();

        // Set the new entity's bounding box and position appropriately
        resultingEntity.SetBoundingBox( boundingBox );
        resultingEntity.SetPosition( boundingBox.GetCenter() );

        // Remove this brush and add the specified entity
        EngineManager.currentFile.RemoveBrush( this );
        EngineManager.currentFile.AddEntity( resultingEntity );

        // Return the entity that we made throughout this method
        return resultingEntity;
    }
}