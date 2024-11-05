using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WxTools.IO;
public class SerialSender : SerialDataTransciever
{
    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TurnOnLed()
    {
        SendData("TurnOnLed ");
    }

    public void TurnOffLed()
    {
        SendData("TurnOffLed");
    }


}
