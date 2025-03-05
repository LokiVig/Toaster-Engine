using System;
using System.Numerics;
using System.Collections.Generic;

using Toast.Engine.Rendering;
using Toast.Engine.Resources;
using Toast.Engine.Entities.Tools;

namespace Toast.Engine.Entities;

/// <summary>
/// An entity.<br/>
/// Something that can, for example, be seen, interacted with, killed, or other, should be defined as an entity.<br/>
/// <br/>
/// <list type="table">
///     <listheader>
///         <term>Example Entities</term>    
///     </listheader>
///     
///     <item>Brush Entities</item>
///     <item>NPCs</item>
/// </list>
/// </summary>
public class Entity
{
    public Vector3 position; // This entity's current position
    public Quaternion rotation; // This entity's current rotation
    public BBox bbox; // This entity's bounding box

    public string id; // This entity's identifier
    public Model model; // This entity's visually pleasing model

    public virtual EntityType type { get; private set; } = EntityType.None; // This entity's type, e.g. player / NPC
    public virtual float maxHealth { get; private set; } // This entity's max health

    protected float health; // This entity's health
    protected Vector3 velocity; // This entity's current velocity
    protected bool alive; // Is this entity alive?
    protected Entity target; // The entity this entity's targeting
    protected Entity lastAttacker; // The last entity to attack this entity

    private const float MAX_VELOCITY = 225; // TODO: Calculate this (mass, therefore terminal velocity) from the entity's BBox! Bad constant!

    public Entity()
    {
        ErrorCheck(); // Check for errors
    }

    public Entity( Vector3 position )
    {
        SetPosition( position ); // Set our position
        ErrorCheck(); // Check for errors
    }

    /// <summary>
    /// Method to check for commonplace errors.
    /// </summary>
    private void ErrorCheck()
    {
        // Make sure our type is valid
        if ( type == EntityType.None )
        {
            Log.Error<NullReferenceException>( $"Entity {this} is of EntityType None, meaning this is an entity that hasn't properly been initialized, read, or otherwise!" );
        }
    }

    /// <summary>
    /// A way to initialize this entity, default for all entities
    /// </summary>
    public void Spawn()
    {
        // Null the velocity
        velocity = Vector3.Zero;

        // Set our health to our maxHealth variable
        health = maxHealth;

        // This entity is now alive
        alive = true;

        // Subscribe to the OnUpdate event
        EngineManager.OnUpdate += Update;

        // Call the OnSpawn event
        OnSpawn();
    }

    /// <summary>
    /// Things to do every frame
    /// </summary>
    protected virtual void Update()
    {
        // Can't have bounding boxes where the maxs have a less value than mins, or vice versa
        if ( bbox.mins.X >= bbox.maxs.X ||
             bbox.mins.Y >= bbox.maxs.Y ||
             bbox.mins.Z >= bbox.maxs.Z )
        {
            Log.Error<ArithmeticException>( $"{this}'s bound boxes are mismatched! ({bbox})" );
        }
    }

    /// <summary>
    /// Creates entity-specific console commands.
    /// </summary>
    protected virtual void CreateCommands()
    {

    }

    /// <summary>
    /// Creates entity-specific keybinds.
    /// </summary>
    protected virtual void CreateKeybinds()
    {

    }

    /// <summary>
    /// Applies velocity to this entity.
    /// </summary>
    protected void ApplyVelocity()
    {
        // Clamp velocity between the min and max values, and normalize it
        velocity = Vector3.Clamp( velocity, new Vector3(-MAX_VELOCITY), new Vector3(MAX_VELOCITY) );
        Vector3.Normalize( velocity );

        // Position is affected by velocity
        position += velocity * EngineManager.deltaTime;

        // Velocity decreases with time (effectively drag)
        velocity *= ( 1 - 0.25f ) * EngineManager.deltaTime;

        // If the velocity's magnitude <= 0.01, it's effectively zero, so zero it out for the sake of ease
        if ( velocity.Length() <= 0.01f )
        {
            velocity = Vector3.Zero;
        }
    }

