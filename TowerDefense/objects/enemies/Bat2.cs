using OpenTK;
using Engine.cgimin.sound;


namespace TowerDefense.objects.enemies
{
    class Bat2 : Enemy
    {
        public Bat2(Vector3 pos, int wave) : base(1, 30, 1000.0f, 1.5f, pos, 40.0f, 0.01f, wave)
        {
       
            _obj = ResourceManager.Objects["ENEMY_3"];
            _texture = ResourceManager.Textures["ENEMY_3_2"];
            _deadSound = new Sound(ResourceManager.Sounds["BAT_DEAD"]);


        }
    }

}
