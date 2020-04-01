using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Engine.cgimin.helpers
{
    public class Lerps
    {
        public static float CurveAngle(float from, float to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            Vector2 fromVector = new Vector2((float)Math.Cos(from), (float)Math.Sin(from));
            Vector2 toVector = new Vector2((float)Math.Cos(to), (float)Math.Sin(to));

            Vector2 currentVector = Slerp(fromVector, toVector, step);

            return (float)Math.Atan2(currentVector.Y, currentVector.X);
        }

        public static Vector2 Slerp(Vector2 from, Vector2 to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            double theta = Math.Acos(Vector2.Dot(from, to));
            if (theta == 0) return to;

            double sinTheta = Math.Sin(theta);
            return (float)(Math.Sin((1 - step) * theta) / sinTheta) * from + (float)(Math.Sin(step * theta) / sinTheta) * to;
        }

        public static Vector3 Slerp(Vector3 start, Vector3 end, float percent)
        {
            // Dot product - the cosine of the angle between 2 vectors.
            float dot = Vector3.Dot(start, end);
            // Clamp it to be in the range of Acos()
            // This may be unnecessary, but floating point
            // precision can be a fickle mistress.
            dot = Clamp(dot, -1.0f, 1.0f);
            // Acos(dot) returns the angle between start and end,
            // And multiplying that by percent returns the angle between
            // start and the final result.
            float theta = (float)Math.Acos(dot) * percent;
            Vector3 RelativeVec = end - start * dot;
            RelativeVec.Normalize();     // Orthonormal basis
                                         // The final result.
            return ((start * (float)Math.Cos(theta)) + (RelativeVec * (float)Math.Sin(theta)));
        }

        public static float Clamp(float val, float min, float max) 
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static Vector3 GetInterpPosition(float t, Vector3 p1, Vector3 p2, Vector3 m1, Vector3 m2)
        {
            float h1 = 2 * t * t * t - 3 * t * t + 1;
            float h2 = -2 * t * t * t + 3 * t * t;
            float h3 = t * t * t - 2 * t * t + t;
            float h4 = t * t * t - t * t;
            return (h1 * p1 + h2 * p2 + h3 * m1 + h4 * m2);
        }

        public static Vector3 GetInterpTangent(float t, Vector3 p1, Vector3 p2, Vector3 m1, Vector3 m2)
        {
            float h1 = 6 * t * t - 6 * t;
            float h2 = -6 * t * t + 6 * t;
            float h3 = 3 * t * t - 4 * t + 1;
            float h4 = 3 * t * t - 2 * t;
            return (h1 * p1 + h2 * p2 + h3 * m1 + h4 * m2).Normalized();
        }
    }
}
