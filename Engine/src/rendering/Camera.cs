﻿using System.Numerics;

using Toast.Engine.Entities;

namespace Toast.Engine.Rendering;

/// <summary>
/// A camera with a frustum, offset, parent entity, etc.
/// </summary>
public class Camera : ToolEntity
{
    public Vector3 offset;

    public Camera( Entity parent, Vector3 offset ) : base( parent )
    {
        this.offset = offset;
        rotation = Quaternion.Identity;
    }

    public Camera( string parent, Vector3 offset ) : base( parent )
    {
        this.offset = offset;
        rotation = Quaternion.Identity;
    }

    public Camera( Entity parent, Vector3 offset, Quaternion rotation ) : this( parent, offset )
    {
        this.rotation = rotation;
    }
}