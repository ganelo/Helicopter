using UnityEngine;
using System.Collections;

public class Sliders : MonoBehaviour {

	public bool isActive = true;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!isActive) return;
		Vector3 pos = transform.localPosition;
		transform.localPosition = new Vector3(pos.x,
		                                      pos.y,
		                                      pos.z-(2*Move.speed*Time.deltaTime));
		if (transform.localPosition.z < -3) {
			Die();
		}
	}

	public void Die() {
		tag = "ObstaclePool";
		isActive = false;
		gameObject.SetActive (false);
		transform.parent = null;
		KillCopter.ObstaclePool.Add(gameObject);
	}

	public void Activate() {
		tag = "Obstacle";
		isActive = true;
		gameObject.SetActive (true);
		// Removal needs to happen near selection for use to minimize race conditions
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.name.Contains ("Copter") && !other.GetComponent<Move>().HavePickup("Star") && isActive) {
			other.SendMessage ("Die");
		}
	}
}
