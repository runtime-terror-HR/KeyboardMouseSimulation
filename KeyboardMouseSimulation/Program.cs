using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;

namespace KeyboardMouseSimulation
{
    static class Program
    {
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
            bool isKeyboard = true;
            Console.WriteLine("Enter Port (eg. COM3) : ");
            var portName = "COM3"; //Console.ReadLine();
            SerialPort port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            try
            {
                port.Open();
            }
            catch(Exception ex)
            {
                Console.WriteLine("error opening port " + ex.Message);
                return;
            }

            int count = 0;
            while (true)
            {
                var rec = port.ReadLine();
                isKeyboard = rec.Contains("255") ? !isKeyboard : isKeyboard;

                Console.WriteLine("recieved no. " + count + " : " + rec);
                count++;

                
                if (isKeyboard)
                {
                    var keys = rec.Split(',');
                    foreach (var key in keys)
                    {
                        var keyval = key.TrimEnd('\r', '\n');
                        var tem = (uint)(int) new Int32Converter().ConvertFromString(keyval);
                        keybd_event(tem, 0, 0, 0);
                    }
                    foreach (var key in keys)
                    {
                        var keyval = key.TrimEnd('\r', '\n');
                        var tem = (uint)(int)new Int32Converter().ConvertFromString(keyval);
                        keybd_event(tem, 0, 0x0002, 0);
                    }
                }
                else
                {
                    var keys = rec.Split('\r');

                    if (keys[0].Contains(","))  // normal mouse movement
                    {

                        keys = keys[0].Split(',');
                        try
                        {
                            int x = int.Parse(keys[0].TrimEnd('\r', '\n'));
                            int y = int.Parse(keys[1].TrimEnd('\r', '\n'));
                            RelativeMove(x, -1 * y);
                        }
                        catch
                        {
                        }
                    }
                    else    // mouse click
                    {   
                    }
                }
            }
        }
    }
}
