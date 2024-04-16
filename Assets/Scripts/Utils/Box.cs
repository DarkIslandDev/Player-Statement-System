using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///   <para>Represents an axis aligned bounding box with all values.</para>
/// </summary>
[Serializable]
public struct Box : IEquatable<Box>
// , IFormattable
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 size;

    /// <summary>
    ///   <para>X value of the minimal point of the box.</para>
    /// </summary>
    public float x
    {
        get => position.x;
        set => position.x = value;
    }

    /// <summary>
    ///   <para>Y value of the minimal point of the box.</para>
    /// </summary>
    public float y
    {
        get => position.y;
        set => position.y = value;
    }

    /// <summary>
    ///   <para>Z value of the minimal point of the box.</para>
    /// </summary>
    public float z
    {
        get => position.z;
        set => position.z = value;
    }


    /// <summary>
    ///   <para>The center of the bounding box.</para>
    /// </summary>
    public Vector3 Center => new Vector3(
        x + size.x / 2,
        y + size.y / 2,
        z + size.z / 2);
    
    /// <summary>
    ///   <para>The minimal point of the box.</para>
    /// </summary>
    public Vector3 Min
    {
        get => new(xMin, yMin, zMin);
        set
        {
            xMin = value.x;
            yMin = value.y;
            zMin = value.z;
        }
    }

    /// <summary>
    ///   <para>The maximal point of the box.</para>
    /// </summary>
    public Vector3 Max
    {
        get => new(xMax, yMax, zMax);
        set
        {
            xMax = value.x;
            yMax = value.y;
            zMax = value.z;
        }
    }
    
    /// <summary>
    ///   <para>The minimal x point of the box.</para>
    /// </summary>
    public float xMin
    {
        get => Math.Min(position.x, position.x + size.x);
        set
        {
            float xMax = this.xMax;
            position.x = value;
            size.x = xMax - this.position.x;
        }
    }

    /// <summary>
    ///   <para>The minimal y point of the box.</para>
    /// </summary>
    public float yMin
    {
        get => Math.Min(position.y, position.y + size.y);
        set
        {
            float yMax = this.yMax;
            position.y = value;
            size.y = yMax - position.y;
        }
    }

    /// <summary>
    ///   <para>The minimal z point of the box.</para>
    /// </summary>
    public float zMin
    {
        get => Math.Min(position.z, position.z + size.z);
        set
        {
            float zMax = this.zMax;
            position.z = value;
            size.z = xMax - position.z;
        }
    }

    /// <summary>
    ///   <para>The maximal x point of the box.</para>
    /// </summary>
    public float xMax
    {
        get => Math.Max(position.x, position.x + size.x);
        set => size.x = value - position.x;
    }

    /// <summary>
    ///   <para>The maximal y point of the box.</para>
    /// </summary>
    public float yMax
    {
        get => Math.Max(position.y, position.y + size.y);
        set => size.y = value - position.y;
    }

    /// <summary>
    ///   <para>The maximal z point of the box.</para>
    /// </summary>
    public float zMax
    {
        get => Math.Max(position.z, position.z + size.z);
        set => size.z = value - position.z;
    }

    /// <summary>
    ///   <para>The position of the bounding box.</para>
    /// </summary>
    public Vector3 Position
    {
        get => position;
        set => position = value;
    }

    /// <summary>
    ///   <para>The total size of the box.</para>
    /// </summary>
    public Vector3 Size
    {
        get => size;
        set => size = value;
    }

    public Box(float xMin, float yMin, float zMin, float sizeX, float sizeY, float sizeZ)
    {
        position = new Vector3(xMin, yMin, zMin);
        size = new Vector3(sizeX, sizeY, sizeZ);
    }

    public Box(Vector3 position, Vector3 size)
    {
        this.position = position;
        this.size = size;
    }

    /// <summary>
    ///  <para>Is point contained in the bounding box?</para>
    /// </summary>
    /// <param name="position">Point to check.</param>
    /// <returns>
    ///     <para>Is point contained in the bounding box?</para>
    /// </returns>
    public bool Contains(Vector3 position) =>
        position.x >= xMin &&
        position.y >= yMin &&
        position.z >= zMin &&
        position.x < xMax &&
        position.y < yMax &&
        position.z < zMax;

    // /// <summary>
    // ///   <para>Returns a formatted string for the bounds.</para>
    // /// </summary>
    // /// <param name="format">A numeric format string.</param>
    // /// <param name="formatProvider">An object that specifies culture-specific formatting.</param>
    // public override string ToString() => this.ToString((string)null, (IFormatProvider)null);
    //
    // /// <summary>
    // ///   <para>Returns a formatted string for the bounds.</para>
    // /// </summary>
    // /// <param name="format">A numeric format string.</param>
    // /// <param name="formatProvider">An object that specifies culture-specific formatting.</param>
    // public string ToString(string format) => this.ToString(format, (IFormatProvider)null);
    //
    // /// <summary>
    // ///   <para>Returns a formatted string for the bounds.</para>
    // /// </summary>
    // /// <param name="format">A numeric format string.</param>
    // /// <param name="formatProvider">An object that specifies culture-specific formatting.</param>
    // public string ToString(string format, IFormatProvider formatProvider)
    // {
    //     formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;
    //     
    //     return UnityString.Format("Position: {0}, Size: {1}", 
    //         position.ToString(format, formatProvider), 
    //         position.ToString(format, formatProvider));
    // }

    public static bool operator ==(Box lhs, Box rhs) => lhs.position == rhs.position && lhs.size == rhs.size;

    public static bool operator !=(Box lhs, Box rhs) => !(lhs == rhs);

    public bool Equals(Box other) => position.Equals(other.position) && size.Equals(other.size);

    public override bool Equals(object obj) => obj is Box obj1 && this.Equals(obj1);

    public override int GetHashCode() => position.GetHashCode() ^ size.GetHashCode() << 2;

    /// <summary>
    ///   <para>A BoundsInt.PositionCollection that contains all positions within the BoundsInt.</para>
    /// </summary>
    public Box.PositionEnumerator allPositionWithin => new PositionEnumerator(Min, Max);

    /// <summary>
    ///   <para>An iterator that allows you to iterate over all positions within the BoundsInt.</para>
    /// </summary>
    public struct PositionEnumerator : IEnumerator<Vector3>, IEnumerator, IDisposable
    {
        private readonly Vector3 min;
        private readonly Vector3 max;
        private Vector3 current;

        public PositionEnumerator(Vector3 min, Vector3 max)
        {
            this.min = current = min;
            this.max = max;
            Reset();
        }

        /// <summary>
        ///   <para>Returns this as an iterator that allows you to iterate over all positions within the BoundsInt.</para>
        /// </summary>
        /// <returns>
        ///   <para>This BoundsInt.PositionEnumerator.</para>
        /// </returns>
        public Box.PositionEnumerator GetEnumerator() => this;

        /// <summary>
        ///   <para>Moves the enumerator to the next position.</para>
        /// </summary>
        /// <returns>
        ///   <para>Whether the enumerator has successfully moved to the next position.</para>
        /// </returns>
        public bool MoveNext()
        {
            if (current.z >= max.z || current.y >= max.y)
                return false;
            ++current.x;
            if (current.x >= max.x)
            {
                current.x = min.x;
                if (current.x >= max.x)
                    return false;
                ++current.y;
                if (current.y >= max.y)
                {
                    current.y = min.y;
                    ++current.z;
                    if (current.z >= max.z)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        ///   <para>Resets this enumerator to its starting state.</para>
        /// </summary>
        public void Reset()
        {
            current = min;
            --current.x;
        }

        /// <summary>
        ///   <para>Current position of the enumerator.</para>
        /// </summary>
        public Vector3 Current => current;

        object IEnumerator.Current => Current;

        void IDisposable.Dispose()
        {
        }
    }
    
    public static Box CalculateBounds(Mesh mesh)
    {
        Vector3 min = new Vector3(
            mesh.bounds.min.x,
            mesh.bounds.min.y,
            mesh.bounds.min.z);

        Vector3 max = new Vector3(
            mesh.bounds.max.x,
            mesh.bounds.max.y,
            mesh.bounds.max.z);

        Vector3 position = new Vector3(
            mesh.bounds.center.x,
            mesh.bounds.center.y,
            mesh.bounds.center.z);

        Vector3 size = new Vector3(
            mesh.bounds.size.x,
            mesh.bounds.size.y,
            mesh.bounds.size.z);

        float x = position.x;
        float y = position.y;
        float z = position.z;

        float xMin = Math.Min(position.x, position.x + size.x);
        float yMin = Math.Min(position.y, position.y + size.y);
        float zMin = Math.Min(position.z, position.z + size.z);

        float xMax = Math.Max(position.x, position.x + size.x);
        float yMax = Math.Max(position.y, position.y + size.y);
        float zMax = Math.Max(position.z, position.z + size.z);

        return new Box
        {
            Position = position,
            Size = size,
            x = x,
            y = y,
            z = z,
            Min = min,
            Max = max,
            xMin = xMin,
            yMin = yMin,
            zMin = zMin,
            xMax = xMax,
            yMax = yMax,
            zMax = zMax
        };
    }
}