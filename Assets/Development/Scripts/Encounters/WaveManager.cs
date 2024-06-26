using Body;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [Header("Stats")]
        [ReadOnly][SerializeField] public int kills = 0;
        [ReadOnly][SerializeField] public int partyKills = 0;

        [Header("Settings")]
        [SerializeField] private float minutesBetweenWaves = 0.5f;
        [SerializeField] private float deleteDeadPartyDelay = 2f;

        [SerializeField] private bool randomizeWavePoints = false;
        [SerializeField] private bool patientWaves = false;

        [SerializeField] private List<Wave> waves;
        [SerializeField] private List<Transform> spawnpoints;

        [SerializeField] private Transform[] wayPoints;

        [SerializeField] private bool debug;

        [Header("Player Stats")]
        [SerializeReference] private int startingSkillPoints;

        [Header("Status")]
        [ReadOnly][SerializeField] private int wave = -1;
        [ReadOnly][SerializeField] private int spawnpoint = -1;

        [ReadOnly][SerializeField] private List<Party> parties = new();

        [Foldout("Events", true)]
        public UnityEvent<ASelectable> onMemberSpawned;
        public UnityEvent<Party> onPartyDied;
        [Foldout("Events")] public UnityEvent<ASelectable> deregisterMember;


        private bool initialized = false;
        public Coroutine coroutine;

        public void Start()
        {
            coroutine ??= StartCoroutine(CountdownToWave());
        }

        public void Update()
        {
            if (!initialized)
            {
                initialized = true;
                Party.mainParty.ResetAttributes(startingSkillPoints);
            }
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
            if (party.isMainParty)
            {
                wave = -1;
                kills = 0;
                partyKills = 0;
                party.ResetAttributes(startingSkillPoints);
            }
            else
            {
                parties.Remove(party);

                StartCoroutine(DeletePartyAfterTime(deleteDeadPartyDelay, party));

                partyKills += 1;

                if (parties.Count == 0)
                {
                    if (Game.main.ActiveMenu != GameModes.Menu.Death)
                    {
                        Game.main.SetMode(GameModes.Menu.CharacterSheet);
                        Party.mainParty.LevelUp();
                        Print("Counting down till next wave arrives.", debug);
                        coroutine ??= StartCoroutine(CountdownToWave());
                    }
                }
            }
        }

        public IEnumerator DeletePartyAfterTime(float time, Party party)
        {
            yield return new WaitForSeconds(time);

            foreach (Character member in party.members)
            {
                deregisterMember.Invoke(member.body.GetComponent<ASelectable>());
            }

            Destroy(party.gameObject);
        }

        public void OnMemberDied(Character member)
        {
            kills += 1;
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
                party.onMemberDeath += OnMemberDied;
                party.onAllDead.AddListener(() => { OnPartyDied(party); });
                party.TargetParty = Party.mainParty;
                OnPartySpawned(party);

                AddWayPoints(party.transform.GetChild(0).GetComponent<EnemyAI>());
            }

            if (!patientWaves)
                coroutine ??= StartCoroutine(CountdownToWave());
        }

        private void AddWayPoints(EnemyAI ai)
        {
            ai.SetWayPoints(wayPoints[UnityEngine.Random.Range(0, wayPoints.Length)], wayPoints[UnityEngine.Random.Range(0, wayPoints.Length)]);
        }

        public IEnumerator CountdownToWave()
        {
            yield return new WaitForSeconds(minutesBetweenWaves * 60);
            coroutine = null;
            SpawnNewWave();
        }

        // Testing

        [ButtonMethod]
        public void TestStartWave()
        {
            SpawnNewWave();
        }

        [ButtonMethod]
        public void TestStartWaveDelay()
        {
            coroutine ??= StartCoroutine(CountdownToWave());
        }

        [ButtonMethod]
        public void TestKillWave()
        {
            Party[] parties = this.parties.ToArray();
            foreach (Party party in parties)
            {
                OnPartyDied(party);
            }
        }
    }
}