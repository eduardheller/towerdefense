using Engine.cgimin.material.ship;
using Engine.cgimin.object3d;
using OpenTK;
using OpenTK.Input;

namespace TowerDefense.states.visual
{
    /// <summary>
    /// State für das Schiff, dass bei jeder Well zufliegt und wegfliegt
    /// </summary>
    public class ShipWaveState : IGameState
    {

        private PlayState _playState;
        private ShipMaterial _shipMaterial;
        private BaseObject3D _ship;
        private int _texture;
        private Vector3 _position;
        private Vector3 _end;
        private float _alpha;
        private bool _isAtDestination;
        private float _time;
        private bool _disappearing;
        private float _smoothTimer;

        public ShipWaveState(PlayState playState)
        {

            _playState = playState;
            _position = _playState.MapContext.StartPosition + new Vector3(0,10,-5.5f);
            _end = _playState.MapContext.StartPosition + new Vector3(0,1, -5.5f) ;
            _alpha = 0.0f;
            _time = 0.0f;
            _isAtDestination = false;
            _disappearing = false;
            _smoothTimer = 1.0f;
        }

        public override void Init()
        {
            base.Init();
            _shipMaterial = new ShipMaterial();
            _texture = ResourceManager.Textures["SHIP"];
            _ship = ResourceManager.Objects["SHIP"];
            _ship.Transformation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-90));
            _ship.Transformation *= Matrix4.CreateTranslation(_position);
            if (!_playState.Music.IsPlaying()) _playState.Music.Play();
        }


        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);

            _position = Vector3.Lerp(_position, _end, (float)e.Time* _smoothTimer);
 
            _ship.Transformation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-90));
            _ship.Transformation *= Matrix4.CreateTranslation(_position);

            if (_position.Y < _end.Y+0.1f && !_isAtDestination)
            {
                _position = _end;
                _playState.WavePause = false;
                _playState.AssignShipState(this);
                _isAtDestination = true;
                
            }

            if (_position.Y >= _end.Y-0.1f && _disappearing)
            {
                GameManager.RemoveState(this);
            }


            if (!_disappearing) _alpha += (float)e.Time;
            else _alpha -= (float)e.Time;
            _time += (float)e.Time;

            if (_disappearing) _smoothTimer += (float)e.Time;
        }

        public void Dissapear()
        {
            if (!_disappearing)
            {
                _end = _playState.MapContext.StartPosition + new Vector3(0, 10, -5.5f);
                _disappearing = true;
                _alpha = 1.0f;
                _smoothTimer = 0.0f;
            }

        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);
            
            _shipMaterial.Draw(_ship, _ship.Transformation,_time, _alpha,  _texture, 32);
        }

        public override void OnResize(int screenWidth, int screenHeight)
        {
            base.OnResize(screenWidth, screenHeight);
            
        }
    }
}
