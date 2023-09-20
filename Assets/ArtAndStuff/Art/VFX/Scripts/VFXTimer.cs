using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class VFXTimer : MonoBehaviour
{
    [SerializeField] private string[] properties;
    [SerializeField] private float range = 1;
    public UnityEvent<string, float> sendToTargets;

    [SerializeField] private bool wrap;
    [ConditionalField("wrap", false)]
    [SerializeField] private bool clamp;
    [SerializeField] private float elapsedTime;

    private void OnEnable()
    {
        properties ??= new string[0];
        sendToTargets ??= new();
    }

    private void Update()
    {
        if (wrap)
        {
            elapsedTime = Mathf.Repeat(elapsedTime + Time.deltaTime, range);
        }
        else if (clamp)
        {
            elapsedTime = Mathf.Clamp(elapsedTime + Time.deltaTime, 0, range);
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
        foreach (string property in properties)
        {
            sendToTargets.Invoke(property, elapsedTime);
        }
    }
}
