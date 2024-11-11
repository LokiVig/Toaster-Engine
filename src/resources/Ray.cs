using DoomNET.WTF;
using DoomNET.Entities;
using System;
using System.Numerics;

namespace DoomNET.Resources;

public class Ray
{
    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Vector3 rayStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None)
    {
        Vector3 rayEnd = (rayStart + rayDirection.Normalized()) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag(RayIgnore.Entities))
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

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag(RayIgnore.Brushes))
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

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine($"Trace failed.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Vector3 rayStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object ignoreObject = null)
    {
        Vector3 rayEnd = (rayStart + rayDirection.Normalized()) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag(RayIgnore.Entities))
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
                            hitObject = null;
                            continue;
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: {(ignoreObject != null ? ignoreObject : "N/A")}\n");

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag(RayIgnore.Brushes))
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
                            hitObject = null;
                            continue;
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: {(ignoreObject != null ? ignoreObject : "N/A")}\n");

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine($"Trace failed.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: {(ignoreObject != null ? ignoreObject : "N/A")}\n");

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Vector3 rayStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoreObjects = null)
    {
        Vector3 rayEnd = (rayStart + rayDirection.Normalized()) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag(RayIgnore.Entities))
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
                                hitObject = null;
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag(RayIgnore.Brushes))
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
                                hitObject = null;
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine($"Trace failed.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Vector3 rayStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object ignoreObject = null, float rayLength = 5000)
    {
        Vector3 rayEnd = (rayStart + rayDirection.Normalized()) * rayLength;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag(RayIgnore.Entities))
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
                            hitObject = null;
                            continue;
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: {(ignoreObject != null ? ignoreObject : "N/A")}\n");

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag(RayIgnore.Brushes))
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
                            hitObject = null;
                            continue;
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: {(ignoreObject != null ? ignoreObject : "N/A")}\n");

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine($"Trace failed.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: {(ignoreObject != null ? ignoreObject : "N/A")}\n");

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Vector3 rayStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoreObjects = null, float rayLength = 5000)
    {
        Vector3 rayEnd = (rayStart + rayDirection.Normalized()) * rayLength;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag(RayIgnore.Entities))
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
                                hitObject = null;
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object(s): {(ignoreObjects != null ? ignoreObjects : "N/A")}\n");

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag(RayIgnore.Brushes))
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

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object(s): {(ignoreObjects != null ? ignoreObjects : "N/A")}\n");

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine($"Trace failed.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object(s): {(ignoreObjects != null ? ignoreObjects : "N/A")}\n");

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Entity"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Entity entStart, Entity entDir, out object hitObject, RayIgnore rayIgnore = RayIgnore.None)
    {
        Vector3 rayEnd = (entStart.GetPosition() + entDir.GetPosition().Normalized()) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag(RayIgnore.Entities))
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

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {entStart}\n" +
                                $"\tDirection: {entDir}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag(RayIgnore.Brushes))
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

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {entStart}\n" +
                                $"\tDirection: {entDir}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine($"Trace failed.\n" +
                                $"\tStart: {entStart}\n" +
                                $"\tDirection: {entDir}\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Entity"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace(Entity entStart, Entity entDir, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object ignoreObject = null)
    {
        Vector3 rayEnd = (entStart.GetPosition() + entDir.GetPosition().Normalized()) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag(RayIgnore.Entities))
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

                    if (ignoreObject != null)
                    {
                        if (hitObject == ignoreObject)
                        {
                            hitObject = null;
                            continue;
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {entStart}\n" +
                                $"\tDirection: {entDir}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag(RayIgnore.Brushes))
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

                    // Log to the console that we've succeeded
                    Console.WriteLine($"Trace succeeded.\n" +
                                $"\tStart: {entStart}\n" +
                                $"\tDirection: {entDir}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine($"Trace failed.\n" +
                                $"\tStart: {entStart}\n" +
                                $"\tDirection: {entDir}\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n");

        // We didn't hit anything, so it's false
        return false;
    }
}

public enum RayIgnore
{
    None, // Hit everything
    Brushes, // Ignore brushes
    Entities // Ignore entities
}