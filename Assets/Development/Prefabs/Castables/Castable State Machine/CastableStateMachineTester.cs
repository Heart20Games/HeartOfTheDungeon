using HotD.Body;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotD.Castables
{
    public class CastableStateMachineTester : MonoBehaviour
    {
        public CastableStateMachine target;

        // Tests
        [Header("Tests")]
        public StateAction testValues;
        public TestCastCompatible testCastCompatible;
        public Character testCharacter;
        public CastableItem testItem;

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
            target.Initialize(testCharacter, testItem, 1);
        }
        [ButtonMethod]
        public void TestCharacterInitializeThenEquipAndTrigger()
        {
            target.Initialize(testCharacter, testItem, 1);
            target.QueueAction(CastAction.Equip);
            target.QueueAction(CastAction.Trigger);
        }
    }
}