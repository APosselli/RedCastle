using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSwitch : InteractiveObjectBase {


	// Update is called once per frame
	void Update () {

		//if the switch is on, turn on all the objects that it is connected to. If the switch is off, turn them off. 
		if (InteractionTriggerArray [0]) {
			for (int i = 0; i < TargetsArray.Length; i++) {
				TargetsArray [i].InteractionTriggerArray [0] = true;
			}
			
		} else {
			for (int i = 0; i < TargetsArray.Length; i++) {
				TargetsArray [i].InteractionTriggerArray [0] = false;
			}
		}
		
	}
}
