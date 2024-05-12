using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using HotD.Castables;
using System.Collections;
using HotD.Body;

public class StateCastableTests
{
    [SerializeField] private Character characterPrefab;
    [SerializeField] private CastableItem testItem;

    private GameObject parent;
    private StateCastable state;
    private Damager damager;

    private GameObject parent2;
    private Character character;

    [OneTimeSetUp]
    public void InitialSetUp()
    {
        if (characterPrefab != null && testItem != null)
        {
            parent = new GameObject("State Castable Tests Parent");
            state = parent.AddComponent<StateCastable>();
            
            // Damager
            damager = parent.AddComponent<Damager>();
            
            // Charge Set Up
            state.AddChargeTransitions();
            state.CreateChargeThenCastExecutors();

            // Character
            character = Object.Instantiate(characterPrefab);

            state.Initialize(character, testItem, 1);
        }
    }

    [OneTimeTearDown]
    public void FinalTearDown()
    {
        if (parent != null)
        {
            Object.Destroy(parent);
        }
        if (character != null)
        {
            Object.Destroy(character.gameObject);
        }
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
