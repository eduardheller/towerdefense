using System;
using Engine.cgimin.camera;
using Engine.cgimin.object3d;
using Engine.cgimin.light;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.shadowmappingcascaded;

namespace Engine.cgimin.material.normalmappingfogshadowcascaded
{
    public class NormalMappingMaterialFogShadowCascaded : BaseMaterial
    {
        private int modelMatrixLocation;
        private int modelViewMatrixLocation;
        private int modelviewProjectionMatrixLocation;
        private int DepthBiasMVPLocation1;
        private int DepthBiasMVPLocation2;
        private int DepthBiasMVPLocation3;

        private int lightDirectionLocation;
        private int lightAmbientLocation;
        private int lightDiffuseLocation;
        private int lightSpecularLocation;
        private int cameraPositionLocation;
        private int materialShininessLocation;

		private int colorTextureLocation;
        private int normalTextureLocation;
        private int shadowTextureLocation1;
        private int shadowTextureLocation2;
        private int shadowTextureLocation3;

        private int dist1Location;
        private int dist2Location;
        private int dist3Location;

        private int fogStartLocation;
		private int fogEndLocation;
		private int fogColorLocation;

        public NormalMappingMaterialFogShadowCascaded()
        {
            // Shader-Programm wird aus den externen Files generiert...
            CreateShaderProgram(MATERIAL_DIRECTORY + "normalmappingfogshadowcascaded/NormalMappingFogShadowCascaded_VS.glsl",
                                MATERIAL_DIRECTORY + "normalmappingfogshadowcascaded/NormalMappingFogShadowCascaded_FS.glsl");

            // GL.BindAttribLocation, gibt an welcher Index in unserer Datenstruktur welchem "in" Parameter auf unserem Shader zugeordnet wird
            // folgende Befehle müssen aufgerufen werden...
            GL.BindAttribLocation(Program, 0, "in_position");
            GL.BindAttribLocation(Program, 1, "in_normal");
            GL.BindAttribLocation(Program, 2, "in_uv");
            GL.BindAttribLocation(Program, 3, "in_tangent");
            GL.BindAttribLocation(Program, 4, "in_bitangent");

            // ...bevor das Shader-Programm "gelinkt" wird.
            GL.LinkProgram(Program);

            // Die Stelle an der im Shader der per "uniform" der Input-Paremeter "modelview_projection_matrix" definiert wird, wird ermittelt.
            modelviewProjectionMatrixLocation = GL.GetUniformLocation(Program, "modelview_projection_matrix");

            // Die Stelle für die den "model_matrix" - Parameter wird ermittelt.
            modelMatrixLocation = GL.GetUniformLocation(Program, "model_matrix");

            modelViewMatrixLocation = GL.GetUniformLocation(Program, "model_view_matrix");

            // Die Stelle fur den "specular_shininess" - Parameter
            materialShininessLocation = GL.GetUniformLocation(Program, "specular_shininess");

            DepthBiasMVPLocation1 = GL.GetUniformLocation(Program, "DepthBiasMVP1");
            DepthBiasMVPLocation2 = GL.GetUniformLocation(Program, "DepthBiasMVP2");
            DepthBiasMVPLocation3 = GL.GetUniformLocation(Program, "DepthBiasMVP3");

            // Die Stellen im Fragemant-Shader für Licht-parameter ermitteln.
            lightDirectionLocation = GL.GetUniformLocation(Program, "light_direction");
            lightAmbientLocation = GL.GetUniformLocation(Program, "light_ambient_color");
            lightDiffuseLocation = GL.GetUniformLocation(Program, "light_diffuse_color");
            lightSpecularLocation = GL.GetUniformLocation(Program, "light_specular_color");
            cameraPositionLocation = GL.GetUniformLocation(Program, "camera_position");
            colorTextureLocation = GL.GetUniformLocation(Program, "color_texture"); 
            normalTextureLocation = GL.GetUniformLocation(Program, "normalmap_texture");

            shadowTextureLocation1 = GL.GetUniformLocation(Program, "shadowmap_texture1");
            shadowTextureLocation2 = GL.GetUniformLocation(Program, "shadowmap_texture2");
            shadowTextureLocation3 = GL.GetUniformLocation(Program, "shadowmap_texture3");

            dist1Location = GL.GetUniformLocation(Program, "dist1");
            dist2Location = GL.GetUniformLocation(Program, "dist2");
            dist3Location = GL.GetUniformLocation(Program, "dist3");

            fogStartLocation = GL.GetUniformLocation (Program, "fogStart");
			fogEndLocation = GL.GetUniformLocation (Program, "fogEnd");
			fogColorLocation = GL.GetUniformLocation (Program, "fogColor");
        }

