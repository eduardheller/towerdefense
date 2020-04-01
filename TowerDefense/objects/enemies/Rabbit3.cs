using OpenTK;
using Engine.cgimin.sound;


namespace TowerDefense.objects.enemies
{
    class Rabbit3 : Enemy
    {
        public Rabbit3(Vector3 pos, int wave) : base(1, 30, 900.0f, 2.0f, pos, 40.0f, 0.01f, wave)
        {
            _obj = ResourceManager.Objects["ENEMY_2"];
            _texture = ResourceManager.Textures["ENEMY_2_3"];
            _deadSound = new Sound(ResourceManager.Sounds["RABBIT_DEAD"]);
        }
    }

}
