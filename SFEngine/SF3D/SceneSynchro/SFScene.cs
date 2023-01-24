/*
 * SFScene holds all visual data (except resources) and is able to manipulate it
 * CatElemToScene generates scene description based on provided game data element, useful for asset viewer
 */

using OpenTK;
using SFEngine.SFCFF;
using SFEngine.SFLua;
using SFEngine.SFLua.lua_sql;
using SFEngine.SFResources;
using System;
using System.Collections.Generic;

namespace SFEngine.SF3D.SceneSynchro
{
    public class SFSceneMeta
    {
        public float duration = 0f;
        public bool is_animated = false;
        public string name = "";
    }

    public class SFDecalInfo
    {
        public Vector2 center;
        public Vector2 offset;
        public int angle;
        public SFMap.SFCoord topleft;
        public SFMap.SFCoord bottomright;
        public SceneNodeSimple decal_node;
    }

    public class SFScene
    {
        // TODO: multiple scenes (at least one for map editor and one for asset viewer
        public SFSceneMeta scene_meta { get; private set; } = new SFSceneMeta();    // scene metadata

        public int frame_counter = 0;      // total frames rendered
        public int frames_per_second { get; private set; } = Settings.FramesPerSecond;   // framerate
        public System.Diagnostics.Stopwatch delta_timer { get; private set; } = new System.Diagnostics.Stopwatch();     // timer which manages delta time
        public float deltatime = 0f;       // current delta time value in seconds
        public float DeltaTime
        {
            get
            {
                if (time_flowing)
                {
                    return deltatime;
                }

                return 0f;
            }
        }
        public float current_time { get; private set; } = 0f;        // current scene time in seconds
        private bool time_flowing = false;

        public SceneNode root;   // tree with all objects, visible or not
        public SFMap.SFMap map = null;  // contains heightmap and lakes
        public SceneNodeCamera camera;  // temporarily not connected to root

        public SceneNode selected_node;    // selected node is rendered differently

        public Atmosphere atmosphere { get; private set; }

        public HashSet<SFModel3D> model_set_simple = new HashSet<SFModel3D>();

        public HashSet<SFSubModel3D> opaque_pass_models = new HashSet<SFSubModel3D>();
        public HashSet<SFSubModel3D> transparent_pass_models = new HashSet<SFSubModel3D>();
        public HashSet<SFSubModel3D> water_pass_models = new HashSet<SFSubModel3D>();
        public HashSet<SFSubModel3D> additive_pass_models = new HashSet<SFSubModel3D>();

        public HashSet<SceneNodeAnimated> an_primary_nodes = new HashSet<SceneNodeAnimated>();

        public LinearPool<SFDecalInfo> decal_info = new LinearPool<SFDecalInfo>();

        public SFModel3D missing_node_mesh;

        public void Init()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFScene.Init() called");

            root = new SceneNode("Root");
            camera = new SceneNodeCamera("Camera");

            atmosphere = new Atmosphere();

