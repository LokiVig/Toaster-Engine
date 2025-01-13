using System;

namespace Toast.Engine.Entities.NPC;

/// <summary>
/// Gets the scene's brushes, finds out which are walkable, and paths to any target entity
/// the entity may be focusing on using the A* algorithm.
/// </summary>
public class NPCPathfinder
{
    public NPCEntity parent;

    public NPCPathfinder( NPCEntity _parent )
    {
        if ( _parent == null )
        {
            EngineProgram.DoError( "NPCPathfinder _parent variable is null!", new NullReferenceException() );
        }

        parent = _parent;
    }

    /// <summary>
    /// Utilize the A* algorithm to pathfind to the parent's target.
    /// </summary>
    /// <param name="updateFreq">Defines the update frequency in milliseconds (deltatime assisted). Default (0) means every Update call.</param>
    /// <returns>Returns a specific result (<see cref="PathfindResult"/>) that the algorithm's come up with.</returns>
    public PathfindResult PathfindToTarget(float updateFreq = 0f)
    {
        return PathfindResult.Unknown;
    }
}

public enum PathfindResult
{
    Successful,
    Failed,
    Unknown
}