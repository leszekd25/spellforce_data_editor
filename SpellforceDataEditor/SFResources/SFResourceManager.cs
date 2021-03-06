﻿/*
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
    public static class SFResourceManager
    {
        public static SFResourceContainer<SFTexture> Textures { get; private set; } = new SFResourceContainer<SFTexture>("texture", ".dds|.tga");
        public static SFResourceContainer<SFModel3D> Models { get; private set; } = new SFResourceContainer<SFModel3D>("mesh", ".msb");
        public static SFResourceContainer<SFAnimation> Animations { get; private set; } = new SFResourceContainer<SFAnimation>("animation", ".bob");
        public static SFResourceContainer<SFBoneIndex> BSIs { get; private set; } = new SFResourceContainer<SFBoneIndex>("skinning\\b20", ".bsi");
        public static SFResourceContainer<SFModelSkin> Skins { get; private set; } = new SFResourceContainer<SFModelSkin>("skinning\\b20", ".msb");
        public static SFResourceContainer<SFSkeleton> Skeletons { get; private set; } = new SFResourceContainer<SFSkeleton>("animation", ".bor");
        public static SFResourceContainer<StreamResource> Musics { get; private set; } = new SFResourceContainer<StreamResource>("sound", ".mp3");
        public static SFResourceContainer<StreamResource> Sounds { get; private set; } = new SFResourceContainer<StreamResource>("sound", ".wav");
        public static SFResourceContainer<StreamResource> Messages { get; private set; } = new SFResourceContainer<StreamResource>("", ".wav|.mp3"); //modified externally
        public static string current_resource = "";
        public static List<string> mesh_names { get; private set; } = new List<string>();
        public static List<string> skeleton_names { get; private set; } = new List<string>();
        public static List<string> animation_names { get; private set; } = new List<string>();
        public static List<string> music_names { get; private set; } = new List<string>();
        public static List<string> sound_names { get; private set; } = new List<string>();
        public static Dictionary<string, List<string>> message_names { get; private set; } = new Dictionary<string, List<string>>();

        public static bool ready { get; set; } = false;

        //generate mesh names, for use in SF3DManager
        public static void FindAllMeshes()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes() called");
            if(ready)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): Already loaded");
                return;
            }    

            string[] filter_mesh = { "sf8.pak", "sf22.pak", "sf32.pak" };
            string[] filter_skel = { "sf4.pak", "sf22.pak", "sf32.pak" };
            string[] filter_anim = { "sf5.pak", "sf22.pak", "sf32.pak" };
            string[] filter_musi = { "sf3.pak", "sf20.pak", "sf30.pak" };
            string[] filter_snds = { "sf2.pak", "sf20.pak", "sf30.pak" };
            string[] filter_mess_battle = { "sf2.pak", "sf23.pak", "sf33.pak" };
            string[] filter_mess_other = { "sf10.pak", "sf20.pak", "sf23.pak", "sf33.pak" };
            string[] filter_mess_talk = { "sf10.pak", "sf23.pak", "sf33.pak" };

            mesh_names = SFUnPak.SFUnPak.ListAllWithExtension("mesh", ".msb", filter_mesh);
            skeleton_names = SFUnPak.SFUnPak.ListAllWithExtension("animation", ".bor", filter_skel);
            skeleton_names.RemoveAll(x => !(x.StartsWith("figure")));
            animation_names = SFUnPak.SFUnPak.ListAllWithExtension("animation", ".bob", filter_anim);
            music_names = SFUnPak.SFUnPak.ListAllWithExtension("sound", ".mp3", filter_musi);
            sound_names = SFUnPak.SFUnPak.ListAllWithExtension("sound", ".wav", filter_snds);
            message_names["RTS Battle"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\battle", ".wav", filter_mess_battle);
            message_names["NPC"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech", ".mp3", filter_mess_other);
            message_names["Male"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\male", ".mp3", filter_mess_talk);
            message_names["Female"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\female", ".mp3", filter_mess_talk);
            message_names["RTS Workers"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\messages", ".mp3", filter_mess_talk);

            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): 3D models found: " + mesh_names.Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): Skeletons found: " + skeleton_names.Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): Animations found: " + animation_names.Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): Music files found: " + music_names.Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): Miscellaneous sound files found: " + sound_names.Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): RTS Battle message sound files found: " + message_names["RTS Battle"].Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): NPC message sound files found: " + message_names["NPC"].Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): Male message sound files found: " + message_names["Male"].Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): Female message sound files found: " + message_names["Female"].Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.FindAllMeshes(): RTS Workers message sound files found: " + message_names["RTS Workers"].Count.ToString());

            ready = true;
        }

        public static void DisposeAll()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.DisposeAll() called");
            Models.DisposeAll();
            Animations.DisposeAll();
            BSIs.DisposeAll();
            Skins.DisposeAll();
            Skeletons.DisposeAll();
            Textures.DisposeAll();    // likely unneeded, but still called
            Musics.DisposeAll();
            Sounds.DisposeAll();
            Messages.DisposeAll();
        }

        public static bool LoadModel(string model_name)
        {
            if (model_name != "")
            {
                int result = Models.Load(model_name);
                if ((result != 0) && (result != -1))
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFResourceManager.LoadModel(): Model could not be loaded (model name: "
                    + model_name + ")");
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool LoadSkeleton(string skeleton_name)
        {
            if (skeleton_name != "")
            {
                int result = Skeletons.Load(skeleton_name);
                if ((result != 0) && (result != -1))
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFResourceManager.LoadSkeleton(): Skeleton could not be loaded (skeleton name: "
                    + skeleton_name + ")");
                    return false;
                }
                return true;
            }
            return false;
        }

        public static bool LoadSkin(string skin_name)
        {
            if (skin_name != "")
            {
                int result = Skins.Load(skin_name);
                if ((result != 0) && (result != -1))
                {
                    LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFResourceManager.LoadSkin(): Skin could not be loaded (skin name: "
                    + skin_name + ")");
                    return false;
                }
                return true;
            }
            return false;
        }

        public static void LogMemoryUsage()
        {
            string log_string = "Memory usage (bytes): ";
            log_string += "TEXTURES " + Textures.GetResourceSize().ToString()
                       + ", 3D MODELS " + Models.GetResourceSize().ToString()
                       + ", SKELETONS " + Skeletons.GetResourceSize().ToString()
                       + ", SKINS " + Skins.GetResourceSize().ToString()
                       + ", BSI" + BSIs.GetResourceSize().ToString()
                       + ", ANIMATIONS " + Animations.GetResourceSize().ToString()
                       + ", SOUNDS " + (Sounds.GetResourceSize() + Musics.GetResourceSize() + Messages.GetResourceSize()).ToString();
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.LogMemoryUsage() called");
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, log_string);
        }
    }
}
