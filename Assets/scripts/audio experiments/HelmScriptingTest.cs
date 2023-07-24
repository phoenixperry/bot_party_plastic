using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; //this gives us mixer access 

public class HelmScriptingTest : MonoBehaviour {

    public AudioMixer helmMixer; 

    //you have to have all events in OnEnable and OnDisable. This is CRITICAL. Events will just hang around and if a game object calls an event that is no longer there because the object it is on has been deleted but wasn't removed in an OnDisable function beforehand, the game can crash out. 
    public void OnEnable()
    {
        //this is how to get a component acctached to the same game object as this script. I found that i could do this in the scripting referrence for HelmSequencer. A few more events are ALlNotesOff, NoteOff, NoteOn, StartOnNextCycle. See reference for more infos. http://tytel.org/audiohelm/scripting/class_audio_helm_1_1_helm_sequencer.html
        GetComponent<AudioHelm.Sequencer>().OnBeat += ChangeColor;
    }

    public void OnDisable()
    {
        GetComponent<AudioHelm.Sequencer>().OnBeat -= ChangeColor;
    }
   
    //here's what it looks like when you subscribe a function to an event.   
    public void ChangeColor(int note) {

        //this line of code simply ties the main camera's background color to the incoming note of the sequence 
        Camera.main.backgroundColor = new Color(note/15.0f, 0.0f, note/15.0f);
        Debug.Log("i'm firing on the beat and the note was "+note);

        /* ok to pull of changing a patch while the game is running you need to watch this: https://unity3d.com/learn/tutorials/topics/audio/exposed-audiomixer-parameters (you can ignore the part where they map it to on screen UI)
        There's stuff to set up in the editor. You need to go into the patch on the master channel and expose it's parameter. Then you need to rename it's parameter. You could call it cat butt but for sanity here I just named it what it was */
        helmMixer.SetFloat("resonance", note/15.0f);//here's setting the resonace based on note 
        helmMixer.SetFloat("osc1waveform" ,note); //heres messing with the osc waveform based on the note 

        float val = 0.0f;

        helmMixer.GetFloat("resonance", out val); //if you just want to know how to get what the parameter is, maybe to save it for later, here's how to do it? 

        Debug.Log("Watch the resonance change: " + val);

        //regular old Unity effects 
        helmMixer.SetFloat("flangeRate", note); //here's how to just make a general old flange effect native to Unity change I have added to helm :D 
    }




    public void Start()
    {
        
   
    }
    public void Update()
    {

    }



}
