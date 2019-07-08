using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using SpellforceDataEditor.SFChunkFile;
using SpellforceDataEditor.SF3D.SFRender;

namespace SpellforceDataEditor.SFMap
{
    public class SFMap
    {
        public static SFCoord[] NEIGHBORS = {new SFCoord(1, 0), new SFCoord(1, -1), new SFCoord(0, -1), new SFCoord(-1, -1),
                                             new SFCoord(-1, 0), new SFCoord(-1, 1), new SFCoord(0, 1), new SFCoord(1, 1)};

        public int width { get; private set; } = 0;
        public int height { get; private set; } = 0;
        public SFMapHeightMap heightmap { get; private set; } = null;
        public SFMapBuildingManager building_manager { get; private set; } = null;
        public SFMapUnitManager unit_manager { get; private set; } = null;
        public SFMapObjectManager object_manager { get; private set; } = null;
        public SFMapInteractiveObjectManager int_object_manager { get; private set; } = null;
        public SFMapDecorationManager decoration_manager { get; private set; } = null;
        public SFMapPortalManager portal_manager { get; private set; } = null;
        public SFMapLakeManager lake_manager { get; private set; } = null;
        public SFMapMetaData metadata { get; private set; } = null;
        public SFMapSelectionHelper selection_helper { get; private set; } = new SFMapSelectionHelper();
        public SFMapNPCManager npc_manager { get; private set; } = null;
        public SFCFF.SFGameData gamedata { get; private set; } = null;

        public int Load(string filename, SFCFF.SFGameData gd, ToolStripLabel tx)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load() called, filename: " + filename);
            tx.Text = "Loading...";
            tx.GetCurrentParent().Refresh();
            SFChunkFile.SFChunkFile f = new SFChunkFile.SFChunkFile();
            int res = f.Open(filename);
            if (res != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not open map file!");
                return res;
            }

            // load map size and tile indices
            gamedata = gd;

            short size;

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading tile data");
            tx.Text = "Loading map data...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c2 = f.GetChunkByID(2);
            if (c2 == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not read map size or tile data!");
                f.Close();
                return -4;
            }
            using (BinaryReader br = c2.Open())
            {
                size = br.ReadInt16();
                br.ReadByte();
                width = size;
                height = size;
                heightmap = new SFMapHeightMap(width, height) { map = this };
                heightmap.SetTilesRaw(br.ReadBytes(size * size));
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Map size is " + size.ToString());
            }
            c2.Close();

            // load terrain texture data
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading tile definitions");
            SFChunkFileChunk c3 = f.GetChunkByID(3);
            if (c3 != null)
            {
                using (BinaryReader br = c3.Open())
                {
                    byte[] data = br.ReadBytes((int)br.BaseStream.Length);
                    MemoryStream ch3_ms = new MemoryStream(data);
                    using (BinaryReader ch3_br = new BinaryReader(ch3_ms))
                    {
                        for (int i = 0; i < 255; i++)
                        {
                            heightmap.texture_manager.texture_tiledata[i].ind1 = ch3_br.ReadByte();
                            heightmap.texture_manager.texture_tiledata[i].ind2 = ch3_br.ReadByte();
                            heightmap.texture_manager.texture_tiledata[i].ind3 = ch3_br.ReadByte();
                            heightmap.texture_manager.texture_tiledata[i].weight1 = ch3_br.ReadByte();
                            heightmap.texture_manager.texture_tiledata[i].weight2 = ch3_br.ReadByte();
                            heightmap.texture_manager.texture_tiledata[i].weight3 = ch3_br.ReadByte();
                            heightmap.texture_manager.texture_tiledata[i].reindex_data = ch3_br.ReadByte();
                            heightmap.texture_manager.texture_tiledata[i].reindex_index = ch3_br.ReadByte();
                            ch3_br.ReadByte(); ch3_br.ReadByte();
                            heightmap.texture_manager.texture_tiledata[i].material_property = ch3_br.ReadByte();
                            ch3_br.ReadByte();
                            byte b_m = ch3_br.ReadByte(); byte b_v = ch3_br.ReadByte();
                            heightmap.texture_manager.texture_tiledata[i].blocks_movement = ((b_m % 2) == 1 ? true : false);
                            heightmap.texture_manager.texture_tiledata[i].blocks_vision = ((b_v % 2) == 1 ? true : false);
                        }
                    }
                }
                c3.Close();
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading textures");
            SFChunkFileChunk c4 = f.GetChunkByID(4);
            if (c4 != null)
            {
                using (BinaryReader br = c4.Open())
                {
                    heightmap.texture_manager.SetTextureIDsRaw(br.ReadBytes((int)br.BaseStream.Length));
                }
                c4.Close();
            }

            // generate texture data
            heightmap.texture_manager.Init();

            // load heightmap
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading heightmap");
            for (short i = 0; i < size; i++)
            {
                SFChunkFileChunk c6_i = f.GetChunkByID(6, i);
                if (c6_i == null)
                {
                    f.Close();
                    LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not read heightmap!");
                    return -4;
                }
                using (BinaryReader br = c6_i.Open())
                {
                    heightmap.SetRowRaw(i, br.ReadBytes(size * 2));
                }
                c6_i.Close();
            }

            // generate heightmap models and reindex textures
            heightmap.Generate();

            npc_manager = new SFMapNPCManager() { map = this };

            // load buildings
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading buildings");
            tx.Text = "Loading buildings...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c11 = f.GetChunkByID(11);
            if (c11 != null)
            {
                using (BinaryReader br = c11.Open())
                {
                    building_manager = new SFMapBuildingManager() { map = this };
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int angle = br.ReadInt16();
                        int npc_id = br.ReadInt16();   // presumed
                        int b_type = br.ReadByte();
                        int b_lvl = 1;
                        int race_id = 0;
                        if (c11.get_data_type() > 1)
                            b_lvl = br.ReadByte();
                        if (c11.get_data_type() > 2)
                            race_id = br.ReadByte();
                        AddBuilding(b_type, pos, angle, npc_id, b_lvl, race_id);
                    }
                }
                c11.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Buildings loaded: " + building_manager.buildings.Count.ToString());
            }

            // load units
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading units");
            tx.Text = "Loading units...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c12 = f.GetChunkByID(12);
            if (c12 != null)
            {
                using (BinaryReader br = c12.Open())
                {
                    unit_manager = new SFMapUnitManager() { map = this };
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int angle = br.ReadInt16();
                        int unit_id = br.ReadInt16();
                        int npc_id = br.ReadUInt16();
                        int unknown = br.ReadUInt16();
                        int group = br.ReadByte();
                        int unknown2 = 0;
                        if (c12.get_data_type() >= 5)
                            unknown2 = br.ReadByte();
                        AddUnit(unit_id, pos, angle, npc_id, unknown, group, unknown2);
                    }
                }
                c12.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Units loaded: " + unit_manager.units.Count.ToString());
            }

