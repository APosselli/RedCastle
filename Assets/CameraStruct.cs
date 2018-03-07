using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nick
public class CameraStruct : GameObjectStruct {

	// attach effect
	public bool IsAttached;
	public int AttachedTo; //which gameobject
	public Vector3 AttatchedOffset;

	// look at effect
	public bool IsLookAt;
	public int LookAt; // which gameobject

	//values for viewport
	//set in camera
	//public float ViewportWidth;//w
	//public float ViewportHeight;//h
}
