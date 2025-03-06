using System;
using System.Numerics;

using Toast.Engine.Resources;
using Toast.Engine.Entities.NPC;

namespace Toast.Engine.Entities;

public class NPCEntity : Entity
{
    public override EntityType type => EntityType.NPC; // This entity is of type NPC

    protected virtual Type[] hateList { get; set; }
    protected virtual Type[] adoreList { get; set; }

    private NPCPathfinder pathfinder;

    public NPCEntity() { }

    public NPCEntity( Vector3 position ) : base( position ) { }

    public NPCEntity( Entity parent ) : base( parent ) { }

    public NPCEntity( string parent ) : base( parent ) { }

    public NPCEntity( Entity parent, Vector3 position ) : base( parent, position ) { }

    public NPCEntity( string parent, Vector3 position ) : base( parent, position ) { }

    protected override void OnSpawn()
    {
        base.OnSpawn();

        pathfinder = new NPCPathfinder( this );
    }

    protected override void Update()
    {
        base.Update();

        // If we have a target to walk towards...
        if ( target != null )
        {
            // Attempt to pathfind to the target!
            PathfindResult result = pathfinder.PathfindToTarget();

            // If the pathfinder failed, or returned unknown...
            if ( result == PathfindResult.Failed || result == PathfindResult.Unknown )
            {
                // Lose the target
                SetTarget( (Entity)null );
            }

            // Make this entity look at its target
            LookAtEntity( target );
        }
        else // Otherwise...
        {
            // Trace rays out to try and find entities we hate or adore
            //LookForTarget(); // This is computationally expensive! Optimize, fix, whatever the hell
        }
    }

    /// <summary>
    /// Trace out rays horizontally to check for entities we can target.<br/>
    /// For the entity to be targeted its type needs to be in either the <see cref="hateList"/> or <see cref="adoreList"/>.
    /// </summary>
    private void LookForTarget()
    {
        // Do a horizontal ray check (don't look up or down) for any entities in our vision
        for ( int i = 0; i < 50; i++ )
        {
            // Do a ray trace to see whether or not we can find a target
            // Ignores brushes and brush entities, we should never be able to target them
            if ( Ray.Trace( this, /* Forward */ Vector3.One, out object hitObject, RayIgnore.Brushes & RayIgnore.BrushEntities, logInfo: false ) )
            {
                // Check our adore list for if the entity we hust hit should be followed
                foreach ( Type type in adoreList )
                {
                    if ( hitObject.GetType() == type )
                    {
                        SetTarget( hitObject as Entity );
                    }
                }

                // Check our hated list for if the entity we just hit should be targeted
                foreach ( Type type in hateList )
                {
                    if ( hitObject.GetType() == type )
                    {
                        SetTarget( hitObject as Entity );
                    }
                }

                // Otherwise, just assume we're neutral, or just didn't find any entity
                continue;
            }
        }
    }
}