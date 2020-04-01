using System;
using OpenTK;
namespace TowerDefense.particles
{
    public class Particle
    {
        private Vector3 _position;
        private Vector3 _velocity;
        private float _gravity;
        private float _lifeTime;
        private float _rotation;
        private float _scale;
        private float _elapsedTime;
        private ParticleAtlas _texture;
        private float _blend;
        private Vector2 _textureOffsetCurrent;
        private Vector2 _textureOffsetNext;

        public Vector3 Position
        {
            get { return _position; }
        }

        public Vector3 Velocity
        {
            get { return _velocity; }
        }

        public ParticleAtlas Texture
        {
            get{ return _texture; }
            set{ _texture = value; }
        }

        public Vector2 TextureOffsetCurrent
        {
            get{ return _textureOffsetCurrent; }
            set{ _textureOffsetCurrent = value; }
        }

        public Vector2 TextureOffsetNext
        {
            get{ return _textureOffsetNext; }
            set{ _textureOffsetNext = value; }
        }

        public float Blend
        {
            get { return _blend; }
            set{ _blend = value; }
        }

        public float Scale
        {
            get{ return _scale; }
            set{ _scale = value; }
        }

        public Particle(Vector3 position, Vector3 velocity, ParticleAtlas texture, float gravity, float lifetime, float rotation, float scale)
        {
            _position = position;
            _velocity = velocity;
            _gravity = gravity;
            _lifeTime = lifetime;
            _rotation = rotation;
            Scale = scale;
            _texture = texture;
            TextureOffsetCurrent = new Vector2();
            TextureOffsetNext = new Vector2();
        }

        public bool Update(FrameEventArgs e, int index, float[] _texoffsets1,
                                                        float[] _texoffsets2,
                                                        float[] _centers,
                                                        float[] _billboardSize,
                                                        float[] _blends,
                                                        float[] _angles)
        {
            _velocity.Y -= _gravity * (float)e.Time;
            Vector3 change = new Vector3(_velocity);
            change = Vector3.Multiply(change, (float)e.Time);
            _position += change;
            TextureUpdate(e);

            _texoffsets1[index*2] = _textureOffsetCurrent.X;
            _texoffsets1[index*2 + 1] = _textureOffsetCurrent.Y;

            _texoffsets2[index * 2] = _textureOffsetNext.X;
            _texoffsets2[index * 2 + 1] = _textureOffsetNext.Y;

            _centers[index * 3] = _position.X;
            _centers[index * 3 + 1] = _position.Y;
            _centers[index * 3 + 2] = _position.Z;

            _billboardSize[index * 2] = Scale;
            _billboardSize[index * 2 + 1] = Scale;

            _blends[index] = _blend;

            _angles[index] = _rotation;


            _elapsedTime += (float)e.Time;
            return _elapsedTime < _lifeTime;
        }
        
        private void TextureUpdate(FrameEventArgs e)
        {
            float lifeNorm = _elapsedTime / _lifeTime;
            int textureCounts = _texture.RowsCount * _texture.ColumnCount;
            float atlasProg = lifeNorm * textureCounts;
            int currentIndex = (int)Math.Floor(atlasProg);
            int nextIndex = currentIndex;
            if (currentIndex < textureCounts - 1) nextIndex = currentIndex + 1;
            _blend = atlasProg % 1;
            SetTextureOffset(ref _textureOffsetCurrent, currentIndex);
            SetTextureOffset(ref _textureOffsetNext, nextIndex);    
        }

        private void SetTextureOffset(ref Vector2 offset, int index)
        {
            int column = index % _texture.ColumnCount;
            int row = index / _texture.RowsCount;
            offset.X = (float)column / _texture.ColumnCount;
            offset.Y = (float)row / _texture.RowsCount;
        }

    }
}
