// ReSharper disable CompareOfFloatsByEqualityOperator

using System;

namespace DoomNET.Resources;

/// <summary>
/// Struct defining a point in 3 dimensions (Z -> up)
/// </summary>
public struct Vector3 : IEquatable<Vector3>
{
	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }

	public Vector3()
	{
		this = Zero;
	}

	public Vector3(float xyz)
	{
		x = y = z = xyz;
	}

	public Vector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3(Vector2 vector2, float z)
	{
		x = vector2.x;
		y = vector2.y;
		this.z = z;
	}

	public static readonly Vector3 One = new Vector3(1, 1, 1);
	public static readonly Vector3 Zero = new Vector3(0, 0, 0);

	public static readonly Vector3 UnitX = new Vector3(1, 0, 0);
	public static readonly Vector3 UnitY = new Vector3(0, 1, 0);
	public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);

	public static readonly Vector3 Up = new Vector3(0, 0, 1);
	public static readonly Vector3 Down = new Vector3(0, 0, -1);
	public static readonly Vector3 Right = new Vector3(1, 0, 0);
	public static readonly Vector3 Left = new Vector3(-1, 0, 0);
	public static readonly Vector3 Forward = new Vector3(0, 1, 0);
	public static readonly Vector3 Backward = new Vector3(0, -1, 0);

	public static float DistanceBetween(Vector3 source, Vector3 dest)
	{
		return (source - dest).Magnitude();
	}

	public float DistanceTo(Vector3 other)
	{
		return (this - other).Magnitude();
	}

	public float Magnitude()
	{
		return (float)Math.Sqrt(x * x + y * y + z * z);
	}

	public Vector3 Normalized()
	{
		float magnitude = Magnitude();

		if (magnitude > 0)
		{
			return new Vector3(x / magnitude, y / magnitude, z / magnitude);
		}
		else
		{
			return new Vector3();
		}
	}

	public static Vector3 Normalize(Vector3 vector)
	{
		return vector.Normalized();
	}

	public static Vector3 Cross(Vector3 a, Vector3 b)
	{
		return new Vector3
		(
			a.y * b.z - a.z * b.y, // Calculate X component
			a.z * b.x - a.x * b.z, // Calculate Y component
			a.x * b.y - a.y * b.x  // Calculate Z component
		);
	}

	public static float Dot(Vector3 a, Vector3 b)
	{
		return a.x * b.x + a.y + b.y + a.z * b.z;
	}

	public readonly override string ToString()
	{
		return $"<{x:0.##}, {y:0.##}, {z:0.##}>";
	}

	#region OPERATORS

	public static Vector3 operator +(Vector3 lhs) => lhs;
	public static Vector3 operator -(Vector3 lhs) => new Vector3(-lhs.x, -lhs.y, -lhs.z);

	public static bool operator ==(Vector3 lhs, Vector3 rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
	}

	public static bool operator ==(Vector3 lhs, float rhs)
	{
		return lhs.x == rhs && lhs.y == rhs && lhs.z == rhs;
	}

	public static bool operator ==(float lhs, Vector3 rhs)
	{
		return lhs == rhs.x && lhs == rhs.y && lhs == rhs.z;
	}

	public static bool operator !=(Vector3 lhs, Vector3 rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator !=(Vector3 lhs, float rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator !=(float lhs, Vector3 rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator <(Vector3 lhs, Vector3 rhs)
	{
		return lhs.Magnitude() - rhs.Magnitude() < 0;
	}

	public static bool operator <(Vector3 lhs, float rhs)
	{
		return lhs.Magnitude() - rhs < 0;
	}

	public static bool operator <(float lhs, Vector3 rhs)
	{
		return lhs - rhs.Magnitude() < 0;
	}

	public static bool operator >(Vector3 lhs, Vector3 rhs)
	{
		return lhs.Magnitude() - rhs.Magnitude() > 0;
	}

	public static bool operator >(Vector3 lhs, float rhs)
	{
		return lhs.Magnitude() - rhs > 0;
	}

	public static bool operator >(float lhs, Vector3 rhs)
	{
		return lhs - rhs.Magnitude() > 0;
	}

	public static bool operator <=(Vector3 lhs, Vector3 rhs)
	{
		return !(lhs > rhs);
	}

	public static bool operator <=(Vector3 lhs, float rhs)
	{
		return !(lhs > rhs);
	}

	public static bool operator <=(float lhs, Vector3 rhs)
	{
		return !(lhs > rhs);
	}

	public static bool operator >=(Vector3 lhs, Vector3 rhs)
	{
		return !(lhs < rhs);
	}

	public static bool operator >=(Vector3 lhs, float rhs)
	{
		return !(lhs < rhs);
	}

	public static bool operator >=(float lhs, Vector3 rhs)
	{
		return !(lhs < rhs);
	}

	public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
	{
		return new Vector3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
	}

	public static Vector3 operator +(Vector3 lhs, float rhs)
	{
		return new Vector3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
	}

	public static Vector3 operator +(float lhs, Vector3 rhs)
	{
		return new Vector3(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z);
	}

	public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
	{
		return new Vector3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
	}

	public static Vector3 operator -(Vector3 lhs, float rhs)
	{
		return new Vector3(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs);
	}

	public static Vector3 operator -(float lhs, Vector3 rhs)
	{
		return new Vector3(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z);
	}

	public static Vector3 operator *(Vector3 lhs, Vector3 rhs)
	{
		return new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
	}

	public static Vector3 operator *(Vector3 lhs, float rhs)
	{
		return new Vector3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
	}

	public static Vector3 operator *(float lhs, Vector3 rhs)
	{
		return new Vector3(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z);
	}

	public static Vector3 operator /(Vector3 lhs, Vector3 rhs)
	{
		if (lhs == 0 || rhs == 0)
		{
			throw new DivideByZeroException();
		}

		return new Vector3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
	}

	public static Vector3 operator /(Vector3 lhs, float rhs)
	{
		if (lhs == 0 || rhs == 0)
		{
			throw new DivideByZeroException();
		}

		return new Vector3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
	}

	public static Vector3 operator /(float lhs, Vector3 rhs)
	{
		if (lhs == 0 || rhs == 0)
		{
			throw new DivideByZeroException();
		}

		return new Vector3(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z);
	}

	public static explicit operator float(Vector3 lhs)
	{
		return lhs.Magnitude();
	}

	public float this[int i]
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

	public bool Equals(Vector3 other)
	{
		return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
	}

	public override bool Equals(object obj)
	{
		return obj is Vector3 other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(x, y, z);
	}
}