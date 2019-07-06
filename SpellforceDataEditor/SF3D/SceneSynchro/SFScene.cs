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
    public class SFScene
    {
        // TODO: multiple scenes (at least one for map editor and one for asset viewer
        public SFSceneDescriptionMeta scene_meta { get; private set; } = null;     // scene metadata
        public Dictionary<string, ObjectSimple3D> objects_static { get; private set; } = new Dictionary<string, ObjectSimple3D>();   // static objects in the scene
        public Dictionary<string, objectAnimated> objects_dynamic { get; private set; } = new Dictionary<string, objectAnimated>();  // animated objects in the scene

        private int frame_counter = 0;      // total frames rendered
        public int frames_per_second { get; private set; } = 25;   // framerate
        public System.Diagnostics.Stopwatch delta_timer { get; private set; } = new System.Diagnostics.Stopwatch();     // timer which manages delta time
        private float deltatime = 0f;       // current delta time value in seconds
        public float current_time { get; private set; } = 0f;        // current scene time in seconds

        public SFVisualLinkContainer mesh_data { get; private set; } = new SFVisualLinkContainer();

        public Dictionary<SFTexture, LinearPool<TexturedGeometryListElementSimple>> tex_list_simple { get; private set; } = new Dictionary<SFTexture, LinearPool<TexturedGeometryListElementSimple>>();
        public Dictionary<SFTexture, LinearPool<TexturedGeometryListElementAnimated>> tex_list_animated { get; private set; } = new Dictionary<SFTexture, LinearPool<TexturedGeometryListElementAnimated>>();

        public void Init()
        {
            delta_timer.Start();
        }

        // creates a scene given scene description, adding objects if necessary
        public void ParseSceneDescription(SFSceneDescription scene)
        {
            scene_meta = scene.meta;
            foreach (SFSceneDescriptionLine sl in scene.get_lines())
            {
                switch (sl.type)
                {
                    case SCENE_ITEM_TYPE.OBJ_SIMPLE:
                        AddObjectStatic(sl.args[0], sl.args[1], sl.args[2]);
                        break;
                    case SCENE_ITEM_TYPE.OBJ_ANIMATED:
                        AddObjectDynamic(sl.args[0], sl.args[1], sl.args[2]);
                        break;
                    case SCENE_ITEM_TYPE.OBJ_BONE:
                        AddObjectBoneanchor(sl.args[0], sl.args[1], sl.args[2]);
                        break;
                    case SCENE_ITEM_TYPE.SCENE_ANIM:
                        if (sl.args[0] == "1")
                            scene.meta.is_animated = true;
                        else if (sl.args[0] == "0")
                            scene.meta.is_animated = false;
                        break;
                    default:
                        LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFSceneManager.ParseSceneDescription(): Unknown type (type = " + sl.type + ")");
                        break;
                }
            }
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

        // creates a scene description: game unit and its equipment
        private void MakeUnitScene(int unit_id, SFSceneDescription sd)
        {
            //find unit data element (cat 18)
            SFCategoryElement unit_data = SFCategoryManager.gamedata.categories[17].find_binary_element<UInt16>(0, (UInt16)unit_id);
            if (unit_data == null)
                return;
            //find unit eq element (cat 19)
            SFCategoryElement unit_eq = SFCategoryManager.gamedata.categories[18].find_binary_element<UInt16>(0, (UInt16)unit_id);
            if (unit_eq == null)
                return;

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
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.MakeUnitScene(): Chest id not found!");
                return;
            }
            //find chest skin/animations
            string chest_name = mesh_data.GetItemMesh(chest_id, is_female);
            if (chest_name == "")
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.MakeUnitScene(): Chest model name not found (chest id = " + chest_id.ToString() + ")");
                return;
            }
            string anim_name = mesh_data.GetItemAnimation(chest_id, is_female);

            //special case: monument unit, needs to be considered separately
            string unit_handle = Utility.CleanString(unit_data.get_single_variant(10));
            if ((unit_handle.StartsWith("Unit")) && (!unit_handle.Contains("Titan")))
            {
                chest_name += "_cold";
            }


            //add anim model to scene
            sd.add_line(SCENE_ITEM_TYPE.OBJ_ANIMATED, new string[] { chest_name, "", "MAIN" });
            sd.meta.obj_to_anim["MAIN"] = anim_name;

            //get legs item (5) (animated)
            UInt16 legs_id = GetItemID(unit_eq, 5);
            if (chest_id != 0)
            {
                //find chest skin/animations
                string legs_name = mesh_data.GetItemMesh(legs_id, is_female);
                if (legs_name != "")
                {
                    sd.add_line(SCENE_ITEM_TYPE.OBJ_ANIMATED, new string[] { legs_name, "", legs_name });
                    sd.meta.obj_to_anim[legs_name] = anim_name;
                }
            }
            //special case: anim_name is of "figure_hero": need to also add human head (animated)
            if ((anim_name.Contains("figure_hero")) && (unit_stats != null))
            {

                int head_id = (UInt16)unit_stats.get_single_variant(24).value;
                string head_name = mesh_data.GetHeadMesh(head_id, is_female);
                if (head_name != "")
                {
                    sd.add_line(SCENE_ITEM_TYPE.OBJ_ANIMATED, new string[] { head_name, "", head_name });
                    sd.meta.obj_to_anim[head_name] = anim_name;
                }
                else
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.MakeUnitScene(): Head model not found (head id = " + head_id.ToString() + ")");
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
                    sd.add_line(SCENE_ITEM_TYPE.OBJ_BONE, new string[] { "MAIN", "Head", "HEAD" });
                    sd.add_line(SCENE_ITEM_TYPE.OBJ_SIMPLE, new string[] { helmet_name, "HEAD", helmet_name });
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
                    sd.add_line(SCENE_ITEM_TYPE.OBJ_BONE, new string[] { "MAIN", "R Hand weapon", "RHAND" });
                    sd.add_line(SCENE_ITEM_TYPE.OBJ_SIMPLE, new string[] { rhand_name, "RHAND", "I_RHAND" });
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
                    sd.add_line(SCENE_ITEM_TYPE.OBJ_BONE, new string[] { "MAIN", (is_shield ? "L Forearm shield" : "L Hand weapon"), "LHAND" });
                    sd.add_line(SCENE_ITEM_TYPE.OBJ_SIMPLE, new string[] { lhand_name, "LHAND", "I_LHAND" });
                }
            }

            sd.add_line(SCENE_ITEM_TYPE.SCENE_ANIM, new string[] { "1" });
        }

        // generates a scene description given gamedata element
        public SFSceneDescription CatElemToScene(int category, int element)
        {
            SFSceneDescription sd = new SFSceneDescription();
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
                    sd.add_line(SCENE_ITEM_TYPE.OBJ_SIMPLE, new string[] { m_name, "", m_name });

                    break;
                case 23:
                case 24:
                case 25:
                    //get item id
                    SFCategoryElement building = SFCategoryManager.gamedata.categories[category].get_element(element);
                    if (building == null)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Building not found (category is " + category.ToString()
                            + ", element is " + element.ToString() + ")");
                        break;
                    }
                    int building_id = (UInt16)building.get_single_variant(0).value;

                    //find item mesh
                    List<string> m_arr = mesh_data.GetBuildingMeshes(building_id);
                    if (m_arr == null)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Building mesh list not found (building id = " + building_id.ToString() + ")");
                        break;
                    }

                    //create scene
                    foreach (string m in m_arr)
                        sd.add_line(SCENE_ITEM_TYPE.OBJ_SIMPLE, new string[] { m, "", m });

                    break;
                case 33:
                case 34:
                case 35:
                    //get item id
                    SFCategoryElement obj = SFCategoryManager.gamedata.categories[category].get_element(element);
                    if (obj == null)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Object not found (category is " + category.ToString()
                            + ", element is " + element.ToString() + ")");
                        break;
                    }
                    int obj_id = (UInt16)obj.get_single_variant(0).value;

                    //find item mesh
                    List<string> m_lst = mesh_data.GetObjectMeshes(obj_id);
                    if (m_lst == null)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Object mesh list not found (building id = " + obj_id.ToString() + ")");
                        break;
                    }

                    //create scene
                    foreach (string m in m_lst)
                        sd.add_line(SCENE_ITEM_TYPE.OBJ_SIMPLE, new string[] { m, "", m });

                    break;
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                    //get unit id
                    SFCategoryElement un = SFCategoryManager.gamedata.categories[category].get_element(element);
                    if (un == null)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.CatElemToScene(): Unit not found (category is " + category.ToString()
                            + ", element is " + element.ToString() + ")");
                        break;
                    }
                    int un_id = (UInt16)un.get_single_variant(0).value;

                    MakeUnitScene(un_id, sd);
                    break;
                default:
                    break;
            }
            return sd;
        }

        // removes objects from the scene and resets scene data
        public void ClearScene()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFSceneManager.ClearScene() called");
            while (objects_static.Count != 0)
                DeleteObject(objects_static.Keys.ElementAt(0));

            while (objects_dynamic.Count != 0)
                DeleteObject(objects_dynamic.Keys.ElementAt(0));

            /*List<string> keys = objects_static.Keys.ToList();
            foreach (string k in keys)
                DeleteObject(k);
            keys = objects_dynamic.Keys.ToList();
            foreach (string k in keys)
                DeleteObject(k);*/
            scene_meta = null;
        }

        // adds a static object to the scene given mesh name, and assigns it a name and parent object
        public void AddObjectStatic(string mesh_name, string parent_name, string obj_name)
        {
            Object3D par = null;
            if (objects_static.ContainsKey(parent_name))
                par = objects_static[parent_name];
            else if (objects_dynamic.ContainsKey(parent_name))
                par = objects_dynamic[parent_name];

            bool loaded = true;
            if (mesh_name != "")
            {
                int result = SFResourceManager.Models.Load(mesh_name);
                if ((result != 0) && (result != -1))
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectStatic(): Mesh could not be loaded (mesh name: "
                    + mesh_name + ")");
                    loaded = false;
                }
            }
            else
                loaded = false;

            ObjectSimple3D obj_s1 = new ObjectSimple3D();
            obj_s1.Name = obj_name;
            if (par == null)
                obj_s1.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
            obj_s1.Parent = par;
            if (loaded)
                obj_s1.Mesh = SFResourceManager.Models.Get(mesh_name);
            objects_static.Add(obj_name, obj_s1);
            obj_s1.update_modelMatrix();
        }


        // adds an animated object to the scene given mesh name, and assigns it a name and parent object
        public void AddObjectDynamic(string skel_name, string parent_name, string obj_name)
        {
            Object3D par = null;
            if (objects_static.ContainsKey(parent_name))
                par = objects_static[parent_name];
            else if (objects_dynamic.ContainsKey(parent_name))
                par = objects_dynamic[parent_name];

            bool loaded = true;
            int result = SFResourceManager.Skeletons.Load(skel_name);
            if ((result != 0) && (result != -1))
            {
                loaded = false;
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectDynamic(): Skeleton could not be loaded (skeleton name: "
                    + skel_name + ")");
            }
            result = SFResourceManager.Skins.Load(skel_name);
            if ((result != 0) && (result != -1))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectDynamic(): Skin could not be loaded (skin name: "
                       + skel_name + ")");
                loaded = false;
            }

            objectAnimated obj_d1 = new objectAnimated();
            obj_d1.Name = obj_name;
            if (par == null)
                obj_d1.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
            obj_d1.Parent = par;
            if (loaded)
                obj_d1.SetSkeletonSkin(SFResourceManager.Skeletons.Get(skel_name), SFResourceManager.Skins.Get(skel_name));
            objects_dynamic.Add(obj_name, obj_d1);
            obj_d1.update_modelMatrix();
        }

        // adds a bone anchor object to the scene given animated object name and the object's skeleton bone name, and assigns it a name
        // bone anchor follows a bone on the scene
        public void AddObjectBoneanchor(string obj_anim_name, string skel_bone_name, string obj_name)
        {
            objectAnimated par = null;
            if (objects_dynamic.ContainsKey(obj_anim_name))
                par = objects_dynamic[obj_anim_name];
            else
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectBoneanchor(): Bone not found (object name: "
                    + obj_anim_name + ", bone name: " + skel_bone_name + ")");

            ObjectBoneAnchor obj_b1 = new ObjectBoneAnchor();
            obj_b1.Name = obj_name;
            obj_b1.Visible = false;
            obj_b1.Parent = par;
            obj_b1.SetBone(skel_bone_name);
            objects_dynamic.Add(obj_name, obj_b1);
            obj_b1.update_modelMatrix();
        }

        public void DeleteObject(string obj_name)
        {
            Object3D obj_s = null;

            if (objects_static.ContainsKey(obj_name))
            {
                obj_s = objects_static[obj_name];
                objects_static.Remove(obj_name);
            }
            else if (objects_dynamic.ContainsKey(obj_name))
            {
                obj_s = objects_dynamic[obj_name];
                objects_dynamic.Remove(obj_name);
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SF3D, "SFSceneManager.DeleteObject(): Object does not exist (obj name: "
                    + obj_name + ")");
                return;
            }
            
            obj_s.Dispose();

            for (int i = 0; i < obj_s.Children.Count; i++)
                DeleteObject(obj_s.Children[i].Name);
        }

        // adds unit to a scene
        public void AddObjectUnit(int unit_id, string object_name, bool animated = false)
        {
            if (!animated)
            {
                AddObjectStatic("", "", object_name);

                //find unit data element (cat 18)
                SFCategoryElement unit_data = SFCategoryManager.gamedata.categories[17].find_binary_element<UInt16>(0, (UInt16)unit_id);
                if (unit_data == null)
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectUnit(): Unit does not exist (unit id = "
                        + unit_id + ")");
                    return;
                }
                //find unit eq element (cat 19)
                SFCategoryElement unit_eq = SFCategoryManager.gamedata.categories[18].find_binary_element<UInt16>(0, (UInt16)unit_id);
                if (unit_eq == null)
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectUnit(): Unit has no inventory assigned to it (unit id = "
                        + unit_id + ")");
                    return;
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
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectUnit(): Unit does not have chestpiece assigned (unit id = "
                        + unit_id + ")");
                    return;
                }
                //find chest skin/animations
                string chest_name = mesh_data.GetItemMesh(chest_id, is_female);
                if (chest_name == "")
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectUnit(): Undefined chestpiece mesh (unit id = "
                        + unit_id + ")");
                    return;
                }
                string anim_name = mesh_data.GetItemAnimation(chest_id, is_female);

                //special case: monument unit, needs to be considered separately
                string unit_handle = Utility.CleanString(unit_data.get_single_variant(10));
                if ((unit_handle.StartsWith("Unit")) && (!unit_handle.Contains("Titan")))
                {
                    chest_name += "_cold";
                }


                //add anim model to scene
                AddObjectDynamic(chest_name, object_name, object_name + "_CHEST");
                // apply flat shade
                objectAnimated uo = objects_dynamic[object_name + "_CHEST"];
                if (uo.skin != null)
                    foreach (SFModelSkinChunk msc in uo.skin.submodels)
                        msc.material.flat_shade = true;

                //get legs item (5) (animated)
                UInt16 legs_id = GetItemID(unit_eq, 5);
                if (chest_id != 0)
                {
                    //find chest skin/animations
                    string legs_name = mesh_data.GetItemMesh(legs_id, is_female);
                    if (legs_name != "")
                    {
                        AddObjectDynamic(legs_name, object_name, object_name + "_LEGS");
                        uo = objects_dynamic[object_name + "_LEGS"];
                        if (uo.skin != null)
                            foreach (SFModelSkinChunk msc in uo.skin.submodels)
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
                        AddObjectDynamic(head_name, object_name, object_name + "_HEAD");
                        uo = objects_dynamic[object_name + "_HEAD"];
                        if (uo.skin != null)
                            foreach (SFModelSkinChunk msc in uo.skin.submodels)
                                msc.material.flat_shade = true;
                    }
                    else
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectUnit(): Unit head has undefined mesh (unit id = "
                            + unit_id.ToString() + ", head id = "+head_id.ToString()+")");
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
                        AddObjectBoneanchor(object_name + "_CHEST", "Head", object_name + "_HEADBONE");
                        AddObjectStatic(helmet_name, object_name + "_HEADBONE", object_name + "_HELMET");
                        ObjectSimple3D ho = objects_static[object_name + "_HELMET"];
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
                        AddObjectBoneanchor(object_name + "_CHEST", "R Hand weapon", object_name + "_RHANDBONE");
                        AddObjectStatic(rhand_name, object_name + "_RHANDBONE", object_name + "_RHAND");
                        ObjectSimple3D ho = objects_static[object_name + "_RHAND"];
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
                        AddObjectBoneanchor(object_name + "_CHEST", (is_shield ? "L Forearm shield" : "L Hand weapon"), object_name + "_LHANDBONE");
                        AddObjectStatic(lhand_name, object_name + "_LHANDBONE", object_name + "_LHAND");
                        ObjectSimple3D ho = objects_static[object_name + "_LHAND"];
                        if (ho.Mesh != null)
                            foreach (SFMaterial mat in ho.Mesh.materials)
                                mat.flat_shade = true;
                    }
                }
            }
        }

        // adds game object to the scene
        public void AddObjectObject(int object_id, string object_name, bool apply_shading)
        {
            List<string> m_lst = mesh_data.GetObjectMeshes(object_id);

            // create root
            AddObjectStatic("", "", object_name);

            if (m_lst == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectObject(): Object mesh is usassigned (object id = "
                    + object_id + ")");
                return;
            }

            // add meshes
            for (int i = 0; i < m_lst.Count; i++)
            {
                string m = m_lst[i];
                string mesh_obj_name = object_name + "_" + i.ToString();
                AddObjectStatic(m, object_name, mesh_obj_name);
                if (objects_static[mesh_obj_name].Mesh != null)
                    foreach (SFMaterial mat in objects_static[mesh_obj_name].Mesh.materials)
                        mat.apply_shading = apply_shading;
            }
        }

        // adds building to the scene
        public void AddObjectBuilding(int building_id, string object_name)
        {
            List<string> m_lst = mesh_data.GetBuildingMeshes(building_id);

            // create root
            AddObjectStatic("", "", object_name);

            if (m_lst == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSceneManager.AddObjectBuilding(): Building mesh is usassigned (building id = "
                    + building_id + ")");
                return;
            }

            // add meshes
            for (int i = 0; i < m_lst.Count; i++)
            {
                string m = m_lst[i];
                AddObjectStatic(m, object_name, object_name + "_" + i.ToString());
            }
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

        // updates scene
        // if time_flow is true, updates delta time
        public void LogicStep(bool time_flow = true)
        {
            if (time_flow)
            {
                delta_timer.Stop();
                deltatime = delta_timer.ElapsedMilliseconds / (float)1000;
                delta_timer.Restart();
            }
            else
                deltatime = 0f;

            // 1st pass: gather modified objects
            List<ObjectSimple3D> modified_static = new List<ObjectSimple3D>();
            foreach (ObjectSimple3D obj in objects_static.Values)
                if (obj.Modified)
                    modified_static.Add(obj);

            List<objectAnimated> modified_dynamic = new List<objectAnimated>();
            foreach (objectAnimated obj in objects_dynamic.Values)
                if (obj.Modified)
                    modified_dynamic.Add(obj);

            // 2nd pass: update objects
            foreach (ObjectSimple3D obj in modified_static)
                obj.update_modelMatrix();

            foreach (objectAnimated obj in modified_dynamic)
                obj.update_modelMatrix();

            // 3rd pass: update animations
            foreach (objectAnimated obj in objects_dynamic.Values)
            {
                if (obj.skin == null)
                    continue;
                if (obj.anim_playing)
                    obj.set_animation_time(current_time);
            }

            current_time += deltatime;
            if(scene_meta != null)
                if (current_time > scene_meta.duration)
                    current_time -= scene_meta.duration;

            frame_counter++;
        }

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
    }
}
