using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class CastingVFXController : MonoBehaviour
{
    private VisualEffect castingVFX;

    [SerializeField] private float[] levelCharges;

    [SerializeField] private float castingCharge; //0 to 1 scale that determines how far into the casting of the spell the VFX are
    [SerializeField] private float castingTime; //how long the spell will run before the casting VFX should be dismissed
    [SerializeField] private float castingWindUpTime = 0; //float representing additional charge on castingCharge before tracking 0 to 1. This is for a precasting wind-up. Only applied to level 3 casts. 
    [SerializeField] private float timeToDestruct = 1; //duration in seconds before the GameObject is destroyed after enabling castingEnd

    [SerializeField] private bool updateCharge;

    private int currentChargeLevel;

    private void Awake()
    {
        castingVFX = GetComponent<VisualEffect>();
    }

    private void Update()
    {
        if (!updateCharge) return;

        IncrementSpellCharges();
        IncrementCastingCharge();
    }

    private void IncrementSpellCharges()
    {
        if (currentChargeLevel >= levelCharges.Length) return;

        levelCharges[currentChargeLevel] += Time.deltaTime;
        if (levelCharges[currentChargeLevel] >= 1.0f)
        {
            currentChargeLevel++;
        }

        UpdateCharges();
    }

    public void IncrementCastingCharge()
    {
        castingCharge += Time.deltaTime;
        if(castingCharge >= castingTime + castingWindUpTime)
        {
            //EndCasting();
        }
    }

    private void EndCasting()
    {
        StartCoroutine(EndCastingCoroutine());
    }

    public void SetCastingTime(float t)
    {
        castingTime = t;
    }

    public void SetCastingWindUpTime(float t)
    {
        castingWindUpTime = t;
    }

    private IEnumerator EndCastingCoroutine()
    {
        castingVFX.SetBool("CastingEnd", true);
        yield return new WaitForSeconds(timeToDestruct);
        Debug.Log("Should be destroyed now.");
        Destroy(gameObject);
    }

    private void UpdateCharges()
    {
        castingVFX.SetFloat("Level 3 Charge", levelCharges[2]);
        castingVFX.SetFloat("Level 2 Charge", levelCharges[1]);
        castingVFX.SetFloat("Level 1 Charge", levelCharges[0]);
        castingVFX.SetFloat("Casting", castingCharge);
    }
} 