using OpenTK;
using Engine.cgimin.sound;


namespace TowerDefense.objects.enemies
{
    class Rabbit4 : Enemy
    {
        public Rabbit4(Vector3 pos, int wave) : base(1, 40, 1200.0f, 2.0f, pos, 40.0f, 0.01f, wave)
        {
            _obj = ResourceManager.Objects["ENEMY_2"];
            _texture = ResourceManager.Textures["ENEMY_2_4"];
            _deadSound = new Sound(ResourceManager.Sounds["RABBIT_DEAD"]);
   
        }
    }

}
