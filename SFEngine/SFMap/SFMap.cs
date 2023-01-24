using SFEngine.SF3D.SFRender;
using SFEngine.SFChunk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SFEngine.SFMap
{
    public delegate void dOnMapLoadStageChange(string text, System.Drawing.Color col);

    public class SFMap
    {
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
        public SFMapWeatherManager weather_manager { get; private set; } = null;
        public SFMapMetaData metadata { get; private set; } = null;
        public SFMapSelectionHelper selection_helper { get; private set; } = new SFMapSelectionHelper();
        public SFMapOcean ocean { get; private set; } = new SFMapOcean();
        public uint PlatformID { get; private set; } = 6666;

        public dOnMapLoadStageChange OnMapLoadStateChange = OnMapLoadStateChange_dummy;

        private static void OnMapLoadStateChange_dummy(string text, System.Drawing.Color col)
        {

        }

        public int Load(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load() called, filename: " + filename);
            OnMapLoadStateChange.Invoke("Loading...", System.Drawing.Color.Black);
            SFChunk.SFChunkFile f = new SFChunk.SFChunkFile();
            int res = f.OpenFile(filename);
            if (res != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not open map file!");
                return res;
            }

            // load map size and tile indices

            short size;

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading tile data");
            OnMapLoadStateChange.Invoke("Loading map data...", System.Drawing.Color.Black);

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
            else
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not find tile definitions! This is seriously bad!");
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
            else
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not load textures! This is very bad and will cause instability!");
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
                    LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not read heightmap chunk ID " + i.ToString() + "! This is very bad!");
                }
                else
                {
                    using (BinaryReader br = c6_i.Open())
                    {
                        heightmap.SetRowRaw(i, br.ReadBytes(size * 2));
                    }
                    c6_i.Close();
                }
            }

            // generate heightmap models and reindex textures
            heightmap.Generate();

            // load buildings
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading buildings");
            OnMapLoadStateChange.Invoke("Loading buildings...", System.Drawing.Color.Black);

            building_manager = new SFMapBuildingManager() { map = this };
            SFChunkFileChunk c11 = f.GetChunkByID(11);
            if (c11 != null)
            {
                using (BinaryReader br = c11.Open())
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int angle = br.ReadInt16();
                        int npc_id = br.ReadInt16();   // presumed
                        int b_type = br.ReadByte();
                        int b_lvl = 1;
                        int race_id = -1;
                        if (c11.header.ChunkDataType > 1)
                        {
                            b_lvl = br.ReadByte();
                        }

                        if (c11.header.ChunkDataType > 2)
                        {
                            race_id = br.ReadByte();
                        }

                        AddBuilding(b_type, pos, angle, npc_id, b_lvl, race_id);
                    }
                }
                c11.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Buildings loaded: " + building_manager.buildings.Count.ToString());
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not find building data");
            }

            // load units
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading units");
            OnMapLoadStateChange.Invoke("Loading units...", System.Drawing.Color.Black);

            unit_manager = new SFMapUnitManager() { map = this };
            SFChunkFileChunk c12 = f.GetChunkByID(12);
            if (c12 != null)
            {
                using (BinaryReader br = c12.Open())
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int flags = br.ReadInt16();
                        int unit_id = br.ReadInt16();
                        int npc_id = br.ReadUInt16();
                        int unknown = br.ReadUInt16();
                        int group = br.ReadByte();
                        int unknown2 = 0;
                        if (c12.header.ChunkDataType >= 5)
                        {
                            unknown2 = br.ReadByte();
                        }

                        AddUnit(unit_id, pos, flags, npc_id, unknown, group, unknown2);
                    }
                }
                c12.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Units loaded: " + unit_manager.units.Count.ToString());
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not find unit data");
            }

            // load objects
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading objects");
            OnMapLoadStateChange.Invoke("Loading objects...", System.Drawing.Color.Black);

            object_manager = new SFMapObjectManager() { map = this };
            SFChunkFileChunk c29 = f.GetChunkByID(29);
            if (c29 != null)
            {
                using (BinaryReader br = c29.Open())
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int object_id = br.ReadInt16();
                        int angle = br.ReadInt16();
                        int npc_id = br.ReadUInt16();
                        int unk1 = 0;
                        if (c29.header.ChunkDataType == 6)
                        {
                            unk1 = br.ReadUInt16();
                            br.ReadBytes(4);
                        }
                        else if (c29.header.ChunkDataType == 5)
                        {
                            unk1 = br.ReadUInt16();
                            br.ReadBytes(2);
                        }
                        else if (c29.header.ChunkDataType == 4)
                        {
                            unk1 = br.ReadUInt16();
                        }

                        if ((object_id >= 65) && (object_id <= 67))   // editor only
                        {
                            continue;
                        }

                        AddObject(object_id, pos, angle, npc_id, unk1);
                    }
                }
                c29.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Objects loaded: " + object_manager.objects.Count.ToString());
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not find object data");
            }

            // load interactive objects
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading interactive objects");
            OnMapLoadStateChange.Invoke("Loading interactive objects...", System.Drawing.Color.Black);

            int_object_manager = new SFMapInteractiveObjectManager() { map = this };
            SFChunkFileChunk c30 = f.GetChunkByID(30);
            if (c30 != null)
            {
                using (BinaryReader br = c30.Open())
                {
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
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not find interactive object data");
            }

            // load decorations
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading decal data");
            OnMapLoadStateChange.Invoke("Loading decorations...", System.Drawing.Color.Black);

            decoration_manager = new SFMapDecorationManager() { map = this };
            SFChunkFileChunk c31 = f.GetChunkByID(31);
            if (c31 != null)
            {
                using (BinaryReader br = c31.Open())
                {
                    decoration_manager.dec_assignment = br.ReadBytes(1048576);
                }
                c31.Close();
            }
            else
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not find decal data! This is bad...");
                decoration_manager.dec_assignment = new byte[1048576];
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
                        {
                            decoration_manager.dec_groups[i].dec_id[j] = br.ReadUInt16();
                        }

                        for (int j = 0; j < 30; j++)
                        {
                            decoration_manager.dec_groups[i].weight[j] = br.ReadByte();
                        }
                    }
                }
                c32.Close();
            }
            else
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not find decal group data! This is bad...");
                for (int i = 0; i < 255; i++)
                {
                    decoration_manager.dec_groups[i] = new SFMapDecorationGroup();
                }
            }
            decoration_manager.GenerateDecorations();

            // load portals
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading portals");
            OnMapLoadStateChange.Invoke("Loading portals...", System.Drawing.Color.Black);

            portal_manager = new SFMapPortalManager() { map = this };
            SFChunkFileChunk c35 = f.GetChunkByID(35);
            if (c35 != null)
            {
                using (BinaryReader br = c35.Open())
                {
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
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not find portal data");
            }

            // load lakes
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading lakes");
            OnMapLoadStateChange.Invoke("Loading lakes...", System.Drawing.Color.Black);

            lake_manager = new SFMapLakeManager() { map = this };
            SFChunkFileChunk c40 = f.GetChunkByID(40);
            if (c40 != null)
            {
                // these do nothing, because in saved maps no lakes overlap
                List<SFMapLake> consumed_lakes = new List<SFMapLake>();
                List<int> consumed_lake_indices = new List<int>();
                using (BinaryReader br = c40.Open())
                {
                    int lake_count = br.ReadByte();
                    for (int i = 0; i < lake_count; i++)
                    {
                        short x = br.ReadInt16();
                        short y = br.ReadInt16();
                        short z_diff = br.ReadInt16();
                        byte type = br.ReadByte();
                        lake_manager.AddLake(new SFCoord(x, y), z_diff, type, SFEngine.Utility.NO_INDEX, consumed_lakes, consumed_lake_indices);
                    }
                }
                c40.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Lakes found: " + lake_manager.lakes.Count.ToString());
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Could not find lake data");
            }

            // load map flags
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading map flags");
            OnMapLoadStateChange.Invoke("Loading map flags...", System.Drawing.Color.Black);

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
                        heightmap.SetFlag(pos, SFMapHeightMapFlag.FLAG_MOVEMENT, true);
                        //heightmap.chunk42_data.Add(pos);
                    }
                }
                c42.Close();
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Movement flag data not found");
            }

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
                        heightmap.SetFlag(pos, SFMapHeightMapFlag.FLAG_VISION, true);
                        //heightmap.chunk56_data.Add(pos);
                    }
                }
                c56.Close();
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Vision flag data not found");
            }

            SFChunkFileChunk c60 = f.GetChunkByID(60);
            if (c60 != null)
            {
                byte flags;
                SFCoord pos = new SFCoord();
                using (BinaryReader br = c60.Open())
                {
                    int pos_count = br.ReadInt32();
                    for (int i = 0; i < pos_count; i++)
                    {
                        flags = br.ReadByte();
                        pos.x = br.ReadInt16();
                        pos.y = br.ReadInt16();
                        if ((flags & 0x1) == 0x1)
                        {
                            heightmap.SetFlag(pos, SFMapHeightMapFlag.FLAG_MOVEMENT, true);
                        }
                        //heightmap.chunk42_data.Add(pos);
                        if ((flags & 0x2) == 0x2)
                        {
                            heightmap.SetFlag(pos, SFMapHeightMapFlag.FLAG_VISION, true);
                        }
                        //heightmap.chunk56_data.Add(pos);
                    }
                }
                c60.Close();
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Additional flag data not found");
            }

            // load weather
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading weather data");
            OnMapLoadStateChange.Invoke("Loading weather...", System.Drawing.Color.Black);

            weather_manager = new SFMapWeatherManager();
            SFChunkFileChunk c44 = f.GetChunkByID(44);
            if (c44 != null)
            {
                using (BinaryReader br = c44.Open())
                {
                    int weather_count = br.ReadByte();
                    for (int i = 0; i < weather_count; i++)
                    {
                        weather_manager.weather[i] = br.ReadByte();
                    }
                }
            }

            // load metadata
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading player spawn data");
            OnMapLoadStateChange.Invoke("Loading metadata...", System.Drawing.Color.Black);

            metadata = new SFMapMetaData();
            SFChunkFileChunk c55 = f.GetChunkByID(55);
            if (c55 != null)
            {
                using (BinaryReader br = c55.Open())
                {
                    int player_count = br.ReadInt32();
                    for (int i = 0; i < player_count; i++)
                    {
                        short x = br.ReadInt16();
                        short y = br.ReadInt16();
                        ushort text_id = br.ReadUInt16();
                        short unknown = br.ReadInt16();

                        // discard spawns which do not have bindstones at specified positions
                        SFCoord pos = new SFCoord(x, y);
                        int bindstone_index = Utility.NO_INDEX;
                        for (int j = 0; j < int_object_manager.bindstones_index.Count; j++)
                        {
                            if (int_object_manager.int_objects[int_object_manager.bindstones_index[j]].grid_position == pos)
                            {
                                bindstone_index = j;
                                break;
                            }
                        }

                        if (bindstone_index == Utility.NO_INDEX)
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
                        metadata.spawns[i].bindstone_index = bindstone_index;
                    }
                    metadata.player_count = player_count;
                }
                c55.Close();
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Player spawns loaded: " + metadata.player_count.ToString());
            }
            else
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Player spawn data not found!");
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading team compositions");
            metadata.multi_teams = new List<SFMapMultiplayerTeamComposition>();
            SFChunkFileChunk c53 = f.GetChunkByID(53);
            if (c53 != null)
            {
                using (BinaryReader br = c53.Open())
                {
                    // fix for maps that don't have as many spawns as bindstones
                    // coop 04,09 etc. bugfix: player count is not equivalent to bindstone count!
                    if (metadata.spawns.Count < int_object_manager.bindstones_index.Count)
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Not enough player spawns in map file! Filling in...");
                        // fill missing player spawns with unused bindstones
                        for (int i = metadata.spawns.Count; i < int_object_manager.bindstones_index.Count; i++)
                        {
                            for (int j = 0; j < int_object_manager.bindstones_index.Count; j++)
                            {
                                bool found = false;
                                for (int k = 0; k < i; k++)
                                {
                                    if (int_object_manager.int_objects[int_object_manager.bindstones_index[j]].grid_position == metadata.spawns[k].pos)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    metadata.spawns.Add(new SFMapSpawn());
                                    metadata.spawns[i].pos = int_object_manager.int_objects[int_object_manager.bindstones_index[j]].grid_position;
                                    metadata.spawns[i].text_id = 0;
                                    metadata.spawns[i].unknown = 0;
                                    metadata.spawns[i].bindstone_index = j;
                                    break;
                                }
                            }
                        }
                        metadata.player_count = int_object_manager.bindstones_index.Count;
                    }

                    if (c53.header.ChunkDataType == 2)
                    {
                        LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Found team composition data");
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

                                    int player = metadata.FindPlayerBySpawnPos(new SFCoord(x, y));

                                    tcomp.players[i].Add(new SFMapTeamPlayer(player, text_id));
                                }
                                p_num = br.ReadInt32();
                            }
                            metadata.multi_teams.Add(tcomp);
                            cur_teamcount += 1;
                        }
                    }
                    else if (c53.header.ChunkDataType == 4)
                    {
                        LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Found team composition data");
                        metadata.map_type = SFMapType.MULTIPLAYER;
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

                                    int player = metadata.FindPlayerBySpawnPos(new SFCoord(x, y));

                                    tcomp.players[i].Add(new SFMapTeamPlayer(player, text_id, s1, s2));

                                }
                                p_num = br.ReadInt32();
                            }
                            metadata.multi_teams.Add(tcomp);
                            cur_teamcount += 1;
                        }

                        // load minimap
                        int width = p_num;
                        int height = br.ReadInt32();
                        metadata.original_minimap = new SFMapMinimap();
                        metadata.original_minimap.width = width;
                        metadata.original_minimap.height = height;
                        metadata.original_minimap.data = br.ReadBytes(width * height * 3);
                    }
                    else
                    {
                        LogUtils.Log.Warning(LogUtils.LogSource.SFMap, "SFMap.Load(): Found team composition data, but could not load it!");
                    }
                }
                c53.Close();
            }
            else
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Team composition data not found, assuming campaign map type");
                metadata.map_type = SFMapType.CAMPAIGN;
            }

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Loading coop spawn parameters");
            metadata.coop_spawns = new List<SFMapCoopAISpawn>();
            SFChunkFileChunk c59 = f.GetChunkByID(59);
            if (c59 != null)
            {
                using (BinaryReader br = c59.Open())
                {
                    LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Found coop spawn parameters, assuming coop map type");
                    metadata.map_type = SFMapType.COOP;
                    for (int i = 0; i < 3; i++)
                    {
                        metadata.coop_spawn_params[i].param1 = br.ReadSingle();
                        metadata.coop_spawn_params[i].param2 = br.ReadSingle();
                        metadata.coop_spawn_params[i].param3 = br.ReadSingle();
                        metadata.coop_spawn_params[i].param4 = br.ReadSingle();
                    }
                }
                c59.Close();

                // load coop spawns
                if (c29 != null)
                {
                    using (BinaryReader br2 = c29.Open())
                    {
                        if (c29.header.ChunkDataType == 6)
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
                                {
                                    br2.ReadBytes(4);
                                }
                                else
                                {
                                    short spawn_id = br2.ReadInt16();
                                    short spawn_certain = br2.ReadInt16();
                                    metadata.coop_spawns.Add(new SFMapCoopAISpawn(
                                        object_manager.objects[obj_i],
                                        spawn_id,
                                        spawn_certain));

                                    // add mesh to the object
                                    SF3D.SceneSynchro.SceneNode obj_node = object_manager.objects[obj_i].node;

                                    string m = "editor_dummy_spawnpoint";
                                    SFRenderEngine.scene.AddSceneNodeSimple(obj_node, m, obj_node.Name + "_SPAWNCIRCLE");
                                }
                                if ((object_id >= 65) && (object_id <= 67))    // editor only
                                {
                                    obj_i--;
                                }

                                obj_i++;
                            }
                        }
                    }
                    c29.Close();
                }
            }
            else if (metadata.map_type == SFMapType.MULTIPLAYER)
            {
                LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Coop parameters not found, assuming multiplayer mode");
            }

            // terrain flags overlay
            heightmap.RebuildTerrainTexture(new SFCoord(0, 0), new SFCoord(width - 1, height - 1));

            // selection helper stuff
            selection_helper.AssignToMap(this);
            ocean.map = this;
            ocean.CreateOceanObject();

            FindPlatformID(filename);

            // done

            f.Close();

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Load(): Load successful!");
            OnMapLoadStateChange.Invoke("Map loaded", System.Drawing.Color.Black);

            return 0;
        }

        public int Save(string filename)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save() called, filename: " + filename);

            SFChunk.SFChunkFile f = new SFChunk.SFChunkFile();
            int res = f.CreateFile(filename, SFChunkFileType.MAP);
            if (res != 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.Save(): Failed to create map file (filename: " + filename + ")");
                return res;
            }

            int data_size;

            // chunk 2
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving tile data");
            data_size = 3 + heightmap.width * heightmap.height;
            byte[] tiledata = new byte[heightmap.width * heightmap.height];
            for(int y = 0; y < heightmap.height; y++)
            {
                for(int x = 0; x < heightmap.width; x++)
                {
                    tiledata[y * heightmap.width + x] = heightmap.GetTile(new SFCoord(x, y));
                }
            }
            byte[] c2_data = new byte[data_size];
            using (BinaryWriter bw = new BinaryWriter(new MemoryStream(c2_data)))
            {
                bw.Write((short)heightmap.height);
                bw.Write((byte)0);
                //bw.Write(heightmap.tile_data);
                bw.Write(tiledata);
            }
            f.AddChunk(2, 0, true, 6, c2_data);

            // chunks 6
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving heightmap");
            ushort[] c6i_data = new ushort[heightmap.height];
            byte[] c6i_rawdata = new byte[heightmap.height * 2];
            for (int i = 0; i < heightmap.width; i++)
            {
                heightmap.GetRowRaw(i, ref c6i_data);
                Buffer.BlockCopy(c6i_data, 0, c6i_rawdata, 0, c6i_rawdata.Length);
                f.AddChunk(6, (short)i, true, 6, c6i_rawdata);
            }

            // chunk 3
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving tile definitions");
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
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving textures");
            byte[] c4_data = new byte[63];
            for (int i = 0; i < 63; i++)
            {
                c4_data[i] = (byte)heightmap.texture_manager.texture_id[i];
            }
            f.AddChunk(4, 0, true, 4, c4_data);

            // preparing for chunks 42 and 56
            List<SFCoord> flags_movement = new List<SFCoord>();
            List<SFCoord> flags_vision = new List<SFCoord>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SFCoord pos = new SFCoord(x, y);
                    if (heightmap.IsFlagSet(pos, SFMapHeightMapFlag.FLAG_MOVEMENT))
                    {
                        flags_movement.Add(pos);
                    }

                    if (heightmap.IsFlagSet(pos, SFMapHeightMapFlag.FLAG_VISION))
                    {
                        flags_vision.Add(pos);
                    }
                }
            }

            // chunk 42
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving map flags");

            byte[] c42_data = new byte[flags_movement.Count * 4];
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Movement flag count: " + flags_movement.Count.ToString());

            using (MemoryStream ms = new MemoryStream(c42_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < flags_movement.Count; i++)
                    {
                        bw.Write((short)flags_movement[i].x);
                        bw.Write((short)flags_movement[i].y);
                    }
                }
            }
            f.AddChunk(42, 0, true, 1, c42_data);

            byte[] c56_data = new byte[flags_vision.Count * 4];
            using (MemoryStream ms = new MemoryStream(c56_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < flags_vision.Count; i++)
                    {
                        bw.Write((short)flags_vision[i].x);
                        bw.Write((short)flags_vision[i].y);
                    }
                }
            }
            f.AddChunk(56, 0, true, 1, c56_data);

            // chunk 40
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving lakes");
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
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving decal data");
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
                        {
                            bw.Write((short)decoration_manager.dec_groups[i].dec_id[j]);
                        }

                        for (int j = 0; j < 30; j++)
                        {
                            bw.Write((byte)decoration_manager.dec_groups[i].weight[j]);
                        }
                    }
                }
            }
            f.AddChunk(32, 0, true, 1, c32_data);

            // chunk 29
            // FIX for flags eating objects
            HashSet<SFCoord> obj_positions = new HashSet<SFCoord>();
            for (int i = 0; i < object_manager.objects.Count; i++)
            {
                obj_positions.Add(object_manager.objects[i].grid_position);
            }

            for (int i = 0; i < int_object_manager.int_objects.Count; i++)
            {
                obj_positions.Add(int_object_manager.int_objects[i].grid_position);
            }

            for (int i = 0; i < unit_manager.units.Count; i++)
            {
                obj_positions.Add(unit_manager.units[i].grid_position);
            }

            for (int i = 0; i < building_manager.buildings.Count; i++)
            {
                obj_positions.Add(building_manager.buildings[i].grid_position);
            }

            for (int i = 0; i < portal_manager.portals.Count; i++)
            {
                obj_positions.Add(portal_manager.portals[i].grid_position);
            }

            // FIX for maps being broken when opened in original editor: reintroduce flag objects
            //HashSet<SFCoord> merged_flags = new HashSet<SFCoord>(heightmap.chunk42_data.Intersect(heightmap.chunk56_data));
            HashSet<SFCoord> merged_flags = new HashSet<SFCoord>(flags_movement.Intersect(flags_vision));

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving objects");
            byte[] c29_data;// = new byte[object_manager.objects.Count * 16];
            using (MemoryStream ms = new MemoryStream(32768 * 16))     // 16384 is max number of objects, doubling that in case there are more flags than anticipated
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
                        {
                            bw.Write((int)0);
                        }
                        else
                        {
                            SFMapCoopAISpawn coop_spawn = new SFMapCoopAISpawn();
                            if (!metadata.GetCoopAISpawnByObject(object_manager.objects[i], ref coop_spawn))
                            {
                                bw.Write((int)0);
                            }
                            else
                            {
                                bw.Write((short)coop_spawn.spawn_id);
                                bw.Write((short)coop_spawn.spawn_certain);
                            }
                        }
                    }
                    // flags type 65
                    foreach (SFCoord p in flags_movement)//heightmap.chunk42_data)
                    {
                        if (!merged_flags.Contains(p))
                        {
                            if (obj_positions.Contains(p))
                            {
                                continue;
                            }

                            bw.Write((short)p.x);
                            bw.Write((short)p.y);
                            bw.Write((short)65);
                            bw.Write((short)0);
                            bw.Write((short)0);
                            bw.Write((short)0);
                            bw.Write((int)0);
                        }
                    }
                    // flags type 66
                    foreach (SFCoord p in flags_vision)//heightmap.chunk56_data)
                    {
                        if (!merged_flags.Contains(p))
                        {
                            if (obj_positions.Contains(p))
                            {
                                continue;
                            }

                            bw.Write((short)p.x);
                            bw.Write((short)p.y);
                            bw.Write((short)66);
                            bw.Write((short)0);
                            bw.Write((short)0);
                            bw.Write((short)0);
                            bw.Write((int)0);
                        }
                    }
                    // flags type 67
                    foreach (SFCoord p in merged_flags)
                    {
                        if (obj_positions.Contains(p))
                        {
                            continue;
                        }

                        bw.Write((short)p.x);
                        bw.Write((short)p.y);
                        bw.Write((short)67);
                        bw.Write((short)0);
                        bw.Write((short)0);
                        bw.Write((short)0);
                        bw.Write((int)0);
                    }
                }
                c29_data = ms.ToArray();
            }
            f.AddChunk(29, 0, true, 6, c29_data);

            // chunk 35
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving portals");
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
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving interactive objects");
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
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving buildings");
            short bld_chunk_type = 3;
            //if (metadata.map_type == SFMapType.COOP)    // testing fix
            //    bld_chunk_type = 2;

            byte[] c11_data = new byte[building_manager.buildings.Count * (bld_chunk_type == 2 ? 10 : 11)];
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
                        if (bld_chunk_type > 2)
                        {
                            bw.Write((byte)building_manager.buildings[i].race_id);
                        }
                    }
                }
            }
            f.AddChunk(11, 0, true, bld_chunk_type, c11_data);

            // chunk 12
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving units");
            byte[] c12_data = new byte[unit_manager.units.Count * 14];
            using (MemoryStream ms = new MemoryStream(c12_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < unit_manager.units.Count; i++)
                    {
                        bw.Write((short)unit_manager.units[i].grid_position.x);
                        bw.Write((short)unit_manager.units[i].grid_position.y);
                        bw.Write((short)unit_manager.units[i].unknown_flags);
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
            // chunk 44
            weather_manager.Normalize();
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving weather");
            byte[] c44_data = new byte[weather_manager.weather.Length + 1];
            using (MemoryStream ms = new MemoryStream(c44_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write((byte)weather_manager.weather.Length);
                    for (int i = 0; i < weather_manager.weather.Length; i++)
                    {
                        bw.Write(weather_manager.weather[i]);
                    }
                }
            }
            f.AddChunk(44, 0, true, 1, c44_data);

            // chunk 46
            byte m_group = (byte)Math.Min(254, unit_manager.GetHighestGroup());
            byte[] c46_data = new byte[m_group + 2];
            c46_data[0] = (byte)(m_group + 1);
            for (byte i = 0; i <= m_group; i++)
            {
                c46_data[i + 1] = i;
            }

            f.AddChunk(46, 0, true, 1, c46_data);

            // chunk 53
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving team compositions");
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
                SFMapMinimap mmap = null;
                switch (metadata.minimap_source)
                {
                    case SFMapMinimapSource.ORIGINAL:
                        mmap = metadata.original_minimap;
                        break;
                    case SFMapMinimapSource.EDITOR:
                        mmap = new SFMapMinimap();
                        mmap.FromBitmap(metadata.new_minimap.ToBitmap());
                        break;
                    case SFMapMinimapSource.CUSTOM:
                        mmap = metadata.custom_minimap;
                        break;
                }
                if (mmap == null)
                {
                    mmap = metadata.original_minimap;
                }

                byte[] c53_data = new byte[team_array.Length + 8 + (mmap.width * mmap.height * 3)];
                using (MemoryStream ms = new MemoryStream(c53_data))
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(team_array);
                        bw.Write(mmap.width);
                        bw.Write(mmap.height);
                        bw.Write(mmap.data);
                    }
                }
                f.AddChunk(53, 0, true, chunk_type, c53_data);
            }

            // chunk 55
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving player spawn data");
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
            // SKIPPED

            // chunk 8000 not used?

            // chunk 59
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Save(): Saving coop spawn parameters");
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

        public int CreateDefault(ushort size, MapGen.MapGenerator generator)
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.CreateDefault() called, map size: " + size.ToString());

            // load map size and tile indices
            OnMapLoadStateChange.Invoke("Creating map data...", System.Drawing.Color.Black);

            byte[] tilearray = new byte[size * size];
            for (int i = 0; i < size * size; i++)
            {
                tilearray[i] = 0;
            }

            width = size;
            height = size;
            heightmap = new SFMapHeightMap(width, height) { map = this };
            heightmap.SetTilesRaw(tilearray);

            // load terrain texture data
            byte[] moveflags = new byte[] {0, 0, 0, 0, 0, 0, 1,
                                           1, 1, 1, 0, 1, 0, 0, 0,
                                           0, 1, 0, 1, 0, 0, 0, 0,
                                           0, 0, 0, 0, 1, 0, 1, 1};

            byte[] tile_data = new byte[255 * 14];
            tile_data.Initialize();
            for (byte i = 0; i < 255; i++)
            {
                // byte 0
                if (i == 0)
                {
                    tile_data[i * 14 + 0] = 1;
                }

                if ((i >= 1) && (i <= 31))
                {
                    tile_data[i * 14 + 0] = i;
                }

                if (i >= 224)
                {
                    tile_data[i * 14 + 0] = (byte)(i - 192);
                }
                // byte 3
                if ((i <= 31) || (i >= 224))
                {
                    tile_data[i * 14 + 3] = 255;
                }
                // byte 6
                tile_data[i * 14 + 6] = i;
                if ((i >= 1) && (i <= 31))
                {
                    tile_data[i * 14 + 6] = (byte)(i + 223);
                }
                // byte 7
                tile_data[i * 14 + 7] = i;
                if (i >= 224)
                {
                    tile_data[i * 14 + 7] = (byte)(i - 223);
                }
                // bytes 8-9
                tile_data[i * 14 + 8] = 255;
                tile_data[i * 14 + 9] = 128;
                // byte 10
                if (((i >= 1) && (i <= 120)) || (i >= 224))
                {
                    tile_data[i * 14 + 10] = 10;
                }
                // byte 11
                tile_data[i * 14 + 11] = 255;
                // byte 12: movement flag, byte 13: vision flag - both set later
                if ((i >= 1) && (i <= 31))
                {
                    tile_data[i * 14 + 12] = moveflags[i - 1];
                }

                if (i >= 224)
                {
                    tile_data[i * 14 + 12] = moveflags[i - 224];
                }
            }


            for (int i = 0; i < 255; i++)
            {
                heightmap.texture_manager.texture_tiledata[i].ind1 = tile_data[i * 14 + 0];
                heightmap.texture_manager.texture_tiledata[i].ind2 = tile_data[i * 14 + 1];
                heightmap.texture_manager.texture_tiledata[i].ind3 = tile_data[i * 14 + 2];
                heightmap.texture_manager.texture_tiledata[i].weight1 = tile_data[i * 14 + 3];
                heightmap.texture_manager.texture_tiledata[i].weight2 = tile_data[i * 14 + 4];
                heightmap.texture_manager.texture_tiledata[i].weight3 = tile_data[i * 14 + 5];
                heightmap.texture_manager.texture_tiledata[i].reindex_data = tile_data[i * 14 + 6];
                heightmap.texture_manager.texture_tiledata[i].reindex_index = tile_data[i * 14 + 7];
                heightmap.texture_manager.texture_tiledata[i].material_property = tile_data[i * 14 + 10];
                byte b_m = tile_data[i * 14 + 12]; byte b_v = tile_data[i * 14 + 13];
                heightmap.texture_manager.texture_tiledata[i].blocks_movement = ((b_m % 2) == 1 ? true : false);
                heightmap.texture_manager.texture_tiledata[i].blocks_vision = ((b_v % 2) == 1 ? true : false);
            }


            byte[] texture_ids = new byte[63];
            texture_ids[0] = 0;
            texture_ids[1] = 69; texture_ids[2] = 66; texture_ids[3] = 67; texture_ids[4] = 70;
            texture_ids[5] = 59; texture_ids[6] = 6; texture_ids[7] = 23; texture_ids[8] = 18;
            texture_ids[9] = 9; texture_ids[10] = 10; texture_ids[11] = 93; texture_ids[12] = 12;
            texture_ids[13] = 13; texture_ids[14] = 17; texture_ids[15] = 15; texture_ids[16] = 22;
            texture_ids[17] = 33; texture_ids[18] = 54; texture_ids[19] = 29; texture_ids[20] = 68;
            texture_ids[21] = 5; texture_ids[22] = 73; texture_ids[23] = 75; texture_ids[24] = 77;
            texture_ids[25] = 64; texture_ids[26] = 26; texture_ids[27] = 80; texture_ids[28] = 74;
            texture_ids[29] = 30; texture_ids[30] = 83; texture_ids[31] = 32;
            for (int i = 1; i < 32; i++)
            {
                texture_ids[i + 31] = (byte)(texture_ids[i] + SFMapTerrainTextureManager.TEXTURES_AVAILABLE);
            }

            heightmap.texture_manager.SetTextureIDsRaw(texture_ids);

            // generate texture data
            heightmap.texture_manager.Init();

            // load heightmap
            if (generator != null)
            {
                heightmap.height_data = generator.ProduceHeightmap();
            }
            else
            {
                for (int i = 0; i < size * size; i++)
                {
                    heightmap.height_data[i] = 0;
                }
            }

            // generate heightmap models and reindex textures
            heightmap.Generate();

            // load buildings
            building_manager = new SFMapBuildingManager() { map = this };

            // load units
            unit_manager = new SFMapUnitManager() { map = this };

            // load objects
            object_manager = new SFMapObjectManager() { map = this };

            // load interactive objects
            int_object_manager = new SFMapInteractiveObjectManager() { map = this };

            // load decorations

            byte[] dec_data = new byte[1048576];
            dec_data.Initialize();
            decoration_manager = new SFMapDecorationManager() { map = this };
            decoration_manager.dec_assignment = dec_data;

            for (int i = 0; i < 255; i++)
            {
                decoration_manager.dec_groups[i] = new SFMapDecorationGroup();
            }

            decoration_manager.GenerateDecorations();

            // load portals
            portal_manager = new SFMapPortalManager() { map = this };

            // load lakes
            lake_manager = new SFMapLakeManager() { map = this };

            // load weather

            weather_manager = new SFMapWeatherManager();

            // load map flags

            // load metadata
            metadata = new SFMapMetaData { map_type = SFMapType.COOP };
            metadata.multi_teams = new List<SFMapMultiplayerTeamComposition>();
            metadata.coop_spawns = new List<SFMapCoopAISpawn>();

            byte[] image_data = new byte[128 * 128 * 3];
            for (int i = 0; i < 128 * 128 * 3; i++)
            {
                image_data[i] = (byte)((i * 1024) / 3);
            }

            metadata.original_minimap = new SFMapMinimap();
            metadata.original_minimap.width = 128;
            metadata.original_minimap.height = 128;
            metadata.original_minimap.data = image_data;

            // flag overlay
            heightmap.RebuildTerrainTexture(new SFCoord(0, 0), new SFCoord(width - 1, height - 1));

            // selection helper stuff
            selection_helper.AssignToMap(this);
            ocean.map = this;
            ocean.CreateOceanObject();

            // done

            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Create() finished successfully");
            OnMapLoadStateChange.Invoke("Map created", System.Drawing.Color.Black);

            return 0;
        }

        public void Unload()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMap.Unload() called");
            if (heightmap != null)
            {
                heightmap.Unload();
            }

            if (metadata != null)
            {
                metadata.Unload();             // minimap texture
            }

            if (selection_helper != null)
            {
                selection_helper.Dispose();    // selection 3d mesh
            }

            if (ocean != null)
            {
                ocean.Dispose();
            }

            if (lake_manager != null)
            {
                lake_manager.Dispose();
            }

            if (building_manager != null)
            {
                building_manager.Dispose();
            }

            building_manager = null;
            unit_manager = null;
            object_manager = null;
            int_object_manager = null;
            decoration_manager = null;
            portal_manager = null;
            lake_manager = null;
            heightmap = null;
            metadata = null;
            selection_helper = null;
            ocean = null;
            //npc_manager = null;
        }

        // helper function for decals
        // center corresponds to world coordinate for the center (SFCOORD of the object)
        public void UpdateNodeDecal(SF3D.SceneSynchro.SceneNode node, OpenTK.Vector2 center, OpenTK.Vector2 offset, int angle)
        {
            if (heightmap == null)
            {
                return;
            }
            // assumption: a node can only have one subnode that is a decal
            foreach (SF3D.SceneSynchro.SceneNodeSimple n in node.Children)
            {
                if (n.IsDecal)
                {
                    SF3D.SFModel3D m_old = n.Mesh;
                    if (m_old == null)
                    {
                        return;
                    }

                    OpenTK.Vector2 bb_topleft = m_old.aabb.a.Xy;
                    OpenTK.Vector2 bb_bottomright = m_old.aabb.b.Xy;

                    SF3D.SFModel3D m_new = new SF3D.SFModel3D();
                    SF3D.SFSubModel3D sbm_new = new SF3D.SFSubModel3D();
                    SF3D.SFMaterial sfm_new = m_old.submodels[0].material;

                    OpenTK.Vector3[] vertices;
                    OpenTK.Vector3[] normals;
                    OpenTK.Vector2[] uvs;
                    byte[] colors;
                    uint[] indices;

                    SFCoord map_bb_topleft;
                    SFCoord map_bb_bottomright;

                    heightmap.GenerateDecalGeometry(bb_topleft, bb_bottomright, center, offset, (float)(angle * Math.PI / 180),
                        out vertices, out normals, out uvs, out indices, out map_bb_topleft, out map_bb_bottomright);
                    colors = new byte[vertices.Length * 4];
                    for (int i = 0; i < colors.Length; i += 4)
                    {
                        colors[i + 0] = 255;
                        colors[i + 1] = 255;
                        colors[i + 2] = 255;
                        colors[i + 3] = 255;
                    }

                    sbm_new.CreateRaw(vertices, uvs, colors, normals, indices, sfm_new);
                    m_new.aabb = m_old.aabb;
                    m_new.CreateRaw(new SF3D.SFSubModel3D[] { sbm_new }, true);

                    n.Mesh = null;
                    SFResources.SFResourceManager.Models.Dispose(m_old.Name);
                    SFResources.SFResourceManager.Models.AddManually(m_new, "_DECAL_" + node.Name + "_" + n.Name);
                    n.Mesh = m_new;
                    n.Rotation = node.rotation.Inverted();
                    n.Scale = new OpenTK.Vector3(1.28f);

                    SF3D.SceneSynchro.SFDecalInfo decal_info = SFRenderEngine.scene.decal_info.elements[n.DecalIndex];
                    decal_info.topleft = map_bb_topleft;
                    decal_info.bottomright = map_bb_bottomright;
                    decal_info.center = center;
                    decal_info.offset = offset;
                    decal_info.angle = angle;

                    return;
                }
            }
        }

        public void AddObject(int game_id, SFCoord pos, int angle, int npc_id, int unk1, int index = -1)
        {
            SFMapObject obj = object_manager.AddObject(game_id, pos, angle, unk1, index);
            heightmap.SetFlag(pos, SFMapHeightMapFlag.ENTITY_OBJECT, true);
            object_manager.ApplyObjectBlockFlags(obj.grid_position, obj.angle, (ushort)obj.game_id, true);

            obj.npc_id = npc_id;

            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.SceneSynchro.SceneNode _obj = obj.node;
            _obj.Position = heightmap.GetFixedPosition(pos);
            _obj.Scale = new OpenTK.Vector3(100 / 128.0f);
            _obj.SetAnglePlane(angle);
            UpdateNodeDecal(_obj, new OpenTK.Vector2(pos.x, pos.y), OpenTK.Vector2.Zero, angle);

            heightmap.GetChunk(pos).AddObject(obj);
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
            heightmap.SetFlag(obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, false);
            object_manager.ApplyObjectBlockFlags(obj.grid_position, obj.angle, (ushort)obj.game_id, false);

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


            if (obj.node != null)
            {
                SFRenderEngine.scene.RemoveSceneNode(obj.node);
            }

            obj.node = SFRenderEngine.scene.AddSceneObject(new_object_id, obj.GetName(), true, true);
            obj.node.SetParent(heightmap.GetChunkNode(obj.grid_position));

            object_manager.AddObjectCollisionBoundary(new_object_id);

            object_manager.ApplyObjectBlockFlags(obj.grid_position, obj.angle, (ushort)obj.game_id, false);
            obj.game_id = new_object_id;
            object_manager.ApplyObjectBlockFlags(obj.grid_position, obj.angle, (ushort)obj.game_id, true);

            // object transform
            float z = heightmap.GetZ(obj.grid_position) / 100.0f;
            obj.node.Position = heightmap.GetFixedPosition(obj.grid_position);
            obj.node.Scale = new OpenTK.Vector3(100 / 128f);
            obj.node.SetAnglePlane(obj.angle);
            UpdateNodeDecal(obj.node, new OpenTK.Vector2(obj.grid_position.x, obj.grid_position.y), OpenTK.Vector2.Zero, obj.angle);

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

            object_manager.ApplyObjectBlockFlags(obj.grid_position, obj.angle, (ushort)obj.game_id, false);
            obj.angle = angle;
            object_manager.ApplyObjectBlockFlags(obj.grid_position, obj.angle, (ushort)obj.game_id, true);

            obj.node.SetAnglePlane(angle);
            UpdateNodeDecal(obj.node, new OpenTK.Vector2(obj.grid_position.x, obj.grid_position.y), OpenTK.Vector2.Zero, obj.angle);

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

            // move unit and set chunk dependency
            heightmap.GetChunkNode(obj.grid_position).MapChunk.objects.Remove(obj);
            heightmap.SetFlag(obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, false);
            object_manager.ApplyObjectBlockFlags(obj.grid_position, obj.angle, (ushort)obj.game_id, false);
            obj.grid_position = new_pos;
            object_manager.ApplyObjectBlockFlags(obj.grid_position, obj.angle, (ushort)obj.game_id, true);
            heightmap.SetFlag(obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, true);
            heightmap.GetChunkNode(obj.grid_position).MapChunk.objects.Add(obj);
            obj.node.SetParent(heightmap.GetChunkNode(obj.grid_position));

            // change visual transform
            float z = heightmap.GetZ(new_pos) / 100.0f;
            obj.node.Position = heightmap.GetFixedPosition(new_pos);
            UpdateNodeDecal(obj.node, new OpenTK.Vector2(obj.grid_position.x, obj.grid_position.y), OpenTK.Vector2.Zero, obj.angle);

            return 0;
        }

        public void AddInteractiveObject(int game_id, SFCoord pos, int angle, int unk_byte, int index = -1)
        {
            SFMapInteractiveObject obj = int_object_manager.AddInteractiveObject(game_id, pos, angle, unk_byte, index);
            heightmap.SetFlag(obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, true);
            object_manager.ApplyObjectBlockFlags(obj.grid_position, obj.angle, (ushort)obj.game_id, true);

            float z = heightmap.GetZ(pos) / 100.0f;

            SF3D.SceneSynchro.SceneNode _obj = obj.node;
            _obj.Position = heightmap.GetFixedPosition(pos);
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);
            UpdateNodeDecal(_obj, new OpenTK.Vector2(pos.x, pos.y), OpenTK.Vector2.Zero, angle);

            heightmap.GetChunk(pos).AddInteractiveObject(obj);
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

            // move unit and set chunk dependency
            heightmap.GetChunkNode(int_obj.grid_position).MapChunk.int_objects.Remove(int_obj);

            object_manager.ApplyObjectBlockFlags(int_obj.grid_position, int_obj.angle, (ushort)int_obj.game_id, false);
            heightmap.SetFlag(int_obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, false);
            int_obj.grid_position = new_pos;
            heightmap.SetFlag(int_obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, true);
            object_manager.ApplyObjectBlockFlags(int_obj.grid_position, int_obj.angle, (ushort)int_obj.game_id, true);

            heightmap.GetChunkNode(int_obj.grid_position).MapChunk.int_objects.Add(int_obj);
            int_obj.node.SetParent(heightmap.GetChunkNode(int_obj.grid_position));

            // change visual transform
            float z = heightmap.GetZ(new_pos) / 100.0f;
            int_obj.node.Position = heightmap.GetFixedPosition(new_pos);
            UpdateNodeDecal(int_obj.node, new OpenTK.Vector2(int_obj.grid_position.x, int_obj.grid_position.y), OpenTK.Vector2.Zero, int_obj.angle);

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
            heightmap.SetFlag(int_obj.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, false);
            object_manager.ApplyObjectBlockFlags(int_obj.grid_position, int_obj.angle, (ushort)int_obj.game_id, true);

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

            object_manager.ApplyObjectBlockFlags(int_obj.grid_position, int_obj.angle, (ushort)int_obj.game_id, false);
            int_obj.angle = angle;
            object_manager.ApplyObjectBlockFlags(int_obj.grid_position, int_obj.angle, (ushort)int_obj.game_id, true);

            SF3D.SceneSynchro.SceneNode _obj = int_obj.node;
            _obj.SetAnglePlane(angle);
            UpdateNodeDecal(int_obj.node, new OpenTK.Vector2(int_obj.grid_position.x, int_obj.grid_position.y), OpenTK.Vector2.Zero, int_obj.angle);

            return 0;
        }

        public int ReplaceMonument(int monument_index, int new_monument_type)
        {
            if ((new_monument_type < 0) || (new_monument_type > 6))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.ReplaceMonument(): invalid monument type! Monument type = " + new_monument_type.ToString());
                return -1;
            }

            if ((monument_index < 0) || (monument_index >= int_object_manager.monuments_index.Count))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.ReplaceMonument(): invalid monument index! Monument index = " + monument_index.ToString());
                return -2;
            }

            SFMapInteractiveObject io = int_object_manager.int_objects[int_object_manager.monuments_index[monument_index]];

            if (io.node != null)
            {
                SFRenderEngine.scene.RemoveSceneNode(io.node);
            }

            io.node = SFRenderEngine.scene.AddSceneObject(new_monument_type + 771, io.GetName(), true, true);
            io.node.SetParent(heightmap.GetChunkNode(io.grid_position));

            object_manager.ApplyObjectBlockFlags(io.grid_position, io.angle, (ushort)io.game_id, false);
            io.game_id = new_monument_type + 771;
            object_manager.ApplyObjectBlockFlags(io.grid_position, io.angle, (ushort)io.game_id, true);

            float z = heightmap.GetZ(io.grid_position) / 100.0f;
            io.node.Position = heightmap.GetFixedPosition(io.grid_position);
            io.node.Scale = new OpenTK.Vector3(100 / 128f);
            io.node.SetAnglePlane(io.angle);
            UpdateNodeDecal(io.node, new OpenTK.Vector2(io.grid_position.x, io.grid_position.y), OpenTK.Vector2.Zero, io.angle);

            return 0;
        }

        public void AddBuilding(int game_id, SFCoord pos, int angle, int npc_id, int lvl, int race_id, int index = -1)
        {
            SFMapBuilding bld = building_manager.AddBuilding(game_id, pos, angle, npc_id, lvl, race_id, index);
            heightmap.SetFlag(bld.grid_position, SFMapHeightMapFlag.ENTITY_BUILDING, true);
            building_manager.ApplyBuildingBlockFlags(bld, true);

            OpenTK.Vector2 b_offset = building_manager.building_collision[(ushort)bld.game_id].origin;
            float angle_rad = (float)(angle * Math.PI / 180);
            OpenTK.Vector2 b_offset_rotated = new OpenTK.Vector2(b_offset.X, b_offset.Y);
            b_offset_rotated.X = (float)((Math.Cos(angle_rad) * b_offset.X) - (Math.Sin(angle_rad) * b_offset.Y));
            b_offset_rotated.Y = (float)((Math.Sin(angle_rad) * b_offset.X) + (Math.Cos(angle_rad) * b_offset.Y));

            bld.node.Position = heightmap.GetFixedPosition(pos) + new OpenTK.Vector3(-b_offset_rotated.X, 0, b_offset_rotated.Y);
            bld.node.Scale = new OpenTK.Vector3(100 / 128f);
            bld.node.SetAnglePlane(angle);
            UpdateNodeDecal(bld.node, new OpenTK.Vector2(pos.x, pos.y), b_offset, angle);

            heightmap.GetChunk(pos).AddBuilding(bld);
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
            heightmap.SetFlag(building.grid_position, SFMapHeightMapFlag.ENTITY_BUILDING, false);
            building_manager.ApplyBuildingBlockFlags(building, false);

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

            building_manager.ApplyBuildingBlockFlags(building, false);
            if (building.node != null)
            {
                SFRenderEngine.scene.RemoveSceneNode(building.node);
            }

            building_manager.AddBuildingCollisionBoundary(new_building_id);
            building.node = SFRenderEngine.scene.AddSceneBuilding(new_building_id, building.GetName());
            building.node.SetParent(heightmap.GetChunkNode(building.grid_position));

            building.game_id = new_building_id;
            building_manager.ApplyBuildingBlockFlags(building, true);

            SFCoord pos = building.grid_position;
            float z = heightmap.GetZ(pos) / 100.0f;

            OpenTK.Vector2 b_offset = building_manager.building_collision[(ushort)building.game_id].origin;
            float angle_rad = (float)(building.angle * Math.PI / 180);
            OpenTK.Vector2 b_offset_rotated = new OpenTK.Vector2(b_offset.X, b_offset.Y);
            b_offset_rotated.X = (float)((Math.Cos(angle_rad) * b_offset.X) - (Math.Sin(angle_rad) * b_offset.Y));
            b_offset_rotated.Y = (float)((Math.Sin(angle_rad) * b_offset.X) + (Math.Cos(angle_rad) * b_offset.Y));

            building.node.Position = heightmap.GetFixedPosition(pos) + new OpenTK.Vector3(b_offset_rotated.X, 0, +b_offset_rotated.Y);
            building.node.Scale = new OpenTK.Vector3(100 / 128f);
            building.node.SetAnglePlane(building.angle);
            UpdateNodeDecal(building.node, new OpenTK.Vector2(building.grid_position.x, building.grid_position.y), b_offset, building.angle);

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

            building_manager.ApplyBuildingBlockFlags(building, false);
            building.angle = angle;
            building_manager.ApplyBuildingBlockFlags(building, true);

            OpenTK.Vector2 b_offset = building_manager.building_collision[(ushort)building.game_id].origin;
            float angle_rad = (float)(building.angle * Math.PI / 180);
            OpenTK.Vector2 b_offset_rotated = new OpenTK.Vector2(b_offset.X, b_offset.Y);
            b_offset_rotated.X = (float)((Math.Cos(angle_rad) * b_offset.X) - (Math.Sin(angle_rad) * b_offset.Y));
            b_offset_rotated.Y = (float)((Math.Sin(angle_rad) * b_offset.X) + (Math.Cos(angle_rad) * b_offset.Y));

            building.node.Position = heightmap.GetFixedPosition(building.grid_position) + new OpenTK.Vector3(-b_offset_rotated.X, 0, +b_offset_rotated.Y);
            building.node.Scale = new OpenTK.Vector3(100 / 128f);
            building.node.SetAnglePlane(building.angle);
            UpdateNodeDecal(building.node, new OpenTK.Vector2(building.grid_position.x, building.grid_position.y), b_offset, building.angle);

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

            // move unit and set chunk dependency
            heightmap.GetChunkNode(building.grid_position).MapChunk.buildings.Remove(building);
            building_manager.ApplyBuildingBlockFlags(building, false);
            heightmap.SetFlag(building.grid_position, SFMapHeightMapFlag.ENTITY_BUILDING, false);
            building.grid_position = new_pos;
            heightmap.SetFlag(building.grid_position, SFMapHeightMapFlag.ENTITY_BUILDING, true);
            building_manager.ApplyBuildingBlockFlags(building, true);
            heightmap.GetChunkNode(building.grid_position).MapChunk.buildings.Add(building);
            building.node.SetParent(heightmap.GetChunkNode(building.grid_position));

            // change visual transform
            float z = heightmap.GetZ(new_pos) / 100.0f;

            OpenTK.Vector2 b_offset = building_manager.building_collision[(ushort)building.game_id].origin;
            float angle_rad = (float)(building.angle * Math.PI / 180);
            OpenTK.Vector2 b_offset_rotated = new OpenTK.Vector2(b_offset.X, b_offset.Y);
            b_offset_rotated.X = (float)((Math.Cos(angle_rad) * b_offset.X) - (Math.Sin(angle_rad) * b_offset.Y));
            b_offset_rotated.Y = (float)((Math.Sin(angle_rad) * b_offset.X) + (Math.Cos(angle_rad) * b_offset.Y));

            building.node.Position = heightmap.GetFixedPosition(new_pos) + new OpenTK.Vector3(-b_offset_rotated.X, 0, +b_offset_rotated.Y);
            building.node.Scale = new OpenTK.Vector3(100 / 128f);
            building.node.SetAnglePlane(building.angle);
            UpdateNodeDecal(building.node, new OpenTK.Vector2(building.grid_position.x, building.grid_position.y), b_offset, building.angle);

            return 0;
        }

        public void AddPortal(int game_id, SFCoord pos, int angle, int index = -1)
        {
            SFMapPortal ptl = portal_manager.AddPortal(game_id, pos, angle, index);
            heightmap.SetFlag(ptl.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, true);
            object_manager.ApplyObjectBlockFlags(ptl.grid_position, ptl.angle, 778, true);

            ptl.node.Position = heightmap.GetFixedPosition(pos);
            ptl.node.Scale = new OpenTK.Vector3(100 / 128f);
            ptl.node.SetAnglePlane(angle);
            UpdateNodeDecal(ptl.node, new OpenTK.Vector2(pos.x, pos.y), OpenTK.Vector2.Zero, angle);

            heightmap.GetChunk(pos).AddPortal(ptl);
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
            heightmap.SetFlag(portal.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, false);
            object_manager.ApplyObjectBlockFlags(portal.grid_position, portal.angle, 778, false);

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

            // move unit and set chunk dependency
            heightmap.GetChunkNode(portal.grid_position).MapChunk.portals.Remove(portal);
            object_manager.ApplyObjectBlockFlags(portal.grid_position, portal.angle, 778, false);
            heightmap.SetFlag(portal.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, false);
            portal.grid_position = new_pos;
            heightmap.SetFlag(portal.grid_position, SFMapHeightMapFlag.ENTITY_OBJECT, true);
            object_manager.ApplyObjectBlockFlags(portal.grid_position, portal.angle, 778, true);
            heightmap.GetChunkNode(portal.grid_position).MapChunk.portals.Add(portal);
            portal.node.SetParent(heightmap.GetChunkNode(portal.grid_position));

            // change visual transform
            portal.node.Position = heightmap.GetFixedPosition(new_pos);
            UpdateNodeDecal(portal.node, new OpenTK.Vector2(portal.grid_position.x, portal.grid_position.y), OpenTK.Vector2.Zero, portal.angle);

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

            object_manager.ApplyObjectBlockFlags(portal.grid_position, portal.angle, 778, false);
            portal.angle = angle;
            object_manager.ApplyObjectBlockFlags(portal.grid_position, portal.angle, 778, true);

            portal.node.SetAnglePlane(angle);
            UpdateNodeDecal(portal.node, new OpenTK.Vector2(portal.grid_position.x, portal.grid_position.y), OpenTK.Vector2.Zero, portal.angle);

            return 0;
        }

        public void AddUnit(int game_id, SFCoord pos, int flags, int npc_id, int unknown, int group, int unknown2, int index = -1)
        {
            // 1. add new unit in unit manager
            SFMapUnit unit = unit_manager.AddUnit(game_id, pos, flags, index);
            heightmap.SetFlag(unit.grid_position, SFMapHeightMapFlag.ENTITY_UNIT, true);
            unit.npc_id = npc_id;
            unit.unknown = unknown;
            unit.group = group;
            unit.unknown2 = unknown2;

            // 2. modify object transform and appearance

            unit.node.Position = heightmap.GetFixedPosition(pos);
            unit.node.SetAnglePlane(0);
            // find unit scale
            if (SFCFF.SFCategoryManager.gamedata[2024] == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.AddUnit(): There is no unit data block in gamedata!");
                throw new InvalidDataException("SFMap.AddUnit(): Malformed gamedata!");
            }
            int unit_index = SFCFF.SFCategoryManager.gamedata[2024].GetElementIndex(game_id);
            if (unit_index == -1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.AddUnit(): Unit with given id does not exist! Unit id = " + game_id.ToString());
                throw new InvalidDataException("SFMap.AddUnit(): Invalid unit ID!");
            }
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
                unit_manager.RestartAnimation(unit);
            }
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
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.MoveUnit(): Can't move unit to position " + new_pos.ToString());
                return -2;
            }

            // move unit and set chunk dependency
            heightmap.GetChunkNode(unit.grid_position).MapChunk.units.Remove(unit);
            heightmap.SetFlag(unit.grid_position, SFMapHeightMapFlag.ENTITY_UNIT, false);
            unit.grid_position = new_pos;
            heightmap.SetFlag(unit.grid_position, SFMapHeightMapFlag.ENTITY_UNIT, true);
            heightmap.GetChunkNode(unit.grid_position).MapChunk.units.Add(unit);
            unit.node.SetParent(heightmap.GetChunkNode(unit.grid_position));

            // change visual transform
            unit.node.Position = heightmap.GetFixedPosition(new_pos);

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

            unit.node.SetAnglePlane(angle);

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
            heightmap.SetFlag(unit.grid_position, SFMapHeightMapFlag.ENTITY_UNIT, false);

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

            if (unit.node != null)
            {
                SFRenderEngine.scene.RemoveSceneNode(unit.node);
            }

            unit.node = SFRenderEngine.scene.AddSceneUnit(new_unit_id, unit.GetName());
            unit.node.SetParent(heightmap.GetChunkNode(unit.grid_position));

            unit.game_id = new_unit_id;

            // object transform
            float z = heightmap.GetZ(unit.grid_position) / 100.0f;
            unit.node.Position = heightmap.GetFixedPosition(unit.grid_position);
            unit.node.SetAnglePlane(0);
            // find unit scale
            if (SFCFF.SFCategoryManager.gamedata[2024] == null)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.ReplaceUnit(): There is no unit data block in gamedata!");
                throw new InvalidDataException("SFMap.AddUnit(): Malformed gamedata!");
            }
            int unit_index = SFCFF.SFCategoryManager.gamedata[2024].GetElementIndex(unit.game_id);
            if (unit_index == -1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SFMap, "SFMap.ReplaceUnit(): Unit with given id does not exist! Unit id = " + unit.game_id.ToString());
                throw new InvalidDataException("SFMap.AddUnit(): Invalid unit ID!");
            }
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
                unit_manager.RestartAnimation(unit);
            }

            return 0;
        }

        private void FindPlatformID(string fname)
        {
            PlatformID = 6666;

            if (!SFCFF.SFCategoryManager.ready)
            {
                return;
            }

            int li = fname.LastIndexOf("map\\");
            if (li < 0)
            {
                return;
            }

            fname = fname.Substring(li + 4, fname.Length - li - 8);
            fname = fname.ToUpper();

            if (SFCFF.SFCategoryManager.gamedata[2052] == null)
            {
                return;
            }

            foreach (SFCFF.SFCategoryElement e in SFCFF.SFCategoryManager.gamedata[2052].elements)
            {
                if (e[2].ToString().ToUpper() == fname)
                {
                    PlatformID = (uint)e[0];
                    break;
                }
            }
        }

        public SFMapUnit FindUnit(SFCoord pos)
        {
            if (!heightmap.IsFlagSet(pos, SFMapHeightMapFlag.ENTITY_UNIT))
            {
                return null;
            }

            SFMapUnit unit = null;
            SFMapHeightMapChunk chunk = heightmap.GetChunk(pos);
            foreach (SFMapUnit u in chunk.units)
            {
                if (u.grid_position == pos)
                {
                    unit = u;
                    break;
                }
            }

            return unit;
        }

        public SFMapBuilding FindBuildingApprox(SFCoord pos)
        {
            foreach (SFMapBuilding b in building_manager.buildings)
            {
                float sel_scale = 0.0f;
                SFLua.lua_sql.SFLuaSQLBuildingData bld_data = SFLua.SFLuaEnvironment.buildings[b.game_id];
                if (bld_data != null)
                {
                    sel_scale = (float)(bld_data.SelectionScaling / 2);
                }

                OpenTK.Vector2 off = building_manager.building_collision[(ushort)b.game_id].origin;
                float angle = (float)(b.angle * Math.PI / 180);
                OpenTK.Vector2 r_off = new OpenTK.Vector2(off.X, off.Y);
                r_off.X = (float)((Math.Cos(angle) * off.X) - (Math.Sin(angle) * off.Y));
                r_off.Y = (float)((Math.Sin(angle) * off.X) + (Math.Cos(angle) * off.Y));
                SFCoord offset_pos = new SFCoord((int)r_off.X, (int)r_off.Y);

                if (SFCoord.Distance(b.grid_position - offset_pos, pos) <= sel_scale)
                {
                    return b;
                }
            }

            return null;
        }

        public SFMapObject FindObjectApprox(SFCoord pos)
        {
            SFMapObject ob = null;
            float least_distance = float.MaxValue;

            foreach (SFMapObject o in object_manager.objects)
            {
                float sel_scale = 3.0f;
                SFLua.lua_sql.SFLuaSQLObjectData obj_data = SFLua.SFLuaEnvironment.objects[o.game_id];
                if (obj_data != null)
                {
                    if (obj_data.SelectionScaling != 0.0f)
                    {
                        sel_scale = (float)(obj_data.SelectionScaling / 2);
                    }
                }

                float result_distance = SFCoord.Distance(o.grid_position, pos);
                if (result_distance <= sel_scale)
                {
                    if (result_distance < least_distance)
                    {
                        least_distance = result_distance;
                        ob = o;
                    }
                }
            }

            return ob;
        }

        public SFMapEntity FindNPCEntity(int npc_id)
        {
            if (npc_id <= 0)
            {
                return null;
            }

            foreach (SFMapUnit u in unit_manager.units)
            {
                if (u.npc_id == npc_id)
                {
                    return u;
                }
            }

            foreach (SFMapObject o in object_manager.objects)
            {
                if (o.npc_id == npc_id)
                {
                    return o;
                }
            }

            foreach (SFMapBuilding b in building_manager.buildings)
            {
                if (b.npc_id == npc_id)
                {
                    return b;
                }
            }

            return null;
        }
    }
}
