/*
 * SFSkeleton is a resource providing with skin transformation bone data
 * It contains bone tree and constructs base and inverted matrices from given file
 * It also contains helper method for applying parent transformations to given matrices
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SpellforceDataEditor.SFResources;

namespace SpellforceDataEditor.SF3D
{

    public class SFSkeleton: SFResource
    {
        public int bone_count { get; private set; } = 0;
        public Matrix4[] bone_reference_matrices { get; private set; } = null;
        public Matrix4[] bone_inverted_matrices { get; private set; } = null;
        public int[] bone_parents = null;
        public string[] bone_names = null;
        string name = "";

        public void Init()
        {

        }

        //helper function for loading vector from file
        private Vector3 Load_GetVector3(string line)
        {
            int i = line.IndexOf(',');
            if (i == -1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFSkeleton.Load_GetVector3(): Line does not contain vector3! line: '" + line + "')");
                throw new InvalidDataException("ERROR: Corrupted .bor file!");
            }
            int j = line.Substring(i + 2).IndexOf(',');
            if (j == -1)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "SFSkeleton.Load_GetVector3(): Line does not contain vector3! line: '" + line + "')");
                throw new InvalidDataException("ERROR: Corrupted .bor file!");
            }
            Vector3 vec = new Vector3();
            bool success = float.TryParse(line.Substring(0, i), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out vec.X);
            success &= float.TryParse(line.Substring(i + 2, j), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out vec.Y);
            success &= float.TryParse(line.Substring(i + j + 4), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out vec.Z);
            if (!success)
                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSkeleton.Load_GetVector3(): Could not successfully read vector3 (line: '" + line + "')");
            return vec;
        }

        public int Load(MemoryStream ms)
        {
            StreamReader sr = new StreamReader(ms);

            Vector3[] bone_pos = null;
            Quaternion[] bone_rot = null;
            int current_bone = -1;
            string current_bone_name = "";
            int file_level = 0;
            float Rre = 0f;
            Vector3 Rim = new Vector3();

            int line_index = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine().Trim();
                line_index += 1;

                if (line == "")
                    continue;
                if (line[0] == '[')
                    continue;
                if(line[0] == '{')
                {
                    file_level++;
                    continue;
                }
                if(line[0] == '}')
                {
                    file_level--;
                    continue;
                }

                switch (file_level)
                {
                    case 0:
                        break;
                    case 1:
                        if (line.Substring(0, 3) == "NOB")
                        {
                            int bc;
                            if(!Int32.TryParse(line.Substring(6), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out bc))
                                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSkeleton.Load(): Failed to read bone count! line "+line_index.ToString()+": '" + line + "')");
                            bone_count = bc;
                            bone_parents = new int[bc];
                            bone_pos = new Vector3[bc];
                            bone_rot = new Quaternion[bc];
                            bone_reference_matrices = new Matrix4[bc];
                            bone_inverted_matrices = new Matrix4[bc];
                            bone_names = new string[bc];
                        }
                        break;
                    case 2:
                        if (line[0] == 'N')
                            current_bone_name = line.Substring(4).Replace("\"", string.Empty);
                        if (line[0] == 'I')
                        {
                            if(!Int32.TryParse(line.Substring(5), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out current_bone))
                                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSkeleton.Load(): Failed to read bone index! line" + line_index.ToString() + ": '" + line + "')");
                            bone_names[current_bone] = current_bone_name;
                        }
                        if (line[0] == 'F')
                            if(!Int32.TryParse(line.Substring(4), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out bone_parents[current_bone]))
                                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSkeleton.Load(): Failed to read bone parent! line" + line_index.ToString() + ": '" + line + "')");
                        break;
                    case 3:
                        if (line[0] == 'P')
                            bone_pos[current_bone] = Load_GetVector3(line.Substring(4));
                        if (line.Substring(0, 3) == "Rre")
                            if(!Single.TryParse(line.Substring(6), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out Rre))
                                LogUtils.Log.Warning(LogUtils.LogSource.SF3D, "SFSkeleton.Load(): Failed to read bone Re(rotation)! line" + line_index.ToString() + ": '" + line + "')");
                        if (line.Substring(0, 3) == "Rim")
                        {
                            Rim = Load_GetVector3(line.Substring(6));
                            bone_rot[current_bone] = new Quaternion(Rim, Rre);
                        }
                        break;
                        
                    default:
                        break;
                }

            }

            for (int i = 0; i < bone_count; i++)
            {
                Matrix4 temp_matrix = Matrix4.CreateFromQuaternion(bone_rot[i]);
				
                temp_matrix.Row3 = new Vector4(bone_pos[i], 1);
                bone_reference_matrices[i] = temp_matrix;
            
                if (bone_parents[i] != -1)
                    bone_reference_matrices[i] = bone_reference_matrices[i] * bone_reference_matrices[bone_parents[i]];
            
                bone_inverted_matrices[i] = bone_reference_matrices[i].Inverted();
            }
            
            return 0;
        }

        public void CalculateTransformation(Matrix4[] src_matrices, ref Matrix4[] dest_matrices)
        {
            for (int i = 0; i < bone_count; i++)
            {
                if (bone_parents[i] != -1)
                    dest_matrices[i] = src_matrices[i] * dest_matrices[bone_parents[i]];
            }
        }

        public void SetName(string s)
        {
            name = s;
        }

        public string GetName()
        {
            return name;
        }

        public int GetSizeBytes()
        {
            return 132 * bone_count;
        }

        public void Dispose()
        {

        }
    }
}
