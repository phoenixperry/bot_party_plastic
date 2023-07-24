using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioHelm;

public class MusicMakerManager : AbstractManager {
	int[] bot1_notes = { 48, 50, 52, 53, 55, 57, 59 };
	int[] bot2_notes = { 60, 62, 64, 65, 67, 69, 71};
	int[] bot3_notes = { 72, 74, 76, 77, 79, 81, 83 };

	void Start() {
	}

	public override void BoxOneButtonDown ()
	{
		gameObject.transform.Find ("Drums").GetComponent<HelmController> ().NoteOn (48);
	}
	public override void BoxOneButtonUp ()
	{
		gameObject.transform.Find ("Drums").GetComponent<HelmController> ().NoteOff (48);
	}

	public override void BoxTwoButtonDown ()
	{
		gameObject.transform.Find ("Drums").GetComponent<HelmController> ().NoteOn (49);
	}
	public override void BoxTwoButtonUp ()
	{
		gameObject.transform.Find ("Drums").GetComponent<HelmController> ().NoteOff (49);

	}

	public override void BoxThreeButtonDown ()
	{
		gameObject.transform.Find ("Drums").GetComponent<HelmController> ().NoteOn (50);
	}
	public override void BoxThreeButtonUp ()
	{
		gameObject.transform.Find ("Drums").GetComponent<HelmController> ().NoteOff (50);

	}
	/*

	public override void BoxOneStartMoving(double speed)
	{
		BoxOneContinueMoving (speed);
	}
	public override void BoxOneContinueMoving(double speed)
	{
		// So this plays notes proportional to speed of box one from the octave C3 to C4.
		//gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController> ().AllNotesOff ();
		// Note 48 = C3. 
		//gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController> ().NoteOn ((int)(48 + Mathf.Max(0f,(float)(speed-5))*2));
	}

	public override void BoxOneStopMoving()
	{
		//gameObject.transform.Find ("Bot1Midi").GetComponent<HelmController> ().AllNotesOff();
		//gameObject.transform.Find ("Move1").GetComponent<TextMesh> ().text = "1: At rest";
	}

	public override void BoxTwoStartMoving(double speed)
	{
		//gameObject.transform.Find ("Move2").GetComponent<TextMesh> ().text = "2: Moving";
		//BoxTwoContinueMoving (speed);

	}

	public override void BoxTwoContinueMoving(double speed) {
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController> ().AllNotesOff ();
		// Note 60 = C3
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController> ().NoteOn ((int)(60 + Mathf.Max(0f,(float)(speed-5))*2));
	}

	public override void BoxTwoStopMoving()
	{
		gameObject.transform.Find ("Move2").GetComponent<TextMesh> ().text = "2: At rest";
		gameObject.transform.Find ("Bot2Midi").GetComponent<HelmController> ().AllNotesOff ();
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
	}*/
}
