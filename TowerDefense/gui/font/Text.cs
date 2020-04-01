using Engine.cgimin.object3d;
using System;
using System.Collections.Generic;
using OpenTK;

namespace TowerDefense.gui.font
{
    class Text 
    {
        FontLoader _fnt;
        string _text;
        float _xcursor;
        float _ycursor;
        float _alpha;
        float _width;
        float _edge;
        float _size;
        private int _swidth;
        private int _sheight;
        BaseObject3D _object;
        Vector3 _color;
        private bool _isVisible;

        public float Alpha
        {
            get
            {
                return _alpha;
            }

            set
            {
                _alpha = value;
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

        public float Edge
        {
            get
            {
                return _edge;
            }

            set
            {
                _edge = value;
            }
        }

        public Vector3 Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
            }
        }

        public float Size
        {
            get
            {
                return _size;
            }

            set
            {
                _size = value;
            }
        }

        public BaseObject3D Object
        {
            get
            {
                return _object;
            }

            set
            {
                _object = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                _isVisible = value;
            }
        }

        public Text(FontLoader fnt, string text, float x, float y, int swidth, int sheight, Vector3 color, float size, float alpha, float width, float edge)
        {
            _xcursor = 2.0f * (float)x / (float)swidth - 1;
            _ycursor = -(2.0f * (float)y / (float)sheight - 1);
            _swidth = swidth;
            _sheight = sheight;
            _fnt = fnt;
            _text = text;
            _alpha = alpha;
            _width = width;
            _edge = edge;
            Color = color;
            IsVisible = true;
            float sizedivide = (_swidth + _sheight) / 2000f;
            _object = new BaseObject3D();
            _size = size/ sizedivide;
            foreach (var c in text)
            {
                DrawChar(_fnt.Characters[c], _xcursor, _ycursor);
            }
            

            _object.CreateVAO();
        }

        public void ChangeText(string text, float x, float y, float alpha = 1.0f, Vector3 color = default(Vector3))
        {
            if(color != default(Vector3))
            {
                Color = color;
            }

            _alpha = alpha;
            _xcursor = 2.0f * (float)x / (float)_swidth - 1;
            _ycursor = -(2.0f * (float)y / (float)_sheight - 1);
            _object.UnLoad();
            _object = new BaseObject3D();
            foreach (var c in text)
            {
                DrawChar(_fnt.Characters[c], _xcursor, _ycursor);
            }

            _object.CreateVAO();
        }

        private void DrawChar(FontCharacter c, float xcurs, float ycurs)
        {
            float perPixelSize = (float)_sheight / (float)_swidth;

            float xrel = xcurs + c.Xoffset * perPixelSize * _size;
            float yrel = ycurs - c.Yoffset * _size;

            float dx = xrel + c.Width * perPixelSize * _size;
            float dy = yrel - c.Height * _size;

            float ux = c.X;
            float udx = c.X + c.Width;
            float uy = c.Y;
            float udy = c.Y + c.Height;

            // Triangles für Back- und Frontface
            _object.addTriangle(new Vector3(dx, dy, 0), new Vector3(dx, yrel, 0), new Vector3(xrel, dy, 0), new Vector2(udx, udy), new Vector2(udx, uy), new Vector2(ux, udy));
            _object.addTriangle(new Vector3(xrel, dy, 0), new Vector3(dx, yrel, 0), new Vector3(xrel, yrel, 0), new Vector2(ux, udy), new Vector2(udx, uy), new Vector2(ux, uy));
            
            _xcursor += c.Cursorwidth * perPixelSize * _size;

        }
    }
}
