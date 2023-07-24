using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum TouchState { None=0, OneTwo, TwoThree, OneThree, AllConnected };
public class TouchTouchManager : AbstractManager {
	public static int TO_WIN = 10;
	bool enabled = true;
	TouchState target = TouchState.None;
	int score = 0;
	public AudioClip Sound_Win, Sound_Success, Sound_Fail;
	new void OnEnable() {
		base.OnEnable ();
	}

	void Start() {
		score = 0;
		newTarget ();
	}

	void win()
	{
		gameObject.transform.Find ("TargetText").GetComponent<TextMesh>().text = "You win!";
		gameObject.transform.Find ("TouchSound").GetComponent<AudioSource> ().Stop ();
		if (!gameObject.transform.Find ("TouchSound").GetComponent<AudioSource> ().isPlaying) {
			gameObject.transform.Find ("TouchSound").GetComponent<AudioSource> ().clip = Sound_Win as AudioClip;
			gameObject.transform.Find ("TouchSound").GetComponent<AudioSource> ().Play ();
			gameObject.transform.Find ("TouchSound").GetComponent<AudioSource> ().loop = true;
		}
	}

	void newTarget() {
		TouchState new_t = target;
		while (new_t == target) {
			new_t = (TouchState) (Random.Range (1, 5));
		}
		target = new_t;
		gameObject.transform.Find ("TargetText").GetComponent<TextMesh> ().text = "Target: " + target;
		lightUp (target, 30);
	}

	void lightUp(TouchState newTarget, int time) {
		TurnOffLEDOne ();
		TurnOffLEDTwo ();
		TurnOffLEDThree ();
		if (newTarget == TouchState.AllConnected || newTarget == TouchState.OneThree || newTarget == TouchState.OneTwo) {
			TurnOnLEDOne ();
			FadeLEDOneOff (time);
		}
		if (newTarget == TouchState.AllConnected || newTarget == TouchState.TwoThree || newTarget == TouchState.OneTwo) {
			TurnOnLEDTwo ();
			FadeLEDTwoOff (time);
		}
		if (newTarget == TouchState.AllConnected || newTarget == TouchState.OneThree || newTarget == TouchState.TwoThree) {
			TurnOnLEDThree ();
			FadeLEDThreeOff (time);
		}
	}

	void success() {
		score += 1;
		gameObject.transform.Find ("TouchSound").GetComponent<AudioSource> ().clip = Sound_Success as AudioClip;
		gameObject.transform.Find ("TouchSound").GetComponent<AudioSource> ().Play ();
	}

	void fail() {
		gameObject.transform.Find ("TouchSound").GetComponent<AudioSource> ().clip = Sound_Fail as AudioClip;
		gameObject.transform.Find ("TouchSound").GetComponent<AudioSource> ().Play ();
	}

	void checkMatch(TouchState input) {
		if (!enabled)
			return;
		
		Debug.Log ("Test match: " + input);
		if (input == target) {
			success ();
			newTarget ();

		} else if (target == TouchState.AllConnected) {
			// pass. Reason why is because otherwise getting to a three-touch would cause a fail noise.
		} else {
			fail ();
		}

		if (score >= TO_WIN) {
			enabled = false;
			win ();
		}
	}

	public override void BoxOneTwoConnected()
	{
		checkMatch (TouchState.OneTwo);
	}

	public override void BoxOneThreeConnected() {
		checkMatch (TouchState.OneThree);
	}

	public override void BoxTwoThreeConnected() {
		checkMatch (TouchState.TwoThree);
	}

	public override void AllConnected() {
		checkMatch (TouchState.AllConnected);
	}
}
