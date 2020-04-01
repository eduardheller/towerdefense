using System.Collections.Generic;
using Engine.cgimin.texture;
using Engine.cgimin.object3d;
using Engine.cgimin.sound;
using Engine.cgimin.water;

namespace TowerDefense
{
    /// <summary>
    /// Lädt alle Resourcen in Listen
    /// </summary>
    class ResourceManager
    {
        private static readonly string ASSET_OBJ_FOLDER = "assets/obj/";
        private static readonly string TEXTURE_FOLDER = "assets/textures/";
        private static readonly string GUI_FOLDER = "assets/gui/";
        private static readonly string SKYBOX_FOLDER = "assets/textures/skybox/";
        private static readonly string SOUND_FOLDER = "assets/sounds/";

        private static Dictionary<string,BaseObject3D> _objects;
        private static Dictionary<string, int> _textures;
        private static Dictionary<string, int> _guis;
        private static Dictionary<string, int> _sounds;
        private static Water _water;

        public static Dictionary<string, BaseObject3D> Objects
        {
            get { return _objects; }
        }

        public static Dictionary<string, int> Textures
        {
            get { return _textures; }
        }

        public static Dictionary<string, int> GUI
        {
            get { return _guis; }
        }

        public static Dictionary<string, int> Sounds
        {
            get { return _sounds; }
        }

        public static Water Water
        {
            get { return _water; }
        }

        public static void LoadResources()
        {
            _objects = new Dictionary<string, BaseObject3D>();
            _textures = new Dictionary<string, int>();
            _guis = new Dictionary<string, int>();
            _sounds = new Dictionary<string, int>();

            _water = new Water(0.05f);
            LoadSoundResources(_sounds);
            LoadTowerResources(_objects, _textures);
            LoadEnemyResources(_objects, _textures);
            LoadProjectileResources(_objects, _textures);
            LoadEnvironmentResources(_objects, _textures);
            LoadMapResources(_objects, _textures);
            LoadGUIResources(_guis);
            LoadSkyBoxResources(_textures);
            LoadTextResources(_textures);

        }



        private static void LoadSoundResources(Dictionary<string, int> sounds)
        {
            sounds.Add("ARROW", Sound.LoadSound(SOUND_FOLDER + "arrow.wav"));
            sounds.Add("ARROW_GROUND", Sound.LoadSound(SOUND_FOLDER + "arrowground.wav"));
            sounds.Add("BUILD", Sound.LoadSound(SOUND_FOLDER + "build.wav"));
            sounds.Add("CANON", Sound.LoadSound(SOUND_FOLDER + "canon.wav"));
            sounds.Add("CANON_GROUND", Sound.LoadSound(SOUND_FOLDER + "canonground.wav"));
            sounds.Add("MUSIC_BATTLE", Sound.LoadSound(SOUND_FOLDER + "musicbattle.wav"));
            sounds.Add("MUSIC_TITLE", Sound.LoadSound(SOUND_FOLDER + "title.wav"));
            sounds.Add("WAVE_OVER", Sound.LoadSound(SOUND_FOLDER + "waveover.wav"));
            sounds.Add("TESLA", Sound.LoadSound(SOUND_FOLDER + "tesla.wav"));
            sounds.Add("MAGE", Sound.LoadSound(SOUND_FOLDER + "freeze.wav"));
            sounds.Add("MAGE_GROUND", Sound.LoadSound(SOUND_FOLDER + "freezeground.wav"));
            sounds.Add("WIN", Sound.LoadSound(SOUND_FOLDER + "win.wav"));
            sounds.Add("SLIME_DEAD", Sound.LoadSound(SOUND_FOLDER + "slimedead.wav"));
            sounds.Add("RABBIT_DEAD", Sound.LoadSound(SOUND_FOLDER + "rabbitdead.wav"));
            sounds.Add("AMBIENT", Sound.LoadSound(SOUND_FOLDER + "ambient.wav"));
            sounds.Add("BAT_DEAD", Sound.LoadSound(SOUND_FOLDER + "batdead.wav"));
            sounds.Add("BOSS_DEAD", Sound.LoadSound(SOUND_FOLDER + "bossdead.wav"));
            sounds.Add("INTRO", Sound.LoadSound(SOUND_FOLDER + "intro.wav"));
            sounds.Add("LIFE_LOST", Sound.LoadSound(SOUND_FOLDER + "lifelost.wav"));
            sounds.Add("BOSS_MUSIC", Sound.LoadSound(SOUND_FOLDER + "boss.wav"));
            sounds.Add("LOST", Sound.LoadSound(SOUND_FOLDER + "lost.wav"));
        }

