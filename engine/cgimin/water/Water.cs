
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.helpers;
using Engine.cgimin.framebuffer;
using Engine.cgimin.object3d;
using OpenTK;
using Engine.cgimin.camera;
using System;
using Engine.cgimin.texture;
using System.Collections.Generic;
using Engine.cgimin.shadowmapping;

namespace Engine.cgimin.water
{
    public class Water
    {

        public BasicFrameBuffer RefractionFrameBuffer;
        public BasicFrameBuffer ReflectionFrameBuffer;
        public float WaterHeight;
        private int Program;

        private int modelviewProjectionMatrixLocation;
        private int reflectionLocation;
        private int refractionLocation;
        private int dudvLocation;
        private int dudvTexture;
        private int normalLocation;
        private int normalTexture;
        private int timeLocation;
        private int cameraPositionLocation;
        private int modelMatrixLocation;
        private int lightColorLocation;
        private int lightPosLocation;
        private float time;
        private float speed;
        private int indexCount;
        private int waterTileVAO;
        private int shadowMVPLocation;
        private int shadowTextureLocation;
        private int waterVBO;
        private int indexBuffer;
        private int fogStartLocation;
        private int fogEndLocation;
        private int fogColorLocation;

        private void generateWaterTiles()
        {
            List<float> waterData = new List<float>();
            List<int> indices = new List<int>();

            int ind = 0;

            for (int x = 0; x < 65; x++)
            {
                for (int z = 0; z < 65; z++)
                {
                    waterData.Add(x * 4.0f);
                    waterData.Add(0);
                    waterData.Add(z * 4.0f);
                    waterData.Add(x);
                    waterData.Add(z);

                    if (x < 64 && z < 64)
                    {
                        indices.Add(ind);
                        indices.Add(ind + 65);
                        indices.Add(ind + 66);

                        indices.Add(ind);
                        indices.Add(ind + 66);
                        indices.Add(ind + 1);
                    }
                    ind++;
                }
            }

            indexCount = indices.Count;

            GL.GenBuffers(1, out waterVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, waterVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(waterData.Count * sizeof(float)), waterData.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

  
            GL.GenBuffers(1, out indexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(uint) * indices.Count), indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.GenVertexArrays(1, out waterTileVAO);
            GL.BindVertexArray(waterTileVAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, waterVBO);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, 20, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, true, 20, 12);
            GL.BindVertexArray(0);
        }

        public Water(float pspeed)
        {
            generateWaterTiles();


            WaterHeight = 0.0f;

            // Shader-Programm wird aus den externen Files generiert...
            Program = ShaderCompiler.CreateShaderProgram("cgimin/water/Water_VS.glsl","cgimin/water/Water_FS.glsl");

            // GL.BindAttribLocation, gibt an welcher Index in unserer Datenstruktur welchem "in" Parameter auf unserem Shader zugeordnet wird
            // folgende Befehle müssen aufgerufen werden...
            GL.BindAttribLocation(Program, 0, "in_position");
            GL.BindAttribLocation(Program, 1, "in_uv");

            // ...bevor das Shader-Programm "gelinkt" wird.
            GL.LinkProgram(Program);

            // Die Stelle an der im Shader der per "uniform" der Input-Paremeter "modelview_projection_matrix" definiert wird, wird ermittelt.
            modelviewProjectionMatrixLocation = GL.GetUniformLocation(Program, "modelview_projection_matrix");
            reflectionLocation = GL.GetUniformLocation(Program, "reflection_texture");
            refractionLocation = GL.GetUniformLocation(Program, "refraction_texture");
            normalLocation = GL.GetUniformLocation(Program, "normal_texture");
            dudvLocation = GL.GetUniformLocation(Program, "dudv_texture");
            timeLocation = GL.GetUniformLocation(Program, "time");
            cameraPositionLocation = GL.GetUniformLocation(Program, "camera_position");
            modelMatrixLocation = GL.GetUniformLocation(Program, "model");
            lightColorLocation = GL.GetUniformLocation(Program, "light_color");
            lightPosLocation = GL.GetUniformLocation(Program, "lightDir");
            shadowMVPLocation = GL.GetUniformLocation(Program, "DepthBiasMVP");
            shadowTextureLocation = GL.GetUniformLocation(Program, "shadowmap_texture");
            dudvTexture = TextureManager.LoadTexture("assets/textures/dudv.png");
            normalTexture = TextureManager.LoadTexture("assets/textures/waternormal.png");
            fogStartLocation = GL.GetUniformLocation(Program, "fogStart");
            fogEndLocation = GL.GetUniformLocation(Program, "fogEnd");
            fogColorLocation = GL.GetUniformLocation(Program, "fogColor");
            time = 0.0f;
            speed = pspeed;
        }

