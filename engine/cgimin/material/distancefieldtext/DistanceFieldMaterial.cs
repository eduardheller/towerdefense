using Engine.cgimin.object3d;
using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Engine.cgimin.camera;

namespace Engine.cgimin.material.distancefieldtext
{
    public class DistanceFieldMaterial : BaseMaterial
    {
        private int modelviewProjectionMatrixLocation;
        private int alphaValueLocation;
        private int edgeValueLocation;
        private int widthValueLocation;
        private int colorLocation;

        public DistanceFieldMaterial()
        {
            // Shader-Programm wird aus den externen Files generiert...
            CreateShaderProgram(MATERIAL_DIRECTORY + "distancefieldtext/DistanceField_VS.glsl",
                                MATERIAL_DIRECTORY + "distancefieldtext/DistanceField_FS.glsl");

            // GL.BindAttribLocation, gibt an welcher Index in unserer Datenstruktur welchem "in" Parameter auf unserem Shader zugeordnet wird
            // folgende Befehle müssen aufgerufen werden...
            GL.BindAttribLocation(Program, 0, "in_position");
            GL.BindAttribLocation(Program, 2, "in_uv");

            // ...bevor das Shader-Programm "gelinkt" wird.
            GL.LinkProgram(Program);

            // Die Stelle an der im Shader der per "uniform" der Input-Paremeter "modelview_projection_matrix" definiert wird, wird ermittelt.
            modelviewProjectionMatrixLocation = GL.GetUniformLocation(Program, "modelview_projection_matrix");

            alphaValueLocation = GL.GetUniformLocation(Program, "alpha");
            edgeValueLocation = GL.GetUniformLocation(Program, "edge");
            widthValueLocation = GL.GetUniformLocation(Program, "width");
            colorLocation = GL.GetUniformLocation(Program, "color");
        }
       

        public void Draw(BaseObject3D object3d, int textureID, Vector3 color, float alpha,float edge, float width, BlendingFactorSrc sourceBlendFunc = BlendingFactorSrc.SrcAlpha, BlendingFactorDest destBlendFunc = BlendingFactorDest.OneMinusSrcAlpha)
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

            // Die Matrix, welche wir als "modelview_projection_matrix" übergeben, wird zusammengebaut:
            // Objekt-Transformation * Kamera-Transformation * Perspektivische Projektion der kamera.
            // Auf dem Shader wird jede Vertex-Position mit dieser Matrix multipliziert. Resultat ist die Position auf dem Screen.
            Matrix4 modelviewProjection = object3d.Transformation;

            // Die Matrix wird dem Shader als Parameter übergeben
            GL.UniformMatrix4(modelviewProjectionMatrixLocation, false, ref modelviewProjection);

            // Den Alpha-Wert übergeben
            GL.Uniform1(alphaValueLocation, alpha);

            GL.Uniform1(edgeValueLocation, edge);
            GL.Uniform1(widthValueLocation, width);
            GL.Uniform3(colorLocation, color);
            
            // Das Objekt wird gezeichnet
            GL.DrawElements(PrimitiveType.Triangles, object3d.Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Unbinden des Vertex-Array-Objekt damit andere Operation nicht darauf basieren
            GL.BindVertexArray(0);

            // "Blending" wieder ausschalten
            GL.Disable(EnableCap.Blend);
        }


        public override void DrawWithSettings(BaseObject3D object3d, MaterialSettings settings)
        {
           
        }
    }
}
