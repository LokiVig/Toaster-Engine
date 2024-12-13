using DoomNET.Resources;

namespace DoomNET.Entities;

public class Player : Entity
{
    public override EntityType type => EntityType.Player; // This entity is of type Player
    public override float health { get; set; } = 100.0f;
    
    private float armor = 0.0f; // Remove a certain amount of damage taken if armor isn't 0, and decrease the armor value when taking damage

    public Player()
    {
        SetBBox(new BBox(new Vector3(-32, -32, 0), new Vector3(32, 32, 64)));
    }

    public Player(Vector3 position) : base(position)
    {
        SetBBox(new BBox(new Vector3(-32, -32, 0), new Vector3(32, 32, 64)));
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
    }

    protected override void Update()
    {
        base.Update();

        // Handle movements
        HandleMovement();
    }

    public override void TakeDamage( float damage, Entity source = null )
    {
        // Take half amount of damage if we have armor
        if (armor > 0.0f)
        {
            armor -= damage;
            health -= damage / 2;
        }
        else if (armor <= 0.0f) // Take regular amount of damage if no armor
        {
            health -= damage;
        }

        // We have taken damage, OnDamage call!
        OnDamage();
    }
}