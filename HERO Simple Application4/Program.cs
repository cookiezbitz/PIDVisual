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
        private TalonSRX _talon = new TalonSRX(1);
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




        public static void Main()
        {
            //variable declarations
            double read0;
            double read1;
            double read2;
            int forwardbackward = -1;
            double setpoint;
            double error;
            double integral;
            double derivative;
            double prevError;
            double power;
            double kP;
            double kI;
            double kD;


            /* create a gamepad object */
            CTRE.Phoenix.Controller.GameController myGamepad = new CTRE.Phoenix.Controller.GameController(new
            CTRE.Phoenix.UsbHostDevice(1));
            /* create a talon, the Talon Device ID in HERO LifeBoat is zero */
            CTRE.Phoenix.MotorControl.CAN.TalonSRX myTalon = new CTRE.Phoenix.MotorControl.CAN.TalonSRX(1);
            double encodervalue = (double)myTalon.SetSelectedSensorPosition(0, 0, 0);

            /* Factory Default all hardware to prevent unexpected behaviour */
            myTalon.ConfigFactoryDefault();
            /* specify sensor characteristics */
            myTalon.ConfigSelectedFeedbackSensor(FeedbackDevice.CTRE_MagEncoder_Relative, 0);
            myTalon.SetSensorPhase(false); /* make sure positive motor output means sensor moves in position direction */


            /* brake or coast during neutral */
            myTalon.SetNeutralMode(NeutralMode.Brake);

            /* closed-loop and motion-magic parameters */
            myTalon.Config_kF(kSlotIdx, 0.1153f, kTimeoutMs); // 8874 native sensor units per 100ms at full motor output (+1023)
            myTalon.Config_kP(kSlotIdx, 2.00f, kTimeoutMs);
            myTalon.Config_kI(kSlotIdx, 0f, kTimeoutMs);
            myTalon.Config_kD(kSlotIdx, 20f, kTimeoutMs);
            myTalon.Config_IntegralZone(kSlotIdx, 0, kTimeoutMs);
            myTalon.SelectProfileSlot(kSlotIdx, 0); /* select this slot */
            myTalon.ConfigNominalOutputForward(0f, kTimeoutMs);
            myTalon.ConfigNominalOutputReverse(0f, kTimeoutMs);
            myTalon.ConfigPeakOutputForward(1.0f, kTimeoutMs);
            myTalon.ConfigPeakOutputReverse(-1.0f, kTimeoutMs);
            /* Home the relative sensor, 
    alternatively you can throttle until limit switch,
    use an absolute signal like CtreMagEncoder_Absolute or analog sensor.
    */
            myTalon.SetSelectedSensorPosition(0, kTimeoutMs);




            /* simple counter to print and watch using the debugger */
            int counter = 0;
            /* loop forever */
            while (true)
            {

                /*
                error = setpoint - (double)myTalon.SetSelectedSensorPosition(0, 0, 0);
                integral = integral + error;
                if (error = 0 or passes setpoint)
                    integral = 0;
                if (error is outside useful range)
                    integral = 0;
                derivative = error - prevError;
                prevError = error;
                power = error * kP + integral * kI + derivative * kD;
                */


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
                    Debug.Print("button1: " + myGamepad.GetButton(1));
                    Debug.Print("button2: " + myGamepad.GetButton(2));
                    /* print the axis value */
                    Debug.Print("axis:" + myGamepad.GetAxis(1));
                    /* pass axis value to talon */
                    //-.045 to actually stop the thing as a temporary solution because there is an error regarding that
                    myTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, read1 * forwardbackward);

                    if (myGamepad.GetButton(1))
                    {
                        Debug.Print("done here");
                        forwardbackward = -1;
                    }
                    if (myGamepad.GetButton(3))
                    {
                        Debug.Print("done there");
                        forwardbackward = 1;
                    }
                    

                    
                    if (myGamepad.GetButton(2))
                    {
                        forwardbackward = 0;
                    }



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