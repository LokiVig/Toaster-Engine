using System;

using DoomNET.Entities;
using DoomNET.Rendering;

namespace DoomNET.Resources;

/// <summary>
/// A brush, a collection of 6 solid faces, stretching from one point to another, defined with a bounding box
/// </summary>
public struct Brush
{
    public string id { get; set; } // The id of this brush, can be set by the mapper
    public BBox bbox { get; set; } = BBox.One; // The extents of this brush
    public Vertex[] vertices { get; set; } // The vertices of this brush
    
    // public Material[] textures { get; set; } =  // Per-face texture array - should ONLY be 6
    // {
    //     // !!Arbitrary placing!!
    //     // TODO: Implement an actually proper system that makes sense
    //     new Material(Texture.MISSINGTEXTURE), // Top    (^) // Rolling leftward from the top,
    //     new Material(Texture.MISSINGTEXTURE), // Left   (<) // to the left,
    //     new Material(Texture.MISSINGTEXTURE), // Bottom (v) // to the bottom,
    //     new Material(Texture.MISSINGTEXTURE), // Right  (>) // to the right,
    //     new Material(Texture.MISSINGTEXTURE), // Front  (F) // to the front,
    //     new Material(Texture.MISSINGTEXTURE), // Back   (B) // to the back
    // }; 
    
    /// <summary>
    /// Create a brush with specified mins and maxs
    /// </summary>
    /// <param name="mins">The minimum values, e.g. bottom of the brush</param>
    /// <param name="maxs">The maximum values, e.g. top of the brush</param>
    public Brush(Vector3 mins, Vector3 maxs)
    {
        bbox.mins = mins;
        bbox.maxs = maxs;
        InitializeVertices();
    }

    /// <summary>
    /// Create a brush with a specified <see cref="BBox"/>
    /// </summary>
    public Brush(BBox bbox)
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
            // {position on face}, {face} - {position on face}, {face} - {position on face}, {face}
            // This is so you don't have to guess which is which, it's about as much as one can do when it comes to
            // making this have any semblence of being readable
            new Vertex(new Vector3(bbox.maxs.x, bbox.maxs.y, bbox.maxs.z), Vector2.One), // Top left,     front - Bottom left,  top    - Top right,    left
            new Vertex(new Vector3(bbox.mins.x, bbox.maxs.y, bbox.maxs.z), Vector2.One), // Top right,    front - Bottom right, top    - Top left,     right
            new Vertex(new Vector3(bbox.maxs.x, bbox.maxs.y, bbox.mins.z), Vector2.One), // Bottom left,  front - Top left,     bottom - Bottom right, left
            new Vertex(new Vector3(bbox.mins.x, bbox.maxs.y, bbox.mins.z), Vector2.One), // Bottom right, front - Top right,    bottom - Bottom left,  right
            new Vertex(new Vector3(bbox.maxs.x, bbox.mins.y, bbox.maxs.z), Vector2.One), // Top left,     back  - Top left,     top    - Top left,     left
            new Vertex(new Vector3(bbox.mins.x, bbox.mins.y, bbox.maxs.z), Vector2.One), // Top right,    back  - Top right,    top    - Top right,    right
            new Vertex(new Vector3(bbox.maxs.x, bbox.mins.y, bbox.mins.z), Vector2.One), // Bottom left,  back  - Bottom left,  bottom - Bottom left,  left
            new Vertex(new Vector3(bbox.mins.x, bbox.mins.y, bbox.mins.z), Vector2.One), // Bottom right, back  - Bottom right, bottom - Bottom right, right
        ];
        
        #if DEBUG
        // Log the vertices for information's sake
        Console.WriteLine("Vertices initialized. Their values are:");
        foreach (Vertex vertice in vertices)
        {
            Console.WriteLine($"\t{vertice.ToString()}");
        }
        Console.Write("\n");
        #endif // DEBUG
    }
    
    /// <summary>
    /// Set the ID of this brush
    /// </summary>
    public void SetID(string id)
    {
        this.id = id;
    }

    /// <summary>
    /// Set the BBox of this brush
    /// </summary>
    public void SetBBox(BBox bbox)
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
    public BBox GetBBox()
    {
        return bbox;
    }

    /// <summary>
    /// Turn this brush into an entity
    /// </summary>
    public void TurnIntoEntity<T>(T desiredEntity) where T : Entity
    {
        desiredEntity.SetBBox(bbox);
        desiredEntity.SetPosition(bbox.GetCenter());

        Game.currentFile.RemoveBrush(this);
        Game.currentFile.AddEntity(desiredEntity);
    }
}