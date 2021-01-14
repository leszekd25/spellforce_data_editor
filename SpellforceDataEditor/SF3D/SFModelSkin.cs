/*
 * SFModelSkinChunk is a resource which contains info about a single animated part of a model
 * This info is geometry data, texturing data and bone weight data per vertex
 * It's unique in that it doesn't have a dedicated SFResourceContainer, instead it belongs to SFModelSkin object and is managed by it
 * It has to be disposed of, since it loads data to GPU on init
 * SFModelSkin is a resource which in reality is just a list of managed SFModelSkinChunk objects
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SpellforceDataEditor.SFResources;

namespace SpellforceDataEditor.SF3D
{
    public class SFModelSkinChunk: SFResource
    {
        public static MeshCache Cache;



        public int chunk_id = Utility.NO_INDEX;
        public int cache_index = Utility.NO_INDEX;

        byte[] vertex_data = null;     // 12+12+4+8+4 (empty) = 40 bytes per vertex
        public int vertex_size;

        uint[] face_indices = null;
        public int indices_size;

        public int[] bones;
        public SFMaterial material = null;
        public string bsi_name = ""+Utility.S_NONAME;

        public SFModelSkinChunk()
        {

        }

        public void Init()
        {
            cache_index = Cache.AddMesh(vertex_data, face_indices);
            vertex_size = vertex_data.Length;
            vertex_data = null;
            indices_size = face_indices.Length * 4;
            face_indices = null;
        }

        public int Load(MemoryStream ms, object custom_data)
        {
            BinaryReader br = new BinaryReader(ms);

            int vcount, fcount;
            vcount = br.ReadInt32();
            fcount = br.ReadInt32();

            vertex_data = br.ReadBytes(vcount * 40);
            /*
            vertices = new Vector3[vcount]; normals = new Vector3[vcount]; uvs = new Vector2[vcount];
            bone_indices = new Vector4[vcount]; bone_weights = new Vector4[vcount];

            for(int i = 0; i < vcount; i++)
            {
                Vector3 position = new Vector3();
                position.X = br.ReadSingle(); position.Y = br.ReadSingle(); position.Z = br.ReadSingle();
                Vector3 normal = new Vector3();
                normal.X = br.ReadSingle(); normal.Y = br.ReadSingle(); normal.Z = br.ReadSingle();
                Vector4 weight = new Vector4();
                for (int k = 0; k < 4; k++)
                    weight[k] = (float)((int)(br.ReadByte())) / 255.0f;
                Vector2 uv = new Vector2();
                uv.X = br.ReadSingle(); uv.Y = br.ReadSingle();
                Vector4 v_bones = new Vector4();
                for(int k = 0; k < 4; k++)
                    v_bones[k] = (float)br.ReadByte();
                vertices[i] = position; normals[i] = normal; uvs[i] = uv; bone_indices[i] = v_bones; bone_weights[i] = weight;
            }*/

            face_indices = new uint[fcount * 3];
            for(int i = 0; i < fcount; i++)
            {
                face_indices[i * 3] = (uint)br.ReadUInt16();
                face_indices[i * 3 + 1] = (uint)br.ReadUInt16();
                face_indices[i * 3 + 2] = (uint)br.ReadUInt16();
                br.ReadUInt16();
            }

            SFBoneIndex bsi = null;
            int bsi_code = SFResourceManager.BSIs.Load(SFResourceManager.current_resource);
            if ((bsi_code != 0)&&(bsi_code != -1))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFModelSkinChunk.Load(): Could not load bone skin index file (BSI name = " + SFResourceManager.current_resource + ")");
                return bsi_code;
            }
            bsi = SFResourceManager.BSIs.Get(SFResourceManager.current_resource);
            
            SetName(SFResourceManager.current_resource);
            bones = new int[bsi.bone_index_remap[chunk_id].Length];
            bsi.bone_index_remap[chunk_id].CopyTo(bones, 0);

            //load material
            br.ReadInt16();
            material = new SFMaterial();

            string matname;

            material._texID = br.ReadInt32();
            material.unused_uchar = br.ReadByte();
            material.uv_mode = br.ReadByte();
            material.unused_short2 = br.ReadUInt16();
            material.texRenderMode = (RenderMode)br.ReadByte();
            material.texAlpha = br.ReadByte();
            material.matFlags = br.ReadByte();
            material.matDepthBias = br.ReadByte();
            material.texTiling = br.ReadSingle();
            char[] chars = br.ReadChars(64);
            matname = new string(chars).ToLower();
            matname = matname.Substring(0, Math.Max(0, matname.IndexOf('\0')));

            SFTexture tex = null;
            int tex_code = SFResourceManager.Textures.Load(matname);
            if ((tex_code != 0) && (tex_code != -1))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFModelSkin.Load(): Could not load texture (texture name = " + matname + ")");
                tex = SFRender.SFRenderEngine.opaque_tex;
            }
            else
            {
                tex = SFResourceManager.Textures.Get(matname);
                tex.FreeMemory();
            }
            material.texture = tex;
            material.indexStart = 0;
            material.indexCount = (uint)face_indices.Length;

            br.BaseStream.Position += 126;

            return 0;
        }

        public void SetName(string s)
        {
            bsi_name = s;
        }

        public string GetName()
        {
            return bsi_name;
        }

        public int GetSizeBytes()
        {
            return vertex_size + indices_size;
        }

        public void Dispose()
        {
            if (cache_index != Utility.NO_INDEX)
            {
                Cache.RemoveMesh(cache_index);
                cache_index = Utility.NO_INDEX;
            }
            if ((material != null) && (material.texture != null) && (material.texture != SFRender.SFRenderEngine.opaque_tex))
                SFResourceManager.Textures.Dispose(material.texture.GetName());
        }
    }

    public class SFModelSkin: SFResource
    {
        public SFModelSkinChunk[] submodels { get; private set; } = null;
        string name = "";

        public int[][] bones = null;

        public void Init()
        {
            foreach(SFModelSkinChunk msc in submodels)
                msc.Init();
        }

        public int Load(MemoryStream ms, object custom_data)
        {
            BinaryReader br = new BinaryReader(ms);

            br.ReadInt16();
            int modelnum = br.ReadInt16();
            br.ReadInt32();

            submodels = new SFModelSkinChunk[modelnum];

            for(int i = 0; i < modelnum; i++)
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

            Merge();

            return 0;
        }

        public void Merge()
        {
            bones = new int[submodels.Length][];

            for(int i = 0; i < submodels.Length; i++)
            {
                bones[i] = submodels[i].bones;
            }
        }

        public void SetName(string s)
        {
            name = s;
        }

        public string GetName()
        {
            return name;
        }

        public int GetSizeBytes()
        {
            int size = 0;
            for (int i = 0; i < submodels.Length; i++)
                size += submodels[i].GetSizeBytes();
            return size + 4 * bones.Length;
        }

        public void Dispose()
        {
            foreach (var msc in submodels)
                msc.Dispose();
            SFResourceManager.BSIs.Dispose(GetName());
        }
    }
}
