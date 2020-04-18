using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SFEffect
{
    public enum EffectRestrictionMode { kDrwCsParent = 0, kDrwCsWorld = 1, kDrwCsBone = 2, kDrwCsFloor = 4, kDrwCsObject = 8, kDrwCsResetRotation = 16, kDrwCsAim = 32, kDrwCsProjectile = 64, kDrwCsSpan = 128, kDrwCsSwap = 256, kDrwCsTintTarget = 512}
    public class EffectObject
    {
        public string Name = "NewEffect";
        public int NumberOfParticles = 1;
        public ParticleMovie Movie;
        public EffectRestrictionMode Restriction = EffectRestrictionMode.kDrwCsParent;
        public float BoundingRadius = 1.5f;
        public string BoneTarget = "";
        public string BoneSource = "";
        public bool Shadow = false;
        public List<string> Billboards = new List<string>();
        public List<string> Meshes = new List<string>();
        public List<string> Skins = new List<string>();
        public List<string> Lights = new List<string>();
        public List<EffectObject> SubObjects = new List<EffectObject>();
    }
}
