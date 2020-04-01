using OpenTK;
using Engine.cgimin.sound;

namespace TowerDefense.objects.enemies
{
    class Boss1 : Enemy
    {
        public Boss1(Vector3 pos, int wave) : base(1, 1000, 300000.0f, 0.2f, pos, 90.0f, 0.003f, wave)
        {
            _obj = ResourceManager.Objects["BOSS_1"];
            _texture = ResourceManager.Textures["BOSS_1"];
            _deadSound = new Sound(ResourceManager.Sounds["BOSS_DEAD"]);
        }
    }

}
