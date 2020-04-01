using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TowerDefense.particles.particleemmiter
{
    class ParticleCanonStaticEmmiter : ParticleSystem
    {
        public ParticleCanonStaticEmmiter(ParticleAtlas textureAtlas, float pps, float speed, float gravity, float lifetime) : 
            base(textureAtlas, pps, speed, gravity, lifetime)
        {

        }

        public override void EmitParticle(FrameEventArgs e, Vector3 position, Vector3 velocity)
        {
            float dirx = (float)Random.NextDouble() * (360.0f - (-360.0f)) + (-360.0f);
            Vector3 vel = new Vector3(0, 0, 0);
            vel = Vector3.Multiply(vel, Speed);
            ParticleRenderer.AddParticle(new Particle(new Vector3(position), vel, TextureAtlas, Gravity, Lifetime, dirx, 0.2f));
        }
    }
}
