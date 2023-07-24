using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioHelm;

public class FreePlayManager : AbstractManager {
	public ArrayList touchsound = new ArrayList();
	public AudioClip onetwo, twothree, onethree, allthree;
	public GameObject btnInterface;
	TestMarkovMusic markov_piano;

	void Start() {
		TextAsset noteasy = Resources.Load("MarkovFiles/NotEasyBeingGreen") as TextAsset;
		markov_piano = new TestMarkovMusic (noteasy.text);
		touchsound.Add (onetwo);
		touchsound.Add (twothree);
		touchsound.Add (onethree);
		touchsound.Add (allthree);
		TurnOffLEDOne ();
		TurnOffLEDTwo ();
		TurnOffLEDThree ();
		TurnOnLEDOne ();
		TurnOnLEDTwo ();
		TurnOnLEDThree ();
	}
	public override void BoxOneTwoConnected() {
		if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [0] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}
	public override void BoxTwoThreeConnected() {
		if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [1] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}

	public override void BoxOneThreeConnected() {
		if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [2] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}

	public override void AllConnected() {
		//if (!gameObject.GetComponent<AudioSource> ().isPlaying) {
			gameObject.GetComponent<AudioSource> ().clip = touchsound [3] as AudioClip;
			gameObject.GetComponent<AudioSource> ().Play ();
		//}
	}

	public override void BoxOneButtonDown ()
	{
		SetLEDOne (128);
		Button button = btnInterface.transform.Find ("botBtn1").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
		button.onClick.Invoke ();
	}
	public override void BoxOneButtonUp ()
	{
		TurnOnLEDOne ();
		Button button = btnInterface.transform.Find ("botBtn1").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;
		button.CancelInvoke ();
	}

	public override void BoxTwoButtonDown ()
	{
		SetLEDTwo (128);
		Button button = btnInterface.transform.Find ("botBtn2").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
		button.onClick.Invoke ();
	}
	public override void BoxTwoButtonUp ()
	{
		TurnOnLEDTwo ();
		Button button = btnInterface.transform.Find ("botBtn2").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;
		button.CancelInvoke ();
	}

	public override void BoxThreeButtonDown ()
	{
		SetLEDThree (128);
		Button button = btnInterface.transform.Find ("botBtn3").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.red;
		button.colors = cb;
		button.onClick.Invoke ();
	}
	public override void BoxThreeButtonUp ()
	{
		TurnOnLEDThree ();
		Button button = btnInterface.transform.Find ("botBtn3").GetComponent<Button> ();
		ColorBlock cb = button.colors;
		cb.normalColor = Color.green;
		button.colors = cb;
		button.CancelInvoke ();
	}

	public override void BoxOneStartMoving(double speed)
	{
		gameObject.transform.Find ("Move1").GetComponent<TextMesh> ().text = "1: Moving";
		GameObject midibot = gameObject.transform.Find ("Bot1Midi").gameObject;
		midibot.GetComponent<AudioHelmClock> ().Reset ();
		midibot.GetComponent<HelmSequencer> ().enabled = true;
		HelmSequencer seq = midibot.GetComponent<HelmSequencer> ();
		markov_piano.fillSequencer (seq);
		midibot.GetComponent<AudioSource> ().Play ();
	}

	public override void BoxOneContinueMoving(double speed)
	{
		gameObject.transform.Find ("Move1").GetComponent<TextMesh> ().text = "1: "+speed;
		HelmController control = gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController>();
		control.SetParameterPercent(Param.kFilterCutoff,(float) (0.4+(speed/15)*0.6));
		control.SetParameterPercent (Param.kMonoLfo1Amplitude,(float) (0.5 + (speed / 15) * 0.5));
		control.SetParameterPercent (Param.kMonoLfo1Frequency, (float)(0.4 + (speed / 15) * 0.6));
		control.SetParameterPercent(Param.kDelayFrequency, (float)(0.5 + (speed / 15) * 0.5));
		control.SetParameterPercent (Param.kDelayDryWet, (float)((speed / 15) * 0.5));
	}

