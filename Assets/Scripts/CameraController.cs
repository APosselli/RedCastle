using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;
    public float xAxisSens = 1f;
    public float yAxisSens = 1f;
    public float defaultCamDist = 6f;

    private Vector3 relativePosition;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        relativePosition = new Vector3();
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = player.transform.position + relativePosition;
        Vector3 movement = new Vector3(-(Input.GetAxis("Horizontal") * xAxisSens), -(Input.GetAxis("Vertical") * yAxisSens), 0f);
        transform.Translate(movement * Time.deltaTime);

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

        transform.position = (player.transform.position + camDistance * (transform.position - player.transform.position).normalized);
        transform.LookAt(player.transform);
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        Quaternion playerRotation = new Quaternion();
        playerRotation.eulerAngles = rotation;
        player.GetComponent<Rigidbody>().rotation = playerRotation;
        relativePosition = transform.position - player.transform.position;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.GetComponent<Rigidbody>().AddForce(Vector3.up * 250f);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            player.GetComponent<Rigidbody>().AddForce(player.GetComponent<Rigidbody>().transform.forward * 20f);
        }
    }
}
