using OpenTK;
using TowerDefense.objects.towers;
using TowerDefense.gui.font;
using TowerDefense.gui;
using OpenTK.Input;
using Engine.cgimin.camera;
using TowerDefense.states.towerbuild;

namespace TowerDefense.states
{
    /// <summary>
    /// Haupt GUI im Spiel, enthält Buttons für die Tower, sowie Info über Gold,Lives etc...
    /// </summary>
    public class PlayStateGUI : IGameState
    {
        private const int TOWER_BUTTON_XOFFSET = 170;

        private GUIRenderer _guiRenderer;
        private GUIBackground _infoGUI;
        private GUIBackground _buttomOverlay;
        private GUIButton _buttonTowerCanon;
        private GUIButton _buttonTowerArrow;
        private GUIButton _buttonTowerTesla;
        private GUIButton _buttonTowerMage;
        private GUIButton _bloom;
        private GUIButton _bloomOff;
        private GUIButton _exit;
        private GUIButton _restart;

        private GUIToolTip _toolTipArrow;
        private GUIToolTip _toolTipCanon;
        private GUIToolTip _toolTipMage;
        private GUIToolTip _toolTipTesla;

        private FontLoader _fntLoader;
        private TextRenderer _textRender;

        private Text _waveText;
        private Text _lifeText;
        private Text _goldText;
        private Text _fpsText;

        private Text _towerArrowText;
        private Text _towerCanonText;
        private Text _towerMageText;
        private Text _towerTeslaText;

        private PlayState _playState;

        private bool _isOverGUI;

        public bool IsOverGUI
        {
            get { return _isOverGUI; }
        }

        public PlayStateGUI(PlayState playstate)
        {
            _playState = playstate;
            _isOverGUI = false;
        }

        public override void Init()
        {
            base.Init();
            _guiRenderer = new GUIRenderer();
            _fntLoader = new FontLoader("assets/gui/inc.fnt", 8);
            _textRender = new TextRenderer(ResourceManager.Textures["TEXT_ATLAS_1"]);

            int width = GameManager.Window.Width;
            int height = GameManager.Window.Height;

            CreateBottomTowerButtons(width, height);
            CreateOptionButtons(width, height);
            CreateStaticOverlays(width, height);
            CreateInfoText(width, height); 
            CreateTowerGoldText(width, height);
        }

        public override void OnResize(int screenWidth, int screenHeight)
        {
            base.OnResize(screenWidth, screenHeight);
            Init();
        }

        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);
            _guiRenderer.Update(e, GameManager.Window.Mouse.GetState().IsButtonDown(MouseButton.Left), 
                GameManager.Window.Mouse.GetState().IsButtonUp(MouseButton.Left), mouse.X, mouse.Y);

            if (_exit.IsClicked)
            {
                GameManager.Window.Exit();
            }

            if(_restart.IsClicked)
            {
                Camera.SetToPosition(new Vector3(13.6f, 15.3f, 26.92f));
                Camera.SetOrientation(new Vector3(3.14f, -0.89f, 0));
                GameManager.ChangeState(new PlayState("map/Map001.txt"));
            }

            

            if (_bloom.IsClicked)
            {
                GameEngine.Bloom = false;
                _bloomOff.IsVisible = true;
                _bloom.IsVisible = false;
            }
            else if (_bloomOff.IsClicked)
            {
                GameEngine.Bloom = true;
                _bloom.IsVisible = true;
                _bloomOff.IsVisible = false;
            }

