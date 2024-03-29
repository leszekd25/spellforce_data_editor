﻿/*
 * IInterpolatedValue<T> is an interface for an InterpolatedValue class
 * Classes implementing IInterpolatedValue must be able to add new entries at a given time, retrieve
 *      interpolated values at a given time, and return maximum time available
 * Currently this interface is implemented by InterpolatedVector3 and InterpolatedQuaternion
 */

using OpenTK;
using System;

namespace SFEngine.SF3D
{
    public class InterpolatedFloat
    {
        Single[] value;
        float[] time;
        int cur_index = 0;
        float max_time = -1;

        public InterpolatedFloat(int capacity)
        {
            value = new float[capacity];
            time = new float[capacity];
        }

        public void Add(Single v, float t)
        {
            if (t >= max_time)
            {
                value[cur_index] = v;
                time[cur_index] = t;
                cur_index += 1;
                max_time = t;
            }
            else
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedFloat.Add(): Invalid time parameter (time = " + t.ToString() + ", max_time = " + max_time.ToString());
                throw new InvalidOperationException("Invalid time parameter (t <= max_time)");
            }
        }

        public Single Get(float t)
        {
            int size = value.Length;
            if (size == 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedFloat.Get(): Data is empty!");
                throw new IndexOutOfRangeException("Array is empty");
            }
            if (size == 1)
            {
                return value[0];
            }

            for (int i = 0; i < size; i++)
            {
                if (time[i] >= t)
                {
                    if (i == 0)
                    {
                        return value[0];
                    }

                    float t1 = time[i - 1];
                    return value[i - 1] + ((t - t1) / (time[i] - t1)) * (value[i] - value[i - 1]);
                }
            }

            LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedFloat.Get(): Data is malformed!!!!");
            throw new ArithmeticException("Invalid data");
        }

        public float GetMaxTime()
        {
            return max_time;
        }

        public int GetSizeBytes()
        {
            return 4 * time.Length + 4 * value.Length;
        }
    }

    public class InterpolatedVector3
    {
        Vector3[] value;
        float[] time;
        public bool is_static = false;
        int cur_index = 0;
        float max_time = -1;

        public void Reset(int capacity)
        {
            if ((value == null) || (value.Length < capacity))
            {
                value = new Vector3[capacity];
                time = new float[capacity];
            }
            cur_index = 0;
            max_time = 0;
        }

        public void ResolveStatic()
        {
            is_static = (cur_index == 1) || ((cur_index == 2) && (value[0] == value[1]));
        }

        public void Add(Vector3 v, float t)
        {
            if (t >= max_time)
            {
                value[cur_index] = v;
                time[cur_index] = t;
                cur_index += 1;
                max_time = t;
            }
            else
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedVector3.Add(): Invalid time parameter (time = " + t.ToString() + ", max_time = " + max_time.ToString());
                throw new InvalidOperationException("Invalid time parameter (t <= max_time)");
            }
        }

        public Vector3 Get(float t)
        {
            if (is_static)
            {
                return value[0];
            }

            if (cur_index < 10)
            {
                for (int i = 0; i < cur_index; i++)
                {
                    if (time[i] >= t)
                    {
                        if (i == 0)
                        {
                            return value[0];
                        }

                        float t1 = time[i - 1];
                        return Vector3.Lerp(value[i - 1], value[i], (t - t1) / (time[i] - t1));
                    }
                }
            }
            else
            {
                if (t == time[cur_index - 1])
                {
                    return value[cur_index - 1];
                }

                int min = 0; int max = cur_index;
                while (true)
                {
                    int mid_index = (min + max) / 2;
                    if (time[mid_index] <= t)
                    {
                        min = mid_index;
                    }
                    else
                    {
                        max = mid_index;
                    }
                    if (max - min == 1)
                    {
                        return Vector3.Lerp(value[min], value[max], (t - time[min]) / (time[max] - time[min]));
                    }
                }
            }

            LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedVector3.Get(): Data is malformed!!!!");
            throw new ArithmeticException("Invalid data");
        }

