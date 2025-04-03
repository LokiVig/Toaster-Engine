using System;
using System.Numerics;

namespace Toast.Engine.Resources;

/// <summary>
/// A bounding box, defining the height, width and depth of an object
/// </summary>
public struct BoundingBox
{
    public Vector3 mins; // The minimum extents of this BBox
    public Vector3 maxs; // The maximum extents of this BBox

    public static readonly BoundingBox One = new BoundingBox( -Vector3.One, Vector3.One );

    public static readonly BoundingBox SmallTool = new BoundingBox( new Vector3( -8 ), new Vector3( 8 ) );
    public static readonly BoundingBox MediumTool = new BoundingBox( new Vector3( -16 ), new Vector3( 16 ) );
    public static readonly BoundingBox LargeTool = new BoundingBox( new Vector3( -32 ), new Vector3( 32 ) );

    public static readonly BoundingBox SmallEntity = new BoundingBox( new Vector3( -8, -8, 0 ), new Vector3( 8, 8, 16 ) );
    public static readonly BoundingBox MediumEntity = new BoundingBox( new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 32 ) );
    public static readonly BoundingBox LargeEntity = new BoundingBox( new Vector3( -32, -32, 0 ), new Vector3( 32, 32, 64 ) );

    public BoundingBox( Vector3 mins, Vector3 maxs )
    {
        this.mins = mins;
        this.maxs = maxs;
    }

    /// <summary>
    /// Is this BBox intersecting with another BBox?
    /// </summary>
    /// <param name="other">The other BBox to check for intersections with</param>
    /// <returns><see langword="true"/> if intersecting with <paramref name="other"/>, <see langword="false"/> if not</returns>
    public bool IntersectingWith( BoundingBox other )
    {
        // We shouldn't be trying to intersect with ourself!
        if ( other == this )
        {
            return false;
        }

        return mins.X <= other.maxs.X && maxs.X >= other.mins.X &&
               mins.Y <= other.maxs.Y && maxs.Y >= other.mins.Y &&
               mins.Z <= other.maxs.Z && maxs.Z >= other.mins.Z;
    }

    /// <summary>
    /// Is this BBox intersecting with a point (<see cref="Vector3"/>)?
    /// </summary>
    /// <param name="point">A point in space which is either intersecting or not intersecting with this BBox</param>
    /// <returns><see langword="true"/> if the point is intersecting with the BBox, <see langword="false"/> if not</returns>
    public bool IntersectingWith( Vector3 point )
    {
        return point.X >= mins.X && point.X <= maxs.X &&
               point.Y >= mins.Y && point.Y <= maxs.Y &&
               point.Z >= mins.Z && point.Z <= maxs.Z;
    }

    /// <summary>
    /// Checks if a ray intersects this bounding box.
    /// </summary>
    /// <param name="rayOrigin">The origin of the ray.</param>
    /// <param name="rayDirection">The direction of the ray (must be normalized).</param>
    /// <param name="rayLength">The maximum length of the ray.</param>
    /// <returns><see langword="true"/> if the ray intersects the bounding box, <see langword="false"/> otherwise.</returns>
    public bool RayIntersects( Vector3 rayOrigin, Vector3 rayDirection, float rayLength )
    {
        // Avoid division by zero; use an epsilon value
        const float epsilon = 1e-6f;

        float tMin = 0; // Start of intersection interval
        float tMax = rayLength; // End of intersection interval

        // Check each axis
        for ( int i = 0; i < 3; i++ )
        {
            float origin = rayOrigin[i];
            float direction = rayDirection[i];
            float min = mins[i];
            float max = maxs[i];

            if ( MathF.Abs( direction ) < epsilon )
            {
                // Ray is parallel to the object (no intersection if origin is outside the object)
                if ( origin < min || origin > max )
                {
                    return false;
                }
            }
            else
            {
                // Calculate intersection t-values for the near and far planes
                float t1 = ( min - origin ) / direction;
                float t2 = ( max - origin ) / direction;

                // Swap t1 and t2 if needed to ensure t1 is the near intersection
                if ( t1 > t2 )
                {
                    (t1, t2) = (t2, t1);
                }

                // Update the intersection interval
                tMin = MathF.Max( tMin, t1 );
                tMax = MathF.Min( tMax, t2 );

                // If the interval is invalid, there's no intersection
                if ( tMin > tMax )
                {
                    return false;
                }
            }
        }

        // If we reach this point, the ray intersects the bounding box within the valid range
        return true;
    }

    public Vector3 GetCenter()
    {
        return ( maxs + mins ) / 2; // THANK YOU, RUSSELL 🙏
    }

    public override string ToString()
    {
        //
        // This is a bit of a mess...
        // Find a way to improve this! I wanna actually be able to read this!
        //

        if ( this == SmallTool )
        {
            return "Small Tool";
        }
        else if ( this == MediumTool )
        {
            return "Medium Tool";
        }
        else if ( this == LargeTool )
        {
            return "Large Tool";
        }
        else if ( this == SmallEntity )
        {
            return "Small Entity";
        }
        else if ( this == MediumEntity )
        {
            return "Medium Entity";
        }
        else if ( this == LargeEntity )
        {
            return "Large Entity";
        }
        else
        {
            return $"{{{mins}, {maxs}}}";
        }
    }

    public static bool operator ==( BoundingBox lhs, BoundingBox rhs )
    {
        return lhs.mins == rhs.mins && lhs.maxs == rhs.maxs;
    }

    public static bool operator !=( BoundingBox lhs, BoundingBox rhs )
    {
        return lhs.mins != rhs.mins || lhs.maxs != rhs.maxs;
    }

    public override bool Equals( object obj )
    {
        return base.Equals( obj );
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}