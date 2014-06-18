using UnityEngine;
using System.Collections;

// TODO: high scores?

public class Move : MonoBehaviour {

	public static int level = 0;
	public static float speed = 1;
	public static float max_speed = 1.5f;
	public static float distance = 0;
	public static bool isDead = false;
	public static bool isPaused = false;
	public static bool showControls = false;
	public static float clearing = 0;
	public Material CopterMat;
	public ArrayList pickups = new ArrayList();

	bool[] paused_sounds;
	float paused_speed;
	Vector3 paused_angular_velocity;
	Vector3 warped_angular_velocity;
	Quaternion warped_angle;
	Light lite, fill;

	enum Sounds { copter=0, star, explosion, shrink, grow };
	AudioSource[] asources;

	struct LightOpts {
		public float r, g, b, a, intensity, range;
		public LightOpts (float red, float green, float blue, float alpha, float i, float r) {
			this.r = red;
			this.g = green;
			this.b = blue;
			this.a = alpha;
			this.intensity = i;
			this.range = r;
		}
	}

	LightOpts regular, explosion;

	// Use this for initialization
	void Start () {
		// Update Gauges and rotation on time based interval instead of every frame
		InvokeRepeating ("UpdateGauges", 0f, 0.2f);
		InvokeRepeating ("SetCaveRotation", 0f, 3f);
		regular = new LightOpts (1.0f, 1.0f, 1.0f, 1.0f, 1.18f, 30);
		explosion = new LightOpts (255f/255f, 134f/255f, 24f/255f, 1.0f, 4, 50);

		asources = GetComponents<AudioSource> ();
		asources [(int)Sounds.copter].Play ();
		paused_sounds = new bool[asources.Length];
		lite = GameObject.Find ("Light").light;
		fill = GameObject.Find ("Fill Light").light;
	}

	// Update is called once per frame
	void Update () {
		if (isDead || isPaused)
			Screen.showCursor = true;
		else
			Screen.showCursor = false;
		if (isDead) {
			return;
		}
		if (Input.GetKeyDown (KeyCode.Escape) && !isPaused) {
			Pause ();
		}
		else if (Input.GetKeyDown (KeyCode.Escape) && isPaused) {
			UnPause();
		}
		if (isPaused)
			return;
		if (clearing > Time.deltaTime) clearing -= Time.deltaTime;
		else clearing = 0;
		level = Mathf.RoundToInt(distance / 1000);
		max_speed = level*0.1f + 1.5f;
	}
	
