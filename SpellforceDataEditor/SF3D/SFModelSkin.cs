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
        public int chunk_id = -1;

        public Vector3[] vertices = null;
        public Vector2[] uvs = null;
        public Vector3[] normals = null;
        public Vector4[] bone_indices = null;
        public Vector4[] bone_weights = null;

        public uint[] face_indices = null;

        public SFMaterial material = null;
        public int[] bones = null;
        public string bsi_name = ""+Utility.S_NONAME;

        public int vertex_array = -1;
        public int vertex_buffer, uv_buffer, normal_buffer, bone_index_buffer, bone_weight_buffer, element_buffer;

        public SFModelSkinChunk()
        {

        }

        public void Init()
        {

            vertex_array = GL.GenVertexArray();
            vertex_buffer = GL.GenBuffer();
            normal_buffer = GL.GenBuffer();
            uv_buffer = GL.GenBuffer();
            bone_index_buffer = GL.GenBuffer();
            bone_weight_buffer = GL.GenBuffer();
            element_buffer = GL.GenBuffer();

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uv_buffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, uvs.Length * 8, uvs, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normal_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, normals.Length * 12, normals, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bone_index_buffer);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, bone_indices.Length * 16, bone_indices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bone_weight_buffer);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, bone_weights.Length * 16, bone_weights, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, face_indices.Length * 4, face_indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        public int Load(MemoryStream ms)
        {
            BinaryReader br = new BinaryReader(ms);

            int vcount, fcount;
            vcount = br.ReadInt32();
            fcount = br.ReadInt32();
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
            }

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
            if ((tex_code != 0)&&(tex_code != -1))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFModelSkinChunk.Load(): Could not load texture (texture name = " + matname + ")");
                return tex_code;
            }
            tex = SFResourceManager.Textures.Get(matname);
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
            return 12 * vertices.Length
                 + 8 * uvs.Length
                 + 12 * normals.Length
                 + 16 * bone_indices.Length
                 + 16 * bone_weights.Length
                 + 4 * face_indices.Length
                 + 4 * bones.Length;
        }

        public void Dispose()
        {

            if (vertex_array != -1)
            {
                GL.DeleteBuffer(vertex_buffer);
                GL.DeleteBuffer(normal_buffer);
                GL.DeleteBuffer(uv_buffer);
                GL.DeleteBuffer(bone_index_buffer);
                GL.DeleteBuffer(bone_weight_buffer);
                GL.DeleteBuffer(element_buffer);
                GL.DeleteVertexArray(vertex_array);
                vertex_array = -1;
            }
            if((material != null)&&(material.texture != null))
                SFResourceManager.Textures.Dispose(material.texture.GetName());
            SFResourceManager.BSIs.Dispose(GetName());
        }
    }

    public class SFModelSkin: SFResource
    {
        public SFModelSkinChunk[] submodels { get; private set; } = null;
        string name = "";

        public void Init()
        {
            foreach (SFModelSkinChunk submodel in submodels)
                submodel.Init();
        }

        public int Load(MemoryStream ms)
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
                int return_code = chunk.Load(ms);
                if (return_code != 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFModelSkin.Load(): Could not load skin chunk (chunk ID = " + i.ToString() + ")");
                    return return_code;
                }
            }
            return 0;
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
            if (submodels == null)
                return 0;

            int ret = 0;
            for (int i = 0; i < submodels.Length; i++)
                ret += submodels[i].GetSizeBytes();
            return ret;
        }

        public void Dispose()
        {
            if(submodels!=null)
                foreach (SFModelSkinChunk submodel in submodels)
                 submodel.Dispose();
            submodels = null;
        }
    }
}
