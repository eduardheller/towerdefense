using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using Engine.cgimin.helpers;
using Engine.cgimin.camera;

namespace Engine.cgimin.particles
{
    /// <summary>
    /// Diese Klasse ist unfertig und funktioniert (noch) nicht
    /// </summary>

    public class Particles
    {

        private int MAX_PARTICLES = 100;

        private int particles_position_buffer;

        private int particlesCount;
        private int billboard_vertex_buffer;

        private int program;

        private int projectionMatrixLocation;
        private int modelViewMatrixLocation;

        private int squareVerticesLocation;
        private int xyzsLocation;

        private Matrix4 transformation;

        public Particles()
        {

            transformation = Matrix4.Identity;

            program = ShaderCompiler.CreateShaderProgram("cgimin/particles/Particles_VS.glsl", "cgimin/particles/Particles_FS.glsl");

            projectionMatrixLocation = GL.GetUniformLocation(program, "projection_matrix");
            modelViewMatrixLocation = GL.GetUniformLocation(program, "model_view_matrix");

            squareVerticesLocation = GL.GetAttribLocation(program, "squareVertices");
            xyzsLocation = GL.GetAttribLocation(program, "xyzs");

            GL.LinkProgram(program);


            float[] g_vertex_buffer_data = {
             -0.5f, -0.5f, 0.0f,
              0.5f, -0.5f, 0.0f,
             -0.5f, 0.5f, 0.0f,
              0.5f, 0.5f, 0.0f,
            };

            GL.GenBuffers(1, out billboard_vertex_buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, billboard_vertex_buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * 3 * 4), g_vertex_buffer_data, BufferUsageHint.StaticDraw);
            
            GL.GenBuffers(1, out particles_position_buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, particles_position_buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(MAX_PARTICLES * 4 * sizeof(float)),  (IntPtr)0, BufferUsageHint.StreamDraw);

        }


        public void draw(int textureID)
        {
            particlesCount = 100;
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            float[] g_particule_position_size_data = new float[100 * 4];

            for (int i = 0; i < particlesCount; i++)
            {
                g_particule_position_size_data[i * 4] = i;
                g_particule_position_size_data[i * 4 + 1] = 0;
                g_particule_position_size_data[i * 4 + 2] = 0;
                g_particule_position_size_data[i * 4 + 3] = 2.0f;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, particles_position_buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(particlesCount * 4 * sizeof(float)), g_particule_position_size_data, BufferUsageHint.StreamDraw);
            // Hier mit SubBuffer arbieten

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


            GL.UseProgram(program);

            GL.EnableVertexAttribArray(squareVerticesLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, billboard_vertex_buffer);
            GL.VertexAttribPointer(squareVerticesLocation, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(xyzsLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, particles_position_buffer);
            GL.VertexAttribPointer(xyzsLocation, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.VertexBindingDivisor(squareVerticesLocation, 0); 
            GL.VertexBindingDivisor(xyzsLocation, 1);                                            

            Matrix4 projection = Camera.PerspectiveProjection;
            GL.UniformMatrix4(projectionMatrixLocation, false, ref projection);

            // Die Model-View Matrix wird dem Shader übergeben
            Matrix4 modelView = transformation * Camera.Transformation;
            GL.UniformMatrix4(modelViewMatrixLocation, false, ref modelView);

            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, particlesCount);

           

        }




    }
}
