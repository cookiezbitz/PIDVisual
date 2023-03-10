using CTRE.Phoenix;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
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


        /** talon to control */

        /** desired mode to put talon in */
        private ControlMode _mode = ControlMode.PercentOutput;
        /** attached gamepad to HERO, tested with Logitech F710 */
        private GameController _gamepad = new GameController(UsbHostDevice.GetInstance());
        /** constant slot to use */
        const int kSlotIdx = 0;
        /** How long to wait for receipt when setting a param.  Many setters take an optional timeout that API will wait for.
            This is benefical for initial setup (before movement), though typically not desired 
            when changing parameters concurrently with robot operation (gain scheduling for example).*/
        const int kTimeoutMs = 30;

        /* create a gamepad object */
        CTRE.Phoenix.Controller.GameController myGamepad = new CTRE.Phoenix.Controller.GameController(new
        CTRE.Phoenix.UsbHostDevice(1));
        /* create a talon, the Talon Device ID in HERO LifeBoat is zero */
        CTRE.Phoenix.MotorControl.CAN.TalonSRX myTalon = new CTRE.Phoenix.MotorControl.CAN.TalonSRX(1);



        public static void Main()
        {



            //variable declarations
            double read0;
            double read1;
            double read2;
            int forwardbackward = -1;
            float tolerance = 400;
            double currentrotation = 0;
            bool PIDdone = true;
            float stopPID = 0;
            float runPID = 1;



            double error = 0;
            double integral = 0;
            double derivative = 0;
            double prevError = 0;
            double power = 0;
            double kP = 5;
            double kI = 12;
            double kD = 7;
            double setpoint = 1;









            /* create a gamepad object */
            CTRE.Phoenix.Controller.GameController myGamepad = new CTRE.Phoenix.Controller.GameController(new
            CTRE.Phoenix.UsbHostDevice(1));
            /* create a talon, the Talon Device ID in HERO LifeBoat is zero */
            CTRE.Phoenix.MotorControl.CAN.TalonSRX myTalon = new CTRE.Phoenix.MotorControl.CAN.TalonSRX(1);

            double encodervalue = (double)myTalon.SetSelectedSensorPosition(0, 0, 0);

            /* Factory Default all hardware to prevent unexpected behaviour */
            myTalon.ConfigFactoryDefault();
            /* specify sensor characteristics */
            myTalon.SetSensorPhase(false); /* make sure positive motor output means sensor moves in position direction */


            /* brake or coast during neutral */
            myTalon.SetNeutralMode(NeutralMode.Brake);


            /* closed-loop and motion-magic parameters */
            myTalon.ConfigSelectedFeedbackSensor(FeedbackDevice.CTRE_MagEncoder_Relative, kSlotIdx, kTimeoutMs);
            myTalon.Config_kF(kSlotIdx, 0.1153f, kTimeoutMs); // 8874 native sensor units per 100ms at full motor output (+1023)
            myTalon.Config_kP(kSlotIdx, .00001f, kTimeoutMs);
            myTalon.Config_kI(kSlotIdx, 1f, kTimeoutMs);
            myTalon.Config_kD(kSlotIdx, 1f, kTimeoutMs);
            myTalon.Config_IntegralZone(kSlotIdx, 0, kTimeoutMs);
            myTalon.SelectProfileSlot(kSlotIdx, 0); /* select this slot */
            myTalon.ConfigNominalOutputForward(0f, kTimeoutMs);
            myTalon.ConfigNominalOutputReverse(0f, kTimeoutMs);
            myTalon.ConfigPeakOutputForward(runPID, kTimeoutMs);
            myTalon.ConfigPeakOutputReverse(-runPID, kTimeoutMs);


            /* Home the relative sensor, 
    alternatively you can throttle until limit switch,
    use an absolute signal like CtreMagEncoder_Absolute or analog sensor.
    */
            myTalon.SetSelectedSensorPosition(0, kTimeoutMs);


            //mag encoder units are 4096 counts per rotation


            /* simple counter to print and watch using the debugger */
            int counter = 0;
            /* loop forever */


            while (true)
            {





                /* grab analog value */
                read0 = analogInput0.Read();
                read1 = analogInput1.Read();
                read2 = analogInput2.Read();

                kP = read0;
                kI = read1;
                kD = read2;


                encodervalue = myTalon.GetSelectedSensorPosition();

            



                setpoint = 2000000;

                if (PIDdone)
                {
                    error = setpoint - encodervalue;
                    integral = integral + error;
                    if (error == 0 || encodervalue > setpoint)
                        integral = 0;
                    if (error > 100)
                        integral = 0;
                    derivative = error - prevError;
                    prevError = error;
                    power = (error * kP + integral * kI + derivative * kD)/100;
                    Debug.Print("encodervalue: " + encodervalue);
                    Debug.Print("Error: " + error);
                    Debug.Print("power: " + power);
                    
                }

               // myTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, .9);



                  myTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -power);



                /* added inside the while loop */
                if (myGamepad.GetConnectionStatus() == CTRE.Phoenix.UsbDeviceConnection.Connected)
                {


                    if (myGamepad.GetButton(1))
                    {
                        PIDdone = true;
                        Debug.Print("PID true ");
                    }

                    if (myGamepad.GetButton(4))
                    {
                        Debug.Print("PID FALSE, CURRENT ROTATION: " + currentrotation);
                        PIDdone = false;
                        power = 0;
                    }

                    /*
                    if (myGamepad.GetButton(3))
                    {
                        setpoint += .5;
                        Debug.Print("added to setpoint ");
                    }



                    if (myGamepad.GetButton(2))
                    {
                        setpoint += .5;
                        Debug.Print("removed from setpoint ");
                    }
                    */





                    /* allow motor control */
                    CTRE.Phoenix.Watchdog.Feed();
                }
                //       
                /* increment counter */
                ++counter; /* try to land a breakpoint here and hover over 'counter' to see it's current value.
Or add it to the Watch Tab */
                /* wait a bit */
                System.Threading.Thread.Sleep(10);
            }





        }

    }

}


    