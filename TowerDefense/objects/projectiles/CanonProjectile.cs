using System.Collections.Generic;
using OpenTK;
using TowerDefense.particles;
using TowerDefense.particles.particleemmiter;

namespace TowerDefense.objects.projectiles
{
    class CanonProjectile : Projectile
    {
        public CanonProjectile(List<Enemy> enemies, Vector3 target, Vector3 start, float speed) : base(enemies, target, start, speed, new Vector3(0.2f, 0.2f, 0.2f))
        {
            _obj = ResourceManager.Objects["PROJECTILE_1"];
            _texture = ResourceManager.Textures["PROJECTILE_1"];
            _textureNormal = ResourceManager.Textures["PROJECTILE_NORMAL_1"];
            _particleSystem = new ParticleCanonEmmiter(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_6"], 1,1),
               10, 1.0f, -0.1f, 1.5f);
        }
    }
}
