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
    public struct SFModelSkinMapBase
    {
        public int vbase;
        public int fbase;

        public SFModelSkinMapBase(int v, int f)
        {
            vbase = v;
            fbase = f;
        }

        public override string ToString()
        {
            return "VBASE " + vbase.ToString() + ", FBASE " + fbase.ToString();
        }
    }

    // for now, no memory cleanup/reuse, for testing purposes
    static public class SFModelSkinMap
    {
        static public List<Vector3> vertices = new List<Vector3>();
        static public List<Vector2> uvs = new List<Vector2>();
        static public List<Vector3> normals = new List<Vector3>();
        static public List<Vector4> bone_indices = new List<Vector4>();
        static public List<Vector4> bone_weights = new List<Vector4>();

        static public Vector3[] vertices_array = null;
        static public Vector2[] uvs_array = null;
        static public Vector3[] normals_array = null;
        static public Vector4[] bone_indices_array = null;
        static public Vector4[] bone_weights_array = null;

        static public List<uint> face_indices = new List<uint>();
        static public uint[] face_indices_array = null;

        static public int vertex_array { get; private set; } = -1;
        static public int vertex_buffer, uv_buffer, normal_buffer, bone_index_buffer, bone_weight_buffer, element_buffer;

        static private bool up_to_date = true;

        static public void Init()
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
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uv_buffer);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normal_buffer);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bone_index_buffer);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bone_weight_buffer);
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindVertexArray(0);
        }

        static public SFModelSkinMapBase AddNewSkinChunk(BinaryReader br, int vcount, int fcount)
        {
            SFModelSkinMapBase ret = new SFModelSkinMapBase(vertices.Count, face_indices.Count);

            for (int i = 0; i < vcount; i++)
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
                for (int k = 0; k < 4; k++)
                    v_bones[k] = (float)br.ReadByte();
                vertices.Add(position); normals.Add(normal); uvs.Add(uv); bone_indices.Add(v_bones); bone_weights.Add(weight);
            }
            
            for (int i = 0; i < fcount; i++)
            {
                face_indices.Add((uint)br.ReadUInt16());
                face_indices.Add((uint)br.ReadUInt16());
                face_indices.Add((uint)br.ReadUInt16());
                br.ReadUInt16();
            }

            up_to_date = false;
            System.Diagnostics.Debug.WriteLine("VERTICES " + vertices.Count.ToString() + " INDICES " + face_indices.Count.ToString());
            return ret;
        }

        static public int ConvertChunk(int chunk_id, SFModelSkinMapBase mapbase, int vcount, SFBoneIndex bsi)
        {
            for(int i = 0; i < vcount; i++)
            {
                Vector4 cur_bi = bone_indices[mapbase.vbase + i];
                Vector4 new_bi = new Vector4(bsi.bone_index_remap[chunk_id][(int)cur_bi.X],
                    bsi.bone_index_remap[chunk_id][(int)cur_bi.Y],
                    bsi.bone_index_remap[chunk_id][(int)cur_bi.Z],
                    bsi.bone_index_remap[chunk_id][(int)cur_bi.W]);
                bone_indices[mapbase.vbase + i] = new_bi;
            }

            return 0;
        }

        static public int RemoveSkin(int index)
        {
            return 0;
        }

        static public void Update()
        {
            if (up_to_date)
                return;

            if (vertices.Count == 0)
                return;

            vertices_array = vertices.ToArray();
            uvs_array = uvs.ToArray();
            normals_array = normals.ToArray();
            bone_indices_array = bone_indices.ToArray();
            bone_weights_array = bone_weights.ToArray();
            face_indices_array = face_indices.ToArray();

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Count * 12, vertices_array, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uv_buffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, uvs.Count * 8, uvs_array, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normal_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, normals.Count * 12, normals_array, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bone_index_buffer);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, bone_indices.Count * 16, bone_indices_array, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bone_weight_buffer);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, bone_weights.Count * 16, bone_weights_array, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, face_indices.Count * 4, face_indices_array, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);

            up_to_date = true;
        }

        static public void Clear()
        {
            if(vertex_array != -1)
            {
                GL.DeleteBuffer(vertex_buffer);
                GL.DeleteBuffer(normal_buffer);
                GL.DeleteBuffer(uv_buffer);
                GL.DeleteBuffer(bone_index_buffer);
                GL.DeleteBuffer(bone_weight_buffer);
                GL.DeleteBuffer(element_buffer);
                GL.DeleteVertexArray(vertex_buffer);
                vertex_array = -1;
            }

            vertices.Clear();
            normals.Clear();
            uvs.Clear();
            bone_indices.Clear();
            bone_weights.Clear();
            face_indices.Clear();

            vertices_array = null;
            uvs_array = null;
            normals_array = null;
            bone_indices_array = null;
            bone_weights_array = null;
            face_indices_array = null;
        }
    }

    public class SFModelSkinChunk: SFResource
    {
        public int chunk_id = -1;

        public SFModelSkinMapBase mapbase;
        public int base_vertex = 0;

        public SFMaterial material = null;
        public string bsi_name = ""+Utility.S_NONAME;

        public SFModelSkinChunk()
        {

        }

        public void Init()
        {

        }

        public int Load(MemoryStream ms)
        {
            BinaryReader br = new BinaryReader(ms);

            int vcount, fcount;
            vcount = br.ReadInt32();
            fcount = br.ReadInt32();

            base_vertex = SFModelSkinMap.vertices.Count;

            SFBoneIndex bsi = null;
            int bsi_code = SFResourceManager.BSIs.Load(SFResourceManager.current_resource);
            if ((bsi_code != 0) && (bsi_code != -1))
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFModelSkinChunk.Load(): Could not load bone skin index file (BSI name = " + SFResourceManager.current_resource + ")");
                return bsi_code;
            }
            bsi = SFResourceManager.BSIs.Get(SFResourceManager.current_resource);

            mapbase = SFModelSkinMap.AddNewSkinChunk(br, vcount, fcount);
            SFModelSkinMap.ConvertChunk(chunk_id, mapbase, vcount, bsi);
            
            SetName(SFResourceManager.current_resource);

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
            material.indexStart = (uint)mapbase.fbase;
            material.indexCount = (uint)(fcount*3);

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
            return 0;
            /*return 12 * vertices.Length
                 + 8 * uvs.Length
                 + 12 * normals.Length
                 + 16 * bone_indices.Length
                 + 16 * bone_weights.Length
                 + 4 * face_indices.Length
                 + 4 * bones.Length;*/
        }

        public void Dispose()
        {
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
