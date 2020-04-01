using OpenTK;
using System.Collections.Generic;
using TowerDefense.objects.projectiles;
using TowerDefense.particles;
using TowerDefense.particles.particleemmiter;
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.sound;

namespace TowerDefense.objects.towers
{
    class ArrowTower : Tower
    {
        private const float Y_OFFSET_TURRET = 2f;
        private const float Y_OFFSET_BASE = 1.35f;
        private readonly Vector3 YOFFSET_PROJECTILE = new Vector3(0, 2.0f, 0);
        public static int StartCosts = 50;
        private float SCALE = 0.4f;
        private float SCALE_TURRET = 0.4f;
        private Vector3 _shootDistance;
        private Vector3 _shootDistanceTarget;
        private ParticleSystem _particleSystemHit;
        private Sound _shootSound;
        private Sound _shootSoundGround;

        public ArrowTower(Vector3 pos) : base(150, 6, 1000, StartCosts, pos)
        {
            Description = "Arrow Tower";
            AttackDescription = "Single";

            LoadObjectFiles();
            SetPosition(pos);
            _shootDistance = Vector3.Zero;
            _shootDistanceTarget = Vector3.Zero;

            _particleSystemHit = new ParticleArrowHit(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_11"], 4, 4,
                BlendingFactorSrc.DstColor, BlendingFactorDest.OneMinusSrcAlpha),
                1, 0.3f, 0, 2f);

            _shootSound = new Sound(ResourceManager.Sounds["ARROW"]);
            _shootSoundGround = new Sound(ResourceManager.Sounds["ARROW_GROUND"]);

            _shootSound.Gain = 0.8f;
            _shootSoundGround.Gain = 0.3f;
        }

        public override void SetPosition(Vector3 pos)
        {
            _turretMatrix = Matrix4.CreateScale(SCALE_TURRET);
            _turretMatrix *= Matrix4.CreateTranslation(pos + new Vector3(0, Y_OFFSET_TURRET, 0));
            _baseMatrix = Matrix4.CreateScale(SCALE);
            _baseMatrix *= Matrix4.CreateTranslation(pos + new Vector3(0, Y_OFFSET_BASE, 0));
            _hubMatrix = Matrix4.CreateScale(SCALE);
            _hubMatrix *= Matrix4.CreateTranslation(pos + new Vector3(0, Y_OFFSET_BASE, 0));
            Transformation = Matrix4.CreateScale(SCALE);
            Transformation *= Matrix4.CreateTranslation(pos);
            _position = pos;
        }

        public override void Rotate(FrameEventArgs e, Enemy target)
        {
            if (target != null)
            {
                Vector3 distance = target.Position - _position;
                distance.Normalize();

                _turretMatrix = Matrix4.Identity;
                _turretMatrix *= Matrix4.CreateScale(SCALE_TURRET);
                _turretMatrix *= Matrix4.CreateFromQuaternion(GetRotation(Vector3.UnitZ, distance, Vector3.UnitY));
                _turretMatrix *= Matrix4.CreateTranslation(_position + new Vector3(0, Y_OFFSET_TURRET, 0) + _shootDistance);

                _hubMatrix = Matrix4.Identity;
                _hubMatrix *= Matrix4.CreateScale(SCALE);
                _hubMatrix *= Matrix4.CreateFromQuaternion(GetRotation(Vector3.UnitZ, distance, Vector3.UnitY));
                _hubMatrix *= Matrix4.CreateTranslation(_position + new Vector3(0, Y_OFFSET_BASE, 0));

            }
        }

        public override void CreateProjectile(FrameEventArgs e, Enemy target, List<Enemy> enemies)
        {
            if (target != null)
            {
                Vector3 distanceLow = (target.Position - _position).Normalized();
                
                Vector3 distance = target.Position+Vector3.UnitY*2 - _position - new Vector3(0, Y_OFFSET_TURRET + 1.3f, 0);
                distance.Normalize();
                List<Enemy> enemiesdmg = new List<Enemy>();
                enemiesdmg.Add(target);
    
                Projectile proj = new ArrowProjectile(enemiesdmg, target.Position, _position + YOFFSET_PROJECTILE + distance, 5f, 
                    new Vector3(15, 15, 2), GetRotation(Vector3.UnitZ, distance, Vector3.UnitY));
                _projectiles.Add(proj);

                _shootDistance = Vector3.Zero;
                _shootDistanceTarget = -distanceLow * (1 / 3f);

                _shootSound.SetPosition(_position);
                _shootSound.Play();
            }
        }

        public override void UpgradeModifier()
        {
            Strength += Strength;
            Speed -= 20;
            Radius += 0.1f;
            _upgradeCost += GetUpgradeCost();
            LoadObjectFiles();
        }

        public override int GetUpgradeCost()
        {
            return (int)(_upgradeCost*1.3f);
        }

        protected override void LoadObjectFiles()
        {
            SCALE_TURRET = Level * 0.005f + SCALE_TURRET;
            SetPosition(_position);
            if (Level <= 8)
            {
                _objGround = ResourceManager.Objects["TOWER_GROUND_" + Level];
            }
            else
            {
                _objGround = ResourceManager.Objects["TOWER_GROUND_8"];
            }

            _textureTurret = ResourceManager.Textures["TOWER_ARROW"];
            _textureGround = ResourceManager.Textures["TOWER_BASE_1"];

            if (Level <= 2)
            {
                _objTurret = ResourceManager.Objects["TOWER_ARROW_TURRET_1"];
                _objBase = ResourceManager.Objects["TOWER_ARROW_BASE_1"];
                _objHub = ResourceManager.Objects["TOWER_ARROW_HUB_1"];


            }
            else if (Level <= 4 && Level > 2)
            {
                _objTurret = ResourceManager.Objects["TOWER_ARROW_TURRET_2"];
                _objBase = ResourceManager.Objects["TOWER_ARROW_BASE_2"];
                _objHub = ResourceManager.Objects["TOWER_ARROW_HUB_2"];

            }
            else if (Level > 4)
            {
                _objTurret = ResourceManager.Objects["TOWER_ARROW_TURRET_3"];
                _objBase = ResourceManager.Objects["TOWER_ARROW_BASE_3"];
                _objHub = ResourceManager.Objects["TOWER_ARROW_HUB_3"];
                _textureGround = ResourceManager.Textures["TOWER_BASE_2"];
            }
        }

        public override void Update(FrameEventArgs e)
        {
            float lerpTime = (float)e.Time;
            _shootDistance = Vector3.Lerp(_shootDistance, _shootDistanceTarget, lerpTime*5f);
            if ((_shootDistance - _shootDistanceTarget).Length < 0.1f)
            {
                _shootDistanceTarget = Vector3.Zero;
            }

            if (CurrentTarget != null)
            {

                foreach (Projectile projectile in _projectiles)
                {
                    if (projectile.HasReached)
                    {
                        _particleSystemHit.CreateWithTime(e, CurrentTarget.Position + new Vector3(0, 0.6f, 0), Vector3.Zero, 1f);
                        _shootSoundGround.SetPosition(_position);
                        _shootSoundGround.Play();
                    }

                }

            }
            _particleSystemHit.Update(e);
  
        }
    }
}
