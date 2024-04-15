using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Level3BoltScaling : MonoBehaviour
{
    public GameObject meshCollider;
    public VisualEffect bolt;
    public float scaleSpeed;
    public float castDuration;
    private bool playing = false;
    [SerializeField] private float currentScale;

    // Start is called before the first frame update
    void Start()
    {
        currentScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveAndEnabled)
        {
            if (!playing)
            {
                bolt.SetFloat("Duration", castDuration);
                bolt.SetBool("Effect End", false);
                bolt.Play();
                playing = true;
            } 
            if (currentScale < 50f)
            {
                currentScale += Time.deltaTime * scaleSpeed;
                currentScale = Mathf.Clamp(currentScale, 0f, 50f);
            }
            else if (currentScale >= 50 && (currentScale - 50) < castDuration)
            {
                currentScale += Time.deltaTime;
            }
            else if (currentScale >= 50 && (currentScale - 50) >= castDuration)
            {
                bolt.Stop();
                currentScale = 0f;
                enabled = false;
                playing = false;
                bolt.SetBool("Effect End", true);
            }
            else if (currentScale != 0f) currentScale = 0;
        }
        else if (currentScale != 0f) currentScale = 0;

        bolt.SetFloat("Scale", currentScale);
        meshCollider.transform.localScale = new Vector3(meshCollider.transform.localScale.x, meshCollider.transform.localScale.y, (currentScale * 100));
    }
}
