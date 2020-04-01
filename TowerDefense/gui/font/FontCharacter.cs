using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense.gui.font
{
    class FontCharacter
    {
        private char _character;
        private float _x;
        private float _y;
        private float _width;
        private float _height;
        private float _quadHeight;
        private float _quadWidth;
        private float _xoffset;
        private float _yoffset;
        private float _cursorwidth;

        public char Character
        {
            get
            {
                return _character;
            }

            set
            {
                _character = value;
            }
        }

        public float X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }
        }

        public float Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }
        }

        public float Width
        {
            get
            {
                return _width;
            }

            set
            {
                _width = value;
            }
        }

        public float Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
            }
        }

        public float Xoffset
        {
            get
            {
                return _xoffset;
            }

            set
            {
                _xoffset = value;
            }
        }

        public float Yoffset
        {
            get
            {
                return _yoffset;
            }

            set
            {
                _yoffset = value;
            }
        }

        public float Cursorwidth
        {
            get
            {
                return _cursorwidth;
            }

            set
            {
                _cursorwidth = value;
            }
        }

        public float QuadHeight
        {
            get
            {
                return _quadHeight;
            }

            set
            {
                _quadHeight = value;
            }
        }

        public float QuadWidth
        {
            get
            {
                return _quadWidth;
            }

            set
            {
                _quadWidth = value;
            }
        }

        public FontCharacter(char c, float x, float y, float w, float h, float quadh, float quadw, float xoff, float yoff, float cursorwidth)
        {
            _character = c;
            _x = x;
            _y = y;
            _width = w;
            _height = h;
            _quadWidth = quadw;
            _quadHeight = quadh;
            _xoffset = xoff;
            _yoffset = yoff;
            _cursorwidth = cursorwidth;
        }
       
    }
}