        public void InitFrameBuffers(int w, int h)
        {
            RefractionFrameBuffer = new BasicFrameBuffer(w, h);
            ReflectionFrameBuffer = new BasicFrameBuffer(w, h);
        }

        public void Draw(float delta)
        {
            time += speed * delta;
  
            GL.Disable(EnableCap.CullFace);
            // Das Vertex-Array-Objekt unseres Objekts wird benutzt
            GL.BindVertexArray(waterTileVAO);

            // Unser Shader Programm wird benutzt
            GL.UseProgram(Program);

            // Farb-Textur wird "gebunden"
            GL.Uniform1(reflectionLocation,0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ReflectionFrameBuffer.basicColorTexture);

            // Normalmap-Textur wird "gebunden"
            GL.Uniform1(refractionLocation, 1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, RefractionFrameBuffer.basicColorTexture);

            // Normalmap-Textur wird "gebunden"
            GL.Uniform1(dudvLocation, 2);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, dudvTexture);

            // Normalmap-Textur wird "gebunden"
            GL.Uniform1(normalLocation, 3);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, normalTexture);

            // Shadowmap-Textur wird "gebunden"
            GL.Uniform1(shadowTextureLocation, 4);
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, ShadowMapping.DepthTexture);


            GL.Uniform3(cameraPositionLocation, new Vector3(Camera.Position.X, Camera.Position.Y, Camera.Position.Z));
            

            GL.Uniform3(lightPosLocation, light.Light.lightDirection);
            GL.Uniform3(lightColorLocation, light.Light.lightSpecular.Xyz);

            GL.Uniform1(fogStartLocation, Camera.FogStart);
            GL.Uniform1(fogEndLocation, Camera.FogEnd);
            GL.Uniform3(fogColorLocation, Camera.FogColor);

            GL.Uniform1(timeLocation, time);
            // Das Objekt wird gezeichnet
            float cmpRadius = (float)Math.Sqrt(128 * 128 + 128 * 128);
            int inViewCount = 0;
            for (int x = -1; x < 1; x++)
            {
                for (int z = -1; z < 1; z++)
                {
                    if (Camera.SphereIsInFrustum(new Vector3(x * 256 + 128, 0, z * 256 + 128), cmpRadius))
                    {
                        inViewCount++;
                        Matrix4 model = Matrix4.CreateTranslation(x * 256, WaterHeight, z * 256);

                        // Die Matrix, welche wir als "modelview_projection_matrix" übergeben, wird zusammengebaut:
                        // Objekt-Transformation * Kamera-Transformation * Perspektivische Projektion der kamera.
                        // Auf dem Shader wird jede Vertex-Position mit dieser Matrix multipliziert. Resultat ist die Position auf dem Screen.
                        Matrix4 modelViewProjection = model * Camera.Transformation * Camera.PerspectiveProjection;

                        Matrix4 depthMVP = model * ShadowMapping.ShadowTransformation * ShadowMapping.DepthBias * ShadowMapping.ShadowProjection;
                        GL.UniformMatrix4(shadowMVPLocation, false, ref depthMVP);

                        // Die ModelViewProjection Matrix wird dem Shader als Parameter übergeben
                        GL.UniformMatrix4(modelviewProjectionMatrixLocation, false, ref modelViewProjection);
                        GL.UniformMatrix4(modelMatrixLocation, false, ref model);

                        GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    }
                }
            }

            // Active Textur wieder auf 0, um andere Materialien nicht durcheinander zu bringen
            GL.ActiveTexture(TextureUnit.Texture0);

            // Unbinden des Vertex-Array-Objekt damit andere Operation nicht darauf basieren
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.CullFace);
        }


        public void UnLoad()
        {

        }
    }
}
