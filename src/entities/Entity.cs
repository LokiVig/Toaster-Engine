using System;

using DoomNET.WTF;
using DoomNET.Resources;

namespace DoomNET.Entities;

/// <summary>
/// An entity, usually living and with health, also moving with velocities and gravity applied to it
/// </summary>
public class Entity
{
    public Vector3 position { get; set; } // This entity's current position
    private Vector3 velocity; // This entity's current velocity

    public BBox bbox { get; set; } // This entity's bounding box

    public virtual EntityTypes type { get; set; } // This entity's type, e.g. brush entity or other

    public string id { get; set; } // This entity's identifier

    public System.Numerics.Quaternion rotation { get; set; } // This entity's current rotation

    public virtual float health { get; set; } // The amount of health this entity has

    private bool alive; // Is this entity alive?

    private Entity target; // The entity this entity's targeting

    private Entity lastAttacker; // The last entity to attack this entity

    public Entity()
    {
        position = new();
        bbox = new();
        Spawn();
    }

    public Entity(Vector3 position)
    {
        this.position = position;
        bbox = new();
        Spawn();
    }

    public Entity(Vector3 position, BBox bbox)
    {
        this.position = position;
        this.bbox = bbox;
        Spawn();
    }

    /// <summary>
    /// A way to initialize this entity, default for all entities
    /// </summary>
    public void Spawn()
    {
        // Null the velocity
        velocity = new();

        // This entity is now alive
        alive = true;

        // Subscribe to the OnUpdate event
        DoomNET.OnUpdate += Update;

        // Call the OnSpawn event
        OnSpawn();
    }

    /// <summary>
    /// Things to do every frame
    /// </summary>
    protected virtual void Update()
    {

    }

    /// <summary>
    /// Handle movement, caused by velocity
    /// </summary>
    protected void HandleMovement()
    {
        // Position is affected by velocity
        position += velocity * DoomNET.deltaTime * DoomNET.deltaTime;

        // Velocity decreases with time (effectively drag)
        velocity *= (1 - 0.1f * DoomNET.deltaTime);

        // If the velocity's magnitude <= 0.1, it's effectively zero, so zero it out for the sake of ease
        if (velocity.Magnitude() <= 0.1f)
        {
            velocity = new();
        }
    }

