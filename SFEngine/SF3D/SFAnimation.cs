/*
 * SFBoneAnimation contains info on how a single bone transform changes over time
 * SFAnimation is a set of SFBoneAnimation objects corresponding to a supplied skeleton
 */

using OpenTK;
using SFEngine.SFResources;
using System;
using System.IO;

namespace SFEngine.SF3D
{
    public struct BoneAnimationState
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public class SFBoneAnimation
    {
        public bool is_static = false;
        public Matrix4 static_transform;
        public BoneAnimationState[] state_quantized;

        public int GetSizeBytes()
        {
            if(is_static)
            {
                return 64;
            }
            else
            {
                return state_quantized.Length * 28;
            }
        }
    }

    public class SFAnimation : SFResource
    {
        public const float ANIMATION_FPS = 25.0f;

        public SFBoneAnimation[] bone_animations;

        public float max_time { get; private set; } = 0f;

        public override int Load(byte[] data, int offset, object custom_data)
        {
            MemoryStream ms = new MemoryStream(data, offset, data.Length - offset);
            BinaryReader br = new BinaryReader(ms);

            max_time = 0;
            InterpolatedVector3 position = new InterpolatedVector3();
            InterpolatedQuaternion rotation = new InterpolatedQuaternion();

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
                rotation.Reset(anim_count);
                for (int j = 0; j < anim_count; j++)
                {
                    float[] q_data = new float[5];
                    for (int k = 0; k < 5; k++)
                    {
                        q_data[k] = br.ReadSingle();
                    }

                    Quaternion q = new Quaternion(q_data[1], q_data[2], q_data[3], q_data[0]);
                    rotation.Add(q, q_data[4]);
                }
                rotation.ResolveStatic();

                data1 = br.ReadInt32(); data2 = br.ReadSingle(); data3 = br.ReadSingle();
                data4 = br.ReadInt32(); anim_count = br.ReadInt32();
                position.Reset(anim_count);
                for (int j = 0; j < anim_count; j++)
                {
                    float[] p_data = new float[4];
                    for (int k = 0; k < 4; k++)
                    {
                        p_data[k] = br.ReadSingle();
                    }

                    Vector3 v = new Vector3(p_data[0], p_data[1], p_data[2]);
                    position.Add(v, p_data[3]);
                }
                position.ResolveStatic();

                max_time = Math.Max(position.GetMaxTime(), max_time);

                if ((position.is_static) && (rotation.is_static))
                {
                    ba.is_static = true;
                    ba.static_transform = Matrix4.CreateFromQuaternion(rotation.Get(0));
                    ba.static_transform.Row3 = new Vector4(position.Get(0), 1.0f);
                }
                else
                {
                    ba.is_static = false;
                    ba.state_quantized = new BoneAnimationState[(int)Math.Ceiling(max_time * ANIMATION_FPS) + 1];    // 25 frames per second
                    for (int k = 0; k < ba.state_quantized.Length - 1; k++)
                    {
                        float t = k / ANIMATION_FPS;
                        ba.state_quantized[k] = new BoneAnimationState() { position = position.Get(t), rotation = rotation.Get(t) };
                    }
                    ba.state_quantized[ba.state_quantized.Length - 1] = new BoneAnimationState() { position = position.Get(max_time), rotation = rotation.Get(max_time) };
                }
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
