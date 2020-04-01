using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using TowerDefense.objects;
using Engine.cgimin.object3d;
using Engine.cgimin.material.textureglow;
using Engine.cgimin.framebuffer;
using Engine.cgimin.material.alphatexture;
using Engine.cgimin.camera;
using Engine.cgimin.collision;
using Engine.cgimin.helpers;
using Engine.cgimin.sound;

namespace TowerDefense.states.towerbuild
{
    /// <summary>
    /// State die Aktiviert wird wenn ein Tower gebaut werden soll.
    /// Sie ist für die Umsetzung des Bauens da.
    /// </summary>
    class TowerBuildState : IGameState
    {
        private BasicFrameBuffer _basicFrameBuffer;
        private List<TowerSlot> _towerSlots;
        private TextureGlowMaterial _glowMaterial;
        private AlphaTextureMaterial _alphaTextureMaterial;
        private int _mousex;
        private int _mousey;
        private TowerSlot _currentTowerSlot;
        private PlayState _playState;
        private PlayStateGUI _guiMainState;
        private Type _towerType;
        private int _textureCube;
        private Tower _tower;
        private bool _mouseReleased;
        private PlaneObject3D _radius;
        private int _textureRadius;
        private Sound _buildSound;

        public TowerBuildState(PlayStateGUI guistate, PlayState play, Type towerType)
        {
            _guiMainState = guistate;
            _towerType = towerType;
            _playState = play;
            _radius = new PlaneObject3D();
            _glowMaterial = new TextureGlowMaterial();
            _alphaTextureMaterial = new AlphaTextureMaterial();
            _towerSlots = play.MapContext.TowerSlots;
            _mousex = 0;
            _mousey = 0;
            _currentTowerSlot = null;
            _textureCube = ResourceManager.Textures["BLOCK_1"];
            _textureRadius = ResourceManager.Textures["RADIUS"];
            _mouseReleased = false;
            _buildSound = new Sound(ResourceManager.Sounds["BUILD"]);
  
        }


        public override void Init()
        {
            base.Init();
            _basicFrameBuffer = new BasicFrameBuffer(GameManager.Window.Width,
                GameManager.Window.Height);

           
            _tower = (Tower)Activator.CreateInstance(_towerType, new object[] { Vector3.Zero });
        }

        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {
            base.HandleInput(e, mouse, keyboard);
            _mousex = mouse.X;
            _mousey = mouse.Y;

            if (!_mouseReleased)
            {
                _mouseReleased = mouse.GetState().IsButtonUp(MouseButton.Left);
            }

            if(keyboard[Key.Escape])
            {
                _guiMainState.ShowButtons();
                GameManager.RemoveState(this);
            }

            if (mouse.GetState().IsButtonDown(MouseButton.Left) && _mouseReleased)
            {
                if(_currentTowerSlot != null && _currentTowerSlot.Tower == null)
                {
                    if (_playState.Player.PayGold(_tower.Cost))
                    {
                        _currentTowerSlot.Tower = _tower;
                        _playState.AddTower(_tower);
                        
                    }
                    _currentTowerSlot.IsMouseOver = false;
                    _currentTowerSlot = null;
                    _guiMainState.ShowButtons();
                    GameManager.RemoveState(this);
                    _buildSound.SetPosition(Camera.position);
                    _buildSound.Play();
                }
            }
            
        }
        
        public override void OnResize(int screenWidth, int screenHeight)
        {
            base.OnResize(screenWidth, screenHeight);
            Init();
        }

        public override void Render(FrameEventArgs e)
        {
            base.Render(e);

            if (_currentTowerSlot != null)
            {
                _glowMaterial.Draw(_currentTowerSlot.Object, _currentTowerSlot.Transformation, new Vector4(0, 1, 0, 0.8f), _textureCube);
                _glowMaterial.Draw(_tower.ObjectGround, _tower.Transformation, new Vector4(0, 1, 0, 0.8f), _tower.TextureBase);
                _glowMaterial.Draw(_tower.ObjectTurret, _tower.TurretMatrix, new Vector4(0, 1, 0, 0.8f), _tower.TextureTurret);
                _alphaTextureMaterial.Draw(_radius, _textureRadius, 0.4f);
                if (_tower.ObjectHub != null) _glowMaterial.Draw(_tower.ObjectHub, _tower.HubMatrix, new Vector4(0, 1, 0, 0.8f), _tower.TextureBase);
            }

        }

        public override void Close()
        {
            base.Close();
            _radius.UnLoad();
        }

        public override void Update(FrameEventArgs e)
        {
            base.Update(e);

            MouseRay mouseray = new MouseRay(GameManager.Window.Width, GameManager.Window.Height);
            Vector3 ray = mouseray.get3DMouseCoords(GameManager.Window.Mouse.X, GameManager.Window.Mouse.Y);

            Vector3 invertedray = new Vector3();
            invertedray.X = 1.0f / ray.X;
            invertedray.Y = 1.0f / ray.Y;
            invertedray.Z = 1.0f / ray.Z;
            float length = -1;
            float minlength = 10000;
            _currentTowerSlot = null;
            foreach (TowerSlot slot in _towerSlots)
            {
                slot.IsMouseOver = false;

                if (slot.Tower == null)
                {
                    if (GeometryHelpers.RayAABBIntersect(invertedray, Camera.position, slot.AABB(), out length))
                    {
                        if (length < minlength)
                        {
                            _currentTowerSlot = slot;
                            minlength = length;
                        }
                    }
                }
            }

            if (_currentTowerSlot != null)
            {
                _tower.SetPosition(_currentTowerSlot.Position + Vector3.UnitY);
                _currentTowerSlot.IsMouseOver = true;

                _currentTowerSlot.Transformation = Matrix4.CreateScale(1.05f);
                _currentTowerSlot.Transformation *= Matrix4.CreateTranslation(_currentTowerSlot.Position);

                _radius.Transformation = Matrix4.CreateScale(_tower.Radius);
                _radius.Transformation *= Matrix4.CreateTranslation(_tower.Position + new Vector3(0, 0.1f, 0));

                _glowMaterial.Draw(_currentTowerSlot.Object, _currentTowerSlot.Transformation, new Vector4(0, 1, 0, 0.8f), _textureCube);
                _glowMaterial.Draw(_tower.ObjectGround, _tower.Transformation, new Vector4(0, 1, 0, 0.8f), _tower.TextureBase);
                _glowMaterial.Draw(_tower.ObjectTurret, _tower.TurretMatrix, new Vector4(0, 1, 0, 0.8f), _tower.TextureTurret);
                _alphaTextureMaterial.Draw(_radius, _textureRadius, 0.4f);
                if (_tower.ObjectHub != null) _glowMaterial.Draw(_tower.ObjectHub, _tower.HubMatrix, new Vector4(0, 1, 0, 0.8f), _tower.TextureBase);
            }

        }

        // Damit keine mehrfach States entstehen
        public override bool Equals(object obj)
        {
            if (!(obj is TowerBuildState)) { return false; }
            return (this._playState == ((TowerBuildState)obj)._playState);
        }

        public override int GetHashCode()
        {
            return (this._playState.GetHashCode()) ^ (this._playState.GetHashCode());
        }
    }
}
