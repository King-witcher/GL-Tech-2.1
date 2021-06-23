using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
	/// <summary>
	///     Represents a Vector in a bidimensional space.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Vector : IEquatable<Vector>
    {
		private const float TORAD = (float)Math.PI / 180f;
		private const float TODEGREE = 180f / (float)Math.PI;

		internal readonly float x;
		internal readonly float y;

		/// <summary>
		///     Gets and sets the X cordinate of the vector.
		/// </summary>
		public float X { get => x; }

		/// <summary>
		///     Gets and sets the Y cordinate of the vector.
		/// </summary>
		public float Y { get => y; }

		/// <summary>
		///     Creates a new instance of vector instance based on its cordinates.
		/// </summary>
		/// <param name="x">X cordinate</param>
		/// <param name="y">Y cordinate</param>
		public Vector(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		///     Creates a new instance of vector that has module equal to 1 and a specified angle.
		/// </summary>
		/// <remarks>
		///     Angle conventions are such that angle 0 has (x = 0, y = 1) components and angle 90 has (x = 1, y = 0) components.
		/// </remarks>
		/// <param name="angle">Angle in degrees</param>
		public Vector(float angle) // Not optimized by fast sin and fast cos
		{
			x = (float)Math.Sin(angle * Math.PI / 180);
			y = (float)Math.Cos(angle * Math.PI / 180);
		}

		/// <summary>
		///     Creates a instance of vector with specified angle and module.
		/// </summary>
		/// <param name="angle">Angle in degrees</param>
		/// <param name="module">Module</param>
		/// <returns>A new instance of vector with specified angle and module.</returns>
		public static Vector FromAngle(float angle, float module)
		{
			float x = (float)Math.Sin(angle * Math.PI / 180);
			float y = (float)Math.Cos(angle * Math.PI / 180);
			return new Vector(module * x, module * y);
		}

		/// <summary>
		///     Gets and sets the angle, in degrees, of the vector, approximately preserving it's angle.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Getting angle property will always return a value greater than -90 and less or equal to 270 due to performanceoptimizations and can be changed in future patches.
		///     </para>
		///     <para>
		///         Modifying the angle of a vector is equivalent to creating a new vector with the same module and the specified angle, which can be slow if done many times and, due to <see href="https://en.wikipedia.org/wiki/Floating-point_arithmetic#Accuracy_problems">floating-point accuracy problems</see>, can distort the original vector's module if changed many times. If you wish to constantly change the angle of a vector, consider holding the module value and constantly creating new vectors by module and angle via Vector.FromAngle(float, float) or new Vector(flaot).
		///     </para>
		/// </remarks>
		public float Angle      // Not optimized
		{
			get
			{
				if (Module == 0)
					return 0;

				float temp = 180f * (float)Math.Asin(x / Module) / (float)Math.PI;
				if (y > 0)
					return temp;
				else
					return 180 - temp;
			}
			set
			{
				float delta = value - this.Angle;
				this *= new Vector(delta);
			}
		}

		/// <summary>
		///		Gets and sets the module of the vector, approximately preserving its module.
		/// </summary>
		/// <remarks>
		///     Modifying the module of a vector is equivalent to multiplying the original vector by a certain factor.
		/// </remarks>
		public float Module
		{
			get => (float)Math.Sqrt(x * x + y * y);
			set
			{
				float delta = value / Module;
				this *= delta;
			}
		}

		/// <summary>
		///		Gets the vector that is at the origin of the bidimensional space.
		/// </summary>
		public static Vector Origin { get => new Vector(0, 0); }

		/// <summary>
		///		Equivalent to new Vector (0, 1);
		/// </summary>
		public static Vector Forward { get => new Vector(0, 1); }

		/// <summary>
		///		Equivalent to new Vector (1, 0);
		/// </summary>
		public static Vector Right { get => new Vector(1, 0); }

		/// <summary>
		///		Equivalent to new Vector (0, -1);
		/// </summary>
		public static Vector Backward { get => new Vector(0, -1); }

		/// <summary>
		///		Equivalent to new Vector (-1, 0);
		/// </summary>
		public static Vector Left { get => new Vector(-1, 0); }

		/// <summary>
		///		Gets an array of vectors that compose the verts of a regular polygon.
		/// </summary>
		/// <param name="center">Center of the polygon.</param>
		/// <param name="radius">Radius of the polygon.</param>
		/// <param name="edges">How many edges the polygon have.</param>
		/// <returns>An array of vectors that compose the verts of a regular polygon.</returns>
		public static Vector[] GetPolygon(Vector center, float radius, int edges)
		{
			if (edges == 0)
				throw new ArgumentException("\"edges\" cannot be 0.", "edges");

			Vector[] result = new Vector[edges];
			for (int i = 0; i < edges; i++)
				result[i] = center + radius * new Vector(i * 360 / edges);
			return result;
		}

		/// <summary>
		///		Gets the euclidian distance to another vector.
		/// </summary>
		/// <param name="vector">Another vector</param>
		/// <returns>The euclidian distance between both vectors.</returns>
		public float GetDistance(Vector vector)
		{
			float dx = vector.x - x;
			float dy = vector.y - y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		/// <summary>
		///		Creates a new instance of vector that represents it's components in a different cartisian axis system.
		/// </summary>
		/// <param name="origin">The origin of the specified cartesian system.</param>
		/// <param name="normal">The so called normal component (Y) of the the specified cartesian system.</param>
		/// <returns>A new instance of vector that represents it's components in a different cartisian axis system.</returns>
		/// <remarks>
		///		Note that the new cartesian system's can have a normal component with module different from 1 and is represents the unit of the cartesian system.
		/// </remarks>
		public Vector Projection(Vector origin, Vector normal)
		{
			return (this - origin) / normal;
		}

		/// <summary>
		///		Gets a new instance of vector that has the actual values of a vector as if it was a projection on a different cartesian system. See Vector Projection (Vector, Vector).
		/// </summary>
		/// <param name="origin">The origin of the specified cartesian system.</param>
		/// <param name="normal">The so called normal component (Y) of the the specified cartesian system.</param>
		/// <returns>A new instance of vector that has the actual values of a vector as if it was a projection on a different cartesian system.</returns>
		public Vector AsProjectionOf(Vector origin, Vector normal)
		{
			return this * normal + origin;
		}

		/// <summary>
		///		Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return $"<{x}, {y}>";
		}

		/// <summary>
		///		Determines whether two object instances are equal.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Vector))
			{
				return false;
			}
			bool xeq = x.Equals(((Vector)obj).x);
			bool yeq = y.Equals(((Vector)obj).y);
			if (xeq && yeq)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the dot product of two vectors.
		/// </summary>
		/// <param name="left">First vector</param>
		/// <param name="right">Second vector</param>
		/// <returns>The dot product of two vectors.</returns>
		public static float DotProduct(Vector left, Vector right) =>
			left.x * right.x + left.y * right.y;

		/// <summary>
		///		Determines whether two object instances are equal.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(Vector other)
        {
            return x == other.x &&
                   y == other.y;
        }

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
        {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

		/// <summary>
		/// Just obvious thing. Will take a while until being documented.
		/// </summary>
		public static Vector operator -(Vector vector) =>
			new Vector(
				-vector.x,
				-vector.y);

		/// <summary>
		/// Just obvious thing. Will take a while until being documented.
		/// </summary>
		public static Vector operator +(Vector left, Vector right) =>
			new Vector(
				left.x + right.x,
				left.y + right.y);

		/// <summary>
		/// Just obvious thing. Will take a while until being documented.
		/// </summary>
		public static Vector operator -(Vector left, Vector right) =>
			new Vector(
				left.x - right.x,
				left.y - right.y);

		/// <summary>
		///		Gets the product between two vectors as if they were complex numbers as y + xi.
		/// </summary>
		/// <param name="left">First vector</param>
		/// <param name="right">Second vector</param>
		/// <returns>The product between two vectors as if they were complex numbers as y + xi.</returns>
		public static Vector operator *(Vector left, Vector right) =>
			new Vector(
				left.y * right.x + left.x * right.y,
				left.y * right.y - left.x * right.x);

		/// <summary>
		///	Gets the division between two vectors as if they were complex numbers as y + xi.
		/// </summary>
		/// <param name="dividend">Dividend</param>
		/// <param name="divider">Divider</param>
		/// <returns>The division between two vectors as if they were complex numbers as y + xi.</returns>
		public static Vector operator /(Vector dividend, Vector divider)
		{
			float delta = divider.x * divider.x + divider.y * divider.y;
			if (delta == 0)
				throw new DivideByZeroException("Divider vector cannot be <0, 0>.");

			float x = (dividend.x * divider.y - dividend.y * divider.x) / delta;
			float y = (dividend.y * divider.y + dividend.x * divider.x) / delta;
			return new Vector(x, y);
		}

		/// <summary>
		///	Gets the product between a vector and a scalar.
		/// </summary>
		/// <param name="scalar">Scalar</param>
		/// <param name="vector">Vector</param>
		/// <returns>The product between a vector and a scalar.</returns>
		public static Vector operator *(float scalar, Vector vector) =>
			new Vector(
				vector.x * scalar,
				vector.y * scalar);

		/// <summary>
		/// Just obvious thing. Will take a while until being documented.
		/// </summary>
		public static Vector operator *(Vector vector, float scalar) =>
			scalar * vector;

		/// <summary>
		/// Just obvious thing. Will take a while until being documented.
		/// </summary>
		public static Vector operator /(Vector vector, float scalar) =>
			new Vector(
				vector.x / scalar,
				vector.y / scalar);

		/// <summary>
		/// Just obvious thing. Will take a while until being documented.
		/// </summary>
		public static bool operator ==(Vector left, Vector right)
		{
			bool xeq = left.x == right.x;
			bool yeq = left.y == right.y;
			return xeq && yeq;
		}

		/// <summary>
		/// Just obvious thing. Will take a while until being documented.
		/// </summary>
		public static bool operator !=(Vector left, Vector right)
		{
			bool xdif = left.x != right.x;
			bool ydif = left.y != right.y;
			return xdif || ydif;
		}

		/// <summary>
		/// Converts a tuple into a GLTech2.Vector struct.
		/// </summary>
		/// <param name="components">the components</param>
		public static implicit operator Vector ((float x, float y) components)
		{
			return new Vector(components.x, components.y);
		}
	}
}
