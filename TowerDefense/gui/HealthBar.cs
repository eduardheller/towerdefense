using OpenTK;
using Engine.cgimin.object3d;
using Engine.cgimin.material.billboard;
using TowerDefense.objects;
using OpenTK.Graphics.OpenGL;

namespace TowerDefense.gui
{
    class HealthBar
    {

        private static int _texture = ResourceManager.GUI["HEALTHBAR_BORDER"];
        private static int _textureInner = ResourceManager.GUI["HEALTHBAR"];
        private QuadObject3D _healthBar;
        private BillboardMaterial _billboardMaterial;
        private float _width;
        private float _height;
        private Vector3 _position;
        private Vector2 _size;
        private float _alpha;
        private Vector3 _hPosition;
        
        public HealthBar(float alpha)
        {
            _alpha = alpha;
            _width = 0.60f;
            _height = 0.080f;
            _healthBar = new QuadObject3D();
            _billboardMaterial = new BillboardMaterial();
            _position = Vector3.Zero;
            _size = Vector2.Zero;
        }

        public void Update(Enemy enemy)
        {
            _position = new Vector3(enemy.Position.X, enemy.Position.Y + 1.0f, enemy.Position.Z);
            _hPosition = _position;
            float perc = (enemy.HP * _width) / enemy.MaxHP;
            _size = new Vector2(perc, _height);
  
        }

        public void Render()
        {
            GL.Disable(EnableCap.DepthTest);
            _billboardMaterial.Draw(_healthBar, _hPosition, _size, _alpha, _textureInner);
            _billboardMaterial.Draw(_healthBar, _position, new Vector2(_width,_height), _alpha, _texture);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
