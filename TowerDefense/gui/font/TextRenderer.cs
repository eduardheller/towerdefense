using System;
using Engine.cgimin.camera;
using Engine.cgimin.material;
using Engine.cgimin;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.object3d;
using System.Collections.Generic;
using Engine.cgimin.material.distancefieldtext;

namespace TowerDefense.gui.font
{
    class TextRenderer : BaseMaterial
    {
        private int _textureAtlas;
        private List<Text> _text;
        private DistanceFieldMaterial _distanceFieldMaterial;



        public TextRenderer(int atlas)
        {
            _textureAtlas = atlas;
            _text = new List<Text>();
            _distanceFieldMaterial = new DistanceFieldMaterial();
        }

        public void Add(Text text)
        {
            _text.Add(text);
        }



        public void Render()
        {
            GL.Disable(EnableCap.DepthTest);
            foreach(Text text in _text)
            {
                if(text.IsVisible) _distanceFieldMaterial.Draw(text.Object,_textureAtlas,text.Color,text.Alpha,text.Edge,text.Width);
            }
            GL.Enable(EnableCap.DepthTest);
        }


        public void UnLoad()
        {
            foreach (Text text in _text)
            {
                text.Object.UnLoad();
            }
        }
 
        public override void DrawWithSettings(BaseObject3D object3d, MaterialSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
