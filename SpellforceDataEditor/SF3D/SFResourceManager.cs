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

        public SFResourceManager()
        {
            Textures = new SFResourceContainer<SFTexture>("texture", ".dds", this);
            Models = new SFResourceContainer<SFModel3D>("mesh", ".msb", this);
            Animations = new SFResourceContainer<SFAnimation>("animation", ".bob", this);
            BSIs = new SFResourceContainer<SFBoneIndex>("skinning\\b20", ".bsi", this);
            Skins = new SFResourceContainer<SFModelSkin>("skinning\\b20", ".msb", this);
            Skeletons = new SFResourceContainer<SFSkeleton>("animation", ".bor", this);
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
