using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using SpellforceDataEditor.SFChunkFile;

namespace SpellforceDataEditor.SFMap
{
    public class SFMap
    {
        public static SFCoord[] NEIGHBORS = {new SFCoord(1, 0), new SFCoord(1, -1), new SFCoord(0, -1), new SFCoord(-1, -1),
                                             new SFCoord(-1, 0), new SFCoord(-1, 1), new SFCoord(0, 1), new SFCoord(1, 1)};

        int width = 0;
        int height = 0;
        public SFMapHeightMap heightmap { get; private set; } = null;
        public SFMapUnitManager unit_manager { get; private set; } = null;
        public SFMapObjectManager object_manager { get; private set; } = null;

        public int Load(string filename, SF3D.SceneSynchro.SFSceneManager scene, SFCFF.SFGameData gd)
        {
            SFChunkFile.SFChunkFile f = new SFChunkFile.SFChunkFile();
            int res = f.Open(filename);
            if (res != 0)
                return res;

            // load map size and tile indices
            short size;

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

            // load units
            SFChunkFileChunk c12 = f.GetChunkByID(12);
            if(c12 != null)
            {
                using (BinaryReader br = c12.Open())
                {
                    unit_manager = new SFMapUnitManager();
                    unit_manager.AssignScene(scene);
                    while(br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        br.ReadInt16();
                        int unit_id = br.ReadInt16();
                        br.ReadBytes(6);
                        float z = heightmap.GetZ(pos) / 100.0f;
                        SFMapUnit unit = unit_manager.AddUnit(unit_id, pos);
                        SF3D.Object3D obj = scene.objects_static[unit.GetObjectName()];
                        obj.Position = new OpenTK.Vector3((float)x, (float)z, (float)(size - y));
                        // find unit scale
                        int unit_index = gd.categories[17].get_element_index(unit_id);
                        if (unit_index == -1)
                            throw new InvalidDataException("SFMap.Load(): Invalid unit ID!");
                        SFCFF.SFCategoryElement unit_data = gd.categories[17].get_element(unit_index);
                        unit_index = gd.categories[3].get_element_index((ushort)unit_data.get_single_variant(2).value);
                        if(unit_index == -1)
                            throw new InvalidDataException("SFMap.Load(): Invalid unit data!");
                        unit_data = gd.categories[3].get_element(unit_index);
                        float unit_size = Math.Max(((ushort)unit_data.get_single_variant(19).value), (ushort)40) / 100.0f;
                        obj.Scale = new OpenTK.Vector3(unit_size);
                    }
                }
            }

            // load objects
            SFChunkFileChunk c29 = f.GetChunkByID(29);
            if(c29 != null)
            {
                using (BinaryReader br = c29.Open())
                {
                    object_manager = new SFMapObjectManager();
                    object_manager.AssignScene(scene);
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        int x = br.ReadInt16();
                        int y = br.ReadInt16();
                        SFCoord pos = new SFCoord(x, y);
                        int object_id = br.ReadInt16();
                        int angle = br.ReadInt16();
                        br.ReadBytes(8);
                        float z = heightmap.GetZ(pos) / 100.0f;
                        SFMapObject obj = object_manager.AddObject(object_id, pos, angle);
                        SF3D.Object3D _obj = scene.objects_static[obj.GetObjectName()];
                        _obj.Position = new OpenTK.Vector3((float)x, (float)z, (float)(size - y));
                        _obj.SetAnglePlane(angle);
                    }
                }
            }

            f.Close();

            System.Diagnostics.Debug.WriteLine("Operation successful! Map size " + width.ToString());



            return 0;
        }

        public void Unload()
        {
            heightmap.Unload();
        }
    }
}
