using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public class WaveManager : BaseMonoBehaviour
    {
        [SerializeField] private List<Wave> waves;
        [SerializeField] private List<Transform> spawnpoints;

        [SerializeField] private float minutesBetweenWaves = 0.5f;
        [ReadOnly][SerializeField] private int wave = -1;
        [ReadOnly][SerializeField] private int spawnpoint = -1;

        [SerializeField] private bool randomizeWavePoints = false;

        [ReadOnly][SerializeField] private List<Party> parties = new();

        public Coroutine coroutine;

        public void Start()
        {
            coroutine = StartCoroutine(CountdownToWave());
        }

        public void SpawnNewWave()
        {
            wave += 1;
            for (int i = 0; i <= Mathf.FloorToInt(wave / waves.Count); i++)
            {
                spawnpoint = randomizeWavePoints ? UnityEngine.Random.Range(0, spawnpoints.Count) : spawnpoint += 1;
                Party party = Instantiate(waves[wave % waves.Count].party, spawnpoints[spawnpoint]);
                parties.Add(party);
            }
            coroutine = StartCoroutine(CountdownToWave());
        }

        public IEnumerator CountdownToWave()
        {
            yield return new WaitForSeconds(minutesBetweenWaves * 60);
            SpawnNewWave();
        }
    }
}