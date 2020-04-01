using Engine.cgimin.object3d;
using OpenTK;

namespace TowerDefense.gui
{
    public abstract class GUIElement
    {
        private BaseObject3D guiQuad;

        protected int xpos;
        protected int ypos;
        protected int width;
        protected int height;
        protected float alpha;
        protected int texture;
        private bool visible;
        private bool allowHandle;
        protected bool isDisabled;

        public BaseObject3D Object
        {
            get { return guiQuad; }
        }

        public float Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        public int Texture
        {
            get { return texture; }
            set { texture = value; } 
        }

        public virtual bool IsVisible
        {
            get { return visible; }
            set { visible = value; }
        }

        public bool AllowHandle
        {
            get { return allowHandle; }
            set { allowHandle = value; }
        }

        public bool IsDisabled()
        {
            return isDisabled; 
        }

        public virtual void Disable()
        {
            isDisabled = true;
        }

        public virtual void Enable()
        {
            isDisabled = false;
        }

        public GUIElement(int x, int y, int pwidth, int pheight, int screenWidth, int screenHeight, float alpha, int textureid)
        {

            Alpha = alpha;
            Texture = textureid;

            xpos = x;
            ypos = y;
            width = pwidth;
            height = pheight;

            pwidth += x;
            pheight += y;

            visible = true;

            // Transformiert Screenkoordinaten in NDC
            float xn = 2.0f * (float)x / (float)screenWidth - 1;
            float yn = -(2.0f * (float)y / (float)screenHeight - 1);

            float wn = (2.0f * (float)pwidth / (float)screenWidth - 1);
            float hn = -(2.0f * (float)pheight / (float)screenHeight - 1);

            wn = wn - xn;
            hn = -(yn - hn);

            guiQuad = new BaseObject3D();
            guiQuad.addTriangle(
                new Vector3(xn + wn, yn + hn, 0), 
                new Vector3(xn + wn, yn, 0), new Vector3(xn, yn + hn, 0), 
                new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1));

            guiQuad.addTriangle(
                new Vector3(xn, yn + hn, 0), 
                new Vector3(xn + wn, yn, 0), new Vector3(xn, yn, 0), 
                new Vector2(0,1), new Vector2(1, 0), new Vector2(0, 0));

            guiQuad.CreateVAO();
        }

        public abstract void HandleEvents(FrameEventArgs e, bool down, bool up, int mousex, int mousey);
    }
}
