using OpenTK;
using Engine.cgimin.sound;


namespace TowerDefense.objects.enemies
{
    class Slime4 : Enemy
    {
        public Slime4(Vector3 pos, int wave) : base(0, 90, 4000.0f, 1.0f, pos, 0.4f, 1, wave)
        {
            _obj = ResourceManager.Objects["ENEMY_1"];
            _texture = ResourceManager.Textures["ENEMY_1_4"];
            _deadSound = new Sound(ResourceManager.Sounds["SLIME_DEAD"]);

        }
    }

}
