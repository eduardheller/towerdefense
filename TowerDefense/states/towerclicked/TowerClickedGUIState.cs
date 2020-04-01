using Engine.cgimin.object3d;
using OpenTK;
using OpenTK.Input;
using TowerDefense.objects;
using Engine.cgimin.material.alphatexture;

namespace TowerDefense.states.towerclicked
{
    /// <summary>
    /// State, die die Visualisierung bei dem Anklicken eines Tower da ist
    /// </summary>
    class TowerClickedGUIState : IGameState
    {
        private AlphaTextureMaterial _alphaTextureMaterial;
        private PlaneObject3D _radius;
        private PlaneObject3D _inner;
        private int _textureRadius;
        private Tower _tower;
        private float _timer;

        public TowerClickedGUIState(Tower tower)
        {
            
            _tower = tower;

        }

        public override void Init()
        {
            base.Init();
            _radius = new PlaneObject3D();
            _inner = new PlaneObject3D();
            _textureRadius = ResourceManager.Textures["RADIUS"];
            _alphaTextureMaterial = new AlphaTextureMaterial();
        }

        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);

        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            _timer += (float)e.Time;

            _radius.Transformation = Matrix4.CreateScale(_tower.Radius);
            _radius.Transformation *= Matrix4.CreateTranslation(_tower.Position + new Vector3(0, 0.1f, 0));

            _inner.Transformation = Matrix4.CreateScale(1.0f);
            _inner.Transformation *= Matrix4.CreateTranslation(_tower.Position + new Vector3(0, 0.12f, 0));

        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);

            _alphaTextureMaterial.Draw(_radius, _textureRadius, 0.5f);
            _alphaTextureMaterial.Draw(_inner, _textureRadius, 0.8f);
        }

        public override void OnResize(int screenWidth, int screenHeight)
        {
            base.OnResize(screenWidth, screenHeight);
            GameManager.RemoveState(this);
        }

        public override void Close()
        {
            base.Close();
            _radius.UnLoad();
            _inner.UnLoad();
        }
    }
}
