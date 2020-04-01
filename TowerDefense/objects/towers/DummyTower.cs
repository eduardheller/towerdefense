using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TowerDefense.objects.towers
{
    class DummyTower : Tower
    {
        public DummyTower() : base(0, 0, 0, 0, Vector3.Zero)
        {
        }

        public override void CreateProjectile(FrameEventArgs e, Enemy target, List<Enemy> enemies)
        {
   
        }

        public override int GetUpgradeCost()
        {
            return 0;
        }

        public override void Rotate(FrameEventArgs e, Enemy enemy)
        {
   
        }

        public override void Update(FrameEventArgs e)
        {

        }

        public override void UpgradeModifier()
        {

        }

        protected override void LoadObjectFiles()
        {
  
        }
    }
}
