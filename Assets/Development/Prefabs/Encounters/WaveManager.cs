using Body;
using HotD.Body;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HotD
{
    [Serializable]
    public struct Wave
    {
        public Wave(string name = "[New Wave]")
        {
            this.name = name;
            party = null;
        }
        public string name;
        public Party party;
    }

    public class WaveManager : BaseMonoBehaviour, IPartySpawner
    {
        [SerializeField] private List<Wave> waves;
        [SerializeField] private List<Transform> spawnpoints;

        [SerializeField] private float minutesBetweenWaves = 0.5f;
        [ReadOnly][SerializeField] private int wave = -1;
        [ReadOnly][SerializeField] private int spawnpoint = -1;

        [SerializeField] private bool randomizeWavePoints = false;
        [SerializeField] private bool patientWaves = false;

        [ReadOnly][SerializeField] private List<Party> parties = new();
        public UnityEvent<ASelectable> onMemberSpawned;
        public UnityEvent<Party> onPartyDied;
        public UnityEvent<ASelectable> deregisterMember;

        [SerializeField] private bool debug;

        public Coroutine coroutine;

        public void Start()
        {
            coroutine ??= StartCoroutine(CountdownToWave());
        }

        // IPartySpawner
        public void RegisterPartyAdder(UnityAction<ASelectable> action)
        {
            Print("Registering party member addition receiver...", debug);
            onMemberSpawned.AddListener(action);
        }

        public void RegisterPartyRemover(UnityAction<ASelectable> action)
        {
            Print("Registering party member removal receiver...");
            deregisterMember.AddListener(action);
        }

        public void OnPartySpawned(Party party)
        {
            foreach (Character member in party.members)
            {
                onMemberSpawned.Invoke(member.body.GetComponent<Selectable>());
            }
        }

        public void OnPartyDied(Party party)
        {
            parties.Remove(party);
            foreach (Character member in party.members)
            {
                deregisterMember.Invoke(member.body.GetComponent<ASelectable>());
            }
            Destroy(party);

            if (parties.Count == 0)
                coroutine ??= StartCoroutine(CountdownToWave());
        }

        // Actual Spawning
        public void SpawnNewWave()
        {
            wave += 1;
            for (int i = 0; i <= Mathf.FloorToInt(wave / waves.Count); i++)
            {
                spawnpoint = randomizeWavePoints ? UnityEngine.Random.Range(0, spawnpoints.Count) : spawnpoint += 1;
                Party party = Instantiate(waves[wave % waves.Count].party, spawnpoints[spawnpoint]);
                parties.Add(party);
                party.onAllDead.AddListener(() => { OnPartyDied(party); });
                OnPartySpawned(party);
            }

            if (!patientWaves)
                coroutine ??= StartCoroutine(CountdownToWave());
        }

        public IEnumerator CountdownToWave()
        {
            yield return new WaitForSeconds(minutesBetweenWaves * 60);
            SpawnNewWave();
        }
    }
}