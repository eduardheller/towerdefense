using Engine.cgimin.camera;
using Engine.cgimin.collision;
using Engine.cgimin.helpers;
using OpenTK;
using System.Collections.Generic;
using TowerDefense.objects;

namespace TowerDefense.collision
{
    class TowerRayCollision
    {
        private bool isClicked;
        private bool isReadyToPress;
        private bool isPressedAndOver;
        private bool isOver;

        public bool IsClicked
        {
            get { return isClicked; }
        }

        public TowerRayCollision()
        {
            
        }

        public Tower Handle(bool overgui, bool down, bool up, List<Tower> towers, int mousex, int mousey, int width, int height)
        {
            if (overgui) return null;
            isClicked = false;

            Tower tower = Collision(towers,mousex, mousey,width,height);

            if(tower != null)
            {
               isOver = true;
               tower.IsMouseOver = true;
            }
            else
            {
               isOver = false;
            }
   
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
                return tower;
            }
            return tower;
        }

        public Tower Collision(List<Tower> towers, int mousex, int mousey,int width ,int height)
        {
      
            MouseRay mouseray = new MouseRay(width, height);
            Vector3 ray = mouseray.get3DMouseCoords(mousex, mousey);
            Vector3 invertedray = new Vector3();
            invertedray.X = 1.0f / ray.X;
            invertedray.Y = 1.0f / ray.Y;
            invertedray.Z = 1.0f / ray.Z;
            float length = -1;
            float minlength = 10000;

            Tower returnedTower = null;
            foreach (Tower tower in towers)
            {

                tower.IsMouseOver = false;
                if (GeometryHelpers.RayAABBIntersect(invertedray, Camera.position, tower.AABB(), out length))
                {
                    if (length < minlength)
                    {
                        returnedTower = tower;
                        minlength = length;
                    }
                }
            }
            
            return returnedTower;
        }
    }
}
