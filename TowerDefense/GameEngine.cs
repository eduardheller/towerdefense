using System.Collections.Generic;
using OpenTK;
using Engine.cgimin.camera;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using Engine.cgimin.postprocessing;
using Engine.cgimin.sound;

namespace TowerDefense
{
    /// <summary>
    /// Das ist die Hauptklasse die States(Screens) anführt.
    /// Sie ist für jede State erreichbar.
    /// </summary>
    class GameEngine
    {

        private HashSet<IGameState> _states; // Manche States dürfen nur einmal vorkommen
        private List<IGameState> _tmpStates; // Für die Threadsicherheit
        private List<IGameState> _guiStates;
        private GameWindow _window;
        public static bool Bloom;

        /// <summary>
        /// Die Window Klasse von OpenTK, damit auf Fenstereigenschaften zugegriffen werden kann
        /// </summary>
        public GameWindow Window
        {
            get { return _window; }
            set { _window = value; }
        }


        /// <summary>
        /// Initialisiert Sound, Postprocessor und Camera. Lädt alle erforderlichen Assets in den Speicher
        /// </summary>
        /// <param name="window"></param>
        public GameEngine(GameWindow window)
        {
            Sound.Init();
            _window = window;
            _states = new HashSet<IGameState>();
            _tmpStates = new List<IGameState>();
            _guiStates = new List<IGameState>();
            ResourceManager.LoadResources();
            Camera.Init();
            Bloom = false;
            Sound.SetListener(Camera.Position,new Vector3(0,0,0));
            Postprocessing.Init(window.Width, window.Height);

        }

        public void ChangeGUIState(IGameState state)
        {
            foreach (IGameState st in _guiStates)
                st.Close();

            _guiStates.Clear();
            state.GameManager = this;
            state.Init();
            _guiStates.Add(state);
        }

        public void PushGUIState(IGameState state)
        {
            if (!_guiStates.Contains(state))
            {
                
                _guiStates.Add(state);
                _guiStates[_guiStates.Count - 1].GameManager = this;
                _guiStates[_guiStates.Count - 1].Init();
            }

        }

        public void RemoveGUIState(IGameState state)
        {
            if (_guiStates.Contains(state))
            {
                state.Close();
                _guiStates.Remove(state);
            }
        }

        public void ChangeState(IGameState state)
        {
            foreach (IGameState st in _states)
                st.Close();

            _states.Clear();
            state.GameManager = this;
            state.Init();
            _states.Add(state);
        }

        public void PushState(IGameState state)
        {
            int oldCount = _states.Count;
            _states.Add(state);
            // Check falls Hashmap vergrößert ist, d.h. ein neuer GameState wurde hinzugefügt ( hat den Equal vergleich bestanden )
            if (_states.Count > oldCount)
            {
                
                // Erst jetzt darf GameState initialisiert werden.
                state.GameManager = this;
                state.Init();
            }

        }

        public void RemoveState(IGameState state)
        {

            state.Close();
            _states.Remove(state);
            
        }

        public void Update(FrameEventArgs e)
        {

            _tmpStates.Clear();

            foreach (IGameState state in _states)
                _tmpStates.Add(state);

            while (_tmpStates.Count > 0)
            {
                IGameState state = _tmpStates[0];
                _tmpStates.RemoveAt(0);
                state.Update(e);
            }

            _tmpStates.Clear();

 
            foreach (IGameState state in _guiStates)
                _tmpStates.Add(state);

            while (_tmpStates.Count > 0)
            {
                IGameState state = _tmpStates[0];
                _tmpStates.RemoveAt(0);
                state.Update(e);
            }
            Sound.SetListener(Camera.Position, new Vector3(0, 0, 0));
            Sound.SetListenerOrientation(Camera.GetLookAt(), new Vector3(0, 1, 0));
        }

        public void Render(FrameEventArgs e)
        {
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _tmpStates.Clear();
   

            foreach (IGameState state in _states)
                _tmpStates.Add(state);

            while (_tmpStates.Count > 0)
            {
                IGameState state = _tmpStates[0];
                _tmpStates.RemoveAt(0);
                state.Render(e);
            }

            if (Bloom) Postprocessing.ApplyAndDraw();

            _tmpStates.Clear();

            foreach (IGameState state in _guiStates)
                _tmpStates.Add(state);

            
            while (_tmpStates.Count > 0)
            {
                IGameState state = _tmpStates[0];
                _tmpStates.RemoveAt(0);
                state.Render(e);
   
            }
        }

        public void HandleInput(FrameEventArgs e)
        {

            _tmpStates.Clear();
            foreach (IGameState screen in _states)
                _tmpStates.Add(screen);

            foreach (IGameState screen in _tmpStates)
                screen.HandleInput(e, Window.Mouse, Window.Keyboard);


            _tmpStates.Clear();
            foreach (IGameState screen in _guiStates)
                _tmpStates.Add(screen);

            foreach (IGameState screen in _tmpStates)
                screen.HandleInput(e, Window.Mouse, Window.Keyboard);
        }

        public void OnResize(int width, int height)
        {
            Postprocessing.OnResize(width, height);
            _tmpStates.Clear();
            foreach (IGameState screen in _states)
                _tmpStates.Add(screen);

            foreach (IGameState screen in _tmpStates)
                screen.OnResize(width, height);

            _tmpStates.Clear();
            foreach (IGameState screen in _guiStates)
                _tmpStates.Add(screen);

            foreach (IGameState screen in _tmpStates)
                screen.OnResize(width, height);

        }


    }
}
