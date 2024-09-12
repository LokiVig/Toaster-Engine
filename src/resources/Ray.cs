using DoomNET.WTF;
using DoomNET.Entities;

namespace DoomNET.Resources;

public class Ray
{
    /// <summary>
    /// Trace a ray from a specified starting position to a direction, with ignore flags and length values
    /// </summary>
    /// <returns><see langword="true"></langword> and the object we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Vector3 startPos, Vector3 direction, out object hitObject, RayIgnore ignore = RayIgnore.None, float length = float.MaxValue)
    {
        Vector3 rayEnd = (startPos + direction.Normalized()) * length;
        
        // If we're not ignoring entities
        if (!ignore.HasFlag(RayIgnore.Entities))
        {
            // Check every entity
            foreach ((Entity entity, string id) in DoomNET.file?.GetEntities())
            {
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().Intersecting(rayEnd))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;
                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!ignore.HasFlag(RayIgnore.Brushes))
        {
            // Check every brush
            foreach (Brush brush in DoomNET.file?.GetBrushes())
            {
                // Are we intersecting with this brush's bounding box?
                if (brush.GetBBox().Intersecting(rayEnd))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;
                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is now null, and we're returning false
        hitObject = null;
        return false;
    }
}

public enum RayIgnore
{
    None, // Hit everything
    Brushes, // Ignore brushes
    Entities // Ignore entities
}