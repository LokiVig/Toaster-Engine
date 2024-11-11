using DoomNET.WTF;
using DoomNET.Entities;

namespace DoomNET.Resources;

public class Ray
{
    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Vector3 startPos, Vector3 direction, out object hitObject, RayIgnore ignore = RayIgnore.None)
    {
        Vector3 rayEnd = (startPos + direction.Normalized()) * float.MaxValue;

        // If we're not ignoring entities
        if (!ignore.HasFlag(RayIgnore.Entities))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.file?.GetEntities())
            {
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().IntersectingWith(rayEnd))
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
                if (brush.GetBBox().IntersectingWith(rayEnd))
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

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific objects to ignore, and lengths
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Vector3 startPos, Vector3 direction, out object hitObject, RayIgnore ignore = RayIgnore.None, object ignoreObject = null)
    {
        Vector3 rayEnd = (startPos + direction.Normalized()) * float.MaxValue;

        // If we're not ignoring entities
        if (!ignore.HasFlag(RayIgnore.Entities))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.file?.GetEntities())
            {
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().IntersectingWith(rayEnd))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                    if (ignoreObject != null)
                    {
                        if (hitObject == ignoreObject)
                        {
                            continue;
                        }
                    }

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
                if (brush.GetBBox().IntersectingWith(rayEnd))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    // Uh-oh! The hit brush is an object we wish to ignore, continue in the foreach
                    if (ignoreObject != null)
                    {
                        if (hitObject == ignoreObject)
                        {
                            continue;
                        }
                    }

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is now null, and we're returning false
        hitObject = null;
        return false;
    }
    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific objects to ignore, and lengths
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Vector3 startPos, Vector3 direction, out object hitObject, RayIgnore ignore = RayIgnore.None, object[] ignoreObjects = null)
    {
        Vector3 rayEnd = (startPos + direction.Normalized()) * float.MaxValue;

        // If we're not ignoring entities
        if (!ignore.HasFlag(RayIgnore.Entities))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.file?.GetEntities())
            {
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().IntersectingWith(rayEnd))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[i])
                            {
                                continue;
                            }
                        }
                    }

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
                if (brush.GetBBox().IntersectingWith(rayEnd))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[i])
                            {
                                continue;
                            }
                        }
                    }

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is now null, and we're returning false
        hitObject = null;
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific objects to ignore, and lengths
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Vector3 startPos, Vector3 direction, out object hitObject, RayIgnore ignore = RayIgnore.None, object ignoreObject = null, float length = float.MaxValue)
    {
        Vector3 rayEnd = (startPos + direction.Normalized()) * length;

        // If we're not ignoring entities
        if (!ignore.HasFlag(RayIgnore.Entities))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.file?.GetEntities())
            {
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().IntersectingWith(rayEnd))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                    if (ignoreObject != null)
                    {
                        if (hitObject == ignoreObject)
                        {
                            continue;
                        }
                    }

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
                if (brush.GetBBox().IntersectingWith(rayEnd))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    // Uh-oh! The hit brush is an object we wish to ignore, continue in the foreach
                    if (ignoreObject != null)
                    {
                        if (hitObject == ignoreObject)
                        {
                            continue;
                        }
                    }

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