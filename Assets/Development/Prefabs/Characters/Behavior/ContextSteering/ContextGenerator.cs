using System.Collections.Generic;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSContext;
using static Body.Behavior.ContextSteering.CSIdentity;

[System.Serializable]
public class ContextGenerator
{
    [System.Serializable]
    public struct ContextGenerationData
    {
        public Identity identity;
        [Tooltip("A minimum/maximum distance pair indicating the lowest and highest ranges.")]
        public Vector2 range;
        public bool magnetized;
        [Tooltip("A list of distance/radius pairs indicating where deadzones should be positioned.")]
        public List<Vector2> deadzones;
        public float falloff;
    }

    public List<ContextGenerationData> generationData;

    // Setup
    public void GenerateContexts(ref Dictionary<Identity, List<Context>> contextMap)
    {
        contextMap ??= new();
        for (int i = 0; i < generationData.Count; i++)
            GenerateContexts(contextMap, generationData[i]);
    }

    public void GenerateContexts(Dictionary<Identity, List<Context>> contextMap, ContextGenerationData data)
    {
        if (!contextMap.TryGetValue(data.identity, out var contexts))
        {
            contexts = new ();
            contextMap[data.identity] = contexts;
        }
        GenerateContexts(contexts, data);
    }
    
    public void GenerateContexts(List<Context> contexts)
    {
        for (int i = 0; i < generationData.Count; i++)
            GenerateContexts(contexts, generationData[i]);
    }

    // Generate
    public void GenerateContexts(List<Context> contexts, ContextGenerationData data)
    {
        List<Vector2> deadzones = data.deadzones;
        for (int i = 0; i < deadzones.Count; i++)
        {
            Vector2 lowRange = new();
            Vector2 highRange = new();
            if (i == 0)
            {
                lowRange.x = data.range.x;
                lowRange.y = deadzones[i].x - deadzones[i].y;
            }
            else
            {
                lowRange.x = (deadzones[i].x + deadzones[i - 1].x) / 2;
                lowRange.y = deadzones[i].x - deadzones[i].y;
            }
            if (i == deadzones.Count - 1)
            {
                highRange.x = deadzones[i].x + deadzones[i].y;
                highRange.y = data.range.y;
            }
            else
            {
                highRange.x = deadzones[i].x + deadzones[i].y;
                highRange.y = (deadzones[i].x + deadzones[i + 1].x) / 2;
            }
            contexts.Add(GenerateContext(Range.Close, lowRange, data));
            contexts.Add(GenerateContext(Range.Far, highRange, data));
        }
        contexts.Add(GenerateContext(Range.InRange, data.range, data));
    }

    public Context GenerateContext(Range rangeType, Vector2 range, ContextGenerationData data)
    {
        Vector2 weight = Weight(rangeType, data.magnetized);
        return new(data.identity, rangeType, weight, range, range, data.falloff);
    }

    private readonly Vector2[] forms = new Vector2[] {new(1, 0), new(0, 1)};
    private Vector2 Weight(Range range, bool magnetized)
    {
        float sign = range switch { Range.Far => 1, Range.Close => -1, _ => 0 };
        Vector2 form = forms[((sign > 0 ? 1 : 0) + (magnetized ? 1 : 0)) % forms.Length];
        return form * sign;
    }
}
