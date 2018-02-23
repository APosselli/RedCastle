using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotStruct : MonoBehaviour {

	public ShotStruct PreviousShot;
	public int[] NextShotBranch; // 0 for ShotEnded, 1+ for triggers


	public CameraStruct[] CameraStruct_Array;
	public bool Continue_Cameras; // sets new struct data, keeps old camera in old position

	public GameObjectStruct[] GameObjectStruct_Array;
	public bool Continue_GameObjects; // sets new struct data, keeps old gameobjects in old position


	public float ShotTime;
	public float ShotEndTime;

}