            // setup lighting
            if (atmosphere.altitude_ambient_color == null)
            {
                if (Settings.ToneMapping)
                {
                    atmosphere.altitude_ambient_color = new InterpolatedColor(6);   // ambient color is sky color
                    atmosphere.altitude_ambient_strength = new InterpolatedFloat(5);
                    atmosphere.altitude_sun_color = new InterpolatedColor(7);
                    atmosphere.altitude_sun_strength = new InterpolatedFloat(5);
                    atmosphere.altitude_fog_color = new InterpolatedColor(6);
                    atmosphere.altitude_fog_strength = new InterpolatedFloat(6);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.03f, 0.03f, 0.20f, 1.0f), 0);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.03f, 0.03f, 0.20f, 1.0f), 80);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.63f, 0.32f, 0.30f, 1.0f), 90);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.73f, 0.61f, 0.80f, 1.0f), 100);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.30f, 0.56f, 0.85f, 1.0f), 110);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.30f, 0.56f, 0.85f, 1.0f), 180);
                    atmosphere.altitude_ambient_strength.Add(0.2f, 0);
                    atmosphere.altitude_ambient_strength.Add(0.2f, 80);
                    atmosphere.altitude_ambient_strength.Add(0.3f, 90);
                    atmosphere.altitude_ambient_strength.Add(0.4f, 110);
                    atmosphere.altitude_ambient_strength.Add(0.4f, 180);
                    atmosphere.altitude_sun_color.Add(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 0);
                    atmosphere.altitude_sun_color.Add(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 80);
                    atmosphere.altitude_sun_color.Add(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 89);
                    atmosphere.altitude_sun_color.Add(new Vector4(1.0f, 0.6f, 0.2f, 1.0f), 90);
                    atmosphere.altitude_sun_color.Add(new Vector4(1.0f, 0.9f, 0.7f, 1.0f), 100);
                    atmosphere.altitude_sun_color.Add(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 110);
                    atmosphere.altitude_sun_color.Add(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 180);
                    atmosphere.altitude_sun_strength.Add(0.4f, 0);
                    atmosphere.altitude_sun_strength.Add(0.4f, 80);
                    atmosphere.altitude_sun_strength.Add(0.0f, 90);
                    atmosphere.altitude_sun_strength.Add(1.5f, 110);
                    atmosphere.altitude_sun_strength.Add(1.5f, 180);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.03f, 0.03f, 0.20f, 1.0f), 0);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.03f, 0.03f, 0.20f, 1.0f), 80);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.93f, 0.42f, 0.40f, 1.0f), 90);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.73f, 0.61f, 0.80f, 1.0f), 100);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.50f, 0.68f, 0.95f, 1.0f), 110);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.50f, 0.68f, 0.95f, 1.0f), 180);
                    atmosphere.altitude_fog_strength.Add(1.6f, 0);
                    atmosphere.altitude_fog_strength.Add(1.6f, 80);
                    atmosphere.altitude_fog_strength.Add(1.6f, 90);
                    atmosphere.altitude_fog_strength.Add(1.6f, 100);
                    atmosphere.altitude_fog_strength.Add(1.6f, 110);
                    atmosphere.altitude_fog_strength.Add(1.6f, 180);
                }
                else
                {
                    atmosphere.altitude_ambient_color = new InterpolatedColor(6);   // ambient color is sky color
                    atmosphere.altitude_ambient_strength = new InterpolatedFloat(1);
                    atmosphere.altitude_sun_color = new InterpolatedColor(8);
                    atmosphere.altitude_sun_strength = new InterpolatedFloat(1);
                    atmosphere.altitude_fog_color = new InterpolatedColor(6);
                    atmosphere.altitude_fog_strength = new InterpolatedFloat(1);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.35f, 0.25f, 0.45f, 1.0f), 0);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.35f, 0.25f, 0.45f, 1.0f), 80);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.3f, 0.2f, 0.5f, 1.0f), 90);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.45f, 0.40f, 0.72f, 1.0f), 100);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.58f, 0.80f, 0.96f, 1.0f), 110);
                    atmosphere.altitude_ambient_color.Add(new Vector4(0.58f, 0.80f, 0.96f, 1.0f), 180);
                    atmosphere.altitude_ambient_strength.Add(0.7f, 0);
                    atmosphere.altitude_sun_color.Add(new Vector4(0.30f, 0.40f, 0.64f, 1), 0);
                    atmosphere.altitude_sun_color.Add(new Vector4(0.30f, 0.40f, 0.64f, 1), 70);
                    atmosphere.altitude_sun_color.Add(new Vector4(0.29f, 0.16f, 0.32f, 1), 80);
                    atmosphere.altitude_sun_color.Add(new Vector4(0.23f, 0, 0.0f, 1), 89);
                    atmosphere.altitude_sun_color.Add(new Vector4(0.8f, 0, 0, 1), 90);
                    atmosphere.altitude_sun_color.Add(new Vector4(0.9f, 0.5f, 0.2f, 1), 100);
                    atmosphere.altitude_sun_color.Add(new Vector4(1, 1, 1, 1), 110);
                    atmosphere.altitude_sun_color.Add(new Vector4(1, 1, 1, 1), 180);
                    atmosphere.altitude_sun_strength.Add(1.2f, 0);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.35f, 0.25f, 0.45f, 1.0f), 0);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.35f, 0.25f, 0.45f, 1.0f), 80);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.3f, 0.2f, 0.5f, 1.0f), 90);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.45f, 0.40f, 0.72f, 1.0f), 100);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.58f, 0.80f, 0.96f, 1.0f), 110);
                    atmosphere.altitude_fog_color.Add(new Vector4(0.58f, 0.80f, 0.96f, 1.0f), 180);
                    atmosphere.altitude_fog_strength.Add(1.0f, 0);
                }
            }

            delta_timer.Start();
        }

        public void GenerateMissingMesh()
        {
            if (missing_node_mesh != null)
            {
                return;
            }

            // model displayed when a node is missing a mesh
            Vector3[] vertices = new Vector3[8];
            Vector2[] uvs = new Vector2[8];
            byte[] colors = new byte[32];
            Vector3[] normals = new Vector3[8];

            uint[] indices;

            // generate mouse cursor selected position gizmo
            missing_node_mesh = new SFModel3D();

            float s = 0.5f;
            float h = 1.5f;   // gizmo height
            vertices[0] = new Vector3(-s, -s, -s + h);
            vertices[1] = new Vector3(s, -s, -s + h);
            vertices[2] = new Vector3(-s, -s, s + h);
            vertices[3] = new Vector3(s, -s, s + h);
            vertices[4] = new Vector3(-s, s, -s + h);
            vertices[5] = new Vector3(s, s, -s + h);
            vertices[6] = new Vector3(-s, s, s + h);
            vertices[7] = new Vector3(s, s, s + h);
            for (int i = 0; i < 8; i++)
            {
                colors[4 * i + 0] = 0x7F;
                colors[4 * i + 1] = 0x7F;
                colors[4 * i + 2] = 0xCF;
                colors[4 * i + 3] = 0xFF;
                normals[i] = (vertices[i] - new Vector3(0, 0, h)).Normalized();
            }
            // color two vertices differently to annotate angle
            colors[8] = 0xCF;
            colors[9] = 0x7F;
            colors[10] = 0x7F;
            colors[12] = 0xCF;
            colors[13] = 0x7F;
            colors[14] = 0x7F;

            indices = new uint[] { 0, 2, 1,   1, 2, 3,   4, 5, 6,   5, 7, 6,
                                   0, 4, 1,   1, 4, 5,   1, 3, 5,   3, 7, 5,
                                   3, 2, 7,   2, 6, 7,   2, 0, 6,   0, 4, 6};

            SF3D.SFSubModel3D sbm2 = new SF3D.SFSubModel3D();
            sbm2.CreateRaw(vertices, uvs, colors, normals, indices, null);
            sbm2.material.apply_shading = true;
            sbm2.material.apply_shadow = true;
            sbm2.material.casts_shadow = true;
            sbm2.material.distance_fade = true;
            sbm2.material.emission_strength = 0.1f;
            sbm2.material.emission_color = new Vector4(0.6f, 0.6f, 1.0f, 1.0f);
            sbm2.material.transparent_pass = false;

            missing_node_mesh.CreateRaw(new SF3D.SFSubModel3D[] { sbm2 });
            SFResourceManager.Models.AddManually(missing_node_mesh, "_MISSING_MESH_");
        }


        // generates a scene given gamedata element
        public void CatElemToScene(int category, int element)
        {
            scene_meta.is_animated = false;
            switch (category)
            {
                case 2003:
                case 2004:
                case 2013:
                case 2015:
                case 2017:
                case 2014:
                case 2012:
                case 2018:
                    //get item id
                    int item_id = SFCategoryManager.gamedata[category].GetElementID(element);
                    if (item_id == Utility.NO_INDEX)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Item not found (category is " + category.ToString()
                            + ", element is " + element.ToString() + ")");
                        break;
                    }

                    //find item mesh
                    string m_name = SFLuaEnvironment.GetItemMesh(item_id, false);
                    if (m_name == "")
                    {
                        LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Item mesh not found (item id = " + item_id.ToString() + ")");
                        break;
                    }

                    //create scene
                    SceneNodeSimple item_node = AddSceneNodeSimple(root, m_name, "item");
                    item_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                    scene_meta.name = SFCategoryManager.GetItemName((ushort)item_id);

                    break;
                case 2029:
                case 2030:
                case 2031:
                    //get building id
                    int building_id = SFCategoryManager.gamedata[category].GetElementID(element);
                    if (building_id == Utility.NO_INDEX)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Building not found (category is " + category.ToString()
                            + ", element is " + element.ToString() + ")");
                        break;
                    }
                    SceneNode building_node = AddSceneBuilding(building_id, "building");
                    building_node.SetParent(root);
                    building_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                    scene_meta.name = SFCategoryManager.GetBuildingName((ushort)building_id);

                    break;
                case 2050:
                case 2057:
                case 2065:
                    //get object id
                    int object_id = SFCategoryManager.gamedata[category].GetElementID(element);
                    if (object_id == Utility.NO_INDEX)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Object not found (category is " + category.ToString()
                            + ", element is " + element.ToString() + ")");
                        break;
                    }
                    SceneNode object_node = AddSceneObject(object_id, "object", true, true);
                    object_node.SetParent(root);
                    object_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                    scene_meta.name = SFCategoryManager.GetObjectName((ushort)object_id);

                    break;
                case 2024:
                case 2025:
                case 2026:
                case 2028:
                case 2040:
                case 2001:
                    //get unit id
                    int unit_id = SFCategoryManager.gamedata[category].GetElementID(element);
                    if (unit_id == Utility.NO_INDEX)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Unit not found (category is " + category.ToString()
                            + ", element is " + element.ToString() + ")");
                        break;
                    }
                    SceneNode unit_node = AddSceneUnit(unit_id, "unit");
                    unit_node.SetParent(root);
                    unit_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                    scene_meta.name = SFCategoryManager.GetUnitName((ushort)unit_id);

                    scene_meta.is_animated = true;
                    break;
                default:
                    break;
            }
        }

        // these functions add basic node types to scene
        public SceneNode AddSceneNodeEmpty(SceneNode parent, string new_node_name)
        {
            SceneNode new_node = new SceneNode(new_node_name);
            new_node.SetParent(parent);

            return new_node;
        }

        public SceneNodeSimple AddSceneNodeSimple(SceneNode parent, string model_name, string new_node_name)
        {
            SceneNodeSimple new_node = new SceneNodeSimple(new_node_name);
            new_node.SetParent(parent);

            bool loaded_model = SFResourceManager.LoadModel(model_name);
            if (loaded_model)
            {
                new_node.Mesh = SFResourceManager.Models.Get(model_name);
            }

            if (new_node.Mesh == null)
            {
                SFResourceManager.LoadModel("_MISSING_MESH_");
                new_node.Mesh = SFResourceManager.Models.Get("_MISSING_MESH_");
            }
            return new_node;
        }

        public SceneNodeAnimated AddSceneNodeAnimated(SceneNode parent, string skin_name, string new_node_name, bool primary)
        {
            SceneNodeAnimated new_node = new SceneNodeAnimated(new_node_name);
            if(primary)
            {
                new_node.Primary = true;
                new_node.DrivenNodes = new List<SceneNodeAnimated>();
                new_node.DrivenNodes.Add(new_node);
            }
            new_node.SetParent(parent);

            bool loaded_skin = SFResourceManager.LoadSkeleton(skin_name);
            if (loaded_skin)
            {
                loaded_skin = SFResourceManager.LoadSkin(skin_name);
            }

            if (loaded_skin)
            {
                if(primary)
                {
                    new_node.SetSkeleton(SFResourceManager.Skeletons.Get(skin_name));
                }
                new_node.SetSkin(SFResourceManager.Skins.Get(skin_name));
            }

            loaded_skin = SFResourceManager.LoadModel(skin_name);
            if (loaded_skin)
            {
                new_node.Mesh = SFResourceManager.Models.Get(skin_name);
            }

            return new_node;
        }

        public SceneNodeBone AddSceneNodeBone(SceneNodeAnimated parent, string bone_name, string new_node_name)
        {
            SceneNodeBone new_node = new SceneNodeBone(new_node_name);
            new_node.SetParent(parent);

            new_node.SetBone(bone_name);

            return new_node;
        }

        // these functions create and return nodes which are used to display game elements (units, buildings,...)
        // note that those don't assign parent to the nodes - it's done somewhere else
        public SceneNode AddSceneUnit(int unit_id, string object_name)
        {
            SceneNode unit_node = AddSceneNodeEmpty(null, object_name);    // parent to be assigned later, likely some of the cached mapchunk nodes

            //find unit data element (cat 18)
            if(SFCategoryManager.gamedata[2024] == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): There is no unit data block in gamedata!");
                return unit_node;
            }
            SFCategoryElement unit_data = SFCategoryManager.gamedata[2024].FindElementBinary<UInt16>(0, (UInt16)unit_id);
            if (unit_data == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Unit does not exist (unit id = "
                    + unit_id + ")");
                return unit_node;
            }

            //get unit gender
            SFCategoryElement unit_stats;
            if (SFCategoryManager.gamedata[2005] == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): There is no unit stats data block in gamedata, setting gender to male");
                unit_stats = null;
            }
            else
            {
                unit_stats = SFCategoryManager.gamedata[2005].FindElementBinary<UInt16>(0, (UInt16)unit_data[2]);
            }
            bool is_female = false;
            if (unit_stats != null)
            {
                is_female = ((Byte)unit_stats[21] % 2) == 1;
            }

            //get chest item (2) (animated)
            UInt16 chest_id = SFCategoryManager.GetUnitItem((UInt16)unit_id, 2);
            if (chest_id == 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Unit does not have chestpiece assigned (unit id = "
                    + unit_id + ")");
                return unit_node;
            }
            //find chest skin/animations
            SFLuaSQLItemData chest_data = SFLuaEnvironment.items[chest_id];
            if (chest_data == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Undefined chestpiece mesh (unit id = "
                    + unit_id + ")");
                return unit_node;
            }
            string chest_name = SFLuaEnvironment.GetItemMesh(chest_id, is_female);
            string anim_name = chest_data.AnimSet;
            if (anim_name == "")
            {
                anim_name = "figure_hero";
            }

            //special case: monument unit, needs to be considered separately
            string unit_handle = unit_data[10].ToString();
            if ((unit_handle.StartsWith("Unit")) && (!unit_handle.Contains("Titan")))
            {
                chest_name += "_cold";
            }

            //add anim model to scene
            SceneNodeAnimated uo = AddSceneNodeAnimated(unit_node, chest_name, "Chest", true);
            SceneNodeAnimated uo2;

            //get legs item (5) (animated)
            UInt16 legs_id = SFCategoryManager.GetUnitItem((UInt16)unit_id, 5);
            if (legs_id != 0)
            {
                //find chest skin/animations
                string legs_name = SFLuaEnvironment.GetItemMesh(legs_id, is_female);
                if (legs_name != "")
                {
                    uo2 = AddSceneNodeAnimated(null, legs_name, "Legs", false);
                    uo.DrivenNodes.Add(uo2);
                }
            }
            //special case: anim_name is of "figure_hero": need to also add human head (animated)
            if ((anim_name == "figure_hero") && (unit_stats != null))
            {
                int head_id = (UInt16)unit_stats[22];
                SFLuaSQLHeadData head_data = SFLuaEnvironment.heads[head_id];
                if (head_data == null)
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Unit head has undefined mesh (unit id = "
                        + unit_id.ToString() + ", head id = " + head_id.ToString() + ")");
                }
                else
                {
                    string head_name = "";
                    if (is_female)
                    {
                        head_name = head_data.MeshFemale;
                    }
                    else
                    {
                        head_name = head_data.MeshMale;
                    }

                    uo2 = AddSceneNodeAnimated(null, head_name, "Head", false);
                    uo.DrivenNodes.Add(uo2);
                }
            }

            //get helmet item (0) (boneanchor(Head), simple)
            UInt16 helmet_id = SFCategoryManager.GetUnitItem((UInt16)unit_id, 0);
            if (helmet_id != 0)
            {
                //find item mesh
                string helmet_name = SFLuaEnvironment.GetItemMesh(helmet_id, is_female);
                if (helmet_name != "")
                {
                    //create bone attachment
                    SceneNodeBone bo = AddSceneNodeBone(unit_node.FindNode<SceneNodeAnimated>("Chest"), "Head", "Headbone");
                    SceneNodeSimple ho = AddSceneNodeSimple(bo, helmet_name, "Helmet");
                }
            }

            //get right hand (1) (boneanchor(R Hand weapon), simple)
            UInt16 rhand_id = SFCategoryManager.GetUnitItem((UInt16)unit_id, 1);
            if (rhand_id != 0)
            {
                //find item mesh
                string rhand_name = SFLuaEnvironment.GetItemMesh(rhand_id, is_female);
                if (rhand_name != "")
                {
                    //create bone attachment
                    SceneNodeBone bo = AddSceneNodeBone(unit_node.FindNode<SceneNodeAnimated>("Chest"), "R Hand weapon", "Rhandbone");
                    SceneNodeSimple ho = AddSceneNodeSimple(bo, rhand_name, "Rhand");
                }
            }

            //get left hand (3) (boneanchor(L Hand weapon OR Forearm shield), simple)
            UInt16 lhand_id = SFCategoryManager.GetUnitItem((UInt16)unit_id, 3);
            if (lhand_id != 0)
            {
                //find item mesh
                string lhand_name = SFLuaEnvironment.GetItemMesh(lhand_id, is_female);
                if (lhand_name != "")
                {
                    bool is_shield = false;
                    //check if it's a shield (type 9)
                    SFCategoryElement item_data = SFCategoryManager.gamedata[2003].FindElementBinary<UInt16>(0, lhand_id);
                    if (item_data != null)
                    {
                        int item_type = (Byte)item_data[2];
                        is_shield = item_type == 9;
                    }
                    //create bone attachment
                    SceneNodeBone bo = AddSceneNodeBone(unit_node.FindNode<SceneNodeAnimated>("Chest"), (is_shield ? "L Forearm shield" : "L Hand weapon"), "Lhandbone");
                    SceneNodeSimple ho = AddSceneNodeSimple(bo, lhand_name, "Lhand");
                }
            }

            return unit_node;
        }

        public SceneNode AddSceneObject(int object_id, string object_name, bool apply_shading, bool casts_shadow)
        {
            // create root
            SceneNode obj_node = AddSceneNodeEmpty(null, object_name);

            SFLuaSQLObjectData obj_data = SFLuaEnvironment.objects[object_id];
            List<string> m_lst;
            if (obj_data == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneObject(): Can't find object data (object id = "
                    + object_id + ")");
                m_lst = new List<string>() { "_MISSING_MESH_" };
            }
            else
            {
                m_lst = obj_data.Mesh;
            }

            // add meshes
            for (int i = 0; i < m_lst.Count; i++)
            {
                string m = m_lst[i];
                if (m == "")
                {
                    m = "_MISSING_MESH_";
                }
                //continue;
                SceneNodeSimple n = AddSceneNodeSimple(obj_node, m, i.ToString());

                if (n.Mesh != null)
                {
                    bool decal = m.Contains("decal");
                    if (decal)
                    {
                        n.IsDecal = true;
                        n.Mesh.submodels[0].material.apply_shading = true;
                        n.Mesh.submodels[0].material.transparent_pass = true;
                        n.Mesh.submodels[0].material.casts_shadow = false;
                        n.Mesh.submodels[0].material.matDepthBias = -0.000003f;
                        n.DecalIndex = decal_info.Add(new SFDecalInfo() { decal_node = n });
                        for (int j = 0; j < n.Mesh.submodels.Length; j++)
                        {
                            SFResourceManager.Textures.SetRemoveWhenUnused(n.Mesh.submodels[j].material.texture.Name, false);
                        }
                    }
                    else
                    {
                        foreach (SFSubModel3D sbm in n.Mesh.submodels)
                        {
                            sbm.material.apply_shading = apply_shading;
                            sbm.material.casts_shadow = casts_shadow;
                        }
                    }
                }
            }

            return obj_node;
        }

        public SceneNode AddSceneBuilding(int building_id, string building_name)
        {
            // create root
            SceneNode bld_node = AddSceneNodeEmpty(null, building_name);

            SFLuaSQLBuildingData bld_data = SFLuaEnvironment.buildings[building_id];
            if (bld_data == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneBuilding(): Can't find building data (object id = "
                    + building_id + ")");
                return bld_node;
            }
            List<string> m_lst = bld_data.Mesh;

            // add meshes
            for (int i = 0; i < m_lst.Count; i++)
            {
                string m = m_lst[i];
                if (m.Contains("frame"))
                {
                    continue;
                }

                SceneNodeSimple n = AddSceneNodeSimple(bld_node, m, i.ToString());

                if (n.Mesh != null)
                {
                    bool decal = m.Contains("decal");
                    if (decal)
                    {
                        n.IsDecal = true;
                        n.Mesh.submodels[0].material.apply_shading = true;
                        n.Mesh.submodels[0].material.transparent_pass = true;
                        n.Mesh.submodels[0].material.casts_shadow = false;
                        n.Mesh.submodels[0].material.matDepthBias = -0.000003f;
                        n.DecalIndex = decal_info.Add(new SFDecalInfo() { decal_node = n });
                        for (int j = 0; j < n.Mesh.submodels.Length; j++)
                        {
                            SFResourceManager.Textures.SetRemoveWhenUnused(n.Mesh.submodels[j].material.texture.Name, false);
                        }
                    }
                }
            }

            return bld_node;
        }

        // this removes the node, and if needed, disposes it
        // use this to remove nodes from the scene!
        public void RemoveSceneNode(SceneNode node, bool dispose = true)
        {
            if (node == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFScene.RemoveSceneNode(): node is null!");
                return;
            }
            node.SetParent(null);
            if (dispose)
            {
                node.Dispose();
            }
        }

        // sets scene time
        public void SetSceneTime(float t)
        {
            if (t < 0)
            {
                t = 0;
            }

            if (t > scene_meta.duration)
            {
                t = scene_meta.duration;
            }

            current_time = t;

            root.SetTime(t);
        }

        public void StopTimeFlow()
        {
            time_flowing = false;
        }

        public void ResumeTimeFlow()
        {
            if (!time_flowing)
            {
                deltatime = 1.0f / frames_per_second;
            }

            time_flowing = true;
            delta_timer.Restart();

        }

        // updates root of the scene (and consequently, all children that need to be updated)
        public void Update(float dt)
        {
            // first, clear mesh matrices
            int cur_offset = 0;
            foreach (var mesh in model_set_simple)
            {
                mesh.CurrentMatrixIndex = 0;
                if (mesh.MatrixOffset != cur_offset)
                {
                    mesh.MatrixOffset = cur_offset;
                    mesh.ForceUpdateInstanceMatrices = true;
                }
                cur_offset += mesh.MatrixCount;
            }

            SFSubModel3D.Cache.ResizeInstanceMatrixBuffer(cur_offset);

            root.Update(dt);

            current_time += dt;
            if (scene_meta != null)
            {
                if (current_time > scene_meta.duration)
                {
                    current_time -= scene_meta.duration;
                }
            }

            foreach (var mesh in model_set_simple)
            {
                mesh.ForceUpdateInstanceMatrices = false;
            }

            SFSubModel3D.Cache.CurrentMatrix = cur_offset;
            SFSubModel3D.Cache.MatrixUpload(0, SFSubModel3D.Cache.CurrentMatrix);

            //System.Diagnostics.Debug.WriteLine("INSTANCE MATRIX COUNT: " + cur_offset.ToString());
        }

        public void Clear()
        {
            model_set_simple.Clear();
            opaque_pass_models.Clear();
            transparent_pass_models.Clear();
            water_pass_models.Clear();
            additive_pass_models.Clear();
            decal_info.Clear();
            an_primary_nodes.Clear();

            SFResourceManager.Models.Dispose("_MISSING_MESH_");
            missing_node_mesh = null;
        }
    }
}
