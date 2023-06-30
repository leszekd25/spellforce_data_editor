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
        public Quaternion rotation;
        public Vector3 position;

        public BoneAnimationState(ref Matrix4 transform)
        {
            rotation = transform.ExtractRotation(false);
            position = transform.Row3.Xyz;
        }

        public void ToMatrix(out Matrix4 transform)
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

            transform.Row3 = new Vector4(position, 1);
        }

        public static void Multiply(ref BoneAnimationState bas1, ref BoneAnimationState bas2, out BoneAnimationState result)
        {
            Quaternion qtmp = bas2.rotation;
            Vector3 tmp = bas1.position;

            FastQMultiply(ref bas2.rotation, ref bas1.rotation, out result.rotation);
            FastTransform(ref bas1.position, ref qtmp, out tmp);
            Vector3.Add(ref tmp, ref bas2.position, out result.position);
        }

        // is it faster than Vector3.Transform? that's yet to be tested
        public static void FastTransform(ref Vector3 vec, ref Quaternion quat, out Vector3 result)
        {
            Vector3 xyz = quat.Xyz;
            Vector3 d = new Vector3(
                -vec.X * xyz.Y + vec.Y * xyz.X + vec.Z * quat.W,
                vec.X * quat.W - vec.Y * xyz.Z + vec.Z * xyz.Y,
                vec.X * xyz.Z + vec.Y * quat.W - vec.Z * xyz.X
                );
            Vector3 temp = new Vector3(
                xyz.Y * d.X - xyz.Z * d.Z,
                xyz.Z * d.Y - xyz.X * d.X,
                xyz.X * d.Z - xyz.Y * d.Y
                );
            Vector3.Multiply(ref temp, 2f, out temp);
            Vector3.Add(ref vec, ref temp, out result);
        }

        public static void FastLerp(ref BoneAnimationState bas1, ref BoneAnimationState bas2, float blend, out BoneAnimationState result)
        {
            FastQSlerp(ref bas1.rotation, ref bas2.rotation, blend, out result.rotation);
            FastVLerp(ref bas1.position, ref bas2.position, blend, out result.position);
        }

        public static void FastVLerp(ref Vector3 v1, ref Vector3 v2, float blend, out Vector3 vr)
        {
            vr.X = blend * (v2.X - v1.X) + v1.X;
            vr.Y = blend * (v2.Y - v1.Y) + v1.Y;
            vr.Z = blend * (v2.Z - v1.Z) + v1.Z;
        }

        public static void FastQMultiply(ref Quaternion q1, ref Quaternion q2, out Quaternion qr)
        {
            Vector3 xyz1 = q1.Xyz;
            Vector3 xyz2 = q2.Xyz;
            qr = new Quaternion(
                new Vector3(
                    xyz1.X * q2.W + q1.W * xyz2.X + xyz1.Y * xyz2.Z - xyz1.Z * xyz2.Y,
                    xyz1.Y * q2.W + q1.W * xyz2.Y + xyz1.Z * xyz2.X - xyz1.X * xyz2.Z,
                    xyz1.Z * q2.W + q1.W * xyz2.Z + xyz1.X * xyz2.Y - xyz1.Y * xyz2.X),
                q1.W * q2.W - xyz1.X * xyz2.X - xyz1.Y * xyz2.Y - xyz1.Z * xyz2.Z);
        }

        public static void FastQSlerp(ref Quaternion q1, ref Quaternion q2, float blend, out Quaternion qr)
        {
            float cosHalfAngle = q1.W * q2.W + Vector3.Dot(q1.Xyz, q2.Xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                qr = q1;
                return;
            }
            else if (cosHalfAngle < 0.0f)
            {
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)System.Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            Quaternion tmp = new Quaternion(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
            Quaternion.Normalize(ref tmp, out qr);
        }

        public BoneAnimationState Inverse()
        {
            ToMatrix(out Matrix4 inv);
            inv.Invert();
            return new BoneAnimationState(ref inv);
        }

        public override string ToString()
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

            MemoryStream ms = new MemoryStream(data, offset, data.Length - offset);
            BinaryReader br = new BinaryReader(ms);

            max_time = 0;
            InterpolatedVector3 position = new InterpolatedVector3();
            InterpolatedQuaternion rotation = new InterpolatedQuaternion();

            br.ReadInt16();
            int bone_count = br.ReadInt32();
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

                bone_animations[i] = new BoneAnimationState[(int)Math.Ceiling(max_time * ANIMATION_FPS) + 1];    // 25 frames per second

                for (int k = 0; k < bone_animations[i].Length - 1; k++)
                {
                    float t = k / ANIMATION_FPS;
                    bone_animations[i][k] = new BoneAnimationState() { position = position.Get(t), rotation = rotation.Get(t) };
                }
                bone_animations[i][bone_animations[i].Length - 1] = new BoneAnimationState() { position = position.Get(max_time), rotation = rotation.Get(max_time) };
            }

            for (int k = 0; k < bone_animations[0].Length; k++)
            {
                for (int i = 0; i < bone_count; i++)
                {
                    if (skel.bone_parents[i] != Utility.NO_INDEX)
                    {
                        BoneAnimationState.Multiply(ref bone_animations[i][k], ref bone_animations[skel.bone_parents[i]][k], out bone_animations[i][k]);
                    }
                }
            }

            RAMSize = bone_animations.Length * bone_animations[0].Length * 28;

            return 0;
        }
    }
}
