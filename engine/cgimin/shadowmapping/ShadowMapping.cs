using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Engine.cgimin.light;
using Engine.cgimin.camera;

namespace Engine.cgimin.shadowmapping
{
    public class ShadowMapping
    {

        private static int FramebufferName;
        private static Matrix4 depthProjection;

        public static int DepthTexture { get; private set; }
        public static Matrix4 DepthBias { get; private set; }

        public static Matrix4 ShadowTransformation { get;  private set; }
        public static Matrix4 ShadowProjection { get; private set; }

        private static int textureSize;
        private static float boxXYSize;

        public static void Init(int textureDimension, float boxXYDimension, float boxZDimension) {
            textureSize = textureDimension;
            boxXYSize = boxXYDimension;

            Matrix4.CreateOrthographicOffCenter(-boxXYSize, boxXYSize, -boxXYSize, boxXYSize, -boxZDimension, boxZDimension, out depthProjection);

            //DepthBias = Matrix4.Identity;

            DepthBias = Matrix4.CreateScale(0.5f, 0.5f, 0.5f);
            DepthBias *= Matrix4.CreateTranslation(boxXYDimension * 0.5f, boxXYDimension * 0.5f, -boxZDimension * 0.5f);

            FramebufferName = 0;

            GL.GenFramebuffers(1, out FramebufferName);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferName);

            int depthTextureRef = 0;
            GL.GenTextures(1, out depthTextureRef);
            GL.BindTexture(TextureTarget.Texture2D, depthTextureRef);
            DepthTexture = depthTextureRef;

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent16, textureSize, textureSize, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)All.Lequal);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRToTexture);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, DepthTexture, 0);
            GL.DrawBuffer(DrawBufferMode.None);

        }

        public static void StartShadowMapping()
        {
            GL.Viewport(0, 0, textureSize, textureSize);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferName);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Camera.useOtherView = true;
            Camera.SetProjectionMatrix(depthProjection);
            Camera.SetTransformMatrix(Matrix4.LookAt(Camera.Position, Camera.Position - Light.lightDirection, new Vector3(0, 1, 0)));
            Camera.CreateViewFrustumPlanes(Camera.Transformation * Camera.PerspectiveProjection);

            ShadowTransformation = Camera.View;
            ShadowProjection = Camera.PerspectiveProjection;
        }



        public static void EndShadowMapping()
        {
            Camera.SetBackToLastCameraSettings();
            Camera.useOtherView = false;
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
        }


    }
}
