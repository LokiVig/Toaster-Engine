using System;

using DoomNET.Resources;
using DoomNET.WTF;

namespace DoomNET.Entities;

/// <summary>
/// An entity, usually living and with health, also moving
/// </summary>
public class Entity
{
    private Vector3 position { get; set; } // This entity's current position
    private Vector3 velocity; // This entity's current velocity

    private BBox bbox { get; set; } // This entity's bounding box

    protected virtual EntityTypes type { get; set; } // This entity's type, e.g. brush entity or other

    private string id { get; set; }

    private System.Numerics.Quaternion rotation { get; set; } // This entity's current rotation

    protected virtual float health { get; set; } // The amount of health this entity has
    
    private bool alive; // Is this entity alive?

    private Entity target; // The entity this entity's targeting

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

    // Things to do every frame
    protected virtual void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        position += velocity * DoomNET.deltaTime;

        Vector3 drag = 0.01f * DoomNET.deltaTime * velocity.Normalized();
        velocity -= drag;

        if (velocity.Magnitude() < 0.001f)
        {
            velocity = new();
        }

        if (position >= 8196)
        {
            throw new IndexOutOfRangeException($"Entity {id} out of valid area! ({position.ToString()})");
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
    public void TakeDamage(float damage)
    {
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
    public void OnEvent(EntityEvent eEvent)
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
    public void OnEvent(EntityEvent eEvent, int iValue)
    {
        switch (eEvent)
        {
            case EntityEvent.TakeDamage: // This entity should take iValue damage
                TakeDamage(iValue);
                break;
        }
    }

    /// <summary>
    /// Call an event that takes a float for a value
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity</param>
    /// <param name="fValue">Value as float</param>
    public void OnEvent(EntityEvent eEvent, float fValue)
    {
        switch (eEvent)
        {
            case EntityEvent.TakeDamage: // This entity should take fValue damage
                TakeDamage(fValue);
                break;
        }
    }

    /// <summary>
    /// Call an event that takes a Vector3 for a value
    /// </summary>
    /// <param name="eEvent">Desired event to do to this entity</param>
    /// <param name="vValue">Value as Vector3</param>
    public void OnEvent(EntityEvent eEvent, Vector3 vValue)
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
    public void OnEvent(EntityEvent eEvent, BBox bValue)
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
        if (health < 0) // Is this entity now considered dead?
        {
            // Call the OnDeath event
            OnDeath();
        }
        else if (health < -25.0f) // Should they gib?
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

public enum EntityEvent
{
    None, // Do nothing
    Kill, // Kill the entity
    Delete, // Delete the entity as a whole
    TakeDamage, // Make this entity take damage
    SetPosition, // Set this entity's position
    SetBBox, // Set this entity's BBox
}