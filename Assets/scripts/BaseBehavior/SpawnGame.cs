using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this script is critical for spawning all of the prefabs which implement all the core game data into the hierarcy when level starts

public class SpawnGame : MonoBehaviour {
    //this is to keep the game running in background mode
    public bool debugLog = false;
    public bool runInBackgroundValue = true; 

    // these are the game prefabs. find them in the prefab folder 
    public GameObject serialDataManager;
	public GameObject keyboardDataManager;
    public GameObject botDataManager;
    public GameObject touchManager; 

	public bool serial;

    // add all the important prefabs and get everything setup nicely. once you do this, you can use all the bots and touches, send led changes to the bots. 
    void Start () {
		serialDataManager = Instantiate (serialDataManager, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		//serialDataManager.SetActive (false);
		keyboardDataManager = Instantiate (keyboardDataManager, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		//keyboardDataManager.SetActive (false);
		botDataManager = Instantiate(botDataManager, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		touchManager = Instantiate (touchManager, new Vector3 (0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		serial = true;
		switchControls (serial);
        Application.runInBackground = true;
	}
    //lets you switch between keyboard and serial data 
	void switchControls(bool serial) {
		serialDataManager.SetActive (serial);
		keyboardDataManager.SetActive(!serial);
		if (serial) {
			Debug.Log ("Switched to Serial Control");
		} else {
			Debug.Log ("Switched to Keyboard Control");
		}
	} 
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Semicolon)) {
			serial = !serial;
			switchControls (serial);
		}
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        if (debugLog) {
            runInBackgroundValue = Application.runInBackground; 
        }
        if (!Application.runInBackground)
        {
            Application.runInBackground = true;
            if (debugLog)
            {
                Debug.Log("Resetting appplcation.runInBackground mode to true at: " + Time.time);
            }
        }
	}
}
