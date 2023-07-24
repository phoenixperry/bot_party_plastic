using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 

public class SynthGenerator : MonoBehaviour {

    public double frequency = 440.0;
    private double increment;
    private double phase;
    private double sampling_frequency = 48000.0;
    public float gain;
    public float volume = 0.1f;

    public float[] frequencies;
    public int thisFreq;
    public AudioMixer brutalMixer; 

    // Use this for initialization
    void Start()
    {
        frequencies = new float[8];
        frequencies[0] = 440;
        frequencies[1] = 494;
        frequencies[2] = 554;
        frequencies[3] = 587;
        frequencies[4] = 659;
        frequencies[5] = 740;
        frequencies[6] = 831;
        frequencies[7] = 880;
    }

    void Update()
    {
  
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gain = volume;
            frequency = frequencies[thisFreq];
            thisFreq += 1;
            thisFreq = thisFreq % frequencies.Length;
            brutalMixer.SetFloat("depth", thisFreq / 7.0f);

        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            gain = 0;
        }
    }

    //unity's function to analize audio. I don't know how to not make it play the wave form and the effect? That's for exploration 
    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0f * Mathf.PI / sampling_frequency;
        for (int i = 0; i < data.Length; i += channels) {
            phase += increment;
            data[i] = (float)(gain * Mathf.Sin((float)phase));
            if (channels == 2)
            {
                data[i + 1] = data[i];
            
            }
            if (phase > (Mathf.PI * 2))
            {
                phase = 0.0f; 
            }
        } 
    }


}
