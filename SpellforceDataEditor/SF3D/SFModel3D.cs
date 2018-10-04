using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SF3D
{
    public class SFModel3D: SFResource
    {
        public Vector3[] vertices = null;
        public Vector2[] uvs = null;
        public Vector4[] colors = null;
        public Vector3[] normals = null;
        public uint[] face_indices = null;
        public SFMaterial[] materials = null;
        public int vertex_array = -1;
        public int vertex_buffer, uv_buffer, color_buffer, normal_buffer, element_buffer;

        public SFModel3D()
        {

        }

        public SFModel3D(MemoryStream ms, SFResourceManager rm)
        {
            Load(ms, rm);
            Init();
        }

        public int Load(MemoryStream ms, SFResourceManager rm)
        {
            BinaryReader br = new BinaryReader(ms);

            ushort[] header = new ushort[4];
            for (int i = 0; i < 4; i++)
                header[i] = br.ReadUInt16();

            int modelnum = (int)header[1];
            int total_v = 0;
            int total_f = 0;

            Vector3[][] vbuf = new Vector3[modelnum][];
            Vector3[][] nbuf = new Vector3[modelnum][];
            Vector4[][] cbuf = new Vector4[modelnum][];
            Vector2[][] ubuf = new Vector2[modelnum][];

            uint[][] ebuf = new uint[modelnum][];

            materials = new SFMaterial[modelnum];

            for(int i = 0; i < modelnum; i++)
            {
                int cur_v, cur_f;
                cur_v = br.ReadInt32();
                cur_f = br.ReadInt32();

                vbuf[i] = new Vector3[cur_v]; nbuf[i] = new Vector3[cur_v]; cbuf[i] = new Vector4[cur_v]; ubuf[i] = new Vector2[cur_v];
                ebuf[i] = new uint[cur_f * 3];

                for(int j = 0; j < cur_v; j++)
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
                    vbuf[i][j] = pos; nbuf[i][j] = nor; cbuf[i][j] = col; ubuf[i][j] = uv;
                }

                for (int j = 0; j < cur_f; j++)
                {
                    for (int k = 0; k < 3; k++)
                        ebuf[i][j * 3 + k] = br.ReadUInt16();
                    br.ReadUInt16();
                }

                SFMaterial mat = new SFMaterial();
                mat.indexStart = (uint)total_f * 3;
                mat.indexCount = (uint)cur_f * 3;

                total_f += cur_f;
                total_v += cur_v;

                br.ReadUInt16();

                int texID;
                byte unused_uchar, uv_mode;
                ushort unused_short2;
                byte texRenderMode, texAlpha, matFlags, matDepthBias;
                float texTiling;
                string matname;

                texID = br.ReadInt32();
                unused_uchar = br.ReadByte();
                uv_mode = br.ReadByte();
                unused_short2 = br.ReadUInt16();
                texRenderMode = br.ReadByte();
                texAlpha = br.ReadByte();
                matFlags = br.ReadByte();
                matDepthBias = br.ReadByte();
                texTiling = br.ReadSingle();
                char[] chars = br.ReadChars(64);
                matname = new string(chars);
                matname = matname.Substring(0, Math.Max(0, matname.IndexOf('\0')));

                SFTexture tex = rm.Textures.Get(matname);
                if(tex == null)
                {
                    int tex_code = rm.Textures.Load(matname);
                    if (tex_code != 0)
                        return tex_code;
                    tex = rm.Textures.Get(matname);
                }
                mat.texture = tex;

                materials[i] = mat;
                br.BaseStream.Position += 126;
            }
            //join tables
            vertices = new Vector3[total_v]; normals = new Vector3[total_v]; colors = new Vector4[total_v]; uvs = new Vector2[total_v];
            face_indices = new uint[total_f * 3];
            total_v = 0; total_f = 0;
            for(int i = 0; i < modelnum; i++)
            {
                for(int j = 0; j < vbuf[i].Length; j++)
                {
                    vertices[total_v + j] = vbuf[i][j];
                    normals[total_v + j] = nbuf[i][j];
                    colors[total_v + j] = cbuf[i][j];
                    uvs[total_v + j] = ubuf[i][j];
                }
                for (int j = 0; j < ebuf[i].Length; j++)
                {
                    face_indices[total_f + j] = ebuf[i][j]+(uint)total_v;
                }

                total_v += vbuf[i].Length;
                total_f += ebuf[i].Length;
            }

            
            return 0;
        }

        public void Init()
        {
            vertex_array = GL.GenVertexArray();
            vertex_buffer = GL.GenBuffer();
            uv_buffer = GL.GenBuffer();
            color_buffer = GL.GenBuffer();
            element_buffer = GL.GenBuffer();

            GL.BindVertexArray(vertex_array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length*12, vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, color_buffer);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, colors.Length*16, colors, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, uv_buffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, uvs.Length * 8, uvs, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, face_indices.Length * 4, face_indices, BufferUsageHint.StaticDraw);
            
        }

        public void Dispose()
        {
            if(vertex_array != -1)
            {
                GL.DeleteBuffer(vertex_buffer);
                //GL.DeleteBuffer(normal_buffer);
                GL.DeleteBuffer(color_buffer);
                GL.DeleteBuffer(uv_buffer);
                GL.DeleteBuffer(element_buffer);
                GL.DeleteVertexArray(vertex_array);
                vertex_array = -1;
            }
        }

        new public string ToString()
        {
            return "VCOUNT " + vertices.Length.ToString() + " FCOUNT " + (face_indices.Length / 3).ToString();
        }
    }
}