            // load objects
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading objects");
            tx.Text = "Loading objects...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c29 = f.GetChunkByID(29);
            if (c29 != null)
            {
                using (BinaryReader br = c29.Open())
                {
                    object_manager = new SFMapObjectManager() { map = this };
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int object_id = br.ReadInt16();
                        int angle = br.ReadInt16();
                        int npc_id = br.ReadUInt16();
                        int unk1 = 0;
                        if (c29.get_data_type() == 6)
                        {
                            unk1 = br.ReadUInt16();
                            br.ReadBytes(4);
                        }
                        else if(c29.get_data_type() == 5)
                        {
                            unk1 = br.ReadUInt16();
                            br.ReadBytes(2);
                        }
                        else if (c29.get_data_type() == 4)
                            unk1 = br.ReadUInt16();
                        if ((object_id >= 65) && (object_id <= 67))   // editor only
                            continue;
                        AddObject(object_id, pos, angle, npc_id, unk1);
                    }
                }
                c29.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Objects loaded: " + object_manager.objects.Count.ToString());
            }

            // load interactive objects
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading interactive objects");
            tx.Text = "Loading interactive objects...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c30 = f.GetChunkByID(30);
            if (c30 != null)
            {
                using (BinaryReader br = c30.Open())
                {
                    int_object_manager = new SFMapInteractiveObjectManager() { map = this };
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int object_id = br.ReadInt16();
                        int angle = br.ReadInt16();
                        int unk_byte = br.ReadByte();
                        AddInteractiveObject(object_id, pos, angle, unk_byte);
                    }
                }
                c30.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Interactive objects loaded: " + int_object_manager.int_objects.Count.ToString());
            }

            // load decorations
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading decal data");
            tx.Text = "Loading decorations...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c31 = f.GetChunkByID(31);
            if (c31 != null)
            {
                using (BinaryReader br = c31.Open())
                {
                    decoration_manager = new SFMapDecorationManager() { map = this };
                    decoration_manager.dec_assignment = br.ReadBytes(1048576);
                }
                c31.Close();
            }
            SFChunkFileChunk c32 = f.GetChunkByID(32);
            if (c32 != null)
            {
                using (BinaryReader br = c32.Open())
                {
                    for (int i = 0; i < 255; i++)
                    {
                        decoration_manager.dec_groups[i] = new SFMapDecorationGroup();
                        for (int j = 0; j < 30; j++)
                            decoration_manager.dec_groups[i].dec_id[j] = br.ReadUInt16();
                        for (int j = 0; j < 30; j++)
                        {
                            decoration_manager.dec_groups[i].weight[j] = br.ReadByte();
                            if (decoration_manager.dec_groups[i].weight[j] != 0)
                                decoration_manager.dec_groups[i].dec_used += 1;
                        }
                    }
                }
                c32.Close();
            }
            decoration_manager.GenerateDecorations();

            // load portals
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading portals");
            tx.Text = "Loading portals...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c35 = f.GetChunkByID(35);
            if (c35 != null)
            {
                using (BinaryReader br = c35.Open())
                {
                    portal_manager = new SFMapPortalManager() { map = this };
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int angle = br.ReadInt16();
                        int portal_id = br.ReadInt16();
                        AddPortal(portal_id, pos, angle);
                    }
                }
                c35.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Portals loaded: " + portal_manager.portals.Count.ToString());
            }

            // load lakes
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading lakes");
            SFChunkFileChunk c40 = f.GetChunkByID(40);
            if (c40 != null)
            {
                using (BinaryReader br = c40.Open())
                {
                    lake_manager = new SFMapLakeManager() { map = this };
                    int lake_count = br.ReadByte();
                    for (int i = 0; i < lake_count; i++)
                    {
                        short x = br.ReadInt16();
                        short y = br.ReadInt16();
                        short z_diff = br.ReadInt16();
                        byte type = br.ReadByte();
                        lake_manager.AddLake(new SFCoord(x, y), z_diff, type);
                    }
                }
                c40.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Lakes found: " + lake_manager.lakes.Count.ToString());
            }

            // load map flags
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading map flags");
            tx.Text = "Loading map flags...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c42 = f.GetChunkByID(42);
            if (c42 != null)
            {

                SFCoord pos = new SFCoord();
                using (BinaryReader br = c42.Open())
                {
                    int pos_count = c42.get_original_data_length() / 4;
                    LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Movement flag count: " + pos_count.ToString());
                    for (int i = 0; i < pos_count; i++)
                    {
                        pos.x = br.ReadInt16();
                        pos.y = br.ReadInt16();
                        heightmap.chunk42_data.Add(pos);
                    }
                }
                c42.Close();
            }
            else
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Movement flag data not found");
            SFChunkFileChunk c56 = f.GetChunkByID(56);
            if (c56 != null)
            {

                SFCoord pos = new SFCoord();
                using (BinaryReader br = c56.Open())
                {
                    int pos_count = c56.get_original_data_length() / 4;
                    for (int i = 0; i < pos_count; i++)
                    {
                        pos.x = br.ReadInt16();
                        pos.y = br.ReadInt16();
                        heightmap.chunk56_data.Add(pos);
                    }
                }
                c56.Close();
            }
            else
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Vision flag data not found");
            SFChunkFileChunk c60 = f.GetChunkByID(60);
            if (c60 != null)
            {
                byte unk;
                SFCoord pos = new SFCoord();
                using (BinaryReader br = c60.Open())
                {
                    int pos_count = br.ReadInt32();
                    for (int i = 0; i < pos_count; i++)
                    {
                        unk = br.ReadByte();
                        pos.x = br.ReadInt16();
                        pos.y = br.ReadInt16();
                        heightmap.chunk60_data.Add(new SFMapChunk60Data(unk, pos));
                    }
                }
                c60.Close();
            }
            else
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Additional flag data not found");

            // load metadata
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading player spawn data");
            tx.Text = "Loading metadata...";
            tx.GetCurrentParent().Refresh();
            metadata = new SFMapMetaData();

            SFChunkFileChunk c55 = f.GetChunkByID(55);
            if (c55 != null)
            {
                using (BinaryReader br = c55.Open())
                {
                    int player_count = br.ReadInt32();
                    metadata.spawns = new List<SFMapSpawn>();
                    for (int i = 0; i < player_count; i++)
                    {
                        short x = br.ReadInt16();
                        short y = br.ReadInt16();
                        ushort text_id = br.ReadUInt16();
                        short unknown = br.ReadInt16();

                        // discard spawns which do not have bindstones at specified positions
                        SFCoord pos = new SFCoord(x, y);
                        bool bindstone_exists = false;
                        foreach (SFMapInteractiveObject io in int_object_manager.int_objects)
                            if ((io.game_id == 769) && (io.grid_position == pos))
                            {
                                bindstone_exists = true;
                                break;
                            }
                        if (!bindstone_exists)
                        {
                            LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Spawn data not associated with bindstone, removing (position: " + pos.ToString() + ")");
                            player_count -= 1;
                            i -= 1;
                            continue;
                        }

                        metadata.spawns.Add(new SFMapSpawn());
                        metadata.spawns[i].pos = pos;
                        metadata.spawns[i].text_id = text_id;
                        metadata.spawns[i].unknown = unknown;
                    }
                    metadata.player_count = player_count;
                }
                c55.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Player spawns loaded: " + metadata.player_count.ToString());
            }
            else
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Player spawn data not found!");

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading team compositions");
            SFChunkFileChunk c53 = f.GetChunkByID(53);
            if (c53 != null)
            {
                using (BinaryReader br = c53.Open())
                {
                    metadata.multi_teams = new List<SFMapMultiplayerTeamComposition>();
                    if (c53.get_data_type() == 2)
                    {
                        LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Found team composition data, assuming multiplayer map type");
                        metadata.map_type = SFMapType.MULTIPLAYER;
                        int cur_teamcount = 2;
                        int p_num = br.ReadInt32();
                        while (cur_teamcount <= 4)
                        {
                            if (p_num == 0)
                            {
                                p_num = br.ReadInt32();
                                cur_teamcount += 1;
                                continue;
                            }

                            SFMapMultiplayerTeamComposition tcomp = new SFMapMultiplayerTeamComposition();
                            tcomp.team_count = cur_teamcount;
                            tcomp.players = new List<List<SFMapTeamPlayer>>();
                            for (int i = 0; i < tcomp.team_count; i++)
                            {
                                tcomp.players.Add(new List<SFMapTeamPlayer>());
                                for (int j = 0; j < p_num; j++)
                                {
                                    short x = br.ReadInt16();
                                    short y = br.ReadInt16();
                                    ushort text_id = br.ReadUInt16();
                                    tcomp.players[i].Add(new SFMapTeamPlayer(metadata.GetPlayerBySpawnPos(new SFCoord(x, y)), text_id));
                                }
                                p_num = br.ReadInt32();
                            }
                            metadata.multi_teams.Add(tcomp);
                            cur_teamcount += 1;
                        }
                    }
                    else if (c53.get_data_type() == 4)
                    {
                        LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Found team composition data, assuming coop map type");
                        metadata.map_type = SFMapType.COOP;
                        int cur_teamcount = 1;
                        int p_num = br.ReadInt32();
                        while (cur_teamcount <= 4)
                        {
                            if (p_num == 0)
                            {
                                p_num = br.ReadInt32();
                                cur_teamcount += 1;
                                continue;
                            }

                            SFMapMultiplayerTeamComposition tcomp = new SFMapMultiplayerTeamComposition();
                            tcomp.team_count = cur_teamcount;
                            tcomp.players = new List<List<SFMapTeamPlayer>>();
                            for (int i = 0; i < tcomp.team_count; i++)
                            {
                                tcomp.players.Add(new List<SFMapTeamPlayer>());
                                for (int j = 0; j < p_num; j++)
                                {
                                    short x = br.ReadInt16();
                                    short y = br.ReadInt16();
                                    ushort text_id = br.ReadUInt16();
                                    string s1 = Utility.ReadSFString(br);
                                    string s2 = Utility.ReadSFString(br);
                                    tcomp.players[i].Add(new SFMapTeamPlayer(metadata.GetPlayerBySpawnPos(new SFCoord(x, y)), text_id, s1, s2));

                                }
                                p_num = br.ReadInt32();
                            }
                            metadata.multi_teams.Add(tcomp);
                            cur_teamcount += 1;

                            // load coop spawns
                            metadata.coop_spawns = new List<SFMapCoopAISpawn>();
                            using (BinaryReader br2 = c29.Open())
                            {
                                if (c29.get_data_type() == 6)
                                {
                                    int obj_i = 0;
                                    while (br2.BaseStream.Position < br2.BaseStream.Length)
                                    {
                                        int x = br2.ReadInt16();
                                        int y = br2.ReadInt16();
                                        SFCoord pos = new SFCoord(x, y);
                                        int object_id = br2.ReadInt16();
                                        br2.ReadBytes(6);
                                        if (object_id != 2541)
                                            br2.ReadBytes(4);
                                        else
                                        {
                                            short spawn_id = br2.ReadInt16();
                                            short spawn_certain = br2.ReadInt16();
                                            metadata.coop_spawns.Add(new SFMapCoopAISpawn(
                                                object_manager.objects[obj_i],
                                                spawn_id,
                                                spawn_certain));
                                        }
                                        if ((object_id >= 65) && (object_id <= 67))    // editor only
                                            obj_i--;
                                        obj_i++;
                                    }
                                }
                            }
                            c29.Close();
                        }
                        // load minimap
                        int width = p_num;
                        int height = br.ReadInt32();
                        metadata.minimap = new SFMapMinimap();
                        metadata.minimap.width = width;
                        metadata.minimap.height = height;
                        metadata.minimap.texture_data = br.ReadBytes(width * height * 3);
                        metadata.minimap.GenerateTexture();
                    }
                    else
                        LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Found team composition data, but could not load it!");
                }
                c53.Close();
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Team composition data not found, assuming campaign map type");
                metadata.map_type = SFMapType.CAMPAIGN;
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading coop spawn parameters");
            SFChunkFileChunk c59 = f.GetChunkByID(59);
            if (c59 != null)
            {
                using (BinaryReader br = c59.Open())
                {
                    metadata.coop_spawn_params = new List<SFMapCoopSpawnParameters>();
                    for (int i = 0; i < 3; i++)
                    {
                        metadata.coop_spawn_params.Add(new SFMapCoopSpawnParameters());
                        metadata.coop_spawn_params[i].param1 = br.ReadSingle();
                        metadata.coop_spawn_params[i].param2 = br.ReadSingle();
                        metadata.coop_spawn_params[i].param3 = br.ReadSingle();
                        metadata.coop_spawn_params[i].param4 = br.ReadSingle();
                    }
                }
                c59.Close();
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Creating overlays");
            // create overlays, generation in relevant control...
            heightmap.OverlayCreate("TileMovementBlock", new OpenTK.Vector4(0.5f, 0, 0, 0.7f));
            //for (int i = 0; i < height; i++)
            //    for (int j = 0; j < width; j++)
            //       if (heightmap.texture_manager.texture_tiledata[heightmap.tile_data[i * width + j]].blocks_movement)
            //            heightmap.OverlayAdd("TileMovementBlock", new SFCoord(j, i));

            heightmap.OverlayCreate("ManualMovementBlock", new OpenTK.Vector4(1, 0, 0, 0.7f));
            //foreach (SFCoord p in heightmap.chunk42_data)
            //    heightmap.OverlayAdd("ManualMovementBlock", p);

            heightmap.OverlayCreate("ManualVisionBlock", new OpenTK.Vector4(1, 1, 0, 0.7f));
            //foreach (SFCoord p in heightmap.chunk56_data)
            //    heightmap.OverlayAdd("ManualVisionBlock", p);

            heightmap.OverlayCreate("LakeTile", new OpenTK.Vector4(0.4f, 0.4f, 0.9f, 0.7f));
            //for (int i = 0; i < heightmap.lake_data.Length; i++)
            //{
            //    SFCoord p = new SFCoord(i % width, i / width);
            //    if(heightmap.lake_data[i] > 0)
            //        heightmap.OverlayAdd("LakeTile", p);
            //}

            heightmap.OverlayCreate("ManualLakeTile", new OpenTK.Vector4(0.6f, 0.6f, 1.0f, 0.7f));

            heightmap.OverlayCreate("BuildingBlock", new OpenTK.Vector4(0.3f, 1f, 0.3f, 0.7f));
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (heightmap.building_data[i * width + j] != 0)
                        heightmap.OverlayAdd("BuildingBlock", new SFCoord(j, i));

            heightmap.OverlayCreate("DecorationTile", new OpenTK.Vector4(0.9f, 0.3f, 0.9f, 0.9f));


            foreach (SFMapHeightMapChunk chunk in heightmap.chunks)
            {
                chunk.OverlayUpdate("TileMovementBlock");
                chunk.OverlayUpdate("ManualMovementBlock");
                chunk.OverlayUpdate("ManualVisionBlock");
                chunk.OverlayUpdate("LakeTile");
                chunk.OverlayUpdate("ManualLakeTile");
                chunk.OverlayUpdate("BuildingBlock");
                chunk.OverlayUpdate("DecorationTile");
            }

            //heightmap.OverlaySetVisible("BuildingBlock", true);

            // selection helper stuff
            selection_helper.AssignToMap(this);

            // done

            f.Close();

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Load successful!");
            tx.Text = "Map loaded";

            return 0;
        }

        public int Save(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save() called, filename: " + filename);
            if (gamedata == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.Save(): No gamedata assigned to map!");
                return -1;
            }

            SFChunkFile.SFChunkFile f = new SFChunkFile.SFChunkFile();
            int res = f.Create(filename, SFChunkFileType.MAP);
            if (res != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.Save(): Failed to create map file (filename: " + filename + ")");
                return res;
            }

            int data_size;

            // chunk 2
            data_size = 3 + heightmap.width * heightmap.height;
            byte[] c2_data = new byte[data_size];
            using (BinaryWriter bw = new BinaryWriter(new MemoryStream(c2_data)))
            {
                bw.Write((short)heightmap.height);
                bw.Write((byte)0);
                bw.Write(heightmap.tile_data);
            }
            f.AddChunk(2, 0, true, 6, c2_data);

            // chunks 6
            short[] c6i_data = new short[heightmap.height];
            byte[] c6i_rawdata = new byte[heightmap.height * 2];
            for (int i = 0; i < heightmap.width; i++)
            {
                heightmap.GetRowRaw(i, ref c6i_data);
                Buffer.BlockCopy(c6i_data, 0, c6i_rawdata, 0, c6i_rawdata.Length);
                f.AddChunk(6, (short)i, true, 6, c6i_rawdata);
            }

            // chunk 3
            byte[] c3_data = new byte[3570];
            for (int i = 0; i < 255; i++)
            {
                c3_data[i * 14 + 0] = heightmap.texture_manager.texture_tiledata[i].ind1;
                c3_data[i * 14 + 1] = heightmap.texture_manager.texture_tiledata[i].ind2;
                c3_data[i * 14 + 2] = heightmap.texture_manager.texture_tiledata[i].ind3;
                c3_data[i * 14 + 3] = heightmap.texture_manager.texture_tiledata[i].weight1;
                c3_data[i * 14 + 4] = heightmap.texture_manager.texture_tiledata[i].weight2;
                c3_data[i * 14 + 5] = heightmap.texture_manager.texture_tiledata[i].weight3;
                c3_data[i * 14 + 6] = heightmap.texture_manager.texture_tiledata[i].reindex_data;
                c3_data[i * 14 + 7] = heightmap.texture_manager.texture_tiledata[i].reindex_index;
                c3_data[i * 14 + 8] = 255;
                c3_data[i * 14 + 9] = 128;
                c3_data[i * 14 + 10] = heightmap.texture_manager.texture_tiledata[i].material_property;
                c3_data[i * 14 + 11] = 255;
                c3_data[i * 14 + 12] = (byte)(heightmap.texture_manager.texture_tiledata[i].blocks_movement ? 1 : 0);
                c3_data[i * 14 + 13] = (byte)(heightmap.texture_manager.texture_tiledata[i].blocks_vision ? 1 : 0);
            }
            f.AddChunk(3, 0, true, 3, c3_data);

            // chunk 4
            byte[] c4_data = new byte[63];
            for (int i = 0; i < 63; i++)
            {
                c4_data[i] = (byte)heightmap.texture_manager.texture_id[i];
            }
            f.AddChunk(4, 0, true, 4, c4_data);

            // chunk 42
            heightmap.chunk42_data.Sort();
            byte[] c42_data = new byte[heightmap.chunk42_data.Count * 4];
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Movement flag count: " + heightmap.chunk42_data.Count.ToString());

            using (MemoryStream ms = new MemoryStream(c42_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < heightmap.chunk42_data.Count; i++)
                    {
                        bw.Write((short)heightmap.chunk42_data[i].x);
                        bw.Write((short)heightmap.chunk42_data[i].y);
                    }
                }
            }
            f.AddChunk(42, 0, true, 1, c42_data);

            // chunk 56
            heightmap.chunk56_data.Sort();
            byte[] c56_data = new byte[heightmap.chunk56_data.Count * 4];
            using (MemoryStream ms = new MemoryStream(c56_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < heightmap.chunk56_data.Count; i++)
                    {
                        bw.Write((short)heightmap.chunk56_data[i].x);
                        bw.Write((short)heightmap.chunk56_data[i].y);
                    }
                }
            }
            f.AddChunk(56, 0, true, 1, c56_data);

            // chunk 40
            byte[] c40_data = new byte[1 + lake_manager.lakes.Count * 7];
            using (MemoryStream ms = new MemoryStream(c40_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write((byte)lake_manager.lakes.Count);
                    for (int i = 0; i < lake_manager.lakes.Count; i++)
                    {
                        bw.Write((short)lake_manager.lakes[i].start.x);
                        bw.Write((short)lake_manager.lakes[i].start.y);
                        bw.Write((short)lake_manager.lakes[i].z_diff);
                        bw.Write((byte)lake_manager.lakes[i].type);
                    }
                }
            }
            f.AddChunk(40, 0, true, 1, c40_data);

            // chunk 31
            f.AddChunk(31, 0, true, 2, decoration_manager.dec_assignment);

            // chunk 32
            byte[] c32_data = new byte[22950];
            using (MemoryStream ms = new MemoryStream(c32_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < 255; i++)
                    {
                        for (int j = 0; j < 30; j++)
                            bw.Write((short)decoration_manager.dec_groups[i].dec_id[j]);
                        for (int j = 0; j < 30; j++)
                            bw.Write((byte)decoration_manager.dec_groups[i].weight[j]);
                    }
                }
            }
            f.AddChunk(32, 0, true, 1, c32_data);

            // chunk 29
            byte[] c29_data = new byte[object_manager.objects.Count * 16];
            using (MemoryStream ms = new MemoryStream(c29_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < object_manager.objects.Count; i++)
                    {
                        bw.Write((short)object_manager.objects[i].grid_position.x);
                        bw.Write((short)object_manager.objects[i].grid_position.y);
                        bw.Write((short)object_manager.objects[i].game_id);
                        bw.Write((short)object_manager.objects[i].angle);
                        bw.Write((short)object_manager.objects[i].npc_id);
                        bw.Write((short)object_manager.objects[i].unknown1);
                        if (object_manager.objects[i].game_id != 2541)
                            bw.Write((int)0);
                        else
                        {
                            SFMapCoopAISpawn coop_spawn = new SFMapCoopAISpawn();
                            if (!metadata.GetCoopAISpawnByObject(object_manager.objects[i], ref coop_spawn))
                                bw.Write((int)0);
                            else
                            {
                                bw.Write((short)coop_spawn.spawn_id);
                                bw.Write((short)coop_spawn.spawn_certain);
                            }
                        }
                    }
                }
            }
            f.AddChunk(29, 0, true, 6, c29_data);

            // chunk 35
            byte[] c35_data = new byte[portal_manager.portals.Count * 8];
            using (MemoryStream ms = new MemoryStream(c35_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < portal_manager.portals.Count; i++)
                    {
                        bw.Write((short)portal_manager.portals[i].grid_position.x);
                        bw.Write((short)portal_manager.portals[i].grid_position.y);
                        bw.Write((short)portal_manager.portals[i].angle);
                        bw.Write((short)portal_manager.portals[i].game_id);
                    }
                }
            }
            f.AddChunk(35, 0, true, 1, c35_data);

            // chunk 30
            byte[] c30_data = new byte[int_object_manager.int_objects.Count * 9];
            using (MemoryStream ms = new MemoryStream(c30_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < int_object_manager.int_objects.Count; i++)
                    {
                        bw.Write((short)int_object_manager.int_objects[i].grid_position.x);
                        bw.Write((short)int_object_manager.int_objects[i].grid_position.y);
                        bw.Write((short)int_object_manager.int_objects[i].game_id);
                        bw.Write((short)int_object_manager.int_objects[i].angle);
                        bw.Write((byte)int_object_manager.int_objects[i].unk_byte);
                    }
                }
            }
            f.AddChunk(30, 0, true, 1, c30_data);

            // chunk 11
            byte[] c11_data = new byte[building_manager.buildings.Count * 11];
            using (MemoryStream ms = new MemoryStream(c11_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < building_manager.buildings.Count; i++)
                    {
                        bw.Write((short)building_manager.buildings[i].grid_position.x);
                        bw.Write((short)building_manager.buildings[i].grid_position.y);
                        bw.Write((short)building_manager.buildings[i].angle);
                        bw.Write((short)building_manager.buildings[i].npc_id);
                        bw.Write((byte)building_manager.buildings[i].game_id);
                        bw.Write((byte)building_manager.buildings[i].level);
                        bw.Write((byte)building_manager.buildings[i].race_id);
                    }
                }
            }
            f.AddChunk(11, 0, true, (short)3, c11_data);

            // chunk 12
            byte[] c12_data = new byte[unit_manager.units.Count * 14];
            using (MemoryStream ms = new MemoryStream(c12_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < unit_manager.units.Count; i++)
                    {
                        bw.Write((short)unit_manager.units[i].grid_position.x);
                        bw.Write((short)unit_manager.units[i].grid_position.y);
                        bw.Write((short)unit_manager.units[i].angle);
                        bw.Write((short)unit_manager.units[i].game_id);
                        bw.Write((short)unit_manager.units[i].npc_id);
                        bw.Write((short)unit_manager.units[i].unknown);
                        bw.Write((byte)unit_manager.units[i].group);
                        bw.Write((byte)unit_manager.units[i].unknown2);
                    }
                }
            }
            f.AddChunk(12, 0, true, 5, c12_data);

            // chunks 44 and 46 unused?

            // chunk 53
            short chunk_type;
            byte[] team_array = new byte[0];
            switch (metadata.map_type)
            {
                case SFMapType.CAMPAIGN:
                    chunk_type = 0;
                    break;
                case SFMapType.COOP:
                    chunk_type = 4;
                    team_array = metadata.TeamsToArray(1, true);
                    break;
                case SFMapType.MULTIPLAYER:
                    chunk_type = 2;
                    team_array = metadata.TeamsToArray(2, false);
                    break;
                default:
                    chunk_type = 0;
                    break;
            }
            if (chunk_type != 0)
            {
                byte[] c53_data = new byte[team_array.Length + 8 + (metadata.minimap.width * metadata.minimap.height * 3)];
                using (MemoryStream ms = new MemoryStream(c53_data))
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(team_array);
                        bw.Write(metadata.minimap.width);
                        bw.Write(metadata.minimap.height);
                        bw.Write(metadata.minimap.texture_data);
                    }
                }
                f.AddChunk(53, 0, true, chunk_type, c53_data);
            }

            // chunk 55
            byte[] c55_data = new byte[4 + metadata.spawns.Count * 8];
            using (MemoryStream ms = new MemoryStream(c55_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write((int)metadata.spawns.Count);
                    for (int i = 0; i < metadata.spawns.Count; i++)
                    {
                        bw.Write((short)metadata.spawns[i].pos.x);
                        bw.Write((short)metadata.spawns[i].pos.y);
                        bw.Write((ushort)metadata.spawns[i].text_id);
                        bw.Write((short)metadata.spawns[i].unknown);
                    }
                }
            }
            f.AddChunk(55, 0, true, 1, c55_data);

            // chunk 60
            byte[] c60_data = new byte[4 + 5 * heightmap.chunk60_data.Count];
            using (MemoryStream ms = new MemoryStream(c60_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(heightmap.chunk60_data.Count);
                    for (int i = 0; i < heightmap.chunk60_data.Count; i++)
                    {
                        bw.Write((byte)heightmap.chunk60_data[i].unknown);
                        bw.Write((short)heightmap.chunk60_data[i].pos.x);
                        bw.Write((short)heightmap.chunk60_data[i].pos.y);
                    }
                }
            }
            f.AddChunk(60, 0, true, 1, c60_data);

            // chunk 8000 not used?

            // chunk 59
            if (metadata.map_type == SFMapType.COOP)
            {
                float[] c59_data = new float[12];
                byte[] c59_rawdata = new byte[48];
                for (int i = 0; i < metadata.coop_spawn_params.Count; i++)
                {
                    c59_data[i * 4 + 0] = metadata.coop_spawn_params[i].param1;
                    c59_data[i * 4 + 1] = metadata.coop_spawn_params[i].param2;
                    c59_data[i * 4 + 2] = metadata.coop_spawn_params[i].param3;
                    c59_data[i * 4 + 3] = metadata.coop_spawn_params[i].param4;
                }
                Buffer.BlockCopy(c59_data, 0, c59_rawdata, 0, c59_rawdata.Length);
                f.AddChunk(59, 0, true, 1, c59_rawdata);
            }

            f.Close();
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Map saved successfully");

            return 0;
        }

        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Unload() called");
            heightmap.Unload();
            metadata.Unload();             // minimap texture
            selection_helper.Dispose();    // selection 3d meshpublic SFMapHeightMap heightmap { get; private set; } = null;
            building_manager = null;
            unit_manager = null;
            object_manager = null;
            int_object_manager = null;
            decoration_manager = null;
            portal_manager = null;
            lake_manager = null;
            metadata = null;
            npc_manager = null;
    }

        public void AddDecoration(int game_id, SFCoord pos)
        {
            SFMapDecoration dec = decoration_manager.AddDecoration(game_id, pos);


            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[dec.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
        }

        public void AddObject(int game_id, SFCoord pos, int angle, int npc_id, int unk1)
        {
            SFMapObject obj = object_manager.AddObject(game_id, pos, angle, unk1);
            obj.npc_id = npc_id;
            if (npc_id != 0)
                npc_manager.AddNPCRef(npc_id, obj);


            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[obj.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);


            heightmap.GetChunk(pos).AddObject(obj);
            SFRenderEngine.scene.objects_static[obj.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public int DeleteObject(int object_map_index)
        {
            SFMapObject obj = null;
            if ((object_manager.objects.Count <= object_map_index) || (object_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.DeleteObject(): Invalid object index! Object index = " + object_map_index.ToString());
                return -1;
            }
            obj = object_manager.objects[object_map_index];

            object_manager.RemoveObject(obj);

            if (obj.npc_id != 0)
                npc_manager.RemoveNPCRef(obj.npc_id);

            return 0;
        }

        public int ReplaceObject(int object_map_index, ushort new_object_id)
        {
            SFMapObject obj = null;
            if ((object_manager.objects.Count <= object_map_index) || (object_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.ReplaceObject(): invalid object index! Object index = " + object_map_index.ToString());
                return -1;
            }
            obj = object_manager.objects[object_map_index];

            SFRenderEngine.scene.DeleteObject(obj.GetObjectName());
            SFRenderEngine.scene.AddObjectObject(new_object_id, obj.GetObjectName(), true);

            obj.game_id = new_object_id;

            // object transform
            float z = heightmap.GetZ(obj.grid_position) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[obj.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)obj.grid_position.x, (float)z, (float)(height - obj.grid_position.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(obj.angle);

            return 0;
        }

        // todo: probably account for offset?
        public int RotateObject(int object_map_index, int angle)
        {
            SFMapObject obj = null;
            if ((object_manager.objects.Count <= object_map_index) || (object_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.RotateObject(): invalid object index! Object index = " + object_map_index.ToString());
                return -1;
            }
            obj = object_manager.objects[object_map_index];

            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[obj.GetObjectName()];
            _obj.SetAnglePlane(angle);

            return 0;
        }

        public int MoveObject(int object_map_index, SFCoord new_pos)
        {
            SFMapObject obj = null;
            if ((object_manager.objects.Count <= object_map_index) || (object_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MoveObject(): Invalid object index! Object index = " + object_map_index.ToString());
                return -1;
            }
            obj = object_manager.objects[object_map_index];
            /*if (!heightmap.CanMoveToPosition(new_pos))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MoveObject(): Can't move object to position " + new_pos.ToString());
                return -2;
            }*/

            // move unit and set chunk dependency
            heightmap.GetChunk(obj.grid_position).objects.Remove(obj);
            obj.grid_position = new_pos;
            heightmap.GetChunk(obj.grid_position).objects.Add(obj);

            // change visual transform
            float z = heightmap.GetZ(new_pos) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[obj.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)new_pos.x, (float)z, (float)(height - new_pos.y - 1));

            return 0;
        }

        public void AddInteractiveObject(int game_id, SFCoord pos, int angle, int unk_byte)
        {
            SFMapInteractiveObject obj = int_object_manager.AddInteractiveObject(game_id, pos, angle, unk_byte);

            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[obj.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);


            heightmap.GetChunk(pos).AddInteractiveObject(obj);
            SFRenderEngine.scene.objects_static[obj.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public int MoveInteractiveObject(int int_object_map_index, SFCoord new_pos)
        {
            SFMapInteractiveObject int_obj = null;
            if ((int_object_manager.int_objects.Count <= int_object_map_index) || (int_object_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MoveInteractiveObject(): Invalid interactive object index! Interactive object index = " + int_object_map_index.ToString());
                return -1;
            }
            int_obj = int_object_manager.int_objects[int_object_map_index];
            /*if (!heightmap.CanMoveToPosition(new_pos))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MoveInteractiveObject(): Can't move interactive object to position " + new_pos.ToString());
                return -2;
            }*/

            // move unit and set chunk dependency
            heightmap.GetChunk(int_obj.grid_position).int_objects.Remove(int_obj);
            int_obj.grid_position = new_pos;
            heightmap.GetChunk(int_obj.grid_position).int_objects.Add(int_obj);

            // change visual transform
            float z = heightmap.GetZ(new_pos) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[int_obj.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)new_pos.x, (float)z, (float)(height - new_pos.y - 1));

            return 0;
        }

        public int DeleteInteractiveObject(int int_object_map_index)
        {
            SFMapInteractiveObject int_obj = null;
            if ((int_object_manager.int_objects.Count <= int_object_map_index) || (int_object_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.DeleteInteractiveObject(): Invalid interactive object index! Interactive object index = " + int_object_map_index.ToString());
                return -1;
            }
            int_obj = int_object_manager.int_objects[int_object_map_index];

            int_object_manager.RemoveInteractiveObject(int_obj);

            return 0;
        }

        // todo: probably account for offset?
        public int RotateInteractiveObject(int int_object_map_index, int angle)
        {
            SFMapInteractiveObject int_obj = null;
            if ((int_object_manager.int_objects.Count <= int_object_map_index) || (int_object_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.RotateInteractiveObject(): invalid interactive object index! Interactive object index = " + int_object_map_index.ToString());
                return -1;
            }
            int_obj = int_object_manager.int_objects[int_object_map_index];

            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[int_obj.GetObjectName()];
            _obj.SetAnglePlane(angle);

            return 0;
        }

        public int ReplaceMonument(int monument_index, int new_monument_type)
        {
            if ((new_monument_type < 0) || (new_monument_type > 6))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.ReplaceMonument(): invalid monument type! Monument type = " + new_monument_type.ToString());
                return -1;
            }

            List<int> monument_indexes = new List<int>();
            for (int i = 0; i < int_object_manager.int_objects.Count; i++)
                if ((int_object_manager.int_objects[i].game_id >= 771) && (int_object_manager.int_objects[i].game_id <= 777))
                    monument_indexes.Add(i);

            if ((monument_index < 0) || (monument_index >= monument_indexes.Count))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.ReplaceMonument(): invalid monument index! Monument index = " + monument_index.ToString());
                return -2;
            }

            SFMapInteractiveObject io = int_object_manager.int_objects[monument_indexes[monument_index]];

            SFRenderEngine.scene.DeleteObject(io.GetObjectName());
            SFRenderEngine.scene.AddObjectObject(new_monument_type + 771, io.GetObjectName(), true);
            io.game_id = new_monument_type + 771;

            float z = heightmap.GetZ(io.grid_position) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[io.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)io.grid_position.x, (float)z, (float)(height - io.grid_position.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(io.angle);

            return 0;
        }

        public void AddBuilding(int game_id, SFCoord pos, int angle, int npc_id, int lvl, int race_id)
        {
            SFMapBuilding bld = building_manager.AddBuilding(game_id, pos, angle, npc_id, lvl, race_id);
            if (npc_id != 0)
                npc_manager.AddNPCRef(npc_id, bld);

            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[bld.GetObjectName()];

            OpenTK.Vector2 b_offset = building_manager.building_collision[(ushort)bld.game_id].collision_mesh.origin;
            float angle_rad = (float)(angle * Math.PI / 180);
            OpenTK.Vector2 b_offset_rotated = new OpenTK.Vector2(b_offset.X, b_offset.Y);
            b_offset_rotated.X = (float)((Math.Cos(angle_rad) * b_offset.X) - (Math.Sin(angle_rad) * b_offset.Y));
            b_offset_rotated.Y = (float)((Math.Sin(angle_rad) * b_offset.X) + (Math.Cos(angle_rad) * b_offset.Y));

            _obj.Position = new OpenTK.Vector3((float)pos.x - b_offset_rotated.X, (float)z, (float)(height - pos.y - 1) + b_offset_rotated.Y);
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);

            heightmap.GetChunk(pos).AddBuilding(bld);
            SFRenderEngine.scene.objects_static[bld.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public int DeleteBuilding(int building_map_index)
        {
            SFMapBuilding building = null;
            if ((building_manager.buildings.Count <= building_map_index) || (building_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.DeleteBuilding(): Invalid building index! Building index = " + building_map_index.ToString());
                return -1;
            }
            building = building_manager.buildings[building_map_index];

            building_manager.RemoveBuilding(building);

            if (building.npc_id != 0)
                npc_manager.RemoveNPCRef(building.npc_id);

            return 0;
        }

        public int ReplaceBuilding(int building_map_index, ushort new_building_id)
        {
            SFMapBuilding building;
            if ((building_manager.buildings.Count <= building_map_index) || (building_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.ReplaceBuilding(): invalid building index! Building index = " + building_map_index.ToString());
                return -1;
            }
            building = building_manager.buildings[building_map_index];

            SFRenderEngine.scene.DeleteObject(building.GetObjectName());
            building_manager.RemoveBuildingCollisionBoundary(building.game_id);
            building_manager.AddBuildingCollisionBoundary(new_building_id);
            SFRenderEngine.scene.AddObjectBuilding(new_building_id, building.GetObjectName());

            building.game_id = new_building_id;
            
            SFCoord pos = building.grid_position;
            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[building.GetObjectName()];

            OpenTK.Vector2 b_offset = building_manager.building_collision[(ushort)building.game_id].collision_mesh.origin;
            float angle_rad = (float)(building.angle * Math.PI / 180);
            OpenTK.Vector2 b_offset_rotated = new OpenTK.Vector2(b_offset.X, b_offset.Y);
            b_offset_rotated.X = (float)((Math.Cos(angle_rad) * b_offset.X) - (Math.Sin(angle_rad) * b_offset.Y));
            b_offset_rotated.Y = (float)((Math.Sin(angle_rad) * b_offset.X) + (Math.Cos(angle_rad) * b_offset.Y));

            _obj.Position = new OpenTK.Vector3((float)pos.x - b_offset_rotated.X, (float)z, (float)(height - pos.y - 1) + b_offset_rotated.Y);
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(building.angle);

            return 0;
        }

        public int RotateBuilding(int building_map_index, int angle)
        {
            SFMapBuilding building = null;
            if ((building_manager.buildings.Count <= building_map_index) || (building_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.RotateBuilding(): invalid building index! Building index = " + building_map_index.ToString());
                return -1;
            }
            building = building_manager.buildings[building_map_index];
            
            SFCoord pos = building.grid_position;
            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[building.GetObjectName()];

            OpenTK.Vector2 b_offset = building_manager.building_collision[(ushort)building.game_id].collision_mesh.origin;
            float angle_rad = (float)(building.angle * Math.PI / 180);
            OpenTK.Vector2 b_offset_rotated = new OpenTK.Vector2(b_offset.X, b_offset.Y);
            b_offset_rotated.X = (float)((Math.Cos(angle_rad) * b_offset.X) - (Math.Sin(angle_rad) * b_offset.Y));
            b_offset_rotated.Y = (float)((Math.Sin(angle_rad) * b_offset.X) + (Math.Cos(angle_rad) * b_offset.Y));

            _obj.Position = new OpenTK.Vector3((float)pos.x - b_offset_rotated.X, (float)z, (float)(height - pos.y - 1) + b_offset_rotated.Y);
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(building.angle);

            //render_engine.scene_manager.objects_static[building.GetObjectName()+"_OUTLINE"].Rotation = OpenTK.Quaternion.FromEulerAngles(0, (float)(angle * Math.PI / 180), 0);



            return 0;
        }

        public int MoveBuilding(int building_map_index, SFCoord new_pos)
        {
            SFMapBuilding building = null;
            if ((building_manager.buildings.Count <= building_map_index) || (building_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MoveBuilding(): Invalid building index! Building index = " + building_map_index.ToString());
                return -1;
            }
            building = building_manager.buildings[building_map_index];
            /*if (!heightmap.CanMoveToPosition(new_pos))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MoveBuilding(): Can't move building to position " + new_pos.ToString());
                return -2;
            }*/

            // move unit and set chunk dependency
            heightmap.GetChunk(building.grid_position).buildings.Remove(building);
            building.grid_position = new_pos;
            heightmap.GetChunk(building.grid_position).buildings.Add(building);

            // change visual transform
            float z = heightmap.GetZ(new_pos) / 100.0f;
            SF3D.Object3D obj = SFRenderEngine.scene.objects_static[building.GetObjectName()];

            OpenTK.Vector2 b_offset = building_manager.building_collision[(ushort)building.game_id].collision_mesh.origin;
            float angle_rad = (float)(building.angle * Math.PI / 180);
            OpenTK.Vector2 b_offset_rotated = new OpenTK.Vector2(b_offset.X, b_offset.Y);
            b_offset_rotated.X = (float)((Math.Cos(angle_rad) * b_offset.X) - (Math.Sin(angle_rad) * b_offset.Y));
            b_offset_rotated.Y = (float)((Math.Sin(angle_rad) * b_offset.X) + (Math.Cos(angle_rad) * b_offset.Y));

            obj.Position = new OpenTK.Vector3((float)new_pos.x - b_offset_rotated.X, (float)z, (float)(height - new_pos.y - 1) + b_offset_rotated.Y);
            obj.Scale = new OpenTK.Vector3(100 / 128f);
            obj.SetAnglePlane(building.angle);

            return 0;
        }

        public void AddPortal(int game_id, SFCoord pos, int angle)
        {
            SFMapPortal ptl = portal_manager.AddPortal(game_id, pos, angle);


            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[ptl.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);


            heightmap.GetChunk(pos).AddPortal(ptl);
            SFRenderEngine.scene.objects_static[ptl.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public int DeletePortal(int portal_map_index)
        {
            SFMapPortal portal = null;
            if ((portal_manager.portals.Count <= portal_map_index) || (portal_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.DeletePortal(): Invalid portal index! Portal index = " + portal_map_index.ToString());
                return -1;
            }
            portal = portal_manager.portals[portal_map_index];

            portal_manager.RemovePortal(portal);

            return 0;
        }

        public int MovePortal(int portal_map_index, SFCoord new_pos)
        {
            SFMapPortal portal = null;
            if ((portal_manager.portals.Count <= portal_map_index) || (portal_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MovePortal(): Invalid portal index! Portal index = " + portal_map_index.ToString());
                return -1;
            }
            portal = portal_manager.portals[portal_map_index];
            /*if (!heightmap.CanMoveToPosition(new_pos))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MovePortal(): Can't move portal to position " + new_pos.ToString());
                return -2;
            }*/

            // move unit and set chunk dependency
            heightmap.GetChunk(portal.grid_position).portals.Remove(portal);
            portal.grid_position = new_pos;
            heightmap.GetChunk(portal.grid_position).portals.Add(portal);

            // change visual transform
            float z = heightmap.GetZ(new_pos) / 100.0f;
            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[portal.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)new_pos.x, (float)z, (float)(height - new_pos.y - 1));

            return 0;
        }

        // todo: probably account for offset?
        public int RotatePortal(int portal_map_index, int angle)
        {
            SFMapPortal portal = null;
            if ((portal_manager.portals.Count <= portal_map_index) || (portal_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.RotatePortal(): invalid portal index! Portal index = " + portal_map_index.ToString());
                return -1;
            }
            portal = portal_manager.portals[portal_map_index];

            SF3D.Object3D _obj = SFRenderEngine.scene.objects_static[portal.GetObjectName()];
            _obj.SetAnglePlane(angle);

            return 0;
        }

        public void AddUnit(int game_id, SFCoord pos, int angle, int npc_id, int unknown, int group, int unknown2)
        {
            // 1. add new unit in unit manager
            SFMapUnit unit = unit_manager.AddUnit(game_id, pos, angle);
            unit.npc_id = npc_id;
            unit.unknown = unknown;
            unit.group = group;
            unit.unknown2 = unknown2;
            if (npc_id != 0)
                npc_manager.AddNPCRef(npc_id, unit);

            // 2. modify object transform and appearance

            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D obj = SFRenderEngine.scene.objects_static[unit.GetObjectName()];
            obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            obj.SetAnglePlane(angle);
            // find unit scale
            int unit_index = gamedata.categories[17].get_element_index(game_id);
            if (unit_index == -1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.AddUnit(): Unit with given id does not exist! Unit id = "+game_id.ToString());
                throw new InvalidDataException("SFMap.AddUnit(): Invalid unit ID!");
            }
            SFCFF.SFCategoryElement unit_data = gamedata.categories[17].get_element(unit_index);
            unit_index = gamedata.categories[3].get_element_index((ushort)unit_data.get_single_variant(2).value);
            if (unit_index == -1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.AddUnit(): Unit stats with given id does not exist! Unit stats id = " + unit_data.get_single_variant(2).value.ToString());
                throw new InvalidDataException("SFMap.AddUnit(): Invalid unit data!");
            }
            unit_data = gamedata.categories[3].get_element(unit_index);
            float unit_size = Math.Max(((ushort)unit_data.get_single_variant(19).value), (ushort)40) / 100.0f;
            obj.Scale = new OpenTK.Vector3(unit_size*100/128);
        }

        public int MoveUnit(int unit_map_index, SFCoord new_pos)
        {
            SFMapUnit unit = null;
            if ((unit_manager.units.Count <= unit_map_index) || (unit_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MoveUnit(): Invalid unit index! Unit index = " + unit_map_index.ToString());
                return -1;
            }
            unit = unit_manager.units[unit_map_index];
            if (!heightmap.CanMoveToPosition(new_pos))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MoveUnit(): Can't move unit to position "+new_pos.ToString());
                return -2;
            }

            // move unit and set chunk dependency
            heightmap.GetChunk(unit.grid_position).units.Remove(unit);
            unit.grid_position = new_pos;
            heightmap.GetChunk(unit.grid_position).units.Add(unit);

            // change visual transform
            float z = heightmap.GetZ(new_pos) / 100.0f;
            SF3D.Object3D obj = SFRenderEngine.scene.objects_static[unit.GetObjectName()];
            obj.Position = new OpenTK.Vector3((float)new_pos.x, (float)z, (float)(height - new_pos.y - 1));

            return 0;
        }

        public int RotateUnit(int unit_map_index, int angle)
        {
            SFMapUnit unit = null;
            if ((unit_manager.units.Count <= unit_map_index) || (unit_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.RotateUnit(): Invalid unit index! Unit index = " + unit_map_index.ToString());
                return -1;
            }
            unit = unit_manager.units[unit_map_index];

            SF3D.Object3D obj = SFRenderEngine.scene.objects_static[unit.GetObjectName()];
            obj.SetAnglePlane(angle);

            return 0;
        }

        public int DeleteUnit(int unit_map_index)
        {
            SFMapUnit unit = null;
            if ((unit_manager.units.Count <= unit_map_index) || (unit_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.DeleteUnit(): Invalid unit index! Unit index = " + unit_map_index.ToString());
                return -1;
            }
            unit = unit_manager.units[unit_map_index];

            unit_manager.RemoveUnit(unit);

            if (unit.npc_id != 0)
                npc_manager.RemoveNPCRef(unit.npc_id);

            return 0;
        }

        public int ReplaceUnit(int unit_map_index, ushort new_unit_id)
        {
            SFMapUnit unit = null;
            if ((unit_manager.units.Count <= unit_map_index) || (unit_map_index < 0))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.ReplaceUnit(): Invalid unit index! Unit index = " + unit_map_index.ToString());
                return -1;
            }
            unit = unit_manager.units[unit_map_index];

            SFRenderEngine.scene.DeleteObject(unit.GetObjectName());
            SFRenderEngine.scene.AddObjectUnit(new_unit_id, unit.GetObjectName(), false);

            unit.game_id = new_unit_id;

            // object transform
            float z = heightmap.GetZ(unit.grid_position) / 100.0f;
            SF3D.Object3D obj = SFRenderEngine.scene.objects_static[unit.GetObjectName()];
            obj.Position = new OpenTK.Vector3((float)unit.grid_position.x, (float)z, (float)(height - unit.grid_position.y - 1));
            obj.SetAnglePlane(unit.angle);
            // unit scale
            int unit_index = gamedata.categories[17].get_element_index(unit.game_id);
            if (unit_index == -1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.ReplaceUnit(): Unit with given id does not exist! Unit id = " + unit.game_id.ToString());
                throw new InvalidDataException("SFMap.ReplaceUnit(): Invalid unit ID!");
            }
            SFCFF.SFCategoryElement unit_data = gamedata.categories[17].get_element(unit_index);
            unit_index = gamedata.categories[3].get_element_index((ushort)unit_data.get_single_variant(2).value);
            if (unit_index == -1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.AddUnit(): Unit stats with given id does not exist! Unit stats id = " + unit_data.get_single_variant(2).value.ToString());
                throw new InvalidDataException("SFMap.ReplaceUnit(): Invalid unit data!");
            }
            unit_data = gamedata.categories[3].get_element(unit_index);
            float unit_size = Math.Max(((ushort)unit_data.get_single_variant(19).value), (ushort)40) / 100.0f;
            obj.Scale = new OpenTK.Vector3(unit_size * 100 / 128);

            return 0;
        }
    }
}
