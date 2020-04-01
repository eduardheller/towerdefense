using System;
using System.Collections.Generic;
using OpenTK;
using TowerDefense.objects.projectiles;
using TowerDefense.particles.particleemmiter;
using TowerDefense.particles;
using Engine.cgimin.sound;

namespace TowerDefense.objects.towers
{
    class MageTower : Tower
    {
        private const float Y_OFFSET_TURRET = 1.5f;
        private const float Y_OFFSET_BASE = 1.35f;
        private readonly Vector3 YOFFSET_PROJECTILE = new Vector3(0, 2.0f, 0);
        public static int StartCosts = 100;
        private float SCALE = 0.4f;
        private float SCALE_TURRET = 0.4f;
        private float _timer;
        private ParticleSystem _particleSystemHit;
        private Sound _shootSound;
        private Sound _shootSoundGround;

        public MageTower(Vector3 pos) : base(70, 4, 1000, StartCosts, pos)
        {
            Description = "Mage Tower";
            AttackDescription = "Freeze, Single";

            LoadObjectFiles();
            SetPosition(pos);
            _timer = 0;

            _particleSystemHit = new ParticleMageEmitter(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_10"], 1, 1),
                15, 1.0f, 0, 1f);

            _shootSound = new Sound(ResourceManager.Sounds["MAGE"]);
            _shootSoundGround = new Sound(ResourceManager.Sounds["MAGE_GROUND"]);

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
            
        }

        public override void CreateProjectile(FrameEventArgs e, Enemy target, List<Enemy> enemies)
        {
            if (target != null)
            {
                Vector3 distance = target.Position - _position;
                distance.Normalize();
                List<Enemy> enemiesdmg = new List<Enemy>();
                enemiesdmg.Add(target);
                Projectile proj = new MageProjectile(enemiesdmg, target.Position, _position + YOFFSET_PROJECTILE + distance, 5f);
                _projectiles.Add(proj);
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

            _textureTurret = ResourceManager.Textures["TOWER_MAGE"];
            _textureGround = ResourceManager.Textures["TOWER_BASE_1"];
            _objTurret = ResourceManager.Objects["TOWER_MAGE_TURRET_1"];

            if (Level <= 2)
            {
                _objBase = ResourceManager.Objects["TOWER_MAGE_BASE_1"];
            }
            else if (Level <= 4 && Level > 2)
            {
                _objBase = ResourceManager.Objects["TOWER_MAGE_BASE_2"];

            }
            else if (Level > 4)
            {
                _objBase = ResourceManager.Objects["TOWER_MAGE_BASE_3"];
                _textureGround = ResourceManager.Textures["TOWER_BASE_2"];
            }
        }

        public override void Update(FrameEventArgs e)
        {
            _timer += (float)e.Time;
            _turretMatrix = Matrix4.CreateScale(SCALE_TURRET);
            _turretMatrix *= Matrix4.CreateTranslation(_position + new Vector3(0, Y_OFFSET_TURRET, 0) * (1 / 6f) * (float)Math.Sin(_timer*2) + new Vector3(0, Y_OFFSET_TURRET, 0));

            if (CurrentTarget != null)
            { 

                foreach (Projectile projectile in _projectiles)
                {
                    if (projectile.HasReached)
                    {
                        _particleSystemHit.CreateWithTime(e, CurrentTarget.Position + new Vector3(0, 0.7f, 0), Vector3.Zero, 0.8f);
                        _shootSoundGround.SetPosition(_position);
                        _shootSoundGround.Play();
                    }

                }
            }
            _particleSystemHit.Update(e);
        }
    }
}
