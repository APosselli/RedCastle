using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nick
public class GameController : MonoBehaviour {

	public DialogueManager DialogueManagerSpawned;

	public ShotStruct[] Shot_Struct_Array;
	ShotStruct Previous_Shot;
	ShotStruct Current_Shot;


	// Use this for initialization
	void Start () { 
		
		// spawn everything
		StartShot(0);
			
	}

	//update shot
	void Update ()
	{
		Current_Shot.ShotTime += Time.deltaTime;
		if (Current_Shot.ShotTime >= Current_Shot.ShotEndTime)
		{
			StartShot (Current_Shot.NextShotBranch[0]);
		}

	}

	//call this from other objects, like triggers and puzzles -- 0 is reserved for scene end time
	public void StartBranch(int BranchNumber)
	{
		StartShot (Current_Shot.NextShotBranch[BranchNumber]);
	}

	//begin the next shot and end the previous shot
	void StartShot ( int ShotNumber)
	{
		//set the previous shot, if this isn't the first shot
		if (Current_Shot != null)
		{
			Previous_Shot = Current_Shot;
		}

		Current_Shot = Shot_Struct_Array [ShotNumber];

		// gameobjects

		// spawn gameobjects
		for(int i = 0; i < Current_Shot.GameObjectStruct_Array.Length; i++)
		{
			//continue from previous shot
			if (Current_Shot.Continue_GameObjects && i < Previous_Shot.GameObjectStruct_Array.Length) {
				Current_Shot.GameObjectStruct_Array [i].SpawnedObject = Previous_Shot.GameObjectStruct_Array [i].SpawnedObject;

			} 
			//spawn new cameras
			else {
				Current_Shot.GameObjectStruct_Array [i].SpawnedObject = Instantiate (Current_Shot.GameObjectStruct_Array [i].ObjectPrefab, Current_Shot.GameObjectStruct_Array [i].SpawnPosition, Quaternion.identity);

			}

		}

		// targets

		// spawn interactiveobjects
		for(int i = 0; i < Current_Shot.InteractiveObjectStruct_Array.Length; i++)
		{
			//continue from previous shot
			if (Current_Shot.Continue_GameObjects && i < Previous_Shot.InteractiveObjectStruct_Array.Length) {
				Current_Shot.InteractiveObjectStruct_Array [i].SpawnedInteractiveObject = Previous_Shot.InteractiveObjectStruct_Array [i].SpawnedInteractiveObject;

			} 
			//spawn new interactiveobjects
			else {
				Current_Shot.InteractiveObjectStruct_Array [i].SpawnedInteractiveObject = Instantiate (Current_Shot.InteractiveObjectStruct_Array [i].InteractiveObjectPrefab, Current_Shot.InteractiveObjectStruct_Array [i].SpawnPosition, Quaternion.identity);
			}
		}

		// set targets -- outside of spawn function because it needs to access all spawned objects. 
		for(int i = 0; i < Current_Shot.InteractiveObjectStruct_Array.Length; i++)
		{
			//continue from previous shot
			if (!(Current_Shot.Continue_GameObjects && i < Previous_Shot.InteractiveObjectStruct_Array.Length)) {
				
				//set size of targets array
				Current_Shot.InteractiveObjectStruct_Array [i].SpawnedInteractiveObject.TargetsArray = new InteractiveObjectBase[Current_Shot.InteractiveObjectStruct_Array [i].TargetsArray_Numbers.Length];

				//set targets for interactiveobjects;
				for (int j = 0; j < Current_Shot.InteractiveObjectStruct_Array [i].TargetsArray_Numbers.Length; j++) {
					Current_Shot.InteractiveObjectStruct_Array [i].SpawnedInteractiveObject.TargetsArray [j] = Current_Shot.InteractiveObjectStruct_Array[Current_Shot.InteractiveObjectStruct_Array [i].TargetsArray_Numbers[j]].SpawnedInteractiveObject;
				}
			} 
		}

		// cameras

		// spawn cameras
		for(int i = 0; i < Current_Shot.CameraStruct_Array.Length; i++)
		{
			//continue from previous shot
			if (Current_Shot.Continue_Cameras && i < Previous_Shot.CameraStruct_Array.Length) {
				Current_Shot.CameraStruct_Array [i].SpawnedObject = Previous_Shot.CameraStruct_Array [i].SpawnedObject;
			} 
			//spawn new cameras
			else{
				Current_Shot.CameraStruct_Array[i].SpawnedObject = Instantiate(Current_Shot.CameraStruct_Array[i].ObjectPrefab, Current_Shot.CameraStruct_Array[i].SpawnPosition, Quaternion.identity);
			}
		}

		// dialogue

		if (Current_Shot.ShotDialogueOn) {

			DialogueManagerSpawned.StartDialogue (Current_Shot.ShotDialogue);//test dialogue
		}

		//end the previous shot (if there was one) , don't destroy objects if they are supposed to continue. 
		if (Previous_Shot != null) {
			EndShot (Previous_Shot, Current_Shot);
		}


	}

	//end the previous shot
	void EndShot ( ShotStruct EndingShot, ShotStruct NewShot)
	{
		EndingShot.ShotTime = 0;

		//don't destroy if new shot needs these to continue
		if(!NewShot.Continue_GameObjects)
		{
			// destroy gameobjects
			for(int i = 0; i < EndingShot.GameObjectStruct_Array.Length; i++)
			{
				Destroy (EndingShot.GameObjectStruct_Array [i].SpawnedObject);
			}
		}

		//don't destroy if new shot needs these to continue
		if(!NewShot.Continue_Cameras)
		{
			// destroy cameras
			for(int i = 0; i < EndingShot.CameraStruct_Array.Length; i++)
			{
				Destroy (EndingShot.CameraStruct_Array [i].SpawnedObject);
			}
		}

		//don't destroy if new shot needs these to continue
		if(!NewShot.Continue_InteractiveObjects)
		{
			// destroy cameras
			for(int i = 0; i < EndingShot.InteractiveObjectStruct_Array.Length; i++)
			{
				Destroy (EndingShot.InteractiveObjectStruct_Array [i].SpawnedObject);
			}
		}
	}

		//CameraReference.GetComponent<CameraController> ().player = PlayerReference;
	//TODO: integrate dialogue

}
