using OpenTK;
using OpenTK.Input;

namespace TowerDefense
{
    /// <summary>
    /// Abstrakte GameState Klasse die verschiedene Methoden anbietet, die von der GameEngine ausgeführt wird.
    /// </summary>
    public abstract class IGameState
    {
        GameEngine _gameManager;

        internal GameEngine GameManager
        {
            get { return _gameManager; }
            set { _gameManager = value; }
        }

        public virtual void Init() {  }
        public virtual void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard) { }
        public virtual void Update(FrameEventArgs e) { }
        public virtual void Render(FrameEventArgs e) { }
        public virtual void OnResize(int screenWidth, int screenHeight) { }
        public virtual void Close() { }

    }
}
