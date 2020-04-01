using OpenTK;
using OpenTK.Input;
using TowerDefense.gui.font;

namespace TowerDefense.states.start
{

    /// <summary>
    /// Texteinblendung bei der Kamerafahrt am Anfang der Map
    /// </summary>
    class PreviewStartGUIState : IGameState
    {

        private FontLoader _fntLoader;
        private TextRenderer _textRender;
        private Text _text;
        private int _textAtlas;
        private PreviewStartState _cameraPreview;
        private int _stageCount;
        private float _alphaTimer;
        public PreviewStartGUIState(PreviewStartState camprev)
        {

            _cameraPreview = camprev;
            _stageCount = 0;
        }

        public override void Init()
        {
            base.Init();
            int width = GameManager.Window.Width;
            int height = GameManager.Window.Height;
            _fntLoader = new FontLoader("assets/gui/inc.fnt", 8);
            _textAtlas = ResourceManager.Textures["TEXT_ATLAS_1"];
            _textRender = new TextRenderer(_textAtlas);
            _text = new Text(_fntLoader, "Sunset Valley", width / 2 - 220, height / 2, width, height, new Vector3(1, 1, 1), 1.00f, 1.0f, 0.5f, 0.2f);
            _textRender.Add(_text);
        }

        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            int width = GameManager.Window.Width;
            int height = GameManager.Window.Height;
            _alphaTimer += (float)e.Time;
            if (_cameraPreview.NextStage)
            {
                _stageCount++;
                _alphaTimer = 0.0f;
            }

            switch (_stageCount)
            {
                case 0:
                    _text.ChangeText("Game made by Eduard Heller", width / 2 - 520, height / 2, _alphaTimer);
                    break;
                case 1:
                    _text.ChangeText("Dont let anyone get there", width / 2 - 480, height / 2, _alphaTimer);
                    break;
                case 2:
                    _text.ChangeText("Enemies are coming from there", width / 2 - 480, height / 2, _alphaTimer);
                    break;
                case 3:
                    _text.ChangeText("Defend yourself with your Towers", width / 2 - 600, height / 2, _alphaTimer);
                    break;
                default:
                    break;
            }
        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);

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
