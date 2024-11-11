using System;

namespace DoomNET.Resources;

/// <summary>
/// A bounding box, defining the height, width and depth of an object
/// </summary>
public class BBox
{
    public Vector3 maxs { get; set; } // The max extents of this BBox
    public Vector3 mins { get; set; } // The min extents of this BBox

    public BBox() 
    {
        maxs = new();
        mins = new();
    }

    public BBox(Vector3 maxs, Vector3 mins)
    {
        this.maxs = maxs;
        this.mins = mins;
    }

    /// <summary>
    /// Is this BBox intersecting with another BBox?
    /// </summary>
    /// <param name="other">The other BBox to check for intersections with</param>
    /// <returns><see langword="true"/> if intersecting with <see href="other"/>, <see langword="false"/> if not</returns>
    public bool IntersectingWith(BBox other)
    {
        return mins <= other.maxs && maxs >= other.mins;
    }

    /// <summary>
    /// Is this BBox intersecting with a point (<see cref="Vector3"/>)?
    /// </summary>
    /// <param name="point">A point in space which is either intersecting or not intersecting with this BBox</param>
    /// <returns><see langword="true"/> if the point is intersecting with the BBox, <see langword="false"/> if not</returns>
    public bool IntersectingWith(Vector3 point)
    {
        return point >= mins && point <= maxs;
    }

    public Vector3 GetCenter()
    {
        return (maxs - mins) / 2; // THANK YOU RUSSELL 🙏
    }

    public override string ToString()
    {
        return $"({mins.ToString()}, {maxs.ToString()})";
    }
}