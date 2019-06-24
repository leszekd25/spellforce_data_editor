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

        public int AddDecoration(ushort d_id, byte d_w)
        {
            if (dec_used == 29)
                return -1;
            if (d_w == 0)
                return -2;


            dec_used += 1;
            dec_id[dec_used] = d_id;
            weight[dec_used] = d_w;

            return 0;
        }

        public int RemoveDecoration(int dec_index)
        {
            if((dec_index < 0)||(dec_index >= 30))
                return -1;
            if (weight[dec_index] == 0)
                return -2;

            for(int i = dec_index; i < dec_used; i++)
            {
                dec_id[i] = dec_id[i+1];
                weight[i] = weight[i+1];
            }
            dec_id[dec_used] = 0;
            weight[dec_used] = 0;
            dec_used -= 1;

            return 0;
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
            decorations.Add(dec);

            string dec_name = dec.GetObjectName();
            map.render_engine.scene_manager.AddObjectObject(id, dec_name);
            
            // 3. add new unit in respective chunk
            map.heightmap.GetChunk(position).AddDecoration(dec);
            map.render_engine.scene_manager.objects_static[dec_name].Visible = map.heightmap.GetChunk(position).Visible;

            return dec;
        }

        public void RemoveDecoration(SFMapDecoration d)
        {
            decorations.Remove(d);

            map.render_engine.scene_manager.DeleteObject(d.GetObjectName());

            map.heightmap.GetChunk(d.grid_position).RemoveDecoration(d);
        }

        public void ReplaceDecoration(SFMapDecoration d, int new_id)
        {
            if (d.game_id == new_id)
                return;

            map.render_engine.scene_manager.DeleteObject(d.GetObjectName());
            map.render_engine.scene_manager.AddObjectObject(new_id, d.GetObjectName());
        }

        public byte GetDecAssignment(SFCoord pos)
        {
            return dec_assignment[pos.y * 1024 - pos.x + 1024 - 1];
        }
        
        public byte GetFixedDecAssignment(SFCoord pos)
        {
            return dec_assignment[pos.x * 1024 + pos.y - 1];
        }

        public void SetDecAssignment(SFCoord pos, byte dec)
        {
            dec_assignment[pos.y * 1024 - pos.x + 1024 - 1] = dec;
        }

        public void SetFixedDecAssignment(SFCoord pos, byte dec)
        {
            dec_assignment[pos.x * 1024 + pos.y - 1] = dec;
        }

        public SFCoord GetDecPosition(int offset)
        {
            return new SFCoord(1024 - (offset % 1024) - 1, offset / 1024);
        }

        public SFCoord GetFixedDecPosition(int offset)
        {
            return new SFCoord(offset / 1024, (offset % 1024) + 1);
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

        // assumes positions are valid
        public void ModifyDecorations(HashSet<SFCoord> pos, int dec_type)
        {
            foreach(SFCoord p in pos)
            {
                SetFixedDecAssignment(p, (byte)dec_type);

                SFMapHeightMapChunk chunk = map.heightmap.GetChunk(p);
                SFMapDecoration dec = null;
                foreach(SFMapDecoration d in chunk.decorations)
                    if(d.grid_position == p)
                    {
                        dec = d;
                        break;
                    }
                /*if (dec != null)
                    RemoveDecoration(dec);
                SetFixedDecAssignment(p, (byte)dec_type);
                if(dec_type != 0)
                {
                    ushort dec_id = dec_groups[GetFixedDecAssignment(p)].ChooseRandom();
                    if (dec_id != 0)
                        map.AddDecoration(dec_id, p);
                }*/
                if (dec != null)    // if there exists a decoration at position
                {

                    ushort dec_id = dec_groups[dec_type].ChooseRandom();
                    if (dec_id != 0)   // if chosen decoration id is not 0, simply replace
                        ReplaceDecoration(dec, dec_id);
                    else               // else, remove (this happens if group 0 or empty group is chosen as well)
                        RemoveDecoration(dec);
                }
                else                // if theres no existing decoration at position
                {
                    ushort dec_id = dec_groups[dec_type].ChooseRandom();
                    if (dec_id != 0)   // if chosen decoration id is not 0, add new decoration at position
                        map.AddDecoration(dec_id, p);
                    // else               // otherwise do nothing lol
                    //    ;
                }
            }
        }

        // updates decorations at positions
        public void ModifyDecorations(byte dec_type)
        {
            if (dec_type == 0)
                return;
            for(int i = 0; i < 1048576; i++)
            {
                if (dec_assignment[i] == dec_type)
                {
                    SFCoord p = GetFixedDecPosition(i);
                    SFMapHeightMapChunk chunk = map.heightmap.GetChunk(p);
                    SFMapDecoration dec = null;
                    foreach (SFMapDecoration d in chunk.decorations)
                        if (d.grid_position == p)
                        {
                            dec = d;
                            break;
                        }
                    if (dec != null)    // if there exists a decoration at position
                    {

                        ushort dec_id = dec_groups[dec_type].ChooseRandom();
                        if (dec_id != 0)   // if chosen decoration id is not 0, simply replace
                            ReplaceDecoration(dec, dec_id);
                        else               // else, remove (this happens if group 0 or empty group is chosen as well)
                            RemoveDecoration(dec);
                    }
                    else                // if theres no existing decoration at position
                    {
                        ushort dec_id = dec_groups[dec_type].ChooseRandom();
                        if (dec_id != 0)   // if chosen decoration id is not 0, add new decoration at position
                            map.AddDecoration(dec_id, p);
                        // else               // otherwise do nothing lol
                        //     ;
                    }
                }
            }
        }
    }
}
