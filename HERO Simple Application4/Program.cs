using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
namespace Hero_Simple_Application5
{
    public class Program
    {
        //initializes analog inputs
        static AnalogInput analogInput0 = new AnalogInput(CTRE.HERO.IO.Port8.Analog_Pin3);
        static AnalogInput analogInput1 = new AnalogInput(CTRE.HERO.IO.Port8.Analog_Pin4);
        static AnalogInput analogInput2 = new AnalogInput(CTRE.HERO.IO.Port8.Analog_Pin5);

        public static void Main()
        {
            //variable declarations
            double read0;
            double read1;
            double read2;

            /* create a gamepad object */
            CTRE.Phoenix.Controller.GameController myGamepad = new CTRE.Phoenix.Controller.GameController(new
            CTRE.Phoenix.UsbHostDevice(1));
            /* create a talon, the Talon Device ID in HERO LifeBoat is zero */
            CTRE.Phoenix.MotorControl.CAN.TalonSRX myTalon = new CTRE.Phoenix.MotorControl.CAN.TalonSRX(1);
            double encodervalue = (double)myTalon.SetSelectedSensorPosition(0, 0, 0);

            /* simple counter to print and watch using the debugger */
            int counter = 0;
            /* loop forever */
            while (true)
            {
                /* grab analog value */
                read0 = analogInput0.Read();
                read1 = analogInput1.Read();
                read2 = analogInput2.Read();


                encodervalue = myTalon.GetSelectedSensorPosition();
                
                //cannot collect encoder position??????????

                /* print the three analog inputs as three columns */
                Debug.Print("" + read0 + "\t" + read1 + "\t" + read2);
                Debug.Print("Encoder position: " + encodervalue);


                /* added inside the while loop */
                if (myGamepad.GetConnectionStatus() == CTRE.Phoenix.UsbDeviceConnection.Connected)
                {
                    // print button value
                    Debug.Print("button: " + myGamepad.GetButton(1));
                    /* print the axis value */
                    Debug.Print("axis:" + myGamepad.GetAxis(1));
                    /* pass axis value to talon */
                    //-.045 to actually stop the thing as a temporary solution because there is an error regarding that
                   myTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput,read1-.07);
                    /*  
                      if (myGamepad.GetButton(1))
                      {
                          myTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0);

                      }
                    */



                    /* allow motor control */
                    CTRE.Phoenix.Watchdog.Feed();
                }
                /* increment counter */
                ++counter; /* try to land a breakpoint here and hover over 'counter' to see it's current value.
Or add it to the Watch Tab */
                /* wait a bit */
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}