    /// <summary>
    /// Get the current health of this entity
    /// </summary>
    public ref float GetHealth()
    {
        return ref health;
    }

    /// <summary>
    /// Get the ID of this entity
    /// </summary>
    public string GetID()
    {
        return id;
    }

    /// <summary>
    /// Gets this entity's position
    /// </summary>
    public ref Vector3 GetPosition()
    {
        return ref position;
    }

    /// <summary>
    /// Gets this entity's velocity
    /// </summary>
    public ref Vector3 GetVelocity()
    {
        return ref velocity;
    }

    /// <summary>
    /// Gets this entity's rotation
    /// </summary>
    public ref Quaternion GetRotation()
    {
        return ref rotation;
    }

    /// <summary>
    /// Gets this entity's bounding box
    /// </summary>
    public ref BBox GetBBox()
    {
        return ref bbox;
    }

    /// <summary>
    /// Is this entity alive?
    /// </summary>
    /// <returns><see langword="true"/> if alive, <see langword="false"/> if dead</returns>
    public bool IsAlive()
    {
        return alive;
    }

    /// <summary>
    /// Sets this entity's ID
    /// </summary>
    public void SetID( string id )
    {
        this.id = id;
    }

    /// <summary>
    /// Set the target of this entity, e.g. an enemy should target the <see cref="PlayerEntity"/>
    /// </summary>
    /// <param name="target">The specific entity we wish to target, 0 should always be the <see cref="PlayerEntity"/></param>
    public void SetTarget( Entity target )
    {
        // We can't check if a null entity is dead or not, 
        // so we're handling a null target before such
        if ( target == null )
        {
            this.target = null;
            return;
        }

        // Can't target a dead entity
        if ( !target.IsAlive() )
        {
            this.target = null;
            return;
        }

        this.target = target;
    }

    /// <summary>
    /// Set the target of this entity by an ID, e.g. an enemy should target the <see cref="PlayerEntity"/>
    /// </summary>
    /// <param name="targetID">The ID of the entity we wish to target, "player" should always be a <see cref="PlayerEntity"/></param>
    public void SetTarget( string targetID )
    {
        target = EngineManager.currentScene?.FindEntity( targetID );
    }

    /// <summary>
    /// Set this entity's position by a <see cref="Vector3"/>
    /// </summary>
    /// <param name="position">The new, desired position of this entity</param>
    public void SetPosition( Vector3 position )
    {
        this.position = position;
    }

    /// <summary>
    /// Set this entity's velocity by a <see cref="Vector3"/>
    /// </summary>
    /// <param name="velocity">The new, desired velocity of this entity</param>
    public void SetVelocity( Vector3 velocity )
    {
        this.velocity = velocity;
    }

    /// <summary>
    /// Set this entity's rotation from a <see cref="Quaternion"/>
    /// </summary>
    /// <param name="rotation">The new, desired rotation of this entity</param>
    public void SetRotation( Quaternion rotation )
    {
        this.rotation = rotation;
    }

    /// <summary>
    /// Main use for this is for when a brush is turned into an entity
    /// </summary>
    /// <param name="bbox">The new bounding box of this entity</param>
    public void SetBBox( BBox bbox )
    {
        this.bbox = bbox;
    }

    /// <summary>
    /// Face the current entity towards another, e.g. the player
    /// </summary>
    /// <param name="entity">The desired entity we wish to look at</param>
    public void LookAtEntity( Entity entity )
    {
        // Do math
    }

    /// <summary>
    /// Adds the argument <see cref="Vector3"/> value to the velocity.
    /// </summary>
    /// <param name="force">The amount of force to add to this entity's velocity.</param>
    /// <param name="multiplier">Multiplies the input vector by this value.</param>
    public void AddForce( Vector3 force, float multiplier = 5 )
    {
        velocity += force * multiplier;
    }

