using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Engine.cgimin.helpers
{
    public class GeometryHelpers
    {

        
        public static bool SphereAARectangleIntersect(Vector3 sphereCenter, float sphereRadius, Vector3 rectMin, Vector3 rectMax)
        {
            if (sphereCenter.X + sphereRadius < rectMin.X) return false;
            if (sphereCenter.Y + sphereRadius < rectMin.Y) return false;
            if (sphereCenter.Z + sphereRadius < rectMin.Z) return false;

            if (sphereCenter.X - sphereRadius > rectMax.X) return false;
            if (sphereCenter.Y - sphereRadius > rectMax.Y) return false;
            if (sphereCenter.Z - sphereRadius > rectMax.Z) return false;

            // noch nicht komplett!!

            return true;
        }

        public static bool RayAABBIntersect(Vector3 invertedray, Vector3 origin, Vector3[] aabb, out float length)
        {

            // lb is the corner of AABB with minimal coordinates - left bottom, rt is maximal corner
            // r.org is origin of ray
            float t1 = (aabb[0].X - origin.X) * invertedray.X;
            float t2 = (aabb[1].X - origin.X) * invertedray.X;
            float t3 = (aabb[0].Y - origin.Y) * invertedray.Y;
            float t4 = (aabb[1].Y - origin.Y) * invertedray.Y;
            float t5 = (aabb[0].Z - origin.Z) * invertedray.Z;
            float t6 = (aabb[1].Z - origin.Z) * invertedray.Z;

            float tmin = Math.Max(Math.Max(Math.Min(t1, t2), Math.Min(t3, t4)), Math.Min(t5, t6));
            float tmax = Math.Min(Math.Min(Math.Max(t1, t2), Math.Max(t3, t4)), Math.Max(t5, t6));

            // if tmax < 0, ray (line) is intersecting AABB, but whole AABB is behing us
            if (tmax < 0)
            {
                length = tmax;
                return false;
            }

            // if tmin > tmax, ray doesn't intersect AABB
            if (tmin > tmax)
            {
                length = tmax;
                return false;
            }

            length = tmin;
            return true;
        }

   

        public static bool RayTriangleIntersect(Vector3 p1, Vector3 p2, Vector3 p3, 
                                                Vector3 origin,  Vector3 dir, out float distance)

        {
            distance = 0;
            Vector3 e1, e2;
            Vector3 p, q, t;
            float det, invDet, u, v;

            e1 = p2 - p1;
            e2 = p3 - p1;

            p = Vector3.Cross(dir, e2);
            det = Vector3.Dot(e1, p);

            if (det > -float.Epsilon && det < float.Epsilon) { return false; }
            invDet = 1.0f / det;

            t = origin - p1;
            u = Vector3.Dot(t, p) * invDet;

            if (u < 0 || u > 1) { return false; }

            q = Vector3.Cross(t, e1);

            v = Vector3.Dot(dir, q) * invDet;

            if (v < 0 || u + v > 1) { return false; }

            distance = (Vector3.Dot(e2, q) * invDet);

            if (distance > float.Epsilon)
            {
                return true;
            }
            return false;
        }



        public static float PlaneDistanceToPoint(Vector3 normal, float d, Vector3 pt)
        {
            return Vector3.Dot(normal, pt) + d;
        }


        public static Vector3 NearestPointOnPlane(Vector3 normal, float d, Vector3 pt)
        {
            float dist = PlaneDistanceToPoint(normal, d, pt);
            return pt - normal * dist;
        }


        public static Vector3 NearestPointOnLine(Vector3  a, Vector3  b,  Vector3  Point)
        {
	        Vector3 c = Point - a;
            Vector3 v = (b - a).Normalized(); 
            float d = (b - a).Length;
            float t = Vector3.Dot(v, c);

            if (t < 0) return a;
	        if (t > d) return b;
	        v *= t;

	        return a + v;
        }

        private static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

    }
}
