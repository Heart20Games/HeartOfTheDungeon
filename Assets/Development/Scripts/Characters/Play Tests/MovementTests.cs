using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using HotD.Body;

public class MovementTests
{
    private GameObject parent;
    private Rigidbody rigidbody;
    private Movement movement;

    [OneTimeSetUp]
    public void InitialSetUp()
    {
        parent = new GameObject("Movement Tests Parent");
        rigidbody = parent.AddComponent<Rigidbody>();
        movement = parent.AddComponent<Movement>();
        movement.settings = ScriptableObject.CreateInstance<MovementSettings>();
        movement.settings.speed = 1;
        movement.npcModifier = 1;
        movement.settings.normalForce = 1;
        movement.settings.gravityForce = 1;
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
        movement.settings.stopDrag = 200;
        movement.SetMoveVector(Vector2.zero);
        Assert.AreEqual(200, rigidbody.drag);
    }

    [Test]
    public void MoveDragTest()
    {
        movement.settings.moveDrag = 100;
        movement.SetMoveVector(Vector2.right);
        Assert.AreEqual(100, rigidbody.drag);
    }

    [UnityTest]
    public IEnumerator ZeroTimeScaleTest()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        movement.SetMoveVector(Vector2.zero);
        movement.canMove = false;
        movement.TimeScale = 1;
        rigidbody.velocity = Vector3.right;

        // Becomes Zero
        movement.TimeScale = 0;
        Assert.AreEqual(Vector3.zero, rigidbody.velocity, "Setting TimeScale to zero should set rigidbody velocity to zero.");

        // Sustains Zero
        movement.ApplyGravity(100000, false);
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(Vector3.zero, rigidbody.velocity, "ApplyGravity failed to conform to TimeScale.");
        
        float modifier = 1;
        movement.ApplyNPCMovement(ref modifier, Vector2.left, 100000);
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(Vector3.zero, rigidbody.velocity, "ApplyNPCMovement failed to conform to TimeScale.");
        
        movement.ApplyPlayerMovement(Vector3.up + Vector3.left, Vector2.left, 100000);
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(Vector3.zero, rigidbody.velocity, "ApplyPlayerMovement failed to conform to TimeScale.");

        // Resets to Original
        movement.TimeScale = 1;
        Assert.AreEqual(Vector3.right, rigidbody.velocity, "Setting TimeScale back to full should set rigidbody velocity to it's original value.");

        yield return null;
    }
}
