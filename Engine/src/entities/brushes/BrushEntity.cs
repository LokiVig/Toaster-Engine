using System.Numerics;

using Toast.Engine.Resources;

namespace Toast.Engine.Entities;

public class BrushEntity : Entity
{
    public override EntityType type => EntityType.Brush;

    public BrushEntity() : base() { }

    public BrushEntity( Vector3 position ) : base( position ) { }

    public BrushEntity( Entity parent ) : base( parent ) { }

    public BrushEntity( Entity parent, Vector3 position ) : base( parent, position ) { }

    /// <summary>
    /// Turns this brush entity into a regular brush.
    /// </summary>
    /// <returns>A brush with the same bounding box as from this entity, or <see langword="null"/> if something went wrong.</returns>
    public Brush TurnIntoBrush()
    {
        // Create a new brush from our bounding box
        Brush resultingBrush = new Brush(transform.boundingBox);

        // Remove this entity from the active file
        EngineManager.currentFile.RemoveEntity( this );

        // Add the newly made brush to the active file
        EngineManager.currentFile.AddBrush( resultingBrush );

        // Return the brush!
        return resultingBrush;
    }
}