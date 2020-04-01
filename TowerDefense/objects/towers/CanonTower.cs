using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using TowerDefense.objects.projectiles;
using TowerDefense.particles;
using TowerDefense.particles.particleemmiter;
using Engine.cgimin.sound;

namespace TowerDefense.objects.towers
{
    class CanonTower : Tower
    {
        
        private const float Y_OFFSET_TURRET = 2f;
        private const float Y_OFFSET_BASE = 1.35f;
        private readonly Vector3 YOFFSET = new Vector3(0, 2.0f, 0);
        public static int StartCosts = 80;
        private float SCALE = 0.4f;
        private float SCALE_TURRET = 0.4f;
        private float RADIUS_PROJECTILE = 4f;
        private ParticleSystem _particleSystem;
        private ParticleSystem _particleSystemExplosion;
        private ParticleSystem _particleSystemHit;
        private Sound _shootSound;
        private Sound _shootSoundGround;

        public CanonTower(Vector3 pos) : base(70,5, 2000, StartCosts, pos)
        {
            Description = "Canon Tower";
            AttackDescription = "Multiple";
            SetPosition(pos);
            _particleSystem = new ParticleCanonStaticEmmiter(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_3"], 4,8),
                5, 2f, 0, 1f);

            _particleSystemExplosion = new ParticleCanonExplosion(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_8"], 6,6),
                1, 0.3f, 0, 2f);

            _particleSystemHit = new ParticleCanonHit(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_7"], 6, 6),
                1, 0.3f, 0, 2f);

            _shootSound = new Sound(ResourceManager.Sounds["CANON"]);
            _shootSoundGround = new Sound(ResourceManager.Sounds["CANON_GROUND"]);
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
                
                _turretMatrix *= Matrix4.CreateTranslation(_position + new Vector3(0, Y_OFFSET_TURRET, 0));

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
                Vector3 distance = target.Position - _position;
                distance.Normalize();
                List<Enemy> enemiesdmg = new List<Enemy>();
                foreach (Enemy enemy in enemies)
                {
                    if((target.Position - enemy.Position).Length< RADIUS_PROJECTILE)
                    {
                        enemiesdmg.Add(enemy);
                    }
                }
                Projectile proj = new CanonProjectile(enemiesdmg, target.Position, _position + YOFFSET +  distance, 5f);
                _projectiles.Add(proj);
                _particleSystemExplosion.CreateWithTime(e, _position + YOFFSET + new Vector3(0, 0.1f, 0) + distance * 1.1f, Vector3.Zero, 1f);
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
            
            _textureTurret = ResourceManager.Textures["TOWER_CANON"];
            _textureGround = ResourceManager.Textures["TOWER_BASE_1"];

            if (Level<=2)
            {
                _objTurret = ResourceManager.Objects["TOWER_CANON_TURRET_1"];
                _objBase = ResourceManager.Objects["TOWER_CANON_BASE_1"];
                _objHub = ResourceManager.Objects["TOWER_CANON_HUB_1"];

                
            }
            else if(Level<=4 && Level>2)
            {
                _objTurret = ResourceManager.Objects["TOWER_CANON_TURRET_2"];
                _objBase = ResourceManager.Objects["TOWER_CANON_BASE_2"];
                _objHub = ResourceManager.Objects["TOWER_CANON_HUB_2"];
                
            }
            else if (Level>4)
            {
                _objTurret = ResourceManager.Objects["TOWER_CANON_TURRET_3"];
                _objBase = ResourceManager.Objects["TOWER_CANON_BASE_3"];
                _objHub = ResourceManager.Objects["TOWER_CANON_HUB_3"];
                _textureGround = ResourceManager.Textures["TOWER_BASE_2"];
            }

        }

        public override void Update(FrameEventArgs e)
        {
            
            if (CurrentTarget != null)
            {
                Vector3 distance = CurrentTarget.Position - _position;
                distance.Normalize();
 

                _particleSystem.Create(e, Position + YOFFSET + new Vector3(0, 0.58f, 0) - distance*0.93f, Vector3.Zero);

                foreach (Projectile projectile in _projectiles)
                {
                    if (projectile.HasReached)
                    {
                        _particleSystemHit.CreateWithTime(e, CurrentTarget.Position + new Vector3(0, 1f, 0), Vector3.Zero, 1f);
                    }
                    _shootSoundGround.SetPosition(_position);
                    _shootSoundGround.Play();
                }


            }
            _particleSystemHit.Update(e);
            _particleSystemExplosion.Update(e);
        }
    }
}
