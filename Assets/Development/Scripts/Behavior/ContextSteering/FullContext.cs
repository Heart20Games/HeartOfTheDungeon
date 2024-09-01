using Body.Behavior.ContextSteering;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static Body.Behavior.ContextSteering.CSContext;
using static Body.Behavior.ContextSteering.CSIdentity;

// Context
[Serializable]
public class FullContext
{
    public Dictionary<Identity, List<Context>> baseContext;
    public Dictionary<Identity, List<Context>> castableContext;

    public Identity currentIdentity;
    public List<Context> currentContexts;
    public bool initialized = false;
    public bool debug = false;

    public FullContext Initialize(CSPreset preset)
    {
        if (!initialized)
        {
            Assert.IsTrue(preset != null, "Preset is null when initializing FullContext.");
            CSContext csContext = preset.GetContext();
            if (csContext != null)
            {
                Assert.IsFalse(csContext == null, $"CSContext is null when initializing FullContext for preset {preset.name}");
                baseContext ??= csContext.ContextMap; //Body.Behavior.Brain.Action.Chase, debug).ContextMap;
                initialized = true;
            }
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
                if (debug && cContext.Count > 0) Debug.Log($"{cContext.Count} Castable Contexts");
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
