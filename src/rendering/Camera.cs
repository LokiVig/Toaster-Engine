using System;

using DoomNET.Resources;

namespace DoomNET.Rendering;

/// <summary>
/// Defines the position and rotation of the player's camera.<br/>
/// This will be used for rendering everything that has to do with <see cref="DoomNET.currentScene"/>.
/// </summary>
public class Camera
{
    public Vector3 position;
    public Quaternion rotation;
    public float fieldOfView = 90;

    public Vector3 right => rotation * new Vector3( 1, 0, 0 ); // X axis
    public Vector3 forward => rotation * new Vector3( 0, 1, 0 ); // Y axis
    public Vector3 up => rotation * new Vector3( 0, 0, 1 ); // Z axis

    public Camera( Vector3 position, Quaternion rotation )
    {
        this.position = position;
        this.rotation = rotation;
    }
}