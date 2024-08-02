using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public interface IVFXCoordinator
{
    public int Level { get; set; }
    public void SetLevel(int level);
    public void StartCast();
    public void EndCast();
    public float[] ChargeTimes { get; set; }
}

public class MagicBolt_ChargingVFXScript : MonoBehaviour, IVFXCoordinator
{

    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private int level;
    [SerializeField] private float[] chargeTimes;
    [SerializeField] private float[] charges;
    [SerializeField] private float castingChargeTime;
    [SerializeField] private float castingCharge;
    [SerializeField] private bool casting;
    [SerializeField] private bool castingEnd;
    [SerializeField] private bool isPlaying = false;

    // Interface Bits
    public int Level
    {
        get => level;
        set => SetLevel(value);
    }
    public void SetLevel(int level)
    {
        this.level = level;
    }
    public void StartCast()
    {
        casting = true;
    }
    public void EndCast()
    {
        castingEnd = true;
    }
    public float[] ChargeTimes
    {
        get => chargeTimes;
        set => chargeTimes = value;
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
            switch (level)
            {
                case 2: Level2(); break;
                case 3: Level3(); break;
            }
        }
    }

    IEnumerator CastingEnd()
    {
        visualEffect.SetBool("CastingEnd", true);
        yield return new WaitForSeconds(2);
        level = 1;
        casting = false;
        castingEnd = false;
        for (int i = 0; i < charges.Length; i++)
        {
            charges[i] = 0f;
        }
        castingCharge = 0f;
        visualEffect.SetFloat("Level 2 Charge", 0f);
        visualEffect.SetFloat("Level 3 Charge", 0f);
        visualEffect.SetFloat("Casting", 0f);
        visualEffect.SetBool("CastingEnd", false);
        visualEffect.Stop();
        isPlaying = false;
    }

    void Casting()
    {
        castingCharge += Time.deltaTime / castingChargeTime;
        visualEffect.SetFloat("Casting", castingCharge);
        Level3();
        Level2();

    }

    void Level3()
    {
        charges[2] += Time.deltaTime / chargeTimes[2];
        visualEffect.SetFloat("Level 3 Charge", charges[2]);
        Level2();
    }

    void Level2()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            visualEffect.Play();
        }
        charges[1] += Time.deltaTime / chargeTimes[1];
        visualEffect.SetFloat("Level 2 Charge", charges[1]);
    }
}
