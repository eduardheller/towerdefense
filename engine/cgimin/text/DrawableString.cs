using System;
using Engine.cgimin.material.alphatexture;
using Engine.cgimin.material.simpletexture;
using Engine.cgimin.object3d;
using Engine.cgimin.texture;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.cgimin.text
{
    public class DrawableString
    {
        private static readonly int texture = TextureManager.LoadTexture("cgimin/text/dina.png");
        private static readonly TextMaterial material = new TextMaterial();

        private readonly StringObject stringObject;

        /// <summary>
        /// Text mit dem der DrawableString erstellt wurde.
        /// </summary>
        public String Text { get; }

        /// <summary>
        /// Transformations Matrix
        /// </summary>
        public Matrix4 Transformation
        {
            get { return stringObject.Transformation; }
            set { stringObject.Transformation = value; }
        }

        public DrawableString(String text)
        {
            Text = text;
            stringObject = new StringObject(text);
        }

		public void Draw(BlendingFactorSrc blendSource = BlendingFactorSrc.SrcAlpha, BlendingFactorDest blendDest = BlendingFactorDest.OneMinusSrcAlpha)
        {
            // Text sollte immer sichtbar sein
            GL.Disable(EnableCap.DepthTest);
			material.Draw(stringObject, DrawableString.texture, 1.0f, blendSource, blendDest);
            GL.Enable(EnableCap.DepthTest);
        }

        public void UnLoad()
        {
            stringObject.UnLoad();
        }

        class StringObject : BaseObject3D
        {
            private const int CharWidth = 64;
            private const int CharHeight = 64;
            private const int RowCount = 16;
            private const int ColumnCount = 16;
            private const float BitmapWidth = 1024;
            private const float BitmapHeight = 1024;

            public StringObject(String text)
            {
                int xBase = 0;
                int yBase = 0;

                foreach (var c in text)
                {
                    if (c == '\n')
                    {
                        yBase--;
                        xBase = 0;
                        continue;
                    }

                    DrawChar(c, xBase, yBase);

                    xBase++;
                }

                CreateVAO();
            }

            private void DrawChar(char c, int xBase, int yBase)
            {
                if (c < 0 || c >= 256)
                    throw new ArgumentOutOfRangeException(nameof(c));

                // Position innerhalb des Bitmaps ermitteln
                int charPos = (int)c;
                int y = (charPos / RowCount) * CharHeight;
                int x = (charPos % ColumnCount) * CharWidth;

                // UV Koordianten berechnen
                float top = y / BitmapHeight;
                float bottom = (y + CharHeight) / BitmapHeight;
                float left = x / BitmapWidth;
                float right = (x + CharWidth) / BitmapWidth;

                // Triangles für Back- und Frontface
                addTriangle(
                    new Vector3(xBase, yBase, 0), new Vector3(xBase, yBase + 1, 0), new Vector3(xBase + 1, yBase, 0),
                    Vector3.One, Vector3.One, Vector3.One,
                    new Vector2(left, bottom), new Vector2(left, top), new Vector2(right, bottom));

                addTriangle(
                    new Vector3(xBase + 1, yBase + 1, 0), new Vector3(xBase + 1, yBase, 0), new Vector3(xBase, yBase + 1, 0),
                    Vector3.One, Vector3.One, Vector3.One,
                    new Vector2(right, top), new Vector2(right, bottom), new Vector2(left, top));

                addTriangle(
                    new Vector3(xBase, yBase, 0), new Vector3(xBase + 1, yBase, 0), new Vector3(xBase, yBase + 1, 0),
                    Vector3.One, Vector3.One, Vector3.One,
                    new Vector2(left, bottom), new Vector2(right, bottom), new Vector2(left, top));

                addTriangle(
                    new Vector3(xBase + 1, yBase + 1, 0), new Vector3(xBase, yBase + 1, 0), new Vector3(xBase + 1, yBase, 0),
                    Vector3.One, Vector3.One, Vector3.One,
                    new Vector2(right, top), new Vector2(left, top), new Vector2(right, bottom));
            }


        }
    }
}
