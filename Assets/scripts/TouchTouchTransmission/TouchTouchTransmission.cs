using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum TouchState { None=0, OneTwo, TwoThree, OneThree, AllConnected };
public class TouchTouchTransmission : AbstractManager {
	public AbstractTTTScript script;
	public AudioMixerGroup speechGroup;
	TouchState target;
	TouchState last_target = TouchState.None;
	static float DELAY_BETWEEN_CLIPS = 0.1f;
	int score = 0;
	int TO_WIN = 50;
	float nextTime = 0;
	bool hasBeenCleared = false;
	public AudioClip Sound_Win, Sound_Success, Sound_Fail;
	float last_menu_secret = 0.0f;
	public override void MenuSecretCiphers() {
		if (last_menu_secret + 1 > Time.time) {
			MenuManager.Menu ();
		}
		last_menu_secret = Time.time;
	}

	public static List<AudioClip> binaryToClips(int number) {
		List<AudioClip> lst = new List<AudioClip>();
		if (number < 0 || number > 255) {
			// pass
		} else {
			int pow = 128;
			while (pow >= 1) {
				if ((number / pow) % 2 == 1) {
					lst.Add (Resources.Load ("TouchTouchTransmission/dialog/1 ") as AudioClip);
				} else {
					lst.Add (Resources.Load ("TouchTouchTransmission/dialog/0 ") as AudioClip);
				}
				pow /= 2;
			}
		}
		return lst;
	}
	public static List<AudioClip> numberToClips(int number) {
		List<AudioClip> lst = new List<AudioClip> ();
		if (number < 0 || number > 199) {
			// pass
		} else if (number > 100) {
			lst = numberToClips (100);
			lst.AddRange(numberToClips (number - 100));
		} else if (number > 20 && number < 99 && !((number % 10) == 0)) {
			lst = numberToClips (10 * (number / 10));
			lst.AddRange(numberToClips (number % 10));
		} else {
			if (number == 0) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/0 ") as AudioClip);
			} else if (number == 1) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/1 ") as AudioClip);
			} else if (number == 2) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/2 ") as AudioClip);
			} else if (number == 3) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/3 ") as AudioClip);
			} else if (number == 4) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/4 ") as AudioClip);
			} else if (number == 5) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/5 ") as AudioClip);
			} else if (number == 6) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/6 ") as AudioClip);
			} else if (number == 7) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/7 ") as AudioClip);
			} else if (number == 8) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/8 ") as AudioClip);
			} else if (number == 9) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/9 ") as AudioClip);
			} else if (number == 10) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/10 ") as AudioClip);
			} else if (number == 11) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/11 ") as AudioClip);
			} else if (number == 12) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/12 ") as AudioClip);
			} else if (number == 13) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/13 ") as AudioClip);
			} else if (number == 14) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/14 ") as AudioClip);
			} else if (number == 15) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/15 ") as AudioClip);
			} else if (number == 16) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/16 ") as AudioClip);
			} else if (number == 17) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/17 ") as AudioClip);
			} else if (number == 18) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/18 ") as AudioClip);
			} else if (number == 19) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/19 ") as AudioClip);
			} else if (number == 20) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/20 ") as AudioClip);
			} else if (number == 30) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/30 ") as AudioClip);
			} else if (number == 40) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/40 ") as AudioClip);
			} else if (number == 50) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/50 ") as AudioClip);
			} else if (number == 60) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/60 ") as AudioClip);
			} else if (number == 70) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/70 ") as AudioClip);
			} else if (number == 80) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/80 ") as AudioClip);
			} else if (number == 90) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/90 ") as AudioClip);
			} else if (number == 100) {
				lst.Add (Resources.Load ("TouchTouchTransmission/dialog/100 ") as AudioClip);
			}    
		}
		return lst;
	}
	public static float getTotalTimeToPlay(List<AudioClip> clips) {
		float time = -DELAY_BETWEEN_CLIPS;
		foreach (AudioClip c in clips) {
			if (c != null) {
				time += c.length + DELAY_BETWEEN_CLIPS;
			}
		}
		return time;
	}
	void OnEnable() {
		base.OnEnable ();
		AbstractTTTScriptPart.OnTerminate += terminate;
		AbstractTTTScriptPart.OnUpdateScore += provideScoreUpdate;
		AbstractTTTScriptPart.OnPlayVoices += playBotVoices;
		AbstractTTTScriptPart.OnPlayVoice += playBotVoice;
		AbstractTTTScriptPart.OnNewTarget += newTarget;
		AbstractTTTScriptPart.OnClearTargets += clearTargets;
		AbstractTTTScriptPart.OnPlayGameSound += playGameSound;
		AbstractTTTScriptPart.OnEndScriptPart += endScriptPart;
	}
	void OnDisable() {
		base.OnDisable();
		AbstractTTTScriptPart.OnTerminate += terminate;
		AbstractTTTScriptPart.OnUpdateScore += provideScoreUpdate;
		AbstractTTTScriptPart.OnPlayVoices -= playBotVoices;
		AbstractTTTScriptPart.OnPlayVoice -= playBotVoice;
		AbstractTTTScriptPart.OnNewTarget -= newTarget;
		AbstractTTTScriptPart.OnClearTargets -= clearTargets;
		AbstractTTTScriptPart.OnPlayGameSound -= playGameSound;
		AbstractTTTScriptPart.OnEndScriptPart -= endScriptPart;

	}
	void terminate() {
		Debug.Log ("TERMINATE HIT");
		MenuManager.Menu ();
	}
	void Update() {
		if (target != TouchState.None && nextTime <= Time.time) {
			fail ();
			target = TouchState.None;
		}
	}
	public void provideScoreUpdate() {
		script.updateScore (new QuickTuple<int, int> (score, TO_WIN));
	}
	public void clearTargets() {
		// This needs to clear ones that are in the addTarget queue too
		target = TouchState.None;
		gameObject.transform.Find("TargetText").GetComponent<TextMesh>().text = "Target: "+target;
		lightUp (target, 0);
		hasBeenCleared = true;
	}
	public void endScriptPart() {
		script.startNextScript ();
		provideScoreUpdate ();
	}
	public void playBotVoice(AudioClip clip) {
		AudioSource botSpeaker = gameObject.transform.Find ("BotSpeaker").GetComponent<AudioSource> ();
		botSpeaker.clip = clip;
		botSpeaker.Play ();
	}
	public void playBotVoices(List<AudioClip> clips) {
		GameObject botSpeaker = gameObject.transform.Find ("BotSpeaker").gameObject;
		float time = (float) AudioSettings.dspTime; 
		foreach (AudioClip c in clips) {
			if (c == null) { 
				Debug.Log ("ERROR! Clip is null");
			} else {
				AudioSource newSpeaker = botSpeaker.AddComponent<AudioSource> ();
				newSpeaker.outputAudioMixerGroup = speechGroup;
				newSpeaker.clip = c;
				newSpeaker.PlayScheduled (time);
				time += c.length + DELAY_BETWEEN_CLIPS;
			}
		}
	}
	public void playGameSound(AudioClip clip) {
		AudioSource gameSpeaker = gameObject.transform.Find ("GameSpeaker").GetComponent<AudioSource> ();
		gameSpeaker.clip = clip;
		gameSpeaker.Play ();
	}
	public void newTarget(TouchState new_target, int duration, float pause) {
		clearTargets ();
		hasBeenCleared = false;
		//Pause goes here

		StartCoroutine(targetAddWrapper(new_target, duration,pause)); // I hate I have to do this and not invoke, but...
	}
	IEnumerator targetAddWrapper(TouchState new_target, int duration, float pause) {
		yield return new WaitForSeconds(pause);

		addNewTarget (new_target, duration);
	}
	public void addNewTarget(TouchState new_target, int duration) {
		if (!hasBeenCleared) {
			if (new_target == TouchState.None) {
				while (new_target == target || new_target == TouchState.None || new_target == last_target) {
					new_target = (TouchState)(Random.Range (1, 5));
				}
			}
			target = new_target;
			nextTime = Time.time + (duration / 10);
			lightUp (target, duration);
			gameObject.transform.Find ("TargetText").GetComponent<TextMesh> ().text = "Target: " + target;
		}
		hasBeenCleared = false;	
	}

	void Start() {
		score = 0;
		TurnOffLEDOne ();
		TurnOffLEDTwo ();
		TurnOffLEDThree ();
		script = gameObject.AddComponent<TestTTTScript>();
		script.startCurrentScript();

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
		last_target = target;
		clearTargets ();
		script.targetSuccess ();
		score += 1;
		provideScoreUpdate ();
	}

	void fail() {
		last_target = target;
		clearTargets ();
		script.targetFailure ();
		provideScoreUpdate ();
	}

	void checkMatch(TouchState input) {
		if (!enabled)
			return;

		if (input == target) {
			success ();
			//newTarget ();

		} else if (target == TouchState.AllConnected || target == TouchState.None) {
			// pass. Reason why is because otherwise getting to a three-touch would cause a fail noise.
			// Or if theyre touching while there's no active one
		} else {
			fail ();
		}

	//if (score >= TO_WIN) {
	//		enabled = false;
	//	}
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

	public override void BoxOneStartMoving(double speed) {
		script.BoxOneStartMoving (speed);
	}
	public override void BoxOneContinueMoving(double speed) {
		script.BoxOneContinueMoving (speed);
	}
	public override void BoxOneStopMoving() {
		script.BoxOneStopMoving ();
	}

	public override void BoxTwoStartMoving(double speed) {
		script.BoxTwoStartMoving (speed);
	}
	public override void BoxTwoContinueMoving(double speed) {
		script.BoxTwoContinueMoving (speed);
	}
	public override void BoxTwoStopMoving() {
		script.BoxTwoStopMoving ();
	}

	public override void BoxThreeStartMoving(double speed) {
		script.BoxThreeStartMoving (speed);
	}
	public override void BoxThreeContinueMoving(double speed) {
		script.BoxThreeContinueMoving (speed);
	}
	public override void BoxThreeStopMoving() {
		script.BoxThreeStopMoving ();
	}
}
