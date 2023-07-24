
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotData : MonoBehaviour

{

    //this class deals with all bot data. The other thing it does is map the accerometer data to a rate. If you want to play with how the accelerometers feel, the equation is here. see the function ProcessCompass and ProcessAccerometer for more 

    public static string botName;
    public static ArrayList compass = new ArrayList();
    public static int btn;
    private static string[] sensors;

	public Bot bot1_data;
	public Bot bot2_data;
	public Bot bot3_data;

    public int integratedCompass;
    int btn1Down = 0; //saves btn down state 
    int btn2Down = 0;
    int btn3Down = 0;

	public delegate void BoxOneButtonDown();
	public static event BoxOneButtonDown OnBoxOneButtonDown;
	public delegate void BoxOneButtonUp();
	public static event BoxOneButtonUp OnBoxOneButtonUp;

	public delegate void BoxTwoButtonDown();
	public static event BoxTwoButtonDown OnBoxTwoButtonDown;
	public delegate void BoxTwoButtonUp();
	public static event BoxTwoButtonUp OnBoxTwoButtonUp;

	public delegate void BoxThreeButtonDown();
	public static event BoxThreeButtonDown OnBoxThreeButtonDown;
	public delegate void BoxThreeButtonUp();
	public static event BoxThreeButtonUp OnBoxThreeButtonUp;

	public delegate void BoxOneStartMoving(double speed);
	public static event BoxOneStartMoving OnBoxOneStartMoving;
	public delegate void BoxOneContinueMoving(double speed);
	public static event BoxOneContinueMoving OnBoxOneContinueMoving;
	public delegate void BoxOneStopMoving ();
	public static event BoxOneStopMoving OnBoxOneStopMoving;

	public delegate void BoxTwoStartMoving(double speed);
	public static event BoxTwoStartMoving OnBoxTwoStartMoving;
	public delegate void BoxTwoContinueMoving(double speed);
	public static event BoxTwoContinueMoving OnBoxTwoContinueMoving;
	public delegate void BoxTwoStopMoving ();
	public static event BoxTwoStopMoving OnBoxTwoStopMoving;

	public delegate void BoxThreeStartMoving(double speed);
	public static event BoxThreeStartMoving OnBoxThreeStartMoving;
	public delegate void BoxThreeContinueMoving(double speed);
	public static event BoxThreeContinueMoving OnBoxThreeContinueMoving;
	public delegate void BoxThreeStopMoving ();
	public static event BoxThreeStopMoving OnBoxThreeStopMoving;

	public delegate void BoxOneStartRotating(double angular_speed);
	public static event BoxOneStartRotating OnBoxOneStartRotating;
	public delegate void BoxOneContinueRotating(double angular_speed);
	public static event BoxOneContinueRotating OnBoxOneContinueRotating;
	public delegate void BoxOneStopRotating ();
	public static event BoxOneStopRotating OnBoxOneStopRotating;

	public delegate void BoxTwoStartRotating(double angular_speed);
	public static event BoxTwoStartRotating OnBoxTwoStartRotating;
	public delegate void BoxTwoContinueRotating(double angular_speed);
	public static event BoxTwoContinueRotating OnBoxTwoContinueRotating;
	public delegate void BoxTwoStopRotating ();
	public static event BoxTwoStopRotating OnBoxTwoStopRotating;

	public delegate void BoxThreeStartRotating(double angular_speed);
	public static event BoxThreeStartRotating OnBoxThreeStartRotating;
	public delegate void BoxThreeContinueRotating(double angular_speed);
	public static event BoxThreeContinueRotating OnBoxThreeContinueRotating;
	public delegate void BoxThreeStopRotating ();
	public static event BoxThreeStopRotating OnBoxThreeStopRotating;
   
    public void Start() {

    }

	void OnEnable() {
        AbstractInputReader.OnBotDataReceived += processData;
	}
    //gets the data to the right bot. 
	private void processData(Bot b) {
		if (b.name == "botOne") {
			processBotDifference (b, bot1_data);
			bot1_data = b;
		} else if (b.name == "botTwo") {
			processBotDifference (b, bot2_data);
			bot2_data = b;
		} else if (b.name == "botThree") {
			processBotDifference (b, bot3_data);
			bot3_data = b;
		}
	}

	private void processBotDifference(Bot b1, Bot b2)
	{
		if (b2.name == "") return;

		// Deals with button presses
		if (b1.btn == "1" && b2.btn == "0") {
			doButtonDownFor (b1);
		} else if (b1.btn == "0" && b2.btn == "1") {
			doButtonUpFor (b1);
		}

		processAccelerometer (b2, b1);
		processCompass (b2, b1);

	}
	public static double DELTA = 0.3;
	public static double THRESHOLD = 4;
	public static double CAP = 10;
	public static double STICKINESS = 2;
	public double bot1_mv_avg = 0.0;
	public double bot2_mv_avg = 0.0;
	public double bot3_mv_avg = 0.0;
	public bool box1_moving = false;
	public bool box2_moving = false;
	public bool box3_moving = false;

	/*
	 * This works by taking a weighted moving average of the speed
	 * v(t+1) = new * d + (1-d) * v(t)
	 * if v(t+1) > T and v(t) < T then Move event
	 * if v(t+1) < T and v(t) > T then Stop Move event
	 * If new > CAP then new = cap (to stop large movement frames from skewing the data too much)
	 */
	private void processAccelerometer(Bot b1, Bot b2)
	{
		int b1x, b2x, b1y, b2y, b1z, b2z;
		if (!b2.def) {
			return;
		}
		int.TryParse(b1.xpos, out b1x); 		int.TryParse(b2.xpos, out b2x);
		int.TryParse(b1.ypos, out b1y); 		int.TryParse(b2.ypos, out b2y);
		int.TryParse(b1.zpos, out b1z); 		int.TryParse(b2.zpos, out b2z);
		double magnitude_change = Mathf.Sqrt (Mathf.Pow((b1x - b2x),2) + Mathf.Pow((b1y - b2y),2) + Mathf.Pow((b1z - b2z), 2));
		if (magnitude_change > CAP) {
			magnitude_change = CAP;
		}
		double new_mv_avg;			
		if (b1.name == "botOne") {
			new_mv_avg = DELTA * magnitude_change + (1 - DELTA) * bot1_mv_avg;
			if (!box1_moving && new_mv_avg > THRESHOLD) {
				box1_moving = true;
				OnBoxOneStartMoving (new_mv_avg);
			} else if (box1_moving && new_mv_avg < THRESHOLD - STICKINESS) {
				box1_moving = false;
				OnBoxOneStopMoving ();
			} else if (box1_moving) {
				OnBoxOneContinueMoving (new_mv_avg);
			}
			bot1_mv_avg = new_mv_avg;
		} else if (b1.name == "botTwo") {
			new_mv_avg = DELTA * magnitude_change + (1 - DELTA) * bot2_mv_avg;
			if (!box2_moving && new_mv_avg > THRESHOLD) {
				box2_moving = true;
				OnBoxTwoStartMoving (new_mv_avg);
			} else if (box2_moving && new_mv_avg < THRESHOLD-STICKINESS) {
				box2_moving = false;
				OnBoxTwoStopMoving ();
			} else if (box2_moving) {
				OnBoxTwoContinueMoving (new_mv_avg);
			}
			bot2_mv_avg = new_mv_avg;
		} else if (b1.name == "botThree") {
			new_mv_avg = DELTA * magnitude_change + (1 - DELTA) * bot3_mv_avg;
			if (!box3_moving && new_mv_avg > THRESHOLD) {
				box3_moving = true;
				OnBoxThreeStartMoving (new_mv_avg);
			} else if (box3_moving && new_mv_avg < THRESHOLD-STICKINESS) {
				box3_moving = false;
				OnBoxThreeStopMoving ();
			} else if (box3_moving) {
				OnBoxThreeContinueMoving (new_mv_avg);
			}
			bot3_mv_avg = new_mv_avg;
		}
	}
	/*
	 * This works by taking a weighted moving average of the angular speed
	 * v(t+1) = new * d + (1-d) * v(t)
	 * if v(t+1) > T and v(t) < T then Rotate event
	 * if v(t+1) < T and v(t) > T then Stop Rotate event
	 * If new > CAP then new = cap (to stop large movement frames from skewing the data too much)
	 */
	public static double COMPASS_DELTA = 0.15;
	public static double COMPASS_THRESHOLD = 15;
	public static double COMPASS_CAP = 40;
	public static double COMPASS_STICKINESS = 5;
	public double bot1_mv_avg_angle = 0.0;
	public double bot2_mv_avg_angle = 0.0;
	public double bot3_mv_avg_angle = 0.0;
	public bool box1_rotating = false;
	public bool box2_rotating = false;
	public bool box3_rotating = false;
	private void processCompass(Bot b1, Bot b2) {
		int b1a, b2a;
		int.TryParse (b1.compass, out b1a);
		int.TryParse (b2.compass, out b2a);
		double new_mv_avg_angle;
		if (b1.name == "botOne") {
			double magnitude_change = Mathf.Abs (b1a - b2a);
			if (magnitude_change > COMPASS_CAP) {
				magnitude_change = COMPASS_CAP;
			}
			new_mv_avg_angle = DELTA * magnitude_change + (1 - DELTA) * bot1_mv_avg_angle;
			if (!box1_rotating && new_mv_avg_angle > COMPASS_THRESHOLD) {
				OnBoxOneStartRotating (new_mv_avg_angle);
				box1_rotating = true;
			} else if (box1_rotating && new_mv_avg_angle < COMPASS_THRESHOLD - COMPASS_STICKINESS) {
				OnBoxOneStopRotating ();
				box1_rotating = false;
			} else if (box1_rotating) {
				OnBoxOneContinueRotating (new_mv_avg_angle);
			}
			bot1_mv_avg_angle = new_mv_avg_angle;
		} else if (b1.name == "botTwo") {
			double magnitude_change = Mathf.Abs (b1a - b2a);
			if (magnitude_change > COMPASS_CAP) {
				magnitude_change = COMPASS_CAP;
			}
			new_mv_avg_angle = DELTA * magnitude_change + (1 - DELTA) * bot2_mv_avg_angle;
			if (!box2_rotating && new_mv_avg_angle > COMPASS_THRESHOLD) {
				OnBoxTwoStartRotating (new_mv_avg_angle);
				box2_rotating = true;
			} else if (box2_rotating && new_mv_avg_angle < COMPASS_THRESHOLD - COMPASS_STICKINESS) {
				OnBoxTwoStopRotating ();
				box2_rotating = false;
			} else if (box2_rotating) {
				OnBoxTwoContinueRotating (new_mv_avg_angle);
			}
			bot2_mv_avg_angle = new_mv_avg_angle;
		} else if (b1.name == "botThree") {
			double magnitude_change = Mathf.Abs (b1a - b2a);
			if (magnitude_change > COMPASS_CAP) {
				magnitude_change = COMPASS_CAP;
			}
			new_mv_avg_angle = DELTA * magnitude_change + (1 - DELTA) * bot3_mv_avg_angle;
			if (!box3_rotating && new_mv_avg_angle > COMPASS_THRESHOLD) {
				OnBoxThreeStartRotating (new_mv_avg_angle);
				box3_rotating = true;
			} else if (box3_rotating && new_mv_avg_angle < COMPASS_THRESHOLD - COMPASS_STICKINESS) {
				OnBoxThreeStopRotating ();
				box3_rotating = false;
			} else if (box3_rotating) {
				OnBoxThreeContinueRotating (new_mv_avg_angle);
			}
			bot3_mv_avg_angle = new_mv_avg_angle;
		}
	}

	private void doButtonUpFor(Bot b1)
	{
		if (b1.name == "botOne")
			OnBoxOneButtonUp ();
		else if (b1.name == "botTwo")
			OnBoxTwoButtonUp ();
		else if (b1.name == "botThree")
			OnBoxThreeButtonUp ();
	}

	private void doButtonDownFor(Bot b1){
		if (b1.name == "botOne")
			OnBoxOneButtonDown ();
		else if (b1.name == "botTwo")
			OnBoxTwoButtonDown ();
		else if (b1.name == "botThree")
			OnBoxThreeButtonDown ();
	}
    
    // not using??? 
    public void updateData(string values)

    {
        compass.Clear();
        sensors = values.Split(' '); //split the array at every space. we use a space to deliminate our values from Arduino 
        botName = sensors[0]; //get which bot we're dealing with, which is saved in the 0 position 
        // Debug.Log(name);
        compass.Add(sensors[1]); //integrated compass 
        compass.Add(sensors[2]); //x 
        compass.Add(sensors[3]); //y
        compass.Add(sensors[4]); //z  
        int.TryParse(sensors[5], out btn); //btn
     
        Debug.Log("Bot Parsed: " + botName + " btn " + btn + "Compass vals" + compass[0] + " " + compass[1] + " " + compass[2] + " " + compass[3]);
        //Debug.Log(botName+ botName.Length); 
        routeData();
    }
    public void routeData()
    {
        if (botName == "botOne")
        {
            //route teh compass values 
            string val = compass[0] as string;
            int.TryParse(val, out integratedCompass);
            //eventually, the compass data should be passed to the functions that need it here 

            //route the accelerometer values 
            val = compass[1] as string;
            int xpos;
            int.TryParse(val, out xpos);
            val = compass[2] as string;
            int ypos;
            int.TryParse(val, out ypos);
            val = compass[3] as string;
            int zpos;
            int.TryParse(val, out zpos);
            //send the accelerometer vals to the bot using them  
            //if the btn is down  & and it was not down in the last frame  
            if (btn == 1 && btn1Down == 0)
            {
                Debug.Log("btn 1 fired");
                //flip on the bool that checks if it is down on so it does not trigger repeatedly. 
                btn1Down = 1;
                //bot1.GetComponent<botBehavior>().triggerSound(); //trigger the sound 
                //var colors = btn1.colors;
                //colors.normalColor = Color.red;
                //btn1.colors = colors;
            }
            else if (btn == 0)
            {
                btn1Down = 0; //reset the btn flag so the button can fire again 
                              //create a color then assign it through to the btn to show it's off now 
                //var colors = btn1.colors;
                //colors.normalColor = Color.green;
                //btn1.colors = colors;
            }

        } else if (botName == "botTwo")
        {
            //route teh compass values 
            string val = compass[0] as string;
            int.TryParse(val, out integratedCompass);
            //eventually, the compass data should be passed to the functions that need it here 
            //route the accelerometer values 
            val = compass[1] as string;
            int xpos;
            int.TryParse(val, out xpos);
            val = compass[2] as string;
            int ypos;
            int.TryParse(val, out ypos);
            val = compass[3] as string;
            int zpos;
            int.TryParse(val, out zpos);
          

            if (btn == 1 && btn2Down == 0)
            {
                Debug.Log("btn 2 fired");
                //flip on the bool that checks if it is down on so it does not trigger repeatedly. 
                btn2Down = 1;
                //bot2.GetComponent<botBehavior>().triggerSound(); //trigger the sound 
                //var colors = btn2.colors;
                //colors.normalColor = Color.red;
                //btn2.colors = colors;
            } else if (btn == 0)
            {
                btn2Down = 0; //reset the btn flag so the button can fire again
                              //create a color then assign it through to the btn to show it's off now 
                //var colors = btn2.colors;
                //colors.normalColor = Color.green;
                //btn2.colors = colors;
            }
        } else if (botName == "botThree")
        {
            //route teh compass values 
            string val = compass[0] as string;
            int.TryParse(val, out integratedCompass);
            //eventually, the compass data should be passed to the functions that need it here 

            //route the accelerometer values 
            val = compass[1] as string;
            int xpos;
            int.TryParse(val, out xpos);
            val = compass[2] as string;
            int ypos;
            int.TryParse(val, out ypos);
            val = compass[3] as string;
            int zpos;
            int.TryParse(val, out zpos);

            if (btn == 1 && btn3Down == 0)
            {
                Debug.Log("btn 3 fired");
                //flip on the bool that checks if it is down on so it does not trigger repeatedly. 
                btn3Down = 1;
                //bot3.GetComponent<botBehavior>().triggerSound(); //trigger the sound 
                //var colors = btn3.colors;
                //colors.normalColor = Color.red;
                //btn3.colors = colors;
            } else if (btn == 0)
            {
                btn3Down = 0; //reset the btn flag so the button can fire again 
                              //create a color then assign it through to the btn to show it's off now 
                //var colors = btn3.colors;
                //colors.normalColor = Color.green;
                //btn3.colors = colors;
            }
      }
     }
    

}
