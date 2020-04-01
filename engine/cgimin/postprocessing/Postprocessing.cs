using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.material.simpletexture;
using Engine.cgimin.object3d;
using Engine.cgimin.postprocessing.bloom;
using Engine.cgimin.framebuffer;

namespace Engine.cgimin.postprocessing
{
    public class Postprocessing
    {

        private static int width;
        private static int height;
        private static BasicFrameBuffer _basicFrameBufferB;
        private static BasicFrameBuffer _basicFrameBufferA;
        private static int FramebufferName;
        private static int GlowTextureName0;
        private static int GlowTextureName1;
        private static int depthrenderbuffer;
        private static bool created;
        private static BaseObject3D fullscreenQuad;

        private static Bloom bloomFullscreenMaterial;
        private static CombineEffectsMaterial combineEffectsMaterial;
        private static BlurFullscreenMaterial blurFullscreenMaterial;
        private static SimpleFullscreenMaterial simpleFullscreenMaterial;

        public static void Init(int screenWidth, int screenHeight)
        {
            fullscreenQuad = new BaseObject3D();
            _basicFrameBufferB = new BasicFrameBuffer(screenWidth,screenHeight);
            _basicFrameBufferA = new BasicFrameBuffer(screenWidth, screenHeight);
            fullscreenQuad.addTriangle(new Vector3(1, -1, 0), new Vector3(1, 1, 0), new Vector3(-1, -1, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 0));
            fullscreenQuad.addTriangle(new Vector3(-1, -1, 0), new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1));
            fullscreenQuad.CreateVAO();

            bloomFullscreenMaterial = new Bloom();
            simpleFullscreenMaterial = new SimpleFullscreenMaterial();
            blurFullscreenMaterial = new BlurFullscreenMaterial();
            combineEffectsMaterial = new CombineEffectsMaterial();

            width = screenWidth;
            height = screenHeight;

            FramebufferName = 0;

            int glowTextureRef0 = 0;
            GL.GenTextures(1, out glowTextureRef0);
            GL.BindTexture(TextureTarget.Texture2D, glowTextureRef0);
            GlowTextureName0 = glowTextureRef0;
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, screenWidth, screenHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);


            int depthrenderbuffer;
            GL.GenRenderbuffers(1, out depthrenderbuffer);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthStencil, screenWidth, screenHeight);

            GL.GenFramebuffers(1, out FramebufferName);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferName);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, GlowTextureName0, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            

            DrawBuffersEnum[] drawEnum = { DrawBuffersEnum.ColorAttachment0};
            GL.DrawBuffers(1, drawEnum);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            created = true;

        }

        public static void OnResize(int width, int height)
        {
            if(created)
            {
                Init(width, height);
            }
          
        }

        public static void Start()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferName);
  
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, width, height);
        }


        public static void ApplyAndDraw()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);



            _basicFrameBufferA.Start();
            simpleFullscreenMaterial.Draw(fullscreenQuad, 0, GlowTextureName0);
            _basicFrameBufferA.End();

            _basicFrameBufferB.Start();
            bloomFullscreenMaterial.Draw(fullscreenQuad, GlowTextureName0);
            _basicFrameBufferB.End();


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferName);
            blurFullscreenMaterial.Draw(fullscreenQuad, _basicFrameBufferB.basicColorTexture, 0, 1 / 9200f, 0);
            blurFullscreenMaterial.Draw(fullscreenQuad, _basicFrameBufferB.basicColorTexture, 0, 0, 1 / 9200f);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

         

            combineEffectsMaterial.Draw(fullscreenQuad, GlowTextureName0, _basicFrameBufferA.basicColorTexture);

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

        }

    }
}
