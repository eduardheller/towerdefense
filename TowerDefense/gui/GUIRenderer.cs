using System.Collections.Generic;
using Engine.cgimin.material.gui;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace TowerDefense.gui
{
    public class GUIRenderer
    {
        private List<GUIElement> elements;
        private GUIMaterial guiMaterial;

        public GUIRenderer()
        {
            elements = new List<GUIElement>();
            guiMaterial = new GUIMaterial();
        }

        public void Add(GUIElement element)
        {
            elements.Add(element);
        }

        public void Remove(GUIElement element)
        {
            if(elements.Contains(element))
                elements.Remove(element);
        }


        public void Update(FrameEventArgs e, bool up,bool down, int mousex, int mousey)
        {
            foreach(GUIElement element in elements)
            {
                if((element.IsVisible && !element.IsDisabled()) || element.AllowHandle)
                    element.HandleEvents(e,up,down, mousex, mousey);
            }
        }

        public void Render()
        {
            GL.Disable(EnableCap.DepthTest);
            foreach (GUIElement element in elements)
            {
                if(element.IsVisible)
                    guiMaterial.Draw(element.Object,element.Object.Transformation, element.Texture, element.Alpha);
                
            }
            GL.Enable(EnableCap.DepthTest);
        }


    }
}
