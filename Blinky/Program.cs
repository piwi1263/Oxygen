using System;
using System.Threading;
using System.Diagnostics;

using Windows.Devices.Gpio;
using nanoFramework.Targets.Community.I2M_OXYGEN_NF;

namespace Blinky
{
    public class Program
    {
        private static GpioPin _led;

        public static void Main()
        {
            try
            {
                // Initate the one and only on board Led
                Console.WriteLine("Program started");

                _led = GpioController.GetDefault().OpenPin(Gpio.UserLed);
                _led.SetDriveMode(GpioPinDriveMode.Output);
                _led.Write(GpioPinValue.Low);

                for (; ; )
                {
                    _led.Write(_led.Read() == GpioPinValue.Low ? GpioPinValue.High : GpioPinValue.Low);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                // Don't leave thread 
                Console.WriteLine(ex.ToString());

                for (; ; )
                {
                    _led.Write(_led.Read() == GpioPinValue.Low ? GpioPinValue.High : GpioPinValue.Low);
                    Thread.Sleep(100);
                }
            }
        }
    }
}
