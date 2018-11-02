/*
    SFResourceManager is an abstraction which allows for easy retrieval of any type of resource from PAK files
    It consists of several SFResourceContainer objects, each of different resource type
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SF3D
{
    public class SFResourceManager
    {
        public SFUnPak.SFUnPak unpacker { get; private set; } = new SFUnPak.SFUnPak();
        public SFResourceContainer<SFTexture> Textures { get; private set; } = new SFResourceContainer<SFTexture>();
        public SFResourceContainer<SFModel3D> Models { get; private set; } = new SFResourceContainer<SFModel3D>();
        public SFResourceContainer<SFAnimation> Animations { get; private set; } = new SFResourceContainer<SFAnimation>();
        public SFResourceContainer<SFBoneIndex> BSIs { get; private set; } = new SFResourceContainer<SFBoneIndex>();
        public SFResourceContainer<SFModelSkin> Skins { get; private set; } = new SFResourceContainer<SFModelSkin>();
        public SFResourceContainer<SFSkeleton> Skeletons { get; private set; } = new SFResourceContainer<SFSkeleton>();
        public string current_resource = "";
        public List<string> mesh_names { get; private set; } = new List<string>();
        public List<string> skeleton_names { get; private set; } = new List<string>();
        public List<string> animation_names { get; private set; } = new List<string>();

        public SFResourceManager()
        {
            Textures = new SFResourceContainer<SFTexture>("texture", ".dds", this);
            Models = new SFResourceContainer<SFModel3D>("mesh", ".msb", this);
            Animations = new SFResourceContainer<SFAnimation>("animation", ".bob", this);
            BSIs = new SFResourceContainer<SFBoneIndex>("skinning\\b20", ".bsi", this);
            Skins = new SFResourceContainer<SFModelSkin>("skinning\\b20", ".msb", this);
            Skeletons = new SFResourceContainer<SFSkeleton>("animation", ".bor", this);
        }

        //generate mesh names, for use in SF3DManager
        public void FindAllMeshes()
        {
            string[] filter_mesh = { "sf8.pak", "sf22.pak", "sf32.pak" };
            string[] filter_skel = { "sf4.pak", "sf22.pak", "sf32.pak" };
            string[] filter_anim = { "sf5.pak", "sf22.pak", "sf32.pak" };
            mesh_names = unpacker.ListAllWithExtension(".msb", filter_mesh);
            skeleton_names = unpacker.ListAllWithExtension(".bor", filter_skel);
            skeleton_names.RemoveAll(x => !(x.StartsWith("figure")));
            animation_names = unpacker.ListAllWithExtension(".bob", filter_anim);
            mesh_names.Sort();
            skeleton_names.Sort();
            animation_names.Sort();
        }

        public void DisposeAll()
        {
            Textures.DisposeAll();
            Models.DisposeAll();
            Animations.DisposeAll();
            BSIs.DisposeAll();
            Skins.DisposeAll();
            Skeletons.DisposeAll();
        }
    }
}
