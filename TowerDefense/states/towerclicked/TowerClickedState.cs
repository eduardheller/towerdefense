using Engine.cgimin.camera;
using Engine.cgimin.sound;
using Engine.cgimin.texture;
using OpenTK;
using OpenTK.Input;
using TowerDefense.gui;
using TowerDefense.gui.font;
using TowerDefense.objects;

namespace TowerDefense.states.towerclicked
{
    /// <summary>
    /// GUI für die Information der Tower und Upgrademöglichkeiten
    /// </summary>
    class TowerClickedState : IGameState
    {
        private GUIBackground _towerBackground;
        private GUIButton _windowTower;
        private GUIButton _buttonUpgrade;
        private GUIButton _buttonSell;
        private GUIRenderer _guiRenderer;
        private Tower _tower;
        private PlayState _playState;

        private FontLoader _fntLoader;
        private TextRenderer _textRender;
        private Text _description;

        private Text _strength;
        private Text _speed;
        private Text _range;
        private Text _attack;
        private Text _upgrade;
        private Text _sell;

        private int _textAtlas;
        private bool _keyDown;
        private bool _keyDeleteDown;
        private Sound _buildSound;
        private TowerClickedGUIState _towerVisualState;

        public Tower Tower
        {
            get
            {
                return _tower;
            }

            set
            {
                _tower = value;
            }
        }

        public TowerClickedState(PlayState playState)
        {
            _playState = playState;
        }

