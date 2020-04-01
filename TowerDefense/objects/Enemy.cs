using System;
using OpenTK;
using Engine.cgimin.material.animmeshmaterial;
using TowerDefense.gui;
using TowerDefense.states;
using Engine.cgimin.sound;

namespace TowerDefense.objects
{
    public abstract class Enemy : GameObject
    {
        protected int _texture;
        private AnimMeshMaterial _animMeshMaterial;
        private AnimMeshMaterialDead _animMeshMaterialDead;

        private const float SHININESS = 32.0f;
        protected float _hp;
        private float _speed;
        private int _destWay;
        private float _wayRan;
        private bool _alive;
        private HealthBar _healthBar;
        protected float _maxhp;
        private Vector3 _rotationAxis;
        private float _rotationAngle;
        private float _scale;
        private int _id;
        private float _time;
        private float _ampl;
        private int _gold;
        private bool _isFreezed;
        private float _freezeTimer;
        private float _maxFreezeTime;
        private float _normalSpeed;
        private float _deadTimer;
        private bool _deadSequence;
        private float _deadMaxTime;
        protected Sound _deadSound;

        private bool _walkedThrough;

        public Enemy(int id, int gold,  float health, float spd, Vector3 pos, float scale, float ampl, int wave)
        {
            _id = id;
            _gold = gold;
            _scale = scale;
            _destWay = 1;
            _position = pos;
            _speed = spd;
            _hp = (1 + ((wave - 1) / 2)) * health;
            _maxhp = _hp;
            _wayRan = 0.0f;
            _alive = true;
            _animMeshMaterial = new AnimMeshMaterial();
            _animMeshMaterialDead = new AnimMeshMaterialDead();
            //_healthBar = new HealthBar(0.8f);
            _time = 0;
            _ampl = ampl;
            _alive = true;
            _freezeTimer = 0.0f;
            _maxFreezeTime = 5.0f;
            _normalSpeed = _speed;
            _deadSequence = false;
            _deadMaxTime = 5.0f;
            _deadTimer = 0.0f;
            _walkedThrough = false;
        }

        public bool IsInDeadSequence
        {
            get { return _deadSequence; }
        }

        public bool IsAlive
        {
            get { return _alive; }
        }

        public void Freeze(float freeze)
        {
            if (!_isFreezed)
            {
                _speed = _speed * (1f / freeze);
                _isFreezed = true;
            }
        }

        public void Damaged(float value)
        {
            _hp -= value;
            if (_hp <= 0)
            {
                if (!_deadSequence)
                {
                    _deadSound.SetPosition(_position);
                    _deadSound.Play();
                }
                _deadSequence = true;
            }

        }

        

        public bool Walk(FrameEventArgs e,Vector3[] wayMarks, PlayState world)
        {
            if (!IsInDeadSequence)
            {
                if (_isFreezed)
                {
                    _freezeTimer += (float)e.Time;
                    if (_freezeTimer >= _maxFreezeTime)
                    {
                        _isFreezed = false;
                        _speed = _normalSpeed;
                        _freezeTimer = 0.0f;
                    }
                }

                float move = (float)e.Time * _speed;
                Vector3 direction = wayMarks[_destWay] - wayMarks[_destWay - 1];
                direction.Normalize();
                Vector3 towards = wayMarks[_destWay] - _position;
                towards.Normalize();
                _position = new Vector3(_position + towards * move);
                _wayRan += move;
                Transformation = Matrix4.Identity;
                towards = wayMarks[_destWay] - _position;
                towards.Normalize();


                if (towards != direction)
                {
                    _position = wayMarks[_destWay];
                    _destWay++;

                    if (_destWay >= wayMarks.Length)
                    {
                        _walkedThrough = true;
                        _alive = false;
                    }
                    else
                    {
                        towards = wayMarks[_destWay] - _position;

                        towards.Normalize();

                        Vector3 distance = wayMarks[_destWay] - _position;
                        Vector3 directionA = new Vector3(0, 0, 1).Normalized();
                        Vector3 directionB = new Vector3(distance).Normalized();

                        _rotationAngle = (float)Math.Acos(Vector3.Dot(directionA, directionB));
                        _rotationAxis = Vector3.Cross(directionA, directionB).Normalized();
                    }
                }

                if (_rotationAngle > 0.0f)
                {
                    Transformation *= Matrix4.CreateFromAxisAngle(_rotationAxis, _rotationAngle);
                }

                Transformation *= Matrix4.CreateScale(_scale);
                Transformation *= Matrix4.CreateTranslation(_position + towards * move);
                return _walkedThrough;
            }
            return false;
         
        }

        public override void Render(FrameEventArgs e)
        {


            if (_deadSequence)
            {
                _deadTimer += (float)e.Time;
                _animMeshMaterialDead.Draw(_obj, Transformation, new Vector3(1, -0.5f, -0.5f), _texture, _deadTimer, _ampl, SHININESS);
                if (_deadTimer > _deadMaxTime){
                    _alive = false;
                    _deadSound.UnLoad();
                }
            }
            else
            {
                _time += (float)e.Time;
                if (!_isFreezed) _animMeshMaterial.Draw(_obj, Transformation, new Vector3(0, 0, 0), _texture, _time, _ampl, SHININESS);
                else _animMeshMaterial.Draw(_obj, Transformation, new Vector3(-0.5f, -0.5f, 1), _texture, _time, _ampl, SHININESS);
            }


            //_healthBar.Render();
        }

        public float WayRan
        {
            get { return _wayRan; }
        }

        public float MaxHP
        {
            get { return _maxhp; }
        }

        public float HP
        {
            get { return _hp; }
        }

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public int Gold
        {
            get
            {
                return _gold;
            }

            set
            {
                _gold = value;
            }
        }
    }

}
