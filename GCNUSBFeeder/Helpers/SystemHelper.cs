using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Windows.Forms;
using Microsoft.Win32;

using System.Diagnostics;
using System.Threading;

namespace GCNUSBFeeder
{
    public class SystemHelper
    {
        public static event EventHandler<Driver.LogEventArgs> Log;

        #region Registry Functions
        public static RegistryKey registryRun = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        public static void addToStartup()
        {
            registryRun.SetValue("GCNAdapter", Application.ExecutablePath.ToString());
        }

        public static void removeFromStartup()
        {
            registryRun.DeleteValue("GCNAdapter", false);
        }

        public static bool isOnStartUp()
        {
            if (registryRun.GetValue("GCNAdapter") != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        public static void checkForMissingDrivers()
        {
            bool vJoy   = false;
            bool libUsb = false;
            SelectQuery query = new SelectQuery("Win32_SystemDriver");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            var drivers = searcher.Get();
            foreach (var d in drivers)
            {
                if (d["Name"].ToString() == "vjoy") vJoy = true;
                if (d["Name"].ToString().Contains("libusb")) libUsb = true;
                if (vJoy && libUsb) break;
            }
            if (!libUsb)
            {
                Log(null, new Driver.LogEventArgs("LibUSB was not detected, please rerun the installer."));
            }
            if (!vJoy)
            {
                Log(null, new Driver.LogEventArgs("vJoy was not detected, please rerun the installer."));
            }
        }

        public static void RunConfigureJoysticks()
        {
            //configjoysticks.bat needs to be in the application root directory for this to work.
            Log(null, new Driver.LogEventArgs("Attempting to reconfigure vJoy..."));
            try
            {
                var p = Process.Start(Application.StartupPath + @"\ConfigJoysticks.bat");
                while (!p.HasExited)
                {
                    Thread.Sleep(1000);
                }
                Log(null, new Driver.LogEventArgs("Reconfigure has completed."));
            }
            catch
            {
                Log(null, new Driver.LogEventArgs("Error: ConfigJoysticks.bat was not found or unable to start."));
            }
        }

        public static void RunLibUsbInstall()
        {
            //install.bat needs to be in the .\LibUSB\ directory for this to work
            Log(null, new Driver.LogEventArgs("Attempting LibUSB WUP-028.inf filter fix..."));
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WorkingDirectory = @".\LibUSB";
                psi.FileName = "install.bat";
                var p = new Process();
                p.StartInfo = psi;
                p.Start();

                while (!p.HasExited)
                {
                    Thread.Sleep(1000);
                }
                Log(null, new Driver.LogEventArgs("LibUSB filter fix has completed."));
            }
            catch
            {
                Log(null, new Driver.LogEventArgs(@"Error: \LibUSB\install.bat was not found or unable to start."));
            }
        }

    }
}
