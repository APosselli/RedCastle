using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScript : MonoBehaviour {

    public Waypoint target;
    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (target != null)
        {
            transform.LookAt(target.position);
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.x = 0;
            rotation.z = 0;
            Quaternion rot = new Quaternion();
            rot.eulerAngles = rotation;
            transform.rotation = rot;
            rb.velocity = (transform.forward).normalized;
        }
        else
            rb.velocity = Vector3.zero;
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Waypoint"))
        {
            target = target.target;
        }
    }
}
