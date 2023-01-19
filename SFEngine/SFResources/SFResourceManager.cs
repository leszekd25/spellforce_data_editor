/*
    SFResourceManager is an abstraction which allows for easy retrieval of any type of resource from PAK files
    It consists of several SFResourceContainer objects, each of different resource type
 */

using SFEngine.SF3D;
using SFEngine.SFSound;
using System.Collections.Generic;
using System.Text;

namespace SFEngine.SFResources
{
    public static class SFResourceManager
    {
        public static SFResourceContainer<SFTexture> Textures { get; private set; } = new SFResourceContainer<SFTexture>("texture", ".dds|.tga", new string[] {"sf35.pak", "sf32.pak", "sf25.pak", "sf22.pak", "sf1.pak", "sf0.pak" });
        public static SFResourceContainer<SFModel3D> Models { get; private set; } = new SFResourceContainer<SFModel3D>("mesh", ".msb", new string[] { "sf32.pak", "sf22.pak", "sf8.pak" });
        public static SFResourceContainer<SFAnimation> Animations { get; private set; } = new SFResourceContainer<SFAnimation>("animation", ".bob", new string[] {"sf32.pak", "sf22.pak", "sf5.pak" });
        public static SFResourceContainer<SFBoneIndex> BSIs { get; private set; } = new SFResourceContainer<SFBoneIndex>("skinning\\b20", ".bsi", new string[] { "sf32.pak", "sf22.pak", "sf8.pak" });
        public static SFResourceContainer<SFModelSkin> Skins { get; private set; } = new SFResourceContainer<SFModelSkin>("skinning\\b20", ".msb", new string[] { "sf32.pak", "sf22.pak", "sf8.pak" });
        public static SFResourceContainer<SFSkeleton> Skeletons { get; private set; } = new SFResourceContainer<SFSkeleton>("animation", ".bor", new string[] { "sf32.pak", "sf22.pak", "sf4.pak" });
        public static SFResourceContainer<StreamResource> Musics { get; private set; } = new SFResourceContainer<StreamResource>("sound", ".mp3", new string[] { "sf30.pak", "sf20.pak", "sf3.pak" });
        public static SFResourceContainer<StreamResource> Sounds { get; private set; } = new SFResourceContainer<StreamResource>("sound", ".wav", new string[] { "sf30.pak", "sf20.pak", "sf2.pak" });
        public static SFResourceContainer<StreamResource> Messages { get; private set; } = new SFResourceContainer<StreamResource>("", ".wav|.mp3", new string[] { "sf33.pak", "sf23.pak", "sf20.pak", "sf10.pak", "sf2.pak" }); //modified externally
        public static string current_resource = "";
        public static List<string> mesh_names { get; private set; } = new List<string>();
        public static List<string> skeleton_names { get; private set; } = new List<string>();
        public static List<string> animation_names { get; private set; } = new List<string>();
        public static List<string> music_names { get; private set; } = new List<string>();
        public static List<string> sound_names { get; private set; } = new List<string>();
        public static Dictionary<string, List<string>> message_names { get; private set; } = new Dictionary<string, List<string>>();

        public static bool pak_resources_listed { get; set; } = false;
        public static bool pak_animations_listed { get; set; } = false;
        public static bool filesystem_resources_listed { get; set; } = false;

        // use this after game directory was specified
        public static void ListAllFilesystemResources()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllFilesystemResources() called");
            if (filesystem_resources_listed)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllFilesystemResources(): Already loaded");
                return;
            }

