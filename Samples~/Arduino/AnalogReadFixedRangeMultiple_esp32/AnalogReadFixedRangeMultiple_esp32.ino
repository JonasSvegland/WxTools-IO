/*
    Arduino - Read Fixed Range Multiple
    This code is designs to be used on an Arduino Uno Rev 3
    together with the examples in the unity3D package 
    WxTools.io.SerialCommunication.1.0.6

    A bread board schematic can also be found in the 
    unity package mentioned above
*/

/*  License:
    MIT License
    Copyright (c) [2018] Jonas Svegland
    
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
/**** Setup for all analogue ports on arduino uno ****
 *  Change min and max values as needed to calibrate sensor data
 *  Remark! Please connect unused analogue ports to ground
 */
 /**** uncomment line below to begin debug output ****/
//#define DEBUG
//#define OUTPUT_ALWAYS


const int NumSensorsInUse = 3;
bool onlyOutputWhenValueChanged = true;
char dataPrefix[] = {'A',   'B',  'C',  'D',  'E'};
int dataPins[] =    {2,     4,    35,   34,   36};
int minValues[] =   {0,     0,    400,    0,    0};
int maxValues[] =   {4095,  4095, 4095, 4095, 4095};

/**** arduino setup function ****
 *  Initialize things as needed
 */
 const int LED_BUILTIN = 13;
void setup() 
{
  // setup pins
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, LOW);
  
  // initialize serial communications at 115200 bps:
  Serial.begin(115200);

  // wait for serial port to connect. Needed for native USB
  while (!Serial) 
  {}
}

/**** DoCommand ****
 *  This function is called when the arduino recieves data via the serialport.
 *  Parse as you please and do what you whant with the result
 */
String command;
void DoCommand()
{
  command.trim();
  command.toLowerCase();

  // below is some examples of how to control the built in LED via commands (pin 13)
  if(command == F("turnonled"))
  {
    digitalWrite(LED_BUILTIN, HIGH);
  }
  else if(command == F("turnoffled"))
  {
    digitalWrite(LED_BUILTIN, LOW);
  }  

  /* TODO: Add more else if with commands above as you like, Encapsulate your string when comparing 
   * with F( and ), i. e. F("Jonas"). This lowers the use of the microcontrollers memory (RAM)
   * https://learn.adafruit.com/memories-of-an-arduino/optimizing-sram
   */
}




/*********************************************************/
/**** In most cases, no need to change anything below ****/ 
/*********************************************************/



const int MinOutValue = 0;
const int MaxOutValue = 1000;
const int LoopDelayMillis = 32;
bool isCommandRead = false;
int lastUnityOutputValues[NumSensorsInUse];

void loop() 
{

  for(int i = 0; i < NumSensorsInUse; i++)
    OutputValue(i); // read and output value from sensor with index 0
  
  // read input command
  ProcessInput();
  
  // read input & process read input and check if it's a valid command
  if(isCommandRead)
    DoCommand();

  // wait for a bit
  delay(LoopDelayMillis);
}


void OutputValue(int index)
{
   // read the analog in value:
  int sensorValue = analogRead(dataPins[index]);
 
 #ifdef DEBUG
   Serial.print(F("// Raw sensor value:"));
   Serial.print(F("#"));
   Serial.print(dataPrefix[index]);
   Serial.println(sensorValue);
 #endif

  int unityOutputValue = map(sensorValue, minValues[index], maxValues[index], MinOutValue, MaxOutValue);
  unityOutputValue = constrain(unityOutputValue, MinOutValue, MaxOutValue);
  
  // Only output value when value has changed from last loop
  #ifndef OUTPUT_ALWAYS
  if(unityOutputValue != lastUnityOutputValues[index])
  #endif
  {
    //Serial.print(dataPrefix[index]);
    Serial.print(F("#"));
    Serial.print(dataPrefix[index]);
    Serial.println(unityOutputValue);
    lastUnityOutputValues[index] = unityOutputValue;
  }
}

void ProcessInput()
{
  isCommandRead = false;
  command = F("");
  
  if(Serial.available() > 0)
    isCommandRead = true;
    
  while(Serial.available() > 0)
  {
    command = Serial.readString();
    
#ifdef DEBUG
    Serial.print(F("//"));
    Serial.println(command);
#endif
  }
}
