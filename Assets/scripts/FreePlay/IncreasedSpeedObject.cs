using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedSpeedObject : MonoBehaviour {

    public AudioHelm.Sequencer sequencer;
    public int increase = 3;
    public int startingNote = 20; 

    // Use this for initialization
    void Start () {
        sequencer.Clear();

        int length = sequencer.length;
        for (int i = 0; i < length-1; ++i)
        {
            sequencer.AddNote(startingNote + i * increase, i, i+1); 
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
}
