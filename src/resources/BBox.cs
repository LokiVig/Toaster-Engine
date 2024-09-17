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
    public bool Intersecting(BBox other)
    {

        if ((mins.x <= other.maxs.x && maxs.x >= other.mins.x) &&
            (mins.y <= other.maxs.y && maxs.y >= other.mins.y) &&
            (mins.z <= other.maxs.z && maxs.y >= other.mins.z))
        {
            OnIntersect?.Invoke();
            return true;
        }

        return false;
    }

    public bool Intersecting(Vector3 point)
    {
        if ((point.x >= mins.x && point.x <= maxs.x) &&
            (point.y >= mins.y && point.y <= maxs.y) &&
            (point.z >= mins.z && point.z <= maxs.z))
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