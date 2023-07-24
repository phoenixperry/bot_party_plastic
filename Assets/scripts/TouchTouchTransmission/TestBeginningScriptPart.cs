using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public class TestBeginningTTTScriptPart : AbstractTTTScriptPart {

	float nextTime = 0;
	int currentPart = 0;
	public override void startPart() {
		gameObject.transform.Find ("Intro").Find ("IntroDrum").GetComponent<SampleSequencer> ().enabled = true;
		gameObject.transform.Find ("Intro").Find ("IntroLead").GetComponent<HelmSequencer> ().StartOnNextCycle();
		gameObject.transform.Find ("Intro").Find ("IntroBass").GetComponent<HelmSequencer> ().StartOnNextCycle();
		Debug.Log ("Test beginning start");
		currentPart = 1;
		partOne ();
	}
	public override void stopPart() {
		gameObject.transform.Find("Intro").Find("IntroLead").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Intro").Find ("IntroBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Intro").Find ("IntroDrum").GetComponent<SampleSequencer> ().enabled = false;
	}
	void Update() {
		if (currentPart == 1 && nextTime <= Time.time) {
			partTwo ();
			currentPart = 2;
		} else if (currentPart == 2 && nextTime <= Time.time) {
			SendClearTargets ();
			SendEndScriptPart ();
		}
	}
	public override void targetSuccess() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Success 2") as AudioClip);
		SendNewTarget (TouchState.None, 50, 1);
	}
	public override void targetFailure() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Fail 2") as AudioClip);
		SendNewTarget (TouchState.None,70, 1);
	}
	void partOne() {
		nextTime = Time.time + 40;
		List<AudioClip> clips = new List<AudioClip>() { Resources.Load ("TouchTouchTransmission/dialog/Beginning Transmiss 1") as AudioClip 
		};
		SendPlayVoices (clips);
		SendNewTarget (TouchState.None, 70, 1);
	}
	void partTwo() {
		List<AudioClip> clips = new List<AudioClip>() { Resources.Load ("TouchTouchTransmission/dialog/Tranmiss Insuff") as AudioClip,
			Resources.Load ("TouchTouchTransmission/dialog/Engage Proto Inc Through") as AudioClip
		};
		SendPlayVoices (clips);
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
}