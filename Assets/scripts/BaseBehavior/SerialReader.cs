using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;
using System;



//public struct Colors {
//   public static Color     
//} 

public class SerialReader : AbstractInputReader
{

    //opens a queue of bites, which is the opposite of a stack. It's first in first out. Just like a line, you deal with the data in the order it shows up. 
    Queue<byte[]> writeQueue;

    SerialPort stream = null; //serial port data 

    MenuButtonState menu_state;

    string incommingData; //data coming in through the port 

    bool openStream; //is the SerialPort Open? 

    //list the port names and return the first serial port in the stack of possible serial ports on your machine, this is usually your arduino
    string getSerialPort()
    {

        string[] ports = SerialPort.GetPortNames();
        if (ports.Length == 0) {
            Debug.Log("No serial port found.");
            return "";
        }
        return ports[0]; // TODO: At present this uses the first found serial input. --cap
    }


    public bool OpenStream()
    {

        string port = getSerialPort(); //gets the first port at position 0. 

        if (port == "")
        {
            Debug.Log("Terminating stream enable...\n (Hint: Hit semicolon (;) to switch to keyboard input.)");
            return false; // TODO: The case of there not being input is handled a little inelegantly. --cap
        }

        stream = new SerialPort(port, 115200); //opens the serial port 
        stream.WriteTimeout = 10; //this is long. - we might want to test this. 
        stream.ReadTimeout = 10; // Need to nicely handle this.
        Debug.Log("Opening stream...");
        stream.Open();
        return true;
    }

    public bool CloseStream()
    {
        // Stream already open
        if (stream == null || !stream.IsOpen)
        {
            return false;
        }
        else
        {
            stream.BaseStream.Close();
            stream.Close();

            stream = null;

            return true;
        }
    }

    public string ReadDataFromArduino()
    {
        // Stream not open
        if (stream == null ||
            !stream.IsOpen)
            return null;

        try
        {
            // Attemps a read
            return stream.ReadLine();
        }
        catch (TimeoutException exception)
        {
            // Error!
            Debug.Log(exception);
            return null;
        }
    }

    public string[] SplitIncomingDataToStrings(string incomingSensorData)
    {
        string[] sensors = incomingSensorData.Split(' ');
        return sensors;
    }

    public void SetIncomingDataToGameData(string[] sensors) {
        //this is the touch passes
        //Debug.Log(sensors[0] + "this much data"); 
        if (sensors.Length == 2)
        {
            passOnTouch(new TouchedBots(sensors[0], sensors[1])); //creates a new touchedBots struct and passes in data.  

            Debug.Log(sensors[0] + sensors[1]);
        }
        //this is the accelerometers 
        else if (sensors.Length == 6)
        {

            passOnBotDataReceived(new Bot(sensors[0], sensors[1], sensors[2], sensors[3], sensors[4], sensors[5]));
        }

        //this is menu data
        else if (sensors.Length == 3)
        {
            // Menu Button update
            MenuButtonState newMenu = new MenuButtonState(sensors[1], sensors[2]);
            if (menu_state.def)
            {
                if (newMenu.oc && !menu_state.oc)
                {
                    MenuFreePlay();
                }

                if (newMenu.slc && !menu_state.slc)
                {
                    MenuSecretCiphers();
                }
            }
            menu_state = newMenu;
        }
    }

    void checkForWrites()
    {
        while (writeQueue.Count > 0 && stream != null)
        {
            stream.Write(writeQueue.Dequeue(), 0, 2); //this sends the first byte in the writeQueue, it starts with the first byte in the buffer and sends 2 bytes of data. we are sending only 2 bytes to arduino this way to save memory and increase speed. 
        }
    }

    //queues up data to write to the serial port
    public void queueWrite(byte[] wri)
    {
        writeQueue.Enqueue(wri);
    }

    //this update function simply checks if anything needs to be written. 
    void Update()
    {
        checkForWrites(); //makes sure if there's data in our writeQueue, it sends. 
 
    }
    void OnEnable()
    {
        base.OnEnable(); //calls the base class enable function
        OnWriteToSerial += queueWrite; //calls queueWrite when the OnWriteSerial event is called. 
        writeQueue = new Queue<byte[]>();
        openStream = OpenStream();
        if(openStream)
        { 
        InvokeRepeating("handleData",0, 0.001f);
        Debug.Log("Starting stream coroutine...");
        }
    }

    //function which is called when the coroutine starts up.  It fires off the call back function and returns data if it can read something from the port.
    string data; 
    public void handleData()
    {
            if (openStream)
            {
                data = ReadDataFromArduino();
                Debug.Log("I AM HERE");
                if (data != null)
                {
                    Debug.Log(data);
                    string[] dataStrings = SplitIncomingDataToStrings(data);
                    SetIncomingDataToGameData(dataStrings);
                }
            }
     }
    
    void OnDisable()
    {
        base.OnDisable();
        OnWriteToSerial -= queueWrite;
        CancelInvoke("handleData");
        CloseStream(); 
    }

}

