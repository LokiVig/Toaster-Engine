#pragma warning disable CS0660
#pragma warning disable CS0661

using System;

namespace DoomNET.Resources;

public struct Vector4
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float w { get; set; }

    public Vector4()
    {
        this = Zero;
    }

    public Vector4( float xyzw )
    {
        x = y = z = w = xyzw;
    }

    public Vector4( float x, float y, float z, float w )
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Vector4( Vector3 vector3, float w = 0 )
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
        this.w = w;
    }

    public static readonly Vector4 One = new Vector4( 1, 1, 1, 1 );
    public static readonly Vector4 Zero = new Vector4( 0, 0, 0, 0 );

    public static float DistanceBetween( Vector4 source, Vector4 dest )
    {
        return ( source - dest ).Magnitude();
    }

    public float DistanceTo( Vector4 other )
    {
        return ( this - other ).Magnitude();
    }

    public float Magnitude()
    {
        return (float)Math.Sqrt( x * x + y * y + z * z + w * w );
    }

    public Vector4 Normalized()
    {
        float magnitude = Magnitude();

        if (magnitude > 0)
        {
            return new Vector4( x / magnitude, y / magnitude, z / magnitude, w / magnitude );
        }
        else
        {
            return new Vector4();
        }
    }

    public static Vector4 Normalize( Vector4 vector )
    {
        return vector.Normalized();
    }

    public void Homogenize()
    {
        if (w != 0)
        {
            x /= w;
            y /= w;
            z /= w;
        }
    }

    public static Vector4 Cross(Vector4 a, Vector4 b )
    {
        return new Vector4
        ( 
            a.y * b.z - a.z * b.y, // Calculate X component
            a.z * b.x - a.x * b.z, // Calculate Y component
            a.x * b.y - a.y * b.x, // Calculate Z component
            a.w                    // This is just a.w
        );
    }

    public readonly override string ToString()
    {
        return $"<{x:0.##}, {y:0.##}, {z:0.##}, {w:0.##}>";
    }

    #region OPERATORS
    public static Vector4 operator +( Vector4 lhs ) => lhs;
    public static Vector4 operator -( Vector4 lhs ) => new Vector4( -lhs.x, -lhs.y, -lhs.z, -lhs.w );

    public static bool operator ==( Vector4 lhs, Vector4 rhs )
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
    }

    public static bool operator ==( Vector4 lhs, float rhs )
    {
        return lhs.x == rhs && lhs.y == rhs && lhs.z == rhs && lhs.w == rhs;
    }

    public static bool operator ==( float lhs, Vector4 rhs )
    {
        return lhs == rhs.x && lhs == rhs.y && lhs == rhs.z && lhs == rhs.w;
    }

    public static bool operator !=( Vector4 lhs, Vector4 rhs )
    {
        return !( lhs == rhs );
    }

    public static bool operator !=( Vector4 lhs, float rhs )
    {
        return !( lhs == rhs );
    }

    public static bool operator !=( float lhs, Vector4 rhs )
    {
        return !( lhs == rhs );
    }

    public static bool operator <( Vector4 lhs, Vector4 rhs )
    {
        return lhs.Magnitude() - rhs.Magnitude() < 0;
    }

    public static bool operator <( Vector4 lhs, float rhs )
    {
        return lhs.Magnitude() - rhs < 0;
    }

    public static bool operator <( float lhs, Vector4 rhs )
    {
        return lhs - rhs.Magnitude() < 0;
    }

    public static bool operator >( Vector4 lhs, Vector4 rhs )
    {
        return lhs.Magnitude() - rhs.Magnitude() > 0;
    }

    public static bool operator >( Vector4 lhs, float rhs )
    {
        return lhs.Magnitude() - rhs > 0;
    }

    public static bool operator >( float lhs, Vector4 rhs )
    {
        return lhs - rhs.Magnitude() > 0;
    }

    public static bool operator <=( Vector4 lhs, Vector4 rhs )
    {
        return !( lhs > rhs );
    }

    public static bool operator <=( Vector4 lhs, float rhs )
    {
        return !( lhs > rhs );
    }

    public static bool operator <=( float lhs, Vector4 rhs )
    {
        return !( lhs > rhs );
    }

    public static bool operator >=( Vector4 lhs, Vector4 rhs )
    {
        return !( lhs < rhs );
    }

    public static bool operator >=( Vector4 lhs, float rhs )
    {
        return !( lhs < rhs );
    }

    public static bool operator >=( float lhs, Vector4 rhs )
    {
        return !( lhs < rhs );
    }

    public static Vector4 operator +( Vector4 lhs, Vector4 rhs )
    {
        return new Vector4( lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w );
    }

    public static Vector4 operator +( Vector4 lhs, float rhs )
    {
        return new Vector4( lhs.x + rhs, lhs.y + rhs, lhs.z + rhs, lhs.w + rhs );
    }

    public static Vector4 operator +( float lhs, Vector4 rhs )
    {
        return new Vector4( lhs + rhs.x, lhs + rhs.y, lhs + rhs.z, lhs + rhs.w );
    }

    public static Vector4 operator -( Vector4 lhs, Vector4 rhs )
    {
        return new Vector4( lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w );
    }

    public static Vector4 operator -( Vector4 lhs, float rhs )
    {
        return new Vector4( lhs.x - rhs, lhs.y - rhs, lhs.z - rhs, lhs.w - rhs );
    }

    public static Vector4 operator -( float lhs, Vector4 rhs )
    {
        return new Vector4( lhs - rhs.x, lhs - rhs.y, lhs - rhs.z, lhs - rhs.w );
    }

    public static Vector4 operator *( Vector4 lhs, Vector4 rhs )
    {
        return new Vector4( lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w );
    }

    public static Vector4 operator *( Vector4 lhs, float rhs )
    {
        return new Vector4( lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs );
    }

    public static Vector4 operator *( float lhs, Vector4 rhs )
    {
        return new Vector4( lhs * rhs.x, lhs * rhs.y, lhs * rhs.z, lhs * rhs.w );
    }

    public static Vector4 operator /( Vector4 lhs, Vector4 rhs )
    {
        if (lhs == 0 || rhs == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector4( lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w );
    }

    public static Vector4 operator /( Vector4 lhs, float rhs )
    {
        if (lhs == 0 || rhs == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector4( lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w / rhs );
    }

    public static Vector4 operator /( float lhs, Vector4 rhs )
    {
        if (lhs == 0 || rhs == 0)
        {
            throw new DivideByZeroException();
        }

        return new Vector4( lhs / rhs.x, lhs / rhs.y, lhs / rhs.z, lhs / rhs.w );
    }

    public static explicit operator float( Vector4 lhs )
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

                case 3:
                    return w;

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

                case 3:
                    w = value;
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
    #endregion // OPERATORS
}