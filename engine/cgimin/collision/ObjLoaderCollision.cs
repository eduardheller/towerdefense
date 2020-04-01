using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTK;

namespace Engine.cgimin.collision
{
    public class ObjLoaderCollision
    {
        private struct TriangleData
        {
            public Vector3 p1, p2, p3;
            public int collisionID;
        }

        private List<TriangleData> collisionTriangles;

        public ObjLoaderCollision(String filePath, float scaleFactor = 1.0f, int idOverWrite = -1)
        {
            collisionTriangles = new List<TriangleData>();

            List<Vector3> v = new List<Vector3>();
            List<Vector2> vt = new List<Vector2>();

            var input = File.ReadLines(filePath);

            foreach (string line in input)
            {
                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    if (parts[0] == "v") v.Add(new Vector3(float.Parse(parts[1], CultureInfo.InvariantCulture) * scaleFactor, float.Parse(parts[2], CultureInfo.InvariantCulture) * scaleFactor, float.Parse(parts[3], CultureInfo.InvariantCulture) * scaleFactor));
                    if (parts[0] == "vt") vt.Add(new Vector2(float.Parse(parts[1], CultureInfo.InvariantCulture), 1.0f - float.Parse(parts[2], CultureInfo.InvariantCulture)));

                    if (parts[0] == "f")
                    {
                        string[] triIndicesV1 = parts[1].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] triIndicesV2 = parts[2].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] triIndicesV3 = parts[3].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                        int id;

                        if (idOverWrite >= 0)
                        {
                            id = idOverWrite;
                        }
                        else
                        {
                            Vector2 uv = vt[Convert.ToInt32(triIndicesV1[1]) - 1];
                            id = (int)(uv.X / 0.25f) + (int)(uv.Y / 0.25f) * 4;
                        }

                        addCollisionTriangle(v[Convert.ToInt32(triIndicesV1[0]) - 1], v[Convert.ToInt32(triIndicesV2[0]) - 1], v[Convert.ToInt32(triIndicesV3[0]) - 1], id);
                    }
                }
            }

        }


        private void addCollisionTriangle(Vector3 p1, Vector3 p2, Vector3 p3, int collisionID)
        {
            TriangleData triangle = new TriangleData();
            triangle.p1 = p1;
            triangle.p2 = p2;
            triangle.p3 = p3;
            triangle.collisionID = collisionID;
            collisionTriangles.Add(triangle);
        }


        public void AddToCollisionContainer(BaseCollisionContainer container, Matrix4 transform)
        {
            int len = collisionTriangles.Count - 1;

            for (int i = 0; i < len; i++) {
                container.AddTriangleToCollision((new Vector4(collisionTriangles[i].p1, 1.0f) * transform).Xyz,
                                                 (new Vector4(collisionTriangles[i].p2, 1.0f) * transform).Xyz,
                                                 (new Vector4(collisionTriangles[i].p3, 1.0f) * transform).Xyz, collisionTriangles[i].collisionID);
            }

        } 

    }
}
