using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WxTools.IO;

public class SerialMover :  SerialDataTransciever
{

    Vector3 orgPos;
    float ratio;

	// Use this for initialization
	void Start ()
    {
        orgPos = transform.position;
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = Vector3.Lerp(orgPos, orgPos + Vector3.up * 4.0f, ratio);
        
	}

    protected override void ParseData(string data)
    {
        base.ParseData(data);

        // "#A10000,2000,100"
        data = "10000,2000,100";

        string[] strings = data.Split(',');

        Vector3 gyro = Vector3.zero;
        int x;
        if(int.TryParse(strings[0],out x ))
        {
            gyro.x = x / 1000.0f;
        }



    }

    protected override void RecieveDataAsRatio01(float ratio)
    {
        this.ratio = ratio;
    }
}
