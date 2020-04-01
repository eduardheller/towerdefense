using System.Collections.Generic;
using OpenTK;
using TowerDefense.particles;
using TowerDefense.particles.particleemmiter;

namespace TowerDefense.objects.projectiles
{
    class MageProjectile : Projectile
    {
        private const float FREEZE = 2.0f;
        public MageProjectile(List<Enemy> enemies, Vector3 target, Vector3 start, float speed) : base(enemies, target, start, speed)
        {

            _particleSystem = new ParticleMageEmitter(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_10"], 1, 1),
                155, 1.0f, 0, 0.2f);
        }

        public override void Freeze(Enemy enemy)
        {
            base.Freeze(enemy);
            enemy.Freeze(FREEZE);
        }
    }
}
