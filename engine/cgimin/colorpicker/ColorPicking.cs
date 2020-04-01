using Engine.cgimin.camera;
using Engine.cgimin.object3d;
using Engine.cgimin.material;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Engine.cgimin.colorpicker
{
    public class ColorPicking : BaseMaterial
    {
        private int pickingColorLocation;
        private int modelviewProjectionMatrixLocation;

        public ColorPicking()
        {
            // Shader-Programm wird aus den externen Files generiert...
            CreateShaderProgram("cgimin/colorpicker/ColorPicking_VS.glsl",
                                "cgimin/colorpicker/ColorPicking_FS.glsl");

            // GL.BindAttribLocation, gibt an welcher Index in unserer Datenstruktur welchem "in" Parameter auf unserem Shader zugeordnet wird
            // folgende Befehle müssen aufgerufen werden...
            GL.BindAttribLocation(Program, 0, "in_position");

            // ...bevor das Shader-Programm "gelinkt" wird.
            GL.LinkProgram(Program);

            // Die Stelle an der im Shader der per "uniform" der Input-Paremeter "modelview_projection_matrix" definiert wird, wird ermittelt.
            modelviewProjectionMatrixLocation = GL.GetUniformLocation(Program, "modelview_projection_matrix");

            // Die Stelle für die den "model_matrix" - Parameter wird ermittelt.
            pickingColorLocation = GL.GetUniformLocation(Program, "pickingColor");
            
        }

        public void Draw(BaseObject3D object3d, Matrix4 transformation, int id)
        {

            int r = (id & 0x000000FF) >> 0;
            int g = (id & 0x0000FF00) >> 8;
            int b = (id & 0x00FF0000) >> 16;

            // das Vertex-Array-Objekt unseres Objekts wird benutzt
            GL.BindVertexArray(object3d.Vao);

            // Unser Shader Programm wird benutzt
            GL.UseProgram(Program);

            // Die Matrix, welche wir als "modelview_projection_matrix" übergeben, wird zusammengebaut:
            // Objekt-Transformation * Kamera-Transformation * Perspektivische Projektion der kamera.
            // Auf dem Shader wird jede Vertex-Position mit dieser Matrix multipliziert. Resultat ist die Position auf dem Screen.
            Matrix4 modelViewProjection = transformation * Camera.Transformation * Camera.PerspectiveProjection;

            // Die ModelViewProjection Matrix wird dem Shader als Parameter übergeben
            GL.UniformMatrix4(modelviewProjectionMatrixLocation, false, ref modelViewProjection);

            GL.Uniform4(pickingColorLocation, r / 255.0f, g / 255.0f, b / 255.0f, 1.0f);

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
