/*
 * SFModel3D is a resource which contains geometry and texture info for a 3D model
 * It also binds this info to a GPU buffer, and as such, it has to be disposed of manually
 * */

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
    public class SFSubModel3D: SFResource
    {
        public int submodel_id = Utility.NO_INDEX;
        public Vector3[] vertices = null;
        public Vector2[] uvs = null;
        public Vector4[] colors = null;
        public Vector3[] normals = null;
        public uint[] face_indices = null;

        public ArrayPool<Matrix4> instance_matrices = new ArrayPool<Matrix4>();
        //public bool needs_matrix_reload = false;

        public SFMaterial material = null;
        public Physics.BoundingBox aabb;
        public int vertex_array = Utility.NO_INDEX;
        public int vertex_buffer, uv_buffer, normal_buffer, color_buffer, element_buffer, instance_matrix_buffer;

        public void Init()
        {
            vertex_array = GL.GenVertexArray();
            vertex_buffer = GL.GenBuffer();
            uv_buffer = GL.GenBuffer();
            normal_buffer = GL.GenBuffer();
            color_buffer = GL.GenBuffer();
            element_buffer = GL.GenBuffer();
            instance_matrix_buffer = GL.GenBuffer();

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * 12, vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normal_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, normals.Length * 12, normals, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uv_buffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, uvs.Length * 8, uvs, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, color_buffer);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, colors.Length * 16, colors, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 0, 0);

            // matrix takes 4 registers
            GL.BindBuffer(BufferTarget.ArrayBuffer, instance_matrix_buffer);
            GL.BufferData<Matrix4>(BufferTarget.ArrayBuffer, instance_matrices.Count * 64, instance_matrices.GetData(), BufferUsageHint.StreamDraw);
            for (int i = 0; i < 4; i++)
            {
                GL.EnableVertexAttribArray(4 + i);
                GL.VertexAttribPointer(4 + i, 4, VertexAttribPointerType.Float, false, 64, 16 * i);
                GL.VertexAttribDivisor(4 + i, 1);
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, face_indices.Length * 4, face_indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        // ASSUMES VAO IS ALREADY BOUND!!
        public void ReloadInstanceMatrices()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, instance_matrix_buffer);
            GL.BufferData<Matrix4>(BufferTarget.ArrayBuffer, instance_matrices.Count * 64, instance_matrices.GetData(), BufferUsageHint.StreamDraw);
            //needs_matrix_reload = false;
        }

        public int Load(MemoryStream ms, object custom_data)
        {
            return 0;
        }

        public int Load(BinaryReader br)
        {
            int vcount, fcount;
            vcount = br.ReadInt32();
            fcount = br.ReadInt32();

            vertices = new Vector3[vcount]; normals = new Vector3[vcount]; uvs = new Vector2[vcount]; colors = new Vector4[vcount];
            face_indices = new uint[fcount * 3];
            material = new SFMaterial();

            for (int j = 0; j < vcount; j++)
            {
                Vector3 pos = new Vector3();
                pos.X = br.ReadSingle(); pos.Y = br.ReadSingle(); pos.Z = br.ReadSingle();
                Vector3 nor = new Vector3();
                nor.X = br.ReadSingle(); nor.Y = br.ReadSingle(); nor.Z = br.ReadSingle();
                Vector4 col = new Vector4();
                for (int k = 0; k < 4; k++)
                    col[k] = (float)((int)(br.ReadByte())) / 255.0f;
                Vector2 uv = new Vector2();
                uv.X = br.ReadSingle(); uv.Y = br.ReadSingle();
                br.ReadInt32();
                vertices[j] = pos; normals[j] = nor; uvs[j] = uv; colors[j] = col;
            }

            for (int j = 0; j < fcount; j++)
            {
                for (int k = 0; k < 3; k++)
                    face_indices[j * 3 + k] = br.ReadUInt16();
                br.ReadUInt16();
            }

            br.ReadUInt16();

            material = new SFMaterial();
            material.indexStart = (uint)0;
            material.indexCount = (uint)face_indices.Length;

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
            //System.Diagnostics.Debug.WriteLine(matname + " " + mat.ToString());

            SFTexture tex = null;
            int tex_code = SFResourceManager.Textures.Load(matname);
            if ((tex_code != 0) && (tex_code != -1))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFModel3D.Load(): Could not load texture (texture name = " + matname + ")");
                return tex_code;
            }
            tex = SFResourceManager.Textures.Get(matname);
            tex.FreeMemory();

            material.texture = tex;
            //System.Diagnostics.Debug.WriteLine(tex.ToString());

            // secondary material
            br.BaseStream.Position += 80;
            // colors
            br.BaseStream.Position += 12;
            // other unneeded data (chunk bbox)
            br.BaseStream.Position += 32;

            return 0;
        }

        public int CreateRaw(Vector3[] _vertices, Vector2[] _uvs, Vector4[] _colors, Vector3[] _normals, uint[] _indices, SFMaterial _material)
        {
            // reset first
            Dispose();
            instance_matrices = new ArrayPool<Matrix4>();

            vertices = _vertices;
            uvs = _uvs;
            colors = _colors;
            normals = _normals;
            face_indices = _indices;
            if (_material == null)
            {
                material = new SFMaterial();
                material.indexStart = 0;
                material.indexCount = (uint)face_indices.Length;
                material.texture = null;
            }
            else
                material = _material;

            return 0;
        }

        public void SetName(string s)
        {

        }

        public string GetName()
        {
            return "SUBMODEL " + submodel_id.ToString();
        }

        public int GetSizeBytes()
        {
            return 12 * vertices.Length
                 + 8 * uvs.Length
                 + 16 * colors.Length
                 + 12 * normals.Length
                 + 4 * face_indices.Length;
        }

        public void Dispose()
        {
            instance_matrices.Dispose();
            if (vertex_array != Utility.NO_INDEX)
            {
                GL.DeleteBuffer(vertex_buffer);
                GL.DeleteBuffer(normal_buffer);
                GL.DeleteBuffer(uv_buffer);
                GL.DeleteBuffer(color_buffer);
                GL.DeleteBuffer(element_buffer);
                GL.DeleteBuffer(instance_matrix_buffer);
                GL.DeleteVertexArray(vertex_array);
                vertex_array = Utility.NO_INDEX;
            }
            if ((material != null) && (material.texture != null))
                SFResourceManager.Textures.Dispose(material.texture.GetName());
        }
    }

    public class SFModel3D: SFResource
    {
        public SFSubModel3D[] submodels = null;
        public Physics.BoundingBox aabb;
        string name = "";

        public SFModel3D()
        {

        }

        public SFModel3D(MemoryStream ms)
        {
            Load(ms, null);
            Init();
        }

        public int Load(MemoryStream ms, object custom_data)
        {
            BinaryReader br = new BinaryReader(ms);

            ushort[] header = new ushort[4];
            for (int i = 0; i < 4; i++)
                header[i] = br.ReadUInt16();

            int modelnum = (int)header[1];

            submodels = new SFSubModel3D[modelnum];
            for(int i=0; i<modelnum; i++)
            {
                SFSubModel3D sbm = new SFSubModel3D();
                submodels[i] = sbm;
                sbm.submodel_id = i;
                int return_code = sbm.Load(br);
                if (return_code != 0)
                {
                    LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFModel3D.Load(): Could not load submodel (submodel ID = " + i.ToString() + ")");
                    return return_code;
                }
                if (i != modelnum - 1)
                    br.BaseStream.Position += 2;
            }
            
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

        public int CreateRaw(SFSubModel3D[] _submodels)
        {
            // reset first
            Dispose();

            submodels = _submodels;
            for (int i = 0; i < submodels.Length; i++)
                submodels[i].submodel_id = i;

            // aabb
            RecalculateBoundingBox();

            return 0;
        }

        public void Init()
        {
            foreach (SFSubModel3D sbm in submodels)
                sbm.Init();
        }

        public void RecalculateBoundingBox()
        {
            float x1, x2, y1, y2, z1, z2;
            x1 = 10000;
            x2 = -10000;
            y1 = 10000;
            y2 = -10000;
            z1 = 10000;
            z2 = -10000;
            foreach (SFSubModel3D sbm in submodels)
            {
                foreach (Vector3 v in sbm.vertices)
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
            if(submodels != null)
                foreach (SFSubModel3D submodel in submodels)
                    submodel.Dispose();
            submodels = null;
        }

        new public string ToString()
        {
            return "SUBMODEL COUNT: " + (submodels.Length).ToString();
        }
    }
}
