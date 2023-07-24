using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//data structure to hold all of the bot data. 
public struct Bot {
	public Bot(string name_, string compass_, string xpos_, string ypos_, string zpos_, string btn_) {
		name = name_; compass = compass_; xpos = xpos_; ypos = ypos_; zpos = zpos_; btn = btn_; def = true;
	}
    //easy way to see the data each bot 
	public void printBotData() {
		Debug.Log ("Bot: " + name + ", compass: " + compass + " x,y,z = " + xpos + "," + ypos + "," + zpos + " btn = " + btn);
	}

    //vars to copy incomming data into 
	public string name;
	public string compass;
	public string xpos;
	public string ypos;
	public string zpos;
	public string btn;
	public bool def;
}
public struct MenuButtonState {
	public MenuButtonState(string oc_button, string slc_button) {
		if (oc_button.Equals ("1")) {
			oc = true;
		} else {
			oc = false;
		}

		if (slc_button.Equals ("1")) {
			slc = true;
		} else {
			slc = false;
		}
		def = true;
	}

	public bool slc;
	public bool oc;
	public bool def;
}

//data strcuture holding the two bots touching 
public struct TouchedBots {
	public TouchedBots(string touch1, string touch2) {
		botsTouched = touch1;
		touch = touch2;         
	}
        
	public void printTouchData() {
		Debug.Log ("Touching: " + touch + " and " + botsTouched);
	}
	public string botsTouched;
	public string touch; 
}

public enum LED_CHANGES {None=0, On, Off, Set, FadeOn, FadeOff};
public class AbstractInputReader : MonoBehaviour {
	public delegate void WriteToSerial(byte[] wri); //all methods that subscribe to this delegate must be void and pass in a bite of 8 bits in an array  
	public static event WriteToSerial OnWriteToSerial; //this is the event to register your functions to 

    //these two bytes for the flag then the value. 
    // Serial write sends data that looks like this 
    // XX YYYYYY / ZZZZZZZZ``X` = LED number(01 = 1, 10 = 2, 11 = 3, 00 = RESERVED)
    //`Y` = Type of change(Flags, where 100000 = on, 010000 = off, 001000 = set to value, 000100 = fade on, 000010 = fade off, 000001 = RESERVED)    
    //`Z` = Parameter(Interpreted as an 8bit unsigned integer) (

    public void HandleLEDChange(int led, LED_CHANGES type, int parameter) {
		byte first = (byte) (((byte) led) << 6); //sets the LED to trigger to be the last two bits of the bit 
        //these lines simply set the flags up for what mode to trigger
		if (type == LED_CHANGES.On) {
			first += 32; //0010 0000
		} else if (type == LED_CHANGES.Off) {
			first += 16; //0001 0000 
		} else if (type == LED_CHANGES.Set) {
			first += 8; //0000 1000
		} else if (type == LED_CHANGES.FadeOn) {
			first += 4; //0000 0100
		} else if (type == LED_CHANGES.FadeOff) {
			first += 2; //000 0010 
		}
        //call function to write the data and pass the vars first and parameter as a bite array of 2 elements . 
		passWrite(new byte[] { first, (byte)parameter});
	}
    //function calls the werite to serial function and sends it a bite array. 
	public static void passWrite(byte[] wri) {
		if (OnWriteToSerial != null) {
			OnWriteToSerial (wri);
		}
	}

    //adds HandleLedChange to the DoLedChange event 
	public void OnEnable() {
		AbstractManager.DoLEDChange += HandleLEDChange;
	}
	public void OnDisable() {
		AbstractManager.DoLEDChange -= HandleLEDChange;
	}

	protected Bot b;
	protected TouchedBots touchedBots;
	public delegate void BotDataReceived(Bot b_);
	public static event BotDataReceived OnBotDataReceived; //if data size of bot data string is received, call this event and pass in a bot. 

	public delegate void TouchManagerReceived(TouchedBots t_); //if data size of touch data string received, call thisOn Touch and pass in a touchedBots enum. 
	public static event TouchManagerReceived OnTouch; 

    //the function make sure the bot data is not not and if not, calls the OnBotDataReceived event
	protected void passOnBotDataReceived(Bot b_) {
		if (OnBotDataReceived != null) OnBotDataReceived (b_);
	}
    //the function to make sure the touch data is not null and calls the OnTouch event 
	protected void passOnTouch(TouchedBots t_) {
		if (OnTouch != null) OnTouch (t_);
	}
	public delegate void MenuFreePlayPushed ();
	public static event MenuFreePlayPushed OnMenuFreePlayPushed;
	public delegate void MenuSecretCiphersPushed();
	public static event MenuSecretCiphersPushed OnMenuSecretCiphersPushed;
	bool freeplay_down, secret_down;
	protected void MenuFreePlay() {
		freeplay_down = true;
		if (OnMenuFreePlayPushed != null) {
				OnMenuFreePlayPushed ();
		}
	}
	protected void MenuSecretCiphers() {
		secret_down = true;
		if (OnMenuSecretCiphersPushed != null) {
			OnMenuSecretCiphersPushed ();
		}
	}
		

	// Update is called once per frame
	void Update () {
		
	}
}
