using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectBase : MonoBehaviour {

	public string Name;
	public InteractiveObjectBase[] TargetsArray;
	public bool[] InteractionTriggerArray;

	public bool StopPlayerMovement; //if player interacts with this does it stop his movement. Yes for a switch, no for like a button, thing to pick up. 

	// Use this for initialization
	void Start () {
		Debug.Log (Name + " start");
		for (int i = 0; i < TargetsArray.Length; i++) {
			Debug.Log (Name + " on " + TargetsArray[i].Name);
		}

		InteractionTriggerArray = new bool[10];
	}
		
	//player interaction with this object
	public void StartTrigger(){
		InteractionTriggerArray [0] = true;
	}

	//player interaction with this object stops
	public void StopTrigger(){
		InteractionTriggerArray [0] = false;
	}

	void Update(){
		if(InteractionTriggerArray[0])
			Debug.Log (Name + " on");
	
	}

	//this is just the base class -- set interaction between objects in classes derived from this class;
}
