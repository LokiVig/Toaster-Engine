using System;

namespace DoomNET.Resources;

/// <summary>
/// A bounding box, defining the height, width and depth of an object
/// </summary>
public class BBox
{
    public Vector3 maxs { get; set; } // The max extents of this BBox
    public Vector3 mins { get; set; } // The min extents of this BBox

    public static readonly BBox One = new BBox(Vector3.One, -Vector3.One);
    public static readonly BBox Zero = new BBox(Vector3.Zero, Vector3.Zero);
    
    public BBox()
    {
        maxs = new();
        mins = new();
    }

    public BBox( Vector3 maxs, Vector3 mins )
    {
        this.maxs = maxs;
        this.mins = mins;
    }

    /// <summary>
    /// Is this BBox intersecting with another BBox?
    /// </summary>
    /// <param name="other">The other BBox to check for intersections with</param>
    /// <returns><see langword="true"/> if intersecting with <see href="other"/>, <see langword="false"/> if not</returns>
    public bool IntersectingWith( BBox other )
    {
        // We shouldn't be trying to intersect with ourselves!
        if (other == this)
        {
            return false;
        }

        return mins <= other.maxs && maxs >= other.mins;
    }

    /// <summary>
    /// Is this BBox intersecting with a point (<see cref="Vector3"/>)?
    /// </summary>
    /// <param name="point">A point in space which is either intersecting or not intersecting with this BBox</param>
    /// <returns><see langword="true"/> if the point is intersecting with the BBox, <see langword="false"/> if not</returns>
    public bool IntersectingWith( Vector3 point )
    {
        return point.x >= mins.x && point.x <= maxs.x &&
               point.y >= mins.y && point.y <= maxs.y &&
               point.z >= mins.z && point.z <= maxs.z;
    }

    public Vector3 GetCenter()
    {
        return ( maxs - mins ) / 2; // THANK YOU RUSSELL 🙏
    }

    public override string ToString()
    {
        return $"{{{mins.ToString()}, {maxs.ToString()}}}";
    }
}