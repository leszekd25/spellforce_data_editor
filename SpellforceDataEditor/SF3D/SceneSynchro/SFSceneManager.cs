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

namespace SpellforceDataEditor.SF3D.SceneSynchro
{
    public class SFSceneManager
    {
        public SFSceneDescriptionMeta scene_meta { get; private set; } = null;
        public Dictionary<string, ObjectSimple3D> objects_static { get; private set; } = new Dictionary<string, ObjectSimple3D>();
        public Dictionary<string, objectAnimated> objects_dynamic { get; private set; } = new Dictionary<string, objectAnimated>();

        private int frame_counter = 0;
        public int frames_per_second { get; private set; } = 25;
        public System.Diagnostics.Stopwatch delta_timer { get; private set; } = new System.Diagnostics.Stopwatch();
        private float deltatime = 0f;
        public float current_time { get; private set; } = 0f;

        SFVisualLinkContainer mesh_data = new SFVisualLinkContainer();
        public SFResources.SFResourceManager resources { get; set; } = null;

        public void Init()
        {
            delta_timer.Start();
        }

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
                        break;
                }
            }
        }

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

        private void MakeUnitScene(int unit_id, SFSceneDescription sd)
        {
            //find unit data element (cat 18)
            SFCategoryElement unit_data = SFCategoryManager.get_category(17).find_binary_element<UInt16>(0, (UInt16)unit_id);
            if (unit_data == null)
                return;
            //find unit eq element (cat 19)
            SFCategoryElement unit_eq = SFCategoryManager.get_category(18).find_binary_element<UInt16>(0, (UInt16)unit_id);
            if (unit_eq == null)
                return;

            //get unit gender
            SFCategoryElement unit_stats = SFCategoryManager.get_category(3).find_binary_element<UInt16>
                                                                     (0, (UInt16)unit_data.get_single_variant(2).value);
            bool is_female = false;
            if (unit_stats != null)
                is_female = ((Byte)unit_stats.get_single_variant(23).value % 2) == 1;

            //get chest item (2) (animated)
            UInt16 chest_id = GetItemID(unit_eq, 2);
            if (chest_id == 0)
                return;
            //find chest skin/animations
            string chest_name = mesh_data.GetItemMesh(chest_id, is_female);
            if (chest_name == "")
                return;
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
                    SFCategoryElement item_data = SFCategoryManager.get_category(6).find_binary_element<UInt16>(0, lhand_id);
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
                    SFCategoryElement item = SFCategoryManager.get_category(category).get_element(element);
                    if (item == null)
                        break;
                    int item_id = (UInt16)item.get_single_variant(0).value;

                    //find item mesh
                    string m_name = mesh_data.GetItemMesh(item_id, false);
                    if (m_name == "")
                        break;

                    //create scene
                    sd.add_line(SCENE_ITEM_TYPE.OBJ_SIMPLE, new string[] { m_name, "", m_name });

                    break;
                case 23:
                case 24:
                case 25:
                    //get item id
                    SFCategoryElement building = SFCategoryManager.get_category(category).get_element(element);
                    if (building == null)
                        break;
                    int building_id = (UInt16)building.get_single_variant(0).value;

                    //find item mesh
                    List<string> m_arr = mesh_data.GetBuildingMeshes(building_id);
                    if (m_arr == null)
                        break;

                    //create scene
                    foreach (string m in m_arr)
                        sd.add_line(SCENE_ITEM_TYPE.OBJ_SIMPLE, new string[] { m, "", m });

                    break;
                case 33:
                case 34:
                case 35:
                    //get item id
                    SFCategoryElement obj = SFCategoryManager.get_category(category).get_element(element);
                    if (obj == null)
                        break;
                    int obj_id = (UInt16)obj.get_single_variant(0).value;

                    //find item mesh
                    List<string> m_lst = mesh_data.GetObjectMeshes(obj_id);
                    if (m_lst == null)
                        break;

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
                    SFCategoryElement un = SFCategoryManager.get_category(category).get_element(element);
                    if (un == null)
                        break;
                    int un_id = (UInt16)un.get_single_variant(0).value;

                    MakeUnitScene(un_id, sd);
                    break;
                default:
                    break;
            }
            return sd;
        }

        public void ClearScene()
        {
            objects_static.Clear();
            objects_dynamic.Clear();
            scene_meta = null;
        }

        public void AddObjectStatic(string mesh_name, string parent_name, string obj_name)
        {
            Object3D par = null;
            if (objects_static.ContainsKey(parent_name))
                par = objects_static[parent_name];
            else if (objects_dynamic.ContainsKey(parent_name))
                par = objects_dynamic[parent_name];

            bool loaded = true;
            int result = resources.Models.Load(mesh_name);
            if ((result != 0) && (result != -1))
                loaded = false;

            ObjectSimple3D obj_s1 = new ObjectSimple3D();
            if (par == null)
                obj_s1.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
            obj_s1.Parent = par;
            if (loaded)
                obj_s1.Mesh = resources.Models.Get(mesh_name);
            objects_static.Add(obj_name, obj_s1);
            obj_s1.update_modelMatrix();
        }

        public void AddObjectDynamic(string skel_name, string parent_name, string obj_name)
        {
            Object3D par = null;
            if (objects_static.ContainsKey(parent_name))
                par = objects_static[parent_name];
            else if (objects_dynamic.ContainsKey(parent_name))
                par = objects_dynamic[parent_name];

            bool loaded = true;
            int result = resources.Skeletons.Load(skel_name);
            if ((result != 0) && (result != -1))
                loaded = false;
            result = resources.Skins.Load(skel_name);
            if ((result != 0) && (result != -1))
                loaded = false;

            objectAnimated obj_d1 = new objectAnimated();
            if (par == null)
                obj_d1.Rotation = Quaternion.FromAxisAngle(new Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
            obj_d1.Parent = par;
            if (loaded)
                obj_d1.SetSkeletonSkin(resources.Skeletons.Get(skel_name), resources.Skins.Get(skel_name));
            objects_dynamic.Add(obj_name, obj_d1);
            obj_d1.update_modelMatrix();
        }

        public void AddObjectBoneanchor(string obj_anim_name, string skel_bone_name, string obj_name)
        {
            objectAnimated par = null;
            if (objects_dynamic.ContainsKey(obj_anim_name))
                par = objects_dynamic[obj_anim_name];

            ObjectBoneAnchor obj_b1 = new ObjectBoneAnchor();
            obj_b1.Visible = false;
            obj_b1.SetBone(par, skel_bone_name);
            objects_dynamic.Add(obj_name, obj_b1);
            obj_b1.update_modelMatrix();
        }

        public void SetSceneTime(float t)
        {
            if (t < 0)
                t = 0;
            if (t > scene_meta.duration)
                t = scene_meta.duration;
            current_time = t;
        }

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

            foreach (ObjectSimple3D obj in objects_static.Values)
            {
                if (obj.Modified)
                    obj.update_modelMatrix();
            }

            foreach (objectAnimated obj in objects_dynamic.Values)
            {
                if (obj.Modified)
                    obj.update_modelMatrix();
                if (obj.skin == null)
                    continue;
                if (obj.anim_playing)
                    obj.set_animation_time(current_time);
            }

            current_time += deltatime;
            if (current_time > scene_meta.duration)
                current_time -= scene_meta.duration;

            frame_counter++;
        }
    }
}
