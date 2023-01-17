﻿/*
 * SFBoneIndex is a resource which contains info for skin
 * GPU is limited in that it has to know how much data it has to load - meaning you can't load dynamic arrays of matrices, for example
 * Skeletons are just that - they're dynamic in size
 * To overcome this issue, Spellforce splits its animated models into parts, each of which is animated by at most 20 bones
 * SFBoneIndex contains info on which part is animated by which bones of supplied skeleton, and  in which order the bones are
 * Technically, SFBoneIndex didn't have to be a resource, it's one purely for convenience
 */

using SFEngine.SFResources;
using System.IO;

namespace SFEngine.SF3D
{
    public class SFBoneIndex : SFResource
    {
        public int modelnum { get; private set; } = 0;
        public int[] bone_count { get; private set; } = null;
        public int[] material_per_segment { get; private set; } = null;
        public int[][] bone_index_remap { get; private set; } = null;

        public override int Load(MemoryStream ms, object custom_data)
        {
            BinaryReader br = new BinaryReader(ms);
            modelnum = br.ReadInt32();
            bone_count = new int[modelnum];
            material_per_segment = new int[modelnum];
            bone_index_remap = new int[modelnum][];
            for (int i = 0; i < modelnum; i++)
            {
                material_per_segment[i] = br.ReadInt32();
                bone_count[i] = br.ReadInt32();
                bone_index_remap[i] = new int[bone_count[i]];
                for (int j = 0; j < bone_count[i]; j++)
                {
                    bone_index_remap[i][j] = br.ReadInt32();
                }
            }

            RAMSize = 0;
            for (int i = 0; i < modelnum; i++)
            {
                RAMSize += bone_count[i] + 1;
            }

            RAMSize *= 4;
            RAMSize += 4;

            return 0;
        }
    }
}