    /// <summary>
    /// Get the current health of this entity
    /// </summary>
    public float GetHealth()
    {
        return health;
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
    public Vector3 GetPosition()
    {
        return position;
    }

    /// <summary>
    /// Gets this entity's velocity
    /// </summary>
    public Vector3 GetVelocity()
    {
        return velocity;
    }

    /// <summary>
    /// Gets this entity's bounding box
    /// </summary>
    public BBox GetBBox()
    {
        return bbox;
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
    public void SetID(string id)
    {
        this.id = id;
    }

    /// <summary>
    /// Set the target of this entity, e.g. an enemy should target the <see cref="Player"/>
    /// </summary>
    /// <param name="target">The specific entity we wish to target, 0 should always be the <see cref="Player"/></param>
    public void SetTarget(Entity target)
    {
        // Can't target a dead entity
        if (!target.IsAlive())
        {
            return;
        }

        this.target = target;
    }

    /// <summary>
    /// Set the target of this entity by an ID, e.g. an enemy should target the <see cref="Player"/>
    /// </summary>
    /// <param name="targetID">The ID of the entity we wish to target, 0 should always be the <see cref="Player"/></param>
    public void SetTarget(string targetID)
    {
        target = DoomNET.file.FindEntity(targetID);
    }

    /// <summary>
    /// Set this entity's position by a <see cref="Vector3"/>
    /// </summary>
    /// <param name="position">The new, desired position of this entity</param>
    public void SetPosition(Vector3 position)
    {
        this.position = position;
    }

    /// <summary>
    /// Set this entity's velocity by a <see cref="Vector3"/>
    /// </summary>
    /// <param name="velocity">The new, desired velocity of this entity</param>
    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    /// <summary>
    /// Main use for this is for when a brush is turned into an entity
    /// </summary>
    /// <param name="bbox">The new bounding box of this entity</param>
    public void SetBBox(BBox bbox)
    {
        this.bbox = bbox;
    }

    /// <summary>
    /// Face the current entity towards another, e.g. the player
    /// </summary>
    /// <param name="entity">The desired entity we wish to look at</param>
    public void LookAtEntity(Entity entity = null)
    {
        // Do math
    }

    /// <summary>
    /// Subtract this entity's health by the parameter and trigger related events
    /// </summary>
    /// <param name="damage">The amount of damage this entity should take</param>
    public virtual void TakeDamage(float damage, Entity source = null)
    {
        // We've been damaged by someone or something!
        // How queer! We must log this to the console immediately!!
        Console.WriteLine($"Entity \"{GetID()}\" took damage.\n" +
                          $"\tDamage: {damage}\n" +
                          $"\tSource: {(source != null ? source + $" (\"{source.GetID()}\")" : "N/A")}\n" +
                          $"\tNew health: {health - damage}\n");

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

    #region ONEVENTS
    /// <summary>
    /// Call non-input-taking event
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity</param>
    public void OnEvent(EntityEvent eEvent, Entity source = null)
    {
        switch (eEvent)
        {
            case EntityEvent.None: // Do nothing (why'd you want this?)
                break;

            case EntityEvent.Kill: // Kill this entity
                OnDeath();
                break;

            case EntityEvent.Delete: // Delete this entity
                // Delete this entity, somehow
                break;

            default: // Most likely happens when an invalid event was attempted on this entity
                return;
        }
    }

    /// <summary>
    /// Call an event that takes an integer for a value
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity</param>
    /// <param name="iValue">Value as int</param>
    public void OnEvent(EntityEvent eEvent, int iValue, Entity source = null)
    {
        switch (eEvent)
        {
            case EntityEvent.TakeDamage: // This entity should take iValue damage
                TakeDamage(iValue, source);
                break;
        }
    }

    /// <summary>
    /// Call an event that takes a float for a value
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity</param>
    /// <param name="fValue">Value as float</param>
    public void OnEvent(EntityEvent eEvent, float fValue, Entity source = null)
    {
        switch (eEvent)
        {
            case EntityEvent.TakeDamage: // This entity should take fValue damage
                TakeDamage(fValue, source);
                break;
        }
    }

    /// <summary>
    /// Call an event that takes a Vector3 for a value
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity</param>
    /// <param name="vValue">Value as Vector3</param>
    public void OnEvent(EntityEvent eEvent, Vector3 vValue, Entity source = null)
    {
        switch (eEvent)
        {
            case EntityEvent.SetPosition: // Set this entity's position according to vValue
                SetPosition(vValue);
                break;
        }
    }

    /// <summary>
    /// Call an event that takes a BBox for a value
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity</param>
    /// <param name="bValue">Value as BBox</param>
    public void OnEvent(EntityEvent eEvent, BBox bValue, Entity source = null)
    {
        switch (eEvent)
        {
            case EntityEvent.SetBBox: // Set this entity's BBox according to bValue
                SetBBox(bValue);
                break;
        }
    }
    #endregion // ONEVENTS

    /// <summary>
    /// Per entity definition of what to do when they've spawned
    /// </summary>
    protected virtual void OnSpawn()
    {
    }

    /// <summary>
    /// Things to do when this entity takes damage
    /// </summary>
    protected virtual void OnDamage()
    {
        if (health <= 0) // Is this entity now considered dead?
        {
            // Call the OnDeath event
            OnDeath();
        }
        else if (health <= -25.0f) // Should they gib?
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
        DoomNET.OnUpdate -= Update;

        // Log to the console that this entity has died!
        Console.WriteLine($"Entity {this} (\"{GetID()}\") has died.\n" +
                          $"\tLast attacker: {lastAttacker}");
    }

    /// <summary>
    /// Things to do when this entity dies a gory death
    /// </summary>
    protected virtual void OnXDeath()
    {
        // Also trigger OnDeath, but replace their sprite with a gory version
        OnDeath();
    }
}