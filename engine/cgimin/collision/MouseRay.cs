using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Engine.cgimin.camera;

namespace Engine.cgimin.collision
{
    public class MouseRay
    {
        private int screenWidth;
        private int screenHeight;

        public MouseRay(int sWidth, int sHeight)
        {
            screenWidth = sWidth;
            screenHeight = sHeight;
        }

        public Vector3 get3DMouseCoords(int mousex, int mousey)
        {
            Vector3 normalizedCoords = getNormalizedMouseCoords(mousex, mousey);
            Vector4 clipCoords = new Vector4(normalizedCoords.X, normalizedCoords.Y, -1f, 1f);
            Vector4 eyeRay = getEyeRay(clipCoords).Normalized();
            return getWorldRay(eyeRay);
        } 

        private Vector3 getWorldRay(Vector4 eyeRay)
        {
            Vector3 worldRay = (Camera.Transformation * eyeRay).Xyz;
            return worldRay.Normalized();
        }

        private Vector4 getEyeRay(Vector4 clipCoords)
        {
            Vector4 eyeRay = Matrix4.Invert(Camera.PerspectiveProjection) * clipCoords;
            return new Vector4(eyeRay.X, eyeRay.Y, -1f, 0f);
        }
  
        private Vector3 getNormalizedMouseCoords(int mousex, int mousey)
        {
            float x = 2.0f * mousex / (float)screenWidth - 1;
            float y = 1  - (2.0f * mousey) / (float)screenHeight;
            float z = 1.0f;
            return new Vector3(x, y, z);
        }

    }
}
