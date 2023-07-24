using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{


    //    //this class handles all of the touches between boxes 

    public static string touchingBoxes; //what boxes are touching 
    public static int touch = 0; //touch state 
    public GameObject bot1;
    public GameObject bot2;
    public GameObject bot3;
    public AudioClip onetwo;
    public AudioClip twothree;
    public AudioClip onethree;
    public AudioClip allthree;
    public ArrayList touchsound = new ArrayList();
    private bool allOn = false;

    private bool boxOneTwoOn = false;
    private bool boxTwoThreeOn = false;
    private bool boxOneThreeOn = false;

    public int boxOneTwoPreviousState;
    public int boxOneThreePreviousState;
    public int boxTwoThreePreviousState;
    public int allTouchingPreviousState;

    public delegate void BoxOneTwoAction(); //all methods that subscribe to this delegate must be void and pass in no data 
    public static event BoxOneTwoAction OnBoxOneTwoTouched; //this is the event to register your functions to 


    public delegate void BoxOneThreeAction();
    public static event BoxOneThreeAction OnBoxOneThreeTouched;

    public delegate void BoxTwoThreeAction();
    public static event BoxTwoThreeAction OnBoxTwoThreeTouched;

    public delegate void AllBoxesConnectedAction();
    public static event AllBoxesConnectedAction OnAllBoxesConnected;


    public delegate void BoxOneTwoReleaseAction(); //all methods that subscribe to this delegate must be void and pass in no data 
    public static event BoxOneTwoReleaseAction OnBoxOneTwoReleased; //this is the event to register your functions to 


    public delegate void BoxOneThreeleaseAction(); //all methods that subscribe to this delegate must be void and pass in no data 
    public static event BoxOneTwoReleaseAction OnBoxOneThreeReleased; //this is the event to register your functions to 

    public delegate void BoxTwoThreeReleaseAction(); //all methods that subscribe to this delegate must be void and pass in no data 
    public static event BoxTwoThreeReleaseAction OnBoxTwoThreeReleased; //this is the event to register your functions to 


    public delegate void AllBoxesReleasedAction();
    public static event AllBoxesReleasedAction OnAllBoxesReleased;

    private void Start()
    {

    }

	void OnEnable() {
        AbstractInputReader.OnTouch += processOnTouch;
	}

	void OnDisable() {
        AbstractInputReader.OnTouch -= processOnTouch;
	}

	private void processOnTouch(TouchedBots t_) {
		/* This is essentially just the old updateData code, but slightly modified.
		 * It sends events if there are *CHANGES* in touches
		 * I.e. new touches, or touches being broken. */
		string touchingBoxes = t_.botsTouched;
		int touch;
		int.TryParse(t_.touch, out touch);
		if (touchingBoxes == "BoxOneTwo" && touch == 1 && allOn == false && boxOneTwoOn == false)
		{
				if(OnBoxOneTwoTouched != null)
					OnBoxOneTwoTouched(); 
			boxOneTwoOn = true;
			boxOneTwoPreviousState = touch; 
		}
		else if (touchingBoxes == "BoxOneTwo" && touch == 0 && boxOneTwoPreviousState == 1)
		{
			boxOneTwoOn = false;
			boxOneTwoPreviousState = touch; 
			if (OnBoxOneTwoReleased != null)
				OnBoxOneTwoReleased();
		}

		else if (touchingBoxes == "BoxTwoThree" && touch == 1 && allOn == false && boxTwoThreeOn == false)
		{
				if (OnBoxTwoThreeTouched != null)
					OnBoxTwoThreeTouched();
			boxTwoThreeOn = true;
			boxTwoThreePreviousState = touch;
		}
		else if (touchingBoxes == "BoxTwoThree" && touch == 0 && boxTwoThreePreviousState == 1)
		{
			boxTwoThreeOn = false;
			if (OnBoxTwoThreeReleased != null)
				OnBoxTwoThreeReleased();
			boxTwoThreePreviousState = touch;
		}

		else if (touchingBoxes == "BoxOneThree" && touch == 1 && allOn == false && boxOneThreeOn == false)
		{
				if (OnBoxOneThreeTouched != null)
					OnBoxOneThreeTouched();
			
			boxOneThreeOn = true;
			boxOneThreePreviousState = touch;
		}
		else if (touchingBoxes == "BoxOneThree" && touch == 0 && boxOneThreePreviousState == 1)
		{
			boxOneThreeOn = false;
			if (OnBoxOneThreeReleased != null)
				OnBoxOneThreeReleased();
			boxOneThreePreviousState = touch;
		}

		else if (touchingBoxes == "AllBoxes" && touch == 1 && allOn == false)
		{
				if (OnAllBoxesConnected != null) {
					OnAllBoxesConnected(); 
				} 
			allOn = true;
			allTouchingPreviousState = touch; 
		}else if (touchingBoxes == "AllBoxes" && touch == 0 && allTouchingPreviousState == 1)
		{
			allOn = false;
			if(OnAllBoxesReleased !=null)
				OnAllBoxesReleased();

			allTouchingPreviousState = touch; 


		}


	}
}