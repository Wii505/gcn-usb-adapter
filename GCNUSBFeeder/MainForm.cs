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
        Driver mainDriver;
        public MainForm()
        {
            mainDriver = new Driver();
            InitializeComponent();
            Driver.Log += Driver_Log;
            JoystickHelper.Log += Driver_Log;
            FormClosing += MainForm_FormClosing;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }

        private void btnStart_Click(object sender, EventArgs e)
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

        private void btnStop_Click(object sender, EventArgs e)
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
    }
}
