using System;
using System.Collections.Generic;
using Engine.cgimin.camera;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Engine.cgimin.helpers;
using Engine.cgimin.light;
using Engine.cgimin.texture;
using System.Drawing;
using System.Drawing.Imaging;

namespace Engine.cgimin.terrain
{
    public class Terrain
    {

        private struct TerrainTile {
            public int Vao;
            public int IndexCount;

            public Vector3 midPoint;
            public float radius;
        }

        private List<TerrainTile> tiles;

        private int program;

        private int modelviewProjectionMatrixLocation;
        private int camSubPositionLocation;

        private int terrainXZPosLocation;
        private int terrainSizeLocation;
        private int terrainHeightLocation;

        private int heightMapLocation;
        private int colorTextureLocation;
        private int normalTextureLocation;
        private int textureScaleLocation;

        private int materialShininessLocation;
        private int lightDirectionLocation;
        private int lightAmbientLocation;
        private int lightDiffuseLocation;
        private int lightSpecularLocation;

		private int fogStartLocation;
		private int fogEndLocation;
        private int fogColorLocation;

        // height texture info
        private int heightMapTextureID;
        private int terrainTextureSize;
        private int startHeightLocation;


        private int[,] heightInfo;
        private float currentHeight;

        public Terrain(String heighMapPath, float heightValue)
        {
            currentHeight = heightValue;

            // Die Heightmap als Bitmap laden
            Bitmap heightBitmap = new Bitmap(heighMapPath);

            // Info zur Terrain-Größe ziehen
            terrainTextureSize = heightBitmap.Width;

            // BitmapDatan generieren
            BitmapData heightmapData = heightBitmap.LockBits(new Rectangle(0, 0, heightBitmap.Width, heightBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // heightmap-Datan in einen byte-Array kopieren
            IntPtr ptr = heightmapData.Scan0;
            int bytes = Math.Abs(heightmapData.Stride) * heightBitmap.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            heightBitmap.UnlockBits(heightmapData);

            // die höhen in die finale Textur
            heightInfo = new int[terrainTextureSize, terrainTextureSize];
            int index = 1;
            for (int y = 0; y < terrainTextureSize; y++)
            {
                for (int x = 0; x < terrainTextureSize; x++)
                {
                    heightInfo[x, y] = rgbValues[index];
                    index += 4; //r, g, b, a pro stride
                }
            }

            // Die Heightmap als OpenGL-Textur generieren
            heightMapTextureID = TextureManager.GenerateTextureFromBitmap(heightBitmap);


            tiles = new List<TerrainTile>();

            for (int xTile = -10; xTile < 11; xTile++)
            {
                for (int zTile = -10; zTile < 11; zTile++)
                {
                    /*
                    int lod = (int)Math.Sqrt(xTile * xTile + zTile * zTile);
                    lod -= 2;
                    if (lod < 1) lod = 1;
                    lod = lod * lod;
                    if (lod > 8) lod = 8;
                    */

                    int lod = 8;

                    tiles.Add(GenerateTerrainTile(xTile * 64, zTile * 64, lod));
                }
            }
            CreateTerrainShaders();
        }


        public float GetHeight(float xPos, float zPos)
        {
    
            int modAddition = terrainTextureSize * 20;

            float p1 = heightInfo[((int)xPos + modAddition) % terrainTextureSize, ((int)zPos + modAddition) % terrainTextureSize];
            float p2 = heightInfo[((int)(xPos + 1) + modAddition) % terrainTextureSize, ((int)zPos + modAddition) % terrainTextureSize];
            float p3 = heightInfo[((int)(xPos + 1) + modAddition) % terrainTextureSize, ((int)(zPos + 1) + modAddition) % terrainTextureSize];
            float p4 = heightInfo[((int)xPos + modAddition) % terrainTextureSize, ((int)(zPos + 1) + modAddition) % terrainTextureSize];

            float xMod = (xPos + modAddition) % 1.0f;
            float zMod = (zPos + modAddition) % 1.0f;

            float inP1 = (p2 - p1) * xMod + p1;
            float inP2 = (p3 - p4) * xMod + p4;

            return ((inP2 - inP1) * zMod + inP1) / 256.0f * currentHeight;
        }


        public void SetTerrainHeight(float terrainHeight)
        {
            currentHeight = terrainHeight;
        }


        private TerrainTile GenerateTerrainTile(float xMargin, float zMargin, int lod)
        {
            TerrainTile returnTile = new TerrainTile();
            List<float> tarrainData = new List<float>();
            List<int> indices = new List<int>();

            returnTile.midPoint = new Vector3(xMargin, 0, zMargin);

            int ind = 0;
            int loopStart = -32 / lod;
            int loopEnd = 32 / lod + 1;
            int indexSecRowLeft = 64 / lod + 1;
            int indexSecRowRight = 64 / lod + 2;
            int lastRowCompare = 32 / lod;

            for (int x = loopStart; x < loopEnd; x++)
            {
                for (int z = loopStart; z < loopEnd; z++)
                {
                    tarrainData.Add(x * lod + xMargin);
                    tarrainData.Add(0);
                    tarrainData.Add(z * lod + zMargin);
             
                    if (x < lastRowCompare && z < lastRowCompare)
                    {
                        indices.Add(ind);
                        indices.Add(ind + indexSecRowLeft);
                        indices.Add(ind + indexSecRowRight);
                        
                        indices.Add(ind);
                        indices.Add(ind + indexSecRowRight);
                        indices.Add(ind + 1);      
                    }
                    ind++;
                }
            }

            returnTile.IndexCount = indices.Count;

            int terrainVBO;
            GL.GenBuffers(1, out terrainVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, terrainVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tarrainData.Count * sizeof(float)), tarrainData.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            int indexBuffer;
            GL.GenBuffers(1, out indexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(uint) * indices.Count), indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.GenVertexArrays(1, out returnTile.Vao);
            GL.BindVertexArray(returnTile.Vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, terrainVBO);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
            GL.BindVertexArray(0);

            return returnTile;
        }


        private void CreateTerrainShaders()
        {
            program = ShaderCompiler.CreateShaderProgram("cgimin/terrain/Terrain_VS.glsl", "cgimin/terrain/Terrain_FS.glsl");

            GL.BindAttribLocation(program, 0, "in_position");
            GL.LinkProgram(program);

            modelviewProjectionMatrixLocation = GL.GetUniformLocation(program, "modelview_projection_matrix");
            camSubPositionLocation = GL.GetUniformLocation(program, "cam_sub_position");

            terrainXZPosLocation = GL.GetUniformLocation(program, "xzPos");
            terrainSizeLocation = GL.GetUniformLocation(program, "terrain_size");

            terrainHeightLocation = GL.GetUniformLocation(program, "terrain_height");
 
            materialShininessLocation = GL.GetUniformLocation(program, "specular_shininess");

            lightDirectionLocation = GL.GetUniformLocation(program, "light_direction");
            lightAmbientLocation = GL.GetUniformLocation(program, "light_ambient_color");
            lightDiffuseLocation = GL.GetUniformLocation(program, "light_diffuse_color");
            lightSpecularLocation = GL.GetUniformLocation(program, "light_specular_color");
            heightMapLocation = GL.GetUniformLocation(program, "height_map");
            colorTextureLocation = GL.GetUniformLocation(program, "color_texture");
            normalTextureLocation = GL.GetUniformLocation(program, "normalmap_texture");
            textureScaleLocation = GL.GetUniformLocation(program, "texture_scale");
            startHeightLocation = GL.GetUniformLocation(program, "start_height");
            fogStartLocation = GL.GetUniformLocation(program, "fogStart");
            fogEndLocation = GL.GetUniformLocation(program, "fogEnd");

            fogColorLocation = GL.GetUniformLocation(program, "fogColor");
        }


        public void Draw(float startheight, int textureID, int normalTextureID, float textureScale, float shininess) {
            
            GL.UseProgram(program);

            // Höhen-Textur wird "gebunden"
            GL.Uniform1(colorTextureLocation, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, heightMapTextureID);

            // Farb-Textur wird "gebunden"
            GL.Uniform1(colorTextureLocation, 1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // Normalmap-Textur wird "gebunden"
            GL.Uniform1(normalTextureLocation, 2);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, normalTextureID);

            // Die Licht Parameter werden übergeben
            GL.Uniform3(lightDirectionLocation, Light.lightDirection);
            GL.Uniform4(lightAmbientLocation, Light.lightAmbient);
            GL.Uniform4(lightDiffuseLocation, Light.lightDiffuse);
            GL.Uniform4(lightSpecularLocation, Light.lightSpecular);

            // Shininess
            GL.Uniform1(materialShininessLocation, shininess);

            float camXmod8 = (Camera.Position.X) % 8.0f;
            float camZmod8 = (Camera.Position.Z) % 8.0f;

            // Die Transformierung des Terrians, abhängig von der Kamera
            Matrix4 terrainTransformation = Camera.Transformation;
            // Die Position wird abgezogen, das Terrain "rotiert" lediglich mit der Kamera. Die "Positionierung" erfolgt auf dem Vertex-Shader 
            terrainTransformation *= Matrix4.CreateTranslation(-Camera.Transformation.M41, -Camera.Transformation.M42, -Camera.Transformation.M43);
            // Die Translation innerhalb der höchsten Kachel-Einheit 8 wird erstellt... 
            Vector4 camInTilePart = new Vector4(-camXmod8, -Camera.Position.Y, -camZmod8, 1);
            // ... multipliziert mit der Terrain-Transformation...
            camInTilePart *= terrainTransformation;
            // ... und wieder zur Terrain-Transformation hinzugefügt
            terrainTransformation *= Matrix4.CreateTranslation(camInTilePart.X, camInTilePart.Y, camInTilePart.Z);

            // ModelView-Projection in diesem Fall nur die terrainTransfomration (in der die Kamera rotation enthalten ist) *  Perspective-Projection  
            Matrix4 modelviewProjection = terrainTransformation * Camera.PerspectiveProjection;
            GL.UniformMatrix4(modelviewProjectionMatrixLocation, false, ref modelviewProjection);

            // Die ModelView-Matrix wird ebenfalls übergeben
            GL.Uniform3(camSubPositionLocation,  new Vector3(camXmod8, Camera.Position.Y, camZmod8));

            // Die XZ Postition für den Look-Up für die Höhe des Terrains wird berechnet. Jeweils die Kamera-Position - 
            Vector2 texXZPos = new Vector2(Camera.Position.X - camXmod8, Camera.Position.Z - camZmod8);
            GL.Uniform2(terrainXZPosLocation, ref texXZPos);

            // Die Dimension der Height-Map angeben
            GL.Uniform1(terrainSizeLocation, (float)terrainTextureSize);

            // Die Skalierung der Oberflächentextur
            GL.Uniform1(textureScaleLocation, textureScale);

            // Die Terrain-Höhe wird übergeben
            GL.Uniform1(terrainHeightLocation, currentHeight);
            GL.Uniform1(startHeightLocation, startheight);
            // Fog Values
            GL.Uniform1(fogStartLocation, Camera.FogStart);
            GL.Uniform1(fogEndLocation, Camera.FogEnd);
            GL.Uniform3(fogColorLocation, Camera.FogColor);

            for (int i = 0; i < tiles.Count; i++)
            {
                GL.BindVertexArray(tiles[i].Vao);
                GL.DrawElements(PrimitiveType.Triangles, tiles[i].IndexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }

            GL.BindVertexArray(0);
            GL.ActiveTexture(TextureUnit.Texture0);

        }
    


    }
}
