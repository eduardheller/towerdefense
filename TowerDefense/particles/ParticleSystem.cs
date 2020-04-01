using System;
using OpenTK;

namespace TowerDefense.particles
{
    public abstract class ParticleSystem
    {
        private float _pps;
        private float _speed;
        private float _gravity;
        private float _lifetime;
        private Random _random;
        private float _elapsedTime;
        private float _lastTime;
        private ParticleAtlas _textureAtlas;
        private bool _timeCreate;
        private float _timeCreateTimer;
        private float _timeCreateMaxTime;
        private Vector3 _position;
        private double _time;
        private Vector3 _velocity;

        public ParticleAtlas TextureAtlas
        {
            get { return _textureAtlas; }
            set { _textureAtlas = value; }
        }

        public Random Random
        {
            get
            {
                return _random;
            }

            set
            {
                _random = value;
            }
        }

        public float Speed
        {
            get
            {
                return _speed;
            }

            set
            {
                _speed = value;
            }
        }

        public float Gravity
        {
            get
            {
                return _gravity;
            }

            set
            {
                _gravity = value;
            }
        }

        public float Lifetime
        {
            get
            {
                return _lifetime;
            }

            set
            {
                _lifetime = value;
            }
        }

        public double Time
        {
            get
            {
                return _time;
            }

            set
            {
                _time = value;
            }
        }

        public ParticleSystem(ParticleAtlas textureAtlas, float pps, float speed, float gravity, float lifetime)
        {
            _pps = pps;
            Speed = speed;
            Gravity = gravity;
            Lifetime = lifetime;
            TextureAtlas = textureAtlas;
            Random = new Random();
            _timeCreate = false;
            Time = 0;
            _lastTime = 0.0f;
        }

        public void Create(FrameEventArgs e, Vector3 position, Vector3 velocity)
        {
            _position = position;
            _velocity = velocity;
            float perSec = 1f / _pps;
            _elapsedTime += (float)e.Time;
            if (_elapsedTime - _lastTime > perSec)
            {
                _lastTime = _elapsedTime;
                EmitParticle(e,position, velocity);
            }       
            
        }

        public void CreateWithTime(FrameEventArgs e, Vector3 position , Vector3 velocity, float duration)
        {
            _timeCreate = true;
            _position = position;
            _timeCreateMaxTime = duration;
            float perSec = 1f / _pps;
            _elapsedTime += perSec;
            _velocity = velocity;
        }

        public void Update(FrameEventArgs e)
        {
            Time += e.Time;
            if (_timeCreate)
            {
                Create(e, _position,_velocity);
                _timeCreateTimer += (float)e.Time;
                if (_timeCreateTimer >= _timeCreateMaxTime)
                {
                    _timeCreateTimer = 0.0f;
                    _timeCreate = false;
                }
            }
            
        }
        public abstract void EmitParticle(FrameEventArgs e, Vector3 position, Vector3 velocity);
    }
}
