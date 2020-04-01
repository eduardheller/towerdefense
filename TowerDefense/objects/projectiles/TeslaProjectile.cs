using OpenTK;
using System.Collections.Generic;
using TowerDefense.particles;
using TowerDefense.particles.particleemmiter;

namespace TowerDefense.objects.projectiles
{
    class TeslaProjectile : Projectile
    {
        public TeslaProjectile(List<Enemy> enemies, Vector3 target, Vector3 start, float speed) : base(enemies, target, start, speed)
        {
            

            _particleSystem = new ParticleTeslaEmmiter(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_9"], 4, 8),
                100, 0.2f, 0, 0.5f);
        }
    }
}
