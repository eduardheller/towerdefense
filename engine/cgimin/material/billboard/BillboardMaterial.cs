using System;
using Engine.cgimin.camera;
using Engine.cgimin.object3d;
using Engine.cgimin.light;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.cgimin.material.billboard
{
    public class BillboardMaterial : BaseMaterial
    {
        private int viewMatrixLocation;
        private int centerLocation;
        private int sizeLocation;
        private int projectionMatrixLocation;
        private int alphaLocation;

        public BillboardMaterial()
        {
            // Shader-Programm wird aus den externen Files generiert...
            CreateShaderProgram(MATERIAL_DIRECTORY + "billboard/Billboard_VS.glsl",
                                MATERIAL_DIRECTORY + "billboard/Billboard_FS.glsl");

            // GL.BindAttribLocation, gibt an welcher Index in unserer Datenstruktur welchem "in" Parameter auf unserem Shader zugeordnet wird
            // folgende Befehle müssen aufgerufen werden...
            GL.BindAttribLocation(Program, 0, "in_position");
            GL.BindAttribLocation(Program, 2, "in_uv");
            
            // ...bevor das Shader-Programm "gelinkt" wird.
            GL.LinkProgram(Program);

            // Die Stelle an der im Shader der per "uniform" der Input-Paremeter "projection_matrix" definiert wird, wird ermittelt.
            projectionMatrixLocation = GL.GetUniformLocation(Program, "projection_matrix");

            // Die Stelle für die den "model_view_matrix" - Parameter wird ermittelt.
            viewMatrixLocation = GL.GetUniformLocation(Program, "view_matrix");

            centerLocation = GL.GetUniformLocation(Program, "center");

            alphaLocation = GL.GetUniformLocation(Program, "alpha");

            sizeLocation = GL.GetUniformLocation(Program, "billboard_size");

        }

        public void Draw(BaseObject3D object3d, Vector3 center, Vector2 size, float alpha, int textureID, BlendingFactorSrc sourceBlendFunc = BlendingFactorSrc.SrcAlpha, BlendingFactorDest destBlendFunc = BlendingFactorDest.OneMinusSrcAlpha)
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

            GL.Uniform3(centerLocation, ref center);

            GL.Uniform2(sizeLocation, ref size);

            GL.Uniform1(alphaLocation, alpha);
        
            // Das Objekt wird gezeichnet
            GL.DrawElements(PrimitiveType.Triangles, object3d.Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
    
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
