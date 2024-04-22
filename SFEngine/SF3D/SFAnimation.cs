/*
 * SFBoneAnimation contains info on how a single bone transform changes over time
 * SFAnimation is a set of SFBoneAnimation objects corresponding to a supplied skeleton
 */

using OpenTK.Mathematics;
using SFEngine.SFResources;
using System;
using System.IO;

namespace SFEngine.SF3D
{
    public struct BoneAnimationState(in Matrix4 transform)
    {
        static long INVOCATIONS = 0;
        static long B11Match = 0;
        static long B12Match = 0;
        static long B13Match = 0;
        static long B21Match = 0;
        static long B22Match = 0;

        public Quaternion rotation = transform.ExtractRotation(false);
        public Vector3 position = transform.Row3.Xyz;

        public readonly void ToMatrix(out Matrix4 transform)
        {
            Vector3 xyz = rotation.Xyz;
            float sqx = xyz.X * xyz.X;
            float sqy = xyz.Y * xyz.Y;
            float sqz = xyz.Z * xyz.Z;
            float sqw = rotation.W * rotation.W;

            float xy = xyz.X * xyz.Y;
            float xz = xyz.X * xyz.Z;
            float xw = xyz.X * rotation.W;

            float yz = xyz.Y * xyz.Z;
            float yw = xyz.Y * rotation.W;

            float zw = xyz.Z * rotation.W;

            float s2 = 2f / (sqx + sqy + sqz + sqw);

            transform.Row0.X = 1f - (s2 * (sqy + sqz));
            transform.Row1.Y = 1f - (s2 * (sqx + sqz));
            transform.Row2.Z = 1f - (s2 * (sqx + sqy));

            transform.Row0.Y = s2 * (xy + zw);
            transform.Row1.X = s2 * (xy - zw);

            transform.Row2.X = s2 * (xz + yw);
            transform.Row0.Z = s2 * (xz - yw);

            transform.Row2.Y = s2 * (yz - xw);
            transform.Row1.Z = s2 * (yz + xw);

            transform.Row0.W = 0;
            transform.Row1.W = 0;
            transform.Row2.W = 0;

            transform.Row3 = new(position, 1);
        }

        public void FromMatrix(in Matrix4 mat)
        {
            position = mat.Row3.Xyz;
            rotation = Quaternion.FromMatrix(new Matrix3(mat));
        }

        public static void Multiply(in BoneAnimationState bas1, in BoneAnimationState bas2, out BoneAnimationState result)
        {
            Quaternion qtmp = bas2.rotation;

            Quaternion.Multiply(in bas2.rotation, in bas1.rotation, out result.rotation);
            Vector3.Transform(in bas1.position, in qtmp, out Vector3 tmp);
            Vector3.Add(in tmp, in bas2.position, out result.position);
        }

        public static void FastLerp(in BoneAnimationState bas1, in BoneAnimationState bas2, float blend, out BoneAnimationState result)
        {
            FastQSlerp(in bas1.rotation, in bas2.rotation, blend, out result.rotation);
            FastVLerp(in bas1.position, in bas2.position, blend, out result.position);
        }

        public static void FastVLerp(in Vector3 v1, in Vector3 v2, float blend, out Vector3 vr)
        {
            vr.X = blend * (v2.X - v1.X) + v1.X;
            vr.Y = blend * (v2.Y - v1.Y) + v1.Y;
            vr.Z = blend * (v2.Z - v1.Z) + v1.Z;
        }

        public static void FastQSlerp(in Quaternion q1, in Quaternion q2, float blend, out Quaternion qr)
        {
            Quaternion q2_copy = q2;

            float cosHalfAngle = q1.W * q2_copy.W + Vector3.Dot(q1.Xyz, q2_copy.Xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                qr = q1;
                return;
            }
            else if (cosHalfAngle < 0.0f)
            {
                q2_copy.Xyz = -q2_copy.Xyz;
                q2_copy.W = -q2_copy.W;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = MathF.Acos(cosHalfAngle);
                float sinHalfAngle = MathF.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = MathF.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = MathF.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            Quaternion tmp = new(blendA * q1.Xyz + blendB * q2_copy.Xyz, blendA * q1.W + blendB * q2_copy.W);
            qr = tmp;
        }

        public readonly BoneAnimationState Inverse()
        {
            ToMatrix(out Matrix4 inv);
            Matrix4.Invert(in inv, out inv);
            return new BoneAnimationState(in inv);
        }

        public override readonly string ToString()
        {
            return $"{position}, {rotation}";
        }
    }

    public class SFAnimation : SFResource
    {
        public const float ANIMATION_FPS = 25.0f;

        public BoneAnimationState[][] bone_animations;

        public float max_time { get; private set; } = 0f;

        public override int Load(byte[] data, int offset, object custom_data)
        {
            SFSkeleton skel = (SFSkeleton)custom_data;
            if(skel == null)
            {
                return -10;
            }

            MemoryStream ms = new(data, offset, data.Length - offset);
            BinaryReader br = new(ms);

            max_time = 0;
            InterpolatedVector3 position = new();
            InterpolatedQuaternion rotation = new();

            br.ReadInt16();
            int bone_count = br.ReadInt32();
            if(skel.bone_count != bone_count)
            {
                return -11;
            }
            bone_animations = new BoneAnimationState[bone_count][];

            for (int i = 0; i < bone_count; i++)
            {
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

                    Quaternion q = new(q_data[1], q_data[2], q_data[3], q_data[0]);
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

                    Vector3 v = (p_data[0], p_data[1], p_data[2]);
                    position.Add(v, p_data[3]);
                }
                position.ResolveStatic();

                max_time = Math.Max(position.GetMaxTime(), max_time);

                bone_animations[i] = new BoneAnimationState[(int)Math.Ceiling(max_time * ANIMATION_FPS) + 1];    // 25 frames per second

                for (int k = 0; k < bone_animations[i].Length - 1; k++)
                {
                    float t = k / ANIMATION_FPS;
                    bone_animations[i][k] = new BoneAnimationState() { position = position.Get(t), rotation = rotation.Get(t) };
                }
                bone_animations[i][^1] = new BoneAnimationState() { position = position.Get(max_time), rotation = rotation.Get(max_time) };
            }

            for (int k = 0; k < bone_animations[0].Length; k++)
            {
                for (int i = 0; i < bone_count; i++)
                {
                    if (skel.bone_parents[i] != Utility.NO_INDEX)
                    {
                        BoneAnimationState.Multiply(in bone_animations[i][k], in bone_animations[skel.bone_parents[i]][k], out bone_animations[i][k]);
                    }
                }
            }

            RAMSize = bone_animations.Length * bone_animations[0].Length * 28;

            return 0;
        }
    }
}
