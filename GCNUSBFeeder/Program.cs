using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows.Forms;

namespace GCNUSBFeeder
{
    static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());            
        }
    }
}
