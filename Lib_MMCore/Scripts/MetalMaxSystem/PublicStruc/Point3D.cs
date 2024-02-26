using System;

namespace MetalMaxSystem
{
    //结构是值类型

    /// <summary>
    /// 三维点结构体
    /// </summary>
    public struct Point3D
    {
        public double x;
        public double y;
        public double z;

        public Point3D(double newX, double newY, double newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        public void Move(double dx, double dy, double dz)
        {
            x += dx;
            y += dy;
            z += dz;
        }

        public double DistanceTo(Point3D otherPoint)
        {
            double dx = otherPoint.x - x;
            double dy = otherPoint.y - y;
            double dz = otherPoint.z - z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static Point3D operator +(Point3D point1, Point3D point2)
        {
            return new Point3D(point1.x + point2.x, point1.y + point2.y, point1.z + point2.z);
        }

        public static Point3D operator -(Point3D point1, Point3D point2)
        {
            return new Point3D(point1.x - point2.x, point1.y - point2.y, point1.z - point2.z);
        }
    }

}

