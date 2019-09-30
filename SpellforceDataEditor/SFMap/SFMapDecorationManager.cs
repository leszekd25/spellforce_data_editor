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
        public List<int> random_cache = new List<int>();

        public SFMapDecorationGroup()
        {
            weight.Initialize();
            dec_id.Initialize();
        }

        public ushort ChooseRandom()
        {
            if (random_cache.Count == 0)
                return 0;
            return dec_id[random_cache[MathUtils.Rand() % random_cache.Count]];
        }

        public void SetDecoration(int dec_index, ushort d_id, byte d_w = 255)
        {
            if ((dec_id[dec_index] == 0) || (weight[dec_index] == 0)) 
            {
                if ((d_id != 0) && (d_w != 0))
                    random_cache.Add(dec_index);
            }
            if ((dec_id[dec_index] != 0) && (weight[dec_index] != 0)) 
            {
                if ((d_id == 0) || (d_w == 0))
                    random_cache.Remove(dec_index);
            }
            dec_id[dec_index] = d_id;
            weight[dec_index] = d_w;
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
            SF3D.SceneSynchro.SceneNode node = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(id, dec_name, false);
            node.SetParent(map.heightmap.GetChunkNode(position));
            
            // 3. add new unit in respective chunk
            map.heightmap.GetChunk(position).AddDecoration(dec);

            return dec;
        }

        public void RemoveDecoration(SFMapDecoration d)
        {
            decorations.Remove(d);
            
            SF3D.SceneSynchro.SceneNode chunk_node = map.heightmap.GetChunkNode(d.grid_position);
            SF3D.SceneSynchro.SceneNode dec_node = chunk_node.FindNode<SF3D.SceneSynchro.SceneNode>(d.GetObjectName());
            if (dec_node != null)
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(dec_node);

            map.heightmap.GetChunk(d.grid_position).RemoveDecoration(d);
        }

        public void ReplaceDecoration(SFMapDecoration d, int new_id)
        {
            if (d.game_id == new_id)
                return;
            
            SF3D.SceneSynchro.SceneNode chunk_node = map.heightmap.GetChunkNode(d.grid_position);
            SF3D.SceneSynchro.SceneNode dec_node = chunk_node.FindNode<SF3D.SceneSynchro.SceneNode>(d.GetObjectName());
            if(dec_node != null)
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(dec_node);

            SF3D.SceneSynchro.SceneNode dec_node2 = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(new_id, d.GetObjectName(), false);
            dec_node2.SetParent(chunk_node);
            dec_node2.Position = map.heightmap.GetFixedPosition(d.grid_position);
            dec_node2.Rotation = OpenTK.Quaternion.FromAxisAngle(new OpenTK.Vector3(1f, 0f, 0f), (float)-Math.PI / 2);
            dec_node2.Scale = new OpenTK.Vector3(100 / 128f);

            d.game_id = new_id;
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
