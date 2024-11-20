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
    public float fieldOfView = (float)Math.PI / 2;
    public float aspectRatio { private get; set; }

    public float pitch
    {
        get => Quaternion.RadiansToDegrees(_pitch);
        set
        {
            // Clamp the value between -89 and 89 to prevent the camera from going upside down
            float angle = Math.Clamp(value, -89f, 89f);
            _pitch = Quaternion.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    public float yaw
    {
        get => Quaternion.RadiansToDegrees(_yaw);
        set
        {
            _yaw = Quaternion.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    public Vector3 right => _right; // X axis
    public Vector3 forward => _forward; // Y axis
    public Vector3 up => _up; // Z axis
    private Vector3 _right = Vector3.UnitX;
    private Vector3 _forward = Vector3.UnitY;
    private Vector3 _up = Vector3.UnitZ;
    private float _pitch;
    private float _yaw = -(float)Math.PI / 2;

    public Camera( Vector3 position, float aspectRatio)
    {
        this.position = position;
        this.aspectRatio = aspectRatio;
    }
    
    private void UpdateVectors()
    {
        // Calculate the forward matrix
        _forward.x = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        _forward.y = MathF.Sin(_pitch);
        _forward.z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
        
        // We need to make sure the vectors are all normalized
        _forward = Vector3.Normalize(_forward);
        
        // Calculate both the right and the up vector using cross product
        _right = Vector3.Normalize(Vector3.Cross(_forward, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _forward));
    }
}