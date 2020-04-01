using System;
using Engine.cgimin.camera;
using Engine.cgimin.object3d;
using Engine.cgimin.light;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.cgimin.material.particleatlas
{

    public class ParticleMaterial : BaseMaterial
    {
        private int viewMatrixLocation;
        private int projectionMatrixLocation;
        private int rowsLocation;
        private int columnsLocation;

        public ParticleMaterial()
        {



            // Shader-Programm wird aus den externen Files generiert...
            CreateShaderProgram(MATERIAL_DIRECTORY + "particleatlas/ParticleAtlas_VS.glsl",
                                MATERIAL_DIRECTORY + "particleatlas/ParticleAtlas_FS.glsl");

      
            // GL.BindAttribLocation, gibt an welcher Index in unserer Datenstruktur welchem "in" Parameter auf unserem Shader zugeordnet wird
            // folgende Befehle müssen aufgerufen werden...
            GL.BindAttribLocation(Program, 0, "in_position");
            GL.BindAttribLocation(Program, 2, "in_uv");
            GL.BindAttribLocation(Program, 5, "in_texoffset1");
            GL.BindAttribLocation(Program, 6, "in_texoffset2");
            GL.BindAttribLocation(Program, 7, "in_center");
            GL.BindAttribLocation(Program, 8, "in_billboard_size");
            GL.BindAttribLocation(Program, 9, "in_blend");
            GL.BindAttribLocation(Program, 10, "in_angle");

            // ...bevor das Shader-Programm "gelinkt" wird.
            GL.LinkProgram(Program);

            // Die Stelle an der im Shader der per "uniform" der Input-Paremeter "projection_matrix" definiert wird, wird ermittelt.
            projectionMatrixLocation = GL.GetUniformLocation(Program, "projection_matrix");

            // Die Stelle für die den "model_view_matrix" - Parameter wird ermittelt.
            viewMatrixLocation = GL.GetUniformLocation(Program, "view_matrix");

            rowsLocation = GL.GetUniformLocation(Program, "rows");
            columnsLocation = GL.GetUniformLocation(Program, "columns");

        }

        public void Draw(BaseObject3D object3d, int count, int rows, int columns, int textureID, BlendingFactorSrc sourceBlendFunc = BlendingFactorSrc.One, BlendingFactorDest destBlendFunc = BlendingFactorDest.One)
        {
            // "Blending" einschalten
            GL.Enable(EnableCap.Blend);

            // Blend Func setzen. Je nach Parameter unterschiedliche Blend-Effekte..
            GL.BlendFunc(sourceBlendFunc, destBlendFunc);

            // Textur wird "gebunden"
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // das Vertex-Array-Objekt unseres Objekts wird benutzt
            GL.BindVertexArray(object3d.Vao);
           
            // Unser Shader Programm wird benutzt
            GL.UseProgram(Program);

            // Die ModelViewProjection Matrix wird dem Shader als Parameter übergeben
            Matrix4 projection = Camera.PerspectiveProjection;
            GL.UniformMatrix4(projectionMatrixLocation, false, ref projection);

            // Die Model-View Matrix wird dem Shader übergeben
            Matrix4 view = Camera.Transformation;
            GL.UniformMatrix4(viewMatrixLocation, false, ref view);

            GL.Uniform1(rowsLocation, rows);
            GL.Uniform1(columnsLocation, columns);
            // Das Objekt wird gezeichnet
            GL.DrawElementsInstanced(PrimitiveType.Triangles,object3d.Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero, count);

            // Unbinden des Vertex-Array-Objekt damit andere Operation nicht darauf basieren
            GL.BindVertexArray(0);

            GL.Disable(EnableCap.Blend);
        }

        public override void DrawWithSettings(BaseObject3D object3d, MaterialSettings settings)
        {
            //Draw(object3d, settings.colorTexture, settings.blendFactorSource, settings.blendFactorDest);
        }

    }

}
