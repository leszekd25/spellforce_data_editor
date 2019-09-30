using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SFMap.MapEdit
{
    public class MapOverlayChunk
    {
        public SF3D.SFModel3D mesh { get; private set; } = null;
        public List<SFCoord> points { get; private set; } = new List<SFCoord>();
        public Vector4 color = new Vector4(1);

        public void Update(SFMapHeightMapChunk hmap_chunk, string name)
        {
            if (mesh != null)
                SFResources.SFResourceManager.Models.Dispose(mesh.GetName());
            mesh = null;

            if (points.Count == 0)
                return;

            Vector3[] vertices = new Vector3[points.Count * 5];
            Vector2[] uvs = new Vector2[points.Count * 5];
            Vector4[] colors = new Vector4[points.Count * 5];
            Vector3[] normals = new Vector3[points.Count  * 5];
            uint[] elements = new uint[points.Count * 12];
            
            uint k = 0;
            foreach(SFCoord p in points)
            {
                int offset = (p.y * hmap_chunk.hmap.chunk_size + p.x) * 6;
                float h = hmap_chunk.vertices[offset].Y;
                // vertices
                vertices[k * 5 + 0] = new Vector3(p.x - 0.2f, h, (hmap_chunk.hmap.chunk_size - p.y - 1) - 0.2f);
                vertices[k * 5 + 1] = new Vector3(p.x + 0.2f, h, (hmap_chunk.hmap.chunk_size - p.y - 1) - 0.2f);
                vertices[k * 5 + 2] = new Vector3(p.x - 0.2f, h, (hmap_chunk.hmap.chunk_size - p.y - 1) + 0.2f);
                vertices[k * 5 + 3] = new Vector3(p.x + 0.2f, h, (hmap_chunk.hmap.chunk_size - p.y - 1) + 0.2f);
                vertices[k * 5 + 4] = new Vector3(p.x, h + 1f, (hmap_chunk.hmap.chunk_size - p.y - 1));
                // colors
                colors[k * 5 + 0] = color;
                colors[k * 5 + 1] = color;
                colors[k * 5 + 2] = color;
                colors[k * 5 + 3] = color;
                colors[k * 5 + 4] = color;
                // normals
                normals[k * 5 + 0] = new Vector3(0, 1, 0);
                normals[k * 5 + 1] = new Vector3(0, 1, 0);
                normals[k * 5 + 2] = new Vector3(0, 1, 0);
                normals[k * 5 + 3] = new Vector3(0, 1, 0);
                normals[k * 5 + 4] = new Vector3(0, 1, 0);
                // elements
                elements[k * 12 + 0] = k * 5 + 0;
                elements[k * 12 + 1] = k * 5 + 1;
                elements[k * 12 + 2] = k * 5 + 4;
                elements[k * 12 + 3] = k * 5 + 1;
                elements[k * 12 + 4] = k * 5 + 3;
                elements[k * 12 + 5] = k * 5 + 4;
                elements[k * 12 + 6] = k * 5 + 3;
                elements[k * 12 + 7] = k * 5 + 2;
                elements[k * 12 + 8] = k * 5 + 4;
                elements[k * 12 + 9] = k * 5 + 2;
                elements[k * 12 + 10] = k * 5 + 0;
                elements[k * 12 + 11] = k * 5 + 4;

                k++;
            }

            SF3D.SFSubModel3D sbm = new SF3D.SFSubModel3D();
            sbm.CreateRaw(vertices, uvs, colors, normals, elements, null);
            sbm.instance_matrices.AddElem(hmap_chunk.owner.ResultTransform);
            
            mesh = new SF3D.SFModel3D();
            mesh.CreateRaw(new SF3D.SFSubModel3D[] { sbm });
            SFResources.SFResourceManager.Models.AddManually(mesh, "OVERLAY_" + name + "_" + hmap_chunk.ix.ToString() + "_" + hmap_chunk.iy.ToString());
        }

        public void Dispose()
        {
            if (mesh != null)
                mesh.Dispose();
        }
    }
}
