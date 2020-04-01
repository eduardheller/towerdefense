using OpenTK;
using Engine.cgimin.object3d;
using Engine.cgimin.material.normalmapping;

namespace TowerDefense.objects
{
    public class Way : GameObject
    {
  
        private int _id;
        private static int _idCounter = 0;

        private NormalMappingMaterial _normalMappingMaterial;
        private int _textureCube;
        private int _textureNormal;
        private const float SHININESS = 32.0f;

        public Way()
        {

            _id = _idCounter;
            _idCounter++;
            _normalMappingMaterial = new NormalMappingMaterial();
            _textureCube = ResourceManager.Textures["BLOCK_WAY_1"];
            _textureNormal = ResourceManager.Textures["BLOCK_WAY_NORMAL_1"];
            _obj = ResourceManager.Objects["BLOCK_1"];
        }

        public override void Render(FrameEventArgs e)
        {
            _normalMappingMaterial.Draw(_obj, Transformation, _textureCube, _textureNormal, SHININESS);
        }
    }
}
