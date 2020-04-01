using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.material.simpletexture;
using Engine.cgimin.object3d;

namespace Engine.cgimin.framebuffer
{
    public class BasicFrameBuffer
    {
        private int width;
        private int height;

        private int FramebufferName;
        public int basicColorTexture;

        private static BaseObject3D fullscreenQuad;

        public BasicFrameBuffer(int screenWidth, int screenHeight)
        {
            fullscreenQuad = new BaseObject3D();
            fullscreenQuad.addTriangle(new Vector3(1, -1, 0), new Vector3(-1, -1, 0), new Vector3(1, 1, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 1));
            fullscreenQuad.addTriangle(new Vector3(-1, -1, 0), new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1));
            fullscreenQuad.CreateVAO();
      
            width = screenWidth;
            height = screenHeight;

            FramebufferName = 0;
            GL.GenFramebuffers(1, out FramebufferName);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferName);

            int basicColorRef = 0;
            GL.GenTextures(1, out basicColorRef);
            GL.BindTexture(TextureTarget.Texture2D, basicColorRef);
            basicColorTexture = basicColorRef;
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, screenWidth, screenHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            int depthrenderbuffer;
            GL.GenRenderbuffers(1, out depthrenderbuffer);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, screenWidth, screenHeight);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, basicColorTexture, 0);

            DrawBuffersEnum[] drawEnum = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1 };
            GL.DrawBuffers(1, drawEnum);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Start()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferName);

            GL.ClearColor(OpenTK.Graphics.Color4.Transparent);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Viewport(0, 0, width, height);
            GL.Enable(EnableCap.DepthTest);
        }

        public void End()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
