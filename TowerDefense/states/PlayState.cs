using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using TowerDefense.objects;
using System.Diagnostics;
using TowerDefense.map;
using Engine.cgimin.camera;
using TowerDefense.gui;
using OpenTK.Input;
using Engine.cgimin.sound;
using TowerDefense.collision;
using TowerDefense.states.visual;
using TowerDefense.states.towerclicked;
using TowerDefense.states.wavepause;
using TowerDefense.states.end;

namespace TowerDefense.states
{
    /// <summary>
    /// HauptState, welches immer im Spiel aktiv ist. 
    /// Ist für die Gegner und Türmen zuständig. Behandelt auch Kamerafahrten und Aktionen
    /// </summary>
    public class PlayState : SceneRenderState
    {
        private const float SCREEN_EDGE_MOUSE_MOVE = 0.4f;
        private const float ENEMY_SPAWN_RATE = 1; // In Sekunden
        private Stopwatch _stopWatch;
        private int _lastSecond;
        private List<Enemy> _enemies;
        private List<List<MapContext.Wave>> _waves;
        private List<Tower> _towers;
        private Vector2 _lastMousePosition;
        private const float fov = 60;
        private int _currentWaveIndex;
        private int _currentEnemyIndex;
        private int _currentEnemyCount;
        private bool _wavePause;
        private bool _waitForAllKill;
        private HealthBar _healthBar;
        private Player _player;
        private Tower _currTowerOver;
        private PlayStateGUI _mainGuiState;
        private TowerClickedState _towerClickedState;
        private Sound _music;
        private Sound _waveover;
        private Sound _ambient;
        private Sound _lifeLost;
        private Sound _bossMusic;
        private ShipWaveState _shipState;
        private bool _inputActive;
        private TowerRayCollision _towerRayCollision;

        public PlayState(string map) : base(map)
        {
            _player = new Player(100, 0, _currentWaveIndex, MapContext.Waves.Count, 15);
            _currTowerOver = null;
            _enemies = new List<Enemy>();
            _waves = MapContext.Waves;
            _towers = new List<Tower>();
            _stopWatch = new Stopwatch();
            _towerRayCollision = new TowerRayCollision();
            _lastMousePosition = Vector2.Zero;
            _stopWatch.Start();
            _currentWaveIndex = 0;
            _currentEnemyIndex = 0;
            _currentEnemyCount = 0;
            _wavePause = true;
            _waitForAllKill = false;
            _healthBar = new HealthBar(0.8f);
            InputActive = true;
            Music = new Sound(ResourceManager.Sounds["MUSIC_BATTLE"], true);
            _waveover = new Sound(ResourceManager.Sounds["WAVE_OVER"]);
            _ambient = new Sound(ResourceManager.Sounds["AMBIENT"], true);
            _lifeLost = new Sound(ResourceManager.Sounds["LIFE_LOST"]);
            _bossMusic = new Sound(ResourceManager.Sounds["BOSS_MUSIC"], true);
            _ambient.Play();
            _ambient.Gain = 0.7f;
            Music.Gain = 0.7f;

        }

        public override void Init()
        {
            base.Init();
            MainGuiState = new PlayStateGUI(this);
            _towerClickedState = new TowerClickedState(this);
            GameManager.ChangeGUIState(MainGuiState);
            GameManager.PushGUIState(new GUIWavePauseState(this));
        }

        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);

            // WheelDelta veraltet, aber zuverlässig
            #pragma warning disable 618
            int deltam = mouse.WheelDelta;
            #pragma warning restore 618
            if (InputActive)
            {
                if (keyboard[Key.W]) Camera.Move((float)e.Time * 25.0f, 0.0f, 0.1f, 0.0f);
                if (keyboard[Key.S]) Camera.Move((float)e.Time * 25.0f, 0.0f, -0.1f, 0.0f);
                if (keyboard[Key.A]) Camera.Move((float)e.Time * 25.0f, -0.1f, 0.0f, 0.0f);
                if (keyboard[Key.D]) Camera.Move((float)e.Time * 25.0f, 0.1f, 0.0f, 0.0f);

                if (keyboard[Key.Up]) Camera.Move((float)e.Time * 25.0f, 0.0f, 0.1f, 0.0f);
                if (keyboard[Key.Down]) Camera.Move((float)e.Time * 25.0f, 0.0f, -0.1f, 0.0f);
                if (keyboard[Key.Left]) Camera.Move((float)e.Time * 25.0f, -0.1f, 0.0f, 0.0f);
                if (keyboard[Key.Right]) Camera.Move((float)e.Time * 25.0f, 0.1f, 0.0f, 0.0f);

                int mousex = mouse.X;
                int mousey = mouse.Y;

                if(mousex< SCREEN_EDGE_MOUSE_MOVE) Camera.Move((float)e.Time * 25.0f, -0.1f, 0.0f, 0.0f);
                if(mousex>GameManager.Window.Width- SCREEN_EDGE_MOUSE_MOVE) Camera.Move((float)e.Time * 25.0f, 0.1f, 0.0f, 0.0f);
                if (mousey < SCREEN_EDGE_MOUSE_MOVE) Camera.Move((float)e.Time * 25.0f, 0.0f, 0.1f, 0.0f);
                if (mousey > GameManager.Window.Height - SCREEN_EDGE_MOUSE_MOVE) Camera.Move((float)e.Time * 25.0f, 0.0f, -0.1f, 0.0f);

                Camera.MoveForward(deltam * 1f);

                bool rightClicked = Mouse.GetState().IsButtonDown(MouseButton.Right);
                Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);


