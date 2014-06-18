using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {

	public GameObject Pickup;
	public Material Warp, Clear, Shrink, Star, Sight;
	public ArrayList PickupPool = new ArrayList();

	float lastSpawnPoint;

	// Use this for initialization
	void Start () {
		lastSpawnPoint = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Move.distance - lastSpawnPoint < (500+Move.level*Move.speed*100) || Move.isDead || Move.isPaused)
			return;
		Debug.Log (500 + Move.level * Move.speed * 100);
		// Spawn pickup
		lastSpawnPoint = Move.distance;
		GameObject pickup;
		if (PickupPool.Count == 0) {
			pickup = (GameObject)Instantiate (Pickup);
		} else {
			pickup = (GameObject)PickupPool[PickupPool.Count-1];
			PickupPool.Remove (pickup);
			pickup.GetComponent<ModifyCopter>().Activate();
		}
		pickup.transform.position = new Vector3(Random.Range (-1f,1f),
		                                        Random.Range (-1f, 1f),
		                                        Random.Range(25,10));
		if (Random.value < 1f / 5) {
			pickup.renderer.material = Warp;
			pickup.GetComponent<ModifyCopter>().life = 3;
		} else if (Random.value < 1f/4) { // 1/4 of 4/5 == 1/5
			pickup.renderer.material = Star;
			pickup.GetComponent<ModifyCopter>().life = 10;
		} else if (Random.value < 1f/3) { // 1/3 of 3/5 == 1/5
			pickup.renderer.material = Shrink;
			pickup.GetComponent<ModifyCopter>().life = 3;
		} else if (Random.value < 1f/2) { // 1/2 of 2/5 == 1/5
			pickup.renderer.material = Clear;
			pickup.GetComponent<ModifyCopter>().life = 0.25f;
		} else {
			pickup.renderer.material = Sight;
			pickup.GetComponent<ModifyCopter>().life = 3;
		}
	}
}
