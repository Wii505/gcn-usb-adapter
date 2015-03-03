using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using GCNUSBFeeder.Properties;

namespace GCNUSBFeeder
{
    partial class Update : Form
    {
        public Update()
        {
            InitializeComponent();
            cbDisableAutoUpdates.Checked = !(bool)Settings.Default["autoUpdate"];
            if (!MainForm.updater.updateAvailable)
            {
                ActiveForm.Close();
                return;
            }
            textBoxDescription.Text = MainForm.updater.response.updateDescription;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            MainForm.updater.GenerateRedirect();
            Application.Exit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
        }

        private void cbDisableAutoUpdates_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default["autoUpdate"] = !cbDisableAutoUpdates.Checked;
            Settings.Default.Save();
        }
    }
}
