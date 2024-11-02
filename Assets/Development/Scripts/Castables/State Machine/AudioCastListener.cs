using FMODUnity;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    using static Coordination;

    /*
     * The Audio Cast Listener is an inspector-friendly way to control FMOD Studio Event Emitter transitions in the form of a Cast Listener.
     */

    public class AudioCastListener : CastListener
    {
        [SerializeField] private List<LevelSetParam> levelSetParams = new();
        [SerializeField] private Triggers playOn;
        [SerializeField] private Triggers endOn;
        [SerializeField] private StudioEventEmitter eventEmitter;

        [SerializeField] private bool debug;

        private void Awake()
        {
            eventEmitter = eventEmitter != null ? eventEmitter : GetComponent<StudioEventEmitter>();
        }

        public override void LevelSet(int level)
        {
            if (this.level == level) return;

            Print($"Setting Level {level}", debug, this);
            base.LevelSet(level);
            foreach (var param in levelSetParams)
            {
                if (param.InRange(level) && param.waitForTrigger == Triggers.None)
                {
                    Print($"Setting Parameter {param.name} to {param.LevelOverride(level)}", debug, this);
                    eventEmitter.SetParameter(param.name, param.LevelOverride(level));
                }
            }
        }

        public override void SetTriggers(Triggers triggers)
        {
            Print($"Triggering {triggers}", debug, this);

            base.SetTriggers(triggers);
            foreach (var param in levelSetParams)
            {
                if (param.SatisfiesTrigger(triggers))
                {
                    Print($"Setting Parameter {param.name} to {param.LevelOverride(level)}", debug, this);
                    eventEmitter.SetParameter(param.name, param.LevelOverride(level));
                }
            }

            if ((triggers & playOn) != 0)
            {
                Print("Play", debug, this);
                eventEmitter.Play();
            }

            if ((triggers & endOn) != 0)
            {
                Print("Stop", debug, this);
                eventEmitter.Stop();
            }
        }

        [Serializable]
        struct LevelSetParam
        {
            public string name;
            public int minLevel;
            public int maxLevel;
            public bool overrideLevel;
            [ConditionalField("overrideLevel")]
            public int levelOverride;
            public Triggers waitForTrigger;

            public readonly int LevelOverride(int level)
            {
                return overrideLevel ? levelOverride : level;
            }

            public readonly bool InRange(int level)
            {
                return level == Mathf.Clamp(level, minLevel, maxLevel);
            }

            public readonly bool SatisfiesTrigger(Triggers triggers)
            {
                return (waitForTrigger & triggers) != 0;
            }
        }
    }
}