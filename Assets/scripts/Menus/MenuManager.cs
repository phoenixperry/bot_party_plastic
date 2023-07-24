using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.Timeline;

public class MenuManager : AbstractManager
{
    int FRAME_NEXT_SOUND = 0;
    public AudioClip Bot1_Move_Sound, Bot2_Move_Sound, Bot3_Move_Sound;
	public List<string> clipNames;
	public List<AudioClip> clips;
    public static int frameCount;
    public int RandomNumber { get; private set; }
	void Start() {
		TurnOffLEDOne ();
		TurnOffLEDTwo ();
		TurnOffLEDThree ();
		FRAME_NEXT_SOUND = Time.frameCount + 600;
		AudioSource speaker = gameObject.transform.Find ("RandomSound").GetComponent<AudioSource> ();
		speaker.clip = Resources.Load ("Menu/capage-drafts/Standby Mode Initiated") as AudioClip;
		speaker.Play ();
	}
	void OnEnable() {
		base.OnEnable ();
		string BASE_DIR = "Menu/capage-drafts/";
		// TODO: Proper idle sounds
		clipNames.Add (BASE_DIR+"Assist Us");
		clipNames.Add (BASE_DIR+"Awaiting Mode Instructions");
		clipNames.Add (BASE_DIR+"Choose Your Transfer Proto");
		clipNames.Add (BASE_DIR+"Open Communication");
		clips = new List<AudioClip> ();
		populateClips ();
	}
	public override void MenuFreePlay() {
		FreePlay ();
	}
	public override void MenuSecretCiphers() {
		TouchTouchRevolution ();
	}
	void populateClips() {
		for (int i = 0; i < clipNames.Count; i++) {
			AudioClip c = Resources.Load(clipNames [i]) as AudioClip;
			if (c) { 
				clips.Add (c);
			} else {
				Debug.Log ("ERROR - Clip is null: " + clipNames [i] + ", skipping.");
			}
		}
	}
	public void MenuWrapper() {
		Menu ();
	}
	public void FreePlayWrapper() {
		FreePlay ();
	}
	public void TouchTouchRevolutionWrapper() {
		TouchTouchRevolution ();
	}

	public static void Menu() {
		SceneManager.LoadScene ("Menu", LoadSceneMode.Single);
	}

    public static void FreePlay()
    {
        SceneManager.LoadScene("OpenCommunication", LoadSceneMode.Single);
    }

    public static void TouchTouchRevolution()
    {
        SceneManager.LoadScene("TouchTouchTransmission", LoadSceneMode.Single);
    }

    void Update()
    {

        if (Time.frameCount >= FRAME_NEXT_SOUND)
        {
            playRandomSound();
            FRAME_NEXT_SOUND = Time.frameCount + Random.Range(1000, 2000);
			//TouchTouchRevolution (); //this little hack wiht just loop LSC 
        }

    }

    void playRandomSound()
    {
		if (clips.Count == 0) {
			Debug.Log ("No sounds found, something wrong here.");
			return;
		}
		int randomClip = Random.Range(0, clips.Count);
		Debug.Log (clips[0]);
		Debug.Log (clips);
		AudioSource source = gameObject.transform.Find ("RandomSound").GetComponent<AudioSource> ();
		source.clip = clips[randomClip];
		Debug.Log (source.clip);
		source.Play();
        //Destroy(source, clips[randomClip].length);
        //gameObject.GetComponent<AudioSource>().clip = Menu_Sound as AudioClip; //Menu_Sound as AudioClip;
        //gameObject.GetComponent<AudioSource>().Play();
        //Debug.Log("Play Me!");
    }

    public override void BoxOneStartMoving(double speed)
    {
        gameObject.GetComponent<AudioSource>().clip = Bot1_Move_Sound as AudioClip;
        gameObject.GetComponent<AudioSource>().Play();
        //Debug.Log("You've woken up Bot1!");
    }

    public override void BoxTwoStartMoving(double speed)
    {
        gameObject.GetComponent<AudioSource>().clip = Bot2_Move_Sound as AudioClip;
        gameObject.GetComponent<AudioSource>().Play();
        //Debug.Log("You've woken up Bot2!");
    }

    public override void BoxThreeStartMoving(double speed)
    {
        gameObject.GetComponent<AudioSource>().clip = Bot3_Move_Sound as AudioClip;
        gameObject.GetComponent<AudioSource>().Play();
        //Debug.Log("You've woken up Bot3!");
    }
}
