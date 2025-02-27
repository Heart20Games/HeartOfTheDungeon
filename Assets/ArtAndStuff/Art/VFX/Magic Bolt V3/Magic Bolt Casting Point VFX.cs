using HotD.Castables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.VFX;

public class MagicBoltCastingPointVFX : BaseMonoBehaviour
{
    public VisualEffect boltCastingPoint;
    public ParticleSystem boltExplosion;
    public float elapsedTime;
    public float castDuration;
    private float currentPosition;
    private bool playing = false;
    public UnityEvent onEnable = new();

    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = 0f;
        currentPosition = 0f;
    }

    bool hasEnabled = false;
    private void OnEnable()
    {
        onEnable.Invoke();
        hasEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveAndEnabled)
        {
            if (!playing)
            {
                boltCastingPoint.SetFloat("Duration", 0f);
                boltExplosion.Play();
                boltCastingPoint.Play();
                playing = true;
            } 
            else if (elapsedTime < castDuration)
            {
                elapsedTime += Time.deltaTime;
                currentPosition = elapsedTime/castDuration;
                boltCastingPoint.SetFloat("Duration", currentPosition);
            }
            else
            {
                boltCastingPoint.Stop();
                enabled = false;
                playing = false;
                elapsedTime = 0f;
                currentPosition = 0f;

                Assert.IsTrue(hasEnabled, "Somehow CP_VFX is being disabled despite not having been enabled?");
                if (!hasEnabled)
                {
                    Break(true, this, "Somehow CP_VFX is being disabled despite not having been enabled?");
                }
            }
        }
    }
}
