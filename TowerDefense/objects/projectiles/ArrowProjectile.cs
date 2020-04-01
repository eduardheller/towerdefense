using System.Collections.Generic;
using OpenTK;
using TowerDefense.particles;
using TowerDefense.particles.particleemmiter;

namespace TowerDefense.objects.projectiles
{
    class ArrowProjectile : Projectile
    {
        public ArrowProjectile(List<Enemy> enemies, Vector3 target, Vector3 start, float speed, Vector3 scale, Quaternion quaternion) : base(enemies, target, start, speed, scale, quaternion)
        {
            _obj = ResourceManager.Objects["PROJECTILE_2"];
            _texture = ResourceManager.Textures["PROJECTILE_3"];
            _textureNormal = ResourceManager.Textures["PROJECTILE_NORMAL_1"];
            _particleSystem = new ParticleArrowEmitter(
                new ParticleAtlas(ResourceManager.Textures["PARTICLE_ATLAS_1"], 8,8),
                0, 1.0f, 1, 0.2f);
        }
    }
}
