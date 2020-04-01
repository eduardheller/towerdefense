using System;
using Engine.cgimin.texture;
using Engine.cgimin.object3d;
using Engine.cgimin.material.simpletexture;
using Engine.cgimin.camera;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Engine.cgimin.skybox
{
    public class SkyBox
    {

        private static int frontID;
        private static int backID;
        private static int leftID;
        private static int rightID;
        private static int upID;
        private static int downID;

        private static BaseObject3D frontSide;
        private static BaseObject3D backSide;
        private static BaseObject3D leftSide;
        private static BaseObject3D rightSide;
        private static BaseObject3D upSide;
        private static BaseObject3D downSide;

        private static SimpleTextureMaterial simpleTextureMaterial;

        public static void Init(int front, int back, int left, int right, int up, int down, float scaletop, float scaledown)
        {
            simpleTextureMaterial = new SimpleTextureMaterial();

            frontID = front;
            backID = back;
            leftID = left;
            rightID = right;
            upID = up;
            downID = down;

            float s = 400.0f;

            frontSide = new BaseObject3D();
            frontSide.addTriangle(new Vector3(-s, -s * scaledown, -s), new Vector3(s, -s * scaledown, -s), new Vector3(s, s / scaletop, -s), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0));
            frontSide.addTriangle(new Vector3(-s, -s * scaledown, -s), new Vector3(s, s / scaletop, -s), new Vector3(-s, s / scaletop, -s), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 0));
            frontSide.CreateVAO();

            backSide = new BaseObject3D();
            backSide.addTriangle(new Vector3(-s, -s * scaledown, s), new Vector3(s, -s * scaledown, s), new Vector3(s, s / scaletop, s), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 0));
            backSide.addTriangle(new Vector3(-s, -s * scaledown, s), new Vector3(s, s / scaletop, s), new Vector3(-s, s / scaletop, s), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0));
            backSide.CreateVAO();

            rightSide = new BaseObject3D();
            rightSide.addTriangle(new Vector3(-s, -s * scaledown, -s), new Vector3(-s, s / scaletop, -s), new Vector3(-s, s / scaletop, s), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0));
            rightSide.addTriangle(new Vector3(-s, -s * scaledown, -s), new Vector3(-s, s / scaletop, s), new Vector3(-s, -s * scaledown, s), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, 1));
            rightSide.CreateVAO();

            leftSide = new BaseObject3D();
            leftSide.addTriangle(new Vector3(s, -s * scaledown, -s), new Vector3(s, s / scaletop, -s), new Vector3(s, s / scaletop, s), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0));
            leftSide.addTriangle(new Vector3(s, -s * scaledown, -s), new Vector3(s, s / scaletop, s), new Vector3(s, -s * scaledown, s), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1));
            leftSide.CreateVAO();

            upSide = new BaseObject3D();
            upSide.addTriangle(new Vector3(-s, s / scaletop, -s), new Vector3(s, s / scaletop, -s), new Vector3(s, s / scaletop, s), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1));
            upSide.addTriangle(new Vector3(-s, s / scaletop, -s), new Vector3(s, s / scaletop, s), new Vector3(-s, s / scaletop, s), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1));
            upSide.CreateVAO();

            downSide = new BaseObject3D();
            downSide.addTriangle(new Vector3(-s, -s * scaledown, -s), new Vector3(s, -s * scaledown, -s), new Vector3(s, -s * scaledown, s), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1));
            downSide.addTriangle(new Vector3(-s, -s * scaledown, -s), new Vector3(s, -s * scaledown, s), new Vector3(-s, -s * scaledown, s), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1));
            downSide.CreateVAO();

        }

        public static void Draw()
        {
            Matrix4 saveTrasform = Camera.Transformation;
            Matrix4 cameraTransform = Matrix4.CreateTranslation(Camera.Position.X, Camera.Position.Y, Camera.Position.Z) * saveTrasform;
            Camera.SetTransformMatrix(cameraTransform);

            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            simpleTextureMaterial.Draw(frontSide, frontSide.Transformation, frontID);
            simpleTextureMaterial.Draw(backSide, backSide.Transformation, backID);
            simpleTextureMaterial.Draw(leftSide, leftSide.Transformation, leftID);
            simpleTextureMaterial.Draw(rightSide, rightSide.Transformation, rightID);
            simpleTextureMaterial.Draw(upSide, upSide.Transformation, upID);
            simpleTextureMaterial.Draw(downSide, downSide.Transformation, downID);

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            Camera.SetTransformMatrix(saveTrasform);
        }

    }
}