	public override void BoxOneStopMoving()
	{
		GameObject midibot = gameObject.transform.Find ("Bot1Midi").gameObject;
		midibot.GetComponent<AudioSource> ().Stop ();
		midibot.GetComponent<HelmSequencer> ().Clear ();
		midibot.GetComponent<HelmSequencer> ().currentIndex = -1;
		midibot.GetComponent<HelmSequencer> ().enabled = false;
		gameObject.transform.Find ("Move1").GetComponent<TextMesh> ().text = "1: At rest";
	}

	public override void BoxTwoStartMoving(double speed)
	{
		gameObject.transform.Find ("Move2").GetComponent<TextMesh> ().text = "2: Moving";
		BoxTwoContinueMoving (speed);
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmSequencer> ().enabled = true;

	}

	public override void BoxTwoContinueMoving(double speed) {

		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmSequencer> ().enabled = true;
		HelmController control = gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController>();
		control.SetParameterPercent (Param.kFilterCutoff, (float)(0.4 + (speed / 15) * 0.4));
		control.SetParameterPercent (Param.kReverbDryWet,(float)((speed / 15) * 0.6));
	}

	public override void BoxTwoStopMoving()
	{
		gameObject.transform.Find ("Move2").GetComponent<TextMesh> ().text = "2: At rest";
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmSequencer> ().enabled = false;
	}

	public override void BoxThreeStartMoving(double speed)
	{
		BoxThreeContinueMoving (speed);
		gameObject.transform.Find ("Move3").GetComponent<TextMesh> ().text = "3: Moving";
	}

	public override void BoxThreeContinueMoving(double speed)
	{
		gameObject.transform.Find ("Bot3Midi").GetComponent<HelmController> ().AllNotesOff ();
		// Note 72 = C5.
		gameObject.transform.Find ("Bot3Midi").GetComponent<HelmController> ().NoteOn ((int)(72 + Mathf.Max(0f,(float)(speed-5))*2));
	}
	public override void BoxThreeStopMoving()
	{
		gameObject.transform.Find ("Move3").GetComponent<TextMesh> ().text = "3: At rest";
		gameObject.transform.Find ("Bot3Midi").GetComponent<HelmController> ().AllNotesOff ();
	}

	public override void BoxOneStartRotating(double angular_speed) {
		gameObject.transform.Find ("Rotate1").GetComponent<TextMesh> ().text = "1: Spinning!";
		BoxOneContinueRotating (angular_speed);
	}
	public override void BoxOneContinueRotating (double angular_speed)
	{
		gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, (float)angular_speed / 40);
	}
	public override void BoxOneStopRotating() {
		gameObject.transform.Find ("Rotate1").GetComponent<TextMesh> ().text = "1: Not Spinning!";
		gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, 0);
	}

	public override void BoxTwoStartRotating(double angular_speed) {
		gameObject.transform.Find ("Rotate2").GetComponent<TextMesh> ().text = "2: Spinning!";
	}
	public override void BoxTwoContinueRotating (double angular_speed)
	{
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, (float)angular_speed / 40);

	}
	public override void BoxTwoStopRotating() {
		gameObject.transform.Find ("Rotate2").GetComponent<TextMesh> ().text = "2: Not Spinning!";
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, 0);
	}

	public override void BoxThreeStartRotating(double angular_speed) {
		gameObject.transform.Find ("Rotate3").GetComponent<TextMesh> ().text = "3: Spinning!";
	}
	public override void BoxThreeContinueRotating (double angular_speed)
	{
		gameObject.transform.Find ("Bot3Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, (float)angular_speed / 40);
	}
	public override void BoxThreeStopRotating() {
		gameObject.transform.Find ("Rotate3").GetComponent<TextMesh> ().text = "3: Not Spinning!";
		gameObject.transform.Find ("Bot3Midi").GetComponent<HelmController> ().SetParameterPercent (Param.kResonance, 0);
	}
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
