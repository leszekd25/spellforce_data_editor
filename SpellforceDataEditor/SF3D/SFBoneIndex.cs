using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellforceDataEditor.SF3D
{
    public class SFBoneIndex: SFResource
    {
        public int modelnum { get; private set; } = 0;
        public int[] bone_count { get; private set; } = null;
        public int[][] bone_index_remap { get; private set; } = null;

        public void Init()
        {

        }

        public int Load(MemoryStream ms, SFResourceManager rm)
        {
            BinaryReader br = new BinaryReader(ms);
            modelnum = br.ReadInt32();
            bone_count = new int[modelnum];
            bone_index_remap = new int[modelnum][];
            for(int i = 0; i < modelnum; i++)
            {
                br.ReadInt32();
                bone_count[i] = br.ReadInt32();
                bone_index_remap[i] = new int[bone_count[i]];
                for(int j = 0; j < bone_count[i]; j++)
                {
                    bone_index_remap[i][j] = br.ReadInt32();
                }
            }

            return 0;
        }

        public void Dispose()
        {

        }
    }
}
