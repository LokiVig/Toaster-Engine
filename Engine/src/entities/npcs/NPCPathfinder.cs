﻿using System;

using Toast.Engine.Resources;

namespace Toast.Engine.Entities.NPC;

/// <summary>
/// Gets the scene's brushes, finds out which are walkable, and paths to any target entity
/// the entity may be targeting on using the A* algorithm.
/// </summary>
public class NPCPathfinder
{
    public NPCEntity parent;

    public NPCPathfinder( NPCEntity parent )
    {
        if ( parent == null )
        {
            Log.Error<NullReferenceException>( "NPCPathfinder parent variable is null!" );
        }

        this.parent = parent;
    }

    /// <summary>
    /// Utilize the A* algorithm to pathfind to the parent's target.
    /// </summary>
    /// <param name="updateFreq">Defines the update frequency in milliseconds (deltatime assisted). Default (0) means every Update call.</param>
    /// <returns>A specific result (<see cref="PathfindResult"/>) that the algorithm's come up with.</returns>
    public PathfindResult PathfindToTarget(float updateFreq = 0f)
    {
        return PathfindResult.Unknown;
    }
}

public enum PathfindResult
{
    Unknown,
    Failed,
    Successful
}