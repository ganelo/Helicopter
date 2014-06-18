using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public static bool showControls = false;

	void OnGUI() {
		if (!showControls) {
			// Make a group on the center of the screen
			GUI.BeginGroup (new Rect (Screen.width / 2 - 50, Screen.height / 2 - 50, 120, 180));
			// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
			
			GUI.Box (new Rect (0,0,120,180), "Copter");
			if (GUI.Button (new Rect (10,40,100,30), "Play")) {
				Application.LoadLevel ("Main");
			}
			if (GUI.Button (new Rect(10, 80, 100, 30), "Controls")) {
				showControls = true;
			}
			if (GUI.Button (new Rect(10,120,100,30), "Quit")) {
				Application.Quit();
			}
			
			GUI.EndGroup ();
		} else {
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
				Application.LoadLevel ("Main");
			}
			
			if (GUI.Button (new Rect(10,200,Screen.width/4-20,30), "Quit")) {
				Application.Quit();
			}
			
			GUI.EndGroup ();
		}
	}
}
