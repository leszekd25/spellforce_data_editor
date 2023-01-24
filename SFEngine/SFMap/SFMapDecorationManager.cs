using System;
using System.Collections.Generic;

namespace SFEngine.SFMap
{
    public class SFMapDecoration
    {
        static int max_id = 0;

        public SFCoord grid_position = new SFCoord(0, 0);
        public int id = -1;
        public byte game_id = 0;
        public SF3D.SceneSynchro.SceneNode node = null;

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

        public SFMapDecorationGroup()
        {
            weight.Initialize();
            dec_id.Initialize();
        }

        public void SetDecoration(int dec_index, ushort d_id, byte d_w = 255)
        {
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

        public SFMapDecoration AddDecoration(byte id, SFCoord position)
        {
            System.Diagnostics.Debug.Assert(id != 0, "SFMapDecorationManager.AddDecoration(): ID is 0!");

            SFMapDecoration dec = new SFMapDecoration();
            dec.grid_position = position;
            dec.game_id = id;
            decorations.Add(dec);

            dec.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeEmpty(map.heightmap.GetChunkNode(position), dec.GetObjectName());
            SetDecAssignment(position, id);

            SFMapDecorationGroup dec_group = dec_groups[id];
            for (int i = 0; i < 30; i++)
            {
                if (dec_group.dec_id[i] == 0)
                {
                    continue;
                }

                if (dec_group.weight[i] == 0)
                {
                    continue;
                }

                if ((MathUtils.Rand() % 255) >= (dec_group.weight[i] - 1))
                {
                    continue;
                }

                SF3D.SceneSynchro.SceneNode dec_node_i = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(dec_group.dec_id[i], i.ToString(), (Settings.ShadingQuality == 2), Settings.DecalShadows); ;
                dec_node_i.SetParent(dec.node);

                OpenTK.Vector2 offset = new OpenTK.Vector2(MathUtils.Randf(-0.4f, 0.4f), MathUtils.Randf(-0.4f, 0.4f));
                dec_node_i.Position = new OpenTK.Vector3(
                (position.x % SFMapHeightMapMesh.CHUNK_SIZE) + offset.X,
                map.heightmap.GetRealZ(new OpenTK.Vector2(position.x + offset.X, map.height - position.y - 1 - offset.Y)),
                ((map.height - position.y - 1) % SFMapHeightMapMesh.CHUNK_SIZE) - offset.Y);

                dec_node_i.SetAnglePlane(MathUtils.Rand() % 360);
                dec_node_i.Scale = new OpenTK.Vector3(100 / 128f);
            }

            // 3. add new unit in respective chunk
            map.heightmap.GetChunk(position).AddDecoration(dec);

            return dec;
        }

        public void RemoveDecoration(SFMapDecoration d)
        {
            decorations.Remove(d);
            SetDecAssignment(d.grid_position, 0);

            if (d.node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(d.node);
            }

            d.node = null;

            map.heightmap.GetChunk(d.grid_position).RemoveDecoration(d);
        }

        public void ReplaceDecoration(SFMapDecoration d, byte new_id)
        {
            System.Diagnostics.Debug.Assert(new_id != 0, "SFMapDecorationManager.ReplaceDecoration(): ID is 0!");

            if (d.node != null)
            {
                SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(d.node);
            }

            d.node = SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeEmpty(map.heightmap.GetChunkNode(d.grid_position), d.GetObjectName());

            SFMapDecorationGroup dec_group = dec_groups[new_id];
            for (int i = 0; i < 30; i++)
            {
                if (dec_group.dec_id[i] == 0)
                {
                    continue;
                }

                if (dec_group.weight[i] == 0)
                {
                    continue;
                }

                if ((MathUtils.Rand() % 255) > (dec_group.weight[i] - 1))
                {
                    continue;
                }

                SF3D.SceneSynchro.SceneNode dec_node_i = SF3D.SFRender.SFRenderEngine.scene.AddSceneObject(dec_group.dec_id[i], i.ToString(), (Settings.ShadingQuality == 2), Settings.DecalShadows); ;
                dec_node_i.SetParent(d.node);

                OpenTK.Vector2 offset = new OpenTK.Vector2(MathUtils.Randf(-0.4f, 0.4f), MathUtils.Randf(-0.4f, 0.4f));
                dec_node_i.Position = new OpenTK.Vector3(
                (d.grid_position.x % SFMapHeightMapMesh.CHUNK_SIZE) + offset.X,
                map.heightmap.GetRealZ(new OpenTK.Vector2(d.grid_position.x + offset.X, map.height - d.grid_position.y - 1 - offset.Y)),
                ((map.height - d.grid_position.y - 1) % SFMapHeightMapMesh.CHUNK_SIZE) - offset.Y);

                dec_node_i.SetAnglePlane(MathUtils.Rand() % 360);
                dec_node_i.Scale = new OpenTK.Vector3(100 / 128f);
            }

            d.game_id = (byte)new_id;
        }

        public byte GetDecAssignment(SFCoord pos)
        {
            return dec_assignment[pos.x * 1024 + pos.y];
        }

        public void SetDecAssignment(SFCoord pos, byte dec)
        {
            dec_assignment[pos.x * 1024 + pos.y] = dec;
            map.heightmap.SetFlag(pos, SFMapHeightMapFlag.ENTITY_DECAL, dec != 0);
        }

        public SFCoord GetDecPosition(int offset)
        {
            return new SFCoord(offset / 1024, (offset % 1024));
        }

        public void GenerateDecorations()
        {
            ushort size = (ushort)map.width;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    byte dec_a = dec_assignment[x * 1024 + y];
                    if (dec_a != 0)
                    {
                        SFCoord pos = new SFCoord(x, y);
                        AddDecoration(dec_a, pos);
                    }
                }
            }
        }