            if (_buttonTowerArrow.IsOver)
            {
                _isOverGUI = true;
            }
            else if (_buttonTowerCanon.IsOver)
            {
                _isOverGUI = true;
            }
            else if (_buttonTowerMage.IsOver)
            {
                _isOverGUI = true;
            }
            else if (_buttonTowerTesla.IsOver)
            {
                _isOverGUI = true;
            }
            else
            {
                _isOverGUI = false;
            }
        }

        public override void Update(FrameEventArgs e)
        {

            int width = GameManager.Window.Width;
            int height = GameManager.Window.Height;

            _goldText.ChangeText(_playState.Player.Gold.ToString(),  width/2 - 125, 15);
            _waveText.ChangeText(_playState.Player.CurrentWave + "/" + _playState.Player.MaxWave, width / 2 - 10, 15);
            _lifeText.ChangeText(_playState.Player.Lives.ToString(), width / 2 + 108, 15);
            _fpsText.ChangeText("FPS:" + (1f / e.Time).ToString("0."), 5, 5);

            if (CanonTower.StartCosts > _playState.Player.Gold)
                _buttonTowerCanon.Disable();
            else
                _buttonTowerCanon.Enable();

            if (MageTower.StartCosts > _playState.Player.Gold)
                _buttonTowerMage.Disable();
            else
                _buttonTowerMage.Enable();

            if (TeslaTower.StartCosts > _playState.Player.Gold)
                _buttonTowerTesla.Disable();
            else
                _buttonTowerTesla.Enable();

            if (ArrowTower.StartCosts > _playState.Player.Gold)
                _buttonTowerArrow.Disable();
            else
                _buttonTowerArrow.Enable();


     
            
            if (_buttonTowerArrow.IsClicked)
            {
                _playState.GameManager.PushState(new TowerBuildState(this, _playState, typeof(ArrowTower)));
                HideButtons();
            }
            else if (_buttonTowerCanon.IsClicked)
            {
                _playState.GameManager.PushState(new TowerBuildState(this, _playState, typeof(CanonTower)));
                HideButtons();
            }
            else if (_buttonTowerMage.IsClicked)
            {
                _playState.GameManager.PushState(new TowerBuildState(this, _playState, typeof(MageTower)));
                HideButtons();
            }
            else if (_buttonTowerTesla.IsClicked)
            {
                _playState.GameManager.PushState(new TowerBuildState(this, _playState, typeof(TeslaTower)));
                HideButtons();
            }

        }

        public void ShowButtons()
        {
            _buttonTowerArrow.IsVisible = true;
            _buttonTowerCanon.IsVisible = true;
            _buttonTowerMage.IsVisible = true;
            _buttonTowerTesla.IsVisible = true;
            _towerArrowText.IsVisible = true;
            _towerCanonText.IsVisible = true;
            _towerMageText.IsVisible = true;
            _towerTeslaText.IsVisible = true;
            _buttomOverlay.IsVisible = true;

        }

        public void HideButtons()
        {
            _buttonTowerArrow.IsVisible = false;
            _buttonTowerCanon.IsVisible = false;
            _buttonTowerMage.IsVisible = false;
            _buttonTowerTesla.IsVisible = false;
            _towerArrowText.IsVisible = false;
            _towerCanonText.IsVisible = false;
            _towerMageText.IsVisible = false;
            _towerTeslaText.IsVisible = false;
            _buttomOverlay.IsVisible = false;
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


        private void CreateBottomTowerButtons(int width, int height)
        {
            _buttonTowerArrow = new GUIButton(width / 2 - 310, height - 155, 121, 123, 
                width, height, 1f, ResourceManager.GUI["BUTTON_TOWER_ARROW"]);
            _buttonTowerArrow.OverImage = ResourceManager.GUI["BUTTON_TOWER_ARROW_OVER"];
            _buttonTowerArrow.DisableImage = ResourceManager.GUI["BUTTON_TOWER_ARROW_DISABLED"];

            _buttonTowerCanon = new GUIButton(width / 2 - 310 + (TOWER_BUTTON_XOFFSET), height - 155, 
                121, 123, width, height, 1f, ResourceManager.GUI["BUTTON_TOWER_CANON"]);
            _buttonTowerCanon.OverImage = ResourceManager.GUI["BUTTON_TOWER_CANON_OVER"];
            _buttonTowerCanon.DisableImage = ResourceManager.GUI["BUTTON_TOWER_CANON_DISABLED"];

            _buttonTowerMage = new GUIButton(width / 2 - 310 + (TOWER_BUTTON_XOFFSET * 2), height - 155, 
                121, 123, width, height, 1f, ResourceManager.GUI["BUTTON_TOWER_MAGE"]);
            _buttonTowerMage.OverImage = ResourceManager.GUI["BUTTON_TOWER_MAGE_OVER"];
            _buttonTowerMage.DisableImage = ResourceManager.GUI["BUTTON_TOWER_MAGE_DISABLED"];

            _buttonTowerTesla = new GUIButton(width / 2 - 310 + (TOWER_BUTTON_XOFFSET * 3), height - 155, 
                121, 123, width, height, 1f, ResourceManager.GUI["BUTTON_TOWER_TESLA"]);
            _buttonTowerTesla.OverImage = ResourceManager.GUI["BUTTON_TOWER_TESLA_OVER"];
            _buttonTowerTesla.DisableImage = ResourceManager.GUI["BUTTON_TOWER_TESLA_DISABLED"];

            _buttomOverlay = new GUIBackground(width / 2 - 640, height - 32, 1280, 30, width, height, 
                1f, ResourceManager.GUI["OVERLAY_1"]);

           

            Text toolTipText = new Text(_fntLoader, "", 5, 92, width, height,
                new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _textRender.Add(toolTipText);
            _toolTipArrow = new GUIToolTip(_buttonTowerArrow, toolTipText, "Single, High Range", 240, 40, width, height, 0.6f, 0);

            toolTipText = new Text(_fntLoader, "", 5, 92, width, height,
                new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _textRender.Add(toolTipText);
            _toolTipCanon = new GUIToolTip(_buttonTowerCanon, toolTipText, "Multiple Targets, Slow", 270, 40, width, height, 0.6f, 0);

            toolTipText = new Text(_fntLoader, "", 5, 92, width, height,
                new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _textRender.Add(toolTipText);
            _toolTipMage = new GUIToolTip(_buttonTowerMage, toolTipText, "Weak but Freezes", 209, 40, width, height, 0.6f, 0);

            toolTipText = new Text(_fntLoader, "", 5, 92, width, height,
                new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _textRender.Add(toolTipText);
            _toolTipTesla = new GUIToolTip(_buttonTowerTesla, toolTipText, "Short Range, Fast", 225, 40, width, height, 0.6f, 0);

            _guiRenderer.Add(_buttonTowerArrow);
            _guiRenderer.Add(_buttonTowerCanon);
            _guiRenderer.Add(_buttonTowerMage);
            _guiRenderer.Add(_buttonTowerTesla);
            _guiRenderer.Add(_buttomOverlay);
            _guiRenderer.Add(_toolTipArrow);
            _guiRenderer.Add(_toolTipCanon);
            _guiRenderer.Add(_toolTipMage);
            _guiRenderer.Add(_toolTipTesla);
        }

        private void CreateOptionButtons(int width, int height)
        {
            _bloom = new GUIButton(width - 140, 2, 35, 35, width, height,
                0.9f, ResourceManager.GUI["BUTTON_BLOOM"]);
            _bloom.OverImage = ResourceManager.GUI["BUTTON_BLOOM"];
            _bloom.IsVisible = false;

            _bloomOff = new GUIButton(width - 140, 2, 35, 35, width, height,
                0.9f, ResourceManager.GUI["BUTTON_BLOOM_OFF"]);
            _bloomOff.OverImage = ResourceManager.GUI["BUTTON_BLOOM_OFF"];

            _exit = new GUIButton(width - 40, 2, 35, 35, width, height,
                0.9f, ResourceManager.GUI["BUTTON_EXIT"]);
            _exit.OverImage = ResourceManager.GUI["BUTTON_EXIT"];

            _restart = new GUIButton(width - 90, 2, 35, 35, width, height,
                  0.8f, ResourceManager.GUI["BUTTON_RESTART"]);
            _restart.OverImage = ResourceManager.GUI["BUTTON_RESTART"];

            _guiRenderer.Add(_bloom);
            _guiRenderer.Add(_bloomOff);
            _guiRenderer.Add(_exit);
            _guiRenderer.Add(_restart);
        }

        private void CreateStaticOverlays(int width, int height)
        {
            _infoGUI = new GUIBackground(width/2-171, 2, 343, 43, width, height,
                0.7f, ResourceManager.GUI["INFO"]);

            _guiRenderer.Add(_infoGUI);

        }

        private void CreateInfoText(int width, int height)
        {
            _goldText = new Text(_fntLoader, "Gold: ", 5, 24, width, height, 
                new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _waveText = new Text(_fntLoader, "Wave: ", 5, 59, width, height, 
                new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _lifeText = new Text(_fntLoader, "Lives: ", 5, 92, width, height, 
                new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _fpsText = new Text(_fntLoader, "FPS: ", 5, 92, width, height, 
                new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);

            _textRender.Add(_goldText);
            _textRender.Add(_waveText);
            _textRender.Add(_lifeText);
            _textRender.Add(_fpsText);
        }

        private void CreateTowerGoldText(int width, int height)
        {
            _towerArrowText = new Text(_fntLoader, ArrowTower.StartCosts.ToString(), 
                width / 2 - 263, height - 30, width, height, 
                new Vector3(1, 1, 1), 0.40f, 1.0f, 0.5f, 0.2f);
            _towerCanonText = new Text(_fntLoader, CanonTower.StartCosts.ToString(),
                width / 2 - 263 + (TOWER_BUTTON_XOFFSET), height - 30, width, height, 
                new Vector3(1, 1, 1), 0.40f, 1.0f, 0.5f, 0.2f);
            _towerMageText = new Text(_fntLoader, MageTower.StartCosts.ToString(), 
                width / 2 - 263 + (TOWER_BUTTON_XOFFSET * 2) - 5, height - 30, width, height, 
                new Vector3(1, 1, 1), 0.40f, 1.0f, 0.5f, 0.2f);
            _towerTeslaText = new Text(_fntLoader, TeslaTower.StartCosts.ToString(), 
                width / 2 - 263 + (TOWER_BUTTON_XOFFSET * 3) - 5, height - 30, width, height, 
                new Vector3(1, 1, 1), 0.40f, 1.0f, 0.5f, 0.2f);

            _textRender.Add(_towerArrowText);
            _textRender.Add(_towerCanonText);
            _textRender.Add(_towerMageText);
            _textRender.Add(_towerTeslaText);
        }


    }
}
