using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using static Body.Behavior.ContextSteering.CSIdentity;

namespace Body.Behavior.ContextSteering
{
    public class CSMapping
    {
        // Enums
        public const sbyte POS = 1;
        public const sbyte NEG = -1;
        public const sbyte NA = 0;
        public const sbyte NAN = 2;
        public enum MapType { Interest=POS, Danger=NEG, None=NA, Other=NAN }

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
                float angle = Mathf.Lerp(0, 360, (float)i / resolution) * Mathf.Deg2Rad;
                baseline[i] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            }
            return baseline;
        }

        // Map Structs

        [Serializable]
        public class Map
        {
            public Map()
            {
                dirs = null;
                componentCount = -1;
                sign = NAN;
                valid = false;
            }
            public Map(float[] dirs, sbyte sign)
            {
                this.dirs = dirs ?? new float[resolution];
                this.componentCount = 0;
                this.sign = sign;
                valid = true;
            }
            public Map(float[] dirs, sbyte sign, bool initialize)
            {
                this.dirs = !initialize ? dirs : (dirs ?? new float[resolution]);
                this.componentCount = 0;
                this.sign = sign;
                valid = true;
            }
            
            public float[] dirs;
            public int componentCount;
            public sbyte sign;
            public bool valid;
            public int Length => dirs.Length;

            public void IncrementComponentCount() { componentCount += 1; }
            public void ResetComponentCount() { componentCount = 0; }
            public float this[int index]
            {
                get { return dirs[index]; }
                set { dirs[index] = value; }
            }
            public bool IsZero()
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    if (dirs[i] != 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            public void Clear()
            {
                for (int i = 0; i < this.dirs.Length; i++)
                {
                    dirs[i] = 0;
                }
                componentCount = 0;
            }
            public new string ToString() { return dirs.ToString(); }
            public float MaxValue() { return Mathf.Max(dirs); }
        }

        [Serializable]
        public struct Maps
        {
            public Maps(Dictionary<Identity, Map> interests, Dictionary<Identity, Map> dangers)
            {
                this.interests = interests ?? new();
                this.dangers = dangers ?? new();
                this.vectors = new();
                this.counts = new();
                this.identities = new();
            }
            
            public Dictionary<Identity, Map> interests;
            public Dictionary<Identity, Map> dangers;
            public Dictionary<Identity, Vector3> vectors;
            public Dictionary<Identity, int> counts;
            private List<Identity> identities;
            public int Length => 2;

            // Indexing
            public Dictionary<Identity, Map> this[int i] { get { return i switch { 0 => interests, 1 => dangers, _ => null, }; } }
            public Dictionary<Identity, Map> this[MapType type] { get { return type switch { MapType.Interest => interests, MapType.Danger => dangers, _ => null, }; } }
            public Map this[MapType type, Identity identity]
            {
                get
                {
                    Dictionary<Identity, Map> maps = this[type];
                    if (!maps.ContainsKey(identity))
                    {
                        maps[identity] = type switch
                        {
                            MapType.Interest => new(null, POS, true),
                            MapType.Danger => new(null, NEG, true),
                            _ => null
                        };
                        identities.Add(identity);
                    }
                    return maps[identity];
                }
            }

            // Vector
            public void CalculateVector(Dictionary<Identity, Map> maps, sbyte sign, int i)
            {
                foreach (Identity identity in maps.Keys)
                {
                    Assert.IsFalse(float.IsNaN(maps[identity][i]));
                    if (!vectors.ContainsKey(identity))
                        vectors.Add(identity, new());
                    vectors[identity] += maps[identity][i] * sign * Baseline[i];
                    maps[identity][i] = 0;
                    Assert.IsFalse(float.IsNaN(vectors[identity].x) || float.IsNaN(vectors[identity].y) || float.IsNaN(vectors[identity].z));
                }
            }
            public void SumComponentCount(Dictionary<Identity, Map> maps, Identity identity)
            {
                if (interests.TryGetValue(identity, out Map map))
                {
                    counts[identity] += map.componentCount;
                    map.componentCount = 0;
                }
            }
            public void CalculateVectors()
            {
                vectors.Clear();
                counts.Clear();
                for (int i = 0; i < resolution; i++)
                {
                    CalculateVector(interests, POS, i);
                    CalculateVector(dangers, NEG, i);
                }
                foreach (Identity identity in vectors.Keys)
                {
                    counts[identity] = 0;
                    SumComponentCount(interests, identity);
                }
                foreach (Identity identity in counts.Keys)
                {
                    if (counts[identity] > 0)
                        vectors[identity] /= counts[identity];
                }
            }

            // Map Totals
            public float MapTotal(Dictionary<Identity, Map> maps, int idx)
            {
                float total = 0f;
                int count = 0;
                foreach (Identity identity in maps.Keys)
                {
                    total += maps[identity][idx];
                    count += maps[identity].componentCount;
                }
                return count > 0 ? total/count : 0;
            }
            public float this[int i, int j] { get => MapTotal(this[i], j); }

            // Max Value
            public float MaxValue()
            {
                float max = 0;
                for (int j = 0; j < resolution; j++)
                {
                    float iSum = 0f;
                    int iCount = 0;
                    float dSum = 0f;
                    float dCount = 0;
                    for (int i = 0; i < identities.Count; i++)
                    {
                        Identity identity = identities[i];
                        if (interests.TryGetValue(identity, out Map map))
                        {
                            iSum += map[j];
                            iCount += map.componentCount;
                        }
                        if (dangers.TryGetValue(identity, out map))
                        {
                            dSum += map[j];
                            dCount += map.componentCount;
                        }
                    }
                    if (iCount > 0)
                        max = Mathf.Max(max, iSum/iCount);
                    if (dCount > 0)
                        max = Mathf.Max(max, dSum/dCount);
                }
                return max;
            }

            // Sign
            public sbyte Sign(int idx)
            {
                return idx switch
                {
                    0 => POS,
                    1 => NEG,
                    _ => NA
                };
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