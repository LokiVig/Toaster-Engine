using System;
using System.Linq;

using Toast.Engine.Math;
using Toast.Engine.Entities;

namespace Toast.Engine.Resources;

public struct Ray
{
    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Vector3"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Vector3 rayStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, bool logInfo = true, float rayLength = 5000 )
    {
        Vector3 rayEnd = ( rayStart + rayDirection.Normalized() ) * rayLength;

        // If we're not ignoring entities
        if ( ( rayIgnore & RayIgnore.Entities ) != 0 )
        {
            // Check every entity
            foreach ( Entity entity in EngineManager.currentScene?.GetEntities()! )
            {
                //
                // Automatically ignored entities
                //

                // Ignore tool entities
                if ( entity.type == EntityType.Tool )
                {
                    continue;
                }

                // Ignore any entities that share the same position as the rayStart
                if ( entity.GetPosition() == rayStart )
                {
                    continue;
                }

                //
                // Flag ignored entities
                //

                // Ignore brush entities
                if ( ( rayIgnore & RayIgnore.BrushEntities ) == 0 && entity is BrushEntity )
                {
                    continue;
                }

                // Ignore NPCs
                if ( ( rayIgnore & RayIgnore.NPCEntities ) == 0 && entity is NPCEntity )
                {
                    continue;
                }

                // Ignore players
                if ( ( rayIgnore & RayIgnore.Players ) == 0 && entity is PlayerEntity )
                {
                    continue;
                }

                // Are we intersecting with this entity's bounding box?
                if ( entity.GetBBox().RayIntersects( rayStart, rayEnd, rayLength ) )
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
        if ( !rayIgnore.HasFlag( RayIgnore.Brushes ) )
        {
            // Check every brush
            foreach ( Brush brush in EngineManager.currentScene?.GetBrushes()! )
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

        // Also log to console
        Log.Info( $"Trace failed.\n" +
                          $"\tStart: {rayStart}\n" +
                          $"\tDirection: {rayDirection} ({rayDirection.Normalized()})\n" +
                          $"\tHit object: N/A\n" +
                          $"\tRayIgnore: {rayIgnore}\n" +
                          $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Entity"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Entity entStart, Entity entDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, bool logInfo = true, float rayLength = 5000 )
    {
        Vector3 rayEnd = ( entStart.GetPosition() + entDirection.GetPosition().Normalized() ) * rayLength;

        // If we're not ignoring entities
        if ( ( rayIgnore & RayIgnore.Entities ) != 0 )
        {
            // Check every entity
            foreach ( Entity entity in EngineManager.currentScene?.GetEntities()! )
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
                if ( entity == entStart )
                {
                    continue;
                }

                //
                // Flag ignored entities
                //

                // Ignore brush entities
                if ( ( rayIgnore & RayIgnore.BrushEntities ) == 0 && entity is BrushEntity )
                {
                    continue;
                }

                // Ignore NPCs
                if ( ( rayIgnore & RayIgnore.NPCEntities ) == 0 && entity is NPCEntity )
                {
                    continue;
                }

                // Ignore players
                if ( ( rayIgnore & RayIgnore.Players ) == 0 && entity is PlayerEntity )
                {
                    continue;
                }

                // Are we intersecting with this entity's bounding box?
                if ( entity.GetBBox().RayIntersects( entStart.GetPosition(), rayEnd, rayLength ) )
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

                    if ( logInfo )
                    {
                        // Log to the console that we've succeeded
                    }

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if ( ( rayIgnore & RayIgnore.Brushes ) != 0 )
        {
            // Check every brush
            foreach ( Brush brush in EngineManager.currentScene?.GetBrushes()! )
            {
                // Are we intersecting with this brush's bounding box?
                if ( brush.GetBBox().RayIntersects( entStart.GetPosition(), rayEnd, rayLength ) )
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
                        Log.Info( $"Trace succeeded.\n" +
                                          $"\tStart: {entStart}\n" +
                                          $"\tDirection: {entDirection}\n" +
                                          $"\tHit object: {hitObject}\n" +
                                          $"\tRayIgnore: {rayIgnore}\n" +
                                          $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );
                    }

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Log to the console that we've failed
        Log.Info( $"Trace failed.\n" +
                          $"\tStart: {entStart}\n" +
                          $"\tDirection: {entDirection}\n" +
                          $"\tHit object: {hitObject}\n" +
                          $"\tRayIgnore: {rayIgnore}\n" +
                          $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );

        // We didn't hit anything, so it's false
        return false;
    }

    /// <summary>
    /// Trace a ray from a specified starting position (<see cref="Vector3"/>) to a direction (<see cref="Entity"/>), 
    /// with <see cref="RayIgnore"/> flags, specific <see langword="object"/>(s) to ignore, and lengths (<see langword="float"/>) 
    /// </summary>
    /// <returns><see langword="true"/> and the <see langword="object"/> we hit, <see langword="false"/> and <see langword="null"/> if nothing was hit.</returns>
    public static bool Trace( Vector3 rayStart, Entity entDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, float rayLength = 5000 )
    {
        Vector3 rayEnd = ( rayStart + entDirection.GetPosition().Normalized() ) * rayLength;

        // If we're not ignoring entities
        if ( ( rayIgnore & RayIgnore.Entities ) != 0 )
        {
            // Check every entity
            foreach ( Entity entity in EngineManager.currentScene?.GetEntities()! )
            {
                //
                // Automatically ignored entities
                //

                // Ignore tool entities
                if ( entity.type == EntityType.Tool )
                {
                    continue;
                }

                // Ignore any entities that share the same position as the rayStart
                if ( entity.GetPosition() == rayStart )
                {
                    continue;
                }

                //
                // Flag ignored entities
                //

                // Ignore brush entities
                if ( ( rayIgnore & RayIgnore.BrushEntities ) == 0 && entity is BrushEntity )
                {
                    continue;
                }

                // Ignore NPCs
                if ( ( rayIgnore & RayIgnore.NPCEntities ) == 0 && entity is NPCEntity )
                {
                    continue;
                }

                // Ignore players
                if ( ( rayIgnore & RayIgnore.Players ) == 0 && entity is PlayerEntity )
                {
                    continue;
                }

                // Are we intersecting with this entity's bounding box?
                if ( entity.GetBBox().RayIntersects( rayStart, rayEnd, rayLength ) )
                {
                    // We've hit this entity!
                    // The hitObject is now this entity, and we're returning true
                    hitObject = entity;

                    if ( ignoredObjects != null )
                    {
                        for ( int i = 0; i < ignoredObjects.Length; i++ )
                        {
                            // Uh-oh! The hit entity is an object we wish to ignore, continue in the foreach
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
                    Log.Info( $"Trace succeeded.\n" +
                                      $"\tStart: {rayStart}\n" +
                                      $"\tDirection: {entDirection}\n" +
                                      $"\tHit object: {hitObject}\n" +
                                      $"\tRayIgnore: {rayIgnore}\n" +
                                      $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );

                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if ( ( rayIgnore & RayIgnore.Brushes ) != 0 )
        {
            // Check every brush
            foreach ( Brush brush in EngineManager.currentScene?.GetBrushes()! )
            {
                // Are we intersecting with this brush's bounding box?
                if ( brush.GetBBox().RayIntersects( rayStart, rayEnd, 5000 ) )
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

                    // Log to the console that we've succeeded
                    Log.Info( $"Trace succeeded.\n" +
                                      $"\tStart: {rayStart}\n" +
                                      $"\tDirection: {entDirection}\n" +
                                      $"\tHit object: {hitObject}\n" +
                                      $"\tRayIgnore: {rayIgnore}\n" +
                                      $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Log.Info( $"Trace failed.\n" +
                          $"\tStart: {rayStart}\n" +
                          $"\tDirection: {entDirection}\n" +
                          $"\tHit object: N/A\n" +
                          $"\tRayIgnore: {rayIgnore}\n" +
                          $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );

        // We didn't hit anything, so it's false
        return false;
    }

    public static bool Trace( Entity entStart, Vector3 rayDirection, out object hitObject, RayIgnore rayIgnore = RayIgnore.None, object[] ignoredObjects = null, float rayLength = 5000 )
    {
        Vector3 rayEnd = ( entStart.GetPosition() + rayDirection.Normalized() ) * rayLength;

        // If we're not ignoring entities
        if ( ( rayIgnore & RayIgnore.Entities ) != 0 )
        {
            // Check every entity
            foreach ( Entity entity in EngineManager.currentScene?.GetEntities()! )
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
                if ( entity == entStart )
                {
                    continue;
                }

                //
                // Flag ignored entities
                //

                // Ignore brush entities
                if ( ( rayIgnore & RayIgnore.BrushEntities ) == 0 && entity is BrushEntity )
                {
                    continue;
                }

                // Ignore NPCs
                if ( ( rayIgnore & RayIgnore.NPCEntities ) == 0 && entity is NPCEntity )
                {
                    continue;
                }

                // Ignore players
                if ( ( rayIgnore & RayIgnore.Players ) == 0 && entity is PlayerEntity )
                {
                    continue;
                }

                // Are we intersecting with this entity's bounding box?
                if ( entity.GetBBox().RayIntersects( entStart.GetPosition(), rayEnd, rayLength ) )
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
                    LogSuccessfulTrace()


                    return true;
                }
            }
        }

        // If we're not ignoring brushes
        if ( ( rayIgnore & RayIgnore.Brushes ) != 0 )
        {
            // Check every brush
            foreach ( Brush brush in EngineManager.currentScene?.GetBrushes()! )
            {
                // Are we intersecting with this brush's bounding box?
                if ( brush.GetBBox().RayIntersects( entStart.GetPosition(), rayEnd, rayLength ) )
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

                    // Log to the console that we've succeeded
                    Log.Info( $"Trace succeeded.\n" +
                                      $"\tStart: {entStart}\n" +
                                      $"\tDirection: {rayDirection} ({rayDirection.Normalized()})\n" +
                                      $"\tHit object: {hitObject}\n" +
                                      $"\tRayIgnore: {rayIgnore}\n" +
                                      $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );

                    return true;
                }
            }
        }

        // We hit nothing!
        // The hitObject is therefore null
        hitObject = null;

        // Also log to console
        Log.Info( $"Trace failed.\n" +
                          $"\tStart: {entStart}\n" +
                          $"\tDirection: {rayDirection} ({rayDirection.Normalized()})\n" +
                          $"\tHit object: N/A\n" +
                          $"\tRayIgnore: {rayIgnore}\n" +
                          $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );

        // We didn't hit anything, so it's false
        return false;
    }

    private static void LogTrace( Vector3 rayStart, Vector3 rayDirection, object hitObject, RayIgnore rayIgnore, object[] ignoredObjects )
    {
        Log.Info( $"Trace {( hitObject != null ? "succeeded" : "failed" )}.\n" +
                          $"\tStart: {rayStart}\n" +
                          $"\tDirection: {rayDirection}\n" +
                          $"\tHit object: {hitObject ?? "N/A"}\n" +
                          $"\tRayIgnore: {rayIgnore}\n" +
                          $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );
    }

    private static void LogTrace( Entity entStart, Entity entDirection, object hitObject, RayIgnore rayIgnore, object[] ignoredObjects, bool successful )
    {
        Log.Info( $"Trace {( hitObject != null ? "succeeded" : "failed" )}.\n" +
                          $"\tStart: {entStart}\n" +
                          $"\tDirection: {entDirection}\n" +
                          $"\tHit object: {hitObject ?? "N/A"}\n" +
                          $"\tRayIgnore: {rayIgnore}\n" +
                          $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );
    }

    private static void LogTrace( Vector3 rayStart, Entity entDirection, object hitObject, RayIgnore rayIgnore, object[] ignoredObjects, bool successful )
    {
        Log.Info( $"Trace {( hitObject != null ? "succeeded" : "failed" )}.\n" +
                          $"\tStart: {rayStart}\n" +
                          $"\tDirection: {entDirection}\n" +
                          $"\tHit object: {hitObject ?? "N/A"}\n" +
                          $"\tRayIgnore: {rayIgnore}\n" +
                          $"\tIgnored objects: {( ignoredObjects == null ? "N/A" : $"\n\t\t{string.Join( "\n\t\t", ignoredObjects.Select( obj => obj.ToString() ) )}" )}" );
    }

    private static void LogTrace( Entity entStart, Vector3 rayDirection, object hitObject, RayIgnore rayIgnore, object[] ignoredObjects, bool successful )
    {
        Log.Info( $"Trace {( hitObject != null ? "succeeded" : "failed" )}.\n" +
                          $"\tStart: {entStart}\n" +
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