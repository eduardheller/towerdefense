using System.Collections.Generic;
using System.Linq;
using OpenTK;
using System.Diagnostics;
using Engine.cgimin.object3d;
using Engine.cgimin.material.textureglow;
using Engine.cgimin.material.ambientdiffusespecularshadow;
using System;
using TowerDefense.particles;

namespace TowerDefense.objects
{
    public abstract class Tower : GameObject
    {
        private const float SHININESS = 32.0f;
        private AmbientDiffuseSpecularShadowMaterial _ambientdiffusespecular;
        private TextureGlowMaterial _glowMaterial;

        protected BaseObject3D _objGround;
        protected BaseObject3D _objTurret;
        protected BaseObject3D _objBase;
        protected BaseObject3D _objHub;

        protected Matrix4 _turretMatrix;
        protected Matrix4 _hubMatrix;
        protected Matrix4 _baseMatrix;

        protected int _textureGround;
        protected int _textureTurret;

        private float _strength;
        private float _radius;
        private float _speed;
        private List<Enemy> _targets;
        protected List<Projectile> _projectiles;
        private float lastShotTime;
        private Stopwatch _stopwatch;
        private Enemy _currentTarget;
        private float _scale;
        private int _cost;
        private bool _isMouseOver;
        private string _description;
        private string _attackDescription;
        private int _level;
        protected int _upgradeCost;
 

        private ParticleSystem _particleSystemExplosion;

        public Tower(float str, float rad, float spd, int cost, Vector3 pos) 
        {
            _upgradeCost = cost;
            _cost = cost;
            Level = 1;
            _projectiles = new List<Projectile>();
            _position = new Vector3(pos.X, pos.Y + 1.0f, pos.Z);
            _strength = str;
            _radius = rad;
            Speed = spd;
            lastShotTime = 0.0f;
            _stopwatch = new Stopwatch();
            _targets = new List<Enemy>();
            _stopwatch.Start();
            _ambientdiffusespecular = new AmbientDiffuseSpecularShadowMaterial();
            _glowMaterial = new TextureGlowMaterial();
            _isMouseOver = false;
            LoadObjectFiles();
        }

        public void Upgrade()
        {
            Level++;
            UpgradeModifier();
        }

        public abstract void UpgradeModifier();
        protected abstract void LoadObjectFiles();
        public abstract int GetUpgradeCost();

        public void HandleProjectiles(FrameEventArgs e, List<Enemy> enemies)
        {
            float currMsc = _stopwatch.ElapsedMilliseconds;
            if(currMsc - lastShotTime > Speed)
            {
                lastShotTime = currMsc;
                FocusTarget(e);
                CreateProjectile(e,CurrentTarget, enemies);
                
            }
            foreach (Projectile proj in _projectiles.ToList())
            {
                
                if (proj.HasReached)
                {
                    if (proj.Enemies.Count > 0)
                    {
                        foreach(Enemy enemy in proj.Enemies)
                        {
                            enemy.Damaged(Strength);
                            proj.Freeze(enemy);
                        }
                    }
                    _projectiles.Remove(proj);
                }
                proj.Update(e);
            }
        }
        public abstract void Update(FrameEventArgs e);

        public Vector3[] AABB()
        {
            Vector3[] aabb = new Vector3[2];
            aabb[1] = _position + new Vector3(-1, 0f, 1);
            aabb[0] = _position + new Vector3(1, 1f, -1);
            return aabb;
        }

        public virtual void SetPosition(Vector3 pos) { }

