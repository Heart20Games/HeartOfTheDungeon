using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using HotD.Castables;
using System.Collections;
using HotD.Body;
using static HotD.Castables.Coordination;
using UnityEngine.Events;
using UnityEditor.Events;

public class StateCastableTests
{
    private CastableItem testItem;

    private GameObject parent;
    private StateCastable state;
    private Damager damager;

    private GameObject parent2;
    private ICharacter character;

    [OneTimeSetUp]
    public void InitialSetUp()
    {
        parent = new GameObject("State Castable Tests Parent");
        state = parent.AddComponent<StateCastable>();
        state.fields = new();

        // Damager
        damager = parent.AddComponent<Damager>();

        // Charge Set Up
        state.AddChargeTransitions();
        CreateChargeThenCastExecutors(state);

        // Character
        parent2 = new GameObject("Character Parent");
        character = parent2.AddComponent<CharacterStub>();

        if (testItem == null)
        {
            testItem = new();
        }
        state.Initialize(character, testItem);
    }

    [OneTimeTearDown]
    public void FinalTearDown()
    {
        Object.Destroy(parent);
        Object.Destroy(parent2);
    }

    [Test]
    public void ExamplePlayerTestScriptSimplePasses()
    {
        
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ExamplePlayerTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    public void CreateChargeThenCastExecutors(StateCastable target)
    {
        Assert.IsNotNull(target);

        GameObject parent = new("Charge");
        parent.transform.SetParent(target.transform);
        var executor = parent.AddComponent<DelegatedExecutor>();

        executor.State = CastState.Activating;

        executor.supportedTransitions.Add(new(
            "Charge on Start", CastAction.Start,
            Triggers.StartAction, Triggers.None
        ));
        executor.supportedTransitions.Add(new(
            "Cast on Release", CastAction.Release,
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
