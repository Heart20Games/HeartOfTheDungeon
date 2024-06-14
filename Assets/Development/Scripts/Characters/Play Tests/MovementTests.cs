using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using HotD.Body;

public class MovementTests
{
    private GameObject parent;
    private Rigidbody rigidbody;
    private TestMovement movement;

    [OneTimeSetUp]
    public void InitialSetUp()
    {
        parent = new GameObject("Movement Tests Parent");
        rigidbody = parent.AddComponent<Rigidbody>();
        movement = parent.AddComponent<TestMovement>();
        movement.Settings = ScriptableObject.CreateInstance<MovementSettings>();
        movement.Settings.speed = 1;
        movement.npcModifier = 1;
        movement.Settings.normalForce = 1;
        movement.Settings.gravityForce = 1;
    }

    [OneTimeTearDown]
    public void FinalTearDown()
    {
        Object.Destroy(parent);
    }

    // Tests

    // A Test behaves as an ordinary method
    [Test]
    public void StopDragTest()
    {
        // Use the Assert class to test conditions
        movement.Settings.stopDrag = 200;
        movement.MoveVector = Vector2.zero;
        Assert.AreEqual(200, rigidbody.drag);
    }

    [Test]
    public void MoveDragTest()
    {
        movement.Settings.moveDrag = 100;
        movement.MoveVector = Vector2.right;
        Assert.AreEqual(100, rigidbody.drag);
    }

    [UnityTest]
    public IEnumerator ZeroTimeScaleTest()
    {
        movement.StartZeroTimeScaleTest();
        yield return movement;
    }
}

public class TestMovement : Movement, IMonoBehaviourTest
{
    private bool finished = false;
    public bool IsTestFinished => finished;

    public void StartZeroTimeScaleTest()
    {
        StartCoroutine(ZeroTimeScaleTest());
    }
    private IEnumerator ZeroTimeScaleTest()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        MoveVector = Vector2.zero;
        canMove = false;
        TimeScale = 1;
        myRigidbody.velocity = Vector3.right;

        // Becomes Zero
        TimeScale = 0;
        Assert.AreEqual(Vector3.zero, myRigidbody.velocity, "Setting TimeScale to zero should set rigidbody velocity to zero.");

        // Sustains Zero
        ApplyGravityForce(100000, false);
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(Vector3.zero, myRigidbody.velocity, "ApplyGravity failed to conform to TimeScale.");

        float modifier = 1;
        ApplyNPCMovement(ref modifier, Vector2.left, 100000);
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(Vector3.zero, myRigidbody.velocity, "ApplyNPCMovement failed to conform to TimeScale.");

        ApplyPlayerMovement(Vector3.up + Vector3.left, Vector2.left, 100000);
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(Vector3.zero, myRigidbody.velocity, "ApplyPlayerMovement failed to conform to TimeScale.");

        // Resets to Original
        TimeScale = 1;
        Assert.AreEqual(Vector3.right, myRigidbody.velocity, "Setting TimeScale back to full should set rigidbody velocity to it's original value.");

        finished = false;
        yield return null;
    }
}
