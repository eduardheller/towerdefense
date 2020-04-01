using OpenTK;
using Engine.cgimin.sound;

namespace TowerDefense.objects.enemies
{
    class Bat4 : Enemy
    {
        public Bat4(Vector3 pos, int wave) : base(1, 50, 2000.0f, 1.5f, pos, 40.0f, 0.01f, wave)
        {
            _obj = ResourceManager.Objects["ENEMY_3"];
            _texture = ResourceManager.Textures["ENEMY_3_4"];
            _deadSound = new Sound(ResourceManager.Sounds["BAT_DEAD"]);
        }
    }

}
