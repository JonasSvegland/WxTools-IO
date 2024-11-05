using UnityEngine;
using WxTools.IO;

public class SerialToggler : SerialDataTransciever
{
    [Header("Parse settings")]
    [SerializeField]
    private int limitValueWhenTrue = 128;
    [SerializeField]
    private bool currentState = true;

    [SerializeField]
    private int readValue = 0;

    [SerializeField]
    private int maxReadValue = 0;

    [SerializeField]
    private int minReadValue = 0;

    [Header("Visual settings")]
    [SerializeField]
    private Color colorWhenTrue;
    [SerializeField]
    private Color colorWhenFalse;


    private bool lastState;

    // Use this for initialization
    void Start ()
    {
        currentState = true;
        lastState = !currentState;
    }

    // Update is called once per frame
    protected override void ParseData(string data)
    {
        int value = 0;
        if (int.TryParse(data, out value))
        {
            if (value > maxReadValue)
                maxReadValue = value;

            if (value < minReadValue)
                minReadValue = value;

            readValue = value;

            if (value >= limitValueWhenTrue)
                currentState = true;
            else
                currentState = false;

        }
         

    }



    // Update is called once per frame
    void Update()
    {
        if(lastState != currentState)
        {
            Renderer r = GetComponentInChildren<Renderer>();
            if(r != null)
                r.material.color = currentState ? colorWhenTrue : colorWhenFalse;
        }


        lastState = currentState;
    }
}
