using System;
using System.IO;
using Engine.cgimin.object3d;
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.helpers;

namespace Engine.cgimin.material
{

    public struct MaterialSettings
    {
        // Texturen
        public int colorTexture;
        public int normalTexture;
        public int cubeTexture;

        // Animierte Texture
        public float textureAnimationSpeed;

        // Shininess für Materialien mit Specular
        public float shininess;

        // Werte für das Blending 
        public bool transparent;
        public float alpha;
        public BlendingFactorSrc blendFactorSource;
        public BlendingFactorDest blendFactorDest;

        // Postprocessing Flag
        public bool postProcessing;
    }


    public abstract class BaseMaterial
    {
        private int VertexObject;
        private int FragmentObject;

        protected const string MATERIAL_DIRECTORY = "cgimin/material/";

        public int Program;

        public void CreateShaderProgram(string pathVS, string pathFS)
        {
            Program = ShaderCompiler.CreateShaderProgram(pathVS, pathFS);

            // Hinweis: Program wird noch nicht gelinkt.
        }


        /// <summary>
        /// Abstrakt, damit erbende Klassen zum implementieren einer Draw-Methode gezwungen werden.
        /// </summary>
        public abstract void DrawWithSettings(BaseObject3D object3d, MaterialSettings settings);

    }
}
