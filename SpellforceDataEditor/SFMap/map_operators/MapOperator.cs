using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap.map_operators
{
    public interface IMapOperator
    {
        // returns whether operator is ready to use
        bool Finished { get; set; }
        // if this is set to true, operator will call Apply() when it's pushed to queue
        // this is helpful to remove duplicate code where possible
        bool ApplyOnPush { get; set; }
        // operator holds map state before and after the action is performed
        // map state before is filled out when the operator is created
        // map state after is filled out in this function, which is called after the action is performed
        // sometimes the state after is known before the action is performed, in which case no additional actions are needed here
        void Finish(SFMap map);
        // performs an action given the operator's inputs
        void Apply(SFMap map);
        // performs a reverse of an action given the operator's inputs
        void Revert(SFMap map);
    }

    // a special type of operator, for when there are multiple sub-operators that should perform its actions in one go
    public class MapOperatorCluster: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public List<IMapOperator> SubOperators = new List<IMapOperator>();

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        public void Apply(SFMap map)
        {
            for (int i = 0; i < SubOperators.Count; i++)
                SubOperators[i].Apply(map);
        }

        public void Revert(SFMap map)
        {
            for (int i = SubOperators.Count - 1; i >= 0; i--)
                SubOperators[i].Revert(map);
        }

        public override string ToString()
        {
            return "cluster ("+SubOperators.Count+" sub-operators)";
        }
    }

    public class MapOperatorTerrainHeight : IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public Dictionary<SFCoord, ushort> PreOperatorHeights = new Dictionary<SFCoord, ushort>();
        public Dictionary<SFCoord, ushort> PostOperatorHeights = new Dictionary<SFCoord, ushort>();

        // when operator is created, the terrain is not yet modified, and PreOperatorHeights can be filled out
        // when Finish() is used, the terrain has been modified, so PostOperatorHeights can now be filled out
        public void Finish(SFMap map)
        {
            foreach (var kv in PreOperatorHeights)
                PostOperatorHeights.Add(kv.Key, map.heightmap.height_data[kv.Key.y * map.width + kv.Key.x]);

            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            SFCoord tl, br;
            map.heightmap.GetBoxFromArea(PostOperatorHeights.Keys, out tl, out br);

            map.heightmap.RebuildGeometry(tl, br);
            MainForm.mapedittool.ui.RedrawMinimap(PostOperatorHeights.Keys);
            MainForm.mapedittool.update_render = true;
        }

        public void Apply(SFMap map)
        {
            foreach (var kv in PostOperatorHeights)
                map.heightmap.height_data[kv.Key.y * map.width + kv.Key.x] = kv.Value;

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            foreach (var kv in PreOperatorHeights)
                map.heightmap.height_data[kv.Key.y * map.width + kv.Key.x] = kv.Value;

            UpdateMap(map);
        }

        public override string ToString()
        {
            return "set_height (count: " + PostOperatorHeights.Count + ")";
        }
    }

    public class MapOperatorTerrainTexture : IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public Dictionary<SFCoord, byte> PreOperatorTextures = new Dictionary<SFCoord, byte>();
        public Dictionary<SFCoord, byte> PostOperatorTextures = new Dictionary<SFCoord, byte>();

        public void Finish(SFMap map)
        {
            foreach (var kv in PreOperatorTextures)
                PostOperatorTextures.Add(kv.Key, map.heightmap.tile_data[kv.Key.y * map.width + kv.Key.x]);

            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            SFCoord tl, br;
            map.heightmap.GetBoxFromArea(PostOperatorTextures.Keys, out tl, out br);

            map.heightmap.RebuildTerrainTexture(tl, br);
            map.heightmap.RefreshOverlay();
            MainForm.mapedittool.ui.RedrawMinimap(PostOperatorTextures.Keys);
            MainForm.mapedittool.update_render = true;
        }

        public void Apply(SFMap map)
        {
            foreach (var kv in PostOperatorTextures)
                map.heightmap.tile_data[kv.Key.y * map.width + kv.Key.x] = kv.Value;

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            foreach (var kv in PreOperatorTextures)
                map.heightmap.tile_data[kv.Key.y * map.width + kv.Key.x] = kv.Value;

            UpdateMap(map);
        }

        public override string ToString()
        {
            return "set_texture (count: " + PostOperatorTextures.Count + ")";
        }
    }

    public class MapOperatorTerrainFlag: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public Dictionary<SFCoord, byte> PreOperatorFlags = new Dictionary<SFCoord, byte>();
        public Dictionary<SFCoord, byte> PostOperatorFlags = new Dictionary<SFCoord, byte>();

        public void Finish(SFMap map)
        {
            foreach (var kv in PreOperatorFlags)
                PostOperatorFlags.Add(kv.Key, map.heightmap.tile_data[kv.Key.y * map.width + kv.Key.x]);

            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            map.heightmap.RefreshOverlay();
            MainForm.mapedittool.update_render = true;
        }

        public void Apply(SFMap map)
        {
            foreach (var kv in PostOperatorFlags)
                map.heightmap.overlay_data_flags[kv.Key.y * map.width + kv.Key.x] = kv.Value;

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            foreach (var kv in PreOperatorFlags)
                map.heightmap.overlay_data_flags[kv.Key.y * map.width + kv.Key.x] = kv.Value;

            UpdateMap(map);
        }

        public override string ToString()
        {
            return "set_flag (count: " + PostOperatorFlags.Count + ")";
        }
    }

    public class MapOperatorLake: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public SFCoord pos;
        public short z_diff;
        public int type;
        public int lake_index;
        public bool change_add;

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            MainForm.mapedittool.ui.RedrawMinimap();
            MainForm.mapedittool.update_render = true;
        }

        public void Apply(SFMap map)
        {
            if (change_add)
                map.lake_manager.AddLake(pos, z_diff, type, lake_index);
            else
            {
                int lake_index = map.lake_manager.GetLakeIndexAt(pos);
                if (lake_index == Utility.NO_INDEX)
                    return;

                map.lake_manager.RemoveLake(map.lake_manager.lakes[lake_index]);
            }

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            if (!change_add)
                map.lake_manager.AddLake(pos, z_diff, type, lake_index);
            else
            {
                int lake_index = map.lake_manager.GetLakeIndexAt(pos);
                if (lake_index == Utility.NO_INDEX)
                    return;

                map.lake_manager.RemoveLake(map.lake_manager.lakes[lake_index]);
            }

            UpdateMap(map);
        }

        public override string ToString()
        {
            return "set_lake (index: "+lake_index.ToString()+", pos: " + pos.ToString() + ", mode: " + (change_add ? "add" : "remove") + ")";
        }
    }

    public class MapOperatorLakeType: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public int lake_index;
        public int PreOperatorType;
        public int PostOperatorType;

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            map.lake_manager.RebuildLake(map.lake_manager.lakes[lake_index]);
            MainForm.mapedittool.ui.RedrawMinimap();
            MainForm.mapedittool.update_render = true;
        }

        public void Apply(SFMap map)
        {
            map.lake_manager.lakes[lake_index].type = PostOperatorType;

            // update UI if this is the currently selected lake
            if (MainForm.mapedittool.selected_inspector is map_controls.MapLakeInspector)
            {
                if (((map_controls.MapLakeInspector)MainForm.mapedittool.selected_inspector).selected_lake == map.lake_manager.lakes[lake_index])
                    ((map_controls.MapLakeInspector)MainForm.mapedittool.selected_inspector).OnSelect(map.lake_manager.lakes[lake_index]);
            }

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            map.lake_manager.lakes[lake_index].type = PreOperatorType;
            
            // update UI if this is the currently selected lake
            if (MainForm.mapedittool.selected_inspector is map_controls.MapLakeInspector)
            {
                if (((map_controls.MapLakeInspector)MainForm.mapedittool.selected_inspector).selected_lake == map.lake_manager.lakes[lake_index])
                    ((map_controls.MapLakeInspector)MainForm.mapedittool.selected_inspector).OnSelect(map.lake_manager.lakes[lake_index]);
            }

            UpdateMap(map);
        }

        public override string ToString()
        {
            return "set_type (index: " + lake_index.ToString() + ", type: " + PreOperatorType.ToString() + " -> " + PostOperatorType.ToString() + ")";
        }
    }

    public class MapOperatorModifyTextureSet : IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public Dictionary<int, int> PreOperatorTextureIDMap = new Dictionary<int, int>();
        public Dictionary<int, int> PostOperatorTextureIDMap = new Dictionary<int, int>();

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        public void SetTexture(SFMap map, Dictionary<int, int> tex_id_map)
        {
            foreach (var kv in tex_id_map)
                map.heightmap.texture_manager.SetBaseTexture(kv.Key, kv.Value);

            // ui update code here
            MainForm.mapedittool.update_render = true;
            MainForm.mapedittool.ui.RedrawMinimap();
            MainForm.mapedittool.external_operator_ModifyTextureSet();
        }

        public void Apply(SFMap map)
        {
            SetTexture(map, PostOperatorTextureIDMap);
        }

        public void Revert(SFMap map)
        {
            SetTexture(map, PreOperatorTextureIDMap);
        }

        public override string ToString()
        {
            return "modify_texture_set (texture count: " + PreOperatorTextureIDMap.Count.ToString() + ")";
        }
    }

    public class MapOperatorAddOrRemoveTileType: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public int tile_index;
        public bool is_adding = true;

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        private void Add(SFMap map)
        {
            map.heightmap.texture_manager.tile_defined[tile_index] = true;
            MainForm.mapedittool.external_operator_ModifyTextureSet();
        }

        private void Remove(SFMap map)
        {
            map.heightmap.texture_manager.tile_defined[tile_index] = false;
            MainForm.mapedittool.external_operator_ModifyTextureSet();
        }

        public void Apply(SFMap map)
        {
            if (is_adding)
                Add(map);
            else
                Remove(map);
        }

        public void Revert(SFMap map)
        {
            if (is_adding)
                Remove(map);
            else
                Add(map);
        }

        public override string ToString()
        {
            return (is_adding ? "add_tile" : "remove_tile") + " (index: " + tile_index.ToString() + ")";
        }
    }

    public class MapOperatorTileChangeState: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public byte tile_index;
        public SFMapTerrainTextureTileData PreOperatorTileState;
        public SFMapTerrainTextureTileData PostOperatorTileState;

        public void Finish(SFMap map)
        {
            PostOperatorTileState = map.heightmap.texture_manager.texture_tiledata[tile_index];

            Finished = true;
        }

        public void SetTileState(SFMap map, byte tile_index, SFMapTerrainTextureTileData state)
        {
            map.heightmap.texture_manager.texture_tiledata[tile_index] = state;

            MainForm.mapedittool.external_operator_TileChangeState(tile_index);
        }

        public void Apply(SFMap map)
        {
            SetTileState(map, tile_index, PostOperatorTileState);
        }

        public void Revert(SFMap map)
        {
            SetTileState(map, tile_index, PreOperatorTileState);
        }

        public override string ToString()
        {
            return "tile_change_state (index: " + tile_index.ToString() + ", state: " + PreOperatorTileState.ToString() + " -> " + PostOperatorTileState.ToString() + ")";
        }
    }

    public class MapOperatorUnitAddOrRemove : IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public SFMapUnit unit;
        public int index;
        public bool is_adding;    // true - entityadd, false: entityremove

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            MainForm.mapedittool.ui.RedrawMinimapIcons();
            MainForm.mapedittool.update_render = true;
        }

        private void Add(SFMap map)
        {
            map.AddUnit(unit.game_id, unit.grid_position, unit.unknown_flags, unit.npc_id, unit.unknown, unit.group, unit.unknown2, index);

            if (MainForm.mapedittool.selected_inspector is map_controls.MapUnitInspector)
            {
                ((map_controls.MapUnitInspector)MainForm.mapedittool.selected_inspector).LoadNextUnit(index);
            }
        }

        private void Remove(SFMap map)
        {
            if (MainForm.mapedittool.selected_inspector is map_controls.MapUnitInspector)
            {
                if (index == ((MapEdit.MapUnitEditor)MainForm.mapedittool.selected_editor).selected_unit)
                {
                    MainForm.mapedittool.selected_editor.Select(Utility.NO_INDEX);
                    MainForm.mapedittool.InspectorSelect(null);
                }

                map.DeleteUnit(index);

                ((map_controls.MapUnitInspector)MainForm.mapedittool.selected_inspector).RemoveUnit(index);
            }
            else
                map.DeleteUnit(index);
        }

        public void Apply(SFMap map)
        {
            if (is_adding)
                Add(map);
            else
                Remove(map);

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            if (is_adding)
                Remove(map);
            else
                Add(map);

            UpdateMap(map);
        }

        public override string ToString()
        {
            return (is_adding ? "add_unit" : "remove_unit") + " (index: " + index.ToString() + ", id: " + unit.game_id.ToString() + ", pos: " + unit.grid_position.ToString() + ")";
        }
    }


    public class MapOperatorBuildingAddOrRemove : IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public SFMapBuilding building;
        public int index;
        public bool is_adding;    // true - entityadd, false: entityremove

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            MainForm.mapedittool.ui.RedrawMinimapIcons();
            MainForm.mapedittool.update_render = true;
        }

        private void Add(SFMap map)
        {
            map.AddBuilding(building.game_id, building.grid_position, building.angle, building.npc_id, building.level, building.race_id, index);

            if (MainForm.mapedittool.selected_inspector is map_controls.MapBuildingInspector)
            {
                ((map_controls.MapBuildingInspector)MainForm.mapedittool.selected_inspector).LoadNextBuilding(index);
            }
        }

        private void Remove(SFMap map)
        {
            if (MainForm.mapedittool.selected_inspector is map_controls.MapBuildingInspector)
            {
                if (index == ((MapEdit.MapBuildingEditor)MainForm.mapedittool.selected_editor).selected_building)
                {
                    MainForm.mapedittool.selected_editor.Select(Utility.NO_INDEX);
                    MainForm.mapedittool.InspectorSelect(null);
                }

                map.DeleteBuilding(index);

                ((map_controls.MapBuildingInspector)MainForm.mapedittool.selected_inspector).RemoveBuilding(index);
            }
            else
                map.DeleteBuilding(index);
        }

        public void Apply(SFMap map)
        {
            if (is_adding)
                Add(map);
            else
                Remove(map);

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            if (is_adding)
                Remove(map);
            else
                Add(map);

            UpdateMap(map);
        }

        public override string ToString()
        {
            return (is_adding ? "add_building" : "remove_unit") + " (index: " + index.ToString() + ", id: " + building.game_id.ToString() + ", pos: " + building.grid_position.ToString() + ")";
        }
    }

    public class MapOperatorObjectAddOrRemove : IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public SFMapObject obj;
        public int index;
        public bool is_adding;    // true - entityadd, false: entityremove

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            MainForm.mapedittool.ui.RedrawMinimapIcons();
            MainForm.mapedittool.update_render = true;
        }

        private void Add(SFMap map)
        {
            map.AddObject(obj.game_id, obj.grid_position, obj.angle, obj.npc_id, obj.unknown1, index);

            if (MainForm.mapedittool.selected_inspector is map_controls.MapObjectInspector)
            {
                ((map_controls.MapObjectInspector)MainForm.mapedittool.selected_inspector).LoadNextObject(index);
            }
        }

        private void Remove(SFMap map)
        {
            if (MainForm.mapedittool.selected_inspector is map_controls.MapObjectInspector)
            {
                if (index == ((MapEdit.MapObjectEditor)MainForm.mapedittool.selected_editor).selected_object)
                {
                    MainForm.mapedittool.selected_editor.Select(Utility.NO_INDEX);
                    MainForm.mapedittool.InspectorSelect(null);
                }

                map.DeleteObject(index);

                ((map_controls.MapObjectInspector)MainForm.mapedittool.selected_inspector).RemoveObject(index);
            }
            else
                map.DeleteObject(index);
        }

        public void Apply(SFMap map)
        {
            if (is_adding)
                Add(map);
            else
                Remove(map);

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            if (is_adding)
                Remove(map);
            else
                Add(map);

            UpdateMap(map);
        }

        public override string ToString()
        {
            return (is_adding ? "add_object" : "remove_object") + " (index: " + index.ToString() + ", id: " + obj.game_id.ToString() + ", pos: " + obj.grid_position.ToString() + ")";
        }
    }

    public class MapOperatorCoopCampAddOrRemove : IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public SFCoord position;
        public int obj_index;
        public int coopcamp_id;
        public int coopcamp_unknown;
        public int coopcamp_index;
        public bool is_adding;    // true - entityadd, false: entityremove

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            MainForm.mapedittool.ui.RedrawMinimapIcons();
            MainForm.mapedittool.update_render = true;
        }

        private void Add(SFMap map)
        {
            map.AddObject(2541, position, 0, 0, 0, obj_index);
            map.metadata.coop_spawns.Insert(coopcamp_index,
                new SFMapCoopAISpawn(map.object_manager.objects[obj_index], coopcamp_id, coopcamp_unknown));

            // add mesh to the object
            SF3D.SceneSynchro.SceneNode obj_node =
                map.heightmap.GetChunkNode(position)
                .FindNode<SF3D.SceneSynchro.SceneNode>(
                    map.object_manager.objects[obj_index].GetName());

            string m = "editor_dummy_spawnpoint";
            SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(obj_node, m, obj_node.Name + "_SPAWNCIRCLE");

            if (MainForm.mapedittool.selected_inspector is map_controls.MapCoopCampInspector)
            {
                ((map_controls.MapCoopCampInspector)MainForm.mapedittool.selected_inspector).LoadNextCoopCamp(coopcamp_index);
            }
        }

        private void Remove(SFMap map)
        {
            if (MainForm.mapedittool.selected_inspector is map_controls.MapCoopCampInspector)
            {
                if (coopcamp_index == ((MapEdit.MapCoopCampEditor)MainForm.mapedittool.selected_editor).selected_spawn)
                {
                    MainForm.mapedittool.selected_editor.Select(Utility.NO_INDEX);
                    MainForm.mapedittool.InspectorSelect(null);
                }

                map.DeleteObject(obj_index);

                ((map_controls.MapCoopCampInspector)MainForm.mapedittool.selected_inspector).RemoveCoopCamp(coopcamp_index);
                map.metadata.coop_spawns.Remove(map.metadata.coop_spawns[coopcamp_index]);
            }
            else
            {
                map.DeleteObject(obj_index);

                map.metadata.coop_spawns.Remove(map.metadata.coop_spawns[coopcamp_index]);
            }
        }

        public void Apply(SFMap map)
        {
            if (is_adding)
                Add(map);
            else
                Remove(map);

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            if (is_adding)
                Remove(map);
            else
                Add(map);

            UpdateMap(map);
        }

        public override string ToString()
        {
            return (is_adding ? "add_coopcamp" : "remove_coopcamp") + " (index: " + coopcamp_index.ToString() + ", id: " + coopcamp_id.ToString() + ", pos: " + position.ToString() + ")";
        }
    }

    public class MapOperatorBindstoneAddOrRemove : IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public int intobj_index;
        public SFCoord bindstone_pos;
        public ushort bindstone_textid;
        public short bindstone_unknown;
        public int bindstone_index;
        public int player_index;
        public bool is_adding;    // true - entityadd, false: entityremove

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            MainForm.mapedittool.ui.RedrawMinimapIcons();
            MainForm.mapedittool.update_render = true;
        }

        private void Add(SFMap map)
        {
            map.AddInteractiveObject(769, bindstone_pos, 0, 1, intobj_index);
            map.metadata.spawns.Insert(player_index, new SFMapSpawn() { bindstone_index = bindstone_index, pos = bindstone_pos, text_id = bindstone_textid, unknown = bindstone_unknown });


            if (MainForm.mapedittool.selected_inspector is map_controls.MapBindstoneInspector)
            {
                ((map_controls.MapBindstoneInspector)MainForm.mapedittool.selected_inspector).LoadNextBindstone(bindstone_index);
            }
        }

        private void Remove(SFMap map)
        {
            if (MainForm.mapedittool.selected_inspector is map_controls.MapBindstoneInspector)
            {
                if (bindstone_index == ((MapEdit.MapBindstoneEditor)MainForm.mapedittool.selected_editor).selected_bindstone)
                {
                    MainForm.mapedittool.selected_editor.Select(Utility.NO_INDEX);
                    MainForm.mapedittool.InspectorSelect(null);
                }

                map.DeleteInteractiveObject(intobj_index);

                ((map_controls.MapBindstoneInspector)MainForm.mapedittool.selected_inspector).RemoveBindstone(bindstone_index);
                map.metadata.spawns.RemoveAt(player_index);
            }
            else
            {
                map.DeleteInteractiveObject(intobj_index);

                map.metadata.spawns.RemoveAt(player_index);
            }
        }

        public void Apply(SFMap map)
        {
            if (is_adding)
                Add(map);
            else
                Remove(map);

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            if (is_adding)
                Remove(map);
            else
                Add(map);

            UpdateMap(map);
        }

        public override string ToString()
        {
            return (is_adding ? "add_bindstone" : "remove_bindstone") + " (index: " + bindstone_index.ToString() + ", id: " + bindstone_textid.ToString() + ", pos: " + bindstone_pos.ToString() + ")";
        }
    }

    public class MapOperatorPortalAddOrRemove : IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public SFMapPortal portal;
        public int index;
        public bool is_adding;    // true - entityadd, false: entityremove

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            MainForm.mapedittool.ui.RedrawMinimapIcons();
            MainForm.mapedittool.update_render = true;
        }

        private void Add(SFMap map)
        {
            map.AddPortal(portal.game_id, portal.grid_position, portal.angle, index);

            if (MainForm.mapedittool.selected_inspector is map_controls.MapPortalInspector)
            {
                ((map_controls.MapPortalInspector)MainForm.mapedittool.selected_inspector).LoadNextPortal(index);
            }
        }

        private void Remove(SFMap map)
        {
            if (MainForm.mapedittool.selected_inspector is map_controls.MapPortalInspector)
            {
                if (index == ((MapEdit.MapPortalEditor)MainForm.mapedittool.selected_editor).selected_portal)
                {
                    MainForm.mapedittool.selected_editor.Select(Utility.NO_INDEX);
                    MainForm.mapedittool.InspectorSelect(null);
                }

                map.DeletePortal(index);

                ((map_controls.MapPortalInspector)MainForm.mapedittool.selected_inspector).RemovePortal(index);
            }
            else
                map.DeletePortal(index);
        }

        public void Apply(SFMap map)
        {
            if (is_adding)
                Add(map);
            else
                Remove(map);

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            if (is_adding)
                Remove(map);
            else
                Add(map);

            UpdateMap(map);
        }

        public override string ToString()
        {
            return (is_adding ? "add_portal" : "remove_portal") + " (index: " + index.ToString() + ", id: " + portal.game_id.ToString() + ", pos: " + portal.grid_position.ToString() + ")";
        }
    }

    public class MapOperatorMonumentAddOrRemove : IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public int intobj_index;
        public SFMapInteractiveObject intobj;
        public int monument_index;
        public bool is_adding;    // true - entityadd, false: entityremove

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        private void UpdateMap(SFMap map)
        {
            MainForm.mapedittool.ui.RedrawMinimapIcons();
            MainForm.mapedittool.update_render = true;
        }

        private void Add(SFMap map)
        {
            map.AddInteractiveObject(intobj.game_id, intobj.grid_position, intobj.angle, intobj.unk_byte, intobj_index);
            if (MainForm.mapedittool.selected_inspector is map_controls.MapMonumentInspector)
            {
                ((map_controls.MapMonumentInspector)MainForm.mapedittool.selected_inspector).LoadNextMonument(monument_index);
            }
        }

        private void Remove(SFMap map)
        {
            if (MainForm.mapedittool.selected_inspector is map_controls.MapMonumentInspector)
            {
                if (monument_index == ((MapEdit.MapMonumentEditor)MainForm.mapedittool.selected_editor).selected_monument)
                {
                    MainForm.mapedittool.selected_editor.Select(Utility.NO_INDEX);
                    MainForm.mapedittool.InspectorSelect(null);
                }

                map.DeleteInteractiveObject(intobj_index);

                ((map_controls.MapMonumentInspector)MainForm.mapedittool.selected_inspector).RemoveMonument(monument_index);
            }
            else
                map.DeleteInteractiveObject(intobj_index);
        }

        public void Apply(SFMap map)
        {
            if (is_adding)
                Add(map);
            else
                Remove(map);

            UpdateMap(map);
        }

        public void Revert(SFMap map)
        {
            if (is_adding)
                Remove(map);
            else
                Add(map);

            UpdateMap(map);
        }

        public override string ToString()
        {
            return (is_adding ? "add_monument" : "remove_monument") + " (index: " + monument_index.ToString() + ", id: " + (intobj.game_id-771).ToString() + ", pos: " + intobj.grid_position.ToString() + ")";
        }
    }

    public enum MapOperatorEntityType { UNIT = 0, BUILDING = 1, OBJECT = 2, COOPCAMP = 3, BINDSTONE = 4, PORTAL = 5, MONUMENT = 6 }

    public enum MapOperatorEntityProperty
    { 
        ID, POSITION, NPCID, ANGLE,
        UNITFLAGS, UNITUNKNOWN, UNITGROUP, UNITUNKNOWN2,
        BUILDINGLEVEL, BUILDINGRACE, OBJECTUNKNOWN, COOPCAMPUNKNOWN, BINDSTONEUNKNOWN
    }

    public class MapOperatorEntityChangeProperty: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public MapOperatorEntityType type;
        public int index;
        public MapOperatorEntityProperty property;
        public object PreChangeProperty;
        public object PostChangeProperty;

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        public void ChangeProperty(SFMap map, bool change_forward)
        {
            object prop;
            if (change_forward)
                prop = PostChangeProperty;
            else
                prop = PreChangeProperty;

            switch (type)
            {
                case MapOperatorEntityType.UNIT:
                    {
                        SFMapUnit unit = map.unit_manager.units[index];
                        switch (property)
                        {
                            case MapOperatorEntityProperty.ID:
                                map.ReplaceUnit(index, (ushort)(int)prop);
                                break;
                            case MapOperatorEntityProperty.POSITION:
                                map.MoveUnit(index, (SFCoord)prop);
                                break;
                            case MapOperatorEntityProperty.NPCID:
                                unit.npc_id = (int)prop;
                                break;
                            case MapOperatorEntityProperty.UNITFLAGS:
                                unit.unknown_flags = (int)prop;
                                break;
                            case MapOperatorEntityProperty.UNITUNKNOWN:
                                unit.unknown = (int)prop;
                                break;
                            case MapOperatorEntityProperty.UNITGROUP:
                                unit.group = (int)prop;
                                break;
                            case MapOperatorEntityProperty.UNITUNKNOWN2:
                                unit.unknown2 = (int)prop;
                                break;
                            default:
                                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "MapOperatorEntityChangeProperty.ChangeProperty(): Invalid property " + property.ToString() + " for entity of type " + type.ToString());
                                break;
                        }
                        break;
                    }
                case MapOperatorEntityType.BUILDING:
                    {
                        SFMapBuilding building = map.building_manager.buildings[index];
                        switch (property)
                        {
                            case MapOperatorEntityProperty.ID:
                                map.ReplaceBuilding(index, (ushort)(int)prop);
                                break;
                            case MapOperatorEntityProperty.POSITION:
                                map.MoveBuilding(index, (SFCoord)prop);
                                break;
                            case MapOperatorEntityProperty.ANGLE:
                                building.angle = (int)prop;
                                map.RotateBuilding(index, (int)prop);
                                break;
                            case MapOperatorEntityProperty.NPCID:
                                building.npc_id = (int)prop;
                                break;
                            case MapOperatorEntityProperty.BUILDINGLEVEL:
                                building.level = (int)prop;
                                break;
                            case MapOperatorEntityProperty.BUILDINGRACE:
                                building.race_id = (int)prop;
                                break;
                            default:
                                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "MapOperatorEntityChangeProperty.ChangeProperty(): Invalid property " + property.ToString() + " for entity of type " + type.ToString());
                                break;
                        }
                        break;
                    }
                case MapOperatorEntityType.OBJECT:
                    {
                        SFMapObject obj = map.object_manager.objects[index];
                        switch (property)
                        {
                            case MapOperatorEntityProperty.ID:
                                map.ReplaceObject(index, (ushort)(int)prop);
                                break;
                            case MapOperatorEntityProperty.POSITION:
                                map.MoveObject(index, (SFCoord)prop);
                                break;
                            case MapOperatorEntityProperty.ANGLE:
                                obj.angle = (int)prop;
                                map.RotateObject(index, (int)prop);
                                break;
                            case MapOperatorEntityProperty.NPCID:
                                obj.npc_id = (int)prop;
                                break;
                            case MapOperatorEntityProperty.OBJECTUNKNOWN:
                                obj.unknown1 = (int)prop;
                                break;
                            default:
                                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "MapOperatorEntityChangeProperty.ChangeProperty(): Invalid property " + property.ToString() + " for entity of type " + type.ToString());
                                break;
                        }
                        break;
                    }
                case MapOperatorEntityType.COOPCAMP:
                    {
                        SFMapObject obj = map.metadata.coop_spawns[index].spawn_obj;
                        int obj_index = map.object_manager.objects.IndexOf(obj);
                        switch (property)
                        {
                            case MapOperatorEntityProperty.ID:
                                map.metadata.coop_spawns[index].spawn_id = (int)prop;
                                break;
                            case MapOperatorEntityProperty.POSITION:
                                map.MoveObject(obj_index, (SFCoord)prop);
                                break;
                            case MapOperatorEntityProperty.COOPCAMPUNKNOWN:
                                map.metadata.coop_spawns[index].spawn_certain = (int)prop;
                                break;
                            default:
                                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "MapOperatorEntityChangeProperty.ChangeProperty(): Invalid property " + property.ToString() + " for entity of type " + type.ToString());
                                break;
                        }
                        break;
                    }
                case MapOperatorEntityType.BINDSTONE:
                    {
                        SFMapInteractiveObject int_obj = map.int_object_manager.int_objects[map.int_object_manager.bindstones_index[index]];
                        int int_obj_index = map.int_object_manager.bindstones_index[index];
                        int player = map.metadata.FindPlayerByBindstoneIndex(index);

                        switch (property)
                        {
                            case MapOperatorEntityProperty.ID:   // text id
                                map.metadata.spawns[player].text_id = (ushort)(int)prop;
                                break;
                            case MapOperatorEntityProperty.POSITION:
                                if(int_obj_index != Utility.NO_INDEX)
                                    map.MoveInteractiveObject(int_obj_index, (SFCoord)prop);
                                map.metadata.spawns[player].pos = (SFCoord)prop;
                                break;
                            case MapOperatorEntityProperty.ANGLE:
                                if (int_obj_index != Utility.NO_INDEX)
                                {
                                    int_obj.angle = (int)prop;
                                    map.RotateInteractiveObject(int_obj_index, (int)prop);
                                }
                                break;
                            case MapOperatorEntityProperty.BINDSTONEUNKNOWN:
                                map.metadata.spawns[player].unknown = (short)(int)prop;
                                break;
                            default:
                                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "MapOperatorEntityChangeProperty.ChangeProperty(): Invalid property " + property.ToString() + " for entity of type " + type.ToString());
                                break;
                        }
                        break;
                    }
                case MapOperatorEntityType.PORTAL:
                    {
                        SFMapPortal portal = map.portal_manager.portals[index];
                        switch (property)
                        {
                            case MapOperatorEntityProperty.ID:
                                portal.game_id = (ushort)prop;
                                break;
                            case MapOperatorEntityProperty.POSITION:
                                map.MovePortal(index, (SFCoord)prop);
                                break;
                            case MapOperatorEntityProperty.ANGLE:
                                portal.angle = (int)prop;
                                map.RotatePortal(index, (int)prop);
                                break;
                            default:
                                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "MapOperatorEntityChangeProperty.ChangeProperty(): Invalid property " + property.ToString() + " for entity of type " + type.ToString());
                                break;
                        }
                        break;
                    }
                case MapOperatorEntityType.MONUMENT:
                    {
                        SFMapInteractiveObject int_obj = map.int_object_manager.int_objects[map.int_object_manager.monuments_index[index]];
                        int int_obj_index = map.int_object_manager.monuments_index[index];

                        switch (property)
                        {
                            case MapOperatorEntityProperty.POSITION:
                                map.MoveInteractiveObject(int_obj_index, (SFCoord)prop);
                                break;
                            case MapOperatorEntityProperty.ANGLE:
                                int_obj.angle = (int)prop;
                                map.RotateInteractiveObject(int_obj_index, (int)prop);
                                break;
                            default:
                                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "MapOperatorEntityChangeProperty.ChangeProperty(): Invalid property " + property.ToString() + " for entity of type " + type.ToString());
                                break;
                        }
                        break;
                    }
                default:
                    LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "MapOperatorEntityChangeProperty.ChangeProperty(): Unsupported entity type " + type.ToString());
                    break;
            }
        }

        public void Apply(SFMap map)
        {
            ChangeProperty(map, true);

            MainForm.mapedittool.update_render = true;
        }

        public void Revert(SFMap map)
        {
            ChangeProperty(map, false);

            MainForm.mapedittool.update_render = true;
        }

        public override string ToString()
        {
            return "change_entity_property (type = " + type.ToString() + ", property = " + property.ToString() + ", index = " + index.ToString()
                + ", value: " + PreChangeProperty.ToString() + " -> " + PostChangeProperty.ToString() + ")";
        }
    }

    public class MapOperatorDecorationPaint: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public Dictionary<SFCoord, byte> PreOperatorDecals = new Dictionary<SFCoord, byte>();
        public Dictionary<SFCoord, byte> PostOperatorDecals = new Dictionary<SFCoord, byte>();

        public void Finish(SFMap map)
        {
            foreach (var kv in PreOperatorDecals)
                PostOperatorDecals.Add(kv.Key, map.decoration_manager.GetDecAssignment(kv.Key));

            Finished = true;
        }

        private void UpdateDecorations(SFMap map, Dictionary<SFCoord, byte> decals)
        {
            map.decoration_manager.SetDecorations(decals);

            map.heightmap.RefreshOverlay();
            MainForm.mapedittool.update_render = true;
        }

        public void Apply(SFMap map)
        {
            UpdateDecorations(map, PostOperatorDecals);
        }

        public void Revert(SFMap map)
        {
            UpdateDecorations(map, PreOperatorDecals);
        }

        public override string ToString()
        {
            return "set_decal (count: " + PostOperatorDecals.Count + ")";
        }
    }

    public class MapOperatorDecorationModifyGroup: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public int group = -1;
        public int index = -1;
        public ushort PreOperatorID;
        public byte PreOperatorWeight;
        public ushort PostOperatorID;
        public byte PostOperatorWeight;

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        public void Apply(SFMap map)
        {
            map.decoration_manager.dec_groups[group].SetDecoration(index, PostOperatorID, PostOperatorWeight);

            map.decoration_manager.UpdateDecorationsOfGroup((byte)index);
            MainForm.mapedittool.UpdateDecGroup((byte)index);
            MainForm.mapedittool.update_render = true;
        }

        public void Revert(SFMap map)
        {
            map.decoration_manager.dec_groups[group].SetDecoration(index, PreOperatorID, PreOperatorWeight);

            map.decoration_manager.UpdateDecorationsOfGroup((byte)index);
            MainForm.mapedittool.UpdateDecGroup((byte)index);
            MainForm.mapedittool.update_render = true;
        }

        public override string ToString()
        {
            return "modify_decal_group (group: " + group.ToString() + ", index: " + index.ToString() + ", id: " + PreOperatorID.ToString() + " -> " + PostOperatorID.ToString()
                + ", weight: " + PreOperatorWeight.ToString() + " -> " + PostOperatorWeight.ToString() + ")";
        }
    }

    public class MapOperatorChangeMapType: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public SFMapType PreOperatorMapType;
        public SFMapType PostOperatorMapType;

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        public void Apply(SFMap map)
        {
            MainForm.mapedittool.SetMapType(PostOperatorMapType);
        }

        public void Revert(SFMap map)
        {
            MainForm.mapedittool.SetMapType(PreOperatorMapType);
        }

        public override string ToString()
        {
            return "set_map_type (type: " + PreOperatorMapType.ToString() + " -> " + PostOperatorMapType.ToString() + ")";
        }
    }

    public class MapOperatorMultiplayerAddOrRemoveTeamComp: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public SFMapMultiplayerTeamComposition team_comp = null;
        public bool is_adding = true;

        public void Finish(SFMap map)
        {
            Finished = true;
        }
        public void Add(SFMap map)
        {
            map.metadata.InsertTeamComp(team_comp);

            MainForm.mapedittool.external_operator_AddOrRemoveTeamComp();
        }

        public void Remove(SFMap map)
        {
            if (map.metadata.multi_teams.Contains(team_comp))
            {
                map.metadata.multi_teams.Remove(team_comp);
                MainForm.mapedittool.external_operator_AddOrRemoveTeamComp();
            }
            else
                throw new Exception("MapOperatorMultiplayerAddOrRemoveTeamComp.Remove(): Could not find team comp to remove!");
        }

        public void Apply(SFMap map)
        {
            if (is_adding)
                Add(map);
            else
                Remove(map);
        }

        public void Revert(SFMap map)
        {
            if (is_adding)
                Remove(map);
            else
                Add(map);
        }

        public override string ToString()
        {
            return (is_adding ? "add_teamcomp" : "remove_teamcomp") + " (teamcomp size: " + team_comp.team_count.ToString() + ")";
        }
    }

    public class MapOperatorMultiplayerMovePlayer: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public int player_index;
        public int comp_index;
        public SFMapTeamPlayer player;
        public int PreOperatorPlayerTeam;
        public int PostOperatorPlayerTeam;

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        public void AddPlayer(SFMap map, SFMapTeamPlayer teamplayer, int teamcomp_index, int team_index, int teamplayer_index)
        {
            map.metadata.multi_teams[teamcomp_index].players[team_index].Insert(teamplayer_index, teamplayer);
            MainForm.mapedittool.external_operator_AddTeamMember(teamplayer);
        }

        public void RemovePlayer(SFMap map, int teamcomp_index, int team_index, int teamplayer_index)
        {
            map.metadata.multi_teams[teamcomp_index].players[team_index].RemoveAt(teamplayer_index);
            MainForm.mapedittool.external_operator_RemoveTeamMember(teamplayer_index);
        }

        public void Apply(SFMap map)
        {
            if (PostOperatorPlayerTeam != -1)
                AddPlayer(map, player, comp_index, PostOperatorPlayerTeam, player_index);
            else
                RemovePlayer(map, comp_index, PreOperatorPlayerTeam, player_index);
        }

        public void Revert(SFMap map)
        {
            if (PostOperatorPlayerTeam != -1)
                RemovePlayer(map, comp_index, PostOperatorPlayerTeam, player_index);
            else
                AddPlayer(map, player, comp_index, PreOperatorPlayerTeam, player_index);
        }

        public override string ToString()
        {
            return "team_move_player (comp_index: " + comp_index.ToString() + ", team: "
                + PreOperatorPlayerTeam.ToString() + " -> " + PostOperatorPlayerTeam.ToString() + ", player index: " + player_index.ToString() + ")";
        }
    }

    public enum MapOperatorPlayerStateType { TEXT_ID = 0, NAME, LEVEL_RANGE }
    public class MapOperatorMultiplayerSetPlayerState: IMapOperator
    {
        public bool Finished { get; set; } = false;
        public bool ApplyOnPush { get; set; } = false;
        public int player_index;
        public int team_index;
        public int comp_index;
        public MapOperatorPlayerStateType state_type;
        public object PreOperatorState;
        public object PostOperatorState;

        public void Finish(SFMap map)
        {
            Finished = true;
        }

        public void SetState(SFMap map, MapOperatorPlayerStateType st, object val)
        {
            switch(st)
            {
                case MapOperatorPlayerStateType.TEXT_ID:
                    map.metadata.multi_teams[comp_index]
                    .players[team_index][player_index]
                    .text_id = (UInt16)val;
                    break;
                case MapOperatorPlayerStateType.NAME:
                    map.metadata.multi_teams[comp_index]
                    .players[team_index][player_index]
                    .coop_map_type = (string)val;
                    break;
                case MapOperatorPlayerStateType.LEVEL_RANGE:
                    map.metadata.multi_teams[comp_index]
                    .players[team_index][player_index]
                    .coop_map_lvl = (string)val;
                    break;
            }
            MainForm.mapedittool.external_operator_SetPlayerState(comp_index, team_index, player_index);
        }

        public void Apply(SFMap map)
        {
            SetState(map, state_type, PostOperatorState);
        }

        public void Revert(SFMap map)
        {
            SetState(map, state_type, PreOperatorState);
        }

        public override string ToString()
        {
            return "player_set_state (comp_index: " + comp_index.ToString()
                + ", team: " + team_index.ToString() + ", player index: " + player_index.ToString() + ", state_type: " + state_type.ToString()
                + ", state: " + PreOperatorState.ToString() + " -> " + PostOperatorState.ToString() + ")";
        }
    }
}
