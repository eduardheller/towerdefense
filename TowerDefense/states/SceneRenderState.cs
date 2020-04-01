using OpenTK;
using TowerDefense.map;
using TowerDefense.particles;
using Engine.cgimin.camera;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Engine.cgimin.light;
using Engine.cgimin.shadowmapping;
using Engine.cgimin.water;
using Engine.cgimin.skybox;
using Engine.cgimin.object3d;
using Engine.cgimin.postprocessing;


namespace TowerDefense.states
{
    /// <summary>
    /// Hauptklasse für die PlayState, die vorrangig nur Renderaufgaben übernimmt ( Shadow, Water, Skybox ) etc..
    /// </summary>
    public class SceneRenderState : IGameState
    {
        private MapLoader _map;
        private MapRenderer _mapRenderer;
        protected MapContext _mapContext;
        private Water _water;
        private const float fov = 60;
        QuadObject3D quad = new QuadObject3D();
        SimpleFullscreenMaterial simple = new SimpleFullscreenMaterial();

        public SceneRenderState(string map)
        {
            SkyBox.Init(ResourceManager.Textures["SKY_FRONT"],
                        ResourceManager.Textures["SKY_BACK"],
                        ResourceManager.Textures["SKY_RIGHT"],
                        ResourceManager.Textures["SKY_LEFT"],
                        ResourceManager.Textures["SKY_TOP"],
                        ResourceManager.Textures["SKY_BOTTOM"],
                        1, 1);

            _map = new MapLoader(map);
            _mapContext = _map.MapContext;
            _mapRenderer = new MapRenderer(_mapContext);
            ShadowMapping.Init(2048, 55, 55);
            ParticleRenderer.Init();
            
        }

        public override void Init()
        {
            InitScene();
            _water = ResourceManager.Water;
            _water.InitFrameBuffers(GameManager.Window.Width, GameManager.Window.Height);
            
        }

        public override void HandleInput(FrameEventArgs e, MouseDevice mouse, KeyboardDevice keyboard)
        {

        }

        public override void Update(FrameEventArgs e)
        {
            UpdateEntities(e);
            Camera.Update(e);
            ParticleRenderer.Update(e);
            
        }

        public override void Render(FrameEventArgs e)
        {
            // Für Wasser Clipping Aktivieren
            GL.Enable(EnableCap.ClipDistance0);

            // ** REFLECTION FRAMEBUFFER **
            _water.ReflectionFrameBuffer.Start();
            // Bewegt Kamera nach unten um die Reflection aufzunehmen
            float dist = 2 * (Camera.position.Y - _water.WaterHeight);
            Camera.position.Y -= dist;
            Camera.orientation.Y = -Camera.orientation.Y;

            SkyBox.Draw();
            _mapRenderer.Render(e, new Vector4(0, 1, 0, 0));
            RenderEntities(e);
            ParticleRenderer.Render(e);
            _water.ReflectionFrameBuffer.End();


            // ** REFRACTION FRAMEBUFFER **
            _water.RefractionFrameBuffer.Start();
            // Bewegt Kamera nach oben um die Refraction aufzunehmen
            Camera.position.Y += dist;
            Camera.orientation.Y = -Camera.orientation.Y;

            SkyBox.Draw();
            _mapRenderer.Render(e, new Vector4(0, -1, 0, 0));
            RenderEntities(e);
            ParticleRenderer.Render(e);
            _water.RefractionFrameBuffer.End();

            
            // Shadowmap
            ShadowMapping.StartShadowMapping();
            _mapRenderer.Render(e, new Vector4(0, 1, 0, 0));
            RenderEntities(e);
            ShadowMapping.EndShadowMapping();

            // Eigentliches Rendern der Entities
            if (GameEngine.Bloom) Postprocessing.Start();
            SkyBox.Draw();
            _water.Draw((float)e.Time);
            _mapRenderer.Render(e, new Vector4(0, 1, 0, 0));

            RenderEntities(e);

            // HUD( healtbars ) und Partikel am Ende wegen Blending
            RenderHUD(e); 
            ParticleRenderer.Render(e);
 
        }

        public override void OnResize(int screenWidth, int screenHeight)
        {
            base.OnResize(screenWidth, screenHeight);
            Camera.SetWidthHeightFov(screenWidth, screenHeight, Camera.Fov);
        }


        public virtual void RenderEntities(FrameEventArgs e) { }
        public virtual void RenderHUD(FrameEventArgs e) { }
        public virtual void UpdateEntities(FrameEventArgs e) { }

        private void InitScene()
        {

            // Kamera initialisieren
            Camera.SetWidthHeightFov(GameManager.Window.Width, GameManager.Window.Height, fov, 1, 1000);
 
            Camera.InitPositionRestriction(new Vector3(_mapContext.MapWidth+20, 30, _mapContext.MapHeight+20), new Vector3(-20, 1, -20));
            Camera.SetupFog(1, 333, new Vector3(0.8f, 0.545f, 0.545f));

            // Tiefenpuffer einschalten
            GL.Enable(EnableCap.DepthTest);

            // Licht setzen
            Light.SetDirectionalLight(new Vector3(0.1f, 1.0f, 0.1f), new Vector4(0.1f, 0.1f, 0.1f, 1), new Vector4(0.9f, 0.9f, 0.9f, 0.0f), new Vector4(0.2f, 0.2f, 0.2f, 0.0f));
            Light.lighAttenuation = 50.0f;
            Light.lighPosition = new Vector3(4, 10, 0);

        }
    }
}
