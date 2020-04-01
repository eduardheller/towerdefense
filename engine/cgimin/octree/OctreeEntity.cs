using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.cgimin.object3d;
using Engine.cgimin.material;
using OpenTK;

namespace Engine.cgimin.octree
{
    public class OctreeEntity
    {

        public BaseObject3D Object3d;
        public BaseMaterial Material;
        public MaterialSettings MaterialSetting;
        public Matrix4 Transform;

        public bool drawn;

        public OctreeEntity(BaseObject3D object3d, BaseMaterial material, MaterialSettings materialSetting, Matrix4 transform)
        {
            Object3d = object3d;
            Material = material;
            MaterialSetting = materialSetting;
            Transform = transform;
            drawn = false;
        }

    }
}
