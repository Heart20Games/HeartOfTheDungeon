using HotD.Body;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

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
            target.SetState(testValues.state);
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
            target.QueueAction(CastAction.Trigger);
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
            target.QueueAction(CastAction.Release);
            yield return new WaitForSeconds(delay);
            target.QueueAction(CastAction.End);
        }
    }
}