        public float GetMaxTime()
        {
            return max_time;
        }

        public int GetSizeBytes()
        {
            return 4 * time.Length + 12 * value.Length;
        }
    }

    public class InterpolatedQuaternion
    {
        Quaternion[] value;
        float[] time;
        public bool is_static = false;
        int cur_index = 0;
        float max_time = 0;

        public void Reset(int capacity)
        {
            if ((value == null) || (value.Length < capacity))
            {
                value = new Quaternion[capacity];
                time = new float[capacity];
            }
            cur_index = 0;
            max_time = 0;
        }

        public void ResolveStatic()
        {
            is_static = (cur_index == 1) || ((cur_index == 2) && (value[0] == value[1]));
        }


        public void Add(Quaternion v, float t)
        {
            if (t >= max_time)
            {
                value[cur_index] = v;
                time[cur_index] = t;
                cur_index += 1;
                max_time = t;
            }
            else
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedQuaternion.Add(): Invalid time parameter (time = " + t.ToString() + ", max_time = " + max_time.ToString());
                throw new InvalidOperationException("Invalid time parameter (t <= max_time)");
            }
        }

        public Quaternion Get(float t)
        {
            if (is_static)
            {
                return value[0];
            }

            // optimization: instead of reading data from start every time, read from last point it was read
            if (cur_index < 10)
            {
                for (int i = 0; i < cur_index; i++)
                {
                    if (time[i] >= t)
                    {
                        if (i == 0)
                        {
                            return value[0];
                        }

                        float t1 = time[i - 1];
                        return Quaternion.Slerp(value[i - 1], value[i], (t - t1) / (time[i] - t1));
                    }
                }
            }
            else
            {
                if (t == time[cur_index - 1])
                {
                    return value[cur_index - 1];
                }

                int min = 0; int max = cur_index;
                while (true)
                {
                    int mid_index = (min + max) / 2;
                    if (time[mid_index] <= t)
                    {
                        min = mid_index;
                    }
                    else
                    {
                        max = mid_index;
                    }
                    if (max - min == 1)
                    {
                        return Quaternion.Slerp(value[min], value[max], (t - time[min]) / (time[max] - time[min]));
                    }
                }
            }

            LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedQuaternion.Get(): Data is malformed!!!!");
            throw new ArithmeticException("Invalid data");
        }

        public float GetMaxTime()
        {
            return max_time;
        }

        public int GetSizeBytes()
        {
            return 4 * time.Length + 16 * value.Length;
        }
    }

    public class InterpolatedColor
    {
        Vector4[] value;
        float[] time;
        int cur_index = 0;
        float max_time = -1;

        public InterpolatedColor(int capacity)
        {
            value = new Vector4[capacity];
            time = new float[capacity];
        }

        public void Add(Vector4 v, float t)
        {
            if (t >= max_time)
            {
                value[cur_index] = v;
                time[cur_index] = t;
                cur_index += 1;
                max_time = t;
            }
            else
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedColor.Add(): Invalid time parameter (time = " + t.ToString() + ", max_time = " + max_time.ToString());
                throw new InvalidOperationException("Invalid time parameter (t <= max_time)");
            }
        }

        public Vector4 Get(float t)
        {
            int size = value.Length;
            if (size == 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedColor.Get(): Data is empty!");
                throw new IndexOutOfRangeException("Array is empty");
            }
            if (size == 1)
            {
                return value[0];
            }

            for (int i = 0; i < time.Length; i++)
            {
                if (time[i] >= t)
                {
                    if (i == 0)
                    {
                        return value[0];
                    }

                    float t1 = time[i - 1];
                    return Vector4.Lerp(value[i - 1], value[i], (t - t1) / (time[i] - t1));
                }
            }

            LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedColor.Get(): Data is malformed!!!!");
            throw new ArithmeticException("Invalid data");
        }

        public float GetMaxTime()
        {
            return max_time;
        }

        public int GetSizeBytes()
        {
            return 4 * time.Length + 16 * value.Length;
        }
    }
}
