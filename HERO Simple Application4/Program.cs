using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
namespace Hero_Simple_Application5
{
    public class Program
    {
        public static void Main()
        {
            /* create a gamepad object */
            CTRE.Phoenix.Controller.GameController myGamepad = new CTRE.Phoenix.Controller.GameController(new
           CTRE.Phoenix.UsbHostDevice(1));
            /* create a talon, the Talon Device ID in HERO LifeBoat is zero */
            CTRE.Phoenix.MotorControl.CAN.TalonSRX myTalon = new CTRE.Phoenix.MotorControl.CAN.TalonSRX(1);
            /* simple counter to print and watch using the debugger */
            int counter = 0;
            /* loop forever */
            while (true)
            {
                /* added inside the while loop */
                if (myGamepad.GetConnectionStatus() == CTRE.Phoenix.UsbDeviceConnection.Connected)
                {
                    /* print the axis value */
                    Debug.Print("axis:" + myGamepad.GetAxis(1));
                    /* pass axis value to talon */
                    myTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, myGamepad.GetAxis(1));
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