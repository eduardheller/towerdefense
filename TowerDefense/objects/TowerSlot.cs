using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Engine.cgimin.object3d;
using Engine.cgimin.material.normalmappingshadow;
using Engine.cgimin.material.textureglow;

namespace TowerDefense.objects
{
    public class TowerSlot : GameObject
    {
        private float _size;
        private int _id;
        private static int _idCounter = 0;
        private bool _isMouseOver;
        private Tower _tower;

        public bool IsMouseOver
        {
            get { return _isMouseOver; }
            set { _isMouseOver = value; }
        }

        public float Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public int ID
        {
            get { return _id; }
        }

        public Tower Tower
        {
            get { return _tower; }
            set { _tower = value; }
        }

        public Vector3[] AABB()
        {
            Vector3[] aabb = new Vector3[2];
            aabb[1] = this.Position + new Vector3(-1,0.7f, 1 );
            aabb[0] = this.Position + new Vector3(1, 1, -1);
            return aabb;

        }

        public TowerSlot()
        {
            _tower = null;
            _id = _idCounter;
            _idCounter++;
            _isMouseOver = false;
            _obj = ResourceManager.Objects["BLOCK_1"];
        }

        public override void Render(FrameEventArgs e)
        {

        }
    }
}
