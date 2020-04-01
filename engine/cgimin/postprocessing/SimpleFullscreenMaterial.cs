using System;
using Engine.cgimin.camera;
using Engine.cgimin.object3d;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.material;

namespace Engine.cgimin.postprocessing
{
    public class SimpleFullscreenMaterial : BaseMaterial
    {
        private int fragDataLocation;
        public SimpleFullscreenMaterial()
        {
            // Shader-Programm wird aus den externen Files generiert...
            CreateShaderProgram("cgimin/postprocessing/SimpleFullscreen_VS.glsl", "cgimin/postprocessing/SimpleFullscreen_FS.glsl");

            // GL.BindAttribLocation, gibt an welcher Index in unserer Datenstruktur welchem "in" Parameter auf unserem Shader zugeordnet wird
            // folgende Befehle müssen aufgerufen werden...
            GL.BindAttribLocation(Program, 0, "in_position");
            GL.BindAttribLocation(Program, 1, "in_normal");
            GL.BindAttribLocation(Program, 2, "in_uv");

            // ...bevor das Shader-Programm "gelinkt" wird.
            GL.LinkProgram(Program);

            fragDataLocation = GL.GetUniformLocation(Program, "fragData");


        }

        public void Draw(BaseObject3D object3d, int fragdata, int textureID)
        {
            // Textur wird "gebunden"
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // das Vertex-Array-Objekt unseres Objekts wird benutzt
            GL.BindVertexArray(object3d.Vao);

            // Unser Shader Programm wird benutzt
            GL.UseProgram(Program);

            GL.Uniform1(fragDataLocation, fragdata);
                
            // Das Objekt wird gezeichnet
            GL.DrawElements(PrimitiveType.Triangles, object3d.Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

			// Unbinden des Vertex-Array-Objekt damit andere Operation nicht darauf basieren
			GL.BindVertexArray(0);
        }


        public override void DrawWithSettings(BaseObject3D object3d, MaterialSettings settings)
        {
            
        }


    }
}
