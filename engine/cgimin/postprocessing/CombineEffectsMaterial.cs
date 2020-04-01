using System;
using Engine.cgimin.camera;
using Engine.cgimin.object3d;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.material;

namespace Engine.cgimin.postprocessing
{
    class CombineEffectsMaterial : BaseMaterial 
    {
        private int originalTextureLocation;
        private int effectedTextureLocation;
        public CombineEffectsMaterial()
        {
            // Shader-Programm wird aus den externen Files generiert...
            CreateShaderProgram("cgimin/postprocessing/BlurFullscreen_VS.glsl", "cgimin/postprocessing/Combine_Effects_FS.glsl");

            // GL.BindAttribLocation, gibt an welcher Index in unserer Datenstruktur welchem "in" Parameter auf unserem Shader zugeordnet wird
            // folgende Befehle müssen aufgerufen werden...
            GL.BindAttribLocation(Program, 0, "in_position");
            GL.BindAttribLocation(Program, 2, "in_uv");


            GL.GetUniformLocation(Program, "original");
            GL.GetUniformLocation(Program, "effected");
            // ...bevor das Shader-Programm "gelinkt" wird.
            GL.LinkProgram(Program);

        }

        public void Draw(BaseObject3D object3d, int textureID, int texture2ID)
        {
            // Textur wird "gebunden"
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, texture2ID);

            // das Vertex-Array-Objekt unseres Objekts wird benutzt
            GL.BindVertexArray(object3d.Vao);

            // Unser Shader Programm wird benutzt
            GL.UseProgram(Program);

            GL.Uniform1(originalTextureLocation, 0);
            GL.Uniform1(effectedTextureLocation, 1);

            // Das Objekt wird gezeichnet
            GL.DrawElements(PrimitiveType.Triangles, object3d.Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Unbinden des Vertex-Array-Objekt damit andere Operation nicht darauf basieren
            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);

        }


        public override void DrawWithSettings(BaseObject3D object3d, MaterialSettings settings)
        {
            
        }

    }
}
