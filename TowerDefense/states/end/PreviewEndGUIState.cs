using Engine.cgimin.camera;
using Engine.cgimin.sound;
using OpenTK;
using OpenTK.Input;
using TowerDefense.gui;
using TowerDefense.gui.font;

namespace TowerDefense.states.end
{
    /// <summary>
    /// GUI für das Ende des Spiels (Verloren oder Gewonnen)
    /// </summary>
    class PreviewEndGUIState : IGameState
    {
        private int _textureButton;
        private GUIButton _button;
        private GUIRenderer _guiRenderer;
        private PlayState _playState;
        private bool _won;
        private Sound _sound;
        private FontLoader _fntLoader;
        private TextRenderer _textRender;
        private Text _title;
        private int _textAtlas;

        public PreviewEndGUIState(PlayState playState, bool won)
        {
            _playState = playState;
            _won = won;

            if (_won) {
                _sound = new Sound(ResourceManager.Sounds["WIN"]);
                _sound.SetPosition(Camera.Position);
                _sound.Play();
            }
            else
            {
                _sound = new Sound(ResourceManager.Sounds["LOST"],true);
                _sound.SetPosition(Camera.Position);
                _sound.Play();
            }
        }

        public override void Init()
        {
            base.Init();

            _textureButton = ResourceManager.GUI["OVERLAY_1"];

            int width = GameManager.Window.Width;
            int height = GameManager.Window.Height;
            _fntLoader = new FontLoader("assets/gui/inc.fnt", 8);
            _textAtlas = ResourceManager.Textures["TEXT_ATLAS_1"];
            _textRender = new TextRenderer(_textAtlas);

            _title = new Text(_fntLoader, "Victory", width / 2 - 300, height / 2 , width, height,
                new Vector3(1, 1, 1), 1.00f, 1.0f, 0.5f, 0.2f);


            _textRender.Add(_title);

            _button = new GUIButton(width / 2 - 500, height / 2, 1000, 66, width, height, 0.9f, _textureButton);
            _button.OverImage = _textureButton;

            _guiRenderer = new GUIRenderer();
            _guiRenderer.Add(_button);

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

            string text= "Victory, Click to Exit";

            if (!_won) text = "Game Over, Click to Restart";

            if (_button.IsOver)
            {
                _title.ChangeText(text, width / 2 - 450, height / 2, 1.0f);
            }
            else
            {
                _title.ChangeText(text, width / 2 - 450, height / 2, 0.7f);
            }

            if (_button.IsClicked && _won)
            {
                GameManager.RemoveGUIState(this);
                GameManager.Window.Exit();
            }

            if (_button.IsClicked && !_won)
            {
                // Falls restartet werden soll
                _sound.UnLoad();
                Camera.LerpOrientation = false;
                Camera.SetToPosition(new Vector3(13.6f, 15.3f, 26.92f));
                Camera.SetOrientation(new Vector3(3.14f, -0.89f, 0));
                GameManager.ChangeState(new PlayState("map/Map001.txt"));
            }


            _sound.SetPosition(Camera.Position);
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
