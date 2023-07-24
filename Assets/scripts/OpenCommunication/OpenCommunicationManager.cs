using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioHelm;
public enum TouchMusicSet { None=0, Animal=1, Greeting=2};
public class OpenCommunicationManager : AbstractManager {
	GameObject bot1_sound, bot2_sound, bot3_sound;
	AbstractMarkovMusic markov_piano;
	TouchMusicSet whichTouches;

	void Start() {
		TurnOnLEDOne ();
		TurnOnLEDTwo ();
		TurnOnLEDThree ();
		bot1_sound = gameObject.transform.Find ("Bots").Find ("Bot1").gameObject;
		bot2_sound = gameObject.transform.Find ("Bots").Find ("Bot2").gameObject;
		bot3_sound = gameObject.transform.Find ("Bots").Find ("Bot3").gameObject;
		TextAsset noteasy = Resources.Load("MarkovFiles/NotEasyBeingGreen") as TextAsset;
		markov_piano = new TestMarkovMusic (noteasy.text);
		whichTouches = TouchMusicSet.Animal;
		setTouchSound (whichTouches);

	}
	public void setTouchSound(TouchMusicSet which) {
		if (which == TouchMusicSet.None) {
			Debug.Log ("Error, touch music set not set.");
		} else if (which == TouchMusicSet.Animal) {
			gameObject.transform.Find ("Touches").Find ("Touch12").gameObject.GetComponent<AudioSource> ().clip = Resources.Load("OpenCommunication/TouchSounds/Animals/alpha_beta_touch-1") as AudioClip;
			gameObject.transform.Find ("Touches").Find ("Touch23").gameObject.GetComponent<AudioSource> ().clip = Resources.Load("OpenCommunication/TouchSounds/Animals/beta_gamma_touch-1") as AudioClip;
			gameObject.transform.Find ("Touches").Find ("Touch13").gameObject.GetComponent<AudioSource> ().clip = Resources.Load("OpenCommunication/TouchSounds/Animals/alpha_gamma_touch-1") as AudioClip;
		} else if (which == TouchMusicSet.Greeting) {
			gameObject.transform.Find ("Touches").Find ("Touch12").gameObject.GetComponent<AudioSource> ().clip = Resources.Load("OpenCommunication/TouchSounds/Greetings/full-bear-owl") as AudioClip;
			gameObject.transform.Find ("Touches").Find ("Touch23").gameObject.GetComponent<AudioSource> ().clip = Resources.Load("OpenCommunication/TouchSounds/Greetings/full-whimsy-owl") as AudioClip;
			gameObject.transform.Find ("Touches").Find ("Touch13").gameObject.GetComponent<AudioSource> ().clip = Resources.Load("OpenCommunication/TouchSounds/Greetings/full-bear-whimsy") as AudioClip;
		}
	}
	// Box touches
	public override void BoxOneTwoConnected() {
		AudioSource touching = gameObject.transform.Find ("Touches").Find ("Touch12").gameObject.GetComponent<AudioSource> ();
		if (!touching.isPlaying && !gameObject.transform.Find ("Bots").Find ("All").Find ("Touching").gameObject.GetComponent<AudioSource>().isPlaying) {
			touching.Play ();
		}
	}
	public override void BoxTwoThreeConnected() {
		AudioSource touching = gameObject.transform.Find ("Touches").Find ("Touch23").gameObject.GetComponent<AudioSource> ();
		if (!touching.isPlaying && !gameObject.transform.Find ("Bots").Find ("All").Find ("Touching").gameObject.GetComponent<AudioSource>().isPlaying) {
			touching.Play ();
		}
	}

	public override void BoxOneThreeConnected() {
		AudioSource touching = gameObject.transform.Find ("Touches").Find ("Touch13").gameObject.GetComponent<AudioSource> ();
		if (!touching.isPlaying && !gameObject.transform.Find ("Bots").Find ("All").Find ("Touching").gameObject.GetComponent<AudioSource>().isPlaying) {
			touching.Play ();
		}
	}

