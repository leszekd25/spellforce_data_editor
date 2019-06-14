using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using SpellforceDataEditor.SFChunkFile;

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
        public SF3D.SFRender.SFRenderEngine render_engine { get; private set; } = null;
        public SFCFF.SFGameData gamedata { get; private set; } = null;

        public int Load(string filename, SF3D.SFRender.SFRenderEngine re, SFCFF.SFGameData gd, ToolStripLabel tx)
        {
            tx.Text = "Loading...";
            tx.GetCurrentParent().Refresh();
            SFChunkFile.SFChunkFile f = new SFChunkFile.SFChunkFile();
            int res = f.Open(filename);
            if (res != 0)
                return res;

            // load map size and tile indices
            render_engine = re;
            gamedata = gd;

            short size;

            tx.Text = "Loading map data...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c2 = f.GetChunkByID(2);
            if(c2 == null)
            {
                f.Close();
                return -4;
            }
            using (BinaryReader br = c2.Open())
            {
                size = br.ReadInt16();
                br.ReadByte();
                width = size;
                height = size;
                heightmap = new SFMapHeightMap(width, height);
                heightmap.map = this;
                heightmap.SetTilesRaw(br.ReadBytes(size * size));
            }
            c2.Close();

            // load terrain texture data
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
                            heightmap.texture_manager.texture_tiledata[i].blocks_vision = ((b_v % 2) == 1? true : false);
                        }
                    }
                }
                c3.Close();
            }

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
            for (short i = 0; i < size; i++)
            {
                SFChunkFileChunk c6_i = f.GetChunkByID(6, i);
                if (c6_i == null)
                {
                    f.Close();
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



            // load buildings
            tx.Text = "Loading buildings...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c11 = f.GetChunkByID(11);
            if(c11 != null)
            {
                using (BinaryReader br = c11.Open())
                {
                    building_manager = new SFMapBuildingManager();
                    building_manager.map = this;
                    while(br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int angle = br.ReadInt16();
                        int npc_id = br.ReadInt16();   // presumed
                        int b_type = br.ReadByte();
                        int b_lvl = 1;
                        int race_id = 0;
                        if(c11.get_data_type() > 1)
                            b_lvl = br.ReadByte();
                        if (c11.get_data_type() > 2)
                            race_id = br.ReadByte();
                        AddBuilding(b_type, pos, angle, npc_id, b_lvl, race_id);
                    }
                }
                c11.Close();
            }

            // load units
            tx.Text = "Loading units...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c12 = f.GetChunkByID(12);
            if(c12 != null)
            {
                using (BinaryReader br = c12.Open())
                {
                    unit_manager = new SFMapUnitManager();
                    unit_manager.map = this;
                    while(br.BaseStream.Position < br.BaseStream.Length)
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
                        if(c12.get_data_type() == 5)
                            unknown2 = br.ReadByte();
                        AddUnit(unit_id, pos, angle, npc_id, unknown, group, unknown2);
                    }
                }
                c12.Close();
            }

            // load objects
            tx.Text = "Loading objects...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c29 = f.GetChunkByID(29);
            if (c29 != null)
            {
                using (BinaryReader br = c29.Open())
                {
                    object_manager = new SFMapObjectManager();
                    object_manager.map = this;
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int object_id = br.ReadInt16();
                        int angle = br.ReadInt16();
                        int npc_id = br.ReadUInt16();
                        if (c29.get_data_type() == 6)
                            br.ReadBytes(6);
                        else if (c29.get_data_type() == 4)
                            br.ReadBytes(2);
                        if ((object_id >= 65) && (object_id <= 67))   // editor only
                            continue;
                        AddObject(object_id, pos, angle, npc_id);
                    }
                }
                c29.Close();
            }

            // load interactive objects
            tx.Text = "Loading interactive objects...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c30 = f.GetChunkByID(30);
            if (c30 != null)
            {
                using (BinaryReader br = c30.Open())
                {
                    int_object_manager = new SFMapInteractiveObjectManager();
                    int_object_manager.map = this;
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
            }

            // load decorations
            tx.Text = "Loading decorations...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c31 = f.GetChunkByID(31);
            if (c31 != null)
            {
                using (BinaryReader br = c31.Open())
                {
                    decoration_manager = new SFMapDecorationManager();
                    decoration_manager.map = this;
                    decoration_manager.dec_assignment = br.ReadBytes(1048576);
                }
                c31.Close();
            }
            SFChunkFileChunk c32 = f.GetChunkByID(32);
            if (c32 != null)
            { 
                using (BinaryReader br = c32.Open())
                {
                    for(int i = 0; i < 255; i++)
                    {
                        decoration_manager.dec_groups[i] = new SFMapDecorationGroup();
                        for(int j = 0; j < 30; j++)
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
            tx.Text = "Loading portals...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c35 = f.GetChunkByID(35);
            if (c35 != null)
            {
                using (BinaryReader br = c35.Open())
                {
                    portal_manager = new SFMapPortalManager();
                    portal_manager.map = this;
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
            }

            // load lakes
            SFChunkFileChunk c40 = f.GetChunkByID(40);
            if(c40 != null)
            {
                using (BinaryReader br = c40.Open())
                {
                    lake_manager = new SFMapLakeManager();
                    lake_manager.map = this;
                    int lake_count = br.ReadByte();
                    for(int i = 0; i < lake_count; i++)
                    {
                        short x = br.ReadInt16();
                        short y = br.ReadInt16();
                        short z_diff = br.ReadInt16();
                        byte type = br.ReadByte();
                        lake_manager.AddLake(new SFCoord(x, y), z_diff, type);
                    }
                }
                c40.Close();
            }

            // load map flags
            tx.Text = "Loading map flags...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c42 = f.GetChunkByID(42);
            if (c42 != null)
            {

                SFCoord pos = new SFCoord();
                using (BinaryReader br = c42.Open())
                {
                    int pos_count = c42.get_data_length() / 4;
                    for (int i = 0; i < pos_count; i++)
                    {
                        pos.x = br.ReadInt16();
                        pos.y = br.ReadInt16();
                        heightmap.chunk42_data.Add(pos);
                    }
                }
                c42.Close();
            }
            SFChunkFileChunk c56 = f.GetChunkByID(56);
            if (c56 != null)
            {

                SFCoord pos = new SFCoord();
                using (BinaryReader br = c56.Open())
                {
                    int pos_count = c56.get_data_length() / 4;
                    for (int i = 0; i < pos_count; i++)
                    {
                        pos.x = br.ReadInt16();
                        pos.y = br.ReadInt16();
                        heightmap.chunk56_data.Add(pos);
                    }
                }
                c56.Close();
            }
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

            // load metadata
            tx.Text = "Loading metadata...";
            tx.GetCurrentParent().Refresh();
            metadata = new SFMapMetaData();

            SFChunkFileChunk c55 = f.GetChunkByID(55);
            if(c55 != null)
            {
                using (BinaryReader br = c55.Open())
                {
                    int player_count = br.ReadInt32();
                    metadata.player_count = player_count;
                    metadata.spawns = new SFMapSpawn[player_count];
                    for(int i = 0; i < player_count; i++)
                    {
                        short x = br.ReadInt16();
                        short y = br.ReadInt16();
                        ushort text_id = br.ReadUInt16();
                        short unknown = br.ReadInt16();
                        metadata.spawns[i].pos = new SFCoord(x, y);
                        metadata.spawns[i].text_id = text_id;
                        metadata.spawns[i].unknown = unknown;
                    }
                }
                c55.Close();
            }

            SFChunkFileChunk c53 = f.GetChunkByID(53);
            if(c53 != null)
            {
                using (BinaryReader br = c53.Open())
                {
                    metadata.multi_teams = new List<SFMapMultiplayerTeamComposition>();
                    if (c53.get_data_type() == 2)
                    {
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
                            tcomp.players = new List<SFMapTeamPlayer>[tcomp.team_count];
                            for (int i = 0; i < tcomp.team_count; i++)
                            {
                                tcomp.players[i] = new List<SFMapTeamPlayer>();
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
                            tcomp.players = new List<SFMapTeamPlayer>[tcomp.team_count];
                            for (int i = 0; i < tcomp.team_count; i++)
                            {
                                tcomp.players[i] = new List<SFMapTeamPlayer>();
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
                                                (spawn_certain > 0)));
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
                }
                c53.Close();
            }
            else
            {
                metadata.map_type = SFMapType.CAMPAIGN;
            }
            
            SFChunkFileChunk c59 = f.GetChunkByID(59);
            if(c59 != null)
            {
                using (BinaryReader br = c59.Open())
                {
                    metadata.coop_spawn_params = new SFMapCoopSpawnParameters[3];
                    for(int i = 0; i < 3; i++)
                    {
                        metadata.coop_spawn_params[i].param1 = br.ReadSingle();
                        metadata.coop_spawn_params[i].param2 = br.ReadSingle();
                        metadata.coop_spawn_params[i].param3 = br.ReadSingle();
                        metadata.coop_spawn_params[i].param4 = br.ReadSingle();
                    }
                }
                c59.Close();
            }

            // generate overlay data
            heightmap.OverlayCreate("TileMovementBlock", new OpenTK.Vector4(0.5f, 0, 0, 0.7f));
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (heightmap.texture_manager.texture_tiledata[heightmap.tile_data[i * width + j]].blocks_movement)
                        heightmap.OverlayAdd("TileMovementBlock", new SFCoord(j, i));

            heightmap.OverlayCreate("ManualMovementBlock", new OpenTK.Vector4(1, 0, 0, 0.7f));
            foreach (SFCoord p in heightmap.chunk42_data)
                heightmap.OverlayAdd("ManualMovementBlock", p);

            heightmap.OverlayCreate("ManualVisionBlock", new OpenTK.Vector4(1, 1, 0, 0.7f));
            foreach (SFCoord p in heightmap.chunk56_data)
                heightmap.OverlayAdd("ManualVisionBlock", p);

            heightmap.OverlayCreate("LakeTile", new OpenTK.Vector4(0.4f, 0.4f, 0.9f, 0.7f));
            for (int i = 0; i < heightmap.lake_data.Length; i++)
            {
                SFCoord p = new SFCoord(i % width, i / width);
                if(heightmap.lake_data[i] > 0)
                    heightmap.OverlayAdd("LakeTile", p);
            }

            heightmap.OverlayCreate("ManualLakeTile", new OpenTK.Vector4(0.6f, 0.6f, 1.0f, 0.7f));
            
            foreach (SFMapHeightMapChunk chunk in heightmap.chunks)
            {
                chunk.OverlayUpdate("TileMovementBlock");
                chunk.OverlayUpdate("ManualMovementBlock");
                chunk.OverlayUpdate("ManualVisionBlock");
                chunk.OverlayUpdate("LakeTile");
                chunk.OverlayUpdate("ManualLakeTile");
            }

            // selection helper stuff
            selection_helper.AssignToMap(this);

            // done

            f.Close();

            tx.Text = "Map loaded";

            return 0;
        }

        public int Save(string filename)
        {
            if (gamedata == null)
                return -1;

            SFChunkFile.SFChunkFile f = new SFChunkFile.SFChunkFile();
            int res = f.Create(filename, SFChunkFileType.MAP);
            if (res != 0)
                return res;


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
            for(int i = 0; i < heightmap.width; i++)
            {
                heightmap.GetRowRaw(i, ref c6i_data);
                Buffer.BlockCopy(c6i_data, 0, c6i_rawdata, 0, c6i_rawdata.Length);
                f.AddChunk(6, (short)i, true, 6, c6i_rawdata);
            }

            // chunk 3
            byte[] c3_data = new byte[3570];
            for(int i = 0; i < 255; i++)
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
            for(int i = 0; i < 63; i++)
            {
                c4_data[i] = (byte)heightmap.texture_manager.texture_id[i];
            }
            f.AddChunk(4, 0, true, 4, c4_data);

            // chunk 42
            heightmap.chunk42_data.Sort();
            byte[] c42_data = new byte[heightmap.chunk42_data.Count * 4];
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
                    for(int i = 0; i < lake_manager.lakes.Count; i++)
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
                    for(int i = 0; i < object_manager.objects.Count; i++)
                    {
                        bw.Write((short)object_manager.objects[i].grid_position.x);
                        bw.Write((short)object_manager.objects[i].grid_position.y);
                        bw.Write((short)object_manager.objects[i].game_id);
                        bw.Write((short)object_manager.objects[i].angle);
                        bw.Write((short)object_manager.objects[i].npc_id);
                        bw.Write((short)0);
                        if (object_manager.objects[i].game_id != 2541)
                            bw.Write((int)0);
                        else
                        {
                            SFMapCoopAISpawn coop_spawn = new SFMapCoopAISpawn();
                            if(!metadata.GetCoopAISpawnByObject(object_manager.objects[i], ref coop_spawn))
                                bw.Write((int)0);
                            else
                            {
                                bw.Write((short)coop_spawn.spawn_id);
                                bw.Write((short)(coop_spawn.spawn_certain ? 1 : 0));
                            }
                        }
                    }
                }
            }
            f.AddChunk(29, 0, true, 6, c29_data);

            // chunk 35
            byte[] c35_data = new byte[portal_manager.portals.Count * 8];
            using(MemoryStream ms = new MemoryStream(c35_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for(int i = 0; i < portal_manager.portals.Count; i++)
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
                    for(int i = 0; i < int_object_manager.int_objects.Count; i++)
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
            byte[] c11_data = new byte[building_manager.buildings.Count * (metadata.map_type == SFMapType.CAMPAIGN ? 11 : 10)];
            using (MemoryStream ms = new MemoryStream(c11_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for(int i = 0; i < building_manager.buildings.Count; i++)
                    {
                        bw.Write((short)building_manager.buildings[i].grid_position.x);
                        bw.Write((short)building_manager.buildings[i].grid_position.y);
                        bw.Write((short)building_manager.buildings[i].angle);
                        bw.Write((short)building_manager.buildings[i].npc_id);
                        bw.Write((byte)building_manager.buildings[i].game_id);
                        bw.Write((byte)building_manager.buildings[i].level);
                        if(metadata.map_type == SFMapType.CAMPAIGN)
                            bw.Write((byte)building_manager.buildings[i].race_id);
                    }
                }
            }
            f.AddChunk(11, 0, true, (short)(metadata.map_type == SFMapType.CAMPAIGN ? 3 : 2), c11_data);

            // chunk 12
            byte[] c12_data = new byte[unit_manager.units.Count * 14];
            using (MemoryStream ms = new MemoryStream(c12_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for(int i = 0; i < unit_manager.units.Count; i++)
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
            short chunk_type = 0;
            byte[] team_array = new byte[0];
            switch(metadata.map_type)
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
                    team_array = metadata.TeamsToArray(2, true);
                    break;
                default:
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
            byte[] c55_data = new byte[4 + metadata.spawns.Length * 8];
            using (MemoryStream ms = new MemoryStream(c55_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write((int)metadata.spawns.Length);
                    for(int i = 0; i < metadata.spawns.Length; i++)
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
                    for(int i = 0; i < heightmap.chunk60_data.Count; i++)
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
            if(metadata.map_type == SFMapType.COOP)
            {
                float[] c59_data = new float[12];
                byte[] c59_rawdata = new byte[48];
                for (int i = 0; i < metadata.coop_spawn_params.Length; i++)
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

            return 0;
        }

        public void Unload()
        {
            heightmap.Unload();
            metadata.Unload();             // minimap texture
            selection_helper.Dispose();    // selection 3d mesh
        }

        public void AddDecoration(int game_id, SFCoord pos)
        {
            SFMapDecoration dec = decoration_manager.AddDecoration(game_id, pos);


            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = render_engine.scene_manager.objects_static[dec.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);


            heightmap.GetChunk(pos).AddDecoration(dec);
            render_engine.scene_manager.objects_static[dec.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public void AddObject(int game_id, SFCoord pos, int angle, int npc_id)
        {
            SFMapObject obj = object_manager.AddObject(game_id, pos, angle);
            obj.npc_id = npc_id;


            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = render_engine.scene_manager.objects_static[obj.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);


            heightmap.GetChunk(pos).AddObject(obj);
            render_engine.scene_manager.objects_static[obj.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public void AddInteractiveObject(int game_id, SFCoord pos, int angle, int unk_byte)
        {
            SFMapInteractiveObject obj = int_object_manager.AddInteractiveObject(game_id, pos, angle, unk_byte);


            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = render_engine.scene_manager.objects_static[obj.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);


            heightmap.GetChunk(pos).AddInteractiveObject(obj);
            render_engine.scene_manager.objects_static[obj.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public void AddBuilding(int game_id, SFCoord pos, int angle, int npc_id, int lvl, int race_id)
        {
            SFMapBuilding bld = building_manager.AddBuilding(game_id, pos, angle, npc_id, lvl, race_id);


            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = render_engine.scene_manager.objects_static[bld.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);


            heightmap.GetChunk(pos).AddBuilding(bld);
            render_engine.scene_manager.objects_static[bld.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public void AddPortal(int game_id, SFCoord pos, int angle)
        {
            SFMapPortal ptl = portal_manager.AddPortal(game_id, pos, angle);


            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = render_engine.scene_manager.objects_static[ptl.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);


            heightmap.GetChunk(pos).AddPortal(ptl);
            render_engine.scene_manager.objects_static[ptl.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public void AddUnit(int game_id, SFCoord pos, int angle, int npc_id, int unknown, int group, int unknown2)
        {
            // 1. add new unit in unit manager
            SFMapUnit unit = unit_manager.AddUnit(game_id, pos, angle);
            unit.npc_id = npc_id;
            unit.unknown = unknown;
            unit.group = group;
            unit.unknown2 = unknown2;

            // 2. modify object transform and appearance

            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D obj = render_engine.scene_manager.objects_static[unit.GetObjectName()];
            obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y - 1));
            obj.SetAnglePlane(angle);
            // find unit scale
            int unit_index = gamedata.categories[17].get_element_index(game_id);
            if (unit_index == -1)
                throw new InvalidDataException("SFMap.AddUnit(): Invalid unit ID!");
            SFCFF.SFCategoryElement unit_data = gamedata.categories[17].get_element(unit_index);
            unit_index = gamedata.categories[3].get_element_index((ushort)unit_data.get_single_variant(2).value);
            if(unit_index == -1)
                throw new InvalidDataException("SFMap.AddUnit(): Invalid unit data!");
            unit_data = gamedata.categories[3].get_element(unit_index);
            float unit_size = Math.Max(((ushort)unit_data.get_single_variant(19).value), (ushort)40) / 100.0f;
            obj.Scale = new OpenTK.Vector3(unit_size*100/128);
        }

        public int MoveUnit(int unit_map_index, SFCoord new_pos)
        {
            SFMapUnit unit = null;
            if ((unit_manager.units.Count <= unit_map_index)||(unit_map_index < 0))
                return -1;
            unit = unit_manager.units[unit_map_index];
            if (!heightmap.CanMoveToPosition(new_pos))
                return -2;

            // move unit and set chunk dependency
            heightmap.GetChunk(unit.grid_position).units.Remove(unit);
            unit.grid_position = new_pos;
            heightmap.GetChunk(unit.grid_position).units.Add(unit);

            // change visual transform
            float z = heightmap.GetZ(new_pos) / 100.0f;
            SF3D.Object3D obj = render_engine.scene_manager.objects_static[unit.GetObjectName()];
            obj.Position = new OpenTK.Vector3((float)new_pos.x, (float)z, (float)(height - new_pos.y - 1));

            return 0;
        }

        public int DeleteUnit(int unit_map_index)
        {
            SFMapUnit unit = null;
            if ((unit_manager.units.Count <= unit_map_index) || (unit_map_index < 0))
                return -1;
            unit = unit_manager.units[unit_map_index];

            unit_manager.RemoveUnit(unit);

            return 0;
        }

        public int ReplaceUnit(int unit_map_index, ushort new_unit_id)
        {
            SFMapUnit unit = null;
            if ((unit_manager.units.Count <= unit_map_index) || (unit_map_index < 0))
                return -1;
            unit = unit_manager.units[unit_map_index];

            render_engine.scene_manager.DeleteObject(unit.GetObjectName());
            render_engine.scene_manager.AddObjectUnit(new_unit_id, unit.GetObjectName(), false);

            unit.game_id = new_unit_id;

            // object transform
            float z = heightmap.GetZ(unit.grid_position) / 100.0f;
            SF3D.Object3D obj = render_engine.scene_manager.objects_static[unit.GetObjectName()];
            obj.Position = new OpenTK.Vector3((float)unit.grid_position.x, (float)z, (float)(height - unit.grid_position.y - 1));
            obj.SetAnglePlane(unit.angle);
            // unit scale
            int unit_index = gamedata.categories[17].get_element_index(unit.game_id);
            if (unit_index == -1)
                throw new InvalidDataException("SFMap.AddUnit(): Invalid unit ID!");
            SFCFF.SFCategoryElement unit_data = gamedata.categories[17].get_element(unit_index);
            unit_index = gamedata.categories[3].get_element_index((ushort)unit_data.get_single_variant(2).value);
            if (unit_index == -1)
                throw new InvalidDataException("SFMap.AddUnit(): Invalid unit data!");
            unit_data = gamedata.categories[3].get_element(unit_index);
            float unit_size = Math.Max(((ushort)unit_data.get_single_variant(19).value), (ushort)40) / 100.0f;
            obj.Scale = new OpenTK.Vector3(unit_size * 100 / 128);

            return 0;
        }
    }
}
