using Body.Behavior.ContextSteering;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static Body.Behavior.ContextSteering.CSContext;
using static Body.Behavior.ContextSteering.CSIdentity;

// Context
public class FullContext
{
    public Dictionary<Identity, List<Context>> baseContext;
    public Dictionary<Identity, List<Context>> castableContext;

    [HideInInspector] public Identity currentIdentity;
    [HideInInspector] public List<Context> currentContexts;
    [HideInInspector] public bool initialized = false;
    [HideInInspector] public bool debug = false;

    public FullContext Initialize(CSPreset preset)
    {
        if (!initialized)
        {
            baseContext ??= preset.GetContext(Body.Behavior.Brain.Action.Chase, debug).ContextMap;
            initialized = true;
        }
        return this;
    }

    public List<Context> this[Identity identity]
    {
        get
        {
            currentContexts ??= new();
            currentContexts.Clear();
            if (castableContext != null && castableContext.TryGetValue(identity, out List<Context> cContext))
            {
                currentContexts.AddRange(cContext);
            }
            if (baseContext != null && baseContext.TryGetValue(identity, out List<Context> bContext))
            {
                currentContexts.AddRange(bContext);
            }
            currentIdentity = identity;
            return currentContexts;
        }
    }
    public bool TryGet(Identity identity, out List<Context> result)
    {
        result = this[identity];
        return result != null;
    }
}