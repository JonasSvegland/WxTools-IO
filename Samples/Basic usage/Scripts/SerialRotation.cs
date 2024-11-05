using UnityEngine;
using WxTools.IO;

public class SerialRotation : SerialDataTransciever
{
    [SerializeField]
    private Vector3 axis = Vector3.up;

    [SerializeField]
    private float maximumAngle = 360.0f;


    private float angle = 0;

    // Update is called once per frame
    void Update()
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);
        transform.rotation = q;
    }

    protected override void RecieveDataAsRatio01(float ratio)
    {
        angle = ratio * maximumAngle;
    }   
}
