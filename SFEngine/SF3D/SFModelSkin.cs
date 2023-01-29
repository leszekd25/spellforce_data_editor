/*
 * SFModelSkinChunk is a resource which contains info about a single animated part of a model
 * This info is geometry data, texturing data and bone weight data per vertex
 * It's unique in that it doesn't have a dedicated SFResourceContainer, instead it belongs to SFModelSkin object and is managed by it
 * It has to be disposed of, since it loads data to GPU on init
 * SFModelSkin is a resource which in reality is just a list of managed SFModelSkinChunk objects
 */

using SFEngine.SFResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SFEngine.SF3D
{
    public class SFModelSkinChunk// : SFResource
    {
        public static MeshCache Cache;

        public int chunk_id = Utility.NO_INDEX;
        public int cache_index = Utility.NO_INDEX;

        public byte[] vertex_data = null;     // 12+12+4+8+4 (empty) = 40 bytes per vertex
        public int vertex_size;

        public uint[] face_indices = null;
        public int indices_size;

        public SFMaterial material = null;
        public string bsi_name = "" + Utility.S_NONAME;
        public int RAMSize { get; set; }
        public int DeviceSize { get; set; }

        public SFModelSkinChunk()
        {

        }

        public void Init()
        {
            cache_index = Cache.AddMesh(vertex_data, 0, vertex_data.Length, face_indices);
            vertex_size = vertex_data.Length;
            vertex_data = null;
            indices_size = face_indices.Length * 4;
            face_indices = null;

            DeviceSize = vertex_size + indices_size;
            RAMSize = 0;
        }

        public int Load(MemoryStream ms, object custom_data)
        {
            BinaryReader br = new BinaryReader(ms);

            int vcount, fcount;
            vcount = br.ReadInt32();
            fcount = br.ReadInt32();

            vertex_data = br.ReadBytes(vcount * 40);

            face_indices = new uint[fcount * 3];
            for (int i = 0; i < fcount; i++)
            {
                face_indices[i * 3] = (uint)br.ReadUInt16();
                face_indices[i * 3 + 1] = (uint)br.ReadUInt16();
                face_indices[i * 3 + 2] = (uint)br.ReadUInt16();
                br.ReadUInt16();
            }

            //load material
            br.ReadInt16();
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
            material.additive_pass = ((material.texRenderMode == RenderMode.ONE_ONE)||(material.texRenderMode == RenderMode.DESTCOLOR_ONE));
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

            SFTexture tex = null;
            int tex_code = SFResourceManager.Textures.Load(matname, SFUnPak.FileSource.ANY);
            if ((tex_code != 0) && (tex_code != -1))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFModelSkin.Load(): Could not load texture (texture name = " + matname + ")");
                tex = SFRender.SFRenderEngine.opaque_tex;
            }
            else
            {
                tex = SFResourceManager.Textures.Get(matname);
            }
            material.texture = tex;

            br.BaseStream.Position += 126;

            RAMSize = vertex_data.Length + face_indices.Length * 4;
            DeviceSize = 0;

            return 0;
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
            if ((material != null) && (material.texture != null) && (material.texture != SFRender.SFRenderEngine.opaque_tex))
            {
                SFResourceManager.Textures.Dispose(material.texture.Name);
            }
        }
    }

    public class SFModelSkin : SFResource
    {
        public SFModelSkinChunk[] submodels { get; private set; } = null;

        public override void Init()
        {
            RAMSize = 0;
            DeviceSize = 0;

            foreach (SFModelSkinChunk msc in submodels)
            {
                msc.Init();
                RAMSize += msc.RAMSize;
                DeviceSize += msc.DeviceSize;
            }
        }

        public override int Load(byte[] data, int offset, object custom_data)
        {
            MemoryStream ms = new MemoryStream(data, offset, data.Length - offset);
            // load BSI

            SFBoneIndex bsi = null;
            int bsi_code = SFResourceManager.BSIs.Load(SFResourceManager.current_resource, SFUnPak.FileSource.ANY);
            if ((bsi_code != 0) && (bsi_code != -1))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFModelSkinChunk.Load(): Could not load bone skin index file (BSI name = " + SFResourceManager.current_resource + ")");
                return bsi_code;
            }
            bsi = SFResourceManager.BSIs.Get(SFResourceManager.current_resource);


            BinaryReader br = new BinaryReader(ms);

            br.ReadInt16();
            int modelnum = br.ReadInt16();
            br.ReadInt32();

            submodels = new SFModelSkinChunk[modelnum];

            for (int i = 0; i < modelnum; i++)
            {
                SFModelSkinChunk chunk = new SFModelSkinChunk();
                submodels[i] = chunk;
                chunk.chunk_id = i;
                int return_code = chunk.Load(ms, null);
                if (return_code != 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFModelSkin.Load(): Could not load skin chunk (chunk ID = " + i.ToString() + ")");
                    return return_code;
                }
            }

            try
            {
                Merge(bsi);
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFModelSkin.Load(): Invalid skin data!");
                SFResourceManager.BSIs.Dispose(SFResourceManager.current_resource);
                Dispose();
                return -2;
            }

            SFResourceManager.BSIs.Dispose(SFResourceManager.current_resource);

            foreach(SFModelSkinChunk msc in submodels)
            {
                RAMSize += msc.RAMSize;
                DeviceSize += msc.DeviceSize;
            }

            return 0;
        }

        public void Merge(SFBoneIndex bsi)
        {
            // remap bones using BSI
            for (int i = 0; i < submodels.Length; i++)
            {
                var msc = submodels[i];
                for (int j = 0; j < msc.vertex_data.Length; j += 40)
                {
                    msc.vertex_data[j + 36 + 0] = (byte)bsi.bone_index_remap[i][msc.vertex_data[j + 36 + 0]];
                    msc.vertex_data[j + 36 + 1] = (byte)bsi.bone_index_remap[i][msc.vertex_data[j + 36 + 1]];
                    msc.vertex_data[j + 36 + 2] = (byte)bsi.bone_index_remap[i][msc.vertex_data[j + 36 + 2]];
                    msc.vertex_data[j + 36 + 3] = (byte)bsi.bone_index_remap[i][msc.vertex_data[j + 36 + 3]];
                }
            }

            // merge similar submodels
            // assumes materials with the same texture have the same material

            // find chunks per material
            SFModelSkinChunk[] merged_chunks = null;

            List<SFTexture> tex_list = new List<SFTexture>();
            List<List<int>> tex_chunks_list = new List<List<int>>();
            foreach (var msc in submodels)
            {
                if (!tex_list.Contains(msc.material.texture))
                {
                    tex_list.Add(msc.material.texture);
                    tex_chunks_list.Add(new List<int>());

                }

                tex_chunks_list[tex_list.IndexOf(msc.material.texture)].Add(msc.chunk_id);
            }

            merged_chunks = new SFModelSkinChunk[tex_list.Count];

            // merge chunks sharing the same material
            for (int i = 0; i < tex_list.Count; i++)
            {
                SFModelSkinChunk msc = new SFModelSkinChunk();
                merged_chunks[i] = msc;
                msc.chunk_id = i;

                // get total vertex data size and total element data size
                int vlen = 0;
                int elen = 0;
                foreach (var k in tex_chunks_list[i])
                {
                    vlen += submodels[k].vertex_data.Length;
                    elen += submodels[k].face_indices.Length;
                }

                msc.vertex_data = new byte[vlen];
                msc.face_indices = new uint[elen];

                uint vcount = 0;
                uint fcount = 0;
                for (int j = 0; j < tex_chunks_list[i].Count; j++)
                {
                    var msc2 = submodels[tex_chunks_list[i][j]];
                    for (int l = 0; l < msc2.face_indices.Length; l++)
                    {
                        msc2.face_indices[l] += vcount;
                    }

                    Array.Copy(msc2.vertex_data, 0, msc.vertex_data, vcount * 40, msc2.vertex_data.Length);
                    Array.Copy(msc2.face_indices, 0, msc.face_indices, fcount, msc2.face_indices.Length);

                    vcount += (uint)(msc2.vertex_data.Length / 40);
                    fcount += (uint)(msc2.face_indices.Length);
                }

                msc.material = submodels[tex_chunks_list[i][0]].material;
                SFResourceManager.Textures.Load(msc.material.texture.Name, SFUnPak.FileSource.ANY);    // to increase ref counter
            }

            foreach(var msc in submodels)
            {
                SFResourceManager.Textures.Dispose(msc.material.texture.Name);  // to decrease ref counter
            }
            submodels = merged_chunks;
        }

        public override void Dispose()
        {
            if (submodels != null)
            {
                foreach (var msc in submodels)
                {
                    msc.Dispose();
                }
            }

            submodels = null;

            RAMSize = 0;
            DeviceSize = 0;
        }
    }
}
