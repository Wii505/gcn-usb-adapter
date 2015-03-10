using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GCNUSBFeeder.Properties;

namespace GCNUSBFeeder
{
    public partial class Configuration : Form
    {
        public static event EventHandler<Driver.LogEventArgs> Log;
        public static event EventHandler StopProc;

        public Configuration()
        {
            InitializeComponent();
            IntializeSettingsPage();
        }

        public void IntializeSettingsPage()
        {

            cbAutoStart.Checked         = (bool)Settings.Default["autoStart"];
            cbStartWithWindows.Checked  = (bool)Settings.Default["startWithWindows"];
            cbStartInTray.Checked       = (bool)Settings.Default["startInTray"];
            cbAutoUpdates.Checked       = (bool)Settings.Default["autoUpdate"];
            cbDisablevJoyOnExit.Checked = (bool)Settings.Default["disablePortsOnExit"];
            
            cbNoEventMode.Checked       = (bool)Settings.Default["noEventMode"];

            port1Enabled.Checked = (bool)Settings.Default["port1Enabled"];
            port2Enabled.Checked = (bool)Settings.Default["port2Enabled"];
            port3Enabled.Checked = (bool)Settings.Default["port3Enabled"];
            port4Enabled.Checked = (bool)Settings.Default["port4Enabled"];
            
            port1AX.Value = (int)Settings.Default["port1AX"];
            port1AY.Value = (int)Settings.Default["port1AY"];
            port1CX.Value = (int)Settings.Default["port1CX"];
            port1CY.Value = (int)Settings.Default["port1CY"];
            port1LT.Value = (int)Settings.Default["port1LT"];
            port1RT.Value = (int)Settings.Default["port1RT"];

            port2AX.Value = (int)Settings.Default["port2AX"];
            port2AY.Value = (int)Settings.Default["port2AY"];
            port2CX.Value = (int)Settings.Default["port2CX"];
            port2CY.Value = (int)Settings.Default["port2CY"];
            port2LT.Value = (int)Settings.Default["port2LT"];
            port2RT.Value = (int)Settings.Default["port2RT"];

            port3AX.Value = (int)Settings.Default["port3AX"];
            port3AY.Value = (int)Settings.Default["port3AY"];
            port3CX.Value = (int)Settings.Default["port3CX"];
            port3CY.Value = (int)Settings.Default["port3CY"];
            port3LT.Value = (int)Settings.Default["port3LT"];
            port3RT.Value = (int)Settings.Default["port3RT"];

            port4AX.Value = (int)Settings.Default["port4AX"];
            port4AY.Value = (int)Settings.Default["port4AY"];
            port4CX.Value = (int)Settings.Default["port4CX"];
            port4CY.Value = (int)Settings.Default["port4CY"];
            port4LT.Value = (int)Settings.Default["port4LT"];
            port4RT.Value = (int)Settings.Default["port4RT"];
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            txtSaving.Location = new Point(350, 205);
            txtSaving.Visible = true;
            txtSaving.Update();

            if (Settings.Default.currentVersion)
            {
                Settings.Default.Upgrade();
                Settings.Default.currentVersion = false;
            }

            Settings.Default["autoStart"]          = cbAutoStart.Checked;
            Settings.Default["startWithWindows"]   = cbStartWithWindows.Checked;
            Settings.Default["startInTray"]        = cbStartInTray.Checked;
            Settings.Default["autoUpdate"]         = cbAutoUpdates.Checked;
            Settings.Default["disablePortsOnExit"] = cbDisablevJoyOnExit.Checked;

            Settings.Default["noEventMode"] = cbNoEventMode.Checked;

            //disable joysticks we don't want, but only when a change has occured.
            if ((bool)Settings.Default["port1Enabled"] != port1Enabled.Checked)
            {
                if (!port1Enabled.Checked) { SystemHelper.DestroyJoystick(1); }
                else                       { SystemHelper.CreateJoystick(1);  }
            }
            if ((bool)Settings.Default["port2Enabled"] != port2Enabled.Checked)
            {
                if (!port2Enabled.Checked) { SystemHelper.DestroyJoystick(2); }
                else                       { SystemHelper.CreateJoystick(2);  }
            }
            if ((bool)Settings.Default["port3Enabled"] != port3Enabled.Checked)
            {
                if (!port3Enabled.Checked) { SystemHelper.DestroyJoystick(3); }
                else                       { SystemHelper.CreateJoystick(3);  }
            }
            if ((bool)Settings.Default["port4Enabled"] != port4Enabled.Checked)
            {
                if (!port4Enabled.Checked) { SystemHelper.DestroyJoystick(4); }
                else                       { SystemHelper.CreateJoystick(4);  }
            }

            Settings.Default["port1Enabled"] = port1Enabled.Checked;
            Settings.Default["port2Enabled"] = port2Enabled.Checked;
            Settings.Default["port3Enabled"] = port3Enabled.Checked;
            Settings.Default["port4Enabled"] = port4Enabled.Checked;

            Settings.Default["port1AX"] = Convert.ToInt32(port1AX.Value);
            Settings.Default["port1AY"] = Convert.ToInt32(port1AY.Value);
            Settings.Default["port1CX"] = Convert.ToInt32(port1CX.Value);
            Settings.Default["port1CY"] = Convert.ToInt32(port1CY.Value);
            Settings.Default["port1LT"] = Convert.ToInt32(port1LT.Value);
            Settings.Default["port1RT"] = Convert.ToInt32(port1RT.Value);

            Settings.Default["port2AX"] = Convert.ToInt32(port2AX.Value);
            Settings.Default["port2AY"] = Convert.ToInt32(port2AY.Value);
            Settings.Default["port2CX"] = Convert.ToInt32(port2CX.Value);
            Settings.Default["port2CY"] = Convert.ToInt32(port2CY.Value);
            Settings.Default["port2LT"] = Convert.ToInt32(port2LT.Value);
            Settings.Default["port2RT"] = Convert.ToInt32(port2RT.Value);

            Settings.Default["port3AX"] = Convert.ToInt32(port3AX.Value);
            Settings.Default["port3AY"] = Convert.ToInt32(port3AY.Value);
            Settings.Default["port3CX"] = Convert.ToInt32(port3CX.Value);
            Settings.Default["port3CY"] = Convert.ToInt32(port3CY.Value);
            Settings.Default["port3LT"] = Convert.ToInt32(port3LT.Value);
            Settings.Default["port3RT"] = Convert.ToInt32(port3RT.Value);

            Settings.Default["port4AX"] = Convert.ToInt32(port4AX.Value);
            Settings.Default["port4AY"] = Convert.ToInt32(port4AY.Value);
            Settings.Default["port4CX"] = Convert.ToInt32(port4CX.Value);
            Settings.Default["port4CY"] = Convert.ToInt32(port4CY.Value);
            Settings.Default["port4LT"] = Convert.ToInt32(port4LT.Value);
            Settings.Default["port4RT"] = Convert.ToInt32(port4RT.Value);

            Settings.Default.Save();
            PropogateSettings();

            Log(null, new Driver.LogEventArgs("Settings saved, configuration has been updated."));

            txtSaving.Visible = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //used to initialize settings from saved data.
        public static void PropogateSettings()
        {
            if ((bool)Settings.Default["startWithWindows"])
            {
                SystemHelper.addToStartup();
            }
            else
            {
                SystemHelper.removeFromStartup();
            }

            MainForm.autoUpdate         = (bool)Settings.Default["autoUpdate"];

            if (MainForm.autoUpdate)
            {

                MainForm.updater.currentVersion = (int)Settings.Default["applicationVersion"];
                MainForm.updater.updateUrl = (string)Settings.Default["updateURL"];
                MainForm.updater.CheckForUpdates();
            }

            MainForm.disablePortsOnExit = (bool)Settings.Default["disablePortsOnExit"];

            Driver.noEventMode = (bool)Settings.Default["noEventMode"];

            Driver.gcn1Enabled = (bool)Settings.Default["port1Enabled"];
            Driver.gcn2Enabled = (bool)Settings.Default["port2Enabled"];
            Driver.gcn3Enabled = (bool)Settings.Default["port3Enabled"];
            Driver.gcn4Enabled = (bool)Settings.Default["port4Enabled"];

            MainForm.autoStart   = (bool)Settings.Default["autoStart"];
            MainForm.startInTray = (bool)Settings.Default["startInTray"];
            
            Driver.gcn1DZ.analogStick.xRadius  = (int)Settings.Default["port1AX"];
            Driver.gcn1DZ.analogStick.yRadius  = (int)Settings.Default["port1AY"];
            Driver.gcn1DZ.cStick.xRadius       = (int)Settings.Default["port1CX"];
            Driver.gcn1DZ.cStick.yRadius       = (int)Settings.Default["port1CY"];
            Driver.gcn1DZ.LTrigger.radius      = (int)Settings.Default["port1LT"];
            Driver.gcn1DZ.RTrigger.radius      = (int)Settings.Default["port1RT"];

            Driver.gcn2DZ.analogStick.xRadius  = (int)Settings.Default["port2AX"];
            Driver.gcn2DZ.analogStick.yRadius  = (int)Settings.Default["port2AY"];
            Driver.gcn2DZ.cStick.xRadius       = (int)Settings.Default["port2CX"];
            Driver.gcn2DZ.cStick.yRadius       = (int)Settings.Default["port2CY"];
            Driver.gcn2DZ.LTrigger.radius      = (int)Settings.Default["port2LT"];
            Driver.gcn2DZ.RTrigger.radius      = (int)Settings.Default["port2RT"];

            Driver.gcn3DZ.analogStick.xRadius  = (int)Settings.Default["port3AX"];
            Driver.gcn3DZ.analogStick.yRadius  = (int)Settings.Default["port3AY"];
            Driver.gcn3DZ.cStick.xRadius       = (int)Settings.Default["port3CX"];
            Driver.gcn3DZ.cStick.yRadius       = (int)Settings.Default["port3CY"];
            Driver.gcn3DZ.LTrigger.radius      = (int)Settings.Default["port3LT"];
            Driver.gcn3DZ.RTrigger.radius      = (int)Settings.Default["port3RT"];

            Driver.gcn4DZ.analogStick.xRadius  = (int)Settings.Default["port4AX"];
            Driver.gcn4DZ.analogStick.yRadius  = (int)Settings.Default["port4AY"];
            Driver.gcn4DZ.cStick.xRadius       = (int)Settings.Default["port4CX"];
            Driver.gcn4DZ.cStick.yRadius       = (int)Settings.Default["port4CY"];
            Driver.gcn4DZ.LTrigger.radius      = (int)Settings.Default["port4LT"];
            Driver.gcn4DZ.RTrigger.radius      = (int)Settings.Default["port4RT"];
        }

        private void BtnFixLibUsb_Click(object sender, EventArgs e)
        {
            if (Driver.run)
            {
                StopProc(null, EventArgs.Empty);
            }
            SystemHelper.RunLibUsbInstall();
        }

        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            btnSave_Click(sender, e);
            btnClose_Click(sender, e);
        }
    }
}
