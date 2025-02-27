using HotD.Body;
using MyBox;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor.Events;
using UnityEngine;
using static HotD.Castables.Coordination;
using UnityEngine.Events;

namespace HotD.Castables
{
    public class CastableStateMachineTester : MonoBehaviour
    {
        public StateCastable target;

        // Tests
        [Header("Tests")]
        public StateAction testValues;
        public TestCastCompatible testCastCompatible;
        public Character testCharacter;
        public CastableItem testItem;
        public float testDelay = 2f;

        [ButtonMethod]
        public void TestQueueAction()
        {
            target.QueueAction(testValues.action);
        }
        [ButtonMethod]
        public void TestSetState()
        {
            target.TransitionTo(testValues.state);
        }
        [ButtonMethod]
        public void TestInitialize()
        {
            target.Initialize(testCastCompatible, testItem);
        }
        [ButtonMethod]
        public void TestCharacterInitialize()
        {
            target.Initialize(testCharacter, testItem);
        }
        [ButtonMethod]
        public void TestCharacterInitializeAndEquip()
        {
            target.Initialize(testCharacter, testItem);
            target.QueueAction(CastAction.Equip);
        }

        [ButtonMethod]
        public void TestTriggerThenReleaseThenEnd()
        {
            target.QueueAction(CastAction.PrimaryTrigger);
            if (Application.isPlaying)
            {
                StartCoroutine(DelayedReleaseAndEnd(testDelay));
            }
            else
            {
                EditorCoroutineUtility.StartCoroutine(DelayedReleaseAndEnd(testDelay), this);
            }
        }
        private IEnumerator DelayedReleaseAndEnd(float delay)
        {
            yield return new WaitForSeconds(delay);
            target.QueueAction(CastAction.PrimaryRelease);
            yield return new WaitForSeconds(delay);
            target.QueueAction(CastAction.End);
        }

        [ButtonMethod]
        public void CreateChargeThenCastExecutors()
        {
            GameObject parent = new("Charge");
            parent.transform.SetParent(target.transform);
            var executor = parent.AddComponent<DelegatedExecutor>();

            executor.State = CastState.Activating;

            executor.supportedTransitions.Add(new(
                "Charge on Start", CastAction.Start,
                Triggers.StartAction, Triggers.None
            ));
            executor.supportedTransitions.Add(new(
                "Cast on Release", CastAction.PrimaryRelease,
                Triggers.None, Triggers.None, CastAction.End
            ));

            var charger = parent.AddComponent<Charger>();

            charger.resetOnBegin = true;

            charger.onCharged = new();
            UnityEvent onCharged = charger.onCharged;
            UnityEventTools.AddPersistentListener(onCharged, executor.End);

            UnityEvent startAction = executor.supportedTransitions[0].startAction;
            UnityEventTools.AddPersistentListener(startAction, charger.Begin);

            target.CreateCastExecutor();
        }
    }
}