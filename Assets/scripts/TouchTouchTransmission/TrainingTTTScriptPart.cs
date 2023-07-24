using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTTTScriptPart : AbstractTTTScriptPart {

	int currentPart;
	int times_failed = 0;
	float nextTime = 0;
	void Update() {
		if (currentPart == 1 && nextTime <= Time.time) {
			currentPart = 2;
			partTwo ();
		} else if (currentPart == 5 && nextTime <= Time.time) {
			currentPart = 6;
			partSix ();
		} else if (currentPart == 7 && nextTime <= Time.time) {
			currentPart = 8;
			partEight ();
		}
	}
	public override void startPart() {
		Debug.Log ("TRAINING BEGIN");
		currentPart = 1;
		partOne ();
	}

	public override void targetSuccess() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Success 2") as AudioClip);
		if (currentPart == 2) {
			currentPart = 3;
			partThree ();
		} else if (currentPart == 3) {
			currentPart = 4;
			partFour ();
		} else if (currentPart == 4) {
			currentPart = 5;
			partFive ();
		} else if (currentPart == 6) {
			currentPart = 7;
			partSeven ();
		}
	}
	public override void targetFailure() {
		// So fun fact you can actually increase your score by failing once first but um
		// It compiles, ship it.
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Fail 2") as AudioClip);
		times_failed += 1;
		if (times_failed <= 5) {
			currentPart = 1;
			partOne (true);
		} else {
			SendTerminate ();
		}
	}

	void partOne() {
				partOne(false);
	}
	void partOne(bool nudge) {
		List<AudioClip> clips;
		if (!nudge) {
			clips = new List<AudioClip> () { Resources.Load ("TouchTouchTransmission/dialog/Engaging Human Int Train Mode") as AudioClip, 
				Resources.Load ("TouchTouchTransmission/dialog/Humans Touch Light Up") as AudioClip
			};
		} else {
			clips = new List<AudioClip> () { Resources.Load ("TouchTouchTransmission/dialog/Remember Touch LIght Up") as AudioClip };	
		}
		SendPlayVoices (clips);
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
	void partTwo() {
		SendNewTarget (TouchState.OneTwo,250, 0.2f);
	}
	void partThree() {
		SendNewTarget (TouchState.TwoThree,250, 1);
	}
	void partFour() {
		SendNewTarget (TouchState.OneThree,250, 1);
	}
	void partFive() {
		SendClearTargets ();
		List<AudioClip> clips = new List<AudioClip>() { Resources.Load ("TouchTouchTransmission/dialog/Basic Succes Adv Tech") as AudioClip 
		};
		SendPlayVoices (clips);
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
	void partSix() {
		SendNewTarget (TouchState.AllConnected, 250, 0.2f);
	}
	void partSeven() {
		SendClearTargets ();
		List<AudioClip> clips = new List<AudioClip>() { Resources.Load ("TouchTouchTransmission/dialog/Humans Op Param") as AudioClip 
		};
		SendPlayVoices (clips);
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
	void partEight() {
		SendEndScriptPart ();
	}
}
