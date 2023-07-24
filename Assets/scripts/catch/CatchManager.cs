using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum ButtonState
{ None=0, BtnOne, BtnTwo, BtnThree}; //enum lets you use words to make the code more understandable

public class CatchManager : AbstractManager //referencing the Abstract script for calling events
{
    public static int TO_WIN = 10; //the score to win the game
    bool enabled = true; // game state
    ButtonState target = ButtonState.None; //beginning target
    int score = 0; //starting score
    public AudioClip Sound_Win, Sound_Success, Sound_Fail; //sound files in the game
    float TimerLength = 4f;
    float StartTime = 0.0f;
    float CurrentTime = 0.0f;
    float EndTime = 0.0f;
    float NewTimerLength = 5.0f;
    public Button restartBtn;
    new void OnEnable() //new game, start game
    {
        base.OnEnable();
        score = 0;
        newTarget();
        //LEDs all off to start
    }
    void Start()
    {
        EndTime = Time.time + TimerLength;
        //gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
        Button btn = restartBtn.GetComponent<Button>();
      //  btn.onClick.AddListener(TaskOnClick);
    }

    void Update()
    {

        CurrentTime = Time.time;
        if (enabled && StartTime + TimerLength > CurrentTime)
        {
            // Updates the timer on the screen - cap
            gameObject.transform.Find("TimerText").GetComponent<TextMesh>().text = "Time: " + (CurrentTime - StartTime);
        }

        if (enabled && StartTime + TimerLength < CurrentTime)
        {
            // This means they failed because they ran out of time -cap
            fail();
        }

    }

    void win() //what happens when I get 10 points, sound plays
    {
        gameObject.transform.Find("TargetText").GetComponent<TextMesh>().text = "You win!";
        gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().Stop();
        if (!gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().isPlaying)
        {
            gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().clip = Sound_Win as AudioClip;
            gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().Play();
            gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().loop = true;
        }
        enabled = false; // also disables the game.
    }
    void newTarget() //call for a new target button
    {

        ButtonState new_t = target;
        while (new_t == target)
        {
            new_t = (ButtonState)(Random.Range(1, 4));
        }
        target = new_t;

        gameObject.transform.Find("TargetText").GetComponent<TextMesh>().text = "Target: " + target;

        StartTime = Time.time;

        //LED light on for new target and make sure old LED is off again
        //dimmer LED if available (It will be soon -cap)
    }

    void fail()
    {
        // Does cleanup on a GameOver. Disabled the game, plays music, sets text to game over. -cap
        gameObject.transform.Find("TargetText").GetComponent<TextMesh>().text = "Game over!";
        enabled = false;
        GameOver = true;

        gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().clip = Sound_Fail as AudioClip;
        gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().Play();
    }

    void success() //correct button press means more prizes
    {
        score += 1;
        Debug.Log(score);
        TimerLength -= 0.25F; //decreases the time by 0.25 each time you score a point

        //timer shortens as you progress?
        // StartTime = Time.time;
        gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().clip = Sound_Success as AudioClip;
        gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().Play();

    }
    bool GameOver = false;
    void checkMatch(ButtonState input)
    {
        if (!enabled)
            return;

        if (!GameOver)
        {
            if (input == target && Time.time <= EndTime)
            {
                success();
                newTarget();

                if (score < TO_WIN)
                {
                    EndTime = Time.time + TimerLength;
                }
                else
                {
                }

            }
            else if (input != target)
            {
                // GameOver = true;
                Debug.Log(input + "input, target: " + target);
                // If they get it wrong, cause a gameover. -cap
                fail();
            }

            if (score >= TO_WIN)
            {
                win();
                // If they have enough points, cause a win -cap
                Debug.Log("yay");
            }

        }

    }// these are from abstract manager to work the events
     /* void TaskOnClick()
      {
          if (GameOver)
          {
              base.OnEnable();
              score = 0;
              newTarget();
              EndTime = Time.time + TimerLength;
              SceneManager.LoadScene(Catch);
          }
      }*/

    // I need an 'on button press - restart game'


    public override void BoxOneButtonDown()
    {
        //Debug.Log("Hello Bot 1!");
        checkMatch(ButtonState.BtnOne);
    }

    public override void BoxTwoButtonDown()
    {
        // Debug.Log("Hello Bot 2!");
        checkMatch(ButtonState.BtnTwo);
    }

    public override void BoxThreeButtonDown()
    {
        // Debug.Log("Hello Bot 3!");
        checkMatch(ButtonState.BtnThree);
    }
}
/*

    public delegate void LEDChange(int led, LED_CHANGES type, int parameter);
    public static event LEDChange DoLEDChange;
    public void TurnOnLEDOne()
    {
        DoLEDChange(1, LED_CHANGES.On, 0);
    }
    public override void TurnOnLEDTwo()
    {
        DoLEDChange(2, LED_CHANGES.On, 0);
    }
    public override void TurnOnLEDThree()
    {
        DoLEDChange(3, LED_CHANGES.On, 0);
    }

    public override void TurnOffLEDOne()
    {
        DoLEDChange(1, LED_CHANGES.Off, 0);
    }
    public override void TurnOffLEDTwo()
    {
        DoLEDChange(2, LED_CHANGES.Off, 0);
    }
    public override void TurnOffLEDThree()
    {
        DoLEDChange(3, LED_CHANGES.Off, 0);
    }
}
*/
