using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FmodEventReferences : MonoBehaviour
{
    [field: Header("UI Sounds")]
    [field: SerializeField] public EventReference UIHoverLow { get; private set; }
    [field: SerializeField] public EventReference UIHoverHigh { get; private set; }
    [field: SerializeField] public EventReference UISelectHigh { get; private set; }
    [field: SerializeField] public EventReference UIMenuSelect { get; private set; }
    [field: SerializeField] public EventReference UIMenuSwipe { get; private set; }
    [field: SerializeField] public EventReference FloorPlate { get; private set; }
    public static FmodEventReferences instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Ref script in the scene.");
        }
        instance = this;
    }
}