        public override void Render(FrameEventArgs e)
        {
            if (!_isMouseOver)
            {
                _ambientdiffusespecular.Draw(_objGround, Transformation, _textureGround, 0, SHININESS);
                _ambientdiffusespecular.Draw(_objTurret, _turretMatrix, _textureTurret, 0, SHININESS);
                if (_objBase != null) _ambientdiffusespecular.Draw(_objBase, _baseMatrix, _textureTurret, 0, SHININESS);
                if (_objHub != null) _ambientdiffusespecular.Draw(_objHub, _hubMatrix, _textureTurret, 0, SHININESS);
            }
            else
            {

                _glowMaterial.Draw(_objGround,Transformation, new Vector4(0.0f, 1.0f, 1.0f, 1f), _textureGround);
                _glowMaterial.Draw(_objTurret, TurretMatrix, new Vector4(0.0f, 1.0f, 1.0f, 1f), _textureTurret);
                if (_objBase != null) _glowMaterial.Draw(_objBase, _baseMatrix, new Vector4(0.0f, 1.0f, 1.0f, 1f), _textureTurret);
                if (_objHub != null) _glowMaterial.Draw(_objHub, HubMatrix, new Vector4(0.0f, 1.0f, 1.0f, 1f), _textureGround);
             
            }

            foreach (Projectile proj in _projectiles)
            {
                proj.Render(e);
            }
        }

        public void ClearTargets()
        {
            _targets = new List<Enemy>();
        }

        public void AddTargets(FrameEventArgs e,Enemy enemy)
        {

            Rotate(e, CurrentTarget);

            Vector3 dir = Position - enemy.Position;
            float length = dir.Length;
            if (length < Radius)
            {
                _targets.Add(enemy);
            }
        }

        protected Quaternion GetRotation(Vector3 source, Vector3 dest, Vector3 up)
        {
            float dot = Vector3.Dot(source, dest);

            if (Math.Abs(dot - (-1.0f)) < 0.000001f)
            {
                // vector a and b point exactly in the opposite direction, 
                // so it is a 180 degrees turn around the up-axis
                return new Quaternion(up, MathHelper.DegreesToRadians(0.0f));
            }
            if (Math.Abs(dot - (1.0f)) < 0.000001f)
            {
                // vector a and b point exactly in the same direction
                // so we return the identity quaternion
                return Quaternion.Identity;
            }

            float rotAngle = (float)Math.Acos(dot);
            Vector3 rotAxis = Vector3.Cross(source, dest);
            rotAxis = Vector3.Normalize(rotAxis);
            return Quaternion.FromAxisAngle(rotAxis, rotAngle);
        }


        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public float Strength
        {
            get { return _strength; }
            set { _strength = value; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public BaseObject3D ObjectGround
        {
            get { return _objGround; }
            set { _objGround = value; }
        }

        public BaseObject3D ObjectTurret
        {
            get { return _objTurret; }
            set { _objTurret = value; }
        }

        public int TextureBase
        {
            get { return _textureGround; }
            set { _textureGround = value; }
        }

        public int TextureTurret
        {
            get { return _textureTurret; }
            set { _textureTurret = value; }
        }

        public Matrix4 TurretMatrix
        {
            get { return _turretMatrix; }
        }

        public Matrix4 BaseMatrix
        {
            get{ return _baseMatrix; }
        }

        public BaseObject3D ObjectHub
        {
            get { return _objHub; }
        }

        public Matrix4 HubMatrix
        {
            get { return _hubMatrix; }
        }

        public bool IsMouseOver
        {
            get { return _isMouseOver; }
            set { _isMouseOver = value; }
        }

        public int GetSellCost()
        {
            return Level * Cost / 2;
        }
        public int Cost
        {
            get
            {
                return _cost;
            }

            set
            {
                _cost = value;
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

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        public string AttackDescription
        {
            get
            {
                return _attackDescription;
            }

            set
            {
                _attackDescription = value;
            }
        }

        public int Level
        {
            get
            {
                return _level;
            }

            set
            {
                _level = value;
            }
        }

        public Enemy CurrentTarget
        {
            get
            {
                return _currentTarget;
            }

            set
            {
                _currentTarget = value;
            }
        }

        private void FocusTarget(FrameEventArgs e)
        {
            float maxValue = float.MinValue;
            Enemy currentTarget = null;
            foreach (Enemy enemy in _targets)
            {
                float lifetime = enemy.WayRan;
                if (lifetime > maxValue)
                {
                    maxValue = lifetime;
                    currentTarget = enemy;
                }
            }
            CurrentTarget = currentTarget;

        }

        public abstract void CreateProjectile(FrameEventArgs e, Enemy target, List<Enemy> enemies);
        public abstract void Rotate(FrameEventArgs e, Enemy enemy);

    }
}
