using Engine.cgimin.camera;
using Engine.cgimin.helpers;
using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;

namespace TowerDefense.states.end
{
    /// <summary>
    /// Kameraführung am Ende des Spiels / (Gewinnen/Verlieren)
    /// </summary>
    class PreviewEndState : IGameState
    {
        private const int hermitesteps = 16;
        private const float _stepTime = 0.1f;

        private List<Vector3> _preDefinedPositions;
        private List<Vector3> _preDefinedOrientations;
        private List<Vector3> _interpCameraPositions;
        private List<Vector3> _interpCameraOrientations;

        private float _timer;
        private int _currentPosition;

        public PreviewEndState(PlayState parentState, bool won)
        {
            parentState.InputActive = false;

            _interpCameraPositions = new List<Vector3>();
            _interpCameraOrientations = new List<Vector3>();

            CalculateHermitePointsForCamera(won);
            Camera.LerpOrientation = true;
            
        }

        public override void Init()
        {
            base.Init();
        }

        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);
            Camera.SetOrientation(_interpCameraOrientations[_currentPosition]);
            Camera.LerpToPosition(_interpCameraPositions[_currentPosition]);
            _timer += (float)e.Time;
          
            if (_timer > _stepTime)
            {
                _timer = 0;
                if (++_currentPosition > _interpCameraPositions.Count - 1)
                {
                    _currentPosition--; // So bleibt die Kamera immer an dem letzten Punkt
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

        private void CalculateHermitePointsForCamera(bool won)
        {
            Vector3 endPos, endOrientation;

            // Je nachdem, ob gewonnen oder verloren wurde, werden vordefinierte Position zugewiesen
            if (won)
            {
                endPos = new Vector3(13.6f, 15.3f, 26.92f);
                endOrientation = new Vector3(3.14f, -0.89f, 0);
            }
            else
            {
                endPos = new Vector3(24.61f, 9.9f, 35f);
                endOrientation = new Vector3(2.68f, -0.86f, 0);
            }

            _preDefinedOrientations = new List<Vector3>()
            {
                new Vector3(Camera.orientation.X,Camera.orientation.Y,0),
                endOrientation,
            };

            _preDefinedPositions = new List<Vector3>()
            {
                new Vector3(Camera.position.X,Camera.position.Y,Camera.Position.Z ),
                endPos,
            };

            // Kalkuliert die Interpolationspunkte aus der Hermite-Interpolation
            for (int i = 0; i < _preDefinedPositions.Count - 1; i += 1)
            {
                for (float t = 0; t <= hermitesteps; t++)
                {
                    float s = (float)t / (float)hermitesteps;
                    Vector3 pos = Lerps.GetInterpPosition(s, _preDefinedPositions[i], _preDefinedPositions[i + 1],
                        Vector3.UnitX, Vector3.UnitZ);
                    Vector3 orient = Lerps.GetInterpPosition(s, _preDefinedOrientations[i], _preDefinedOrientations[i + 1],
                        Vector3.UnitX, Vector3.UnitZ);

                    _interpCameraPositions.Add(pos);
                    _interpCameraOrientations.Add(orient);
                }

            }
        }

    }
}
