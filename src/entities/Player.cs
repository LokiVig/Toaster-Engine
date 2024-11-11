using DoomNET.Resources;

namespace DoomNET.Entities;

public class Player : Entity
{
    public override EntityTypes type => EntityTypes.Player; // This entity is of type Player
    public override float health => 100.0f;

    private float armor = 0.0f; // Remove a certain amount of damage if armor isn't 0, and decrease the armor value when taking damage

    public Player() : base() { }

    public Player(Vector3 position) : base(position) { }

    public Player(Vector3 position, BBox bbox) : base(position, bbox) { }

    protected override void Update()
    {
        base.Update();

        // Take user inputs to make this player move
        HandleInput();
    }

    public override void TakeDamage(float damage)
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

    /// <summary>
    /// Method to handle player inputs, for moving and looking around
    /// </summary>
    private void HandleInput()
    {
        char inputKey;
        Vector2 mousePos;
    }
}