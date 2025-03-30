using System.Numerics;

namespace Toast.Engine.Entities;

public class BrushEntity : Entity
{
    public override EntityType type => EntityType.Brush;

    public BrushEntity() { }

    public BrushEntity( Vector3 position ) : base( position ) { }

    public BrushEntity( Entity parent ) : base( parent ) { }

    public BrushEntity( Entity parent, Vector3 position ) : base( parent, position ) { }
}