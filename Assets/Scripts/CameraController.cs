using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [Header("General Camera Settings")]
    [Tooltip("The player that the camera follows.")]
    public GameObject player;
    [Tooltip("Sensitivity on the x-axis.")]
    public float xAxisSens = 1f;
    [Tooltip("Sensitivity on the y-axis.")]
    public float yAxisSens = 1f;
    [Tooltip("The default distance the camera is from the player.")]
    public float defaultCamDist = 6f;

    // Vector between the player and the camera.
    private Vector3 relativePosition;

    private Rigidbody playerRb;

	// Use this for initialization
	void Start () {
        // Hide the mouse and keep it centered.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerRb = player.GetComponent<Rigidbody>();
        relativePosition = (transform.position - playerRb.position).normalized * defaultCamDist;
    }
	
	// Update is called once per frame
	void Update () {

        // Make the camera position relative to the player the same as it was the last frame.
        transform.position = player.transform.position + relativePosition;
        // Create a vector for moving the camera based on mouse input and sensitivity.
        Vector3 movement = Vector3.zero;
        if (!(transform.forward == Vector3.down))
            movement = new Vector3(-(Input.GetAxis("Horizontal") * xAxisSens), -(Input.GetAxis("Vertical") * yAxisSens), 0f);
        else
        {
            if(-Input.GetAxis("Vertical") < 0)
            {
                movement = new Vector3(0f, -(Input.GetAxis("Vertical") * yAxisSens), 0f);
            }
        }
        // Translate the camera using the movement vector.
        transform.Translate(movement * Time.deltaTime);

        if(transform.position.y > playerRb.position.y + defaultCamDist)
        {
            Quaternion camRotation = transform.rotation;
            transform.position = Vector3.up * defaultCamDist + playerRb.position;
            transform.rotation = camRotation;
        }

        if(transform.position.y < (playerRb.position.y - 0.25f))
        {
            Vector3 position = transform.position;
            position.y = playerRb.position.y - 0.25f;
            transform.position = position;
        }

        // Use a raycast to determine if anything is between the player and the camera.
        // Sets the camera distance to either the default distance or the distance to the closest object between the
        // player and the camera.
        RaycastHit hit;
        float camDistance;
        if (Physics.Raycast(player.transform.position, transform.position - player.transform.position, out hit, defaultCamDist, ~(1 << 7)))
        {
            camDistance = hit.distance;
        }
        else
        {
            camDistance = defaultCamDist;
        }

        // Actual camera distance is changed here.
        transform.position = (player.transform.position + camDistance * (transform.position - player.transform.position).normalized);
        // Make the camera look at the player.
        if(transform.position != Vector3.up * defaultCamDist + playerRb.position)
            transform.LookAt(player.transform);

        // Rotate the player to face the same direction as the camera on the y-axis.
        Vector3 rotationVector = transform.rotation.eulerAngles;
        rotationVector.x = 0;
        rotationVector.z = 0;
        Quaternion rotationQuaternion = new Quaternion();
        rotationQuaternion.eulerAngles = rotationVector;
        playerRb.rotation = rotationQuaternion;
        relativePosition = transform.position - player.transform.position;

        // Gives the player control of the mouse again.
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
