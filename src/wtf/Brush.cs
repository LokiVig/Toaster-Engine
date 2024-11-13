using DoomNET.Entities;
using DoomNET.Resources;

namespace DoomNET.WTF;

/// <summary>
/// A brush, a collection of 6 solid faces, stretching from one point to another, defined with a bounding box
/// </summary>
public struct Brush
{
    public BBox bbox { get; set; } = new(); // The extents of this brush, bbox.mins.z being the very bottom, bbox.maxs.z being the top
    public string id { get; set; } // The id of this brush, can be set by the mapper

    public Brush() { }

    /// <summary>
    /// Create a brush with specified mins and maxs, and add it to the current WTF file
    /// </summary>
    /// <param name="mins">The minimum values, e.g. bottom of the brush</param>
    /// <param name="maxs">The maximum values, e.g. top of the brush</param>
    public Brush( Vector3 mins, Vector3 maxs )
    {
        bbox.mins = mins;
        bbox.maxs = maxs;
    }

    /// <summary>
    /// Create a brush with a specified bbox, and add it to the current WTF file
    /// </summary>
    public Brush( BBox bbox )
    {
        this.bbox = bbox;
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
    public void SetBBox( BBox bbox )
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
    public void TurnIntoEntity( Entity desiredEntity )
    {
        desiredEntity.SetBBox( bbox );
        desiredEntity.SetPosition( bbox.GetCenter() );

        DoomNET.file.RemoveBrush( this );
        DoomNET.file.AddEntity( desiredEntity );
    }
}