	public override void AllConnected() {
		AudioSource all_touching = gameObject.transform.Find ("Bots").Find ("All").Find ("Touching").gameObject.GetComponent<AudioSource>();
		if (!all_touching.isPlaying) {
			gameObject.transform.Find ("Touches").Find ("Touch12").gameObject.GetComponent<AudioSource> ().Stop ();
			gameObject.transform.Find ("Touches").Find ("Touch23").gameObject.GetComponent<AudioSource> ().Stop ();
			gameObject.transform.Find ("Touches").Find ("Touch13").gameObject.GetComponent<AudioSource> ().Stop ();
			all_touching.Play ();

			/*if (whichTouches == TouchMusicSet.Animal) {
				whichTouches = TouchMusicSet.Greeting;
			} else if (whichTouches == TouchMusicSet.Greeting) {
				whichTouches = TouchMusicSet.Animal;
			}
			setTouchSound (whichTouches);
			*/
		}
	}

	public override void AllReleased() {
		AudioSource all_touching = gameObject.transform.Find ("Bots").Find ("All").Find ("Touching").gameObject.GetComponent<AudioSource>();
		all_touching.Stop ();
	}

	// Bot buttons
	bool btn1, btn2, btn3 = false;
	public override void BoxOneButtonDown ()
	{
		SetLEDOne (128);

		AudioSource btn = gameObject.transform.Find ("Bots").Find ("Bot1").Find ("Button").GetComponent<AudioSource>();
		if (!btn.isPlaying) {
			btn.Play ();
		}

		btn1 = true;
		checkAllThreeButtons ();
	}
	public override void BoxOneButtonUp ()
	{
		TurnOnLEDOne ();

		btn1 = false;
		checkAllThreeButtons ();
	}

	public override void BoxTwoButtonDown ()
	{
		SetLEDTwo (128);

		AudioSource btn = gameObject.transform.Find ("Bots").Find ("Bot2").Find ("Button").GetComponent<AudioSource>();
		if (!btn.isPlaying) {
			btn.Play ();
		}

		btn2 = true;
		checkAllThreeButtons ();
	}
	public override void BoxTwoButtonUp ()
	{
		TurnOnLEDTwo ();

		btn2 = false;
		checkAllThreeButtons ();
	}

	public override void BoxThreeButtonDown ()
	{
		SetLEDThree (128);

		AudioSource btn = gameObject.transform.Find ("Bots").Find ("Bot3").Find ("Button").GetComponent<AudioSource>();
		if (!btn.isPlaying) {
			btn.Play ();
		}

		btn3 = true;
		checkAllThreeButtons ();
	}
	public override void BoxThreeButtonUp ()
	{
		TurnOnLEDThree ();

		btn3 = false;
		checkAllThreeButtons ();

	}

	void checkAllThreeButtons() {
		AudioSource btn = gameObject.transform.Find ("Bots").Find ("All").Find ("Buttons").gameObject.GetComponent<AudioSource>();
		if (btn1 && btn2 && btn3) {
			btn.Play ();
			gameObject.transform.Find ("Bots").Find ("Bot1").Find ("Button").GetComponent < AudioSource> ().Stop ();
			gameObject.transform.Find ("Bots").Find ("Bot2").Find ("Button").GetComponent < AudioSource> ().Stop ();
			gameObject.transform.Find ("Bots").Find ("Bot3").Find ("Button").GetComponent < AudioSource> ().Stop ();
		} else {
			btn.Stop ();
		}
	}

	// Box moving
	public override void BoxOneStartMoving(double speed)
	{
		HelmSequencer seq = bot1_sound.transform.Find("MotionSound").GetComponent<HelmSequencer> ();
		markov_piano.fillSequencer (seq);
		seq.enabled = true;
		//bot1_sound.transform.Find("MotionSound").GetComponent<AudioSource> ().Play ();
		BoxOneContinueMoving (speed);
	}
	int box1_last_note = 0;
	int box1_current_note = 0;
	int lastBeep = 0;
	public override void BoxOneContinueMoving(double speed)
	{
		HelmController control = bot1_sound.transform.Find("MotionSound").GetComponent<HelmController>();
		control.SetParameterPercent (Param.kFilterCutoff, (float)((speed / 15) * 0.4));
		control.SetParameterPercent (Param.kReverbDryWet,(float)((speed / 15) * 0.2
		));
		/*int newNote = markov_piano.getNextNote (box1_last_note, box1_current_note).first;
		int nToNextBeep = (int)Mathf.Max(1f,4 - Mathf.Max((int) ((speed-5)/1.33),0));
		if (lastBeep >= nToNextBeep) {
			control.AllNotesOff ();
			control.NoteOn (newNote);
			box1_last_note = box1_current_note;
			box1_current_note = newNote;
			lastBeep = 0;
		} else {
			lastBeep += 1;
		}*/
	}	

