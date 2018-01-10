using System;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct Vector4Int : IEquatable<Vector4Int>, IFormattable
{
    public static readonly Vector4Int zero = new Vector4Int(0, 0, 0, 0);
    public static readonly Vector4Int one = new Vector4Int(1, 1, 1, 1);
    
    public int x;
    public int y;
    public int z;
    public int w;

    public Vector4Int(int x, int y, int z, int w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Vector4Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = 0;
    }

    public override int GetHashCode()
    {
        var hashCode = this.x.GetHashCode();
        hashCode = (hashCode * 397) ^ this.y.GetHashCode();
        hashCode = (hashCode * 397) ^ this.z.GetHashCode();
        hashCode = (hashCode * 397) ^ this.w.GetHashCode();
        return hashCode;
    }

    public int this[int index]
    {
        get
        {
            if (index == 0) { return this.x; }
            if (index == 1) { return this.y; }
            if (index == 2) { return this.z; }
            if (index == 2) { return this.w; }
            throw new ArgumentOutOfRangeException("Invalid Vector4Int index!");
        }

        set
        {
            if (index == 0) { this.x = value; }
            if (index == 1) { this.y = value; }
            if (index == 2) { this.z = value; }
            if (index == 3) { this.w = value; }
            throw new ArgumentOutOfRangeException("Invalid Vector4Int index!");
        }
    }

    #region conversion
    public static explicit operator Vector4(Vector4Int v) { return new Vector4(v.x, v.y, v.z, v.w); }
    public static explicit operator Vector3(Vector4Int v) { return new Vector3(v.x, v.y, v.w); }
    #endregion

    #region math operations
    public float magnitude
    {
        get { return this.Magnitude(this); }
    }

    public int sqrMagnitude
    {
        get { return this.SqrMagnitude(this); }
    }

    public int dotOne
    {
        get { return Vector4Int.DotOne(this); }
    }

    public float Magnitude(Vector4Int v)
    {
        return Mathf.Sqrt(this.SqrMagnitude(v));
    }

    public int SqrMagnitude(Vector4Int v)
    {
        return v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w;
    }

    public static Vector4Int Scale(Vector4Int l, Vector4Int r)
    {
        return new Vector4Int(l.x * r.x, l.y * r.y, l.z * r.z, l.w * r.w);
    }

    public static int Dot(Vector4Int l, Vector4Int r)
    {
        return l.x * r.x + l.y * r.y + l.z * r.z + l.w * r.w;
    }

    public static int DotOne(Vector4Int v)
    {
        return v.x * v.y * v.z * v.w;
    }
    
    public static Vector4Int Min(Vector4Int l, Vector4Int r)
    {
        return new Vector4Int(Mathf.Min(l.x, r.x), Mathf.Min(l.y, r.y), Mathf.Min(l.z, r.z), Mathf.Min(l.w, r.w));
    }

    public static Vector4Int Max(Vector4Int l, Vector4Int r)
    {
        return new Vector4Int(Mathf.Max(l.x, r.x), Mathf.Max(l.y, r.y), Mathf.Max(l.z, r.z), Mathf.Max(l.w, r.w));
    }

    public static Vector4Int Clamp(Vector4Int v, Vector4Int min, Vector4Int max)
    {
        return new Vector4Int(Mathf.Clamp(v.x, min.x, max.x),
                        Mathf.Clamp(v.y, min.y, max.y),
                        Mathf.Clamp(v.z, min.z, max.z),
                        Mathf.Clamp(v.w, min.w, max.w));
    }

    public static Vector4Int ClosestPowerOfTwo(Vector4Int v)
    {
        return new Vector4Int(Mathf.ClosestPowerOfTwo(v.x),
                        Mathf.ClosestPowerOfTwo(v.y),
                        Mathf.ClosestPowerOfTwo(v.z),
                        Mathf.ClosestPowerOfTwo(v.w));
    }
    #endregion math operations

    #region math operators
    public static Vector4Int operator -(Vector4Int v) { return new Vector4Int(-v.x, -v.y, -v.z, -v.w); }

    public static Vector4Int operator +(Vector4Int l, Vector4Int r) { return new Vector4Int(l.x+r.x, l.y+r.y, l.z+r.z, l.w+r.w); }
    public static Vector4Int operator -(Vector4Int l, Vector4Int r) { return new Vector4Int(l.x-r.x, l.y-r.y, l.z-r.z, l.w-r.w); }
    public static Vector4Int operator *(Vector4Int v, int d) { return new Vector4Int(v.x*d, v.y*d, v.z*d, v.w*d); }
    public static Vector4Int operator *(int d, Vector4Int v) { return new Vector4Int(v.x*d, v.y*d, v.z*d, v.w*d); }
    public static Vector4Int operator /(Vector4Int v, int d) { return new Vector4Int(v.x/d, v.y/d, v.z/d, v.w/d); }
    public static Vector4Int operator /(int d, Vector4Int v) { return new Vector4Int(d/v.x, d/v.y, d/v.z, d/v.w); }

    public static Vector4 operator +(Vector4 l, Vector4Int r) { return new Vector4(l.x+r.x, l.y+r.y, l.z+r.z, l.w+r.w); }
    public static Vector4 operator +(Vector4Int l, Vector4 r) { return new Vector4(l.x+r.x, l.y+r.y, l.z+r.z, l.w+r.w); }
    public static Vector4 operator -(Vector4 l, Vector4Int r) { return new Vector4(l.x-r.x, l.y-r.y, l.z-r.z, l.w-r.w); }
    public static Vector4 operator -(Vector4Int l, Vector4 r) { return new Vector4(l.x-r.x, l.y-r.y, l.z-r.z, l.w-r.w); }
    public static Vector4 operator *(Vector4Int v, float f) { return new Vector4(v.x*f, v.y*f, v.z*f, v.w*f); }
    public static Vector4 operator *(float f, Vector4Int v) { return new Vector4(v.x*f, v.y*f, v.z*f, v.w*f); }
    #endregion math operators

    #region comparison
    public static bool operator ==(Vector4Int l, Vector4Int r) { return l.Equals(r); }
    public static bool operator !=(Vector4Int l, Vector4Int r) { return !l.Equals(r); }

    public bool Equals(Vector4Int other)
    {
        return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
    }

    public override bool Equals(object value)
    {
        return (value is Vector4Int) ? this.Equals((Vector4Int)value) : false;
    }
    #endregion

    #region IFormattable
    public override string ToString()
    {
        return this.ToString(CultureInfo.CurrentCulture);
    }

    public string ToString(IFormatProvider formatProvider)
    {
        return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", this.x, this.y, this.z, this.w);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (format == null)
        {
            return this.ToString(formatProvider);
        }

        return string.Format(formatProvider,
                             "X:{0} Y:{1} Z:{2}, W:{3}",
                             this.x.ToString(format, formatProvider),
                             this.y.ToString(format, formatProvider),
                             this.z.ToString(format, formatProvider),
                             this.w.ToString(format, formatProvider));
    }
    #endregion
}