        // assumes positions are valid
        public void SetDecorationsToGroup(HashSet<SFCoord> pos, byte dec_type)
        {
            foreach (SFCoord p in pos)
            {
                if (map.heightmap.IsFlagSet(p, SFMapHeightMapFlag.ENTITY_DECAL))
                {
                    SFMapHeightMapChunk chunk = map.heightmap.GetChunk(p);
                    SFMapDecoration dec = null;
                    foreach (SFMapDecoration d in chunk.decorations)
                    {
                        if (d.grid_position == p)
                        {
                            dec = d;
                            break;
                        }
                    }

                    if (dec_type != 0)  // if chosen decoration id is not 0, simply replace
                    {
                        if (dec.id != dec_type)
                        {
                            ReplaceDecoration(dec, dec_type);
                        }
                    }
                    else               // else, remove (this happens if group 0 or empty group is chosen as well)
                    {
                        RemoveDecoration(dec);
                    }
                }
                else
                {
                    if (dec_type != 0)   // if chosen decoration id is not 0, add new decoration at position
                    {
                        AddDecoration(dec_type, p);
                    }
                }
            }
        }

        // updates decorations at positions
        // used when group changes its decal objects
        public void UpdateDecorationsOfGroup(byte dec_type)
        {
            if (dec_type == 0)
            {
                return;
            }

            ushort size = (ushort)map.width;
            foreach (var chunk_node in map.heightmap.chunk_nodes)
            {
                SFMapHeightMapChunk chunk = chunk_node.MapChunk;
                foreach (SFMapDecoration d in chunk.decorations)
                {
                    if (d.game_id != dec_type)
                    {
                        continue;
                    }

                    ReplaceDecoration(d, dec_type);
                }
            }
        }

        // sets decorations to given groups
        public void SetDecorations(Dictionary<SFCoord, byte> decals)
        {
            foreach (SFCoord p in decals.Keys)
            {
                if (map.heightmap.IsFlagSet(p, SFMapHeightMapFlag.ENTITY_DECAL))
                {
                    SFMapHeightMapChunk chunk = map.heightmap.GetChunk(p);
                    SFMapDecoration dec = null;
                    foreach (SFMapDecoration d in chunk.decorations)
                    {
                        if (d.grid_position == p)
                        {
                            dec = d;
                            break;
                        }
                    }

                    if (decals[p] != 0)  // if chosen decoration id is not 0, simply replace
                    {
                        if (dec.id != decals[p])
                        {
                            ReplaceDecoration(dec, decals[p]);
                        }
                    }
                    else               // else, remove (this happens if group 0 or empty group is chosen as well)
                    {
                        RemoveDecoration(dec);
                    }
                }
                else
                {
                    if (decals[p] != 0)   // if chosen decoration id is not 0, add new decoration at position
                    {
                        AddDecoration(decals[p], p);
                    }
                }
            }
        }
    }
}
