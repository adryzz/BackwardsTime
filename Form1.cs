using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackwardsTime
{
    public partial class Form1 : Form
    {
        bool Stop = true;
        DateTime Current;
        DateTime Updated;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stop = true;
            Current = DateTime.UtcNow;
            button1.Enabled = false;
            button2.Enabled = false;
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stop = false;
            Current = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
            button1.Enabled = false;
            button2.Enabled = false;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Stop)
            {
                SetDateTime(Current);
            }
            else
            {
                Updated = Updated.AddSeconds(-1);
                SetDateTime(Updated);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            button1.Enabled = true;
            button2.Enabled = true;
            SyncDateTime();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        void SetDateTime(DateTime datetime)
        {
            SYSTEMTIME st = new SYSTEMTIME
            {
                wYear = (short)datetime.Year, // must be short
                wMonth = (short)datetime.Month,
                wDay = (short)datetime.Day,
                wHour = (short)datetime.Hour,
                wMinute = (short)datetime.Minute,
                wSecond = (short)datetime.Second,
                wMilliseconds = (short)datetime.Millisecond
            };

            SetSystemTime(ref st); // invoke this method.
        }

        public static bool SyncDateTime()
        {
            try
            {
                ServiceController serviceController = new ServiceController("w32time");

                if (serviceController.Status != ServiceControllerStatus.Running)
                {
                    serviceController.Start();
                }

                //Logger.TraceInformation("w32time service is running");

                Process processTime = new Process();
                processTime.StartInfo.FileName = "w32tm";
                processTime.StartInfo.Arguments = "/resync";
                processTime.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processTime.Start();
                processTime.WaitForExit();

                //Logger.TraceInformation("w32time service has sync local dateTime from NTP server");

                return true;
            }
            catch (Exception exception)
            {
                //Logger.LogError("unable to sync date time from NTP server", exception);

                return false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.youtube.com/channel/UCWb-66XSFCV5vgKEbl22R6Q");
        }
    }
}
