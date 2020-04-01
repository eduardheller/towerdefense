using OpenTK;
using System;
using System.Collections.Generic;
using TowerDefense.objects;
using Engine.cgimin.object3d;

namespace TowerDefense.map
{
    public class MapContext
    {

        public struct EnvironmentObject
        {
            public Matrix4 Transform;
            public BaseObject3D Object;
            public float animationTimer;

        }
        private List<TowerSlot> _towerSlots;

        private Vector3[] _wayMarks;
        private List<Way> _ways;
        private List<List<Wave>> _waves;
        private Vector3 _mapCenter;
        private int _mapWidth;
        private int _mapHeight;
        private List<EnvironmentObject> _trees;
        private List<EnvironmentObject> _plants;
        private List<EnvironmentObject> _rocks;

        public struct Wave
        {
            public int count;
            public Type enemyId;
        }

        public MapContext()
        {
            _towerSlots = new List<TowerSlot>();
            _ways = new List<Way>();
            _trees = new List<EnvironmentObject>();
            _plants = new List<EnvironmentObject>();
            _rocks = new List<EnvironmentObject>();
            _waves = new List<List<Wave>>();
            
        }

        public void AddTreeObject(EnvironmentObject obj)
        {
            _trees.Add(obj);
        }

        public void AddPlantObject(EnvironmentObject obj)
        {
            Plants.Add(obj);
        }

        public void AddRockObject(EnvironmentObject obj)
        {
            Rocks.Add(obj);
        }

        public void CreateWayMarks(int size)
        {
            _wayMarks = new Vector3[size];
        }

        public void AssignWayMark(int i,Vector3 pos)
        {
            WayMarks[i] = pos;
            if (i == 0)
            {
                _wayMarks[0] += new Vector3(0, 0, -2.0f);
            }
        } 

        public void AddWave(List<Wave> wave)
        {
            Waves.Add(wave);
        }

        public void AddSlot(TowerSlot slot)
        {
            TowerSlots.Add(slot);
        }

        public void AddWay(Way way)
        {
            _ways.Add(way);
        }

        public Vector3 StartPosition
        {
            get { return WayMarks[0]; }
        }

        public List<TowerSlot> TowerSlots
        {
            get { return _towerSlots; }
        }

        public Vector3[] WayMarks
        {
            get { return _wayMarks; }
        }

        public List<Way> Ways
        {
            get { return _ways; }
        }

        public List<List<Wave>> Waves
        {
            get { return _waves; }
        }

        public Vector3 MapCenter
        {
            get { return _mapCenter; }
            set { _mapCenter = value; }
        }

        public List<EnvironmentObject> Trees
        {
            get { return _trees; }
        }

        public List<EnvironmentObject> Plants
        {
            get { return _plants; }
        }

        public List<EnvironmentObject> Rocks
        {
            get {  return _rocks; }
        }

        public int MapWidth
        {
            get
            {
                return _mapWidth;
            }

            set
            {
                _mapWidth = value;
            }
        }

        public int MapHeight
        {
            get
            {
                return _mapHeight;
            }

            set
            {
                _mapHeight = value;
            }
        }
    }
}
