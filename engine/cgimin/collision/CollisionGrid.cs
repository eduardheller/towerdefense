using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Engine.cgimin.collision
{
    public class CollisionGrid : BaseCollisionContainer
    {

        private struct TriangleData
        {
            public Vector3 p1;
            public Vector3 p2;
            public Vector3 p3;
            public int collisionID;
        }

        private struct BoxID
        {
            public int xPos;
            public int yPos;
            public int zPos;
            public bool inside;
        }

        private List<TriangleData> data;

        private List<List<List<List<int>>>> boxes;

        private Vector3 boxMin;
        private Vector3 boxMax;

        private int xCount;
        private int yCount;
        private int zCount;

        private float xLength;
        private float yLength;
        private float zLength;

        public CollisionGrid(int xBoxCount, int yBoxCount, int zBoxCount)
        {
            data = new List<TriangleData>();

            triangles = new List<CollisionTriangle>();
            boxes = new List<List<List<List<int>>>>();

            xCount = xBoxCount;
            yCount = yBoxCount;
            zCount = zBoxCount;
        }


        public void FinalizeCollision()
        {
            boxMin = new Vector3(1000000, 1000000, 1000000);
            boxMax = new Vector3(-1000000, -1000000, -1000000);

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].p1.X < boxMin.X) boxMin.X = data[i].p1.X;
                if (data[i].p1.Y < boxMin.Y) boxMin.Y = data[i].p1.Y;
                if (data[i].p1.Z < boxMin.Z) boxMin.Z = data[i].p1.Z;

                if (data[i].p2.X < boxMin.X) boxMin.X = data[i].p2.X;
                if (data[i].p2.Y < boxMin.Y) boxMin.Y = data[i].p2.Y;
                if (data[i].p2.Z < boxMin.Z) boxMin.Z = data[i].p2.Z;

                if (data[i].p3.X < boxMin.X) boxMin.X = data[i].p3.X;
                if (data[i].p3.Y < boxMin.Y) boxMin.Y = data[i].p3.Y;
                if (data[i].p3.Z < boxMin.Z) boxMin.Z = data[i].p3.Z;


                if (data[i].p1.X > boxMax.X) boxMax.X = data[i].p1.X;
                if (data[i].p1.Y > boxMax.Y) boxMax.Y = data[i].p1.Y;
                if (data[i].p1.Z > boxMax.Z) boxMax.Z = data[i].p1.Z;

                if (data[i].p2.X > boxMax.X) boxMax.X = data[i].p2.X;
                if (data[i].p2.Y > boxMax.Y) boxMax.Y = data[i].p2.Y;
                if (data[i].p2.Z > boxMax.Z) boxMax.Z = data[i].p2.Z;

                if (data[i].p3.X > boxMax.X) boxMax.X = data[i].p3.X;
                if (data[i].p3.Y > boxMax.Y) boxMax.Y = data[i].p3.Y;
                if (data[i].p3.Z > boxMax.Z) boxMax.Z = data[i].p3.Z;
            }

            boxMin -= new Vector3(20, 20, 20);
            boxMax += new Vector3(20, 20, 20);

            xLength = (boxMax.X - boxMin.X) / xCount;
            yLength = (boxMax.Y - boxMin.Y) / yCount;
            zLength = (boxMax.Z - boxMin.Z) / zCount;

            for (int x = 0; x <= xCount; x++)
            {
                boxes.Add(new List<List<List<int>>>());
                for (int y = 0; y <= yCount; y++)
                {
                    boxes[x].Add(new List<List<int>>());
                    for (int z = 0; z <= zCount; z++)
                    {
                        boxes[x][y].Add(new List<int>());
                    }
                }
            }

            for (int i = 0; i < data.Count; i++)
            {
                AddTriangleFinal(data[i].p1, data[i].p2, data[i].p3, data[i].collisionID);
            }

        }


        private BoxID GetBoxGridPosition(Vector3 position)
        {
            BoxID ret = new BoxID();

            ret.xPos = (int)((position.X - boxMin.X) / xLength);
            ret.yPos = (int)((position.Y - boxMin.Y) / yLength);
            ret.zPos = (int)((position.Z - boxMin.Z) / zLength);

            if (position.X < boxMin.X || position.Y < boxMin.Y || position.Z < boxMin.Z ||
                position.X > boxMax.X || position.Y > boxMax.Y || position.Z > boxMax.Z)
            {
                ret.inside = false;
                return ret;
            }

            ret.inside = true;

            return ret;
        }


        public void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3, int collisionID)
        {
            TriangleData tri = new TriangleData();
            tri.p1 = p1;
            tri.p2 = p2;
            tri.p3 = p3;
            tri.collisionID = collisionID;

            data.Add(tri);
        }

        private bool AddTriangleFinal(Vector3 p1, Vector3 p2, Vector3 p3, int collisionID)
        {
            BoxID p1Box = GetBoxGridPosition(p1);
            BoxID p2Box = GetBoxGridPosition(p2);
            BoxID p3Box = GetBoxGridPosition(p3);

            if (!p1Box.inside || !p2Box.inside || !p3Box.inside) return false;

            int index = AddTriangleToCollision(p1, p2, p3, collisionID);

            int minX = Math.Min(p1Box.xPos, p2Box.xPos);
            minX = Math.Min(minX, p3Box.xPos);
            int maxX = Math.Max(p1Box.xPos, p2Box.xPos);
            maxX = Math.Max(maxX, p3Box.xPos);

            int minY = Math.Min(p1Box.yPos, p2Box.yPos);
            minY = Math.Min(minY, p3Box.yPos);
            int maxY = Math.Max(p1Box.yPos, p2Box.yPos);
            maxY = Math.Max(maxY, p3Box.yPos);

            int minZ = Math.Min(p1Box.zPos, p2Box.zPos);
            minZ = Math.Min(minZ, p3Box.zPos);
            int maxZ = Math.Max(p1Box.zPos, p2Box.zPos);
            maxZ = Math.Max(maxZ, p3Box.zPos);

            for (int x = minX; x <= maxX; x++) {
                for (int y = minY; y <= maxY; y++) {
                    for (int z = minZ; z <= maxZ; z++) {
                        boxes[x][y][z].Add(index);
                    }
                }
            }

            return true;
        }


        public override List<int> GetIndicesInRadius(Vector3 position, float radius)
        {
            Vector3 radiusAddition = new Vector3(radius, radius, radius);
            BoxID minBox = GetBoxGridPosition(position - radiusAddition);
            BoxID maxBox = GetBoxGridPosition(position + radiusAddition);

            List<int> returnList = new List<int>();

            if (minBox.inside && maxBox.inside)
            {
                for (int x = minBox.xPos; x <= maxBox.xPos; x++)
                {
                    for (int y = minBox.yPos; y <= maxBox.yPos; y++)
                    {
                        for (int z = minBox.zPos; z <= maxBox.zPos; z++)
                        {
                            returnList.AddRange(boxes[x][y][z]);
                        }
                    }
                }
            }
            return returnList.Distinct().ToList();
        }



    }
}
