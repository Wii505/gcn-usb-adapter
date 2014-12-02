using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibUsbDotNet;
using LibUsbDotNet.Main;
using vJoyInterfaceWrap;


namespace GCNUSBFeeder
{
    public class Driver
    {
        public static event EventHandler<LogEventArgs> Log;
        public static bool run = false;
        public static int refreshRate = 10;

        public static ControllerDeadZones gcn1DZ;
        public static ControllerDeadZones gcn2DZ;
        public static ControllerDeadZones gcn3DZ;
        public static ControllerDeadZones gcn4DZ;

        public Driver()
        {
            gcn1DZ = new ControllerDeadZones();
            gcn2DZ = new ControllerDeadZones();
            gcn3DZ = new ControllerDeadZones();
            gcn4DZ = new ControllerDeadZones();
        }

        public void Start()
        {
            //WUP-028
            //VENDORID 0x57E
            //PRODUCT ID 0x337
            var USBFinder = new UsbDeviceFinder(0x57E, 0x337);
            var GCNAdapter = UsbDevice.OpenUsbDevice(USBFinder);

            UsbEndpointReader reader = null;
            UsbEndpointWriter writer = null;

            if (GCNAdapter != null)
            {
                IUsbDevice wholeGCNAdapter = (IUsbDevice)GCNAdapter;
                if (!ReferenceEquals(wholeGCNAdapter, null))
                {
                    wholeGCNAdapter.SetConfiguration(1);
                    wholeGCNAdapter.ClaimInterface(0);

                    byte[] WriteBuffer = new byte[100];
                    int transferLength;
                    var URB = new UsbSetupPacket(0x21, 11, 0x0001, 0, 0);

                    //activate controller
                    bool success = false;
                    int loop = 0;
                    while (!success && loop < 10)
                    {
                        success = GCNAdapter.ControlTransfer(ref URB, WriteBuffer, 0, out transferLength);
                        loop++;
                    }

                    if (!success) return;

                    reader = GCNAdapter.OpenEndpointReader(ReadEndpointID.Ep01);
                    writer = GCNAdapter.OpenEndpointWriter(WriteEndpointID.Ep02);

                    //prompt controller to start sending
                    writer.Write(Convert.ToByte((char)19), 10, out transferLength);

                    vJoy gcn1 = new vJoy();
                    vJoy gcn2 = new vJoy();
                    vJoy gcn3 = new vJoy();
                    vJoy gcn4 = new vJoy();

                    bool gcn1ok = false;
                    bool gcn2ok = false;
                    bool gcn3ok = false;
                    bool gcn4ok = false;

                    try
                    {
                        if (JoystickHelper.checkJoystick(ref gcn1, 1) && gcn1.AcquireVJD(1))
                        {
                            gcn1ok = true;
                            gcn1.ResetAll();
                        }
                        if (JoystickHelper.checkJoystick(ref gcn2, 2) && gcn2.AcquireVJD(2))
                        {
                            gcn2ok = true;
                            gcn2.ResetAll();
                        }
                        if (JoystickHelper.checkJoystick(ref gcn3, 3) && gcn3.AcquireVJD(3))
                        {
                            gcn3ok = true;
                            gcn3.ResetAll();
                        }
                        if (JoystickHelper.checkJoystick(ref gcn4, 4) && gcn4.AcquireVJD(4))
                        {
                            gcn4ok = true;
                            gcn4.ResetAll();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("HRESULT: 0x8007000B"))
                        {
                          Log(null, new LogEventArgs("Error: vJoy driver mismatch. Did you install the wrong version (x86/x64)?"));
                          Driver.run = false;
                          return;
                        }
                    }

                    if(!(gcn1ok && 
                         gcn2ok &&
                         gcn3ok &&
                         gcn4ok))
                    {
                        Log(null, new LogEventArgs("Warning: Gamepads may not be configured properly. (try Configuration > ConfigureJoysticks.bat)"));
                    }

                    // PORT 1: bytes 02-09
                    // PORT 2: bytes 11-17
                    // PORT 3: bytes 20-27
                    // PORT 4: bytes 29-36l
                    byte[] ReadBuffer = new byte[37]; // 32 (4 players x 8) bytes for input, 5 bytes for formatting

                    Log(null, new LogEventArgs("Driver successfully started, entering input loop."));
                    run = true;
                    while (run)
                    {
                        var ec = reader.Read(ReadBuffer, 10, out transferLength);
                        var input1 = GCNState.GetState(getFastInput1(ref ReadBuffer));
                        var input2 = GCNState.GetState(getFastInput2(ref ReadBuffer));
                        var input3 = GCNState.GetState(getFastInput3(ref ReadBuffer));
                        var input4 = GCNState.GetState(getFastInput4(ref ReadBuffer));

                        if (gcn1ok) { JoystickHelper.setJoystick(ref gcn1, input1, 1, gcn1DZ); }
                        if (gcn2ok) { JoystickHelper.setJoystick(ref gcn2, input2, 2, gcn2DZ); }
                        if (gcn3ok) { JoystickHelper.setJoystick(ref gcn3, input3, 3, gcn3DZ); }
                        if (gcn4ok) { JoystickHelper.setJoystick(ref gcn4, input4, 4, gcn4DZ); }

                        System.Threading.Thread.Sleep(refreshRate);
                    }

                    if (GCNAdapter != null)
                    {
                        if (GCNAdapter.IsOpen)
                        {
                            if (!ReferenceEquals(wholeGCNAdapter, null))
                            {
                                wholeGCNAdapter.ReleaseInterface(0);
                            }
                            GCNAdapter.Close();
                        }
                        GCNAdapter = null;
                        UsbDevice.Exit();
                        Log(null, new LogEventArgs("Closing driver thread..."));
                    }
                    Log(null, new LogEventArgs("Driver thread has been stopped."));
                }
            }
            else
            {
                Log(null, new LogEventArgs("GCN Adapter not detected."));
                Driver.run = false;
            }
        }

        //Ugly, but faster than linq, at the very least.
        private byte[] getFastInput1(ref byte[] input)
        {
            return new byte[] { input[2], input[3], input[4], input[5], input[6], input[7], input[8], input[9] };
        }
        private byte[] getFastInput2(ref byte[] input)
        {
            return new byte[] { input[11], input[12], input[13], input[14], input[15], input[16], input[17], input[18] };
        }
        private byte[] getFastInput3(ref byte[] input)
        {
            return new byte[] { input[20], input[21], input[22], input[23], input[24], input[25], input[26], input[27] };
        }
        private byte[] getFastInput4(ref byte[] input)
        {
            return new byte[] { input[29], input[30], input[31], input[32], input[33], input[34], input[35], input[36] };
        }

        public class LogEventArgs : EventArgs
        {
            public LogEventArgs(string text = "")
            {
                _text = text;
            }

            private string _text;
            public string Text
            {
                get { return _text; }
                set { _text = value; }
            }
        }
    }
}
