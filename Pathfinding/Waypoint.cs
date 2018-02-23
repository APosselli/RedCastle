using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
	
    public Waypoint target; // next waypoint after this one.
	public float delayStartTime; // how long to wait before starting this waypoint.
    public float jumpForce;

    public Vector3 position
    {
        get { return transform.position; } 
    }
}
