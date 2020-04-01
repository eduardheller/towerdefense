using Engine.cgimin.sound;
using OpenTK;
using System.Collections.Generic;
using TowerDefense.objects.projectiles;
using TowerDefense.particles;
using TowerDefense.particles.particleemmiter;

namespace TowerDefense.objects.towers
{
    class TeslaTower : Tower
    {
        private const float Y_OFFSET_TURRET = 1.2f;
        private const float Y_OFFSET_BASE = 1.35f;
        private readonly Vector3 YOFFSET_PROJECTILE = new Vector3(0, 2.0f, 0);
        public static int StartCosts = 120;
        private float SCALE = 0.4f;
        private float SCALE_TURRET = 0.4f;
        private ParticleSystem _particleSystem;
        private ParticleSystem _particleSystemHit;
        private float PROJECTILE_SPEED = 10.0f;
        private Sound _shootSound;

        public TeslaTower(Vector3 pos) : base(40, 3, 100, StartCosts, pos)
        {
            Description = "Tesla Tower";
            AttackDescription = "Single, Fast";
            _particleSystem = new ParticleTeslaStaticEmmiter(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_2"], 1,1),
                15, 0.6f, 0, 0.4f);

            _particleSystemHit = new ParticleTeslaHit(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_9"], 4, 8),
                1, 0.6f, 0, 1f);

            LoadObjectFiles();
            SetPosition(pos);

            _shootSound = new Sound(ResourceManager.Sounds["TESLA"]);
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

        public override void Update(FrameEventArgs e)
        {
            _particleSystem.Create(e, Position + new Vector3(0, Y_OFFSET_TURRET + 1.3f, 0), Vector3.Zero);

            if (CurrentTarget != null)
            {
                
                foreach (Projectile projectile in _projectiles)
                {
                    if (projectile.HasReached)
                    {
                        _particleSystemHit.CreateWithTime(e, CurrentTarget.Position + new Vector3(0, 0.5f, 0), Vector3.Zero, 1f);
                    }

                }


            }
            _particleSystemHit.Update(e);

        }
        public override void Rotate(FrameEventArgs e, Enemy target)
        {
            // Nicht rotieren
        }

        public override void CreateProjectile(FrameEventArgs e, Enemy target, List<Enemy> enemies)
        {
            if (target != null)
            {
                Vector3 distance = target.Position - _position;
                distance.Normalize();
                List<Enemy> enemiesdmg = new List<Enemy>();
                enemiesdmg.Add(target);
                Projectile proj = new TeslaProjectile(enemiesdmg, target.Position, _position + new Vector3(0, Y_OFFSET_TURRET + 1.3f, 0), PROJECTILE_SPEED);
                _projectiles.Add(proj);

                if (!_shootSound.IsPlaying())
                {
                    _shootSound.SetPosition(_position);
                    _shootSound.Play();
                }

            }
            else
            {
                if (_shootSound.IsPlaying())
                {
                    _shootSound.Stop();
                }
            }
        }

        public override void UpgradeModifier()
        {
            Strength += Strength;
            Speed -= 5;
            Radius += 0.1f;
            _upgradeCost += GetUpgradeCost();
            LoadObjectFiles();
        }

        public override int GetUpgradeCost()
        {
            return (int)(_upgradeCost * 1.3f);
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

            _textureTurret = ResourceManager.Textures["TOWER_TESLA"];
            _textureGround = ResourceManager.Textures["TOWER_BASE_1"];

            if (Level <= 2)
            {
                _objTurret = ResourceManager.Objects["TOWER_TESLA_TURRET_1"];
            }
            else if (Level <= 4 && Level > 2)
            {
                _objTurret = ResourceManager.Objects["TOWER_TESLA_TURRET_2"];

            }
            else if (Level > 4)
            {
                _objTurret = ResourceManager.Objects["TOWER_TESLA_TURRET_3"];
                _textureGround = ResourceManager.Textures["TOWER_BASE_2"];
            }
        }

    }
}
