using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractTTTScript : MonoBehaviour {

	public List<AbstractTTTScriptPart> scriptParts;
	int currentPart = 0;
	public void updateScore(QuickTuple<int, int> score) {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].updateScore (score);
		}
	}
	public void targetSuccess() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].targetSuccess ();
		}
	}
	public void targetFailure() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].targetFailure ();
		}
	}
	public void targetCleared() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].targetCleared ();
		}
	}
	public void BoxOneStartMoving(double speed) {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].BoxOneStartMoving (speed);
		}
	}
	public void BoxOneContinueMoving(double speed) {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].BoxOneContinueMoving (speed);
		}
	}	
	public void BoxOneStopMoving() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].BoxOneStopMoving ();
		}
	}
	public void BoxTwoStartMoving(double speed) {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].BoxTwoStartMoving (speed);
		}
	}
	public void BoxTwoContinueMoving(double speed) {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].BoxTwoContinueMoving (speed);
		}
	}	
	public void BoxTwoStopMoving() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].BoxTwoStopMoving ();
		}
	}
	public void BoxThreeStartMoving(double speed) {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].BoxThreeStartMoving (speed);
		}
	}
	public void BoxThreeContinueMoving(double speed) {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].BoxThreeContinueMoving (speed);
		}
	}	
	public void BoxThreeStopMoving() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].BoxThreeStopMoving ();
		}
	}

	public void startNextScript() {
		stopCurrentScript ();
		currentPart += 1;
		startCurrentScript ();
	}
	public void stopCurrentScript() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].stopPart ();
			Destroy(scriptParts [currentPart]);
		}
	}
	public void startCurrentScript() {
		if (currentPart < scriptParts.Count) {
			scriptParts [currentPart].startPart ();
		}
	}
}
