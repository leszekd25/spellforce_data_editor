/*
 * SFSceneLoader is de facto a scene manager, it controls what is displayed in the window
 * Main functions are ParseSceneDescription and CatElemToScene
 * CatElemToScene generates scene description based on provided game data element
 * ParseSceneDescription generates visual data displayed in window, based on provided description
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

namespace SpellforceDataEditor.SF3D.SceneSynchro
{
    public class SFSceneMeta
    {
        public float duration = 0f;
        public bool is_animated = false;
    }

    public class SFScene
    {
        // TODO: multiple scenes (at least one for map editor and one for asset viewer
        public SFSceneMeta scene_meta { get; private set; } = new SFSceneMeta();    // scene metadata

        private int frame_counter = 0;      // total frames rendered
        public int frames_per_second { get; private set; } = 25;   // framerate
        public System.Diagnostics.Stopwatch delta_timer { get; private set; } = new System.Diagnostics.Stopwatch();     // timer which manages delta time
        private float deltatime = 0f;       // current delta time value in seconds
        public float current_time { get; private set; } = 0f;        // current scene time in seconds

        public SFVisualLinkContainer mesh_data { get; private set; } = new SFVisualLinkContainer();

        public SceneNode root;   // tree with all visible objects; invisible objects are not connected to root
        public SFMap.SFMapHeightMap heightmap = null;  // contains heightmap chunk scene nodes
        public SceneNodeCamera camera;

        public LightingAmbient ambient_light { get; } = new LightingAmbient();
        public LightingSun sun_light { get; } = new LightingSun();
        // render engine takes these lists and renders stuff from the lists, one list for each texture for each shader
        public Dictionary<SFTexture, LinearPool<TexturedGeometryListElementSimple>> tex_list_simple { get; private set; } = new Dictionary<SFTexture, LinearPool<TexturedGeometryListElementSimple>>();
        public Dictionary<SFTexture, LinearPool<TexturedGeometryListElementAnimated>> tex_list_animated { get; private set; } = new Dictionary<SFTexture, LinearPool<TexturedGeometryListElementAnimated>>();

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
            int el_size = el.get().Count / 3;
            for (int i = 0; i < el_size; i++)
            {
                if ((Byte)el.get_single_variant(i * 3 + 1).value == slot)
                    return (UInt16)el.get_single_variant(i * 3 + 2).value;
            }
            return 0;
        }
            

        // generates a scene description given gamedata element
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
                    SFCategoryElement item = SFCategoryManager.gamedata.categories[category].get_element(element);
                    if (item == null)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Item not found (category is " + category.ToString()
                            + ", element is " + element.ToString() + ")");
                        break;
                    }
                    int item_id = (UInt16)item.get_single_variant(0).value;

                    //find item mesh
                    string m_name = mesh_data.GetItemMesh(item_id, false);
                    if (m_name == "")
                    {
                        LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Item mesh not found (item id = " + item_id.ToString() + ")");
                        break;
                    }

                    //create scene
                    SceneNodeSimple item_node = AddSceneNodeSimple(root, m_name, "item");
                    item_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);

                    break;
                case 23:
                case 24:
                case 25:
                    //get item id
                    int building_id = (ushort)SFCategoryManager.gamedata.categories[category].get_element(element).get_single_variant(0).value;
                    SceneNode building_node = AddSceneBuilding(building_id, "building");
                    building_node.SetParent(root);
                    building_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);

                    break;
                case 33:
                case 34:
                case 35:
                    //get item id
                    int object_id = (ushort)SFCategoryManager.gamedata.categories[category].get_element(element).get_single_variant(0).value;
                    SceneNode object_node = AddSceneObject(object_id, "object", true);
                    object_node.SetParent(root);
                    object_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);

                    break;
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                    //get unit id
                    int unit_id = (ushort)SFCategoryManager.gamedata.categories[category].get_element(element).get_single_variant(0).value;
                    SceneNode unit_node = AddSceneUnit(unit_id, "unit");
                    unit_node.SetParent(root);
                    unit_node.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
                    
                    scene_meta.is_animated = true;
                    break;
                default:
                    break;
            }
        }
        // NEW SCENE SYSTEM STUFF!

        public int AddTextureEntrySimple(SFTexture tex, TexturedGeometryListElementSimple elem)
        {
            if (!tex_list_simple.ContainsKey(tex))
                tex_list_simple.Add(tex, new LinearPool<TexturedGeometryListElementSimple>());

            return tex_list_simple[tex].Add(elem);
        }

        public void ClearTextureEntrySimple(SFTexture tex, TexturedGeometryListElementSimple elem)
        {
            if (!tex_list_simple.ContainsKey(tex))
                return;

            tex_list_simple[tex].Remove(elem);

            if (tex_list_simple[tex].used_count == 0)
                tex_list_simple.Remove(tex);
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
                new_node.Mesh = SFResourceManager.Models.Get(model_name);

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

        public SceneNode AddSceneUnit(int unit_id, string object_name)
        {
            SceneNode unit_node = AddSceneNodeEmpty(null, object_name);    // parent to be assigned later, likely some of the cached mapchunk nodes

            //find unit data element (cat 18)
            SFCategoryElement unit_data = SFCategoryManager.gamedata.categories[17].find_binary_element<UInt16>(0, (UInt16)unit_id);
            if (unit_data == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Unit does not exist (unit id = "
                    + unit_id + ")");
                return unit_node;
            }
            //find unit eq element (cat 19)
            SFCategoryElement unit_eq = SFCategoryManager.gamedata.categories[18].find_binary_element<UInt16>(0, (UInt16)unit_id);
            if (unit_eq == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Unit has no inventory assigned to it (unit id = "
                    + unit_id + ")");
                return unit_node;
            }

            //get unit gender
            SFCategoryElement unit_stats = SFCategoryManager.gamedata.categories[3].find_binary_element<UInt16>
                                                                        (0, (UInt16)unit_data.get_single_variant(2).value);
            bool is_female = false;
            if (unit_stats != null)
                is_female = ((Byte)unit_stats.get_single_variant(23).value % 2) == 1;

            //get chest item (2) (animated)
            UInt16 chest_id = GetItemID(unit_eq, 2);
            if (chest_id == 0)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Unit does not have chestpiece assigned (unit id = "
                    + unit_id + ")");
                return unit_node;
            }
            //find chest skin/animations
            string chest_name = mesh_data.GetItemMesh(chest_id, is_female);
            if (chest_name == "")
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Undefined chestpiece mesh (unit id = "
                    + unit_id + ")");
                return unit_node;
            }
            string anim_name = mesh_data.GetItemAnimation(chest_id, is_female);

            //special case: monument unit, needs to be considered separately
            string unit_handle = Utility.CleanString(unit_data.get_single_variant(10));
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
                string legs_name = mesh_data.GetItemMesh(legs_id, is_female);
                if (legs_name != "")
                {
                    uo = AddSceneNodeAnimated(unit_node, legs_name, "Legs");
                    if (uo.Skin != null)
                        foreach (SFModelSkinChunk msc in uo.Skin.submodels)
                            msc.material.flat_shade = true;
                }
            }
            //special case: anim_name is of "figure_hero": need to also add human head (animated)
            if ((anim_name.Contains("figure_hero")) && (unit_stats != null))
            {

                int head_id = (UInt16)unit_stats.get_single_variant(24).value;
                string head_name = mesh_data.GetHeadMesh(head_id, is_female);
                if (head_name != "")
                {
                    uo = AddSceneNodeAnimated(unit_node, head_name, "Head");
                    if (uo.Skin != null)
                        foreach (SFModelSkinChunk msc in uo.Skin.submodels)
                            msc.material.flat_shade = true;
                }
                else
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneUnit(): Unit head has undefined mesh (unit id = "
                        + unit_id.ToString() + ", head id = " + head_id.ToString() + ")");
                }
            }

            //get helmet item (0) (boneanchor(Head), simple)
            UInt16 helmet_id = GetItemID(unit_eq, 0);
            if (helmet_id != 0)
            {
                //find item mesh
                string helmet_name = mesh_data.GetItemMesh(helmet_id, is_female);
                if (helmet_name != "")
                {
                    //create bone attachment
                    SceneNodeBone bo = AddSceneNodeBone(unit_node.FindNode<SceneNodeAnimated>("Chest"), "Head", "Headbone");
                    SceneNodeSimple ho = AddSceneNodeSimple(bo, helmet_name, "Helmet");
                    if (ho.Mesh != null)
                        foreach (SFMaterial mat in ho.Mesh.materials)
                            mat.flat_shade = true;
                }
            }

            //get right hand (1) (boneanchor(R Hand weapon), simple)
            UInt16 rhand_id = GetItemID(unit_eq, 1);
            if (rhand_id != 0)
            {
                //find item mesh
                string rhand_name = mesh_data.GetItemMesh(rhand_id, is_female);
                if (rhand_name != "")
                {
                    //create bone attachment
                    SceneNodeBone bo = AddSceneNodeBone(unit_node.FindNode<SceneNodeAnimated>("Chest"), "R Hand weapon", "Rhandbone");
                    SceneNodeSimple ho = AddSceneNodeSimple(bo, rhand_name, "Rhand");
                    if (ho.Mesh != null)
                        foreach (SFMaterial mat in ho.Mesh.materials)
                            mat.flat_shade = true;
                }
            }

            //get left hand (3) (boneanchor(L Hand weapon OR Forearm shield), simple)
            UInt16 lhand_id = GetItemID(unit_eq, 3);
            if (lhand_id != 0)
            {
                //find item mesh
                string lhand_name = mesh_data.GetItemMesh(lhand_id, is_female);
                if (lhand_name != "")
                {
                    bool is_shield = false;
                    //check if it's a shield (type 9)
                    SFCategoryElement item_data = SFCategoryManager.gamedata.categories[6].find_binary_element<UInt16>(0, lhand_id);
                    if (item_data != null)
                    {
                        int item_type = (Byte)item_data.get_single_variant(2).value;
                        is_shield = item_type == 9;
                    }
                    //create bone attachment
                    SceneNodeBone bo = AddSceneNodeBone(unit_node.FindNode<SceneNodeAnimated>("Chest"), (is_shield ? "L Forearm shield" : "L Hand weapon"), "Lhandbone");
                    SceneNodeSimple ho = AddSceneNodeSimple(bo, lhand_name, "Lhand");
                    if (ho.Mesh != null)
                        foreach (SFMaterial mat in ho.Mesh.materials)
                            mat.flat_shade = true;
                }
            }

            return unit_node;
        }

        public SceneNode AddSceneObject(int object_id, string object_name, bool apply_shading)
        {
            List<string> m_lst = mesh_data.GetObjectMeshes(object_id);

            // create root
            SceneNode obj_node = AddSceneNodeEmpty(null, object_name);

            if (m_lst == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneObject(): Object mesh is usassigned (object id = "
                    + object_id + ")");
                return obj_node;
            }

            // add meshes
            for (int i = 0; i < m_lst.Count; i++)
            {
                string m = m_lst[i];
                string mesh_obj_name = object_name + "_" + i.ToString();
                SceneNodeSimple n = AddSceneNodeSimple(obj_node, m, i.ToString());
                if (n.Mesh != null)
                    foreach (SFMaterial mat in n.Mesh.materials)
                        mat.apply_shading = apply_shading;
            }

            return obj_node;
        }

        public SceneNode AddSceneBuilding(int building_id, string building_name)
        {
            List<string> m_lst = mesh_data.GetBuildingMeshes(building_id);

            // create root
            SceneNode bld_node = AddSceneNodeEmpty(null, building_name);

            if (m_lst == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddSceneBuilding(): Building mesh is usassigned (building id = "
                    + building_id + ")");
                return bld_node;
            }

            // add meshes
            for (int i = 0; i < m_lst.Count; i++)
            {
                string m = m_lst[i];
                AddSceneNodeSimple(bld_node, m, i.ToString());
            }

            return bld_node;
        }
        
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

        public void Update(bool time_flow = true)
        {
            if (time_flow)
            {
                delta_timer.Stop();
                deltatime = delta_timer.ElapsedMilliseconds / (float)1000;
                delta_timer.Restart();
            }
            else
                deltatime = 0f;

            root.Update(current_time);

            current_time += deltatime;
            if (scene_meta != null)
                if (current_time > scene_meta.duration)
                    current_time -= scene_meta.duration;

            frame_counter++;
        }
    }
}