        public void Draw(BaseObject3D object3d, int textureID, int normalTextureID, float shininess)
        {
          
            // Das Vertex-Array-Objekt unseres Objekts wird benutzt
            GL.BindVertexArray(object3d.Vao);

            // Unser Shader Programm wird benutzt
            GL.UseProgram(Program);

            // Farb-Textur wird "gebunden"
            GL.Uniform1(colorTextureLocation, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // Normalmap-Textur wird "gebunden"
            GL.Uniform1(normalTextureLocation, 1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, normalTextureID);

            // Shadowmap-Textur wird "gebunden"
            GL.Uniform1(shadowTextureLocation1, 2);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, ShadowMappingCascaded.cascades[0].depthTexture);

            GL.Uniform1(shadowTextureLocation2, 3);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, ShadowMappingCascaded.cascades[1].depthTexture);

            GL.Uniform1(shadowTextureLocation3, 4);
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, ShadowMappingCascaded.cascades[2].depthTexture);

            GL.Uniform1(dist1Location, ShadowMappingCascaded.cascades[0].borderDistance);
            GL.Uniform1(dist2Location, ShadowMappingCascaded.cascades[1].borderDistance);
            GL.Uniform1(dist3Location, ShadowMappingCascaded.cascades[2].borderDistance);

            // Die Matrix, welche wir als "modelview_projection_matrix" übergeben, wird zusammengebaut:
            // Objekt-Transformation * Kamera-Transformation * Perspektivische Projektion der kamera.
            // Auf dem Shader wird jede Vertex-Position mit dieser Matrix multipliziert. Resultat ist die Position auf dem Screen.
            Matrix4 modelViewProjection = object3d.Transformation * Camera.Transformation * Camera.PerspectiveProjection;

            // Die ModelViewProjection Matrix wird dem Shader als Parameter übergeben
            GL.UniformMatrix4(modelviewProjectionMatrixLocation, false, ref modelViewProjection);


            Matrix4 modelView = object3d.Transformation * Camera.Transformation;
            GL.UniformMatrix4(modelViewMatrixLocation, false, ref modelView);

            // Die Model-Matrix wird dem Shader übergeben, zur transformation der Normalen
            // und der Berechnung des Winkels Betrachter / Objektpunkt 
            Matrix4 model = object3d.Transformation;
            GL.UniformMatrix4(modelMatrixLocation, false, ref model);

            // Die Projektion der Tiefen-Textur für den Schatten
            // "depthBias" sorgt dafür, dass der 3D-Koordinatenraum (-1 -> 1) umgerechnet wird auf die Koordinaten der Textur (0 -> 1) 
            Matrix4 depthMVP = object3d.Transformation * ShadowMappingCascaded.cascades[0].shadowTransformation * ShadowMappingCascaded.cascades[0].depthBias * ShadowMappingCascaded.cascades[0].shadowProjection;
            GL.UniformMatrix4(DepthBiasMVPLocation1, false, ref depthMVP);

            Matrix4 depthMVP2 = object3d.Transformation * ShadowMappingCascaded.cascades[1].shadowTransformation * ShadowMappingCascaded.cascades[1].depthBias * ShadowMappingCascaded.cascades[1].shadowProjection;
            GL.UniformMatrix4(DepthBiasMVPLocation2, false, ref depthMVP2);

            Matrix4 depthMVP3 = object3d.Transformation * ShadowMappingCascaded.cascades[2].shadowTransformation * ShadowMappingCascaded.cascades[2].depthBias * ShadowMappingCascaded.cascades[2].shadowProjection;
            GL.UniformMatrix4(DepthBiasMVPLocation3, false, ref depthMVP3);

            // Die Licht Parameter werden übergeben
            GL.Uniform3(lightDirectionLocation, Light.lightDirection);
            GL.Uniform4(lightAmbientLocation, Light.lightAmbient);
            GL.Uniform4(lightDiffuseLocation, Light.lightDiffuse);
            GL.Uniform4(lightSpecularLocation, Light.lightSpecular);

            // Shininess
            GL.Uniform1(materialShininessLocation, shininess);

			// Fog Values
			GL.Uniform1 (fogStartLocation, Camera.FogStart);
			GL.Uniform1 (fogEndLocation, Camera.FogEnd);
			GL.Uniform3 (fogColorLocation, Camera.FogColor);

            // Positions Parameter
            GL.Uniform4(cameraPositionLocation, new Vector4(Camera.Position.X, Camera.Position.Y, Camera.Position.Z, 1));

            // Das Objekt wird gezeichnet
            GL.DrawElements(PrimitiveType.Triangles, object3d.Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Active Textur wieder auf 0, um andere Materialien nicht durcheinander zu bringen
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Unbinden des Vertex-Array-Objekt damit andere Operation nicht darauf basieren
            GL.BindVertexArray(0);
        }


        public override void DrawWithSettings(BaseObject3D object3d, MaterialSettings settings)
        {
            Draw(object3d, settings.colorTexture, settings.normalTexture, settings.shininess);
        }


    }
}
