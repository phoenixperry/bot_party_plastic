using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataExposoer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame

    //Hi Frieda!!!!!!!!!!!!!!! This function shows you how to address all of the raw data. 
    public float bot1_xtilt, box1_ytilt, bot1_ztilt, bot1_compass;
    public float bot2_xtilt, bot2_ytilt, bot2_ztilt, bot2_compass;
    public float bot3_xtilt, bot3_ytilt, bot3_ztilt, bot3_compass;


    void Update () {
        //bot 1 

        try
        {
            bot1_xtilt = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot1_data.xpos);

            box1_ytilt = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot1_data.ypos);

            bot1_ztilt = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot1_data.zpos);

            bot1_compass = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot1_data.compass);

            //bot 2
            bot2_xtilt = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot2_data.xpos);

            bot2_ytilt = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot2_data.ypos);

            bot2_ztilt = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot2_data.zpos);

            bot2_compass = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot2_data.compass);

            //bot 3
            bot3_xtilt = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot3_data.xpos);

            bot3_ytilt = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot3_data.ypos);

            bot3_ztilt = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot3_data.zpos);

            bot3_compass = float.Parse(gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot3_data.compass);
        }catch { }


        //Debug.Log(
        // gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot1_data.xpos + " bot 1: xpos " +
        // gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot1_data.xpos + " xpos " +
        //gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot1_data.xpos + " zpos " +
        //gameObject.GetComponent<SpawnGame>().botDataManager.GetComponent<BotData>().bot1_data.xpos + " compass");

    }
}
