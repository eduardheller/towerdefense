﻿using System;
using Engine.cgimin.camera;
using Engine.cgimin.object3d;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.cgimin.material.alphatexture
{
    public class AlphaTextureMaterial : BaseMaterial
    {

        private int modelviewProjectionMatrixLocation;
        private int alphaValueLocation;

        public AlphaTextureMaterial()
        {
            // Shader-Programm wird aus den externen Files generiert...
            CreateShaderProgram(MATERIAL_DIRECTORY + "alphatexture/AlphaTexture_VS.glsl",
                                MATERIAL_DIRECTORY + "alphatexture/AlphaTexture_FS.glsl");

            // GL.BindAttribLocation, gibt an welcher Index in unserer Datenstruktur welchem "in" Parameter auf unserem Shader zugeordnet wird
            // folgende Befehle müssen aufgerufen werden...
            GL.BindAttribLocation(Program, 0, "in_position");
            GL.BindAttribLocation(Program, 1, "in_normal");
            GL.BindAttribLocation(Program, 2, "in_uv");
            
            // ...bevor das Shader-Programm "gelinkt" wird.
            GL.LinkProgram(Program);

            // Die Stelle an der im Shader der per "uniform" der Input-Paremeter "modelview_projection_matrix" definiert wird, wird ermittelt.
            modelviewProjectionMatrixLocation = GL.GetUniformLocation(Program, "modelview_projection_matrix");

            alphaValueLocation = GL.GetUniformLocation(Program, "alpha");
        }

        public void Draw(BaseObject3D object3d, int textureID, float alpha, BlendingFactorSrc sourceBlendFunc = BlendingFactorSrc.SrcAlpha, BlendingFactorDest destBlendFunc = BlendingFactorDest.OneMinusSrcAlpha)
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
            Matrix4 modelviewProjection = object3d.Transformation * Camera.Transformation * Camera.PerspectiveProjection;

            // Die Matrix wird dem Shader als Parameter übergeben
            GL.UniformMatrix4(modelviewProjectionMatrixLocation, false, ref modelviewProjection);

            // Den Alpha-Wert übergeben
            GL.Uniform1(alphaValueLocation, alpha);

            // Das Objekt wird gezeichnet
            GL.DrawElements(PrimitiveType.Triangles, object3d.Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

			// Unbinden des Vertex-Array-Objekt damit andere Operation nicht darauf basieren
			GL.BindVertexArray(0);

            // "Blending" wieder ausschalten
            GL.Disable(EnableCap.Blend);
        }


        public override void DrawWithSettings(BaseObject3D object3d, MaterialSettings settings)
        {
            Draw(object3d, settings.colorTexture, settings.alpha, settings.blendFactorSource, settings.blendFactorDest);
        }


    }
}