        private static void LoadEnvironmentResources(Dictionary<string, BaseObject3D> obj, Dictionary<string, int> textures)
        {
            textures.Add("TREE", TextureManager.LoadTexture(TEXTURE_FOLDER + "Bark01.png"));
            textures.Add("LEAVES", TextureManager.LoadTexture(TEXTURE_FOLDER + "plants01.png"));
            textures.Add("ROCK", TextureManager.LoadTexture(TEXTURE_FOLDER + "Rock01.png"));
            obj.Add("TREE_01", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "tree_1.obj", 0.4f, false, true));
            obj.Add("TREE_01_LEAVES", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "tree_1_leaves.obj", 0.4f, false, true));
            obj.Add("TREE_02", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "tree_2.obj", 0.4f, false, true));
            obj.Add("TREE_02_LEAVES", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "tree_2_leaves.obj", 0.4f, false, true));
            obj.Add("TREE_03", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "tree_3.obj", 0.4f, false, true));
            obj.Add("TREE_03_LEAVES", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "tree_3_leaves.obj", 0.4f, false, true));
            obj.Add("PLANT_01", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "plant_1.obj", 0.4f, false, true));
            obj.Add("PLANT_02", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "plant_2.obj", 0.4f, false, true));
            obj.Add("PLANT_03", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "plant_3.obj", 0.4f, false, true));
            obj.Add("PLANT_04", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "plant_4.obj", 0.4f, false, true));
            obj.Add("ROCK_01", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "rock_1.obj", 0.4f, false, true));
            obj.Add("ROCK_02", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "rock_2.obj", 0.4f, false, true));

            obj.Add("SHIP", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "ship.obj", 1555, false, true));
            textures.Add("SHIP", TextureManager.LoadTexture(TEXTURE_FOLDER + "ship.png"));

            obj.Add("BASE", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "base.obj", 0.7f, false, true));
            textures.Add("BASE", TextureManager.LoadTexture(TEXTURE_FOLDER + "mushroom_house_Diffuse.png"));
            obj.Add("BRIDGE", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "bridge.obj", 0.4f, false, true));
            textures.Add("BRIDGE", TextureManager.LoadTexture(TEXTURE_FOLDER + "bridge.png"));
        }

        private static void LoadTowerResources(Dictionary<string, BaseObject3D> obj, Dictionary<string, int> textures)
        {
            obj.Add("TOWER_GROUND_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "base_0.obj", 1, false, true));
            obj.Add("TOWER_GROUND_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "base_1.obj", 1, false, true));
            obj.Add("TOWER_GROUND_3", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "base_3.obj", 1, false, true));
            obj.Add("TOWER_GROUND_4", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "base_4.obj", 1, false, true));
            obj.Add("TOWER_GROUND_5", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "base_5.obj", 1, false, true));
            obj.Add("TOWER_GROUND_6", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "base_6.obj", 1, false, true));
            obj.Add("TOWER_GROUND_7", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "base_7.obj", 1, false, true));
            obj.Add("TOWER_GROUND_8", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "base_8.obj", 1, false, true));

            // Arrow Tower
            obj.Add("TOWER_ARROW_BASE_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "arrow_base_0.obj", 1, false, true));
            obj.Add("TOWER_ARROW_HUB_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "arrow_hub_0.obj", 1, false, true));
            obj.Add("TOWER_ARROW_TURRET_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "arrow_turret_0.obj", 1, false, true));
            obj.Add("TOWER_ARROW_BASE_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "arrow_base_1.obj", 1, false, true));
            obj.Add("TOWER_ARROW_HUB_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "arrow_hub_1.obj", 1, false, true));
            obj.Add("TOWER_ARROW_TURRET_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "arrow_turret_1.obj", 1, false, true));
            obj.Add("TOWER_ARROW_BASE_3", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "arrow_base_2.obj", 1, false, true));
            obj.Add("TOWER_ARROW_HUB_3", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "arrow_hub_2.obj", 1, false, true));
            obj.Add("TOWER_ARROW_TURRET_3", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "arrow_turret_2.obj", 1, false, true));

            // Canon Tower
            obj.Add("TOWER_CANON_BASE_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "canon_base_0.obj", 1, false, true));
            obj.Add("TOWER_CANON_HUB_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "canon_hub_0.obj", 1, false, true));
            obj.Add("TOWER_CANON_TURRET_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "canon_turret_0.obj", 1, false, true));
            obj.Add("TOWER_CANON_BASE_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "canon_base_1.obj", 1, false, true));
            obj.Add("TOWER_CANON_HUB_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "canon_hub_1.obj", 1, false, true));
            obj.Add("TOWER_CANON_TURRET_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "canon_turret_1.obj", 1, false, true));
            obj.Add("TOWER_CANON_BASE_3", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "canon_base_2.obj", 1, false, true));
            obj.Add("TOWER_CANON_HUB_3", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "canon_hub_2.obj", 1, false, true));
            obj.Add("TOWER_CANON_TURRET_3", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "canon_turret_2.obj", 1, false, true));

            // Mage Tower
            obj.Add("TOWER_MAGE_BASE_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "mage_base_0.obj", 1, false, true));
            obj.Add("TOWER_MAGE_BASE_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "mage_base_1.obj", 1, false, true));
            obj.Add("TOWER_MAGE_BASE_3", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "mage_base_2.obj", 1, false, true));
            obj.Add("TOWER_MAGE_TURRET_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "mage_turret.obj", 1, false, true));

            // Tesla Tower
            obj.Add("TOWER_TESLA_TURRET_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "tesla_turret_0.obj", 1, false, true));
            obj.Add("TOWER_TESLA_TURRET_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "tesla_turret_1.obj", 1, false, true));
            obj.Add("TOWER_TESLA_TURRET_3", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "tesla_turret_2.obj", 1, false, true));


            textures.Add("TOWER_BASE_2", TextureManager.LoadTexture(TEXTURE_FOLDER + "Tower_A.png"));
            textures.Add("TOWER_BASE_1", TextureManager.LoadTexture(TEXTURE_FOLDER + "Tower_B.png"));
            textures.Add("TOWER_MAGE", TextureManager.LoadTexture(TEXTURE_FOLDER + "Crystal.png"));
            textures.Add("TOWER_CANON", TextureManager.LoadTexture(TEXTURE_FOLDER + "Cannon.png"));
            textures.Add("TOWER_ARROW", TextureManager.LoadTexture(TEXTURE_FOLDER + "CrossBow.png"));
            textures.Add("TOWER_TESLA", TextureManager.LoadTexture(TEXTURE_FOLDER + "TeslaCoil.png"));
            textures.Add("RADIUS", TextureManager.LoadTexture(TEXTURE_FOLDER + "radius.png"));


        }

        private static void LoadEnemyResources(Dictionary<string, BaseObject3D> obj, Dictionary<string, int> textures)
        {
            obj.Add("ENEMY_1",new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "slime.obj", 1, false, true));
            textures.Add("ENEMY_1", TextureManager.LoadTexture(TEXTURE_FOLDER + "SlimeGreen.jpg"));
            obj.Add("ENEMY_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "rabbit.obj", 1, false, true));
            textures.Add("ENEMY_2", TextureManager.LoadTexture(TEXTURE_FOLDER + "Rabbit_Green.jpg"));
            obj.Add("ENEMY_3", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "bat.obj", 1, false, true));
            textures.Add("ENEMY_3", TextureManager.LoadTexture(TEXTURE_FOLDER + "Bat_Color2.jpg"));

            textures.Add("ENEMY_1_2", TextureManager.LoadTexture(TEXTURE_FOLDER + "SlimeYellow.jpg"));
            textures.Add("ENEMY_1_3", TextureManager.LoadTexture(TEXTURE_FOLDER + "SlimeBlue.jpg"));
            textures.Add("ENEMY_1_4", TextureManager.LoadTexture(TEXTURE_FOLDER + "SlimeRed.jpg"));

            textures.Add("ENEMY_2_2", TextureManager.LoadTexture(TEXTURE_FOLDER + "Rabbit_Yellow.jpg"));
            textures.Add("ENEMY_2_3", TextureManager.LoadTexture(TEXTURE_FOLDER + "Rabbit_Cyan.jpg"));
            textures.Add("ENEMY_2_4", TextureManager.LoadTexture(TEXTURE_FOLDER + "Rabbit_Red.jpg"));

            textures.Add("ENEMY_3_2", TextureManager.LoadTexture(TEXTURE_FOLDER + "Bat_Color3.jpg"));
            textures.Add("ENEMY_3_3", TextureManager.LoadTexture(TEXTURE_FOLDER + "Bat_Color4.jpg"));
            textures.Add("ENEMY_3_4", TextureManager.LoadTexture(TEXTURE_FOLDER + "Bat_Color.jpg"));

            obj.Add("BOSS_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "boss.obj", 1f, false, true));
            textures.Add("BOSS_1", TextureManager.LoadTexture(TEXTURE_FOLDER + "boss.png"));

        }

        private static void LoadProjectileResources(Dictionary<string, BaseObject3D> obj, Dictionary<string, int> textures)
        {
            obj.Add("PROJECTILE_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "ball.obj", 1, false, true));
            obj.Add("PROJECTILE_2", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "arrow.obj", 2, false, true));
            textures.Add("PROJECTILE_1", TextureManager.LoadTexture(TEXTURE_FOLDER + "cannonball_diff.png"));
            textures.Add("PROJECTILE_2", TextureManager.LoadTexture(TEXTURE_FOLDER + "iceball.jpg"));
            textures.Add("PROJECTILE_3", TextureManager.LoadTexture(TEXTURE_FOLDER + "Arrow00_diff.png"));
            textures.Add("PROJECTILE_NORMAL_1", TextureManager.LoadTexture(TEXTURE_FOLDER + "n1.jpg"));
            textures.Add("PARTICLE_ATLAS_1", TextureManager.LoadTexture(TEXTURE_FOLDER + "ParticleAtlas.png"));
            textures.Add("PARTICLE_ATLAS_2", TextureManager.LoadTexture(TEXTURE_FOLDER + "Lightning01.png"));
            textures.Add("PARTICLE_ATLAS_3", TextureManager.LoadTexture(TEXTURE_FOLDER + "Sparkles01.png"));
            textures.Add("PARTICLE_ATLAS_4", TextureManager.LoadTexture(TEXTURE_FOLDER + "Fire.png"));
            textures.Add("PARTICLE_ATLAS_5", TextureManager.LoadTexture(TEXTURE_FOLDER + "Smoke.png"));
            textures.Add("PARTICLE_ATLAS_6", TextureManager.LoadTexture(TEXTURE_FOLDER + "Smoke02.png"));
            textures.Add("PARTICLE_ATLAS_7", TextureManager.LoadTexture(TEXTURE_FOLDER + "Explosion7.png"));
            textures.Add("PARTICLE_ATLAS_8", TextureManager.LoadTexture(TEXTURE_FOLDER + "Explosion5.png"));
            textures.Add("PARTICLE_ATLAS_9", TextureManager.LoadTexture(TEXTURE_FOLDER + "light.png"));
            textures.Add("PARTICLE_ATLAS_10", TextureManager.LoadTexture(TEXTURE_FOLDER + "ice.png"));
            textures.Add("PARTICLE_ATLAS_11", TextureManager.LoadTexture(TEXTURE_FOLDER + "blood_hit_05.png"));
        }

        private static void LoadMapResources(Dictionary<string, BaseObject3D> obj, Dictionary<string, int> textures)
        {
            obj.Add("BLOCK_1", new ObjLoaderObject3D(ASSET_OBJ_FOLDER + "cube.obj", 1, false, true));
            textures.Add("BLOCK_1", TextureManager.LoadTexture(TEXTURE_FOLDER + "Ground01.png"));
            textures.Add("BLOCK_NORMAL_1", TextureManager.LoadTexture(TEXTURE_FOLDER + "Ground02_normals.png"));
            textures.Add("BLOCK_WAY_1", TextureManager.LoadTexture(TEXTURE_FOLDER + "Ground02.png"));
            textures.Add("BLOCK_WAY_NORMAL_1", TextureManager.LoadTexture(TEXTURE_FOLDER + "Ground03_normals.png"));


        }


        private static void LoadTextResources(Dictionary<string, int> textures)
        {
            textures.Add("TEXT_ATLAS_1", TextureManager.LoadTexture("assets/gui/inc.png"));
        }

        private static void LoadGUIResources(Dictionary<string, int> gui)
        {

            gui.Add("HEALTHBAR", TextureManager.LoadTexture(GUI_FOLDER + "health.png"));
            gui.Add("HEALTHBAR_BORDER", TextureManager.LoadTexture(GUI_FOLDER + "healthborder.png"));

            gui.Add("BUTTON_EXIT", TextureManager.LoadTexture(GUI_FOLDER + "exit.png"));
            gui.Add("BUTTON_TOWER_CANON", TextureManager.LoadTexture(GUI_FOLDER + "tower_canon_avalible.png"));
            gui.Add("BUTTON_TOWER_CANON_DISABLED", TextureManager.LoadTexture(GUI_FOLDER + "tower_canon_unavalible.png"));
            gui.Add("BUTTON_TOWER_CANON_OVER", TextureManager.LoadTexture(GUI_FOLDER + "tower_canon_over.png"));
            gui.Add("BUTTON_TOWER_MAGE", TextureManager.LoadTexture(GUI_FOLDER + "tower_mage_avalible.png"));
            gui.Add("BUTTON_TOWER_MAGE_DISABLED", TextureManager.LoadTexture(GUI_FOLDER + "tower_mage_unavalible.png"));
            gui.Add("BUTTON_TOWER_MAGE_OVER", TextureManager.LoadTexture(GUI_FOLDER + "tower_mage_over.png"));
            gui.Add("BUTTON_TOWER_ARROW", TextureManager.LoadTexture(GUI_FOLDER + "tower_arrow_avalible.png"));
            gui.Add("BUTTON_TOWER_ARROW_DISABLED", TextureManager.LoadTexture(GUI_FOLDER + "tower_arrow_unavalible.png"));
            gui.Add("BUTTON_TOWER_ARROW_OVER", TextureManager.LoadTexture(GUI_FOLDER + "tower_arrow_over.png"));
            gui.Add("BUTTON_TOWER_TESLA", TextureManager.LoadTexture(GUI_FOLDER + "tower_tesla_avalible.png"));
            gui.Add("BUTTON_TOWER_TESLA_DISABLED", TextureManager.LoadTexture(GUI_FOLDER + "tower_tesla_unavalible.png"));
            gui.Add("BUTTON_TOWER_TESLA_OVER", TextureManager.LoadTexture(GUI_FOLDER + "tower_tesla_over.png"));

            gui.Add("WINDOW_CanonTower", TextureManager.LoadTexture(GUI_FOLDER + "window_canon_0.png"));
            gui.Add("WINDOW_CanonTower_OVER", TextureManager.LoadTexture(GUI_FOLDER + "window_canon_1.png"));
            gui.Add("WINDOW_MageTower", TextureManager.LoadTexture(GUI_FOLDER + "window_mage_0.png"));
            gui.Add("WINDOW_MageTower_OVER", TextureManager.LoadTexture(GUI_FOLDER + "window_mage_1.png"));
            gui.Add("WINDOW_ArrowTower", TextureManager.LoadTexture(GUI_FOLDER + "window_arrow_0.png"));
            gui.Add("WINDOW_ArrowTower_OVER", TextureManager.LoadTexture(GUI_FOLDER + "window_arrow_1.png"));
            gui.Add("WINDOW_TeslaTower", TextureManager.LoadTexture(GUI_FOLDER + "window_tesla_0.png"));
            gui.Add("WINDOW_TeslaTower_OVER", TextureManager.LoadTexture(GUI_FOLDER + "window_tesla_1.png"));

            gui.Add("OVERLAY_1", TextureManager.LoadTexture(GUI_FOLDER + "button_wave.png"));
            gui.Add("INFO", TextureManager.LoadTexture(GUI_FOLDER + "info.png"));

            gui.Add("BUTTON_BLOOM", TextureManager.LoadTexture(GUI_FOLDER + "bloom_on.png"));
            gui.Add("BUTTON_BLOOM_OFF", TextureManager.LoadTexture(GUI_FOLDER + "bloom_off.png"));

            gui.Add("BUTTON_RESTART", TextureManager.LoadTexture(GUI_FOLDER + "restart.png"));

            gui.Add("BUTTON_SELL", TextureManager.LoadTexture(GUI_FOLDER + "sell.png"));
            gui.Add("BUTTON_UPGRADE", TextureManager.LoadTexture(GUI_FOLDER + "upgrade_enabled.png"));
            gui.Add("BUTTON_UPGRADE_DISABLED", TextureManager.LoadTexture(GUI_FOLDER + "upgrade_disabled.png"));
        }

        private static void LoadSkyBoxResources(Dictionary<string, int> textures)
        {
            textures.Add("SKY_FRONT", TextureManager.LoadTexture(SKYBOX_FOLDER + "SkyNoon_Front.png",true));
            textures.Add("SKY_BACK", TextureManager.LoadTexture(SKYBOX_FOLDER + "SkyNoon_Back.png", true));
            textures.Add("SKY_RIGHT", TextureManager.LoadTexture(SKYBOX_FOLDER + "SkyNoon_Right.png", true));
            textures.Add("SKY_LEFT", TextureManager.LoadTexture(SKYBOX_FOLDER + "SkyNoon_Left.png", true));
            textures.Add("SKY_TOP", TextureManager.LoadTexture(SKYBOX_FOLDER + "SkyNoon_Top.png", true));
            textures.Add("SKY_BOTTOM", TextureManager.LoadTexture(SKYBOX_FOLDER + "SkyNoon_Bottom.png", true));
        }
    }
}
