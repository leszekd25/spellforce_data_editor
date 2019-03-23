using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapDecoration
    {
        static int max_id = 0;

        public SFCoord grid_position = new SFCoord(0, 0);
        public int id = -1;
        public int game_id = -1;

        public string GetObjectName()
        {
            return "DECORATION_" + id.ToString();
        }

        public SFMapDecoration()
        {
            id = max_id;
            max_id += 1;
        }

        public override string ToString()
        {
            return GetObjectName();
        }
    }

    public class SFMapDecorationGroup
    {
        public Byte[] weight = new byte[30];
        public ushort[] dec_id = new ushort[30];
        public int dec_used = 0;

        public SFMapDecorationGroup()
        {
            weight.Initialize();
            dec_id.Initialize();
        }

        public ushort ChooseRandom()
        {
            if (dec_used == 0)
                return 0;
            return dec_id[1+Utility.Rand() % dec_used];
        }
    }

    public class SFMapDecorationManager
    {
        public Byte[] dec_assignment = new Byte[1048576];
        public SFMapDecorationGroup[] dec_groups = new SFMapDecorationGroup[255];
        public List<SFMapDecoration> decorations { get; private set; } = new List<SFMapDecoration>();
        public SFMap map = null;

        public SFMapDecoration AddDecoration(int id, SFCoord position)
        {
            SFMapDecoration dec = new SFMapDecoration();
            dec.grid_position = position;
            dec.game_id = id;

            string dec_name = dec.GetObjectName();
            map.render_engine.scene_manager.AddObjectObject(id, dec_name);
            return dec;
        }

        public byte GetDecAssignment(SFCoord pos)
        {
            return dec_assignment[pos.y * 1024 - pos.x + 1024 - 1];
        }

        public void SetDecAssignment(SFCoord pos, byte dec)
        {
            dec_assignment[pos.y * 1024 - pos.x + 1024 - 1] = dec;
        }

        public SFCoord GetDecPosition(int offset)
        {
            return new SFCoord(1024 - (offset % 1024) - 1, offset / 1024);
        }

        public SFCoord GetFixedDecPosition(int offset)
        {
            return new SFCoord(offset / 1024, -(map.height - (offset % 1024) - 1)+map.height);
        }

        public void GenerateDecorations()
        {
            ushort size = (ushort)map.width;
            SFCoord pos;
            for(int i = 0; i < 1048576; i++)
            {
                if(dec_assignment[i] != 0)
                {
                    // choose decoration
                    pos = GetFixedDecPosition(i);
                    ushort dec_id = dec_groups[dec_assignment[i]].ChooseRandom();
                    if (dec_id != 0)
                        map.AddDecoration(dec_id, pos);
                }
            }
        }
    }
}
