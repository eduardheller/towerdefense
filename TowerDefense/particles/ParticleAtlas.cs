using OpenTK.Graphics.OpenGL;

namespace TowerDefense.particles
{
    public class ParticleAtlas
    {
        private int _texture;
        private int _rowsCount;
        private int _columnCount;
        private BlendingFactorSrc _src;
        private BlendingFactorDest _dest;

        public int RowsCount
        {
            get { return _rowsCount; }
        }

        public int Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        public int ColumnCount
        {
            get
            {
                return _columnCount;
            }

        }

        public BlendingFactorSrc Src
        {
            get
            {
                return _src;
            }

        }

        public BlendingFactorDest Dest
        {
            get
            {
                return _dest;
            }

        }

        public ParticleAtlas(int texture, int rows, int columns, BlendingFactorSrc src = BlendingFactorSrc.One, BlendingFactorDest dest = BlendingFactorDest.One)
        {
            Texture = texture;
            _rowsCount = rows;
            _columnCount = columns;
            _src = src;
            _dest = dest;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is ParticleAtlas)) { return false; }
            return (this._texture == ((ParticleAtlas)obj)._texture);
        }

        public override int GetHashCode()
        {
            if (_texture < 0) return 0;
            return this._texture.GetHashCode();
        }
    }
}
