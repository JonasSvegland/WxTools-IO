using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WxTools.IO
{

    /// <summary>
    /// SerialCommunication
    /// </summary>
    public class SerialCommunication : MonoBehaviour
    {
        /// <summary>
        /// Get the product version
        /// </summary>
        /// <returns>product version</returns>
        public static string GetProductVersion()
        {
            return "v1.1.0";
        }

        [Space()]
        //[ReadOnly]
        [SerializeField]
        private string version = SerialCommunication.GetProductVersion();

        [Header("Basic settings")]
        [Tooltip("The baudrate must be the same as setup on the hardware")]
        [SerializeField]
        private string serialPort = "COM5";
        [SerializeField]
        [Tooltip("The baudrate must be the same as setup on the hardware")]
        private int baudRate = 115200;

        [Header("Debug")]
        [SerializeField]
        private bool debugLogAllData = false;
        [SerializeField]
        private bool debugLogCommentData = false;

        [Header("Advanced - Read & write settings")]
        [Tooltip("Only change if you know why")]
        [SerializeField]
        private bool discardInBufferIntervals = false;
        [SerializeField]
        private int discardIntervalMilliseconds = 250;
        [SerializeField]
        private int readTimeOutMilliseconds = 32;
        [SerializeField]
        private int readIntervalMilliseconds = 16;
        [SerializeField]
        private int writeTimeOutMilliseconds = 32;
        [SerializeField]
        private int writeIntervalInMilliseconds = 16;
        
        [Header("Advanced - Data settings")]
        [Tooltip("Only change if you know why")]
        [SerializeField]
        public int arduinoMaximumOutputValue = 1000;

      


        private Queue<string> sendBuffer;
        private SerialPort port;

        /// <summary>
        /// SerialDataRecievedEventHandler
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="data">The serial data recived on the SerialPort</param>
        public delegate void SerialDataRecievedEventHandler(object sender, string data);

        /// <summary>
        /// A event called when data is recievbed on the serial port
        /// </summary>
        public event SerialDataRecievedEventHandler DataRecieved;

        void Awake()
        {
            if (sendBuffer == null)
                sendBuffer = new Queue<string>();

            Debug.Log("SerialCommunication " + GetProductVersion());

            
        }

      
        /// <summary>
        /// Initialize everything
        /// Create and open a SerialPort
        /// Start two coroutines, one for reading and one for writing
        /// </summary>
        public void Start()
        {
           

            OpenPort();
            StartCoroutine(ReadPortDataCoroutine());
            StartCoroutine(WritePortDataCoroutine());

            if (discardInBufferIntervals)
                StartCoroutine(DiscardPortBuffer());
        }


      


        /// <summary>
        /// Called when the SerialCommunication object is being destroyed
        /// </summary>
        public void OnDestroy()
        {
            // if port exists and is open, close it
            if (port != null && port.IsOpen)
            {
                try
                {
                    port.Close();
                }
                catch (IOException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// Create and open a SerialPort
        /// </summary>
        private void OpenPort()
        {
            String portNames = "";
            foreach (String port in SerialPort.GetPortNames())
                portNames += port + " ";
            Debug.Log("Available ports: " + portNames);

            Debug.Log("Open SerialPort on " + serialPort);
            try
            {
                if (port == null)
                    port = new SerialPort(serialPort, baudRate);
                else if (port.IsOpen)
                    port.Close();

                port.ReadTimeout = readTimeOutMilliseconds;
                port.WriteTimeout = writeTimeOutMilliseconds;
                port.NewLine = "\n";
                // The default is 8 data bits, no parity, one stop bit.
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;

                port.Open();

                
            }
            catch (IOException e)
            {
                Debug.LogException(e);
            }
            catch (InvalidOperationException e)
            {
                Debug.LogException(e);
            }
            catch (ArgumentException e)
            {
                Debug.LogException(e);
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogException(e);
            }

            if(port.IsOpen)
                Debug.Log("Port opened successfully");
        }

        /// <summary>
        /// Enqueue data to be written to the SerialPort
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void SendData(SerialDataTransciever sender, string data)
        {
            if (sendBuffer != null)
                sendBuffer.Enqueue(data);
        }
        /// <summary>
        /// Discards incoming data in intervals
        /// </summary>
        /// <returns>IEnumerator for Unitys coroutine functionality</returns>
        IEnumerator DiscardPortBuffer()
        {
            if (port == null)
            {
                Debug.LogError("No SerialPort");
                yield return null;
            }

            while (port.IsOpen)
            {
                port.DiscardInBuffer();
                yield return new WaitForSeconds(discardIntervalMilliseconds * 0.001f);
            }
        }

        /// <summary>
        /// Read data from the SerialPort
        /// </summary>
        /// <returns>IEnumerator for Unitys coroutine functionality</returns>
        IEnumerator ReadPortDataCoroutine()
        {
            if (port == null)
            {
                Debug.LogError("No SerialPort");
                yield return null;
            }
            else if(port.IsOpen == false)
            {
                Debug.LogError("SerialPort not open");
                yield return null;
            }

            string latest = "";



            while (port.IsOpen)
            {
                try
                {
                    // Read some data
                    latest = port.ReadLine();
                    if(debugLogAllData)
                        Debug.Log("Data: " + latest);

                    if (latest.StartsWith("//") && debugLogCommentData == true)
                        Debug.Log("Comment: " + latest.Remove(0, 2));
                    else
                        DataRecieved(this, latest);
                }
                catch (TimeoutException e)
                {        
                    //DO nothing
                }
                catch (InvalidOperationException e)
                {
                    Debug.LogWarning(e.ToString());
                }

                yield return new WaitForSeconds(readIntervalMilliseconds * 0.001f);
            }

            
        }

        /// <summary>
        /// Write enqueued data to the SerialPort
        /// </summary>
        /// <returns>IEnumerator for Unitys coroutine functionality</returns>
        IEnumerator WritePortDataCoroutine()
        {
            if (port == null)
            {
                Debug.LogError("No SerialPort");
                yield return null;
            }
            else if (port.IsOpen == false)
            {
                Debug.LogError("SerialPort not open");
                yield return null;
            }

            while (port.IsOpen)
            {
                try
                {
                    // Send some data
                    if (sendBuffer != null && sendBuffer.Count > 0)
                    {
                        string data = sendBuffer.Dequeue();
                        Debug.Log("Sending data" + data);
                        port.WriteLine(data);
                    }
                }
                catch (TimeoutException e)
                {
                    Debug.LogWarning(e.ToString());
                }
                catch (ArgumentNullException e)
                {
                    Debug.LogWarning(e.ToString());
                }
                catch (InvalidOperationException e)
                {
                    Debug.LogWarning(e.ToString());
                }

                yield return new WaitForSeconds(writeIntervalInMilliseconds * 0.001f);
            }

         
        }
    }
}
