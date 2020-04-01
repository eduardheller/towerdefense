using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Engine.cgimin.texture
{
    public class TextureManager
    {

        // Methode zum Laden einer Textur
        public static int LoadTexture(string fullAssetPath, bool clampEdges = false, bool filtered = true)
        {
            return GenerateTextureFromBitmap(new Bitmap(fullAssetPath), clampEdges, filtered);
        }


        // generiert eine Textur aus einer Bitmap
        public static int GenerateTextureFromBitmap(Bitmap bmp, bool clampEdges = false, bool filtered = true)
        {
            // Textur wird generiert
            int returnTextureID = GL.GenTexture();

            // Textur wird "gebunden", folgende Befehle beziehen sich auf die gesetzte Textur (Statemachine)
            GL.BindTexture(TextureTarget.Texture2D, returnTextureID);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            

            // Textur-Parameter, Pixelformat etc.
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            if (filtered)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);

            }

            if (clampEdges)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            }
            else {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            }

            float maxAniso;
            GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);

            bmp.UnlockBits(bmpData);

            bmp.Dispose();
            // Mip-Map Daten werden generiert
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Textur-ID wird zurückgegeben
            return returnTextureID;
        }


        public static int LoadCubemap(List<string> faces)
        {
            //GLuint textureID;
            //glGenTextures(1, &textureID);
            int textureID = GL.GenTexture();

            //glActiveTexture(GL_TEXTURE0);
            GL.ActiveTexture(TextureUnit.Texture0);

            //glBindTexture(GL_TEXTURE_CUBE_MAP, textureID);
            GL.BindTexture(TextureTarget.TextureCubeMap, textureID);

            for (int i = 0; i < faces.Count; i++)
            {

                Bitmap bmp = new Bitmap(faces[i]);
                int width = bmp.Width;
                int height = bmp.Height;

                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
           

            return textureID;
        }  


    }
}
