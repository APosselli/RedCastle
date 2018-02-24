using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//cheat to test switch;
	public InteractiveObjectBase CheatToTestSwitch; 

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

	public bool StopAllMovement;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (StopAllMovement) {
			rb.velocity = Vector3.zero;
		} else {
			Movement ();
		}

		Interaction ();

    }

	// Contain the movement logic here
	void Movement(){
		Vector3 force = new Vector3();

		// Get input for movement on the x, y plane. Apply it to the force.
		if (Input.GetKey(KeyCode.W))
		{
			force += rb.transform.forward;
		}
		if(Input.GetKey(KeyCode.S))
		{
			force += -rb.transform.forward;
		}
		if(Input.GetKey(KeyCode.A))
		{
			force += -rb.transform.right;
		}
		if(Input.GetKey(KeyCode.D))
		{
			force += rb.transform.right;
		}

		// Make the player come to a stop more quickly.
		Vector3 playerVelocity = rb.velocity;
		playerVelocity.y = 0;

		if(force == Vector3.zero && playerVelocity.magnitude > brakingVelocity)
		{
			force = -playerVelocity * brakeForce;
		}

		// Normalize the force for movement on the x, y plane, so moving diagonally doesn't make the player faster.
		// Multiply by the movement force we can change how fast the player speeds up.
		force = force.normalized * movementForce;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			force += Vector3.up * jumpForce;
		}

		rb.AddForce(force);

		// Limit the player speed.
		if(rb.velocity.magnitude > maxVelocity)
		{
			rb.velocity = rb.velocity.normalized * maxVelocity;
		}
	}

	void Interaction(){
		StopAllMovement = false;

		//cheat to test switch
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			CheatToTestSwitch.StartTrigger ();
			if (CheatToTestSwitch.StopPlayerMovement)
				StopAllMovement = true;//check object for "StopPlayerMovement"
		}

		if (Input.GetKeyUp (KeyCode.UpArrow)){
			CheatToTestSwitch.StopTrigger ();
		}

	}
}
