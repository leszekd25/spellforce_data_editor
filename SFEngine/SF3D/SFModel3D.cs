/*
 * SFModel3D is a resource which contains geometry and texture info for a 3D model
 * It also binds this info to a GPU buffer, and as such, it has to be disposed of manually
 * */

using OpenTK;
using SFEngine.SFResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SFEngine.SF3D
{
    public class SFSubModel3D// : SFResource
    {
        public static MeshCache Cache;

        public SFModel3D owner = null;
        public int submodel_id = Utility.NO_INDEX;
        public int cache_index = Utility.NO_INDEX;

        byte[] vertex_data = null;     // 12+12+4+8+4 (empty) = 40 bytes per vertex
        public int vertex_data_offset;
        public int vertex_data_size;
        uint[] face_indices = null;
        public int indices_size;

        public SFMaterial material = null;
        public Physics.BoundingBox aabb;

        public int RAMSize { get; set; }
        public int DeviceSize { get; set; }

        public void Init()
        {
            cache_index = Cache.AddMesh(vertex_data, vertex_data_offset, vertex_data_size, face_indices);
            vertex_data = null;
            face_indices = null;

            DeviceSize = vertex_data_size + indices_size;
            RAMSize = 0;
        }

        public int Load(BinaryReader br, byte[] data, int offset)
        {
            int vcount, fcount;
            vcount = br.ReadInt32();
            fcount = br.ReadInt32();

            if (vcount == 0)
            {
                br.BaseStream.Position += 206;
                return 1;
            }

            // vertex_data taken directly from file: vertices, normals, colors, uvs
            vertex_data = data;
            vertex_data_size = vcount * 40;
            vertex_data_offset = (int)(offset + br.BaseStream.Position);
            br.BaseStream.Position += vertex_data_size;

            face_indices = new uint[fcount * 3];
            unsafe
            {
                fixed(byte* pdata = data)
                {
                    int face_offset = vertex_data_offset + vertex_data_size;
                    for (int j = 0; j < fcount; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            face_indices[j * 3 + k] = (uint)(*(ushort*)(pdata + face_offset + (j * 4 + k) * 2));
                        }
                    }
                }
            }
            indices_size = face_indices.Length * 4;
            br.BaseStream.Position += fcount * 8;

            br.ReadUInt16();

            material = new SFMaterial();

            string matname;
            Encoding enc = Encoding.ASCII;

            material._texID = br.ReadInt32();
            material.unused_uchar = br.ReadByte();
            material.uv_mode = br.ReadByte();
            material.unused_short2 = br.ReadUInt16();
            material.texRenderMode = (RenderMode)br.ReadByte();
            material.texAlpha = br.ReadByte();
            material.matFlags = br.ReadByte();
            material.matDepthBias = br.ReadByte() * -0.000003f;
            material.texTiling = br.ReadSingle();
            material.transparent_pass = ((material.matFlags & 4) == 0);
            material.additive_pass = (material.texRenderMode == RenderMode.ONE_ONE);
            if (material.additive_pass)
            {
                if (Settings.ToneMapping)
                {
                    material.emission_strength = 1.0f;
                }
            }
            byte[] chars = br.ReadBytes(64);
            matname = enc.GetString(chars);
            matname = matname.Substring(0, Math.Max(0, matname.IndexOf('\0')));
            matname = matname.ToLower();

            if(!SFResourceManager.Textures.Load(matname, SFUnPak.FileSource.ANY, out material.texture, out int ec))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFModel3D.Load(): Could not load texture (texture name = " + matname + ")");
                material.texture = SFRender.SFRenderEngine.opaque_tex;
            }

            // secondary material
            br.BaseStream.Position += 80;
            // colors
            br.BaseStream.Position += 12;
            // other unneeded data (chunk bbox)
            br.BaseStream.Position += 32;

            RAMSize = vertex_data.Length + face_indices.Length * 4;
            DeviceSize = 0;

            return 0;
        }

        public int CreateRaw(Vector3[] _vertices, Vector2[] _uvs, byte[] _colors, Vector3[] _normals, uint[] _indices, SFMaterial _material)
        {
            // reset first
            Dispose();

            vertex_data = new byte[_vertices.Length * 40];
            unsafe
            {
                fixed (byte* vertex_data_pt = vertex_data)
                {
                    fixed (byte* colors_pt = _colors)
                    {
                        for (int i = 0; i < _vertices.Length; i++)
                        {
                            *(Vector3*)(vertex_data_pt + (i * 40 + 0)) = _vertices[i];
                            *(Vector3*)(vertex_data_pt + (i * 40 + 12)) = _normals[i];
                            *(uint*)(vertex_data_pt + (i * 40 + 24)) = *(uint*)(colors_pt + (4 * i));
                            *(Vector2*)(vertex_data_pt + (i * 40 + 28)) = _uvs[i];
                            *(uint*)(vertex_data_pt + (i * 40 + 36)) = 0;
                        }
                    }
                }
            }
            vertex_data_offset = 0;
            vertex_data_size = vertex_data.Length;

            face_indices = _indices;
            if (_material == null)
            {
                material = new SFMaterial();
                material.texture = SFRender.SFRenderEngine.opaque_tex;
            }
            else
            {
                material = _material;
            }

            RAMSize = vertex_data.Length + face_indices.Length * 4;
            DeviceSize = 0;

            return 0;
        }

        public IEnumerable<Vector3> GetVertices()
        {
            using (MemoryStream ms = new MemoryStream(vertex_data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    for (int i = 0; i < vertex_data.Length; i += 40)
                    {
                        br.BaseStream.Position = i;
                        float x = br.ReadSingle();
                        float y = br.ReadSingle();
                        float z = br.ReadSingle();
                        yield return new Vector3(x, y, z);
                    }
                }
            }
        }

        public IEnumerable<Vector3> GetNormals()
        {
            using (MemoryStream ms = new MemoryStream(vertex_data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    for (int i = 0; i < vertex_data.Length; i += 40)
                    {
                        br.BaseStream.Position = i + 12;
                        float x = br.ReadSingle();
                        float y = br.ReadSingle();
                        float z = br.ReadSingle();
                        yield return new Vector3(x, y, z);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (cache_index != Utility.NO_INDEX)
            {
                Cache.RemoveMesh(cache_index);
                cache_index = Utility.NO_INDEX;

                RAMSize = 0;
                DeviceSize = 0;
            }
            if ((material != null) && (material.texture != SFRender.SFRenderEngine.opaque_tex))
            {
                SFResourceManager.Textures.Dispose(material.texture);
                material = null;
            }

            owner = null;
        }
    }

    public class SFModel3D : SFResource
    {
        public SFSubModel3D[] submodels = null;
        public Physics.BoundingBox aabb;

        // instance matrix data, from MeshCache
        public int CurrentMatrixIndex = 0;       // reset every frame
        public int MatrixOffset = -1;             // offset into MeshCache.MatrixData array  
        public int MatrixCount = 0;              // number of matrices used; CurrentMatrixIndex stops here
        public bool ForceUpdateInstanceMatrices = true;  // if true, each instance of this mesh will update its transform on next frame


        public override int Load(byte[] data, int offset, object custom_data)
        {
            MemoryStream ms = new MemoryStream(data, offset, data.Length - offset);
            BinaryReader br = new BinaryReader(ms);

            ushort[] header = new ushort[4];
            for (int i = 0; i < 4; i++)
            {
                header[i] = br.ReadUInt16();
            }

            int modelnum = (int)header[1];

            List<SFSubModel3D> tmp_submodels = new List<SFSubModel3D>();
            int failed_submodels = 0;
            for (int i = 0; i < modelnum; i++)
            {
                SFSubModel3D sbm = new SFSubModel3D();
                int return_code = sbm.Load(br, data, offset);
                if (return_code == 1)
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFModel3D.Load(): Submodel ID " + i.ToString() + " was empty, skipping");
                    failed_submodels += 1;
                }
                else if (return_code != 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFModel3D.Load(): Could not load submodel (submodel ID = " + i.ToString() + ")");
                    return return_code;
                }
                else
                {
                    tmp_submodels.Add(sbm);
                    sbm.submodel_id = i - failed_submodels;
                    sbm.owner = this;
                }

                if (i != modelnum - 1)
                {
                    br.BaseStream.Position += 2;
                }

                RAMSize += sbm.RAMSize;
                DeviceSize += sbm.DeviceSize;
            }
            submodels = tmp_submodels.ToArray();

            // bbox
            float x1, x2, y1, y2, z1, z2;
            x1 = br.ReadSingle();
            y1 = br.ReadSingle();
            z1 = br.ReadSingle();
            x2 = br.ReadSingle();
            y2 = br.ReadSingle();
            z2 = br.ReadSingle();
            aabb = new Physics.BoundingBox(new Vector3(x1, y1, z1), new Vector3(x2, y2, z2));

            return 0;
        }

        public int CreateRaw(SFSubModel3D[] _submodels, bool skip_recalc_on_not_null = false)
        {
            // reset first
            Dispose();

            submodels = _submodels;
            for (int i = 0; i < submodels.Length; i++)
            {
                submodels[i].submodel_id = i;
                submodels[i].owner = this;
            }

            // aabb
            RecalculateBoundingBox(skip_recalc_on_not_null);

            return 0;
        }

        public override void Init()
        {
            RAMSize = 0;
            DeviceSize = 0;
            foreach (SFSubModel3D sbm in submodels)
            {
                sbm.Init();
                RAMSize += sbm.RAMSize;
                DeviceSize += sbm.DeviceSize;
            }
        }

        public void RecalculateBoundingBox(bool skip_on_not_null = false)
        {
            if ((skip_on_not_null) && (aabb != Physics.BoundingBox.Zero))
            {
                return;
            }

            // this homunculus here will have to stay for a while longer
            float x1, x2, y1, y2, z1, z2;
            x1 = 10000;
            x2 = -10000;
            y1 = 10000;
            y2 = -10000;
            z1 = 10000;
            z2 = -10000;
            foreach (SFSubModel3D sbm in submodels)
            {
                foreach (Vector3 v in sbm.GetVertices())
                {
                    x1 = Math.Min(x1, v.X);
                    x2 = Math.Max(x2, v.X);
                    y1 = Math.Min(y1, v.Y);
                    y2 = Math.Max(y2, v.Y);
                    z1 = Math.Min(z1, v.Z);
                    z2 = Math.Max(z2, v.Z);
                }
            }
            aabb = new Physics.BoundingBox(new Vector3(x1, y1, z1), new Vector3(x2, y2, z2));
        }

        public override void Dispose()
        {
            if (submodels != null)
            {
                foreach (SFSubModel3D submodel in submodels)
                {
                    submodel.Dispose();
                }
            }

            submodels = null;

            RAMSize = 0;
            DeviceSize = 0;
        }

        public override string ToString()
        {
            return "SUBMODEL COUNT: " + (submodels.Length).ToString();
        }
    }
}
