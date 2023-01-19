﻿/*
 * SFBoneAnimation contains info on how a single bone transform changes over time
 * SFAnimation is a set of SFBoneAnimation objects corresponding to a supplied skeleton
 */

using OpenTK;
using SFEngine.SFResources;
using System;
using System.IO;

namespace SFEngine.SF3D
{
    public class SFBoneAnimation
    {
        public InterpolatedVector3 position;
        public InterpolatedQuaternion rotation;
        public bool is_static = false;
        public Matrix4 static_transform;

        public void ResolveStatic()
        {
            position.ResolveStatic();
            rotation.ResolveStatic();
            is_static = (position.is_static) && (rotation.is_static);
            if (is_static)
            {
                GetInterpolatedMatrix4(0, ref static_transform);
            }
        }

        private void GetInterpolatedMatrix4(float t, ref Matrix4 mat)
        {
            mat = Matrix4.CreateFromQuaternion(rotation.Get(t));
            mat.Row3 = new Vector4(position.Get(t), 1);
        }

        public void GetMatrix4(float t, ref Matrix4 mat)
        {
            if (is_static)
            {
                mat = static_transform;
            }
            else
            {
                GetInterpolatedMatrix4(t, ref mat);
            }
        }

        public int GetSizeBytes()
        {
            return position.GetSizeBytes() + rotation.GetSizeBytes();
        }
    }

    public class SFAnimation : SFResource
    {
        public SFBoneAnimation[] bone_animations;

        public float max_time { get; private set; } = 0f;

        public override int Load(byte[] data, int offset, object custom_data)
        {
            MemoryStream ms = new MemoryStream(data, offset, data.Length - offset);
            BinaryReader br = new BinaryReader(ms);

            max_time = 0;

            br.ReadInt16();
            int bone_count = br.ReadInt32();
            bone_animations = new SFBoneAnimation[bone_count];

            for (int i = 0; i < bone_count; i++)
            {
                SFBoneAnimation ba = new SFBoneAnimation();
                bone_animations[i] = ba;

                int data1, data4, anim_count;
                float data2, data3;

                data1 = br.ReadInt32(); data2 = br.ReadSingle(); data3 = br.ReadSingle();
                data4 = br.ReadInt32(); anim_count = br.ReadInt32();
                ba.rotation = new InterpolatedQuaternion(anim_count);
                for (int j = 0; j < anim_count; j++)
                {
                    float[] q_data = new float[5];
                    for (int k = 0; k < 5; k++)
                    {
                        q_data[k] = br.ReadSingle();
                    }

                    Quaternion q = new Quaternion(q_data[1], q_data[2], q_data[3], q_data[0]);
                    ba.rotation.Add(q, q_data[4]);
                }

                data1 = br.ReadInt32(); data2 = br.ReadSingle(); data3 = br.ReadSingle();
                data4 = br.ReadInt32(); anim_count = br.ReadInt32();
                ba.position = new InterpolatedVector3(anim_count);
                for (int j = 0; j < anim_count; j++)
                {
                    float[] p_data = new float[4];
                    for (int k = 0; k < 4; k++)
                    {
                        p_data[k] = br.ReadSingle();
                    }

                    Vector3 v = new Vector3(p_data[0], p_data[1], p_data[2]);
                    ba.position.Add(v, p_data[3]);
                }
                ba.ResolveStatic();

                max_time = Math.Max(ba.position.GetMaxTime(), max_time);
            }

            RAMSize = 0;
            for (int i = 0; i < bone_animations.Length; i++)
            {
                RAMSize += bone_animations[i].GetSizeBytes();
            }

            return 0;
        }
    }
}
