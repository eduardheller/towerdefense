using OpenTK;


namespace TowerDefense.gui
{
    class GUIBackground : GUIElement
    {
        public GUIBackground(int x, int y, int pwidth, int pheight, int screenWidth, int screenHeight, float alpha, int textureid) : base(x, y, pwidth, pheight, screenWidth, screenHeight, alpha, textureid)
        {
        }

        public override void HandleEvents(FrameEventArgs e, bool down, bool up, int mousex, int mousey)
        {

        }
    }
}
