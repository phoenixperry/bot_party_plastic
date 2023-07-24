using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;
public class TestIntermediateTTTScriptPart : AbstractTTTScriptPart {

	float nextTime = 0;
	int currentPart = 0;
	public override void startPart() {
		gameObject.transform.Find("Inter").Find("InterLead").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Inter").Find ("InterBass").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Inter").Find ("InterDrum").GetComponent<SampleSequencer> ().enabled = true;
		Debug.Log ("Test intermediate start");
		currentPart = 1;
		partOne ();
	}
	public override void stopPart() {
		gameObject.transform.Find("Inter").Find("InterLead").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Inter").Find ("InterBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Inter").Find ("InterDrum").GetComponent<SampleSequencer> ().enabled = false;
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
		SendNewTarget (TouchState.None, 40, 1);
	}
	public override void targetFailure() {
		SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Fail 2") as AudioClip);
		SendNewTarget (TouchState.None,50, 1);
	}
	void partOne() {
		nextTime = Time.time + 35;
		SendNewTarget (TouchState.None, 50, 1);
	}
	void partTwo() {
		List<AudioClip> clips = new List<AudioClip>() { Resources.Load ("TouchTouchTransmission/dialog/Tranmiss Op Engage Accel") as AudioClip 
		};
		SendPlayVoices (clips);
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
}