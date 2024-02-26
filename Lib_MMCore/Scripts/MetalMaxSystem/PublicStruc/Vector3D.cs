using System;

namespace MetalMaxSystem
{
    //结构是值类型

    /// <summary>
    /// 三维向量结构体
    /// </summary>
    public struct Vector3D
    {
        public double x;
        public double y;
        public double z;

        public Vector3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3D operator +(Vector3D vec1, Vector3D vec2)
        {
            return new Vector3D(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
        }

        public static Vector3D operator -(Vector3D vec1, Vector3D vec2)
        {
            return new Vector3D(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
        }

        public static Vector3D operator *(Vector3D vec, double scalar)
        {
            return new Vector3D(vec.x * scalar, vec.y * scalar, vec.z * scalar);
        }

        public static Vector3D operator /(Vector3D vec, double scalar)
        {
            return new Vector3D(vec.x / scalar, vec.y / scalar, vec.z / scalar);
        }

        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public Vector3D Normalize()
        {
            double mag = Magnitude();
            return new Vector3D(x / mag, y / mag, z / mag);
        }
    }
}