	void FixedUpdate() {
		if (isDead || isPaused)
			return;
		
		// Simulate gravity
		rigidbody.AddForce (transform.TransformDirection(Vector3.down) * 9.81f,
		                    ForceMode.Acceleration);
		
		// Increment distance
		distance += speed * 100 * Time.deltaTime;
		
		// Hold open the throttle (but Copter has a top speed)
		if (Input.GetKey (KeyCode.Space) && speed < max_speed) {
			speed += Time.deltaTime/2;
		} else if (speed > Time.deltaTime) {
			// Momentum is a thing
			speed -= Time.deltaTime;
		}
		
		// Multiple if's to allow multiple directions at the same time
		// Modify velocity directly to avoid interacting with angular velocity
		if (Input.GetKey (KeyCode.LeftArrow) ||
		    Input.GetKey (KeyCode.A)) {
			rigidbody.velocity += transform.TransformDirection (Vector3.left)*speed*25*Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.RightArrow) ||
		    Input.GetKey (KeyCode.D)) {
			rigidbody.velocity += transform.TransformDirection (Vector3.right)*speed*25*Time.deltaTime;
		} 
		if (Input.GetKey (KeyCode.UpArrow) ||
		    Input.GetKey (KeyCode.W)) {
			rigidbody.velocity += transform.TransformDirection (Vector3.up)*speed*35*Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.DownArrow) ||
		    Input.GetKey (KeyCode.S)) {
			rigidbody.velocity += transform.TransformDirection (Vector3.down)*speed*25*Time.deltaTime;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag.Equals("Obstacle") && HavePickup ("Star")) {
			asources[(int)Sounds.explosion].Play ();
			other.SendMessage("Die");
		}
	}

	void OnGUI() {
		if (isDead) {
			// Make a group on the center of the screen
			GUI.BeginGroup (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 200, 120, 140));
			// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
			
			GUI.Box (new Rect (0,0,120,140), "Game Over :(");
			if (GUI.Button (new Rect (10,40,100,30), "Play again?")) {
				distance = 0;
				speed = 1;
				isDead = false;
				Application.LoadLevel ("Main");
			}
			if (GUI.Button (new Rect(10,80,100,30), "Quit")) {
				Application.Quit();
			}
			
			GUI.EndGroup ();
		}
		
		if (isPaused && !showControls) {
			// Make a group on the center of the screen
			GUI.BeginGroup (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 200, 120, 180));
			// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
			
			GUI.Box (new Rect (0,0,120,180), "Copter");
			if (GUI.Button (new Rect (10,40,100,30), "Continue")) {
				UnPause ();
			}
			if (GUI.Button (new Rect(10, 80, 100, 30), "Controls")) {
				showControls = true;
			}
			if (GUI.Button (new Rect(10,120,100,30), "Quit")) {
				Application.Quit();
			}
			
			GUI.EndGroup ();
		}
		
		if (showControls) {
			// Make a group on the center of the screen
			GUI.BeginGroup (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 200, Screen.width/4, 240));
			// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
			
			GUI.Box (new Rect (0,0,Screen.width/4,240), "Copter");
			
			GUI.Label(new Rect(10, 20, Screen.width/4, 150), "Spacebar = Throttle\nWASD/Arrows = Directions\n"+
			          "Esc = Pause\nRED = Clear cave\n" +
			          "PURPLE = Shrink Copter\n" +
			          "BLUE = Clear fog\n" +
			          "YELLOW = Star\n" +
			          "GREEN = Arrest rotation");
			
			if (GUI.Button (new Rect (10,160,Screen.width/4-20,30), "Continue")) {
				UnPause();
			}
			
			if (GUI.Button (new Rect(10,200,Screen.width/4-20,30), "Quit")) {
				Application.Quit();
			}
			
			GUI.EndGroup ();
		}
	}
	
	void Pause() {
		PauseMusic ();
		paused_speed = speed;
		paused_angular_velocity = rigidbody.angularVelocity;
		speed = 0;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		isPaused = true;
	}
	
	void UnPause() {
		UnPauseMusic ();
		rigidbody.constraints = RigidbodyConstraints.None;
		speed = paused_speed;
		rigidbody.angularVelocity = paused_angular_velocity;
		isPaused = false;
		showControls = false;
	}

	public void Die() {
		speed = 0;
		max_speed = 0;
		isDead = true;
		PauseMusic ();
		// Stop/Start in case it was already playing - we want to play from start, not from where it was paused.
		asources [(int)Sounds.explosion].Stop ();
		asources [(int)Sounds.explosion].Play ();
		
		//yield return new WaitForSeconds (asources [(int)Sounds.explosion].clip.length);
		
		ClearPickups ();

		renderer.material = CopterMat;
		renderer.material.color = new Color(255,0,0);
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}

	void PauseMusic() {
		for (int i=0; i < asources.Length; i++ ) {
			if (asources[i].isPlaying) {
				paused_sounds[i] = true;
			} else
				paused_sounds[i] = false;
			// Don't want to get unlucky w/ copter sound on dying
			asources[i].Pause();
		}
	}
	
	void UnPauseMusic() {
		for (int i=0; i < paused_sounds.Length; i++) 
			if (paused_sounds[i])
				asources[i].Play ();
	}

	void UpdateGauges() {
		if (!isPaused) {
			GameObject.Find ("Speedometer").guiText.text = (speed * 100).ToString("F0") + " m/s";
			GameObject.Find ("Odometer").guiText.text = (distance).ToString("F0") + " m";
		}
	}

	void SetCaveRotation() {
		if (isDead || isPaused || HavePickup ("Warp"))
			return;
		int direction = 1;
		if (Random.value < 0.5f) direction *= -1;
		rigidbody.angularVelocity = new Vector3 (0, 0, Random.Range (0.3f, 0.6f)*direction);
	}

	public void ApplyPickup(GameObject pickup) {
		pickups.Add (pickup);
		renderer.material = pickup.renderer.material;
		SendMessage (pickup.tag);
	}

	public void UnApplyPickup(GameObject pickup) {
		pickups.Remove (pickup);
		if (pickups.Count == 0)
			renderer.material = CopterMat;
		else
			renderer.material = ((GameObject)pickups [pickups.Count - 1]).renderer.material;
		SendMessage ("Un"+pickup.tag);
		pickup.SendMessage ("Die"); // Handles returning to PickupPool
	}

	void ClearPickups() {
		while (pickups.Count != 0) {
			UnApplyPickup((GameObject)pickups[pickups.Count-1]);
		}
	}

	public bool HavePickup(string nm) {
		foreach (GameObject pickup in pickups) {
			if (pickup.tag.Equals (nm)) return true;
		}
		return false;
	}

	public void Clear() {
		lite.color = new Color (explosion.r,
		                        explosion.g,
		                        explosion.b,
		                        explosion.a);
		lite.intensity = explosion.intensity;
		lite.range = explosion.range;
		fill.color = new Color (explosion.r,
		                        explosion.g,
		                        explosion.b,
		                        explosion.a);
		fill.intensity = explosion.intensity;
		fill.range = explosion.range;
		asources [(int)Sounds.explosion].Play ();
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag ("Obstacle");
		for (int i = 0; i < obstacles.Length; i++) {
			obstacles[i].SendMessage("Die");
		}
		clearing = speed;
		GameObject.Find ("Fog").renderer.enabled = false;
	}

	public void UnClear() {
		GameObject.Find ("Fog").renderer.enabled = true;
		lite.color = new Color (regular.r,
		                        regular.g,
		                        regular.b,
		                        regular.a);
		lite.intensity = regular.intensity;
		lite.range = regular.range;
		fill.color = new Color (regular.r,
		                        regular.g,
		                        regular.b,
		                        regular.a);
		fill.intensity = regular.intensity;
		fill.range = regular.range;
	}

	public void Shrink() {
		asources [(int)Sounds.shrink].Play ();
		if (transform.localScale.x > 0.25)
			transform.localScale /= 4;
	}

	public void UnShrink() {
		if (!HavePickup("Shrink")) {
			transform.localScale *= 4;
			if (!isDead)
				asources [(int)Sounds.grow].Play ();
		}
	}

	public void Sight() {
		GameObject.Find ("Fog").renderer.enabled = false;
		GameObject.Find ("Light").light.range = 40;
	}

	public void UnSight() {
		if (!HavePickup ("Sight")) {
			GameObject.Find ("Fog").renderer.enabled = true;
			GameObject.Find ("Light").light.range = 30;
		}
	}

	public void Star() {
		collider.isTrigger = true;
		asources [(int)Sounds.copter].Pause ();
		asources [(int)Sounds.star].Play ();
	}

	public void UnStar() {
		if (!HavePickup ("Star")) {
			collider.isTrigger = false;
			asources [(int)Sounds.star].Stop ();
			if (!isDead) {
				asources [(int)Sounds.copter].Play ();
			}
		}
	}

	public void Warp() {
		warped_angular_velocity = rigidbody.angularVelocity;
		warped_angle = transform.rotation;
		//transform.rotation = Quaternion.identity;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	public void UnWarp() {
		if (!HavePickup("Warp")) {
			rigidbody.constraints = RigidbodyConstraints.None;
			if (!isDead) {
				rigidbody.angularVelocity = warped_angular_velocity;
			}
			transform.rotation = warped_angle;
		}
	}


}
