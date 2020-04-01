using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TowerDefense.particles.particleemmiter
{
    class ParticleArrowEmitter : ParticleSystem
    {
        public ParticleArrowEmitter(ParticleAtlas textureAtlas, float pps, float speed, float gravity, float lifetime) : 
            base(textureAtlas, pps, speed, gravity, lifetime)
        {

        }

        public override void EmitParticle(FrameEventArgs e, Vector3 position, Vector3 velocity)
        {
            float dirx = (float)Random.NextDouble() * (1.0f - (-1.0f)) + (-1.0f);
            float diry = (float)Random.NextDouble() * (1.0f - (-1.0f)) + (-1.0f);
            Vector3 vel = new Vector3(dirx, 1, diry);
            vel = Vector3.Multiply(vel, Speed);
            ParticleRenderer.AddParticle(new Particle(new Vector3(position), vel, TextureAtlas, Gravity, Lifetime, 0, 0.5f));
        }
    }
}
