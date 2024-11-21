using DoomNET.Resources;
using DoomNET.Rendering;

namespace DoomNET.Entities;

public class Player : Entity
{
    public override EntityType type => EntityType.Player; // This entity is of type Player
    
    public Camera camera;

    protected float _health = 100.0f;
    
    private float armor = 0.0f; // Remove a certain amount of damage taken if armor isn't 0, and decrease the armor value when taking damage

    public Player() : base()
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

        camera = new Camera( new Vector3(0, 0, bbox.maxs.z - 12) /* (64 - 12 => 52) */, (float)DoomNET.windowWidth / DoomNET.windowHeight, this );
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
            _health -= damage / 2;
        }
        else if (armor <= 0.0f) // Take regular amount of damage if no armor
        {
            _health -= damage;
        }

        // We have taken damage, OnDamage call!
        OnDamage();
    }
}