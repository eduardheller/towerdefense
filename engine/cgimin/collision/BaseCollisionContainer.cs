using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Engine.cgimin.object3d;

namespace Engine.cgimin.collision
{

    public abstract class BaseCollisionContainer
    {

        internal List<CollisionTriangle> triangles;

        public struct CollisionTriangle
        {
            public Vector3 p1;      // Punkt-Position 1
            public Vector3 p2;      // Punkt-Position 2
            public Vector3 p3;      // Punkt-Position 3
            public Vector3 normal;  // Normale

            public float a;         // Hessesche Normalform, a,b,c,d
            public float b;
            public float c;
            public float d;         // d = Distanz zur Dreiecks-Ebene

            public int collisionID; // Kollisions-ID
        }


        public BaseCollisionContainer()
        {

        }

        public int AddTriangleToCollision(Vector3 p1, Vector3 p2, Vector3 p3, int collisionID)
        {

            triangles.Add(new CollisionTriangle());
            int index = triangles.Count - 1;
            CollisionTriangle triangle = new CollisionTriangle();
            triangle.p1 = p1;
            triangle.p2 = p2;
            triangle.p3 = p3;

            triangle.normal = Vector3.Cross(p2 - p1, p3 - p1).Normalized();

            triangle.a = triangle.normal.X;
            triangle.b = triangle.normal.Y;
            triangle.c = triangle.normal.Z;
            triangle.d = -(p1.X * triangle.a + p1.Y * triangle.b + p1.Z * triangle.c);

            triangle.collisionID = collisionID;
            triangles[index] = triangle;

            return index;
        }


        public BaseObject3D TrianglesToObject()
        {
            BaseObject3D returnObject = new BaseObject3D();
            int len = triangles.Count;
            for (int i = 0; i < len; i++)
            {
                Vector2 uv = new Vector2((triangles[i].collisionID % 4) / 4.0f + 0.125f, 0.125f);
                returnObject.addTriangle(triangles[i].p1, triangles[i].p2, triangles[i].p3, uv, uv, uv);
            }
            returnObject.CreateVAO();

            return returnObject;
        }


        public abstract List<int> GetIndicesInRadius(Vector3 position, float radius);


    }
}
