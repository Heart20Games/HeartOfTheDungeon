using Body.Behavior;
using HotD;
using HotD.Body;
using HotD.Castables;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class CharacterModeTests
{
    private GameObject parent;
    private IMovement movement;
    private IBrain brain;
    private ITalker talker;
    private ICaster caster;
    private Character character;

    [OneTimeSetUp]
    public void InitialSetUp()
    {
        parent = new GameObject("Character Mode Tests Parent");
        
        // Movement
        movement = parent.AddComponent<MovementStub>();
        movement.Settings = ScriptableObject.CreateInstance<MovementSettings>();

        // Stubs
        brain = parent.AddComponent<BrainStub>();
        talker = parent.AddComponent<TalkerStub>();
        caster = parent.AddComponent<CasterStub>();

        // Character
        character = parent.AddComponent<Character>();

        // Equipment
        character.StatBlock = ScriptableObject.CreateInstance<CharacterBlock>();
        character.StatBlock.loadout = ScriptableObject.CreateInstance<Loadout>();
        for (int i = 0; i < 5; i++)
        {
            var castItem = ScriptableObject.CreateInstance<CastableItem>();
            castItem.prefab = new GameObject($"Castable {i}");
            var castable = castItem.prefab.AddComponent<CastableStub>();
            
            character.StatBlock.loadout[i] = castItem;
        }
    }

    [OneTimeTearDown]
    public void FinalTearDown()
    {
        Object.Destroy(parent);
    }


    // Tests

    [Test]
    public void EquipTest()
    {
        var all = character.StatBlock.loadout.All();
        Assert.True(all != null);

        var castChildren = parent.GetComponentsInChildren<MonoBehaviour>().OfType<ICastable>();
        Assert.AreEqual(0, castChildren.Count(), "Expected uninitialized character to have no castables on it.");

        character.InitializeCastables();

        castChildren = parent.GetComponentsInChildren<MonoBehaviour>().OfType<ICastable>();
        Assert.AreEqual(all.Count, castChildren.Count(), "Expected initialized character to have all castables on loadout on it.");
    }

    [Test]
    public void ExampleTest()
    {
        Assert.True(character != null);
    }
}