                if (GameManager.Window.Focused)
                {
                    if (rightClicked)
                    {
                        Vector2 delta = _lastMousePosition - mousePosition;
                        Camera.AddRotation(0.001f, delta.X, delta.Y);
                        Mouse.SetPosition(GameManager.Window.Bounds.Left + GameManager.Window.Bounds.Width / 2,
                            GameManager.Window.Bounds.Top + GameManager.Window.Bounds.Height / 2);

                        mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    }

                    _lastMousePosition = mousePosition;
                }

                // Falls über eine GUI eine Kollision stattfindet
                bool overGUI = _towerClickedState.IsOverGUI() || _mainGuiState.IsOverGUI;

                // Wenn über eine GUI dann keine Tower anvisieren
                if (_currTowerOver != null && overGUI) _currTowerOver.IsMouseOver = false; 

                _currTowerOver = _towerRayCollision.Handle(overGUI,Mouse.GetState().IsButtonDown(MouseButton.Left), Mouse.GetState().IsButtonUp(MouseButton.Left),
                    _towers, GameManager.Window.Mouse.X, GameManager.Window.Mouse.Y, GameManager.Window.Width, GameManager.Window.Height);


                if (_currTowerOver != null)
                {
                    // Wenn der neue Tower nicht dem alten entspricht soll nur eine State aufgerufen werden
                    // Da sonst mehrmals demselben Tower eine State zugesprochen werden kann
                    if (_towerRayCollision.IsClicked && _towerClickedState.Tower != _currTowerOver)
                    {

                        GameManager.RemoveGUIState(_towerClickedState);
                        _towerClickedState.Tower = _currTowerOver;
                        GameManager.PushGUIState(_towerClickedState);
                    }
                }
                // Falls auf die Map geklickt wird und dabei nicht auf der GUI, soll die Towerstate ausgeschaltet werden
                else if(_currTowerOver == null && Mouse.GetState().IsButtonDown(MouseButton.Left) &&!overGUI)
                {
                    GameManager.RemoveGUIState(_towerClickedState);
                }

            }
          
        }

        public override void UpdateEntities(FrameEventArgs e)
        {

            foreach (Enemy enemy in _enemies.ToList())
            {
                if (enemy.Walk(e, MapContext.WayMarks, this))
                {
                    _player.RemoveLife();
                    if (_player.IsDead() && !_player.GameOver)
                    {
                        _player.GameOver = true;
                        GameManager.RemoveGUIState(MainGuiState);
                        GameManager.PushGUIState(new PreviewEndGUIState(this, false));
                        GameManager.PushState(new PreviewEndState(this, false));
                        Music.Stop();
                        _bossMusic.Stop();
                    }
                    _lifeLost.SetPosition(Camera.Position);
                    _lifeLost.Play();
                }
   
                if (!enemy.IsAlive)
                {
                    _player.AddGold(enemy.Gold);
                    _enemies.Remove(enemy);
                }

                if (!enemy.IsInDeadSequence)
                {
                    foreach (Tower tower in _towers)
                    {
                        tower.AddTargets(e, enemy);
                    }
                }

            }

            TimeSpan elapsed = _stopWatch.Elapsed;
            int sec = (int)elapsed.TotalSeconds;
            // Timer für die Gegner
            if (sec - _lastSecond >= ENEMY_SPAWN_RATE)
            {
                SpawnEnemy();
                _lastSecond = sec;
            }

            foreach (Tower tower in _towers)
            {
                tower.Update(e);
                tower.HandleProjectiles(e, _enemies);
                tower.ClearTargets();
            }

            Music.SetPosition(Camera.position);

            // Ambient sound (ozean) am Boden positionieren
            _ambient.SetPosition(new Vector3(Camera.Position.X, 0, Camera.Position.Z));

            base.UpdateEntities(e);
        }

        public override void RenderEntities(FrameEventArgs e)
        {
            base.RenderEntities(e);
            foreach (Enemy enemy in _enemies)
            {
                enemy.Render(e);
            }

            foreach (Tower tower in _towers)
            {
                tower.Render(e);
            }
        }

        public override void RenderHUD(FrameEventArgs e)
        {
            base.RenderHUD(e);

            // Healthbar soll immer zu sehen sein und muss reversed gerendert werden
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                _healthBar.Update(_enemies[i]);
                _healthBar.Render();
            }
        }

        public void AddTower(Tower tower)
        {
            if (tower != null) _towers.Add(tower);
        }

        private void SpawnEnemy()
        {

            // Falls Welle aktiv ist
            if (!_wavePause && !_waitForAllKill)
            {
                Type id = _waves[_currentWaveIndex][_currentEnemyIndex].enemyId;
                int enemycount = _waves[_currentWaveIndex][_currentEnemyIndex].count;

                // Erstellt einen Gegner nach der ID aus der Mapdatei
                Enemy enemy = (Enemy)Activator.CreateInstance(id, new object[] { MapContext.StartPosition, _currentWaveIndex + 1});

                if (id.Name == "Boss1")
                {
                    _music.Stop();
                    _bossMusic.Play();
                }

                _currentEnemyCount++;
                _enemies.Add(enemy);

                if (enemycount <= _currentEnemyCount)
                {
                    _currentEnemyCount = 0;
                    _currentEnemyIndex++;
                    int enemywaves = _waves[_currentWaveIndex].Count;

                    if (enemywaves <= _currentEnemyIndex)
                    {
                        _currentEnemyIndex = 0;
                        _currentWaveIndex++;
                        _waitForAllKill = true;

                    }
                }
            }

            // Falls alle Gegner aus dem Schiff rausgegangen sind
            if (_waitForAllKill)
            {
                _shipState.Dissapear();
            }

            // Falls alle Gegner tot sind
            if (_enemies.Count == 0 && _waitForAllKill && !_player.IsDead())
            {
                _waitForAllKill = false;
                _wavePause = true;
                _player.CurrentWave++;
                
                // Falls letzte Welle geschlagen wurde
                if (_waves.Count <= _currentWaveIndex)
                {
                    GameManager.RemoveGUIState(MainGuiState);
                    GameManager.PushGUIState(new PreviewEndGUIState(this,true));
                    GameManager.PushState(new PreviewEndState(this,true));
                    GameManager.RemoveState(_shipState);
                    Music.Stop();
                    _bossMusic.Stop();
                }
                // Falls derzeitige Welle geschlagen wurde
                else
                {
                    GameManager.PushGUIState(new GUIWavePauseState(this));
                    GameManager.RemoveState(_shipState);
                    Music.Stop();
                    _bossMusic.Stop();
                }
            }

        }

        public override void OnResize(int screenWidth, int screenHeight)
        {
            base.OnResize(screenWidth, screenHeight);
            Camera.SetWidthHeightFov(screenWidth, screenHeight, Camera.Fov);
        }

 

        public bool WavePause
        {
            get { return _wavePause; }
            set { _wavePause = value; }
        }

        internal Player Player
        {
            get
            {
                return _player;
            }

            set
            {
                _player = value;
            }
        }

        public MapContext MapContext
        {
            get
            {
                return _mapContext;
            }

            set
            {
                _mapContext = value;
            }
        }

        public PlayStateGUI MainGuiState
        {
            get
            {
                return _mainGuiState;
            }

            set
            {
                _mainGuiState = value;
            }
        }

        public Sound Music
        {
            get
            {
                return _music;
            }

            set
            {
                _music = value;
            }
        }



        public void AssignShipState(ShipWaveState state)
        {
            _shipState = state;
        }

        public void RemoveTower(Tower tower)
        {
            _towers.Remove(tower);
        }


        public bool InputActive
        {
            get
            {
                return _inputActive;
            }

            set
            {
                _inputActive = value;
            }
        }

        public override void Close()
        {
            base.Close();
            _lifeLost.UnLoad();
            _ambient.UnLoad();
            _music.UnLoad();
            _bossMusic.UnLoad();
        }
    }
}
