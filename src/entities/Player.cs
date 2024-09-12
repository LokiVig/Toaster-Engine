using DoomNET.Resources;

namespace DoomNET.Entities;

public class Player : Entity
{
    protected override EntityTypes type => EntityTypes.Player; // This entity is of type Player
    protected override float health => 100.0f;

    private float armor = 0.0f;

    public Player() : base() { }

    public Player(Vector3 position) : base(position) { }

    public Player(Vector3 position, BBox bbox) : base(position, bbox) { }

    protected override void Update()
    {
        base.Update();

        HandleInput();
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