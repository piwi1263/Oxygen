﻿using System;
using System.Threading;
using System.Diagnostics;

using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using nanoFramework.Targets.Community.I2M_OXYGEN_NF;

namespace I2C
{
    public class Program
    {
        // The array containing the digital numbers 0, 1, 2, 3, 4, 5, 6, 7, 8, 9
        private static readonly byte[] numbertable = { 0x3F, 0x06, 0x5B, 0x4F, 0x66, 0x6D, 0x7D, 0x07, 0x7F, 0x6F };
        
        // The 4 digits 7-Segment display from Adafruit
        private static I2cDevice _dsp1;
        private static I2cDevice _dsp2;

        // Heartbeat LED
        private static GpioPin _led;
        
        // Some effects
        private static readonly byte[] e1 = {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };
        private static readonly byte[] e2 = {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01
        };
        private static readonly byte[] e3 = {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        // Timer stuff
        private static Timer _timer;
        private static int _blinker;
        private static int _counter;
        private static string _digits;

        public static void Main()
        {
            try
            {
                // Init the displays
                InitDisplays();

                // Some blinky first
                _led = GpioController.GetDefault().OpenPin(Gpio.UserLed);
                _led.SetDriveMode(GpioPinDriveMode.Output);

                // Create a timer with a second jump
                _blinker = 0;
                _counter = 0;
                _timer = new Timer(Tick, null, 0, 1000);

                // The mandatory heartbeat
                for (; ; )
                {
                    _led.Write(_led.Read() == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High);
                    Thread.Sleep(1000);
                }

            }
            catch (Exception ex)
            {
                // Do whatever please you with the exception caught
                Console.WriteLine(ex.ToString());
                for (; ; )
                {
                    _led.Write(_led.Read() == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High);
                    Thread.Sleep(250);
                }
            }
        }

        private static void Tick(object info)
        {
            try
            {
                // Increment the counter
                _counter++;

                // Make a 4 digit string from that
                _digits = _counter.ToString("D8");

                // Blink per second
                _blinker = _blinker == 0x00 ? 0xFF : 0x00;

                // Show something nice
                _dsp1.Write(new byte[] {
                    0x00, numbertable[int.Parse(_digits.Substring(0, 1))],
                    0x00, numbertable[int.Parse(_digits.Substring(1, 1))],
                    0x00, (byte)_blinker,
                    0x00, numbertable[int.Parse(_digits.Substring(2, 1))],
                    0x00, numbertable[int.Parse(_digits.Substring(3, 1))]
                });
                _dsp2.Write(new byte[] {
                    0x00, numbertable[int.Parse(_digits.Substring(4, 1))],
                    0x00, numbertable[int.Parse(_digits.Substring(5, 1))],
                    0x00, (byte)_blinker,
                    0x00, numbertable[int.Parse(_digits.Substring(6, 1))],
                    0x00, numbertable[int.Parse(_digits.Substring(7, 1))]
                });
            }
            catch (Exception timerEx)
            {
                // Do whatever please you with the exception caught
                Console.WriteLine(timerEx.ToString());
                for (; ; )
                {
                    _led.Write(_led.Read() == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High);
                    Thread.Sleep(50);
                }
            }
        }

        private static void InitDisplays()
        {
            // Instantiate the 4-Digits 7-Segment Display
            _dsp1 = I2cDevice.FromId("I2C1", new I2cConnectionSettings(0x71) { BusSpeed = I2cBusSpeed.StandardMode });

            // Set oscillator on
            _dsp1.Write(new byte[] { 0x21, 0x00 });

            // Blink off, Display on
            _dsp1.Write(new byte[] { 0x81, 0x00 });

            // Set brightness 0xE0 + desired brightness, here 5
            _dsp1.Write(new byte[] { 0xE5, 0x00 });
            
            // Instantiate the 4-Digits 7-Segment Display
            _dsp2 = I2cDevice.FromId("I2C1", new I2cConnectionSettings(0x70) { BusSpeed = I2cBusSpeed.StandardMode });

            // Set oscillator on
            _dsp2.Write(new byte[] { 0x21, 0x00 });

            // Blink off, Display on
            _dsp2.Write(new byte[] { 0x81, 0x00 });

            // Set brightness 0xE0 + desired brightness, here 5
            _dsp2.Write(new byte[] { 0xE5, 0x00 });

            for (int i = 0; i < e2.Length; i += 8)
            {
                _dsp1.Write(new byte[] {
                    0x00, e2[i+0],
                    0x00, e2[i+1],
                    0x00, 0x00,
                    0x00, e2[i+2],
                    0x00, e2[i+3]
                });
                _dsp2.Write(new byte[] {
                    0x00, e2[i+4],
                    0x00, e2[i+5],
                    0x00, 0x00,
                    0x00, e2[i+6],
                    0x00, e2[i+7]
                });
                Thread.Sleep(250);
            }

            for (int i = 0; i < e1.Length; i += 8)
            {
                _dsp1.Write(new byte[] {
                    0x00, e1[i+0],
                    0x00, e1[i+1],
                    0x00, 0x00,
                    0x00, e1[i+2],
                    0x00, e1[i+3]
                });
                _dsp2.Write(new byte[] {
                    0x00, e1[i+4],
                    0x00, e1[i+5],
                    0x00, 0x00,
                    0x00, e1[i+6],
                    0x00, e1[i+7]
                });
                Thread.Sleep(100);
            }

            for (int i = 0; i < e3.Length; i += 8)
            {
                _dsp1.Write(new byte[] {
                    0x00, e3[i+0],
                    0x00, e3[i+1],
                    0x00, 0x00,
                    0x00, e3[i+2],
                    0x00, e3[i+3]
                });
                _dsp2.Write(new byte[] {
                    0x00, e3[i+4],
                    0x00, e3[i+5],
                    0x00, 0x00,
                    0x00, e3[i+6],
                    0x00, e3[i+7]
                });
                Thread.Sleep(100);
            }

            for (int i = 0; i < 5; i++)
            {
                _dsp1.Write(new byte[] {
                    0x00, 0x00,
                    0x00, 0x00,
                    0x00, 0xFF,
                    0x00, 0x00,
                    0x00, 0x00
                });
                _dsp2.Write(new byte[] {
                    0x00, 0x00,
                    0x00, 0x00,
                    0x00, 0xFF,
                    0x00, 0x00,
                    0x00, 0x00
                });
                Thread.Sleep(100);
                _dsp1.Write(new byte[] {
                    0x00, 0x00,
                    0x00, 0x00,
                    0x00, 0x00,
                    0x00, 0x00,
                    0x00, 0x00
                });
                _dsp2.Write(new byte[] {
                    0x00, 0x00,
                    0x00, 0x00,
                    0x00, 0x00,
                    0x00, 0x00,
                    0x00, 0x00
                });
                Thread.Sleep(100);
            }

            // Show zeros
            _dsp1.Write(new byte[] {
                0x00, numbertable[0],
                0x00, numbertable[0],
                0x00, 0x00,
                0x00, numbertable[0],
                0x00, numbertable[0]
            });

            // Show zeros
            _dsp2.Write(new byte[] {
                0x00, numbertable[0],
                0x00, numbertable[0],
                0x00, 0x00,
                0x00, numbertable[0],
                0x00, numbertable[0]
            });
        }
    }
}