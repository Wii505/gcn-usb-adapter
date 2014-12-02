using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace GCNUSBFeeder
{
    public partial class MainForm : Form
    {
        public static bool autoStart = false;
        public static bool startInTray = false;
        Driver mainDriver;
        public MainForm()
        {
            mainDriver = new Driver();
            InitializeComponent();
            Driver.Log             += Driver_Log;
            JoystickHelper.Log     += Driver_Log;
            SystemHelper.Log       += Driver_Log;
            Configuration.Log      += Driver_Log;
            Configuration.StopProc += btnStop_Click;
            this.FormClosing       += MainForm_FormClosing;
            this.FormClosed        += MainForm_FormClosed; //required since we're starting minimzed now
            
            Configuration.PropogateSettings();
            SystemHelper.checkForMissingDrivers();

            if (startInTray)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
            if (autoStart) 
            { 
                btnStart_Click(null, EventArgs.Empty); 
            }

        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.Show();
        }

        public void btnStart_Click(object sender, EventArgs e)
        {
            if (!Driver.run)
            {
                Driver.run = true;
                var threadDelegate = new ThreadStart(mainDriver.Start);
                Thread t = new Thread(threadDelegate);
                Driver_Log(null, new Driver.LogEventArgs("Starting Driver."));
                t.Start();
                trayIcon.Text = "GCN USB Adapter - Started";
            }
            else
            {
                Driver_Log(null, new Driver.LogEventArgs("Driver is already started.\n"));
            }
        }

        public void btnStop_Click(object sender, EventArgs e)
        {
            if (Driver.run)
            {
                Driver.run = false;
                Driver_Log(null, new Driver.LogEventArgs("Stopping Driver."));
                trayIcon.Text = "GCN USB Adapter - Stopped";
            }
            else
            {
                Driver_Log(null, new Driver.LogEventArgs("Driver is not running.\n"));
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnStart_Click(sender, e);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnStop_Click(sender, e);
        }

        private void closeToTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private bool exit = false;
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnStop_Click(sender, e);
            exit = true;
            this.Close();
        }

        private void Driver_Log(object sender, Driver.LogEventArgs e)
        {
            //Invoke to talk safely across threads
            if (InvokeRequired)
            {
                EventHandler<Driver.LogEventArgs> hnd = new EventHandler<Driver.LogEventArgs>(Driver_Log);
                Invoke(hnd, new object[] { sender, e });
                return;
            }
            txtLog.Text += "\n" + e.Text;
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                this.Show();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!exit && Driver.run)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }


        private void btnGamepadInfo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Joy.cpl");
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trayIcon_DoubleClick(sender, e);
        }

        private void startToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            btnStart_Click(sender, e);
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            btnStop_Click(sender, e);
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            exitToolStripMenuItem_Click(sender, e);
        }

        private void configBtn_Click(object sender, EventArgs e)
        {
            Configuration c = new Configuration();
            c.Show();
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            configBtn_Click(sender, e);
        }

        private void configurationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            configBtn_Click(sender, e);
        }
    }
}