        public override void Init()
        {
            base.Init();
            _playState.MainGuiState.HideButtons();
            _towerVisualState = new TowerClickedGUIState(Tower);
            GameManager.PushState(_towerVisualState);
            _keyDown = false;
            _fntLoader = new FontLoader("assets/gui/inc.fnt", 8);
            _textAtlas = TextureManager.LoadTexture("assets/gui/inc.png");
            _textRender = new TextRenderer(_textAtlas);

            _guiRenderer = new GUIRenderer();
  
    
            int width = GameManager.Window.Width;
            int height = GameManager.Window.Height;
            _buildSound = new Sound(ResourceManager.Sounds["BUILD"]);

            _windowTower = new GUIButton(width / 2 - 300, height - 170, 600, 173, width, height, 0.7f, 0);

            _buttonUpgrade = new GUIButton(width / 2 - 120, height - 165,124, 122, width, height, 0.9f, ResourceManager.GUI["BUTTON_UPGRADE"]);
            _buttonUpgrade.OverImage = ResourceManager.GUI["BUTTON_UPGRADE"];

            _buttonUpgrade.DisableImage = ResourceManager.GUI["BUTTON_UPGRADE_DISABLED"];


            _buttonSell = new GUIButton(width / 2 + 235, height - 70, 52, 55, width, height, 0.9f, ResourceManager.GUI["BUTTON_SELL"]);
            _buttonSell.OverImage = ResourceManager.GUI["BUTTON_SELL"];

            _towerBackground = new GUIBackground(width / 2 - 294, height - 165, 131, 162, width, height, 0.8f, ResourceManager.GUI["WINDOW_" + Tower.GetType().Name]);

            _guiRenderer.Add(_windowTower);
            _guiRenderer.Add(_buttonSell);
            _guiRenderer.Add(_towerBackground);
            _guiRenderer.Add(_buttonUpgrade);
    

            _description = new Text(_fntLoader, "Gold: ", 5, 24, width, height, new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _strength = new Text(_fntLoader, "Wave: ", 5, 59, width, height, new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _speed = new Text(_fntLoader, "Lives: ", 5, 92, width, height, new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _range = new Text(_fntLoader, "FPS: ", 5, 92, width, height, new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _attack = new Text(_fntLoader, "FPS: ", 5, 92, width, height, new Vector3(1, 1, 1), 0.30f, 1.0f, 0.5f, 0.3f);
            _upgrade = new Text(_fntLoader, "FPS: ", 5, 92, width, height, new Vector3(1, 1, 0.2f), 0.35f, 1.0f, 0.5f, 0.3f);
            _sell = new Text(_fntLoader, "FPS: ", 5, 92, width, height, new Vector3(1, 0.2f, 1f), 0.3f, 1.0f, 0.5f, 0.3f);

            _textRender.Add(_description);
            _textRender.Add(_strength);
            _textRender.Add(_speed);
            _textRender.Add(_range);
            _textRender.Add(_attack);
            _textRender.Add(_upgrade);
            _textRender.Add(_sell);

        }
        

        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);

            if(_tower != null)
            {

                if (Tower.GetUpgradeCost() > _playState.Player.Gold)
                {

                    _buttonUpgrade.Disable();
                }
                else
                {
                    _buttonUpgrade.Enable();
                }

                if (keyboard.GetState().IsKeyUp(Key.E) && Tower.GetUpgradeCost() <= _playState.Player.Gold && _keyDown)
                {
                    _keyDown = false;
                    _playState.Player.PayGold(Tower.GetUpgradeCost());
                    Tower.Upgrade();
                    _buildSound.SetPosition(Camera.Position);
                    _buildSound.Play();
                }

                if (keyboard.GetState().IsKeyUp(Key.Delete) && _keyDeleteDown)
                {
                    _keyDeleteDown = false;
                    _playState.Player.AddGold(Tower.GetSellCost());
                    _playState.RemoveTower(Tower);

                    for (int i = 0; i < _playState.MapContext.TowerSlots.Count; i++)
                    {
                        if (_playState.MapContext.TowerSlots[i].Tower == Tower)
                            _playState.MapContext.TowerSlots[i].Tower = null;
                    }

                    Tower = null;
                    _buildSound.SetPosition(Camera.Position);
                    _buildSound.Play();
                    _playState.MainGuiState.ShowButtons();
                    GameManager.RemoveState(_towerVisualState);
                    GameManager.RemoveGUIState(this);
                }

            }


            if (keyboard.GetState().IsKeyDown(Key.E))
            {
                _keyDown = true;
            }

            if (keyboard.GetState().IsKeyDown(Key.Delete))
            {
                _keyDeleteDown = true;
            }

            if (keyboard[Key.Escape])
            {
                GameManager.RemoveGUIState(this);
            }


            if (_buttonUpgrade.IsClicked)
            {
                _playState.Player.PayGold(Tower.GetUpgradeCost());
                Tower.Upgrade();
                _buildSound.SetPosition(Camera.Position);
                _buildSound.Play();
            }

            if (_buttonSell.IsClicked)
            {
                _keyDeleteDown = false;
                _playState.Player.AddGold(Tower.GetSellCost());
                _playState.RemoveTower(Tower);

                for (int i = 0; i < _playState.MapContext.TowerSlots.Count; i++)
                {
                    if (_playState.MapContext.TowerSlots[i].Tower == Tower)
                        _playState.MapContext.TowerSlots[i].Tower = null;
                }

                Tower = null;
                _buildSound.SetPosition(Camera.Position);
                _buildSound.Play();
                _playState.MainGuiState.ShowButtons();
                GameManager.RemoveState(_towerVisualState);
                GameManager.RemoveGUIState(this);
            }
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);


            _description.ChangeText(Tower.Description + " LVL:"+Tower.Level, GameManager.Window.Width / 2 + 60, GameManager.Window.Height - 155);
            _strength.ChangeText("Strength:"+Tower.Strength, GameManager.Window.Width / 2 + 60, GameManager.Window.Height - 125);
            _speed.ChangeText("Speed:" + Tower.Speed, GameManager.Window.Width / 2 + 60, GameManager.Window.Height - 95);
            _range.ChangeText("Range:" + Tower.Radius, GameManager.Window.Width / 2 + 60, GameManager.Window.Height - 65);
            _attack.ChangeText("Target:"+Tower.AttackDescription, GameManager.Window.Width / 2 + 60, GameManager.Window.Height - 35);
            _sell.ChangeText(Tower.GetSellCost() + "g", GameManager.Window.Width / 2 +242, GameManager.Window.Height - 25);
            _upgrade.ChangeText(Tower.GetUpgradeCost() + "g", GameManager.Window.Width / 2 - 80, GameManager.Window.Height - 35, 1.0f, new Vector3(1, 1, 0.2f));


            _guiRenderer.Update(e, GameManager.Window.Mouse.GetState().IsButtonDown(MouseButton.Left), GameManager.Window.Mouse.GetState().IsButtonUp(MouseButton.Left),
                GameManager.Window.Mouse.X, GameManager.Window.Mouse.Y);

        }

        public bool IsOverGUI()
        {
            if (_windowTower != null)
            {
                return _windowTower.IsOver;
            }
            return false;
        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);
            _guiRenderer.Render();
            _textRender.Render();

        }

        public override void OnResize(int screenWidth, int screenHeight)
        {
            base.OnResize(screenWidth, screenHeight);
            Init();
        }

        public override void Close()
        {
            base.Close();
            _playState.MainGuiState.ShowButtons();
            _tower = null;
            _windowTower = null;
            _textRender.UnLoad();
            if (_towerVisualState!=null)
            {
                GameManager.RemoveState(_towerVisualState);
            }
            
        }
    }
}
