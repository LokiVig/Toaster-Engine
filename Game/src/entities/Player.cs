﻿using Toast.Engine.Entities;
using Toast.Engine.Resources;

namespace Toast.Game.Entities;

public class Player : PlayerEntity
{
    protected float armor = 0.0f; // Remove a certain amount of damage taken if armor isn't 0, and decrease the armor value when taking damage
    protected float maxArmor = 100.0f; // Determines the max amount of armor the player can have

    public override void TakeDamage( float damage, Entity? source = null )
    {
        // Take half amount of damage if we have armor
        if ( armor > 0.0f )
        {
            armor -= damage;
            health -= damage / 2;
        }
        else if ( armor <= 0.0f ) // Take regular amount of damage if no armor
        {
            health -= damage;
        }

        // We have taken damage, OnDamage call!
        OnDamage();
    }
}