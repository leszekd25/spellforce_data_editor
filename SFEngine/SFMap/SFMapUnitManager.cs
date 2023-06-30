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

        public int AddUnit(int game_id, SFCoord position, int flags, int npc_id, int unknown, int group, int unknown2, int index = -1)
        {
            SFMapUnit unit = new SFMapUnit();
            unit.grid_position = position;
            unit.unknown_flags = flags;
            unit.game_id = game_id;
            unit.npc_id = npc_id;
            unit.unknown = unknown;
            unit.group = group;
            unit.unknown2 = unknown2;

            if (index == -1)
            {
                index = units.Count;
            }

            units.Insert(index, unit);

            string obj_name = unit.GetName();
            unit.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneUnit(game_id, obj_name);
            unit.node.SetParent(map.heightmap.GetChunkNode(position));

            map.heightmap.GetChunk(position).units.Add(unit);

            map.heightmap.SetFlag(unit.grid_position, SFMapHeightMapFlag.ENTITY_UNIT, true);

            // modify object transform and appearance
            unit.node.Position = map.heightmap.GetFixedPosition(position);
            unit.node.SetAnglePlane(0);
            // find unit scale
            int unit_index = SFCFF.SFCategoryManager.gamedata[2024].GetElementIndex(game_id);
            float unit_size = 1f;

            SFCFF.SFCategoryElement unit_data = SFCFF.SFCategoryManager.gamedata[2024][unit_index];
            unit_index = SFCFF.SFCategoryManager.gamedata[2005].GetElementIndex((ushort)unit_data[2]);
            if (SFCFF.SFCategoryManager.gamedata[2005] == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.AddUnit(): There is no unit stats block in gamedata, setting unit scale to 100%");
            }
            else
            {
                if (unit_index != -1)
                {
                    unit_data = SFCFF.SFCategoryManager.gamedata[2005][unit_index];
                    unit_size = Math.Min((ushort)200, Math.Max((ushort)unit_data[18], (ushort)50)) / 100.0f;
                }
                else
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.AddUnit(): Could not find unit stats data (unit id = " + game_id.ToString() + "), setting unit scale to 100%");
                }
            }
            unit.node.Scale = new OpenTK.Vector3(unit_size * 100 / 128);

            if (Settings.DynamicMap)
            {
                RestartAnimation(unit);
            }

            return index;
        }

        public void RotateUnit(int unit_map_index, int angle)
        {
            SFMapUnit unit = units[unit_map_index];

            unit.node.SetAnglePlane(angle);
        }

        public void ReplaceUnit(int unit_map_index, ushort new_unit_id)
        {
            SFMapUnit unit = units[unit_map_index];

            if (unit.node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(unit.node);
            }

            unit.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneUnit(new_unit_id, unit.GetName());
            unit.node.SetParent(map.heightmap.GetChunkNode(unit.grid_position));

            unit.game_id = new_unit_id;

            // object transform
            unit.node.Position = map.heightmap.GetFixedPosition(unit.grid_position);
            unit.node.SetAnglePlane(0);
            // find unit scale
            int unit_index = SFCFF.SFCategoryManager.gamedata[2024].GetElementIndex(unit.game_id);
            float unit_size = 1f;

            SFCFF.SFCategoryElement unit_data = SFCFF.SFCategoryManager.gamedata[2024][unit_index];
            unit_index = SFCFF.SFCategoryManager.gamedata[2005].GetElementIndex((ushort)unit_data[2]);
            if (SFCFF.SFCategoryManager.gamedata[2005] == null)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.ReplaceUnit(): There is no unit stats block in gamedata, setting unit scale to 100%");
            }
            else
            {
                if (unit_index != -1)
                {
                    unit_data = SFCFF.SFCategoryManager.gamedata[2005][unit_index];
                    unit_size = Math.Min((ushort)200, Math.Max((ushort)unit_data[18], (ushort)50)) / 100.0f;
                }
                else
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.ReplaceUnit(): Could not find unit stats data (unit id = " + unit.game_id.ToString() + "), setting unit scale to 100%");
                }
            }
            unit.node.Scale = new OpenTK.Vector3(unit_size * 100 / 128);

            if (Settings.DynamicMap)
            {
                RestartAnimation(unit);
            }
        }

        public void RemoveUnit(int unit_index)
        {
            SFMapUnit unit = units[unit_index];

            units.Remove(unit);
            map.heightmap.SetFlag(unit.grid_position, SFMapHeightMapFlag.ENTITY_UNIT, false);

            if (unit.node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(unit.node);
            }

            map.heightmap.GetChunk(unit.grid_position).units.Remove(unit);

        }

        public void MoveUnit(int unit_map_index, SFCoord new_pos)
        {
            SFMapUnit unit = units[unit_map_index];

            // move unit and set chunk dependency
            map.heightmap.GetChunkNode(unit.grid_position).MapChunk.units.Remove(unit);
            map.heightmap.SetFlag(unit.grid_position, SFMapHeightMapFlag.ENTITY_UNIT, false);
            unit.grid_position = new_pos;
            map.heightmap.SetFlag(unit.grid_position, SFMapHeightMapFlag.ENTITY_UNIT, true);
            map.heightmap.GetChunkNode(unit.grid_position).MapChunk.units.Add(unit);
            unit.node.SetParent(map.heightmap.GetChunkNode(unit.grid_position));

            // change visual transform
            unit.node.Position = map.heightmap.GetFixedPosition(new_pos);
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
                SFSkeleton skel = unit.node.FindNode<SF3D.SceneSynchro.SceneNodeAnimated>("Chest").Skeleton;
                if(!SFResources.SFResourceManager.Animations.Load(anim_name, SFUnPak.FileSource.ANY, out SFAnimation anim, out int ec, skel))
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFMapUnitManager.RestartAnimation(): Could not load animation (animation name = " + anim_name + ")");
                }
                else
                {
                    foreach (SF3D.SceneSynchro.SceneNodeAnimated anim_node in unit.node.Children)
                    {
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
