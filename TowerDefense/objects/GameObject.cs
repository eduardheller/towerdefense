using OpenTK;
using Engine.cgimin.object3d;

namespace TowerDefense.objects
{
    public abstract class GameObject
    {
        protected Vector3 _position;
        private Matrix4 _transformation;
        protected BaseObject3D _obj;

        public BaseObject3D Object
        {
            get { return _obj; }
            set { _obj = value; }
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Matrix4 Transformation
        {
            get { return _transformation; }
            set { _transformation = value; }
        }

        public GameObject()
        {
            _obj = null;
            _transformation = Matrix4.Identity;
            _position = Vector3.Zero;
        }

        public abstract void Render(FrameEventArgs e);
    }
}
