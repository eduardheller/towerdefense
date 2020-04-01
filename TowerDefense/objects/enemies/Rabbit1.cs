using OpenTK;
using Engine.cgimin.sound;


namespace TowerDefense.objects.enemies
{
    class Rabbit1 : Enemy
    {
        public Rabbit1(Vector3 pos, int wave) : base(1,10,300.0f, 2.0f, pos,40.0f,0.01f, wave)
        {
            _obj = ResourceManager.Objects["ENEMY_2"];
            _texture = ResourceManager.Textures["ENEMY_2"];
            _deadSound = new Sound(ResourceManager.Sounds["RABBIT_DEAD"]);

        }
    }

}
