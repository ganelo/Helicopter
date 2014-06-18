using UnityEngine;
using System.Collections;

public class KillCopter : MonoBehaviour {

	public GameObject Obstacle;
	GameObject left, right, up, down;
	public static ArrayList ObstaclePool = new ArrayList();
	
	// Use this for initialization
	void Start () {
		left = GameObject.Find ("Left");
		right = GameObject.Find ("Right");
		up = GameObject.Find ("Up");
		down = GameObject.Find ("Down");
	}

	// Update is called once per frame
	void Update () {
		float dim;
		if (this.Equals(left) || this.Equals(right)) {
			dim = right.transform.position.x - left.transform.position.x;
		} else {
			dim = up.transform.position.y - down.transform.position.y;
		}
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
		GameObject obstacle;
		if (obstacles.Length < (Move.level + 12) && Move.clearing == 0f) {

			// Each wall has a 1/4 chance of spawning an object
			if (Random.value <= 0.25) {
				if (ObstaclePool.Count != 0) {
					obstacle = (GameObject)ObstaclePool[ObstaclePool.Count-1];
					ObstaclePool.Remove (obstacle);
					obstacle.GetComponent<Sliders>().Activate();
				} else {
					obstacle = (GameObject)Instantiate(Obstacle);
				}
				obstacle.transform.localScale = new Vector3(1,
				                                            Random.Range (3f*dim/100,
				                                                          Mathf.Min (dim/8 + Move.level,
				                        											 dim/4)),
				                                            1);
				obstacle.transform.parent = transform;
				obstacle.transform.localRotation = Quaternion.identity;
				obstacle.transform.localPosition = new Vector3(Random.Range (-dim/2+0.5f, dim/2-0.5f),
				                                               // Keep entire object in cave
				                                               obstacle.transform.localScale.y,
				                                               Random.Range(4.8f,0.5f));
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.name.Contains ("Copter")) {
			other.SendMessage("Die");
		}
	}
}
