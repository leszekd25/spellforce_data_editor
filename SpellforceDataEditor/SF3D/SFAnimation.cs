/*
 * SFBoneAnimation contains info on how a single bone transform changes over time
 * SFAnimation is a set of SFBoneAnimation objects corresponding to a supplied skeleton
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SpellforceDataEditor.SFResources;

namespace SpellforceDataEditor.SF3D
{
    public class SFBoneAnimation
    {
        public InterpolatedVector3 position { get; private set; } = new InterpolatedVector3();
        public InterpolatedQuaternion rotation { get; private set; } = new InterpolatedQuaternion();

        public CompressedMatrix get_matrix(float t)
        {
            return new CompressedMatrix(position.Get(t), rotation.Get(t));
        }
    }

    public class SFAnimation: SFResource
    {
        public int bone_count { get; private set; } = 0;
        public List<SFBoneAnimation> bone_animations { get; private set; } = new List<SFBoneAnimation>();
        public float max_time { get; private set; } = 0f;

        public void Init()
        {
            return;
        }

        public int Load(MemoryStream ms, SFResourceManager rm)
        {
            BinaryReader br = new BinaryReader(ms);

            max_time = 0;

            br.ReadInt16();
            bone_count = br.ReadInt32();

            for(int i = 0; i < bone_count; i++)
            {
                SFBoneAnimation ba = new SFBoneAnimation();
                bone_animations.Add(ba);

                int data1, data4, anim_count;
                float data2, data3;

                data1 = br.ReadInt32(); data2 = br.ReadSingle(); data3 = br.ReadSingle();
                data4 = br.ReadInt32(); anim_count = br.ReadInt32();
                for (int j = 0; j < anim_count; j++)
                {
                    float[] data = new float[5];
                    for (int k = 0; k < 5; k++)
                        data[k] = br.ReadSingle();
                    Quaternion q = new Quaternion(data[1], data[2], data[3], data[0]);
                    ba.rotation.Add(q, data[4]);
                }

                data1 = br.ReadInt32(); data2 = br.ReadSingle(); data3 = br.ReadSingle();
                data4 = br.ReadInt32(); anim_count = br.ReadInt32();
                for (int j = 0; j < anim_count; j++)
                {
                    float[] data = new float[4];
                    for (int k = 0; k < 4; k++)
                        data[k] = br.ReadSingle();
                    Vector3 v = new Vector3(data[0], data[1], data[2]);
                    ba.position.Add(v, data[3]);
                }

                max_time = Math.Max(ba.position.GetMaxTime(), max_time);
            }

            return 0;
        }

        public CompressedMatrix CalculateBoneMatrix(int bone_index, float t)
        {
            return bone_animations[bone_index].get_matrix(t);
        }

        public void Dispose()
        {
            return;
        }
    }
}
