using UnityEngine;
using System.Collections;

public class TrackCopter : MonoBehaviour {

	GameObject copter;

	// Use this for initialization
	void Start () {
		copter = GameObject.Find ("Copter");
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (copter.transform.position.x,
		                                  copter.transform.position.y,
		                                  copter.transform.position.z-7);
		transform.rotation = copter.transform.rotation;
	}
}
