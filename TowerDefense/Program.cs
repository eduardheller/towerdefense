using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TowerDefense.states;
using TowerDefense.states.menu;

namespace TowerDefense
{

    public class TowerDefense : GameWindow
    {
        private const int ScreenWidth = 1280;
        private const int ScreenHeigth = 720;
        private GameEngine _gameEngine;

        public TowerDefense()
            : base(ScreenWidth, ScreenHeigth, new GraphicsMode(32, 24, 8, 2), "Battle of the Thrones", GameWindowFlags.Default, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        { }


        protected override void OnLoad(EventArgs e)
        {
    
            WindowState = WindowState.Fullscreen;
            VSync = VSyncMode.Off;
            base.OnLoad(e);

            _gameEngine = new GameEngine(this);
            _gameEngine.ChangeState(new SceneRenderState("map/Map001.txt"));
            _gameEngine.ChangeGUIState(new MainMenuState());
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if(Keyboard[Key.AltLeft] && Keyboard[Key.F4])
            {
                Exit();
            }

            _gameEngine.HandleInput(e);
            _gameEngine.Update(e);
            
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _gameEngine.Render(e);
            SwapBuffers();
        }

        protected override void OnFocusedChanged(EventArgs e)
        {
            base.OnFocusedChanged(e);

        }


        protected override void OnUnload(EventArgs e)
        {
           
        }


        protected override void OnResize(EventArgs e)
        {
            _gameEngine.OnResize(Width, Height);
            GL.Viewport(0, 0, Width, Height);
        }



        [STAThread]
        public static void Main()
        {
            using (TowerDefense game = new TowerDefense())
            {
                game.Run(0);
            }
        }


    }
    
}
