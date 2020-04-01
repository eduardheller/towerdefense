using System;
using System.Collections.Generic;
using OpenTK;
using TowerDefense.objects;
using TowerDefense.objects.towers;


namespace TowerDefense.map
{
    /// <summary>
    /// Lädt die Daten aus der mapdatei und speichert sie in Mapcontext
    /// </summary>
    class MapLoader
    {
        private const float GRID_SIZE = 2.1f;
        private const float TOWER_SLOT_SIZE = 1.0f;
        private const float GROUND_Y = 1.0f;

        private float _sizex;
        private float _sizey;
        private MapContext _mapContext;
        private Random _random;
        public MapLoader(String path)
        {
            _mapContext = new MapContext();
            _random = new Random();
            Create(path);
        }

        private void Create(string path)
        {
            float tx = 0;
            float tz = 0;
            string[] lines = System.IO.File.ReadAllLines(path);
            int z = 0;
          
            foreach (string line in lines)
            {
                
                for (int i = 0; i < line.Length; i++)
                {
                    int n;
                    // Wave Informationen
                    if (line[0] == 'W') 
                    {
                        string[] words = line.Split(':');
                        List<MapContext.Wave> waves = new List<MapContext.Wave>();
                        for(int w = 1; w < words.Length-1; w +=2 )
                        {
                            MapContext.Wave wave;
                            string enemyId = words[w];
                            int count = int.Parse(words[w+1]);
                            wave.enemyId = Type.GetType("TowerDefense.objects.enemies."+enemyId);
                            wave.count = count;
                            waves.Add(wave);
                        }
                        MapContext.AddWave(waves);
                        break;
                    }
                    // Anzahl an Wege ermitteln
                    else if (line[0] == 'N') 
                    {
                        string[] words = line.Split(':');
                        int maxWays = int.Parse(words[1]);
                        MapContext.CreateWayMarks(maxWays);
                        break;
                    }
                    // MapLoader Parse
                    else
                    {
                        _sizex = line.Length * GRID_SIZE;
                        // Tower Slot
                        if (line[i] == 'x') 
                        {
                            CreateTowerSlot(tx, tz);
                        }
                        else if (line[i] == 'l')
                        {
                            float rot = (float)_random.NextDouble() * (360.0f - (-360.0f)) + (-360.0f);
                            float anim = (float)_random.NextDouble() * (1000.0f - (-1000.0f)) + (-1000.0f);
                            int id = _random.Next(1, 4);
                            MapContext.EnvironmentObject envobj = new MapContext.EnvironmentObject();
                            envobj = new MapContext.EnvironmentObject();
                            envobj.Transform = Matrix4.CreateRotationY(rot);
                            envobj.Transform *= Matrix4.CreateTranslation(new Vector3(tx, 1, tz));
                            envobj.Object = ResourceManager.Objects["PLANT_0" + id];
                            envobj.animationTimer = anim;
                            MapContext.AddPlantObject(envobj);
                            CreateTowerSlot(tx, tz, true);
                        }
                        else if (line[i] == 't')
                        {
                            float rot = (float)_random.NextDouble() * (360.0f - (-360.0f)) + (-360.0f);
                            float anim = (float)_random.NextDouble() * (1000.0f - (-1000.0f)) + (-1000.0f);
                            int id = _random.Next(1,3);
                            MapContext.EnvironmentObject envobj = new MapContext.EnvironmentObject();
                            envobj.Transform = Matrix4.CreateRotationY(rot);
                            envobj.Transform *= Matrix4.CreateTranslation(new Vector3(tx, 1, tz));
                            envobj.Object = ResourceManager.Objects["TREE_0"+id];
                            envobj.animationTimer = anim;

                            MapContext.AddTreeObject(envobj);
                            envobj = new MapContext.EnvironmentObject();
                            envobj.Transform = Matrix4.CreateRotationY(rot);
                            envobj.Transform *= Matrix4.CreateTranslation(new Vector3(tx, 1, tz));
                            envobj.Object = ResourceManager.Objects["TREE_0"+id+"_LEAVES"];
                            envobj.animationTimer = anim;
                            MapContext.AddPlantObject(envobj);
                            CreateTowerSlot(tx, tz,true);
                        }
                        else if (line[i] == 'r')
                        {
                            float rot = (float)_random.NextDouble() * (360.0f - (-360.0f)) + (-360.0f);
                            int id = _random.Next(1, 2);
                            MapContext.EnvironmentObject envobj = new MapContext.EnvironmentObject();
                            envobj = new MapContext.EnvironmentObject();
                            envobj.Transform = Matrix4.CreateRotationY(rot);
                            envobj.Transform *= Matrix4.CreateTranslation(new Vector3(tx, 1, tz));
                            envobj.Object = ResourceManager.Objects["ROCK_0" + id];
                            envobj.animationTimer = 0;
                            MapContext.AddRockObject(envobj);
                            CreateTowerSlot(tx, tz, true);
                        }
                        else if (line[i] == '-')
                        {

                        }
                        else if (int.TryParse(line[i].ToString(), out n)) 
                        {
                            // Wegpunkt eintragungen
                            if (n > 0) MapContext.AssignWayMark(n-1,new Vector3(tx, GROUND_Y, tz));
                            CreateWay(new Vector3(tx, 0, tz));
                            
                        }
                        tx += GRID_SIZE;
                    }

                }
                tz += GRID_SIZE;
                tx = 0f;
                z++;
            }
            _sizey = GRID_SIZE * (z - 1);
            MapContext.MapCenter = CenterPoint;
            MapContext.MapWidth = (int)_sizex;
            MapContext.MapHeight = (int)_sizey;
        }


        private Vector3 CenterPoint
        {
            get { return new Vector3((_sizex / 2.0f) - (GRID_SIZE / 2.0f), 0, _sizey / 2.0f); }
        }

        public MapContext MapContext
        {
            get { return _mapContext; }
        }



        private void CreateTowerSlot(float posx, float posz, bool notower = false)
        {
            TowerSlot towerSlot = new TowerSlot();
            towerSlot.Position = new Vector3(posx, 0, posz);
            towerSlot.Size = TOWER_SLOT_SIZE;
            towerSlot.Transformation = Matrix4.Identity;
            towerSlot.Transformation *= Matrix4.CreateTranslation(towerSlot.Position);
            if (notower) towerSlot.Tower = new DummyTower(); // Dummy tower
            MapContext.AddSlot(towerSlot);
        }

        private void CreateWay(Vector3 pos)
        {
            Way way = new Way();
            way.Position = new Vector3(pos);
            way.Transformation = Matrix4.Identity;
            way.Transformation *= Matrix4.CreateTranslation(way.Position);
            MapContext.AddWay(way);
        }

    }

}
