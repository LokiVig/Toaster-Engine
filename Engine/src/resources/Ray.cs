using System;
using System.Linq;
using System.Numerics;

using Toast.Engine.Entities;

namespace Toast.Engine.Resources;

public struct Ray
{
    public Vector3 start;
    public Vector3 direction;
    public object hit;

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Vector3 rayStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, bool logInfo = true, float rayLength = 5000 )
    {
        Vector3 rayEnd = ( rayStart + Vector3.Normalize( rayDirection ) ) * rayLength;

        // If we're not ignoring entities
        if ( ( rayIgnore & RayIgnore.Entities ) == 0 )
        {
            // Check every entity
            foreach ( Entity entity in EngineManager.currentScene?.GetEntities() )
            {
                //
                // Automatically ignored entities
                //

                // Ignore tool entities
                if ( entity.type == EntityType.Tool )
                {
                    continue;
                }

                // Ignore the ray source, aka the entity that we're tracing from
                if ( entity.GetPosition() == rayStart )
                {
                    continue;
                }

                //
                // Flag ignored entities
                //

                // Ignore brush entities
                if ( ( rayIgnore & RayIgnore.BrushEntities ) != 0 )
                {
                    if ( entity.type == EntityType.Brush )
                    {
                        continue;
                    }
                }

                // Ignore NPCs
                if ( ( rayIgnore & RayIgnore.NPCEntities ) != 0 )
                {
                    if ( entity.type == EntityType.NPC )
                    {
                        continue;
                    }
                }

                // Ignore players
                if ( ( rayIgnore & RayIgnore.Players ) != 0 )
                {
                    if ( entity.type == EntityType.Player )
                    {
                        continue;
                    }
                }

                // Are we intersecting with this entity's bounding box?
                if ( entity.GetBoundingBox().RayIntersects( rayStart, rayEnd, rayLength ) )
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    if ( ignoredObjects != null )
                    {
                        for ( int i = 0; i < ignoredObjects.Length; i++ )
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore!
                            if ( hitObject == ignoredObjects[i] )
                            {
                                hitObject = null;
                                break;
                            }
                        }

                        // If we've confirmed that the entity we hit is null, try the next entity
                        if ( hitObject == null )
                        {
                            continue;
                        }
                    }

                    // Log to the console that we've succeeded
                    if ( logInfo )
                    {
                        LogTrace( rayStart, rayDirection, hitObject, rayIgnore, ignoredObjects );
                    }

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if ( ( rayIgnore & RayIgnore.Brushes ) == 0 )
        {
            // Check every brush
            foreach ( Brush brush in EngineManager.currentScene?.GetBrushes() )
            {
                // Are we intersecting with this brush's bounding box?
                if ( brush.GetBBox().RayIntersects( rayStart, rayEnd, rayLength ) )
                {
                    // We've hit this brush!
                    // The hitObject is now this brush, and we're returning true
                    hitObject = brush;

                    if ( ignoredObjects != null )
                    {
                        for ( int i = 0; i < ignoredObjects.Length; i++ )
                        {
                            // Uh-oh! The hit brush is an object we wish to ignore!
                            if ( hitObject == ignoredObjects[i] )
                            {
                                hitObject = null;
                                break;
                            }
                        }

                        // If we've confirmed that the brush we hit is null, try the next brush
                        if ( hitObject == null )
                        {
                            continue;
                        }
                    }

                    if ( logInfo )
                    {
                        // Log to the console that we've succeeded
                        LogTrace( rayStart, rayDirection, hitObject, rayIgnore, ignoredObjects );
                    }

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        if ( logInfo )
        {
            // Also log to console
            LogTrace( rayStart, rayDirection, hitObject, rayIgnore, ignoredObjects );
        }

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>),
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>)
    /// </summary>
    /// <returns>A new ray with the specified start and direction, and potentially with the hit object, if hit.</returns>
    public static Ray Trace( Vector3 rayStart, Vector3 rayDirection, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, bool logInfo = true, float rayLength = 5000 )
    {
        Ray result = new Ray()
        {
            start = rayStart,
            direction = rayDirection
        };

        Trace( rayStart, rayDirection, out object hitObject, rayIgnore, ignoredObjects, logInfo, rayLength );
        result.hit = hitObject;

        return result;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Entity"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Entity entStart, Entity entDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, bool logInfo = true, float rayLength = 5000 )
    {
        // Return the default trace with the positions of the specified entities
        return Trace( entStart.GetPosition(), entDirection.GetPosition(), out hitObject, rayIgnore, ignoredObjects, logInfo, rayLength );
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Entity"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns>A new ray with the specified start and direction, and potentially with the hit object, if hit.</returns>
    public static Ray Trace( Entity entStart, Entity entDirection, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, bool logInfo = true, float length = 5000 )
    {
        return Trace( entStart.GetPosition(), entDirection.GetPosition(), rayIgnore, ignoredObjects, logInfo, length );
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Vector3 rayStart, Entity entDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, bool logInfo = true, float rayLength = 5000 )
    {
        // Return the default trace with our rayStart argument, and the position of our entity we're aiming towards
        return Trace( rayStart, entDirection.GetPosition(), out hitObject, rayIgnore, ignoredObjects, logInfo, rayLength );
    }
    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns>A new ray with the specified start and direction, and potentially with the hit object, if hit.</returns>
    public static Ray Trace( Vector3 rayStart, Entity entDirection, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, bool logInfo = true, float rayLength = 5000 )
    {
        return Trace( rayStart, entDirection.GetPosition(), rayIgnore, ignoredObjects, logInfo, rayLength );
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Entity"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Entity entStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, bool logInfo = true, float rayLength = 5000 )
    {
        // Return the default trace with the position of our specified start entity, and the specified direction in Vector3'ness
        return Trace( entStart.GetPosition(), rayDirection, out hitObject, rayIgnore, ignoredObjects, logInfo, rayLength );
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Entity"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns>A new ray with the specified start and direction, and potentially with the hit object, if hit.</returns>
    public static Ray Trace( Entity entStart, Vector3 rayDirection, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, bool logInfo = true, float rayLength = 5000 )
    {
        return Trace( entStart.GetPosition(), rayDirection, rayIgnore, ignoredObjects, logInfo, rayLength );
    }

    /// <summary>
    /// Logs a <see cref="Trace(Vector3, Vector3, out object, RayIgnore, object[], bool, float)"/>, automatically determining if it was
    /// successful or not.
    /// </summary>
    private static void LogTrace( Vector3 rayStart, Vector3 rayDirection, object hitObject, RayIgnore rayIgnore, object[] ignoredObjects )
    {
        Log.Info( $"Trace {( hitObject != null ? "succeeded" : "failed" )}.\n" +
                          $"\tStart: {rayStart}\n" +
                          $"\tDirection: {rayDirection}\n" +
                          $"\tHit object: {hitObject ?? "N/A"}\n" +
                          $"\tRayIgnore: {rayIgnore}\n" +
                          $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );
    }
}

[Flags]
public enum RayIgnore
{
    None = 1 << 0, // Hit everything
    Brushes = 1 << 1, // Ignore brushes
    Entities = 1 << 2, // Ignore entities
    BrushEntities = 1 << 3, // Ignore brush entities
    NPCEntities = 1 << 4, // Ignore NPC entities
    Players = 1 << 5, // Ignore players
}