    /// <summary>
    /// Subtract this entity's health by the parameter and trigger related events
    /// </summary>
    /// <param name="damage">The amount of damage this entity should take</param>
    /// <param name="source">The source entity of the damage</param>
    public virtual void TakeDamage( float damage, Entity source = null )
    {
        // We've been damaged by someone or something!
        // How queer! We must log this to the console immediately!!
        Log.Info( $"Entity {this} took damage.\n" +
                          $"\tDamage: {damage}\n" +
                          $"\tSource: {( source != null ? source : "N/A" )}\n" +
                          $"\tNew health: {health - damage}" );

        //
        // I guess we're taking damage now
        //

        // Set the last attacker variable appropriately
        lastAttacker = source;

        // Lower this entity's health by the set amount of damage
        health -= damage;

        // This entity has taken damage! Call the relevant event
        OnDamage();
    }

    /// <summary>
    /// Generate an ID for this entity.
    /// </summary>
    public void GenerateID()
    {
        // Easier to use variable for the list of entities in the current scene
        List<Entity> entities = EngineManager.currentScene?.GetEntities();

        // For every entity...
        for ( int i = 0; i < entities?.Count; i++ )
        {
            // If the entity we're currently checking is us
            if ( entities[i] == this )
            {
                // If we are a player...
                if ( this is PlayerEntity )
                {
                    SetID( "player" ); // We're lucky! We have that special designation on us now
                    return; // We don't want to overwrite that special ID
                }

                // We're just some regular joe, set our entity ID appropriately
                SetID( $"entity {i}" );
            }
        }
    }

    #region ONEVENTS
    /// <summary>
    /// Call parameterless event
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity</param>
    /// <param name="source">The entity that caused this event.</param>
    public void OnEvent( EntityEvent eEvent, Entity source = null )
    {
        switch ( eEvent )
        {
            case EntityEvent.None: // Do nothing (why'd you want this?)
                break;

            case EntityEvent.Kill: // Kill this entity
                OnDeath(); // Instantly just calls the OnDeath method
                break;

            case EntityEvent.Delete: // Delete this entity
                Remove(); // Removes us from everything
                break;

            case EntityEvent.PlaySound: // As a SoundEntity entity, play our audio
                ( this as SoundEntity )?.PlaySound();
                break;

            case EntityEvent.StopSound: // As a SoundEntity entity, stop our audio
                ( this as SoundEntity )?.StopSound();
                break;

            default: // Most likely happens when an invalid event was attempted on this entity
                return;
        }
    }

    /// <summary>
    /// Call an event that takes an <see langword="int"/> for a value.
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity.</param>
    /// <param name="iValue">Value as <see langword="int"/>.</param>
    /// <param name="source">The entity that caused this event.</param>
    public void OnEvent( EntityEvent eEvent, int iValue, Entity source = null )
    {
        switch ( eEvent )
        {
            case EntityEvent.TakeDamage: // This entity should take iValue damage
                TakeDamage( iValue, source );
                break;
        }
    }

    /// <summary>
    /// Call an event that takes a <see langword="float"/> for a value.
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity.</param>
    /// <param name="fValue">Value as <see langword="float"/>.</param>
    /// <param name="source">The entity that caused this event.</param>
    public void OnEvent( EntityEvent eEvent, float fValue, Entity source = null )
    {
        switch ( eEvent )
        {
            case EntityEvent.TakeDamage: // This entity should take fValue damage
                TakeDamage( fValue, source );
                break;
        }
    }

    /// <summary>
    /// Call an event hat takes a <see langword="bool"/> for a value.
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity.</param>
    /// <param name="bValue">Value as <see langword="bool"/>.</param>
    /// <param name="source">The entity that caused this event.</param>
    public void OnEvent( EntityEvent eEvent, bool bValue, Entity source = null )
    {
        // switch (eEvent)
        // {
        //     default:
        //         break;
        // }
    }

