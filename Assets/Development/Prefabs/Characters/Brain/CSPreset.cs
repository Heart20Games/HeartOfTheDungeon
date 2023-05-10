using UnityEngine;
using static CSIdentity;
using static CSContext;

[CreateAssetMenu(fileName = "CSPreset", menuName = "Context Steering/Preset", order = 1)]
public class CSPreset : ScriptableObject
{
    public float testSpeed = 3f;
    public float drawScale = 1.0f;
    public bool draw;

    public CSContext contextPreset;
    public CSIdentity identityPreset;

    public Contexts Contexts { get { return contextPreset.contexts; } }
    public Identity Identity { get { return identityPreset.identity; } }
    public IdentityMapPair[] Pairs { get { return identityPreset.pairs; } }
}
