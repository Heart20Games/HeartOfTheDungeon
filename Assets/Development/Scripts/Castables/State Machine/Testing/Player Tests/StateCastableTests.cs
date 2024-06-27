using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using HotD.Castables;
using System.Collections;
using HotD.Body;

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
        state.CreateChargeThenCastExecutors();

        // Character
        parent2 = new GameObject("Character Parent");
        character = parent2.AddComponent<CharacterStub>();

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
}
