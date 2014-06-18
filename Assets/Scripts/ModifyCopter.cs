using UnityEngine;
using System.Collections;

public class ModifyCopter : MonoBehaviour {
	
	public float life;
	bool isAlive = false;
	public bool isActive = true;
	static GameObject copter;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (!isActive) return;
		if (isAlive && life > Time.deltaTime && !Move.isPaused && !Move.isDead) {
			life -= Time.deltaTime;
		} else if (isAlive && !Move.isPaused && !Move.isDead) {
			copter.GetComponent<Move>().UnApplyPickup(gameObject); // will trigger Die
		}
	}

	void FixedUpdate () {
		if (!isActive) return;
		Vector3 pos = transform.localPosition;
		transform.localPosition = new Vector3(pos.x,
		                                	  pos.y,
		                                 	  pos.z-(8*Move.speed*Time.deltaTime));
		if (!isAlive && transform.position.z < -3) {
			Die();
		}
	}

	public void Die() {
		isActive = false;
		isAlive = false;
		gameObject.SetActive (false);
		GameObject.Find ("Background").GetComponent<Spawn>().PickupPool.Add(gameObject);
	}

	public void Activate() {
		isActive = true;
		gameObject.SetActive (true);
		renderer.enabled = true;
		// Removal needs to happen near selection for use to minimize race conditions
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.name.Contains ("Copter") && !isAlive && isActive) {
			renderer.enabled = false;
			isAlive = true;

			copter = other.gameObject;
			tag = renderer.material.name.Replace (" (Instance)", "");
			copter.GetComponent<Move>().ApplyPickup(gameObject);
		}
	}
}
