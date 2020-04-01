using OpenTK;
using Engine.cgimin.sound;

namespace TowerDefense.objects.enemies
{
    class Bat1 : Enemy
    {
        public Bat1(Vector3 pos, int wave) : base(1, 20, 500.0f, 1.5f, pos, 40.0f, 0.01f, wave)
        {
            _obj = ResourceManager.Objects["ENEMY_3"];
            _texture = ResourceManager.Textures["ENEMY_3"];
            _deadSound = new Sound(ResourceManager.Sounds["BAT_DEAD"]);
        }
    }

}
