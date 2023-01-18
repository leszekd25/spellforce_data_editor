using SFEngine.SF3D;
using System;
using System.Collections.Generic;

namespace SFEngine.SFMap
{
    public class SFMapUnit : SFMapEntity
    {
        static int max_id = 0;

        public int unknown_flags = 0;
        public int unknown = 0;
        public int group = 0;
        public int unknown2 = 0;

        public override string GetName()
        {
            return "UNIT_" + id.ToString();
        }

        public SFMapUnit()
        {
            id = max_id;
            max_id += 1;
        }
    }

    public class SFMapUnitManager
    {
        // anim lib, idle anim name
        public Dictionary<string, string> idle_anim_dict = new Dictionary<string, string>();
        public List<SFMapUnit> units { get; private set; } = new List<SFMapUnit>();
        public SFMap map = null;

        public SFMapUnitManager()
        {
            // generate unknown idle values (see object/object_figure_init.lua)
            idle_anim_dict.Add("figure_npc_gargoyle_normal", "figure_npc_gargoyle_idle");
            idle_anim_dict.Add("figure_animal_buffalo_normal", "figure_animal_buffalo_idle");
            idle_anim_dict.Add("figure_animal_wolf_", "figure_animal_wolf_idle");
            idle_anim_dict.Add("figure_npc_beastman", "figure_npc_beastman_idle");
            idle_anim_dict.Add("figure_npc_demon_lesser", "figure_npc_demon_lesser_idle");
            idle_anim_dict.Add("figure_npc_gargoyle_stone", "figure_npc_gargoyle_idle");
            idle_anim_dict.Add("figure_npc_giant_", "figure_npc_giant_idle");
            idle_anim_dict.Add("figure_npc_minotaur_greater", "figure_npc_minotaur_greater_idle");
            idle_anim_dict.Add("figure_npc_minotaur_lesser", "figure_npc_minotaur_lesser_idle");
            idle_anim_dict.Add("figure_npc_ogre_greater", "figure_npc_ogre_idle");
            idle_anim_dict.Add("figure_npc_ogre_normal", "figure_npc_ogre_idle");
            idle_anim_dict.Add("figure_npc_spectre_normal", "figure_npc_spectre_idle");
            idle_anim_dict.Add("figure_npc_wraith_normal", "figure_npc_wraith_idle");
            idle_anim_dict.Add("figure_npc_zombie_normal_", "figure_npc_zombie_idle");
        }

        public SFMapUnit AddUnit(int id, SFCoord position, int flags, int index)
        {
            SFMapUnit unit = new SFMapUnit();
            unit.grid_position = position;
            unit.unknown_flags = flags;
            unit.game_id = id;

            if (index == -1)
            {
                index = units.Count;
            }

            units.Insert(index, unit);

            string obj_name = unit.GetName();
            unit.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneUnit(id, obj_name);
            unit.node.SetParent(map.heightmap.GetChunkNode(position));

            // 3. add new unit in respective chunk
            map.heightmap.GetChunk(position).AddUnit(unit);

            return unit;
        }

        public void RemoveUnit(SFMapUnit u)
        {
            units.Remove(u);

            if (u.node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(u.node);
            }

            map.heightmap.GetChunk(u.grid_position).RemoveUnit(u);
        }

        public int GetHighestGroup()
        {
            int m_group = 0;
            for (int i = 0; i < units.Count; i++)
            {
                m_group = Math.Max(m_group, units[i].group);
            }

            return m_group;
        }

        // fallback
        private string FindIdleAnim(string anim_lib)
        {
            while (anim_lib != "")
            {
                string cur_name = "";
                int cur_len = 99999;
                foreach (string anim_name in SFResources.SFResourceManager.animation_names)
                {
                    if ((anim_name.StartsWith(anim_lib)) && (anim_name.Contains("idle")))
                    {
                        if(anim_name.Length < cur_len)
                        {
                            cur_name = anim_name;
                            cur_len = anim_name.Length;
                        }
                    }
                }
                if(cur_name != "")
                {
                    return cur_name;
                }

                int last_index = anim_lib.LastIndexOf('_');
                if (last_index < 0)
                {
                    break;
                }

                anim_lib = anim_lib.Substring(0, last_index);
            }

            return "";
        }

        public string GetIdleAnim(string anim_lib)
        {
            if (idle_anim_dict.ContainsKey(anim_lib))
            {
                return idle_anim_dict[anim_lib];
            }

            string res = FindIdleAnim(anim_lib);
            if (res != "")
            {
                idle_anim_dict[anim_lib] = res;
            }

            return res;
        }

        public string GetAnimLib(SFMapUnit unit)
        {
            ushort chest_id = SFCFF.SFCategoryManager.GetUnitItem((ushort)(unit.game_id), 2);
            if (chest_id != 0)
            {
                string anim_lib = SFLua.SFLuaEnvironment.items[chest_id].AnimSet;
                if (anim_lib == "")
                {
                    SFCFF.SFCategoryElement unit_data = SFCFF.SFCategoryManager.gamedata[2024].FindElementBinary<UInt16>(0, (UInt16)(unit.game_id));
                    if (unit_data == null)
                    {
                        return "";
                    }

                    SFCFF.SFCategoryElement unit_stats = SFCFF.SFCategoryManager.gamedata[2005].FindElementBinary<UInt16>(0, (UInt16)unit_data[2]);
                    bool is_female = false;
                    if (unit_stats != null)
                    {
                        is_female = ((Byte)unit_stats[21] % 2) == 1;
                    }

                    if (is_female)
                    {
                        return "figure_hero_female";
                    }
                    else
                    {
                        return "figure_hero_male";
                    }
                }

                return anim_lib;
            }

            return "";
        }

        public void RestartAnimation(SFMapUnit unit)
        {
            string anim_name = GetIdleAnim(GetAnimLib(unit));
            if (anim_name != "")
            {
                SFAnimation anim = null;
                int tex_code = SFResources.SFResourceManager.Animations.Load(anim_name, SFUnPak.FileSource.ANY);
                if ((tex_code != 0) && (tex_code != -1))
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFMapUnitManager.RestartAnimation(): Could not load animation (animation name = " + anim_name + ")");
                }
                else
                {
                    anim = SFResources.SFResourceManager.Animations.Get(anim_name);
                    foreach (SF3D.SceneSynchro.SceneNodeAnimated anim_node in unit.node.Children)
                    {
                        if (anim_node.Primary != null)
                        {
                            continue;
                        }

                        anim_node.SetAnimation(anim);
                        if (anim_node.Animation != null)
                        {
                            anim_node.SetAnimationCurrentTime(MathUtils.Randf(0, anim_node.Animation.max_time));
                        }
                    }
                }
            }
        }
    }
}
