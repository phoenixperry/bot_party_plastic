using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractTTTScriptPart : MonoBehaviour {
	public delegate void EndScriptPart();
	public static event EndScriptPart OnEndScriptPart;

	public delegate void ClearTargets();
	public static event ClearTargets OnClearTargets;

	public delegate void NewTarget(TouchState target, int duration, float pause);
	public static event NewTarget OnNewTarget;

	public delegate void PlayVoice(AudioClip clip);
	public static event PlayVoice OnPlayVoice;
	public delegate void PlayVoices(List<AudioClip> clip);
	public static event PlayVoices OnPlayVoices;

	public delegate void PlayGameSound(AudioClip clip);
	public static event PlayGameSound OnPlayGameSound;

	public delegate void UpdateScore();
	public static event UpdateScore OnUpdateScore;

	public delegate void Terminate ();
	public static event Terminate OnTerminate;
	protected QuickTuple<int, int> score;
	public void updateScore(QuickTuple<int, int> newScore) {
		score = newScore;
	}
	protected int getScore() {
		if (score != null) {
			return score.first;	
		}
		return 0;
	}
	protected int getScoreWin() {
		if (score != null) {
			return score.last;
		}
		return 0;
	}
	protected void OnDisable() {
		SendClearTargets ();
	}
	protected void SendUpdateScore() {
		if (OnUpdateScore != null) {
			OnUpdateScore ();
		}
	}
	protected void SendTerminate() {
		if (OnTerminate != null) {
			OnTerminate ();
		}
	}
	protected void SendPlayGameSound(AudioClip clip) {
		if (OnPlayGameSound != null) {
			OnPlayGameSound (clip);
		}
	}

	protected void SendPlayVoice(AudioClip clip) {
		if (OnPlayVoice != null) {
			OnPlayVoice (clip);
		}
	}

	protected void SendPlayVoices(List<AudioClip> clips) {
		if (OnPlayVoices != null) {
			OnPlayVoices (clips);
		}
	}

	protected void SendEndScriptPart() {
		if (OnEndScriptPart != null) {
			OnEndScriptPart ();
		}
	}
	protected void SendClearTargets() {
		if (OnClearTargets != null) {
			OnClearTargets();
		}
	}
	protected void SendNewTarget(TouchState target, int duration, float pause) {
		if (OnNewTarget != null) {
			OnNewTarget (target, duration, pause);
		}
	}

	public virtual void startPart() {

	}
	public virtual void stopPart() {

	}
	public virtual void targetSuccess() {

	}
	public virtual void targetFailure() {

	}
	public virtual void targetCleared() {

	}

	public virtual void BoxOneStartMoving(double speed) {

	}
	public virtual void BoxOneContinueMoving(double speed) {

	}
	public virtual void BoxOneStopMoving() {

	}
	public virtual void BoxTwoStartMoving(double speed) {

	}
	public virtual void BoxTwoContinueMoving(double speed) {

	}
	public virtual void BoxTwoStopMoving() {

	}
	public virtual void BoxThreeStartMoving(double speed) {

	}
	public virtual void BoxThreeContinueMoving(double speed) {

	}
	public virtual void BoxThreeStopMoving() {

	}
}
