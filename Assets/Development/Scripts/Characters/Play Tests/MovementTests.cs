using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using HotD.Body;

public class MovementTests
{
    private GameObject parent;
    private Movement movement;

    [OneTimeSetUp]
    public void InitialSetUp()
    {
        parent = new GameObject("Movement Tests Parent");
        movement = parent.AddComponent<Movement>();
        movement.settings = ScriptableObject.CreateInstance<MovementSettings>();
    }

    [OneTimeTearDown]
    public void FinalTearDown()
    {
        Object.Destroy(parent);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MovementTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
