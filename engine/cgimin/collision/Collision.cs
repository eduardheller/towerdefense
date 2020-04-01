using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Engine.cgimin.helpers;

namespace Engine.cgimin.collision
{
    public class Collision
    {

        public struct CollisionReturn {
            public bool doesCollide;    // gibt es eine Kollision?
            public Vector3 position;    // Kollisionspunkt bzw. nächster Punkt 
                                        // der Kollision bei Spherecast.

            public Vector3 normal;      // Die Normale des Kollisions-Dreiecks
            public int collisionID;     // Die Kollisions-ID
            public float d;             // Distance, HNF
        }


        public static CollisionReturn Linecast(BaseCollisionContainer container, Vector3 p1, Vector3 p2, int filterID = -1)
        {
            CollisionReturn colReturn = new CollisionReturn();
            colReturn.doesCollide = false;

            Vector3 mid = (p1 + p2) / 2.0f;
            float maxDistance = (p2 - p1).Length;
            Vector3 dir = (p2 - p1).Normalized();

            List<int> indices = container.GetIndicesInRadius(mid, maxDistance / 2.0f);
            int len = indices.Count;
            //Console.WriteLine(len);
            for (int i = 0; i < len; i++) {
                int index = indices[i];
                if (container.triangles[index].collisionID == filterID || filterID == -1)
                {
                    float distance;
                    if (GeometryHelpers.RayTriangleIntersect(container.triangles[index].p1, container.triangles[index].p2, container.triangles[index].p3, p1, dir, out distance))
                    {
                        if (distance < maxDistance)
                        {
                            maxDistance = distance;
                            colReturn.doesCollide = true;
                            colReturn.position = p1 + dir * distance;
                            colReturn.normal = container.triangles[index].normal;
                            colReturn.collisionID = container.triangles[index].collisionID;
                            colReturn.d = container.triangles[index].d;
                        }
                    }
                }
            }
            return colReturn;
        }

        
        public static CollisionReturn Spherecast(BaseCollisionContainer container, Vector3 pos, float radius, int filterID = -1)
        {
            CollisionReturn colReturn = new CollisionReturn();
            colReturn.doesCollide = false;

            List<int> indices = container.GetIndicesInRadius(pos, radius);
            int len = indices.Count;

            float maxDistance = radius;
            
            for (int i = 0; i < len; i++)
            {
                int index = indices[i];
                if (container.triangles[index].collisionID == filterID || filterID == -1)
                {
                    float distPlane = GeometryHelpers.PlaneDistanceToPoint(container.triangles[index].normal, container.triangles[index].d, pos);
                    if (Math.Abs(distPlane) < maxDistance)
                    {
                        // Distanz ist im radius
                        Vector3 nearPlanePoint = GeometryHelpers.NearestPointOnPlane(container.triangles[index].normal, container.triangles[index].d, pos);
                        Vector3 dir = (nearPlanePoint - pos).Normalized();
                        float triangleDist;

                        if (GeometryHelpers.RayTriangleIntersect(container.triangles[index].p1, container.triangles[index].p2, container.triangles[index].p3, pos, dir, out triangleDist))
                        {
                            // Alles klar, der near plane Point liegt im Dreieck
                            colReturn.doesCollide = true;
                            colReturn.position = nearPlanePoint;
                            colReturn.collisionID = container.triangles[index].collisionID;
                            colReturn.normal = container.triangles[index].normal;
                            colReturn.d = container.triangles[index].d;
                            maxDistance = Math.Abs(distPlane);
                        }
                        else
                        {
                            
                            // Es kann auch sein, dass der nächste Punkt auf zur Mitte auf einer Kante des Dreiecks leigt...
                            Vector3 nearLine1 = GeometryHelpers.NearestPointOnLine(container.triangles[index].p1, container.triangles[index].p2, pos);
                            Vector3 nearLine2 = GeometryHelpers.NearestPointOnLine(container.triangles[index].p2, container.triangles[index].p3, pos);
                            Vector3 nearLine3 = GeometryHelpers.NearestPointOnLine(container.triangles[index].p3, container.triangles[index].p1, pos);

                            if ((nearLine1 - pos).LengthSquared < maxDistance * maxDistance)
                            {
                                colReturn.doesCollide = true;
                                colReturn.position = nearLine1;
                                colReturn.collisionID = container.triangles[index].collisionID;
                                colReturn.normal = container.triangles[index].normal;
                                colReturn.d = container.triangles[index].d;
                                maxDistance = (nearLine1 - pos).Length;
                            }

                            if ((nearLine2 - pos).LengthSquared < maxDistance * maxDistance)
                            {
                                colReturn.doesCollide = true;
                                colReturn.position = nearLine2;
                                colReturn.collisionID = container.triangles[index].collisionID;
                                colReturn.normal = container.triangles[index].normal;
                                colReturn.d = container.triangles[index].d;
                                maxDistance = (nearLine2 - pos).Length;
                            }

                            if ((nearLine3 - pos).LengthSquared < maxDistance * maxDistance)
                            {
                                colReturn.doesCollide = true;
                                colReturn.position = nearLine3;
                                colReturn.collisionID = container.triangles[index].collisionID;
                                colReturn.normal = container.triangles[index].normal;
                                colReturn.d = container.triangles[index].d;
                                maxDistance = (nearLine3 - pos).Length;
                            }
                            
                        }
                    }
                }
            }

            return colReturn;
        }
        

    }
}