            int textures_found = Textures.ListAllFilesystemResources();
            int models_found = Models.ListAllFilesystemResources();
            int animations_found = Animations.ListAllFilesystemResources();
            int bsis_found = BSIs.ListAllFilesystemResources();
            int skins_found = Skins.ListAllFilesystemResources();
            int skeletons_found = Skeletons.ListAllFilesystemResources();
            int musics_found = Musics.ListAllFilesystemResources();
            int sounds_found = Sounds.ListAllFilesystemResources();

            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllFilesystemResources(): Textures found: " + textures_found.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllFilesystemResources(): Models found: " + models_found.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllFilesystemResources(): Animations found: " + animations_found.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllFilesystemResources(): BSI files found: " + bsis_found.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllFilesystemResources(): Skins found: " + skins_found.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllFilesystemResources(): Skeletons found: " + skeletons_found.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllFilesystemResources(): Music files found: " + musics_found.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllFilesystemResources(): Sound files found: " + sounds_found.ToString());

            filesystem_resources_listed = true;
        }

        //generate mesh names, for use in SF3DManager
        public static void ListAllPakResources()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources() called");
            if (pak_resources_listed)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): Already loaded");
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
            if (!pak_animations_listed)
            {
                animation_names = SFUnPak.SFUnPak.ListAllWithExtension("animation", ".bob", filter_anim);
            }
            music_names = SFUnPak.SFUnPak.ListAllWithExtension("sound", ".mp3", filter_musi);
            sound_names = SFUnPak.SFUnPak.ListAllWithExtension("sound", ".wav", filter_snds);
            message_names["RTS Battle"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\battle", ".wav", filter_mess_battle);
            message_names["NPC"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech", ".mp3", filter_mess_other);
            message_names["Male"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\male", ".mp3", filter_mess_talk);
            message_names["Female"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\female", ".mp3", filter_mess_talk);
            message_names["RTS Workers"] = SFUnPak.SFUnPak.ListAllWithExtension("sound\\speech\\messages", ".mp3", filter_mess_talk);

            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): 3D models found: " + mesh_names.Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): Skeletons found: " + skeleton_names.Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): Animations found: " + animation_names.Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): Music files found: " + music_names.Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): Miscellaneous sound files found: " + sound_names.Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): RTS Battle message sound files found: " + message_names["RTS Battle"].Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): NPC message sound files found: " + message_names["NPC"].Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): Male message sound files found: " + message_names["Male"].Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): Female message sound files found: " + message_names["Female"].Count.ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakResources(): RTS Workers message sound files found: " + message_names["RTS Workers"].Count.ToString());

            pak_resources_listed = true;
            pak_animations_listed = true;
        }

        // generate anim names, for use in map editor
        // no need to do this if all resources are already listed
        public static void ListAllPakAnimations()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakAnimations() called");
            if (pak_animations_listed)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakAnimations(): Already loaded");
                return;
            }

            string[] filter_anim = { "sf5.pak", "sf22.pak", "sf32.pak" };

            animation_names = SFUnPak.SFUnPak.ListAllWithExtension("animation", ".bob", filter_anim);

            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "SFResourceManager.ListAllPakAnimations(): Animations found: " + animation_names.Count.ToString());

            pak_animations_listed = true;
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
                int result = Models.Load(model_name, SFUnPak.FileSource.ANY);
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
                int result = Skeletons.Load(skeleton_name, SFUnPak.FileSource.ANY);
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
                int result = Skins.Load(skin_name, SFUnPak.FileSource.ANY);
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
            StringBuilder log_msg = new StringBuilder();

            log_msg.Append("Resource memory usage (bytes, RAM): ");
            log_msg.Append("TEXTURES " + Textures.RAMSize.ToString());
            log_msg.Append(", 3D MODELS " + Models.RAMSize.ToString());
            log_msg.Append(", SKELETONS " + Skeletons.RAMSize.ToString());
            log_msg.Append(", SKINS " + Skins.RAMSize.ToString());
            log_msg.Append(", BSI" + BSIs.RAMSize.ToString());
            log_msg.Append(", SOUNDS " + (Sounds.RAMSize + Musics.RAMSize + Messages.RAMSize).ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, log_msg.ToString());
            log_msg.Clear();

            log_msg.Append("Resource memory usage (bytes, device): ");
            log_msg.Append("TEXTURES " + Textures.DeviceSize.ToString());
            log_msg.Append(", 3D MODELS " + Models.DeviceSize.ToString());
            log_msg.Append(", SKELETONS " + Skeletons.DeviceSize.ToString());
            log_msg.Append(", SKINS " + Skins.DeviceSize.ToString());
            log_msg.Append(", BSI" + BSIs.DeviceSize.ToString());
            log_msg.Append(", SOUNDS " + (Sounds.DeviceSize + Musics.DeviceSize + Messages.DeviceSize).ToString());
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, log_msg.ToString());
            log_msg.Clear();
        }

        public static void LogUndisposedResources()
        {
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "Logging undisposed resources from texture container");
            Textures.LogUndisposedResources();
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "Logging undisposed resources from model container");
            Models.LogUndisposedResources();
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "Logging undisposed resources from animation container");
            Animations.LogUndisposedResources();
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "Logging undisposed resources from BSI container");
            BSIs.LogUndisposedResources();
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "Logging undisposed resources from skin container");
            Skins.LogUndisposedResources();
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "Logging undisposed resources from skeleton container");
            Skeletons.LogUndisposedResources();
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "Logging undisposed resources from music container");
            Musics.LogUndisposedResources();
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "Logging undisposed resources from sound container");
            Sounds.LogUndisposedResources();
            LogUtils.Log.Info(LogUtils.LogSource.SFResources, "Logging undisposed resources from message container");
            Messages.LogUndisposedResources();
        }
    }
}
