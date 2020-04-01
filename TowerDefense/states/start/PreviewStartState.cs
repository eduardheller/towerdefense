using Engine.cgimin.camera;
using Engine.cgimin.sound;
using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using Engine.cgimin.helpers;

namespace TowerDefense.states.start
{
    /// <summary>
    /// Kameraführung am Anfang des Spiels / (Einführung)
    /// </summary>
    class PreviewStartState : IGameState
    {
        private const float _seqStepTime = 3.0f;
        private const float _stepTime = 0.1f;
        private const int hermitesteps = 16;

        private List<Vector3> points;
        private List<Vector3> cameraorientationpoints;
        private List<Vector3> camerapositions;
        private List<Vector3> cameraorientation;
        private float _timer;
        private float _seqTimer;
        private float _musicTimer;
        private int _currentIndexPosition;
        private bool nextStage;
        private PreviewStartGUIState _guiState;
        private Sound _ambient;
        private Sound _intro;
        private PlayState _playState;


        public PreviewStartState(PlayState parentState)
        {
            parentState.InputActive = false;
            camerapositions = new List<Vector3>();
            cameraorientation = new List<Vector3>();
            _musicTimer = 1.0f;
            _playState = parentState;
            CalculateHermitePointsForCamera();

            Camera.SetToPosition(new Vector3(-27, 36, 89));
            Camera.SetOrientation(new Vector3(2.4f, -0.52f, 0));
            Camera.LerpOrientation = true;

            // Pusht die GUI in die Engine (für die Texteinblendung in dem Preview)
            _guiState = new PreviewStartGUIState(this);
        }

        public override void Init()
        {
            base.Init();
            _intro = new Sound(ResourceManager.Sounds["INTRO"], true);
            _ambient = new Sound(ResourceManager.Sounds["AMBIENT"], true);
            _intro.Play();
            GameManager.ChangeGUIState(_guiState);
        }

        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);

            if (keyboard[Key.Escape])
            {
                GameManager.RemoveState(this);
                GameManager.RemoveGUIState(_guiState);
                Camera.SetToPosition(new Vector3(13.6f, 15.3f, 26.92f));
                Camera.SetOrientation(new Vector3(3.14f, -0.89f, 0));
            }
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            Camera.SetOrientation(cameraorientation[_currentIndexPosition]);
            Camera.LerpToPosition(camerapositions[_currentIndexPosition]);
            _timer += (float)e.Time;
            _ambient.SetPosition(new Vector3(Camera.Position.X,0,Camera.Position.Y));
            _intro.SetPosition(Camera.Position);

            // Setze die Musiklautstärke dem Timer
            _intro.Gain = _musicTimer;

            // Timer um die Musiklautstärke am Ende der ganzen Sequenz leiser zu drehen
            if (IsLastStep())
            {
                _musicTimer -= (float)e.Time * 0.4f;
            }

            if (_timer> _stepTime)
            {
                if (IsAtNextStep())
                {
                    _seqTimer += (float)e.Time;
                    if (_seqTimer > _seqStepTime)
                    {
                        nextStage = true;
                        _timer = 0;
                        _seqTimer = 0;
                        ++_currentIndexPosition;
                    }
                }
                else
                {
                    _timer = 0;
                    // Falls die letzte Sequenzposition erreicht wurde
                    if (++_currentIndexPosition > camerapositions.Count - 1)
                    {
                        GameManager.RemoveState(this);
                        GameManager.RemoveGUIState(_guiState);
                    }
                }
            }
        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);
        }

        public override void OnResize(int screenWidth, int screenHeight)
        {
            base.OnResize(screenWidth, screenHeight);

        }

        public bool NextStage
        {
            get
            {
                bool stage = nextStage;
                nextStage = false;
                return stage;
            }
        }

        private bool IsLastStep()
        {
            return ((_currentIndexPosition / hermitesteps) == cameraorientationpoints.Count - 1);
        }

        private bool IsAtNextStep()
        {
            return _currentIndexPosition % hermitesteps == 0;
        }

        public override void Close()
        {
            base.Close();
            _ambient.UnLoad();
            _intro.UnLoad();
            _playState.Init();
            _playState.InputActive = true;
            Camera.LerpOrientation = false;
            Camera.SetToPosition(Camera.position);
            
        }

        private void CalculateHermitePointsForCamera()
        {
            cameraorientationpoints = new List<Vector3>()
            {
                new Vector3(2.4f,-0.52f,0),
                new Vector3(2.68f,-0.86f,0),
                new Vector3(3.13f,-0.85f,0),
                new Vector3(3.14f,-0.89f,0),

            };

            points = new List<Vector3>()
            {
                new Vector3(-27,36,89 ),
                new Vector3(24.61f,9.9f,35f),
                new Vector3(13.45f,8.7f,7.3f),
                new Vector3(13.6f,15.3f,26.92f),
            };

            // Kalkuliert die Interpolationspunkte aus der Hermite-Interpolation
            for (int i = 0; i < points.Count - 1; i += 1)
            {
                for (float t = 0; t <= hermitesteps; t++)
                {

                    float s = (float)t / (float)hermitesteps;
                    Vector3 pos = Lerps.GetInterpPosition(s, points[i], points[i + 1], Vector3.UnitX, Vector3.UnitZ);
                    Vector3 pos2 = Lerps.GetInterpPosition(s, cameraorientationpoints[i], cameraorientationpoints[i + 1], Vector3.UnitX, Vector3.UnitZ);
                    camerapositions.Add(pos);
                    cameraorientation.Add(pos2);
                }

            }
        }

    }
}
