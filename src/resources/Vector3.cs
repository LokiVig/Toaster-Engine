#pragma warning disable CS0660
#pragma warning disable CS0661

using System;

using DoomNET.Rendering;

namespace DoomNET.Resources;

/// <summary>
/// Struct defining a point in 3 dimensions (Z -> up)
/// </summary>
public struct Vector3
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public Vector3()
    {
        x = y = z = 0.0f;
    }

    public Vector3( float xyz )
    {
        x = y = z = xyz;
    }

    public Vector3( float x, float y )
    {
        this.x = x;
        this.y = y;
        this.z = 0;
    }

    public Vector3( float x, float y, float z )
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static float DistanceBetween( Vector3 source, Vector3 dest )
    {
        return ( source - dest ).Magnitude();
    }

    public float DistanceTo( Vector3 other )
    {
        return ( this - other ).Magnitude();
    }

    public float Magnitude()
    {
        return (float)Math.Sqrt( x * x + y * y + z * z );
    }

    public Vector3 Normalized()
    {
        float magnitude = Magnitude();

        if (magnitude > 0)
        {
            return new Vector3( x / magnitude, y / magnitude, z / magnitude );
        }
        else
        {
            return new Vector3();
        }
    }

    public static Vector3 Normalize( Vector3 vector )
    {
        return vector.Normalized();
    }

    public static Vector3 ScreenToWorldDirection( int x, int y, Camera camera )
    {
        // Convert screen coordinates to world coordinates
        float aspectRatio = (float)DoomNET.windowWidth / DoomNET.windowHeight;
        float fovScale = (float)Math.Tan( camera.fieldOfView * 0.5 * Math.PI / 180 );

        float px = ( 2 * ( ( x + 0.5f ) / DoomNET.windowWidth ) - 1 ) * aspectRatio * fovScale;
        float py = ( 1 - 2 * ( ( y + 0.5f ) / DoomNET.windowHeight ) ) * fovScale;

        // Get the screen direction vector
        Vector3 screenDirection = new Vector3( px, py, -1 ).Normalized();

        // Transform by the camera's orientation
        return camera.TransformDirection( screenDirection );
    }

    public readonly override string ToString()
    {
        return $"<{x:0.##}, {y:0.##}, {z:0.##}>";
    }

    #region OPERATORS
    public static Vector3 operator +( Vector3 lhs ) => lhs;
    public static Vector3 operator -( Vector3 lhs ) => new Vector3( -lhs.x, -lhs.y, -lhs.z );

    public static bool operator ==( Vector3 lhs, Vector3 rhs )
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }

    public static bool operator ==( Vector3 lhs, float rhs )
    {
        return lhs.x == rhs && lhs.y == rhs && lhs.z == rhs;
    }

    public static bool operator ==( float lhs, Vector3 rhs )
    {
        return lhs == rhs.x && lhs == rhs.y && lhs == rhs.z;
    }

    public static bool operator !=( Vector3 lhs, Vector3 rhs )
    {
        return !( lhs == rhs );
    }

    public static bool operator !=( Vector3 lhs, float rhs )
    {
        return !( lhs == rhs );
    }

    public static bool operator !=( float lhs, Vector3 rhs )
    {
        return !( lhs == rhs );
    }

    public static bool operator <( Vector3 lhs, Vector3 rhs )
    {
        return lhs.x - rhs.x < 0 && lhs.y - rhs.y < 0 && lhs.z - rhs.z < 0;
    }

    public static bool operator <( Vector3 lhs, float rhs )
    {
        return lhs.x - rhs < 0 && lhs.y - rhs < 0 && lhs.z - rhs < 0;
    }

    public static bool operator <( float lhs, Vector3 rhs )
    {
        return lhs - rhs.x < 0 && lhs - rhs.y < 0 && lhs - rhs.z < 0;
    }

    public static bool operator >( Vector3 lhs, Vector3 rhs )
    {
        return lhs.x - rhs.x > 0 && lhs.y - rhs.y > 0 && lhs.z - rhs.z > 0;
    }

    public static bool operator >( Vector3 lhs, float rhs )
    {
        return lhs.x - rhs > 0 && lhs.y - rhs > 0 && lhs.z - rhs > 0;
    }

    public static bool operator >( float lhs, Vector3 rhs )
    {
        return lhs - rhs.x > 0 && lhs - rhs.y > 0 && lhs - rhs.z > 0;
    }

    public static bool operator <=( Vector3 lhs, Vector3 rhs )
    {
        return !( lhs > rhs );
    }

    public static bool operator <=( Vector3 lhs, float rhs )
    {
        return !( lhs > rhs );
    }

    public static bool operator <=( float lhs, Vector3 rhs )
    {
        return !( lhs > rhs );
    }

    public static bool operator >=( Vector3 lhs, Vector3 rhs )
    {
        return !( lhs < rhs );
    }

    public static bool operator >=( Vector3 lhs, float rhs )
    {
        return !( lhs < rhs );
    }

    public static bool operator >=( float lhs, Vector3 rhs )
    {
        return !( lhs < rhs );
    }

    public static Vector3 operator +( Vector3 lhs, Vector3 rhs )
    {
        return new Vector3( lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z );
    }

    public static Vector3 operator +( Vector3 lhs, float rhs )
    {
        return new Vector3( lhs.x + rhs, lhs.y + rhs, lhs.z + rhs );
    }

    public static Vector3 operator +( float lhs, Vector3 rhs )
    {
        return new Vector3( lhs + rhs.x, lhs + rhs.y, lhs + rhs.z );
    }

    public static Vector3 operator -( Vector3 lhs, Vector3 rhs )
    {
        return new Vector3( lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z );
    }

    public static Vector3 operator -( Vector3 lhs, float rhs )
    {
        return new Vector3( lhs.x - rhs, lhs.y - rhs, lhs.z - rhs );
    }

    public static Vector3 operator -( float lhs, Vector3 rhs )
    {
        return new Vector3( lhs - rhs.x, lhs - rhs.y, lhs - rhs.z );
    }

    public static Vector3 operator *( Vector3 lhs, Vector3 rhs )
    {
        return new Vector3( lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z );
    }

    public static Vector3 operator *( Vector3 lhs, float rhs )
    {
        return new Vector3( lhs.x * rhs, lhs.y * rhs, lhs.z * rhs );
    }

    public static Vector3 operator *( float lhs, Vector3 rhs )
    {
        return new Vector3( lhs * rhs.x, lhs * rhs.y, lhs * rhs.z );
    }

    public static Vector3 operator /( Vector3 lhs, Vector3 rhs )
    {
        if (lhs == 0 || rhs == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector3( lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z );
    }

    public static Vector3 operator /( Vector3 lhs, float rhs )
    {
        if (lhs == 0 || rhs == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector3( lhs.x / rhs, lhs.y / rhs, lhs.z / rhs );
    }

    public static Vector3 operator /( float lhs, Vector3 rhs )
    {
        if (lhs == 0 || rhs == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector3( lhs / rhs.x, lhs / rhs.y, lhs / rhs.z );
    }

    public static explicit operator float( Vector3 lhs )
    {
        return lhs.Magnitude();
    }

    public float this[ int i ]
    {
        get
        {
            switch (i)
            {
                case 0:
                    return x;

                case 1:
                    return y;

                case 2:
                    return z;

                default:
                    throw new IndexOutOfRangeException();
            }
        }
        set
        {
            switch (i)
            {
                case 0:
                    x = value;
                    break;

                case 1:
                    y = value;
                    break;

                case 2:
                    z = value;
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
    #endregion // OPERATORS
}