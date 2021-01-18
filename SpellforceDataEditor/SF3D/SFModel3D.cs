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
        public static MeshCache Cache;



        public int submodel_id = Utility.NO_INDEX;
        public int cache_index = Utility.NO_INDEX;

        byte[] vertex_data = null;     // 12+12+4+8+4 (empty) = 40 bytes per vertex
        public int vertex_size;
        uint[] face_indices = null;
        public int indices_size;

        //public ArrayPool<Matrix4> instance_matrices = new ArrayPool<Matrix4>();
        //public bool needs_matrix_reload = false;

        public SFMaterial material = null;
        public Physics.BoundingBox aabb;
        /*public int vertex_array = Utility.NO_INDEX;
        public int vertex_buffer, element_buffer;//, instance_matrix_buffer;*/

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
            return 0;
        }

        public int Load(BinaryReader br)
        {
            int vcount, fcount;
            vcount = br.ReadInt32();
            fcount = br.ReadInt32();

            if(vcount == 0)
            {
                br.BaseStream.Position += 206;
                return 1;
            }

            // vertex_data taken directly from file: vertices, normals, colors, uvs
            vertex_data = br.ReadBytes(vcount * 40);

            face_indices = new uint[fcount * 3];

            for (int j = 0; j < fcount; j++)
            {
                for (int k = 0; k < 3; k++)
                    face_indices[j * 3 + k] = br.ReadUInt16();
                br.ReadUInt16();
            }

            br.ReadUInt16();

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
            //System.Diagnostics.Debug.WriteLine(matname + " " + mat.ToString());

            SFTexture tex = null;
            int tex_code = SFResourceManager.Textures.Load(matname);
            if ((tex_code != 0) && (tex_code != -1))
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFModel3D.Load(): Could not load texture (texture name = " + matname + ")");
                tex = SFRender.SFRenderEngine.opaque_tex;
            }
            else
            {
                tex = SFResourceManager.Textures.Get(matname);
                tex.FreeMemory();
            }

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

        public int CreateRaw(Vector3[] _vertices, Vector2[] _uvs, byte[] _colors, Vector3[] _normals, uint[] _indices, SFMaterial _material)
        {
            // reset first
            Dispose();
            //instance_matrices = new ArrayPool<Matrix4>();

            vertex_data = new byte[_vertices.Length * 40];
            using (MemoryStream ms = new MemoryStream(vertex_data))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for(int i = 0; i < _vertices.Length; i++)
                    {
                        bw.Write(_vertices[i].X);
                        bw.Write(_vertices[i].Y);
                        bw.Write(_vertices[i].Z);
                        bw.Write(_normals[i].X);
                        bw.Write(_normals[i].Y);
                        bw.Write(_normals[i].Z);
                        for (int j = 0; j < 4; j++)
                            bw.Write(_colors[4 * i + j]);
                        bw.Write(_uvs[i].X);
                        bw.Write(_uvs[i].Y);
                        bw.Write((int)0);
                    }
                }
            }

            face_indices = _indices;
            if (_material == null)
            {
                material = new SFMaterial();
                material.texture = SFRender.SFRenderEngine.opaque_tex;
            }
            else
                material = _material;

            return 0;
        }

        public IEnumerable<Vector3> GetVertices()
        {
            using (MemoryStream ms = new MemoryStream(vertex_data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    for (int i = 0; i < vertex_data.Length; i+= 40)
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

        public void SetName(string s)
        {

        }

        public string GetName()
        {
            return "SUBMODEL " + submodel_id.ToString();
        }

        public int GetSizeBytes()
        {
            return vertex_size + indices_size;
        }

        public void Dispose()
        {
            if(cache_index != Utility.NO_INDEX)
            {
                Cache.RemoveMesh(cache_index);
                cache_index = Utility.NO_INDEX;
            }
            //instance_matrices.Dispose();
            /*if (vertex_array != Utility.NO_INDEX)
            {
                GL.DeleteBuffer(vertex_buffer);
                GL.DeleteBuffer(element_buffer);
                //GL.DeleteBuffer(instance_matrix_buffer);
                GL.DeleteVertexArray(vertex_array);
                vertex_array = Utility.NO_INDEX;
            }*/
            if ((material != null) && (material.texture != null) && (material.texture != SFRender.SFRenderEngine.opaque_tex))
                SFResourceManager.Textures.Dispose(material.texture.GetName());
        }
    }

    public class SFModel3D: SFResource
    {
        public SFSubModel3D[] submodels = null;
        public Physics.BoundingBox aabb;

        // instance matrix data, from MeshCache
        public int CurrentMatrixIndex = 0;       // reset every frame
        public int MatrixOffset = 0;             // offset into MeshCache.MatrixData array  
        public int MatrixCount = 0;              // number of matrices used; CurrentMatrixIndex stops here

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

            List<SFSubModel3D> tmp_submodels = new List<SFSubModel3D>();
            int failed_submodels = 0;
            for(int i=0; i<modelnum; i++)
            {
                SFSubModel3D sbm = new SFSubModel3D();
                int return_code = sbm.Load(br);
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
                }

                if (i != modelnum - 1)
                    br.BaseStream.Position += 2;
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
