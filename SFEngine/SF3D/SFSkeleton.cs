/*
 * SFSkeleton is a resource providing with skin transformation bone data
 * It contains bone tree and constructs base and inverted matrices from given file
 * It also contains helper method for applying parent transformations to given matrices
 */

using OpenTK.Mathematics;
using SFEngine.SFResources;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SFEngine.SF3D
{
    public class SFSkeleton : SFResource
    {
        public const int MAX_BONE_PER_CHUNK = 20;
        public int bone_count = 0;
        //public Matrix4[] bone_reference_matrices = null;
        //public Matrix4[] bone_inverted_matrices = null;
        public BoneAnimationState[] bone_reference_state = null;
        public BoneAnimationState[] bone_inverted_state = null;
        public int[] bone_parents = null;
        public string[] bone_names = null;

        //helper function for loading vector from file
        private Vector3 Load_GetVector3(ReadOnlySpan<char> line)
        {
            int i = line.IndexOf(',');
            if (i == -1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFSkeleton.Load_GetVector3(): Line does not contain vector3! line: '" + line.ToString() + "')");
                throw new InvalidDataException("ERROR: Corrupted .bor file!");
            }
            int j = line.Slice(i + 2).IndexOf(',');
            if (j == -1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFSkeleton.Load_GetVector3(): Line does not contain vector3! line: '" + line.ToString() + "')");
                throw new InvalidDataException("ERROR: Corrupted .bor file!");
            }
            Vector3 vec = new Vector3();
            bool success = float.TryParse(line.Slice(0, i), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out vec.X);
            success &= float.TryParse(line.Slice(i + 2, j), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out vec.Y);
            success &= float.TryParse(line.Slice(i + j + 4), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out vec.Z);
            if (!success)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSkeleton.Load_GetVector3(): Could not successfully read vector3 (line: '" + line.ToString() + "')");
            }

            return vec;
        }

        public override int Load(byte[] data, int offset, object custom_data)
        {
            string file = System.Text.Encoding.Default.GetString(data);
            StringUtils.LineSplitEnumerator splitter = new StringUtils.LineSplitEnumerator(file);

            int current_bone = Utility.NO_INDEX;
            string current_bone_name = "";
            int file_level = 0;
            float Rre = 0f;
            Vector3 Rim = new Vector3();

            int line_index = 0;
            ReadOnlySpan<char> line = default;
            while(splitter.NextLine(ref line))
            {
                int line_start = 0;
                int line_end = line.Length - 1;
                for(;line_start < line_end; line_start++)
                {
                    if (!((line[line_start] == ' ') || (line[line_start] == '\t')))
                    {
                        break;
                    }
                }
                for(;line_end >= line_start; line_end--)
                {
                    if (!((line[line_end] == ' ') || (line[line_end] == '\t')))
                    {
                        break;
                    }
                }
                ReadOnlySpan<char> line_span = line.Slice(line_start, line_end - line_start + 1);
                line_index += 1;

                if (line_span.Length == 0)
                {
                    continue;
                }

                if (line_span[0] == '[')
                {
                    continue;
                }

                if (line_span[0] == '{')
                {
                    file_level++;
                    continue;
                }
                if (line_span[0] == '}')
                {
                    file_level--;
                    continue;
                }

                switch (file_level)
                {
                    case 0:
                        break;
                    case 1:
                        if (MemoryExtensions.Equals(line_span.Slice(0, 3), "NOB", StringComparison.Ordinal))
                        {
                            int bc;
                            if (!Int32.TryParse(line_span.Slice(6), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out bc))
                            {
                                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSkeleton.Load(): Failed to read bone count! line " + line_index.ToString() + ": '" + line.ToString() + "')");
                            }

                            bone_count = bc;
                            bone_parents = new int[bc];
                            bone_reference_state = new BoneAnimationState[bc];
                            bone_inverted_state = new BoneAnimationState[bc];
                            bone_names = new string[bc];
                        }
                        break;
                    case 2:
                        if (line_span[0] == 'N')
                        {
                            current_bone_name = line_span.Slice(5, line_span.Length-6).ToString();
                        }

                        if (line_span[0] == 'I')
                        {
                            if (!Int32.TryParse(line_span.Slice(5), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out current_bone))
                            {
                                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSkeleton.Load(): Failed to read bone index! line" + line_index.ToString() + ": '" + line.ToString() + "')");
                            }

                            bone_names[current_bone] = current_bone_name;
                        }
                        if (line_span[0] == 'F')
                        {
                            if (!Int32.TryParse(line_span.Slice(4), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out bone_parents[current_bone]))
                            {
                                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSkeleton.Load(): Failed to read bone parent! line" + line_index.ToString() + ": '" + line.ToString() + "')");
                            }
                        }

                        break;
                    case 3:
                        if (line_span[0] == 'P')
                        {
                            bone_reference_state[current_bone].position = Load_GetVector3(line_span.Slice(4));
                        }

                        if (MemoryExtensions.Equals(line_span.Slice(0, 3), "Rre", StringComparison.Ordinal))
                        {
                            if (!Single.TryParse(line_span.Slice(6), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out Rre))
                            {
                                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSkeleton.Load(): Failed to read bone Re(rotation)! line" + line_index.ToString() + ": '" + line.ToString() + "')");
                            }
                        }

                        if (MemoryExtensions.Equals(line_span.Slice(0, 3), "Rim", StringComparison.Ordinal))
                        {
                            Rim = Load_GetVector3(line_span.Slice(6));
                            bone_reference_state[current_bone].rotation = new Quaternion(Rim, Rre);
                        }
                        break;

                    default:
                        break;
                }

            }

            for (int i = 0; i < bone_count; i++)
            {
                if (bone_parents[i] != Utility.NO_INDEX)
                {
                    BoneAnimationState.Multiply(ref bone_reference_state[i], ref bone_reference_state[bone_parents[i]], out bone_reference_state[i]);
                }
                bone_inverted_state[i] = bone_reference_state[i].Inverse();
            }

            RAMSize = 4 + 124 * bone_count;  // matrix4, matrix4, int, string[64] = 196

            return 0;
        }

        public void CalculateTransformation(Matrix4[] src_matrices, ref Matrix4[] dest_matrices)
        {
            for (int i = 0; i < bone_count; i++)
            {
                if (bone_parents[i] != Utility.NO_INDEX)
                {
                    dest_matrices[i] = src_matrices[i] * dest_matrices[bone_parents[i]];
                }
            }
        }
    }
}
