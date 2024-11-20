using System;

using DoomNET.Entities;

namespace DoomNET.Resources;

public class Ray
{
    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Vector3 rayStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None )
    {
        Vector3 rayEnd = ( rayStart + rayDirection.Normalized() ) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag( RayIgnore.Entities ))
        {
            
            // Check every entity
            foreach (Entity entity in DoomNET.currentScene?.GetEntities())
            {
                // Automatically ignored entities
                
                // Ignore entity spawners - we should never have to trace to an entity spawner!
                if (entity is EntitySpawner)
                {
                    continue;
                }
                
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().RayIntersects( rayStart, rayEnd, 5000 ))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n" );

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag( RayIgnore.Brushes ))
        {
            // Check every brush
            foreach (Brush brush in DoomNET.currentScene?.GetBrushes())
            {
                // Are we intersecting with this brush's bounding box?
                if (brush.GetBBox().RayIntersects( rayStart, rayEnd, 5000 ))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine( $"Trace failed.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n" );

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Vector3 rayStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoreObjects = null )
    {
        Vector3 rayEnd = ( rayStart + rayDirection.Normalized() ) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag( RayIgnore.Entities ))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.currentScene?.GetEntities())
            {
                // Automatically ignored entities
                
                // Ignore entity spawners - we should never have to trace to an entity spawner!
                if (entity is EntitySpawner)
                {
                    continue;
                }
                
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().RayIntersects( rayStart, rayEnd, 5000 ))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                hitObject = null;
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag( RayIgnore.Brushes ))
        {
            // Check every brush
            foreach (Brush brush in DoomNET.currentScene?.GetBrushes())
            {
                // Are we intersecting with this brush's bounding box?
                if (brush.GetBBox().RayIntersects( rayStart, rayEnd, 5000 ))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                hitObject = null;
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine( $"Trace failed.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Vector3 rayStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoreObjects = null, float rayLength = 5000 )
    {   
        Vector3 rayEnd = ( rayStart + rayDirection.Normalized() ) * rayLength;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag( RayIgnore.Entities ))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.currentScene?.GetEntities())
            {
                // Automatically ignored entities
                
                // Ignore entity spawners - we should never have to trace to an entity spawner!
                if (entity is EntitySpawner)
                {
                    continue;
                }
                
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().RayIntersects( rayStart, rayEnd, rayLength ))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                hitObject = null;
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag( RayIgnore.Brushes ))
        {
            // Check every brush
            foreach (Brush brush in DoomNET.currentScene?.GetBrushes())
            {
                // Are we intersecting with this brush's bounding box?
                if (brush.GetBBox().RayIntersects( rayStart, rayEnd, rayLength ))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine( $"Trace failed.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {rayDirection}\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Entity"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Entity entStart, Entity entDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None )
    {
        Vector3 rayEnd = ( entStart.GetPosition() + entDirection.GetPosition().Normalized() ) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag( RayIgnore.Entities ))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.currentScene?.GetEntities())
            {
                // Automatically ignored entities
                
                // Ignore entity spawners - we should never have to trace to an entity spawner!
                if (entity is EntitySpawner)
                {
                    continue;
                }
                
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().RayIntersects( entStart.GetPosition(), rayEnd, 5000 ))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {entStart} (\"{entStart.GetID()}\")\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n" );

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag( RayIgnore.Brushes ))
        {
            // Check every brush
            foreach (Brush brush in DoomNET.currentScene?.GetBrushes())
            {
                // Are we intersecting with this brush's bounding box?
                if (brush.GetBBox().RayIntersects( entStart.GetPosition(), rayEnd, 5000 ))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {entStart} (\"{entStart.GetID()}\")\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine( $"Trace failed.\n" +
                                $"\tStart: {entStart} (\"{entStart.GetID()}\")\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n" );

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Entity"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Entity entStart, Entity entDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoreObjects = null )
    {
        Vector3 rayEnd = ( entStart.GetPosition() + entDirection.GetPosition().Normalized() ) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag( RayIgnore.Entities ))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.currentScene?.GetEntities())
            {
                // Automatically ignored entities
                
                // Ignore entity spawners - we should never have to trace to an entity spawner!
                if (entity is EntitySpawner)
                {
                    continue;
                }
                
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().RayIntersects( entStart.GetPosition(), rayEnd, 5000 ))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                hitObject = null;
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {entStart} (\"{entStart.GetID()}\")\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag( RayIgnore.Brushes ))
        {
            // Check every brush
            foreach (Brush brush in DoomNET.currentScene?.GetBrushes())
            {
                // Are we intersecting with this brush's bounding box?
                if (brush.GetBBox().RayIntersects( entStart.GetPosition(), rayEnd, 5000 ))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {entStart} (\"{entStart.GetID()}\")\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine( $"Trace failed.\n" +
                                $"\tStart: {entStart} (\"{entStart.GetID()}\")\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Entity"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Entity entStart, Entity entDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoreObjects = null, float rayLength = 5000 )
    {
        Vector3 rayEnd = ( entStart.GetPosition() + entDirection.GetPosition().Normalized() ) * rayLength;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag( RayIgnore.Entities ))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.currentScene?.GetEntities())
            {
                // Automatically ignored entities
                
                // Ignore entity spawners - we should never have to trace to an entity spawner!
                if (entity is EntitySpawner)
                {
                    continue;
                }
                
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().RayIntersects( entStart.GetPosition(), rayEnd, rayLength ))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                hitObject = null;
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {entStart} (\"{entStart.GetID()}\")\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag( RayIgnore.Brushes ))
        {
            // Check every brush
            foreach (Brush brush in DoomNET.currentScene?.GetBrushes())
            {
                // Are we intersecting with this brush's bounding box?
                if (brush.GetBBox().RayIntersects( entStart.GetPosition(), rayEnd, rayLength ))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {entStart} (\"{entStart.GetID()}\")\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine( $"Trace failed.\n" +
                           $"\tStart: {entStart} (\"{entStart.GetID()}\")\n" +
                           $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                           $"\tHit object: N/A\n" +
                           $"\tRayIgnore: {rayIgnore}\n" +
                           $"\tIgnore objects: {( ignoreObjects == null ?
                                   "N/A" :
                                   () =>
                                   {
                                       string objects = string.Empty;

                                       foreach (object obj in ignoreObjects)
                                       {
                                           objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                       }

                                       return objects;
                                   }
                               )}\n" );

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Vector3 rayStart, Entity entDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None )
    {
        Vector3 rayEnd = ( rayStart + entDirection.GetPosition().Normalized() ) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag( RayIgnore.Entities ))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.currentScene?.GetEntities())
            {
                // Automatically ignored entities
                
                // Ignore entity spawners - we should never have to trace to an entity spawner!
                if (entity is EntitySpawner)
                {
                    continue;
                }
                
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().RayIntersects( rayStart, rayEnd, 5000 ))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n" );

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag( RayIgnore.Brushes ))
        {
            // Check every brush
            foreach (Brush brush in DoomNET.currentScene?.GetBrushes())
            {
                // Are we intersecting with this brush's bounding box?
                if (brush.GetBBox().RayIntersects( rayStart, rayEnd, 5000 ))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\")\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine( $"Trace failed.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore object: N/A\n" );

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Vector3 rayStart, Entity entDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoreObjects = null )
    {
        Vector3 rayEnd = ( rayStart + entDirection.GetPosition().Normalized() ) * 5000;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag( RayIgnore.Entities ))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.currentScene?.GetEntities())
            {
                // Automatically ignored entities
                
                // Ignore entity spawners - we should never have to trace to an entity spawner!
                if (entity is EntitySpawner)
                {
                    continue;
                }
                
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().RayIntersects( rayStart, rayEnd, 5000 ))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                hitObject = null;
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag( RayIgnore.Brushes ))
        {
            // Check every brush
            foreach (Brush brush in DoomNET.currentScene?.GetBrushes())
            {
                // Are we intersecting with this brush's bounding box?
                if (brush.GetBBox().RayIntersects( rayStart, rayEnd, 5000 ))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine( $"Trace failed.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: N/A\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Vector3 rayStart, Entity entDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoreObjects = null, float rayLength = 5000 )
    {
        Vector3 rayEnd = ( rayStart + entDirection.GetPosition().Normalized() ) * rayLength;

        // If we're not ignoring entities
        if (!rayIgnore.HasFlag( RayIgnore.Entities ))
        {
            // Check every entity
            foreach (Entity entity in DoomNET.currentScene?.GetEntities())
            {
                // Automatically ignored entities
                
                // Ignore entity spawners - we should never have to trace to an entity spawner!
                if (entity is EntitySpawner)
                {
                    continue;
                }
                
                // Are we intersecting with this entity's bounding box?
                if (entity.GetBBox().RayIntersects( rayStart, rayEnd, rayLength ))
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                hitObject = null;
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if (!rayIgnore.HasFlag( RayIgnore.Brushes ))
        {
            // Check every brush
            foreach (Brush brush in DoomNET.currentScene?.GetBrushes())
            {
                // Are we intersecting with this brush's bounding box?
                if (brush.GetBBox().RayIntersects( rayStart, rayEnd, 5000 ))
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    if (ignoreObjects != null)
                    {
                        for (int i = 0; i < ignoreObjects.Length; i++)
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
                            if (hitObject == ignoreObjects[ i ])
                            {
                                continue;
                            }
                        }
                    }

                    // Log to the console that we've succeeded
                    Console.WriteLine( $"Trace succeeded.\n" +
                                $"\tStart: {rayStart}\n" +
                                $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                                $"\tHit object: {hitObject}\n" +
                                $"\tRayIgnore: {rayIgnore}\n" +
                                $"\tIgnore objects: {( ignoreObjects == null ?
                                "N/A" :
                                () =>
                                {
                                    string objects = string.Empty;

                                    foreach (object obj in ignoreObjects)
                                    {
                                        objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                    }

                                    return objects;
                                }
                                )}\n" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Console.WriteLine( $"Trace failed.\n" +
                           $"\tStart: {rayStart}\n" +
                           $"\tDirection: {entDirection} (\"{entDirection.GetID()}\")\n" +
                           $"\tHit object: N/A\n" +
                           $"\tRayIgnore: {rayIgnore}\n" +
                           $"\tIgnore objects: {( ignoreObjects == null ?
                                   "N/A" :
                                   () =>
                                   {
                                       string objects = string.Empty;

                                       foreach (object obj in ignoreObjects)
                                       {
                                           objects += $"{( obj is Entity ent ? $"{obj} (\"{ent.GetID()}\")" : obj )} ";
                                       }

                                       return objects;
                                   }
                               )}\n" );

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