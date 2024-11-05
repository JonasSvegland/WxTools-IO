using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WxTools.IO;

public class SerialSquish : SerialDataTransciever
{
    [SerializeField]
    private Vector3 targetScale = Vector3.one;

    private float scaleRatio = 0;
    private Vector3 originalScale = Vector3.one;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //Quaternion q = Quaternion.AngleAxis(angle, axis);
        transform.localScale = Vector3.Lerp(originalScale, targetScale, scaleRatio);
    }

    protected override void RecieveDataAsRatio01(float ratio)
    {
        scaleRatio = ratio;
    }

    protected override void ParseData(string data)
    {
        
    }
}
