using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using HotD.Castables;
using static HotD.Castables.Coordination;
using static HotD.Castables.CastableToLocation;
using MyBox;

/*
 * This class is used to manage all of the VFX transitions for the Magic Bolt's Casting Point VFX.
 */

public class MagicBolt_ChargingVFXScript : ACastListener
{
    [Foldout("Settings", true)]
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private float[] chargeTimes;
    [SerializeField] private float castingChargeTime;
    [SerializeField] private CastLocation defaultLocation = CastLocation.FiringPoint;
    [SerializeField] private CastLocation finalLocation = CastLocation.WeaponPoint;
    [Range(-4, 4)][SerializeField] private int levelOffset;

    [Foldout("Status", true)]
    [SerializeField] private int level;
    [SerializeField] private float[] charges;
    [SerializeField] private float castingCharge;
    [SerializeField] private bool casting;
    [SerializeField] private bool castingEnd;
    [SerializeField] private bool isPlaying = false;

    // Interface Bits
    public override float[] ChargeTimes
    {
        get => chargeTimes;
        set => ChargeTimesSet(value);
    }
    public override int Level
    {
        get => level + levelOffset;
        set => LevelSet(value);
    }
    public override void ChargeTimesSet(float[] times)
    {
        chargeTimes = times;
    }
    public override void LevelSet(int level)
    {
        if (this.level > 0 && level == 0)
        {
            castingEnd = true;
        }
        this.level = level;
    }
    public override void SetTriggers(Triggers triggers)
    {
        if (HasTrigger(triggers, Triggers.StartCast))
            casting = true;
        if (HasTrigger(triggers, Triggers.EndCast))
            castingEnd = true;
    }

    // Actually Doing Stuff
    private void Awake()
    {
        chargeTimes ??= new float[] { 0, 1, 2 };
        if (charges.Length != chargeTimes.Length)
        {
            charges = new float[chargeTimes.Length];
        }
    }

    private void Update()
    {
        if (castingEnd)
        {
            StartCoroutine(CastingEnd());
        }
        else if (casting)
        {
            Casting();
        }
        else
        {
            switch (Level)
            {
                case 2: Level2(); break;
                case 3: Level3(); break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isPlaying)
        {
            if (Owner != null)
            {
                Transform target = Level >= 3 ? GetLocationTransform(finalLocation, Owner) : GetLocationTransform(defaultLocation, Owner);
                transform.position = target.position;
            }
            LookAtCamera.LookAt(transform, Camera.main.transform, Vector3.up);
        }
    }

    // Casting

    IEnumerator CastingEnd()
    {
        SetBool("CastingEnd", true);
        yield return new WaitForSeconds(2);
        level = 0;
        casting = false;
        castingEnd = false;
        for (int i = 0; i < charges.Length; i++)
        {
            charges[i] = 0f;
        }
        castingCharge = 0f;
        level2StartTime = -1f;
        level3StartTime = -1f;
        SetFloat("Level 2 Charge", 0f);
        SetFloat("Level 3 Charge", 0f);
        SetFloat("Casting", 0f);
        SetBool("CastingEnd", false);
        visualEffect.Stop();
        isPlaying = false;
    }

    void Casting()
    {
        castingCharge += Time.deltaTime / castingChargeTime;
        SetFloat("Casting", castingCharge);
        Level3();
        Level2();

    }

    [SerializeField][ReadOnly] private float level3StartTime = -1;
    void Level3()
    {
        if (level3StartTime < 0)
        {
            level3StartTime = Time.time;
        }
        charges[2] = Mathf.Lerp(0, 1, (Time.time - level3StartTime) / chargeTimes[2]);
        SetFloat("Level 3 Charge", charges[2]);
    }

    [SerializeField][ReadOnly] private float level2StartTime = -1;
    void Level2()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            visualEffect.Play();
            level2StartTime = Time.time;
        }
        charges[1] = Mathf.Lerp(0, 2, (Time.time - level2StartTime) / chargeTimes[1]);
        SetFloat("Level 2 Charge", charges[1]);
    }

    // Property Setters
    private void SetBool(string property, bool value)
    {
        if (visualEffect.HasBool(property))
            visualEffect.SetBool(property, value);
        else
            Debug.LogWarning("Cannot find Bool property on Visual Effect");
    }

    private void SetFloat(string property, float value)
    {
        if (visualEffect.HasFloat(property))
            visualEffect.SetFloat(property, value);
        else
            Debug.LogWarning("Cannot find Float property on Visual Effect");
    }
}
