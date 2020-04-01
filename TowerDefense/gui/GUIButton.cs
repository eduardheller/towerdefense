using OpenTK;


namespace TowerDefense.gui
{
    public class GUIButton : GUIElement
    {

        private int overImage;
        private int disableImage;
        private int tmpImage;
        private bool isOver;
        private bool isClicked;
        private bool isPressedAndOver;
        private bool isReadyToPress;

        public int OverImage
        {
            set { overImage = value; }
            get { return overImage; }
        }

        public int DisableImage
        {
            set {
                disableImage = value;
            }
            get { return disableImage; }
        }


        public bool IsClicked
        {
            get {

                if (!IsVisible) return false;
                return isClicked;

            }
        }

        public bool IsOver
        {
            get { return isOver; }
        }

        public override void Disable()
        {
            isDisabled = true;
            Texture = disableImage;
        }

        public override bool IsVisible
        {
            get
            {
                return base.IsVisible;
            }

            set
            {
                base.IsVisible = value;
                if (base.IsVisible == false) isOver = false;
            }
        }

        public override void Enable()
        {
            isDisabled = false;
            if(isOver) Texture = overImage;
            else Texture = tmpImage;
        }

        public GUIButton(int x, int y, int width, int height, int swidth, int sheight, float alpha, int textureid) :
            base(x,y, width, height, swidth, sheight, alpha, textureid)
        {
            tmpImage = Texture;
        }

        public override void HandleEvents(FrameEventArgs e, bool down, bool up, int mousex, int mousey)
        {
            isClicked = false;

            isOver = IsMouseOver(mousex, mousey);
            if (!isOver)
            {
                isReadyToPress = false;
                isPressedAndOver = false;
            }
            if (isOver && up) isReadyToPress = true;
            if (isOver && isReadyToPress && down) isPressedAndOver = true;
            if (isOver && up && isPressedAndOver)
            {
                isClicked = true;
                isReadyToPress = false;
                isPressedAndOver = false;
            }

        }

        private bool IsMouseOver(int mousex, int mousey)
        {
            bool isOver = (mousex > xpos && mousex < xpos + width)
            && (mousey > ypos && mousey < ypos + height);

            if (isOver)Texture = overImage;
            else Texture = tmpImage;

            return isOver;
        }

    }
}
