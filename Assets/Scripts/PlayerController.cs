using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;
    [Header("Movement Settings")]
    [Tooltip("The max speed the player can travel.")]
    public float maxVelocity = 10f;
    [Tooltip("The force applied to the player when a directional key is pressed. Affects acceleration.")]
    public float movementForce = 20f;
    [Tooltip("The upwards force applied to the player character when they jump.")]
    public float jumpForce = 250f;
    [Tooltip("The force applied to the player to make them come to a stop more quickly. Affects how quickly the player comes to a stop.")]
    public float brakeForce = 4f;
    [Tooltip("How slow the player must be moving before braking stops.")]
    public float brakingVelocity = 2f;

    private bool canJump = false;
    private bool wallJump = false;
    private Vector3 wallJumpVector;

    private float playerHeight;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerHeight = GetComponent<MeshFilter>().mesh.bounds.size.y;
        wallJumpVector = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 force = new Vector3();

        // Get input for movement on the x, y plane. Apply it to the force.
        if (Input.GetKey(KeyCode.W) && canJump)
        {
            force += rb.transform.forward;
        }
        if (Input.GetKey(KeyCode.S) && canJump)
        {
            force += -rb.transform.forward;
        }
        if (Input.GetKey(KeyCode.A) && canJump)
        {
            force += -rb.transform.right;
        }
        if (Input.GetKey(KeyCode.D) && canJump)
        {
            force += rb.transform.right;
        }

        // Make the player come to a stop more quickly.
        Vector3 playerVelocity = rb.velocity;
        playerVelocity.y = 0;

        if (force == Vector3.zero && playerVelocity.magnitude > brakingVelocity && canJump)
        {
            force = -playerVelocity * brakeForce;
        }

        // Normalize the force for movement on the x, y plane, so moving diagonally doesn't make the player faster.
        // Multiply by the movement force we can change how fast the player speeds up.
        force = force.normalized * movementForce;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                force += Vector3.up * jumpForce;
            }
            else if (wallJump)
            {
                force += wallJumpVector;
            }
        }

        rb.AddForce(force);

        // Limit the player speed.
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        SetJump(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        SetJump(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        canJump = false;
        wallJump = false;
    }

    private void SetJump(Collision collision)
    {
        ContactPoint highestContactPoint = collision.contacts[0];
        foreach (ContactPoint point in collision.contacts)
        {
            if (point.point.y > highestContactPoint.point.y)
            {
                highestContactPoint = point;
            }
        }

        if (highestContactPoint.point.y <= (rb.transform.position.y - ((playerHeight / 2) * 0.9f)))
        {
            wallJump = false;
            canJump = true;
        }
        else
        {
            wallJump = true;
            canJump = false;
            wallJumpVector = highestContactPoint.normal;
            wallJumpVector.y = 0;
            Vector3 playerPlanarVector = -transform.forward;
            playerPlanarVector.y = 0;
            wallJumpVector.Normalize();
            wallJumpVector = playerPlanarVector - 2 * (playerPlanarVector - (Vector3.Dot(playerPlanarVector, wallJumpVector) * wallJumpVector));
            wallJumpVector.Normalize();
            wallJumpVector = wallJumpVector * jumpForce;
            wallJumpVector.y = jumpForce;
        }
    }
}
