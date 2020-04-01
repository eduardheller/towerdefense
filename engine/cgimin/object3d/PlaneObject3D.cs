using Engine.cgimin.object3d;
using OpenTK;

namespace Engine.cgimin.object3d
{
    public class PlaneObject3D : BaseObject3D
    {
        float size = 1;
        public PlaneObject3D()
        {
            addTriangle(new Vector3(size, 0, -size), new Vector3(-size, 0, -size), new Vector3(size, 0, size),  new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 0));
            addTriangle(new Vector3(-size, 0, -size), new Vector3(-size, 0, size), new Vector3(size, 0, size), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0));
            CreateVAO();
        }
         
    }
}
