using System;
using System.Numerics;

namespace Toast.Engine.Resources;

/// <summary>
/// This class defines a world and local position in 3D space, a world and local rotation, and a bounding box.
/// </summary>
public class Transform
{
    public Vector3 worldPosition = Vector3.Zero; // The world position of this transform
    public Vector3 localPosition = Vector3.Zero; // The local position of this transform

    public Vector3 velocity = Vector3.Zero; // The velocity of this transform

    public Quaternion worldRotation = Quaternion.Identity; // The world rotation of this transform
    public Quaternion localRotation = Quaternion.Identity; // The local rotation of this transform

    public BoundingBox boundingBox; // The bounding box of this transform

    /// <summary>
    /// Distance to another <see cref="Transform"/>.
    /// </summary>
    /// <param name="other">The other transform to compare against.</param>
    /// <returns>The closest distance between this transform and the other.</returns>
    public float DistanceTo( Transform other )
    {
        float res = Vector3.Distance(worldPosition, other.worldPosition); // Get the distance between this world position and the other's
        res = Math.Min( res, Vector3.Distance( localPosition, other.worldPosition ) ); // Check if local-world is closer than the world-world is
        res = Math.Min( res, Vector3.Distance( localPosition, other.localPosition ) ); // Check if local-local is closer than the local-world is
        
        // Return the result
        return res;
    }

    /// <summary>
    /// Distance to another <see cref="Transform"/> squared.
    /// </summary>
    /// <param name="other">The other transform to compare against.</param>
    /// <returns>The squared, closest distance between this transform and the other.</returns>
    public float DistanceToSqr( Transform other )
    {
        float res = DistanceTo( other ); // Get the original distance
        return res * res; // Return the square distance
    }

    /// <summary>
    /// Updates positions, rotations, etc. every frame.<br/>
    /// Mainly this is to update the local positions / rotations to let them act as an offset from their world position.
    /// </summary>
    public void Update()
    {
        localPosition += worldPosition;
        localRotation += worldRotation;
    }
}