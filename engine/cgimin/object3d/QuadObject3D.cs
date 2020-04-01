using Engine.cgimin.object3d;
using OpenTK;

namespace Engine.cgimin.object3d
{
    public class QuadObject3D : BaseObject3D
    {

        public QuadObject3D()
        {
            addTriangle(new Vector3(1, -1, 0), new Vector3(1, 1, 0), new Vector3(-1, -1, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 0));
            addTriangle(new Vector3(-1, -1, 0), new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1));
            CreateVAO();
        }

    }
}