	public override void BoxOneStopMoving()
	{
		HelmController control = bot1_sound.transform.Find("MotionSound").GetComponent<HelmController>();
		HelmSequencer seq = bot1_sound.transform.Find("MotionSound").GetComponent<HelmSequencer> ();

		seq.enabled = false;
		//control.AllNotesOff();
	}
	// Sustain effect
	public override void BoxTwoStartMoving(double speed)
	{
		HelmSequencer seq = bot2_sound.transform.Find("MotionSound").GetComponent<HelmSequencer> ();
		seq.currentIndex = -1;
		AudioHelmClock clock = bot3_sound.transform.Find ("MotionSound").GetComponent<AudioHelmClock> ();
		clock.Reset ();
		seq.enabled = true;
		BoxTwoContinueMoving (speed);
	}

	int currentNoteOn = 0;
	public override void BoxTwoContinueMoving(double speed) {
		HelmController control = bot2_sound.transform.Find("MotionSound").GetComponent<HelmController>();
		/*control.SetParameterPercent(Param.kFilterCutoff,(float) (0.4+(speed/15)*0.6));
		control.SetParameterPercent (Param.kMonoLfo1Amplitude,(float) (0.5 + (speed / 15) * 0.5));
		control.SetParameterPercent (Param.kMonoLfo1Frequency, (float)(0.4 + (speed / 15) * 0.6));
		control.SetParameterPercent(Param.kDelayFrequency, (float)(0.5 + (speed / 15) * 0.5));
		control.SetParameterPercent (Param.kDelayDryWet, (float)((speed / 15) * 0.5));*/

		//int newNote = (int)(59 + speed/2);
		//if (newNote != currentNoteOn) {
		//		Debug.Log ("Playing note: " + newNote);
		//	ctrl.NoteOn (newNote);
		//	currentNoteOn = newNote;
		//}
	}

	public override void BoxTwoStopMoving()
	{
		HelmController ctrl = bot2_sound.transform.Find("MotionSound").GetComponent<HelmController>();
		ctrl.AllNotesOff ();
		currentNoteOn = 0;
		HelmSequencer seq = bot2_sound.transform.Find("MotionSound").GetComponent<HelmSequencer> ();
		seq.currentIndex = -1;
		seq.enabled = false;
	}

	public override void BoxThreeStartMoving(double speed)
	{
		SampleSequencer seq = bot3_sound.transform.Find ("MotionSound").GetComponent<SampleSequencer> ();
		seq.currentIndex = -1;
		seq.enabled = true;
		BoxThreeContinueMoving (speed);
	}

	public override void BoxThreeContinueMoving(double speed)
	{
		AudioHelmClock clock = bot3_sound.transform.Find ("MotionSound").GetComponent<AudioHelmClock> ();
		clock.bpm = 120 + (int)((speed - 5) * 5);
	}
	public override void BoxThreeStopMoving()
	{
		SampleSequencer seq = bot3_sound.transform.Find ("MotionSound").GetComponent<SampleSequencer> ();
		seq.enabled = false;
	}

	// Box rotations
	public override void BoxOneStartRotating(double angular_speed) {

	}
	public override void BoxOneContinueRotating (double angular_speed)
	{
		
	}

	public override void BoxOneStopRotating() {

	}

	public override void BoxTwoStartRotating(double angular_speed) {
		BoxTwoContinueRotating (angular_speed);
	}
	public override void BoxTwoContinueRotating (double angular_speed)
	{
		HelmController control = bot2_sound.transform.Find("MotionSound").GetComponent<HelmController>();
		control.SetPitchWheel ((float)(angular_speed/40));
	}
	public override void BoxTwoStopRotating() {
		HelmController control = bot2_sound.transform.Find("MotionSound").GetComponent<HelmController>();
		control.SetPitchWheel (0f);
	}

	public override void BoxThreeStartRotating(double angular_speed) {
	
	}
	public override void BoxThreeContinueRotating (double angular_speed)
	{
	
	}
	public override void BoxThreeStopRotating() {
;
	}



	// Menu Management things
	float last_menu_freeplay = 0.0f;
	public override void MenuSecretCiphers() {
		MenuManager.TouchTouchRevolution ();
	}
	public override void MenuFreePlay() {
		if (last_menu_freeplay + 1 > Time.time) {
			MenuManager.Menu ();
		}
		last_menu_freeplay = Time.time;
	}
}
