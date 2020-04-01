using OpenTK;
using TowerDefense.gui.font;

namespace TowerDefense.gui
{
    class GUIToolTip : GUIElement
    {
        private GUIButton _button;
        private float _timer;
        private const float _maxTime = 0.5f;
        int x;
        int y;
        private int _width;
        private int _height;
        private int _pheight;
        private Text _text;
        private string _strText;

        public GUIToolTip(GUIButton button, Text text, string strText, int pwidth, int pheight, int screenWidth, int screenHeight, float alpha, int textureid) : 
            base(0, 0, pwidth, pheight, screenWidth, screenHeight, alpha, textureid)
        {
            _text = text;
            _button = button;
            _strText = strText;
            AllowHandle = true;
            _width = screenWidth;
            _height = screenHeight;
            _pheight = pheight;
        }

        public override void HandleEvents(FrameEventArgs e, bool down, bool up, int mousex, int mousey)
        {
            bool wasDisabled = false;


            if (_button.IsDisabled())
            {
                _button.Enable();
                _button.HandleEvents(e, false, false, mousex, mousey);
                wasDisabled = true;
            }

            if (_button.IsOver && _button.IsVisible && !IsVisible)
            {
                _timer += (float)e.Time;
                if (_timer > _maxTime)
                {
                    IsVisible = true;
                    _text.IsVisible = true;
                    x = mousex;
                    y = mousey;
                }
            }
            else if (!_button.IsOver)
            {
                _timer = 0;
                IsVisible = false;
                _text.IsVisible = false;
            }

            if (IsVisible)
            {
                float xn = 2 * ((float)x / (float)_width);
                float yn = -(2.0f * ((float)(y - _pheight) / (float)_height));
   
                _text.ChangeText(_strText, x + 19, y - _pheight + 9, 1.0f);
                Object.Transformation = Matrix4.CreateTranslation(new Vector3(xn, yn, 0));
            }

            if (wasDisabled) _button.Disable();
          
        }

    }
}
