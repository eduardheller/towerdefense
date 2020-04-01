using System;
using System.Collections.Generic;
using System.Linq;
using Engine.cgimin.object3d;
using Engine.cgimin.material.particleatlas;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TowerDefense.particles
{
    public class ParticleRenderer
    {
        private static QuadObject3D _obj;
        private static Dictionary<ParticleAtlas, List<Particle>> _particles;
        private static ParticleMaterial _particleMaterial;

        private static readonly int MAX_PARTICLES = 10000;
    
        private static float[] _texoffsets1;
        private static float[] _texoffsets2;
        private static float[] _centers;
        private static float[] _billboardSize;
        private static float[] _blends;
        private static float[] _angles;

        private static int _texoffset1VBO;
        private static int _texoffset2VBO;
        private static int _centersVBO;
        private static int _billboardSizeVBO;
        private static int _blendsVBO;
        private static int _anglesVBO;
        
        public static void Init()
        {
            _obj = new QuadObject3D();

            _texoffsets1 = new float[MAX_PARTICLES * 2];
            _texoffsets2 = new float[MAX_PARTICLES * 2];
            _centers = new float[MAX_PARTICLES * 3];
            _billboardSize = new float[MAX_PARTICLES * 2];
            _blends = new float[MAX_PARTICLES];
            _angles = new float[MAX_PARTICLES];

            InitializeInstanceBuffer(ref _texoffset1VBO, MAX_PARTICLES * 2 * sizeof(float));
            InitializeInstanceBuffer(ref _texoffset2VBO, MAX_PARTICLES * 2 * sizeof(float));
            InitializeInstanceBuffer(ref _centersVBO, MAX_PARTICLES * 3 * sizeof(float));
            InitializeInstanceBuffer(ref _billboardSizeVBO, MAX_PARTICLES * 2 * sizeof(float));
            InitializeInstanceBuffer(ref _blendsVBO, MAX_PARTICLES * sizeof(float));
            InitializeInstanceBuffer(ref _anglesVBO, MAX_PARTICLES * sizeof(float));

            GL.BindVertexArray(_obj.Vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _obj.IndexBuffer);

            CreateAttribInstanced(ref _texoffset1VBO, 5, 2, Vector2.SizeInBytes);
            CreateAttribInstanced(ref _texoffset2VBO, 6, 2, Vector2.SizeInBytes);
            CreateAttribInstanced(ref _centersVBO, 7, 3, Vector3.SizeInBytes);
            CreateAttribInstanced(ref _billboardSizeVBO, 8, 2, Vector2.SizeInBytes);
            CreateAttribInstanced(ref _blendsVBO, 9, 1, sizeof(float));
            CreateAttribInstanced(ref _anglesVBO, 10, 1, sizeof(float));

            GL.BindVertexArray(0);

            _particles = new Dictionary<ParticleAtlas, List<Particle>>();
            _particleMaterial = new ParticleMaterial();
        }

        public static void AddParticle(Particle particle)
        {
            if (!_particles.ContainsKey(particle.Texture))
            {
                _particles.Add(particle.Texture, new List<Particle>());
            }
            _particles[particle.Texture].Add(particle);
           
        }

        public static void Update(FrameEventArgs e)
        {

        }

        public static void Render(FrameEventArgs e)
        {
            GL.DepthMask(false);
            foreach (var particleList in _particles)
            {
                
                int totalSize = 0;
                int i = 0;
                int particlesSize = particleList.Value.Count;
                totalSize += particlesSize;

                foreach (Particle particle in particleList.Value.ToList())
                {

                    if (!particle.Update(e, i, _texoffsets1, _texoffsets2, _centers, _billboardSize, _blends, _angles))
                    {
                        particleList.Value.Remove(particle);
                    }
                    if (++i > MAX_PARTICLES) break;
                    
                }
               
                RefreshInstanceBuffer(ref _texoffset1VBO, _texoffsets1, totalSize * sizeof(float) * 2);
                RefreshInstanceBuffer(ref _texoffset2VBO, _texoffsets2, totalSize * sizeof(float) * 2);
                RefreshInstanceBuffer(ref _centersVBO, _centers, totalSize * sizeof(float) * 3);
                RefreshInstanceBuffer(ref _billboardSizeVBO, _billboardSize, totalSize * sizeof(float) * 2);
                RefreshInstanceBuffer(ref _blendsVBO, _blends, totalSize * sizeof(float));
                RefreshInstanceBuffer(ref _anglesVBO, _angles, totalSize * sizeof(float));

                _particleMaterial.Draw(_obj, particleList.Value.Count, 
                    particleList.Key.RowsCount, particleList.Key.ColumnCount,particleList.Key.Texture,particleList.Key.Src,particleList.Key.Dest);
            }
            GL.DepthMask(true);


            
        }

        private static void CreateAttribInstanced(ref int vbo, int index, int size, int stride)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, stride, 0);
            GL.VertexAttribDivisor(index, 1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private static void RefreshInstanceBuffer(ref int vbo, float[] data, int size)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, size, data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private static void InitializeInstanceBuffer(ref int vbo, int size, BufferUsageHint usage = BufferUsageHint.DynamicDraw)
        {
            GL.GenBuffers(1, out vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, usage);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
