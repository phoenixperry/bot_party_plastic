using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMidiFile : MonoBehaviour {

    public AudioHelm.MidiFile file; 

	// Use this for initialization
	void Start () {
     file.LoadMidiData("/AudioHelm/Midi/midifiles-1.mid"); 
	}
	
	// Update is called once per frame
	void Update () {
      int len = file.midiData.length; 

        Debug.Log("The midi file is " + len + " notes long"); 
    
	}

   
}
