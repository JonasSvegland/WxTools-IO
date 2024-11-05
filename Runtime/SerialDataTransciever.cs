using System.Collections.Generic;
using UnityEngine;
using WxTools.Editor;

namespace WxTools.IO
{
    /// <summary>
    /// SerialDataTransciever v1.0.7
    /// </summary>
    public abstract class SerialDataTransciever : MonoBehaviour
    {
        /// <summary>
        /// The connected SerialCommunication object
        /// </summary>
        [Header("Settings")]
        [Tooltip("The connected SerialCommunication object")]
        [SerializeField]
        protected SerialCommunication communicator;

        /// <summary>
        /// The data prefix that this SerialDataTransciever object is listening for
        /// If no data prefix is defined, that raw data will be sent to be parsed
        /// </summary>
        [SerializeField]
        [Tooltip("The data prefix that this SerialDataTransciever object is listening for. If no data prefix is defined, that raw data will be sent to be parsed")]
        protected string dataReadPrefix = "";

        /// <summary>
        /// Smooth data. If turned on, the data will be an average of a number of historical values
        /// <see cref="numberOfHistoryValues"/>
        /// </summary>
        [Header("Data smoothing")]
        [Tooltip("Turn on to start smooth data based on a number of historical values. Smoothing will be an average of the current value and the historical values.")]
        [SerializeField]
        protected bool smoothData = false;

        /// <summary>
        /// The number of historical values to use when smoothing data
        /// </summary>
        [Tooltip("The number of historical values to use when smoothing data")]
        [SerializeField]
        protected int numberOfHistoryValues = 0;

        
        //[ReadOnly]
        [SerializeField]
        protected int lastIntegerValue = 0;

        protected Queue<float> ratioHistory = new Queue<float>();

        void Awake()
        {
            if (communicator != null)
                communicator.DataRecieved += ArduinoSerialCommunication_DataRecieved;
        }

        private void ArduinoSerialCommunication_DataRecieved(object sender, string data)
        {

            // no data
            if (data.Length <= 0)
                return;

            // prefix defined
            if(dataReadPrefix != string.Empty) // prefix defined listen to just that prefix
            {
                //data = "#B1000"

                // data starting with the defined prefix, otherwised discard 
                if (data.StartsWith(dataReadPrefix))
                {
                    
                    // remove prefix from data and send it to be parsed
                    data = data.Remove(0, dataReadPrefix.Length);

                    //data = "1000"

                    ParseData(data);

                    float maximumValue = 1000.0f;
                    if (communicator != null)
                        maximumValue = (float)communicator.arduinoMaximumOutputValue;
                    
                    int value = 0;
                    if (int.TryParse(data, out value) == true)
                    {

                        float ratio = value / maximumValue;
                        ratio = Mathf.Clamp01(ratio);

                        #region *********** Smothing ************
                        
                        if (smoothData)
                        {
                           //ratioHistory.Enqueue(ratio);
                            float r = ratio;
                            
                            foreach (var item in ratioHistory)
                            {
                                r += item;
                            }

                            if (ratioHistory.Count > 0)
                                r /= ratioHistory.Count;

                            if(value != lastIntegerValue)
                                ratioHistory.Enqueue(ratio);

                            if (ratioHistory.Count > numberOfHistoryValues)
                                ratioHistory.Dequeue();

                            ratio = r;
                        }

                        lastIntegerValue = value;
                        #endregion *********** Smothing ************

                        RecieveDataAsRatio01(ratio);
                    }
                    else
                        Debug.LogWarning("TryParse int from data " + data + " failed!");
                }
            }
            // no prefix defined, send raw data to be parsed
            else
            {
               // data = data.Substring(2, data.Length - 2); // hack: remove the two starting chars
                ParseData(data);
            }
        }

        /// <summary>
        /// Send data via the SerialCommunication object
        /// </summary>
        /// <param name="data">The data to send</param>
        protected virtual void SendData(string data)
        {
            if (communicator != null)
                communicator.SendData(this, data);
        }

        /// <summary>
        /// Send data and float value via the SerialCommunication object
        /// </summary>
        /// <param name="data">The data to send</param>
        /// <param name="value">The value to send. everything below 0.000001f will be 0</param>
        protected virtual void SendData(string data, float value)
        {
            if (communicator != null)
            {
                //value = 90
                string d = data;
                float fvalue = value * 1000000.0f;
                int ivalue = (int)fvalue;
                d += ":" + ivalue; // Jonas:90000000
                communicator.SendData(this, d);
            }
        }

        /// <summary>
        /// Override this method to be able to parse and react to recieved data
        /// </summary>
        /// <param name="data">The data to parse</param>
        protected virtual void ParseData(string data)
        {
        }

        /// <summary>
        /// Override this method to be able to parse and react to recieved data
        /// </summary>
        /// <param name="ratio">The ratio ranging from 0 to 1.0f</param>
        protected virtual void RecieveDataAsRatio01(float ratio)
        {
        }

        /// <summary>
        /// Draw gizmos when selected in the Unity Editor
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            if (communicator != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(this.transform.position, communicator.transform.position);
            }
        }
    }
}
