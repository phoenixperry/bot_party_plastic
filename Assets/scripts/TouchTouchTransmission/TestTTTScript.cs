using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTTTScript : AbstractTTTScript {


	void OnEnable() {
		scriptParts = new List<AbstractTTTScriptPart> ();
		scriptParts.Add (gameObject.AddComponent<TrainingTTTScriptPart>());
		scriptParts.Add (gameObject.AddComponent<TestBeginningTTTScriptPart> ());
		scriptParts.Add (gameObject.AddComponent<TestIntermediateTTTScriptPart> ());
		scriptParts.Add (gameObject.AddComponent<BridgeTTTScriptPart> ());
		scriptParts.Add (gameObject.AddComponent<TestBuildupTTTScriptPart> ());

	}
		
}
