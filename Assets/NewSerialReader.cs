using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;



public class NewSerialReader : AbstractInputReader
{

    string incommingData; //data coming in through the port 

    //opens a queue of bites, which is the opposite of a stack. It's first in first out. Just like a line, you deal with the data in the order it shows up. 
    Queue<byte[]> writeQueue;

    SerialPort stream; //serial port data 

    MenuButtonState menu_state;
    
    //list the port names and return the first serial port in the stack of possible serial ports on your machine, this is usually your arduino
    string getSerialPort()
    {
        string[] ports = SerialPort.GetPortNames();
        if (ports.Length == 0)
        {
            Debug.Log("No serial port found.");
            return "";
        }
        return ports[0]; // TODO: At present this uses the first found serial input. --cap
    }

    //this update function simply checks if anything needs to be written. 
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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
