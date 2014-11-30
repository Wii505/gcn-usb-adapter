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

                    // PORT 1: bytes 02-09
                    // PORT 2: bytes 11-17
                    // PORT 3: bytes 20-27
                    // PORT 4: bytes 29-36
                    byte[] ReadBuffer = new byte[37]; // 32 (4 players x 8) bytes for input, 5 bytes for formatting

                    Log(null, new LogEventArgs("Driver successfully started, entering input loop."));
                    run = true;
                    while (run)
                    {
                        var ec = reader.Read(ReadBuffer, 10, out transferLength);
                        var input1 = GCNState.GetState(ReadBuffer.Skip(2).Take(8).ToArray());
                        var input2 = GCNState.GetState(ReadBuffer.Skip(11).Take(8).ToArray());
                        var input3 = GCNState.GetState(ReadBuffer.Skip(20).Take(8).ToArray());
                        var input4 = GCNState.GetState(ReadBuffer.Skip(29).Take(8).ToArray());

                        if (gcn1ok) { JoystickHelper.setJoystick(ref gcn1, input1, 1, gcn1DZ); }
                        if (gcn2ok) { JoystickHelper.setJoystick(ref gcn2, input2, 2, gcn2DZ); }
                        if (gcn3ok) { JoystickHelper.setJoystick(ref gcn3, input3, 3, gcn3DZ); }
                        if (gcn4ok) { JoystickHelper.setJoystick(ref gcn4, input4, 4, gcn4DZ); }

                        //wait before rechecking to prevent performance problems.
                        System.Threading.Thread.Sleep(refreshRate);
                    }

                    //If we never leave the while loop, we never need this.
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
                Log(null, new LogEventArgs("GCN Adapter not detected"));
                Driver.run = false;
            }
        }

        public class LogEventArgs : EventArgs
        {
            public LogEventArgs(string text="")
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
