using System;

namespace Toast.Engine.Math;

public struct Quaternion : IEquatable<Quaternion>
{
	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
	public float w { get; set; }

	public Quaternion(float xyzw)
	{
		x = y = z = w = xyzw;
	}

	public Quaternion(Vector3 xyz, float w)
	{
		x = xyz.x;
		y = xyz.y;
		z = xyz.z;
		this.w = w;
	}

	public Quaternion(float x, float y, float z, float w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	// Identity quaternion (no rotation)
	public static readonly Quaternion Identity = new Quaternion(0, 0, 0, 1);

	/// <summary>
	/// Length of this quaternion
	/// </summary>
	public float Length()
	{
		return (float)MathF.Sqrt(x * x + y * y + z * z + w * w);
	}

	public static Quaternion Normalize(Quaternion quaternion)
	{
		return quaternion.Normalized();
	}

	public Quaternion Normalized()
	{
		return this / Length();
	}

	public Quaternion Conjugate()
	{
		return new Quaternion(-x, -y, -z, -w);
	}

	/// <summary>
	/// Create a rotation quaternion from an axis-angle
	/// </summary>
	public static Quaternion AngleAxis(float angle, Vector3 axis)
	{
		axis = axis.Normalized();
		float halfAngle = angle * 0.5f;
		float sin = (float)MathF.Sin(halfAngle);

		return new Quaternion(axis.x * sin, axis.y * sin, axis.z * sin, (float)MathF.Cos(halfAngle));
	}

	public readonly override string ToString()
	{
		return $"{{{x:0.##}, {y:0.##}, {z:0.##}, {w:0.##}}}";
	}

	#region OPERATORS

	public static Quaternion operator +(Quaternion lhs) => lhs;
	public static Quaternion operator -(Quaternion lhs) => lhs.Conjugate();

	public static bool operator ==(Quaternion lhs, Quaternion rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
	}

	public static bool operator ==(Quaternion lhs, float rhs)
	{
		return lhs.x == rhs && lhs.y == rhs && lhs.z == rhs && lhs.w == rhs;
	}

	public static bool operator ==(float lhs, Quaternion rhs)
	{
		return lhs == rhs.x && lhs == rhs.y && lhs == rhs.z && lhs == rhs.w;
	}

	public static bool operator !=(Quaternion lhs, Quaternion rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator !=(Quaternion lhs, float rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator !=(float lhs, Quaternion rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator <(Quaternion lhs, Quaternion rhs)
	{
		return lhs.Length() - rhs.Length() < 0;
	}

	public static bool operator <(Quaternion lhs, float rhs)
	{
		return lhs.Length() - rhs < 0;
	}

	public static bool operator <(float lhs, Quaternion rhs)
	{
		return lhs - rhs.Length() < 0;
	}

	public static bool operator >(Quaternion lhs, Quaternion rhs)
	{
		return lhs.Length() - rhs.Length() > 0;
	}

	public static bool operator >(Quaternion lhs, float rhs)
	{
		return lhs.Length() - rhs > 0;
	}

	public static bool operator >(float lhs, Quaternion rhs)
	{
		return lhs - rhs.Length() > 0;
	}

	public static bool operator <=(Quaternion lhs, Quaternion rhs)
	{
		return !(lhs > rhs);
	}

	public static bool operator <=(Quaternion lhs, float rhs)
	{
		return !(lhs > rhs);
	}

	public static bool operator <=(float lhs, Quaternion rhs)
	{
		return !(lhs > rhs);
	}

	public static bool operator >=(Quaternion lhs, Quaternion rhs)
	{
		return !(lhs < rhs);
	}

	public static bool operator >=(Quaternion lhs, float rhs)
	{
		return !(lhs < rhs);
	}

	public static bool operator >=(float lhs, Quaternion rhs)
	{
		return !(lhs < rhs);
	}

	public static Quaternion operator +(Quaternion lhs, Quaternion rhs)
	{
		return new Quaternion(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
	}

	public static Quaternion operator +(Quaternion lhs, float rhs)
	{
		return new Quaternion(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs, lhs.w + rhs);
	}

	public static Quaternion operator +(float lhs, Quaternion rhs)
	{
		return new Quaternion(lhs + rhs.x, lhs + rhs.y, lhs + rhs.z, lhs + rhs.w);
	}

	public static Quaternion operator -(Quaternion lhs, Quaternion rhs)
	{
		return new Quaternion(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
	}

	public static Quaternion operator -(Quaternion lhs, float rhs)
	{
		return new Quaternion(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs, lhs.w - rhs);
	}

	public static Quaternion operator -(float lhs, Quaternion rhs)
	{
		return new Quaternion(lhs - rhs.x, lhs - rhs.y, lhs - rhs.z, lhs - rhs.w);
	}

	public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
	{
		return new Quaternion
		(
			lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
			lhs.w * rhs.w - lhs.x * rhs.z + lhs.y * rhs.w + lhs.z * rhs.x,
			lhs.w * rhs.z + lhs.x * rhs.y - lhs.y * rhs.x + lhs.z * rhs.w,
			lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z
		);
	}

	public static Quaternion operator *(Quaternion lhs, float rhs)
	{
		return new Quaternion(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs);
	}

	public static Quaternion operator *(float lhs, Quaternion rhs)
	{
		return new Quaternion(lhs * rhs.x, lhs * rhs.x, lhs * rhs.z, lhs * rhs.w);
	}

	public static Vector3 operator *(Quaternion lhs, Vector3 rhs)
	{
		// Convert point into a quaternion
		Quaternion pointQuat = new Quaternion(rhs.x, rhs.y, rhs.z, 0);
		// Apply rotation; lhs * rhs * lhs^-1
		Quaternion resultQuat = lhs * pointQuat * lhs.Conjugate();

		return new Vector3(resultQuat.x, resultQuat.y, resultQuat.z);
	}

	public static Quaternion operator /(Quaternion lhs, Quaternion rhs)
	{
		return lhs * -rhs;
	}

	public static Quaternion operator /(Quaternion lhs, float rhs)
	{
		return new Quaternion(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w / rhs);
	}

	public static Quaternion operator /(float lhs, Quaternion rhs)
	{
		return new Quaternion(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z, lhs / rhs.w);
	}

	public static explicit operator float(Quaternion lhs)
	{
		return lhs.Length();
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
					z = value;
					break;

				default:
					throw new IndexOutOfRangeException();
			}
		}
	}

	#endregion // OPERATORS

	public bool Equals(Quaternion other)
	{
		return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
	}

	public override bool Equals(object obj)
	{
		return obj is Quaternion other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(x, y, z, w);
	}
}