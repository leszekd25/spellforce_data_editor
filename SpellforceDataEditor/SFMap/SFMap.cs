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
        public SFMapUnitManager unit_manager { get; private set; } = null;
        public SFMapObjectManager object_manager { get; private set; } = null;
        public SFMapDecorationManager decoration_manager { get; private set; } = null;
        public SFMapLakeManager lake_manager { get; private set; } = null;
        public SF3D.SFRender.SFRenderEngine render_engine { get; private set; } = null;
        public SFCFF.SFGameData gamedata { get; private set; } = null;

        public int Load(string filename, SF3D.SFRender.SFRenderEngine re, SFCFF.SFGameData gd, ToolStripStatusLabel tx)
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

            // load terrain texture data
            SFChunkFileChunk c3 = f.GetChunkByID(3);
            if (c3 != null)
            {
                using (BinaryReader br = c3.Open())
                {
                    heightmap.texture_manager.SetTextureReindexRaw(br.ReadBytes((int)br.BaseStream.Length));
                }
            }

            SFChunkFileChunk c4 = f.GetChunkByID(4);
            if (c4 != null)
            {
                using (BinaryReader br = c4.Open())
                {
                    heightmap.texture_manager.SetTextureIDsRaw(br.ReadBytes((int)br.BaseStream.Length));
                }
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
            }

            // generate heightmap models and reindex textures
            heightmap.Generate();

            tx.Text = "Loading units...";
            tx.GetCurrentParent().Refresh();

            // load units
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
                        br.ReadInt16();
                        int unit_id = br.ReadInt16();
                        br.ReadBytes(6);
                        AddUnit(unit_id, pos);
                    }
                }
            }

            // load objects
            tx.Text = "Loading objects...";
            tx.GetCurrentParent().Refresh();

            SFChunkFileChunk c29 = f.GetChunkByID(29);
            if(c29 != null)
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
                        br.ReadBytes(8);
                        AddObject(object_id, pos, angle);
                    }
                }
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
            }
            decoration_manager.GenerateDecorations();

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
            }


            // done

            f.Close();

            tx.Text = "Map loaded";

            return 0;
        }

        public void Unload()
        {
            heightmap.Unload();
        }

        public void AddDecoration(int game_id, SFCoord pos)
        {
            SFMapDecoration dec = decoration_manager.AddDecoration(game_id, pos);


            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = render_engine.scene_manager.objects_static[dec.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);


            heightmap.GetChunk(pos).AddDecoration(dec);
            render_engine.scene_manager.objects_static[dec.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public void AddObject(int game_id, SFCoord pos, int angle)
        {
            SFMapObject obj = object_manager.AddObject(game_id, pos, angle);


            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D _obj = render_engine.scene_manager.objects_static[obj.GetObjectName()];
            _obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height-pos.y));
            _obj.Scale = new OpenTK.Vector3(100 / 128f);
            _obj.SetAnglePlane(angle);


            heightmap.GetChunk(pos).AddObject(obj);
            render_engine.scene_manager.objects_static[obj.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }

        public void AddUnit(int game_id, SFCoord pos)
        {
            // 1. add new unit in unit manager
            SFMapUnit unit = unit_manager.AddUnit(game_id, pos);

            // 2. modify object transform and appearance

            float z = heightmap.GetZ(pos) / 100.0f;
            SF3D.Object3D obj = render_engine.scene_manager.objects_static[unit.GetObjectName()];
            obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(height - pos.y));
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


            // 3. add new unit in respective chunk
            heightmap.GetChunk(pos).AddUnit(unit);
            render_engine.scene_manager.objects_static[unit.GetObjectName()].Visible = heightmap.GetChunk(pos).Visible;
        }
    }
}
