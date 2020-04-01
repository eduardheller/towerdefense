using OpenTK;
using Engine.cgimin.sound;

namespace TowerDefense.objects.enemies
{
    class Slime3 : Enemy
    {
        public Slime3(Vector3 pos, int wave) : base(0, 70, 3000.0f, 1.0f, pos, 0.4f, 1f, wave)
        {
            _obj = ResourceManager.Objects["ENEMY_1"];
            _texture = ResourceManager.Textures["ENEMY_1_3"];
            _deadSound = new Sound(ResourceManager.Sounds["SLIME_DEAD"]);
 
        }
    }

}
