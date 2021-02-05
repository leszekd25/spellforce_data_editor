using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapOcean
    {
        public SFMap map;
        static SF3D.SFModel3D ocean_mesh = null;
        SF3D.SceneSynchro.SceneNodeSimple ocean_obj = null;
        public SFMapOcean()
        {
            // generate selection 3d model
            ocean_mesh = new SF3D.SFModel3D();

            Vector3[] vertices = new Vector3[4];
            Vector2[] uvs = new Vector2[4];
            byte[] colors = new byte[16];
            Vector3[] normals = new Vector3[4];

            vertices[0] = new Vector3(-512, 0f, -512);
            vertices[1] = new Vector3(512, 0f, -512);
            vertices[2] = new Vector3(-512, 0f, 512);
            vertices[3] = new Vector3(512, 0f, 512);
            uvs[0] = new Vector2(-SFMapHeightMapMesh.CHUNK_SIZE, -SFMapHeightMapMesh.CHUNK_SIZE);
            uvs[1] = new Vector2(SFMapHeightMapMesh.CHUNK_SIZE, -SFMapHeightMapMesh.CHUNK_SIZE);
            uvs[2] = new Vector2(-SFMapHeightMapMesh.CHUNK_SIZE, SFMapHeightMapMesh.CHUNK_SIZE);
            uvs[3] = new Vector2(SFMapHeightMapMesh.CHUNK_SIZE, SFMapHeightMapMesh.CHUNK_SIZE);
            for (int i = 0; i < 4; i++)
            {
                normals[i] = new Vector3(0.0f, 1.0f, 0.0f);
            }
            for(int i = 0; i < 4; i++)
            {
                colors[4 * i + 0] = 0x33;
                colors[4 * i + 1] = 0xB2;
                colors[4 * i + 2] = 0xD4;
                colors[4 * i + 3] = 0xB2;
            }

            uint[] indices = { 0, 1, 2, 1, 3, 2 };

            SF3D.SFMaterial material = new SF3D.SFMaterial();
            material.casts_shadow = false;

            string tex_name = "test_ocean_relief_4_l8";
            SF3D.SFTexture tex = null;
            int tex_code = SFResources.SFResourceManager.Textures.Load(tex_name);
            if ((tex_code != 0) && (tex_code != -1))
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFMapOcean(): Could not load texture (texture name = " + tex_name + ")");
            else
            {
                tex = SFResources.SFResourceManager.Textures.Get(tex_name);
                tex.FreeMemory();
            }
            material.texture = tex;
            material.casts_shadow = false;
            material.transparent_pass = true;
            material.apply_shadow = false;
            material.distance_fade = false;

            SF3D.SFSubModel3D sbm1 = new SF3D.SFSubModel3D();
            sbm1.CreateRaw(vertices, uvs, colors, normals, indices, material);

            ocean_mesh.CreateRaw(new SF3D.SFSubModel3D[] { sbm1 });
            SFResources.SFResourceManager.Models.AddManually(ocean_mesh, "_OCEAN_");
        }

        public void CreateOceanObject()
        {
            ocean_obj = SF3D.SFRender.SFRenderEngine.scene.AddSceneNodeSimple(SF3D.SFRender.SFRenderEngine.scene.root, "_OCEAN_", "_OCEAN_");
            ocean_obj.Rotation = Quaternion.FromEulerAngles(0, (float)Math.PI / 2, 0);
        }

        public void SetPosition(Vector3 center_pos)
        {
            float _x = ((int)(center_pos.X / 16)) * 16;
            float _z = ((int)(center_pos.Z / 16)) * 16;
            ocean_obj.SetPosition(new Vector3(_x, 3, _z));
        }

        public void Dispose()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFMap, "SFMapOcean.Dispose() called");
            SF3D.SFRender.SFRenderEngine.scene.RemoveSceneNode(SF3D.SFRender.SFRenderEngine.scene.root.FindNode<SF3D.SceneSynchro.SceneNodeSimple>("_OCEAN_"));
            ocean_obj = null;
        }
    }
}
