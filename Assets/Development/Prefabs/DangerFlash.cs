using UnityEngine;

public class DangerFlash : BaseMonoBehaviour
{
    public float flashDuration = 1.0f;
    public float flashInterval = 0.1f;
    private float flashTimer;
    private float flashCounter;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        flashTimer += Time.deltaTime;
        if (flashTimer >= flashInterval)
        {
            flashCounter += flashInterval;
            flashTimer = 0;
            rend.enabled = !rend.enabled;
        }

        if (flashCounter >= flashDuration)
        {
            rend.enabled = true;
            flashCounter = 0;
            flashTimer = 0;
        }
    }
}
