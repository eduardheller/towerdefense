using Engine.cgimin.object3d;
using Engine.cgimin.texture;
using Engine.cgimin.material.normalmapping;
using OpenTK;
using TowerDefense.particles;
using System.Collections.Generic;

namespace TowerDefense.objects
{
    public class Projectile : GameObject
    {
        protected int _texture;
        protected int _textureNormal;
        protected NormalMappingMaterial _normalMappingMaterial = new NormalMappingMaterial();
        private const float SHININESS = 5.0f;
        protected ParticleSystem _particleSystem;

        private Vector3 _target;
        private Vector3 _startPosition;
        private float _speed;
        private bool _reached;
        private List<Enemy> _enemies;
        private Vector3 _scale;
        private Quaternion _rotation;

        public Projectile(List<Enemy> enemies,Vector3 target, Vector3 start, float speed, Vector3 scale = default(Vector3), Quaternion rotation = default(Quaternion))
        {
            _enemies = enemies;
            _target = target;
            _startPosition = start;
            _position = start;
            _reached = false;
            _speed = speed;
            _scale = scale;
            if (rotation == default(Quaternion)) rotation = Quaternion.Identity;
            _rotation = rotation;
        }

        public void Update(FrameEventArgs e)
        {
            float move = (float)e.Time * _speed;
            Vector3 direction = _target - _startPosition;
            direction.Normalize();
            Vector3 towards = _target - _position;
            towards.Normalize();
            _position = new Vector3(_position + towards * move);

            towards = _target - _position;
            towards.Normalize();

            Vector3 dir = _position - _target;
            float length = dir.Length;
            if(length < 0.1f) _reached = true;
            
            Transformation = Matrix4.Identity;
            Transformation *= Matrix4.CreateScale(_scale);
            Transformation *= Matrix4.CreateFromQuaternion(_rotation);
            Transformation *= Matrix4.CreateTranslation(_position + towards * move);
            _particleSystem.Create(e, _position, towards);
        }

        public virtual void Freeze(Enemy enemy)
        {
            
        }

        public bool HasReached
        {
            get { return _reached; }
        }

        public List<Enemy> Enemies
        {
            get { return _enemies; }
        }

        public override void Render(FrameEventArgs e)
        {
            if(_obj != null) {
                _normalMappingMaterial.Draw(_obj, Transformation, _texture, _textureNormal, SHININESS);
            }
        }

    }
}
