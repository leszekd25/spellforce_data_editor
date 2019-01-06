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
using SpellforceDataEditor.SF3D;
using SpellforceDataEditor.SFSound;
using SpellforceDataEditor.SFUnPak;

namespace SpellforceDataEditor.SFResources
{
    public class SFResourceManager
    {
        public SFResourceContainer<SFTexture> Textures { get; private set; } = new SFResourceContainer<SFTexture>();
        public SFResourceContainer<SFModel3D> Models { get; private set; } = new SFResourceContainer<SFModel3D>();
        public SFResourceContainer<SFAnimation> Animations { get; private set; } = new SFResourceContainer<SFAnimation>();
        public SFResourceContainer<SFBoneIndex> BSIs { get; private set; } = new SFResourceContainer<SFBoneIndex>();
        public SFResourceContainer<SFModelSkin> Skins { get; private set; } = new SFResourceContainer<SFModelSkin>();
        public SFResourceContainer<SFSkeleton> Skeletons { get; private set; } = new SFResourceContainer<SFSkeleton>();
        public SFResourceContainer<StreamResource> Musics { get; private set; } = new SFResourceContainer<StreamResource>();
        public SFResourceContainer<StreamResource> Sounds { get; private set; } = new SFResourceContainer<StreamResource>();
        public SFResourceContainer<StreamResource> Messages { get; private set; } = new SFResourceContainer<StreamResource>();
        public string current_resource = "";
        public List<string> mesh_names { get; private set; } = new List<string>();
        public List<string> skeleton_names { get; private set; } = new List<string>();
        public List<string> animation_names { get; private set; } = new List<string>();
        public List<string> music_names { get; private set; } = new List<string>();
        public List<string> sound_names { get; private set; } = new List<string>();
        public Dictionary<string, List<string>> message_names { get; private set; } = new Dictionary<string, List<string>>();

        public SFResourceManager()
        {
            Textures = new SFResourceContainer<SFTexture>("texture", ".dds", this);
            Models = new SFResourceContainer<SFModel3D>("mesh", ".msb", this);
            Animations = new SFResourceContainer<SFAnimation>("animation", ".bob", this);
            BSIs = new SFResourceContainer<SFBoneIndex>("skinning\\b20", ".bsi", this);
            Skins = new SFResourceContainer<SFModelSkin>("skinning\\b20", ".msb", this);
            Skeletons = new SFResourceContainer<SFSkeleton>("animation", ".bor", this);
            Musics = new SFResourceContainer<StreamResource>("sound", ".mp3", this);
            Sounds = new SFResourceContainer<StreamResource>("sound", ".wav", this);
            Messages = new SFResourceContainer<StreamResource>("", "", this); //modified externally
        }

        //generate mesh names, for use in SF3DManager
        public void FindAllMeshes()
        {
            string[] filter_mesh = { "sf8.pak", "sf22.pak", "sf32.pak" };
            string[] filter_skel = { "sf4.pak", "sf22.pak", "sf32.pak" };
            string[] filter_anim = { "sf5.pak", "sf22.pak", "sf32.pak" };
            string[] filter_musi = { "sf3.pak", "sf20.pak", "sf30.pak" };
            string[] filter_snds = { "sf2.pak", "sf20.pak", "sf30.pak" };
            string[] filter_mess_battle = { "sf2.pak", "sf23.pak", "sf33.pak" };
            string[] filter_mess_other =  { "sf10.pak", "sf20.pak", "sf23.pak", "sf33.pak" };
            string[] filter_mess_talk =   { "sf10.pak", "sf23.pak", "sf33.pak" };

            mesh_names = SFUnPak.SFUnPak.ListAllWithExtension("mesh", ".msb", filter_mesh);
            skeleton_names = SFUnPak.SFUnPak.ListAllWithExtension( "animation", ".bor", filter_skel);
            skeleton_names.RemoveAll(x => !(x.StartsWith("figure")));
            animation_names = SFUnPak.SFUnPak.ListAllWithExtension("animation", ".bob", filter_anim);
            music_names = SFUnPak.SFUnPak.ListAllWithExtension("sound", ".mp3", filter_musi);
            sound_names = SFUnPak.SFUnPak.ListAllWithExtension("sound", ".wav", filter_snds);
            message_names["RTS Battle"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\battle", ".wav", filter_mess_battle);
            message_names["NPC"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech", ".mp3", filter_mess_other);
            message_names["Male"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\male", ".mp3", filter_mess_talk);
            message_names["Female"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\female", ".mp3", filter_mess_talk);
            message_names["RTS Workers"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\messages", ".mp3", filter_mess_talk);
        }

        public void DisposeAll()
        {
            Textures.DisposeAll();
            Models.DisposeAll();
            Animations.DisposeAll();
            BSIs.DisposeAll();
            Skins.DisposeAll();
            Skeletons.DisposeAll();
            Musics.DisposeAll();
            Sounds.DisposeAll();
            Messages.DisposeAll();
        }
    }
}
