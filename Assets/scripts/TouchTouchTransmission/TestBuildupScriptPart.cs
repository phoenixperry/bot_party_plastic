using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;

public class TestBuildupTTTScriptPart : AbstractTTTScriptPart {
	double box1speed = 0;
	double box2speed = 0;
	double box3speed = 0;
	float nextTime = 0;
	int currentPart = 0;
	public override void startPart() {
		gameObject.transform.Find("Resol").Find ("ResolBass").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Resol").Find ("ResolLead").GetComponent<HelmSequencer> ().enabled = true;
		gameObject.transform.Find("Resol").Find ("ResolDrum").GetComponent<SampleSequencer> ().enabled = true;
		Debug.Log ("Test buildup start");
		currentPart = 1;
		partOne ();
	}
	public override void stopPart() {
		gameObject.transform.Find("Resol").Find ("ResolBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Resol").Find ("ResolLead").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Resol").Find ("ResolDrum").GetComponent<SampleSequencer> ().enabled = false;

	}
	void Update() {
		if (currentPart == 1 && nextTime <= Time.time) {
			partTwo ();
			currentPart = 2;
		} else if (currentPart == 2 && nextTime <= Time.time) {
			currentPart = 3;
			partThree ();
		} else if (currentPart == 4 && nextTime <= Time.time) {
			partFive ();
		}
	}
	public override void targetSuccess() {
		if (currentPart == 1) {
			SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Success 2") as AudioClip);
			SendNewTarget (TouchState.None, (int)Mathf.Floor(35f*getTimeReduction()), 0.6f * getTimeReduction());		} 
		else if (currentPart == 3) {
			currentPart = 4;
			partFour ();
		}
	}
	public override void targetFailure() {
		if (currentPart == 1) {
			SendPlayGameSound (Resources.Load ("TouchTouchTransmission/gamesounds/Fail 2") as AudioClip);
			SendNewTarget (TouchState.None, (int)Mathf.Floor (45f * getTimeReduction ()), 0.8f * getTimeReduction ());
		} else if (currentPart == 3) {
			currentPart = 4;
			partFour ();
		}
	}
	public float getTimeReduction() {
		double speed = (box1speed + box2speed + box3speed) / 3;
		if (speed == 0) {
			return 1;
		}
		return (float) (1 - (speed / 20));
	}
	void partOne() {
		nextTime = Time.time + 60;
		//SendPlayVoice(Resources.Load ("TouchTouchTransmission/capage-drafts/test-buildup-start") as AudioClip);
		SendNewTarget (TouchState.None, 70, 1);
	}
	void partTwo() {
		SendClearTargets ();
		List<AudioClip> clips = new List<AudioClip>() { Resources.Load ("TouchTouchTransmission/dialog/Transmiss Compl") as AudioClip,
			Resources.Load ("TouchTouchTransmission/dialog/Just One More TIme HUm") as AudioClip
		};
		SendPlayVoices (clips);
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
	void partThree() {
		SendNewTarget (TouchState.AllConnected, 255, 0.5f);
	}
	void partFour() {
		List<AudioClip> clips = new List<AudioClip> () {
			Resources.Load ("TouchTouchTransmission/gamesounds/fanfare v2") as AudioClip,
			Resources.Load ("TouchTouchTransmission/dialog/You Broadcast") as AudioClip,
		};
		clips.AddRange(TouchTouchTransmission.numberToClips (getScore()));
		clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Gigabytes 1") as AudioClip);
		clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Translation 1") as AudioClip);
		clips.AddRange(TouchTouchTransmission.numberToClips (getScore()));
		clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Points") as AudioClip);
		if (getScore () < getScoreWin ()) {
			clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Bitrate Low 1") as AudioClip);
			clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Recon Hum Int Train Proto") as AudioClip);

		} else if (getScore () > getScoreWin () * 1.5) {
			clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Hu Op Efficiency") as AudioClip);
		} else {
			clips.Add (Resources.Load ("TouchTouchTransmission/dialog/Hu Perf Adeq 1") as AudioClip);
		}
		clips.Add (Resources.Load ("Menu/capage-drafts/Reseting System") as AudioClip);
		gameObject.transform.Find("Resol").Find ("ResolBass").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Resol").Find ("ResolLead").GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find("Resol").Find ("ResolDrum").GetComponent<SampleSequencer> ().enabled = false;
		BoxOneStopMoving ();
		BoxTwoStopMoving ();
		BoxThreeStopMoving ();
		SendPlayVoices(clips);
		SendClearTargets ();
		nextTime = Time.time + TouchTouchTransmission.getTotalTimeToPlay (clips);
	}
	void partFive() {
		SendTerminate();
	}

	public override void BoxOneStartMoving(double speed) {
		if (currentPart < 4) {
			gameObject.transform.Find ("Motion").Find ("Motion1").GetComponent<HelmSequencer> ().enabled = true;
			BoxOneContinueMoving (speed);
		}
	}
	public override void BoxOneContinueMoving(double speed) {
		if (currentPart < 4) {
			box1speed = speed;
			HelmController control = gameObject.transform.Find ("Motion").Find ("Motion1").GetComponent<HelmController>();
			control.SetParameterPercent(Param.kFilterCutoff,(float) (0.4+(speed/15)*0.6));
			control.SetParameterPercent (Param.kMonoLfo1Amplitude,(float) (0.5 + (speed / 15) * 0.5));
			control.SetParameterPercent (Param.kMonoLfo1Frequency, (float)(0.4 + (speed / 15) * 0.6));
			control.SetParameterPercent(Param.kDelayFrequency, (float)(0.5 + (speed / 15) * 0.5));
			control.SetParameterPercent (Param.kDelayDryWet, (float)((speed / 15) * 0.5));
		}
	}
	public override void BoxOneStopMoving() {
		gameObject.transform.Find ("Motion").Find ("Motion1").GetComponent<HelmSequencer> ().enabled = false;
		box1speed = 0;
	}

	public override void BoxTwoStartMoving(double speed) {
		if (currentPart < 4) {
			gameObject.transform.Find ("Motion").Find ("Motion2").GetComponent<HelmSequencer> ().enabled = true;
			BoxTwoContinueMoving (speed);

		}
	}
	public override void BoxTwoContinueMoving(double speed) {
		if (currentPart < 4) {
			box2speed = speed;
			HelmController control = gameObject.transform.Find ("Motion").Find ("Motion2").GetComponent<HelmController>();
			control.SetParameterPercent (Param.kFilterCutoff, (float)(0.4 + (speed / 15) * 0.4));
			control.SetParameterPercent (Param.kReverbDryWet,(float)((speed / 15) * 0.6));
		}
	}
	public override void BoxTwoStopMoving() {
		gameObject.transform.Find ("Motion").Find ("Motion2").GetComponent<HelmSequencer> ().enabled = false;
		box2speed = 0;
	}


	public override void BoxThreeStartMoving(double speed) {
		if (currentPart < 4) {
			gameObject.transform.Find ("Motion").Find ("Motion3").GetComponent<SampleSequencer> ().enabled = true;
			BoxThreeContinueMoving (speed);
		}
	}
	public override void BoxThreeContinueMoving(double speed) {
		if (currentPart < 4) {
			box3speed = speed;
		}
	}
	public override void BoxThreeStopMoving() {
		gameObject.transform.Find ("Motion").Find ("Motion3").GetComponent<SampleSequencer> ().enabled = false;
		box3speed = 0;
	}
}