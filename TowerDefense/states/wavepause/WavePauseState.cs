using OpenTK;
using OpenTK.Input;
using TowerDefense.gui;
using TowerDefense.gui.font;
using TowerDefense.states.visual;

namespace TowerDefense.states.wavepause
{
    /// <summary>
    /// GUI die für die Pause zwischen den Wellen angezeigt wird
    /// </summary>
    class GUIWavePauseState : IGameState
    {
        private GUIButton _buttonNextWave;
        private GUIRenderer _guiRenderer;
        private PlayState _playState;
        private FontLoader _fntLoader;
        private TextRenderer _textRender;
        private Text _text;
        private int _textAtlas;
        private bool _boss;
        public GUIWavePauseState(PlayState playState, bool boss = false)
        {

            _playState = playState;
            _boss = boss;
        }

        public override void Init()
        {
            base.Init();
            int width = GameManager.Window.Width;
            int height = GameManager.Window.Height;
            _fntLoader = new FontLoader("assets/gui/inc.fnt", 8);
            _textAtlas = ResourceManager.Textures["TEXT_ATLAS_1"];
            _textRender = new TextRenderer(_textAtlas);

            _text = new Text(_fntLoader, "Next Wave", width / 2 - 140, 400, width, height, new Vector3(1, 1, 1), 0.8f, 0.7f, 0.5f, 0.2f);
            _textRender.Add(_text);


            _buttonNextWave = new GUIButton(width / 2 - 640, 140, 1280, 66, width, height, 0.9f, ResourceManager.GUI["OVERLAY_1"]);
            _buttonNextWave.OverImage = ResourceManager.GUI["OVERLAY_1"];
            _guiRenderer = new GUIRenderer();
            _guiRenderer.Add(_buttonNextWave);

        }


        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            _guiRenderer.Update(e, GameManager.Window.Mouse.GetState().IsButtonDown(MouseButton.Left), GameManager.Window.Mouse.GetState().IsButtonUp(MouseButton.Left),
                GameManager.Window.Mouse.X, GameManager.Window.Mouse.Y);

            int width = GameManager.Window.Width;
            int height = GameManager.Window.Height;

            string wavestring = "Next Wave";

            if (_buttonNextWave.IsOver)
            {
                _text.ChangeText(wavestring, width / 2 - 120, 150, 1.0f);
            }
            else
            {
                _text.ChangeText(wavestring, width / 2 - 120, 150, 0.7f);
            }

            if (_buttonNextWave.IsClicked)
            {
                GameManager.RemoveGUIState(this);
                GameManager.PushState(new ShipWaveState(_playState));
            }


        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);
            _guiRenderer.Render();
            _textRender.Render();
        }

        public override void Close()
        {
            base.Close();
            _textRender.UnLoad();
        }

        public override void OnResize(int screenWidth, int screenHeight)
        {
            base.OnResize(screenWidth, screenHeight);
            Init();
        }
    }
}
