using System;
using System.Text;
using UnityEngine;

namespace Body.Behavior.ContextSteering
{
    public class CSMapping
    {
        // Enums
        public enum MapType { Interest, Danger, None }
        const sbyte POS = 1;
        const sbyte NA = 0;
        const sbyte NEG = -1;

        // Resolution
        static public int resolution = 12;

        // Baseline
        static private Vector3[] baseline;
        static public Vector3[] Baseline
        {
            get { return baseline ?? InitializeBaseline(); }
            set => baseline = value;
        }
        static private Vector3[] InitializeBaseline()
        {
            baseline = new Vector3[resolution];
            for (int i = 0; i < resolution; i++)
            {
                float angle = Mathf.Lerp(0, 360, (float)i / resolution);
                baseline[i] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            }
            return baseline;
        }

        // Map Structs

        [Serializable]
        public struct Map
        {
            public Map(float[] dirs, sbyte sign)
            {
                this.dirs = dirs ?? new float[resolution];
                this.sign = sign;
                this.valid = true;
            }
            public Map(float[] dirs, sbyte sign, bool initialize)
            {
                this.dirs = !initialize ? dirs : (dirs ?? new float[resolution]);
                this.sign = sign;
                this.valid = true;
            }
            //public float[] dirs;
            public float[] dirs;
            public sbyte sign;
            public bool valid;
            public int Length => dirs.Length;
            public float this[int index]
            {
                get { return dirs[index]; }
                set { dirs[index] = value; }
            }
            public new string ToString()
            {
                return dirs.ToString();
            }
        }

        [Serializable]
        public struct Maps
        {
            public Maps(Map interests, Map dangers)
            {
                this.interests = interests;
                this.dangers = dangers;
            }
            public Maps(float[] interests, float[] dangers)
            {
                this.interests = new(interests, POS);
                this.dangers = new(dangers, NEG);
            }
            public Map interests;
            public Map dangers;
            public int Length => 2;
            public Map this[int index]
            {
                get
                {
                    return index switch
                    {
                        0 => interests,
                        1 => dangers,
                        _ => new(),
                    };
                }
            }
            public Map this[MapType type]
            {
                get
                {
                    return type switch
                    {
                        MapType.Interest => interests,
                        MapType.Danger => dangers,
                        _ => new(),
                    };
                }
            }
        }

        static public void PrintVectors(Vector3[] vectors, string label)
        {
            StringBuilder b = new();
            b.Append(label);
            b.Append(" ");
            b.AppendJoin(", ", vectors);
            Debug.Log(b);
        }
    }
}