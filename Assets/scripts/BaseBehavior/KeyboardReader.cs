using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardReader : AbstractInputReader {

	// Use this for initialization
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Q)) {
			passOnTouch (new TouchedBots ("BoxOneTwo", "1")); 
		} else {
			passOnTouch (new TouchedBots ("BoxOneTwo", "0"));	
		}

		if (Input.GetKey (KeyCode.W)) {
			passOnTouch(new TouchedBots("BoxTwoThree", "1")); 		
		} else {
			passOnTouch (new TouchedBots ("BoxTwoThree", "0"));	
		}

		if (Input.GetKey (KeyCode.E)) {
			passOnTouch(new TouchedBots("BoxOneThree", "1")); 		
		} else {
			passOnTouch (new TouchedBots ("BoxOneThree", "0"));	
		}

		if (Input.GetKey (KeyCode.R)) {
			passOnTouch(new TouchedBots("AllBoxes", "1")); 		
		} else {
			passOnTouch (new TouchedBots ("AllBoxes", "0"));	
		}

		if (Input.GetKey(KeyCode.Alpha1)) {
			passOnBotDataReceived(new Bot("botOne","0","0","0","0","1"));
		} else {
			passOnBotDataReceived(new Bot("botOne","0","0","0","0","0"));
		}

		if (Input.GetKey(KeyCode.Alpha2)) {
			passOnBotDataReceived(new Bot("botTwo","0","0","0","0","1"));
		} else {
			passOnBotDataReceived(new Bot("botTwo","0","0","0","0","0"));
		}

		if (Input.GetKey(KeyCode.Alpha3)) {
			passOnBotDataReceived(new Bot("botThree","0","0","0","0","1"));
		} else {
			passOnBotDataReceived(new Bot("botThree","0","0","0","0","0"));
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			Debug.Log ("P pressed");
			MenuSecretCiphers ();
		}
		if (Input.GetKeyDown (KeyCode.O)) {
			MenuFreePlay ();
		}

	}
}
