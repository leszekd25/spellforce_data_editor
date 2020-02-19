/*
 * SFScene holds all visual data (except resources) and is able to manipulate it
 * CatElemToScene generates scene description based on provided game data element, useful for asset viewer
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SpellforceDataEditor.SFCFF;
using SpellforceDataEditor.SFResources;

using SpellforceDataEditor.SFLua;
using SpellforceDataEditor.SFLua.lua_sql;

namespace SpellforceDataEditor.SF3D.SceneSynchro
{
    public class SFSceneMeta
    {
        public float duration = 0f;
        public bool is_animated = false;
        public string name = "";
    }

    public class SFScene
    {
        // TODO: multiple scenes (at least one for map editor and one for asset viewer
        public SFSceneMeta scene_meta { get; private set; } = new SFSceneMeta();    // scene metadata

        public int frame_counter { get; private set; } = 0;      // total frames rendered
        public int frames_per_second { get; private set; } = Settings.FramesPerSecond;   // framerate
        public System.Diagnostics.Stopwatch delta_timer { get; private set; } = new System.Diagnostics.Stopwatch();     // timer which manages delta time
        private float deltatime = 0f;       // current delta time value in seconds
        public float DeltaTime
        {
            get
            {
                if (time_flowing)
                    return deltatime;
                return 0f;
            }
        }
        public float current_time { get; private set; } = 0f;        // current scene time in seconds
        private bool time_flowing = false;

        public SceneNode root;   // tree with all visible objects; invisible objects are not connected to root
        public SFMap.SFMapHeightMap heightmap = null;  // contains heightmap chunk scene nodes
        public SceneNodeCamera camera;

        public LightingAmbient ambient_light { get; } = new LightingAmbient();
        public LightingSun sun_light { get; } = new LightingSun();
        public Atmosphere atmosphere { get; } = new Atmosphere();
        // render engine takes these lists and renders stuff from the lists, one list for each texture for each shader
        public HashSet<SFSubModel3D> untex_entries_simple = new HashSet<SFSubModel3D>();
        public Dictionary<SFTexture, HashSet<SFSubModel3D>> tex_entries_simple = new Dictionary<SFTexture, HashSet<SFSubModel3D>>();
        public Dictionary<SFTexture, LinearPool<TexturedGeometryListElementAnimated>> tex_list_animated { get; private set; } = new Dictionary<SFTexture, LinearPool<TexturedGeometryListElementAnimated>>();
        public LinearPool<TexturedGeometryListElementSimple> untextured_list_simple { get; private set; } = new LinearPool<TexturedGeometryListElementSimple>();

        public void Init()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFScene.Init() called");

            root = new SceneNode("Root");
            camera = new SceneNodeCamera("Camera");
            camera.SetParent(root);

            delta_timer.Start();
        }

        // helper function, returns item id given a gamedata element from category 19 and item slot
        private UInt16 GetItemID(SFCategoryElement el, Byte slot)
        {
            int el_size = el.variants.Count / 3;
            for (int i = 0; i < el_size; i++)
            {
                if ((Byte)el[i * 3 + 1] == slot)
                    return (UInt16)el[i * 3 + 2];
            }
            return 0;
        }
            

        // generates a scene given gamedata element
        public void CatElemToScene(int category, int element)
        {
            scene_meta.is_animated = false;
            switch (category)
            {
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                    //get item id
                    SFCategoryElement item = SFCategoryManager.gamedata[category][element];
                    if (item == null)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Item not found (category is " + category.ToString()
                            + ", element is " + element.ToString() + ")");
                        break;
                    }
                    int item_id = (UInt16)item[0];

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
                case 23:
                case 24:
                case 25:
                    //get building id
                    int building_id = (ushort)SFCategoryManager.gamedata[category][element][0];
                    SceneNode building_node = AddSceneBuilding(building_id, "building");
                    building_node.SetParent(root);
                    building_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                    scene_meta.name = SFCategoryManager.GetBuildingName((ushort)building_id);

                    break;
                case 33:
                case 34:
                case 35:
                    //get object id
                    int object_id = (ushort)SFCategoryManager.gamedata[category][element][0];
                    SceneNode object_node = AddSceneObject(object_id, "object", true);
                    object_node.SetParent(root);
                    object_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                    scene_meta.name = SFCategoryManager.GetObjectName((ushort)object_id);

                    break;
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                    //get unit id
                    int unit_id = (ushort)SFCategoryManager.gamedata[category][element][0];
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

        // these functions manipulate scene cache, which is used by SFRenderEngine to render stuff
        public int AddUntexturedEntrySimple(TexturedGeometryListElementSimple elem)
        {
            return untextured_list_simple.Add(elem);
        }

        public void ClearUntexturedEntrySimple(TexturedGeometryListElementSimple elem)
        {
            untextured_list_simple.Remove(elem);
            if (untextured_list_simple.used_count == 0)
                untextured_list_simple.Clear();
        }

        public int AddTextureEntryAnimated(SFTexture tex, TexturedGeometryListElementAnimated elem)
        {
            if (!tex_list_animated.ContainsKey(tex))
                tex_list_animated.Add(tex, new LinearPool<TexturedGeometryListElementAnimated>());

            return tex_list_animated[tex].Add(elem);
        }

        public void ClearTextureEntryAnimated(SFTexture tex, TexturedGeometryListElementAnimated elem)
        {
            if (!tex_list_animated.ContainsKey(tex))
                return;

            tex_list_animated[tex].Remove(elem);

            if (tex_list_animated[tex].used_count == 0)
                tex_list_animated.Remove(tex);
        }

        // instanced versions of simple tex entry (works for untex as well)
        // they dont return anything, since they're rebuilt every frame...

        public void AddInstancedEntrySimple(TexturedGeometryListElementSimple elem)
        {
            elem.node.Mesh.submodels[elem.submodel_index].instance_matrices.AddElem(elem.node.ResultTransform);
        }

        public void ClearInstancedEntriesSimple(TexturedGeometryListElementSimple elem)
        {
            elem.node.Mesh.submodels[elem.submodel_index].instance_matrices.Clear();
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
                for (int i = 0; i < new_node.Mesh.submodels.Length; i++)
                {
                    SFSubModel3D sbm = new_node.Mesh.submodels[i];
                    if (sbm.material.texture != null)
                    {
                        if (!tex_entries_simple.ContainsKey(sbm.material.texture))
                            tex_entries_simple.Add(sbm.material.texture, new HashSet<SFSubModel3D>());
                        if (!tex_entries_simple[sbm.material.texture].Contains(sbm))
                            tex_entries_simple[sbm.material.texture].Add(sbm);
                    }
                    else
                    {
                        if (!untex_entries_simple.Contains(sbm))
                            untex_entries_simple.Add(sbm);
                    }
                }
            }

            return new_node;
        }

        public SceneNodeAnimated AddSceneNodeAnimated(SceneNode parent, string skin_name, string new_node_name)
        {
            SceneNodeAnimated new_node = new SceneNodeAnimated(new_node_name);
            new_node.SetParent(parent);

            bool loaded_skin = SFResourceManager.LoadSkeleton(skin_name);
            if (loaded_skin)
                loaded_skin = SFResourceManager.LoadSkin(skin_name);
            if (loaded_skin)
                new_node.SetSkeletonSkin(SFResourceManager.Skeletons.Get(skin_name), SFResourceManager.Skins.Get(skin_name));
            loaded_skin = SFResourceManager.LoadModel(skin_name);
            if (loaded_skin)
                new_node.Mesh = SFResourceManager.Models.Get(skin_name);

            return new_node;
        }

        public SceneNodeBone AddSceneNodeBone(SceneNodeAnimated parent, string bone_name, string new_node_name)
        {
            SceneNodeBone new_node = new SceneNodeBone(new_node_name);
            new_node.SetParent(parent);

            new_node.SetBone(bone_name);

            return new_node;
        }

        public SceneNodeMapChunk AddSceneMapChunk(SceneNode parent, SFMap.SFMapHeightMapChunk chunk, string new_node_name)
        {
            SceneNodeMapChunk new_node = new SceneNodeMapChunk(new_node_name);
            new_node.SetParent(parent);

            new_node.MapChunk = chunk;

            return new_node;
        }

        // these functions create and return nodes which are used to display game elements (units, buildings,...)
        // note that those don't assign parent to the nodes - it's done somewhere else
        public SceneNode AddSceneUnit(int unit_id, string object_name)
        {
            SceneNode unit_node = AddSceneNodeEmpty(null, object_name);    // parent to be assigned later, likely some of the cached mapchunk nodes

            //find unit data element (cat 18)
            SFCategoryElement unit_data = SFCategoryManager.gamedata[17].FindElementBinary<UInt16>(0, (UInt16)unit_id);
            if (unit_data == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Unit does not exist (unit id = "
                    + unit_id + ")");
                return unit_node;
            }
            //find unit eq element (cat 19)
            SFCategoryElement unit_eq = SFCategoryManager.gamedata[18].FindElementBinary<UInt16>(0, (UInt16)unit_id);
            if (unit_eq == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Unit has no inventory assigned to it (unit id = "
                    + unit_id + ")");
                return unit_node;
            }

            //get unit gender
            SFCategoryElement unit_stats = SFCategoryManager.gamedata[3].FindElementBinary<UInt16>(0, (UInt16)unit_data[2]);
            bool is_female = false;
            if (unit_stats != null)
                is_female = ((Byte)unit_stats[23] % 2) == 1;

            //get chest item (2) (animated)
            UInt16 chest_id = GetItemID(unit_eq, 2);
            if (chest_id == 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Unit does not have chestpiece assigned (unit id = "
                    + unit_id + ")");
                return unit_node;
            }
            //find chest skin/animations
            SFLuaSQLItemData chest_data = SFLuaEnvironment.items[chest_id];
            if(chest_data == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Undefined chestpiece mesh (unit id = "
                    + unit_id + ")");
                return unit_node;
            }
            string chest_name = SFLuaEnvironment.GetItemMesh(chest_id, is_female);
            string anim_name = chest_data.AnimSet;
            if (anim_name == "")
                anim_name = "figure_hero";

            //special case: monument unit, needs to be considered separately
            string unit_handle = Utility.CleanString(unit_data[10]);
            if ((unit_handle.StartsWith("Unit")) && (!unit_handle.Contains("Titan")))
            {
                chest_name += "_cold";
            }
            
            //add anim model to scene
            SceneNodeAnimated uo = AddSceneNodeAnimated(unit_node, chest_name, "Chest");
            // apply flat shade
            if (uo.Skin != null)
                foreach (SFModelSkinChunk msc in uo.Skin.submodels)
                    msc.material.flat_shade = true;

            //get legs item (5) (animated)
            UInt16 legs_id = GetItemID(unit_eq, 5);
            if (legs_id != 0)
            {
                //find chest skin/animations
                string legs_name = SFLuaEnvironment.GetItemMesh(legs_id, is_female);
                if (legs_name != "")
                {
                    uo = AddSceneNodeAnimated(unit_node, legs_name, "Legs");
                    if (uo.Skin != null)
                        foreach (SFModelSkinChunk msc in uo.Skin.submodels)
                            msc.material.flat_shade = true;
                }
            }
            //special case: anim_name is of "figure_hero": need to also add human head (animated)
            if ((anim_name == "figure_hero") && (unit_stats != null))
            {
                int head_id = (UInt16)unit_stats[24];
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
                        head_name = head_data.MeshFemale;
                    else
                        head_name = head_data.MeshMale;
                    uo = AddSceneNodeAnimated(unit_node, head_name, "Head");
                    if (uo.Skin != null)
                        foreach (SFModelSkinChunk msc in uo.Skin.submodels)
                            msc.material.flat_shade = true;
                }
            }

            //get helmet item (0) (boneanchor(Head), simple)
            UInt16 helmet_id = GetItemID(unit_eq, 0);
            if (helmet_id != 0)
            {
                //find item mesh
                string helmet_name = SFLuaEnvironment.GetItemMesh(helmet_id, is_female);
                if (helmet_name != "")
                {
                    //create bone attachment
                    SceneNodeBone bo = AddSceneNodeBone(unit_node.FindNode<SceneNodeAnimated>("Chest"), "Head", "Headbone");
                    SceneNodeSimple ho = AddSceneNodeSimple(bo, helmet_name, "Helmet");
                    if (ho.Mesh != null)
                        foreach (SFSubModel3D sbm in ho.Mesh.submodels)
                            sbm.material.flat_shade = true;
                }
            }

            //get right hand (1) (boneanchor(R Hand weapon), simple)
            UInt16 rhand_id = GetItemID(unit_eq, 1);
            if (rhand_id != 0)
            {
                //find item mesh
                string rhand_name = SFLuaEnvironment.GetItemMesh(rhand_id, is_female);
                if (rhand_name != "")
                {
                    //create bone attachment
                    SceneNodeBone bo = AddSceneNodeBone(unit_node.FindNode<SceneNodeAnimated>("Chest"), "R Hand weapon", "Rhandbone");
                    SceneNodeSimple ho = AddSceneNodeSimple(bo, rhand_name, "Rhand");
                    if (ho.Mesh != null)
                        foreach (SFSubModel3D sbm in ho.Mesh.submodels)
                            sbm.material.flat_shade = true;
                }
            }

            //get left hand (3) (boneanchor(L Hand weapon OR Forearm shield), simple)
            UInt16 lhand_id = GetItemID(unit_eq, 3);
            if (lhand_id != 0)
            {
                //find item mesh
                string lhand_name = SFLuaEnvironment.GetItemMesh(lhand_id, is_female);
                if (lhand_name != "")
                {
                    bool is_shield = false;
                    //check if it's a shield (type 9)
                    SFCategoryElement item_data = SFCategoryManager.gamedata[6].FindElementBinary<UInt16>(0, lhand_id);
                    if (item_data != null)
                    {
                        int item_type = (Byte)item_data[2];
                        is_shield = item_type == 9;
                    }
                    //create bone attachment
                    SceneNodeBone bo = AddSceneNodeBone(unit_node.FindNode<SceneNodeAnimated>("Chest"), (is_shield ? "L Forearm shield" : "L Hand weapon"), "Lhandbone");
                    SceneNodeSimple ho = AddSceneNodeSimple(bo, lhand_name, "Lhand");
                    if (ho.Mesh != null)
                        foreach (SFSubModel3D sbm in ho.Mesh.submodels)
                            sbm.material.flat_shade = true;
                }
            }

            return unit_node;
        }

        public SceneNode AddSceneObject(int object_id, string object_name, bool apply_shading)
        {
            // create root
            SceneNode obj_node = AddSceneNodeEmpty(null, object_name);

            SFLuaSQLObjectData obj_data = SFLuaEnvironment.objects[object_id];
            if(obj_data==null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneObject(): Can't find object data (object id = "
                    + object_id + ")");
                return obj_node;
            }
            List<string> m_lst = obj_data.Mesh;

            // add meshes
            for (int i = 0; i < m_lst.Count; i++)
            {
                string m = m_lst[i];
                if (m == "")
                    continue;
                SceneNodeSimple n = AddSceneNodeSimple(obj_node, m, i.ToString());
                if (n.Mesh != null)
                    foreach (SFSubModel3D sbm in n.Mesh.submodels)
                        sbm.material.apply_shading = apply_shading;
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
                    continue;
                AddSceneNodeSimple(bld_node, m, i.ToString());
            }

            return bld_node;
        }
        
        // this removes the node, and if needed, disposes it
        // use this to remove nodes from the scene!
        public void RemoveSceneNode(SceneNode node, bool dispose = true)
        {
            if(node == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFScene.RemoveSceneNode(): node is null!");
                return;
            }
            node.SetParent(null);
            if(dispose)
                node.Dispose();
        }
        
        // sets scene time
        public void SetSceneTime(float t)
        {
            if (t < 0)
                t = 0;
            if (t > scene_meta.duration)
                t = scene_meta.duration;
            current_time = t;
        }

        public void StopTimeFlow()
        {
            time_flowing = false;
        }

        public void ResumeTimeFlow()
        {
            if(!time_flowing)
                deltatime = 1.0f/frames_per_second;
            time_flowing = true;
            delta_timer.Restart();
            
        }

        // updates root of the scene (and consequently, all children that need to be updated)
        public void Update()
        {
            delta_timer.Stop();
            deltatime = delta_timer.ElapsedMilliseconds / (float)1000;
            delta_timer.Restart();

            // for instanced rendering
            foreach (var tex_entry in tex_entries_simple.Values)
                foreach (SFSubModel3D sbm in tex_entry)
                    sbm.instance_matrices.Clear();
            foreach (SFSubModel3D sbm in untex_entries_simple)
                sbm.instance_matrices.Clear();

            root.Update(current_time);

            current_time += deltatime;
            if (scene_meta != null)
                if (current_time > scene_meta.duration)
                    current_time -= scene_meta.duration;

            frame_counter++;
        }
    }
}
