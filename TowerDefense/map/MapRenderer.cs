using Engine.cgimin.object3d;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using TowerDefense.objects;
using Engine.cgimin.material.normalmappingshadowinstanced;
using Engine.cgimin.material.treematerial;

namespace TowerDefense.map
{
    public class MapRenderer
    {
        private MapContext _context;
        private static float[] _positions;
        private int _posVBO;
        private BaseObject3D _obj;
        private BaseObject3D _base;
        private BaseObject3D _bridge;
        private int _textureCube;
        private int _textureNormal;
        private int _textureBase;
        private int _textureBridge;
        private int _textureTree;
        private int _textureTreeLeaves;
        private int _textureRock;
        private int _textureWay;
        private int _textureWayNormal;
        private NormalMappingShadowInstancedMaterial _normalMapping;
        private TreeMaterial _ambientDiffuse;
        private const float SHININESS = 32.0f;
        private float _time;

        public MapRenderer(MapContext context)
        {
            _context = context;
            _positions = new float[context.TowerSlots.Count * 3];
            _obj = ResourceManager.Objects["BLOCK_1"];
            _textureTree = ResourceManager.Textures["TREE"];
            _textureTreeLeaves = ResourceManager.Textures["LEAVES"];
            _textureRock = ResourceManager.Textures["ROCK"];
            _textureCube = ResourceManager.Textures["BLOCK_1"];
            _textureNormal = ResourceManager.Textures["BLOCK_NORMAL_1"];
            _textureWay = ResourceManager.Textures["BLOCK_WAY_1"];
            _textureWayNormal = ResourceManager.Textures["BLOCK_WAY_NORMAL_1"];
            _textureBase = ResourceManager.Textures["BASE"];
            _base = ResourceManager.Objects["BASE"];
            _bridge = ResourceManager.Objects["BRIDGE"];
            _textureBridge = ResourceManager.Textures["BRIDGE"];
            _normalMapping = new NormalMappingShadowInstancedMaterial();
            _ambientDiffuse = new TreeMaterial();

            InitializeInstanceBuffer(ref _posVBO, context.TowerSlots.Count * 3 * sizeof(float));

            GL.BindVertexArray(_obj.Vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _obj.IndexBuffer);

            CreateAttribInstanced(ref _posVBO, 5, 3, Vector3.SizeInBytes);

            GL.BindVertexArray(0);
            _time = 0.0f;
            
            _base.Transformation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-90));
            int last = context.WayMarks.Length - 1;
            _base.Transformation *= Matrix4.CreateTranslation(context.WayMarks[last] +Vector3.UnitY*2);

            _bridge.Transformation = Matrix4.CreateTranslation(context.StartPosition - Vector3.UnitY);

        }

        public void Update(FrameEventArgs e)
        {


        }

        public void Render(FrameEventArgs e, Vector4 clipplane)
        {
            DrawTowerSlots();
            DrawWays();
            _time += (float)e.Time;

            _ambientDiffuse.Draw(_base, _base.Transformation, _textureBase, 32, 0, 0, clipplane);
            _ambientDiffuse.Draw(_bridge, _bridge.Transformation, _textureBridge, 32, 0, 0, clipplane);
            foreach (MapContext.EnvironmentObject tree in _context.Trees)
            {
                _ambientDiffuse.Draw(tree.Object, tree.Transform, _textureTree, 32, _time, tree.animationTimer,clipplane) ;
            }

            foreach (MapContext.EnvironmentObject leaves in _context.Plants)
            {
                _ambientDiffuse.Draw(leaves.Object, leaves.Transform, _textureTreeLeaves, 32, _time, leaves.animationTimer,clipplane);
            }

            foreach (MapContext.EnvironmentObject rock in _context.Rocks)
            {
                _ambientDiffuse.Draw(rock.Object, rock.Transform, _textureRock, 32, 0, rock.animationTimer, clipplane);
            }
        }

        private void CreateAttribInstanced(ref int vbo, int index, int size, int stride)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, stride, 0);
            GL.VertexAttribDivisor(index, 1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void RefreshInstanceBuffer(ref int vbo, float[] data, int size)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, size, data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void InitializeInstanceBuffer(ref int vbo, int size, BufferUsageHint usage = BufferUsageHint.DynamicDraw)
        {
            GL.GenBuffers(1, out vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, size, IntPtr.Zero, usage);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void DrawTowerSlots()
        {
            int i = 0;
            foreach (TowerSlot slot in _context.TowerSlots)
            {
                _positions[i++] = slot.Position.X;
                _positions[i++] = slot.Position.Y;
                _positions[i++] = slot.Position.Z;
            }
            RefreshInstanceBuffer(ref _posVBO, _positions, _context.TowerSlots.Count * sizeof(float) * 3);
            _normalMapping.Draw(_obj, _context.TowerSlots.Count, _textureCube, _textureNormal, SHININESS);
        }

        private void DrawWays()
        {
            int i = 0;
            foreach (Way way in _context.Ways)
            {
                _positions[i++] = way.Position.X;
                _positions[i++] = way.Position.Y;
                _positions[i++] = way.Position.Z;
            }
            RefreshInstanceBuffer(ref _posVBO, _positions, _context.Ways.Count * sizeof(float) * 3);
            _normalMapping.Draw(_obj, _context.Ways.Count, _textureWay, _textureWayNormal, SHININESS);
        }

        public void UnLoad()
        {


        }
    }
}
