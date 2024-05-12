using Body.Behavior;
using HotD;
using HotD.Body;
using HotD.Castables;
using NUnit.Framework;
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
        parent = new GameObject("Movement Tests Parent");
        
        // Movement
        movement = parent.AddComponent<MovementStub>();
        movement.Settings = ScriptableObject.CreateInstance<MovementSettings>();

        // Stubs
        brain = parent.AddComponent<BrainStub>();
        talker = parent.AddComponent<TalkerStub>();
        caster = parent.AddComponent<CasterStub>();

        // Character
        character = parent.AddComponent<Character>();
    }

    [OneTimeTearDown]
    public void FinalTearDown()
    {
        Object.Destroy(parent);
    }


    // Tests

    [Test]
    public void ExampleTest()
    {
        Assert.True(character != null);
    }
}


