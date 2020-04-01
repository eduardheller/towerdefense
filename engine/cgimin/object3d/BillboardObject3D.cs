using System.Collections.Generic;
using OpenTK;

namespace Engine.cgimin.object3d
{
    public class BillboardObject3D : BaseObject3D
    {

        public BillboardObject3D (float width, float height)
        {
            float halfSize = width * 0.5f;
            float halfHeight = height * 0.5f;

            addTriangle(new Vector3(-halfSize, -halfHeight, 0), new Vector3(halfSize, -halfHeight, 0), new Vector3(halfSize, halfHeight, 0),
                        new Vector3(0, 0,-1), new Vector3(0, 0,-1), new Vector3(0, 0,-1),
                        new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0));

            addTriangle(new Vector3(-halfSize, -halfHeight, 0), new Vector3(halfSize, halfHeight, 0), new Vector3(-halfSize, halfHeight, 0),
                        new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1),
                        new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0));

            CreateVAO();
        }

    }
}
