using Engine.cgimin.camera;
using Engine.cgimin.sound;
using OpenTK;
using OpenTK.Input;
using TowerDefense.states.start;
using TowerDefense.gui;
using TowerDefense.gui.font;


namespace TowerDefense.states.menu
{
    /// <summary>
    /// Hauptmenü GUI
    /// </summary>
    class MainMenuState : IGameState
    {
        private FontLoader _fntLoader;
        private TextRenderer _textRender;
        private Text _title;
        private Text _start;
        private Text _exit;
        private GUIBackground _titleOverlay;
        private GUIButton _startOverlay;
        private GUIButton _exitOverlay;
        private GUIRenderer _guiRenderer;
        private int _textureOverlay;
        private Sound _music;

        public MainMenuState()
        {
            _music = new Sound(ResourceManager.Sounds["MUSIC_TITLE"], true);
            _music.SetPosition(Camera.Position);
            _music.Play();
            Camera.SetToPosition(new Vector3(-27, 36, 89));
            Camera.SetOrientation(new Vector3(2.4f, -0.52f, 0));
        }


        public override void Init()
        {
            base.Init();

            _guiRenderer = new GUIRenderer();
            int width = GameManager.Window.Width;
            int height = GameManager.Window.Height;
            _fntLoader = new FontLoader("assets/gui/inc.fnt", 8);
            _textRender = new TextRenderer(ResourceManager.Textures["TEXT_ATLAS_1"]);

            _title = new Text(_fntLoader, "Battle of the Thrones", width / 2 - 400, height / 2 - 280, width, height, 
                new Vector3(1, 1, 1), 1.00f, 1.0f, 0.5f, 0.2f);
            _start = new Text(_fntLoader, "Start", width / 2 - 100, height / 2, width, height,
                    new Vector3(1, 1, 1), 1.00f, 1.0f, 0.5f, 0.2f);
            _exit = new Text(_fntLoader, "Exit", width / 2 - 83, height / 2 + 100, width, height,
                    new Vector3(1, 1, 1), 1.00f, 1.0f, 0.5f, 0.2f);
            _textRender.Add(_title);
            _textRender.Add(_start);
            _textRender.Add(_exit);

            _textureOverlay = ResourceManager.GUI["OVERLAY_1"];
            _startOverlay = new GUIButton(width / 2 - 100, height / 2, 155, 60, width, height, 1f, _textureOverlay);
            _startOverlay.OverImage = _textureOverlay;
            _exitOverlay = new GUIButton(width / 2 - 100, height / 2 + 100, 155, 60, width, height, 1f, _textureOverlay);
            _exitOverlay.OverImage = _textureOverlay;
            _titleOverlay = new GUIBackground(width / 2 - 640, height / 2 - 280, 1280, 60, width, height, 1f, _textureOverlay);
            _guiRenderer.Add(_startOverlay);
            _guiRenderer.Add(_exitOverlay);
            _guiRenderer.Add(_titleOverlay);

        }

        public override void OnResize(int screenWidth, int screenHeight)
        {
            base.OnResize(screenWidth, screenHeight);
            Init();

        }

        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);
            _guiRenderer.Update(e, GameManager.Window.Mouse.GetState().IsButtonDown(MouseButton.Left), GameManager.Window.Mouse.GetState().IsButtonUp(MouseButton.Left), mouse.X, mouse.Y);
        }

        public override void Update(FrameEventArgs e)
        {
            int width = GameManager.Window.Width;
            int height = GameManager.Window.Height;
            if (_startOverlay.IsOver)
            {
                _start.ChangeText("Start", width / 2 - 100, height / 2,  1f);
            }
            else
            {
                _start.ChangeText("Start", width / 2 - 100, height / 2,  0.7f);
            }

            if (_exitOverlay.IsOver)
            {
                _exit.ChangeText("Exit", width / 2 - 83, height / 2 + 100,  1.0f);
            }
            else
            {
                _exit.ChangeText("Exit", width / 2 - 83, height / 2 + 100,  0.7f);
            }

            if (_startOverlay.IsClicked)
            {
                _music.UnLoad();
                PlayState playState = new PlayState("map/Map001.txt");
                GameManager.ChangeState(playState);
                GameManager.PushState(new PreviewStartState(playState));
                GameManager.RemoveGUIState(this);
   
            }
            else if (_exitOverlay.IsClicked)
            {
                GameManager.Window.Exit();
            }

        }

        public override void Render(FrameEventArgs e)
        {
            _guiRenderer.Render();
            _textRender.Render();
        }

        public override void Close()
        {
            base.Close();
            _textRender.UnLoad();
        }
    }
}
