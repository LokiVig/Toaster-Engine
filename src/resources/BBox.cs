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
        return mins <= other.maxs && maxs >= other.mins;
    }

    /// <summary>
    /// Is this BBox intersecting with a point (<see cref="Vector3"/>)?
    /// </summary>
    /// <param name="point">A point in space which is either intersecting or not intersecting with this BBox</param>
    /// <returns><see langword="true"/> if the point is intersecting with the BBox, <see langword="false"/> if not</returns>
    public bool IntersectingWith( Vector3 point )
    {
        return point >= mins && point <= maxs;
    }

    /// <summary>
    /// Checks if a ray intersects with this BBox and provides the distance to the intersection.
    /// </summary>
    /// <param name="origin">The origin of the ray.</param>
    /// <param name="direction">The direction of the ray.</param>
    /// <param name="distance">The distance from origin to the intersection, if any.</param>
    /// <returns><see langword="true"/> if the ray intersects the BBox, <see langword="false"/> if not.</returns>
    public bool IntersectingWith( Vector3 origin, Vector3 direction, out float distance )
    {
        distance = float.MaxValue;

        // Avoid division by zero by setting a small threshold for direction components near zero
        const float epsilon = 1e-8f;

        float tMin = ( (float)( mins - direction ) ) / ( Math.Abs( (float)direction ) < epsilon ? epsilon : (float)direction );
        float tMax = ( (float)( maxs - direction ) ) / ( Math.Abs( (float)direction ) < epsilon ? epsilon : (float)direction );

        if (tMin > tMax)
        {
            (tMin, tMax) = (tMax, tMin);
        }

        // If we get here, there's an intersection; tMin is the closest intersection point
        distance = (float)tMin > 0 ? (float)tMin : (float)tMax;
        return distance >= 0;
    }

    public Vector3 GetCenter()
    {
        return ( maxs - mins ) / 2; // THANK YOU RUSSELL 🙏
    }

    public override string ToString()
    {
        return $"({mins.ToString()}, {maxs.ToString()})";
    }
}