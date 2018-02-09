using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
    public Waypoint target;
    public float jumpForce;

    public Vector3 position
    {
        get { return transform.position; } 
    }
}
