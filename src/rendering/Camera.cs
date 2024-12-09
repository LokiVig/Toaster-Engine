using System;

using DoomNET.Entities;
using DoomNET.Resources;

namespace DoomNET.Rendering;

/// <summary>
/// Defines the position and rotation of the player's camera.<br/>
/// This will be used for rendering everything that has to do with <see cref="Game.currentScene"/>.
/// </summary>
public class Camera
{
    public float fieldOfView = (float)Math.PI / 2;
    public float aspectRatio;
    
    private Vector3 position;
    private Vector3 positionOffset;
    private Entity parent;
    
    public Camera( Vector3 positionOffset, float aspectRatio, Entity parent)
    {
        this.positionOffset = positionOffset;
        this.aspectRatio = aspectRatio;
        this.parent = parent;
        
        Game.OnUpdate += UpdatePosition;
    }

    /// <summary>
    /// We should update the position based off our parent's position and our offset from it.
    /// </summary>
    private void UpdatePosition()
    {
        position = parent.GetPosition() + positionOffset;
    }
}