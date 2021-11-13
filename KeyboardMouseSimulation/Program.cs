using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;

namespace KeyboardMouseSimulation
{
    static class Program
    {

        const int A_key = 0x41;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void RelativeMove(int relx, int rely)
        {
            mouse_event(0x0001, relx, rely, 0, 0);
        }

        [STAThread]
        static void Main()
        {
            SerialPort port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            port.Open();

            int count = 0;
            while (true)
            {
                var rec = port.ReadLine();
                Console.WriteLine("recieved no. " + count + " : " + rec);
                count++;

                var valueInInt = uint.Parse(rec);
                keybd_event(valueInInt, 0, 0, 0);

                //mouse test
                //Thread.Sleep(5000);
                //RelativeMove(10, 0);

            }
        }
    }
}
