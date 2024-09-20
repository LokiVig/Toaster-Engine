using System;

namespace DoomNET.Resources;

/// <summary>
/// A bounding box, defining the height, width and depth of an object
/// </summary>
public class BBox
{
    public Vector3 maxs { get; set; } // The max extents of this BBox
    public Vector3 mins { get; set; } // The min extents of this BBox

    public event Action OnIntersect;

    /// <summary>
    /// Is this BBox intersecting with another?
    /// </summary>
    /// <param name="other">The other BBox to check for intersections</param>
    /// <returns><see langword="true"/> if intersecting with the other, <see langword="false"/> if not</returns>
    public bool IntersectingWith(BBox other)
    {
        if (mins <= other.maxs && maxs >= other.mins)
        {
            OnIntersect?.Invoke();
            return true;
        }

        return false;
    }

    public bool IntersectingWith(Vector3 point)
    {
        if (point >= mins && point <= maxs)
        {
            OnIntersect?.Invoke();
            return true;
        }

        return false;
    }

    public Vector3 GetCenter()
    {
        return (maxs - mins) / 2 + mins; // THANK YOU RUSSELL 🙏
    }
}