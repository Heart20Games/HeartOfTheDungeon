using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using static Body.Behavior.ContextSteering.CSIdentity;


/*
 * The Damager is responsible for deciding how to deal damage to a Damage Receiver.
 * 
 * It uses object Identity and Impact data to ignore duplicates and decide whether or not it should actually deal damage at all.
 * 
 * Intended for use on weapons, projectiles, traps, etc. Anything that needs to deal damage with some level of intelligence.
 */

public interface IDamager
{
    public void HitDamageable(Impactor impactor);
    public void LeftDamageable(Impactor impactor);
}

public class Damager : BaseMonoBehaviour, IDamager
{
    private struct ReceiverData
    {
        public float procTime;
        public float lastTick;
        public Vector3 location;

        public ReceiverData(float procTime, float lastTick, Vector3 location)
        {
            this.procTime = procTime;
            this.lastTick = lastTick;
            this.location = location;
        }

        public readonly ReceiverData UpdateLastTick()
        {
            return new(procTime, Time.time, location);
        }
    }

    private readonly Dictionary<IDamageReceiver, ReceiverData> receiverData = new();
    private readonly List<IDamageReceiver> ignored = new();
    public bool shouldTick = false;
    public bool shouldTickOnProc = true;
    [Range(0, 2f)] public float tickRate = 0.5f; // In seconds
    public int damage = 1;
    public Identity identity = Identity.Neutral;
    public Impactor _Impactor { get => _impactor; set => _impactor = value; }
    private Impactor _impactor;
    [ReadOnly][SerializeField] private int receiverCount = 0;
    [ReadOnly][SerializeField] private int ignoredCount = 0;

    public bool debug = false;


    public void SetIdentity(Identity identity)
    {
        this.identity = identity;
    }


    // Built-ins

    private void Update()
    {
        if (shouldTick)
        {
            DamageTick();
        }
    }


    // Damage Ticks

    

    /// <summary>
    /// A method that applies damage to a given damage reciever, or else applies damage to all receivers added to the damager.
    /// </summary>
    /// <param name="receiver"></param>
    public void DamageTick(IDamageReceiver receiver=null)
    {
        if (receiver == null)
        {
            UpdateDamageTicks();
        }
        else
        {
            DamageTickOn(receiver);
        }

        #region Local Functions
        void DamageTickOn(IDamageReceiver receiver)
        {
            Print($"Damage Tick on {Name}", debug, this);
            if (receiverData.TryGetValue(receiver, out var data))
            {
                receiver.SetDamagePosition(data.location);
                receiverData[receiver] = data.UpdateLastTick(); // Updates the Last Tick field in the data struct to be Time.time.
            }
            receiver.TakeDamage(damage, identity);
        }

        void UpdateDamageTicks()
        {
            var receivers = receiverData.Keys.ToArray();
            for (int i = receiverData.Keys.Count - 1; i >= 0; i--)
            {
                IDamageReceiver receiver = receivers[i];
                Assert.IsNotNull(receiver);
                ReceiverData data = receiverData[receiver];
                if (Time.time >= data.lastTick + tickRate)
                {
                    DamageTickOn(receiver);
                }
            }
        }
        #endregion Local Functions
    }


    // Receivers

    public void AddReceiver(IDamageReceiver receiver, Vector3 impactLocation)
    {
        Print($"Receiver Added on {Name}", debug, this);
        receiverData.Add(receiver, new(Time.time, Time.time, impactLocation));
        receiverCount = receiverData.Count;
        receiver.OnDestroyed += () => { RemoveReceiver(receiver); };
        if (shouldTickOnProc)
        {
            DamageTick(receiver);
        }
    }

    public void RemoveReceiver(IDamageReceiver receiver)
    {
        Print($"Receiver Removed on {(this == null ? name : Name)}", debug, this);
        receiverData.Remove(receiver);
        receiverCount = receiverData.Count;
    }

    public void Ignore(Transform toIgnore)
    {
        if (toIgnore.TryGetComponent<IDamageReceiver>(out var damageable))
        {
            ignored.Add(damageable);
            ignoredCount = ignored.Count;
        }
    }


    // Damageables

    public void HitDamageable(Impactor impactor)
    {
        if (impactor == null || impactor.other.gameObject == null)
        {
            Debug.LogWarning("Impactor is null.", this);
            return;
        }

        // Get the actual IDamageReceiver
        IDamageReceiver other = impactor.other.gameObject.GetComponent<IDamageReceiver>();
        other ??= impactor.gameObject.GetComponent<IDamageReceiver>();
        Print($"Hit Damageable: {other}", debug, this);

        // Cases where we shouldn't do anything.
        if (impactor._Character != null && !impactor._Character.Alive) return;
        if (other == null || ignored.Contains(other) || receiverData.ContainsKey(other))
        {
            Print($"{other} was {(other == null ? "null." : (ignored.Contains(other) ? "ignored." : (receiverData.ContainsKey(other) ? "already hit!" : "skipped?")))}", debug, this);
            return;
        }

        // We can actually add the receiver now.
        Print("Doing stuff with damageable.", debug, this);
        AddReceiver(other, impactor.other.ImpactLocation);
    }

    public void LeftDamageable(Impactor impactor)
    {
        if (impactor == null || impactor.other.gameObject == null)
        {
            Debug.LogWarning("Impactor is null.", this);
            return;
        }

        // Get the actual IDamageReceiver
        IDamageReceiver other = impactor.other.gameObject.GetComponent<IDamageReceiver>();
        Print($"Left Damageable: {other}", debug, this);

        // Cases where we shouldn't do anything.
        if (other == null || ignored.Contains(other) || !receiverData.ContainsKey(other))
        {
            Print($"{other} was {(other == null ? "null." : (ignored.Contains(other) ? "ignored." : (receiverData.ContainsKey(other) ? "not hit yet!" : "skipped?")))}", debug, this);
            return;
        }

        // We can actually remove the receiver now.
        Print("Removing Damageable.", debug, this);
        RemoveReceiver(other);
    }


    // VFX?

    public void SpawnDamagerEffect(GameObject effect)
    {
        Transform childTransform = transform.GetChild(0);

        GameObject go = Instantiate(effect, childTransform);

        go.GetComponent<VisualEffect>().Play();

        Destroy(go, 2f);
    }
}