    /// <summary>
    /// Call an event that takes a <see cref="Vector3"/> for a value.
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity.</param>
    /// <param name="vValue">Value as <see cref="Vector3"/>.</param>
    /// <param name="source">The entity that caused this event.</param>
    public void OnEvent( EntityEvent eEvent, Vector3 vValue, Entity source = null )
    {
        switch ( eEvent )
        {
            case EntityEvent.SetPosition: // Set this entity's position according to vValue
                SetPosition( vValue );
                break;
        }
    }

    /// <summary>
    /// Call an event that takes a <see cref="Quaternion"/> for a value.
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity.</param>
    /// <param name="qValue">Value as <see cref="Quaternion"/>.</param>
    /// <param name="source">The entity that caused this event.</param>
    public void OnEvent( EntityEvent eEvent, Quaternion qValue, Entity source = null )
    {
        switch ( eEvent )
        {
            case EntityEvent.SetRotation:
                SetRotation( qValue );
                break;
        }
    }

    /// <summary>
    /// Call an event that takes a <see cref="Entity"/> for a value.
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity.</param>
    /// <param name="eValue">Value as <see cref="Entity"/>.</param>
    /// <param name="source">The entity that caused this event.</param>
    public void OnEvent( EntityEvent eEvent, Entity eValue, Entity source = null )
    {
        switch ( eEvent )
        {
            case EntityEvent.SpawnEntity:
                ( this as EntitySpawner )?.SpawnEntity( eValue );
                break;
        }
    }

    /// <summary>
    /// Call an event that takes a <see cref="BBox"/> for a value.
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity.</param>
    /// <param name="bbValue">Value as <see cref="BBox"/>.</param>
    /// <param name="source">The entity that caused this event.</param>
    public void OnEvent( EntityEvent eEvent, BBox bbValue, Entity source = null )
    {
        switch ( eEvent )
        {
            case EntityEvent.SetBBox: // Set this entity's BBox according to bValue
                SetBBox( bbValue );
                break;
        }
    }
    #endregion // ONEVENTS

    /// <summary>
    /// Per entity definition of what to do when they've spawned
    /// </summary>
    protected virtual void OnSpawn()
    {
        CreateCommands(); // Create our commands
        CreateKeybinds(); // Create our keybinds
    }

    /// <summary>
    /// Things to do when this entity takes damage
    /// </summary>
    protected virtual void OnDamage()
    {
        if ( health <= 0 ) // Is this entity now considered dead?
        {
            // Call the OnDeath event
            OnDeath();
        }
        else if ( health <= -25.0f ) // Should they gib?
        {
            // Call the OnXDeath event
            OnXDeath();
        }
    }

    /// <summary>
    /// Things to do when this entity dies
    /// </summary>
    protected virtual void OnDeath()
    {
        // This entity is no longer alive
        alive = false;

        // Remove this entity from the update list
        EngineManager.OnUpdate -= Update;

        // Log to the console that this entity has died!
        Log.Info( $"Entity {this} has died.\n" +
                          $"\tLast attacker: {(lastAttacker != null ? lastAttacker : "N/A")}" );
    }

    /// <summary>
    /// Things to do when this entity dies a gory death
    /// </summary>
    protected virtual void OnXDeath()
    {
        // Also trigger OnDeath, but replace their sprite with a gory version
        OnDeath();
    }

    /// <summary>
    /// Deletes a specified entity from the scene and from existence.
    /// </summary>
    /// <param name="ent">The entity we wish to delete.</param>
    public static void Delete( Entity ent )
    {
        EngineManager.OnUpdate -= ent.Update;
        EngineManager.currentScene?.RemoveEntity( ent );
        ent = null;
    }

    /// <summary>
    /// Removes this specific entity.<br/>
    /// Effectively just calls <see cref="Delete(Entity)"/> with <see langword="this"/> as its argument.
    /// </summary>
    public void Remove()
    {
        Delete( this );
    }

    public override string ToString()
    {
        return $"{GetType().Name} (\"{GetID() ?? "N/A"}\")";
    }
}