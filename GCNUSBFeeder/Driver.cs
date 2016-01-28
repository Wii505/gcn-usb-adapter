using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibUsbDotNet;
using LibUsbDotNet.Main;

//using MadWizard.WinUSBNet;
using vJoyInterfaceWrap;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;


namespace GCNUSBFeeder
{
    public class Driver
    {
        public static event EventHandler<LogEventArgs> Log;
        public static bool run = false;

        public static bool noEventMode = true;

        public static ControllerDeadZones gcn1DZ;
        public static ControllerDeadZones gcn2DZ;
        public static ControllerDeadZones gcn3DZ;
        public static ControllerDeadZones gcn4DZ;

        public static bool gcn1Enabled = false;
        public static bool gcn2Enabled = false;
        public static bool gcn3Enabled = false;
        public static bool gcn4Enabled = false;

        private vJoy vjoy = new vJoy();

        private bool gcn1ok = false;
        private bool gcn2ok = false;
        private bool gcn3ok = false;
        private bool gcn4ok = false;

        private long gcn1Ffb = 0;
        private long gcn2Ffb = 0;
        private long gcn3Ffb = 0;
        private long gcn4Ffb = 0;

        private int gcn1FfbDur = 150;
        private int gcn2FfbDur = 150;
        private int gcn3FfbDur = 150;
        private int gcn4FfbDur = 150;

        private bool gcn1FfbInf = false;
        private bool gcn2FfbInf = false;
        private bool gcn3FfbInf = false;
        private bool gcn4FfbInf = false;

        private bool gcn1FfbActive = false;
        private bool gcn2FfbActive = false;
        private bool gcn3FfbActive = false;
        private bool gcn4FfbActive = false;

        public Driver()
        {
            gcn1DZ = new ControllerDeadZones();
            gcn2DZ = new ControllerDeadZones();
            gcn3DZ = new ControllerDeadZones();
            gcn4DZ = new ControllerDeadZones();
        }

        UsbEndpointReader reader = null;
        UsbEndpointWriter writer = null;
        UsbDevice GCNAdapter = null;
        IUsbDevice wholeGCNAdapter = null;

        public void Start()
        {
            //WUP-028
            //VENDORID 0x57E
            //PRODUCT ID 0x337
            
            var USBFinder = new UsbDeviceFinder(0x057E, 0x0337);
            GCNAdapter = UsbDevice.OpenUsbDevice(USBFinder);

            if (GCNAdapter != null)
            {
                int transferLength;

                reader = GCNAdapter.OpenEndpointReader(ReadEndpointID.Ep01);
                writer = GCNAdapter.OpenEndpointWriter(WriteEndpointID.Ep02);

                //prompt controller to start sending
                writer.Write(Convert.ToByte((char)19), 10, out transferLength);

                try
                {
                    if (gcn1Enabled && !JoystickHelper.checkJoystick(ref vjoy, 1)) { SystemHelper.CreateJoystick(1); }
                    if (gcn2Enabled && !JoystickHelper.checkJoystick(ref vjoy, 2)) { SystemHelper.CreateJoystick(2); }
                    if (gcn3Enabled && !JoystickHelper.checkJoystick(ref vjoy, 3)) { SystemHelper.CreateJoystick(3); }
                    if (gcn4Enabled && !JoystickHelper.checkJoystick(ref vjoy, 4)) { SystemHelper.CreateJoystick(4); }

                    if (gcn1Enabled && vjoy.AcquireVJD(1))
                        gcn1ok = true;
                    if (gcn2Enabled && vjoy.AcquireVJD(2))
                        gcn2ok = true;
                    if (gcn3Enabled && vjoy.AcquireVJD(3))
                        gcn3ok = true;
                    if (gcn4Enabled && vjoy.AcquireVJD(4))
                        gcn4ok = true;

                    vjoy.ResetAll();
                    vjoy.FfbRegisterGenCB(FfbRecieved, IntPtr.Zero);
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

                if (noEventMode)
                {
                    // PORT 1: bytes 02-09
                    // PORT 2: bytes 11-17
                    // PORT 3: bytes 20-27
                    // PORT 4: bytes 29-36l
                    byte[] ReadBuffer = new byte[37]; // 32 (4 players x 8) bytes for input, 5 bytes for formatting
                    byte[] WriteBuffer = new byte[5]; // 1 for command, 4 for rumble state
                    WriteBuffer[0] = 0x11;
                    WriteBuffer[1] = 0;
                    Log(null, new LogEventArgs("Driver successfully started, entering input loop."));
                    run = true;
                    Stopwatch sw = Stopwatch.StartNew();
                    while (run)
                    {
                        var ec = reader.Read(ReadBuffer, 10, out transferLength);
                        var input1 = GCNState.GetState(getFastInput1(ref ReadBuffer));
                        var input2 = GCNState.GetState(getFastInput2(ref ReadBuffer));
                        var input3 = GCNState.GetState(getFastInput3(ref ReadBuffer));
                        var input4 = GCNState.GetState(getFastInput4(ref ReadBuffer));

                        if (gcn1ok) { JoystickHelper.setJoystick(ref vjoy, input1, 1, gcn1DZ); }
                        if (gcn2ok) { JoystickHelper.setJoystick(ref vjoy, input2, 2, gcn2DZ); }
                        if (gcn3ok) { JoystickHelper.setJoystick(ref vjoy, input3, 3, gcn3DZ); }
                        if (gcn4ok) { JoystickHelper.setJoystick(ref vjoy, input4, 4, gcn4DZ); }

                        long elapsed = sw.ElapsedMilliseconds;
                        sw.Reset(); sw.Start();

                        if (Interlocked.Read(ref gcn1Ffb) > 0)
                        {
                            if (gcn1FfbInf == false)
                                Interlocked.Add(ref gcn1Ffb, -elapsed);
                            WriteBuffer[1] = (byte)(gcn1FfbActive ? 1 : 0);
                        }
                        else
                            WriteBuffer[1] = 0;
                        if (Interlocked.Read(ref gcn2Ffb) > 0)
                        {
                            if (gcn2FfbInf == false)
                                Interlocked.Add(ref gcn2Ffb, -elapsed);
                            WriteBuffer[2] = (byte)(gcn2FfbActive ? 1 : 0);
                        }
                        else
                            WriteBuffer[2] = 0;
                        if (Interlocked.Read(ref gcn3Ffb) > 0)
                        {
                            if (gcn3FfbInf == false)
                                Interlocked.Add(ref gcn3Ffb, -elapsed);
                            WriteBuffer[3] = (byte)(gcn3FfbActive ? 1 : 0);
                        }
                        else
                            WriteBuffer[3] = 0;
                        if (Interlocked.Read(ref gcn4Ffb) > 0)
                        {
                            if (gcn4FfbInf == false)
                                Interlocked.Add(ref gcn4Ffb, -elapsed);
                            WriteBuffer[4] = (byte)(gcn4FfbActive ? 1 : 0);
                        }
                        else
                            WriteBuffer[4] = 0;
                        writer.Write(WriteBuffer, 10, out transferLength);
                        System.Threading.Thread.Sleep(5);
                    }

                    WriteBuffer[1] = 0; WriteBuffer[2] = 0; WriteBuffer[3] = 0; WriteBuffer[4] = 0;
                    writer.Write(WriteBuffer, 10, out transferLength);

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
                else
                {
                    Log(null, new LogEventArgs("Driver successfully started, entering input loop."));
                    //using  Interrupt request instead of looping behavior.
                    reader.DataReceivedEnabled = true;
                    reader.DataReceived += reader_DataReceived;
                    reader.ReadBufferSize = 37;
                    reader.ReadThreadPriority = System.Threading.ThreadPriority.Highest;
                    run = true;
                }
            }
            else
            {
                Log(null, new LogEventArgs("GCN Adapter not detected."));
                Driver.run = false;
            }
        }

        #region input parsing
        //Ugly, but faster than linq, at the very least.
        private byte[] getFastInput1(ref byte[] input)
        {
            return new byte[] { input[1], input[2], input[3], input[4], input[5], input[6], input[7], input[8], input[9] };
        }
        private byte[] getFastInput2(ref byte[] input)
        {
            return new byte[] { input[10], input[11], input[12], input[13], input[14], input[15], input[16], input[17], input[18] };
        }
        private byte[] getFastInput3(ref byte[] input)
        {
            return new byte[] { input[19], input[20], input[21], input[22], input[23], input[24], input[25], input[26], input[27] };
        }
        private byte[] getFastInput4(ref byte[] input)
        {
            return new byte[] { input[28], input[29], input[30], input[31], input[32], input[33], input[34], input[35], input[36] };
        }
        #endregion

        public void reader_DataReceived(object sender, EndpointDataEventArgs e)
        {
            if (run)
            {
                var data = e.Buffer;
                var input1 = GCNState.GetState(getFastInput1(ref data));
                var input2 = GCNState.GetState(getFastInput2(ref data));
                var input3 = GCNState.GetState(getFastInput3(ref data));
                var input4 = GCNState.GetState(getFastInput4(ref data));

                if (gcn1ok) { JoystickHelper.setJoystick(ref vjoy, input1, 1, gcn1DZ); }
                if (gcn2ok) { JoystickHelper.setJoystick(ref vjoy, input2, 2, gcn2DZ); }
                if (gcn3ok) { JoystickHelper.setJoystick(ref vjoy, input3, 3, gcn3DZ); }
                if (gcn4ok) { JoystickHelper.setJoystick(ref vjoy, input4, 4, gcn4DZ); }
            }
            else
            {
                reader.DataReceivedEnabled = false;

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

        void FfbRecieved(IntPtr data, IntPtr userData)
        {
            int devId = 0;
            FFBPType someT = new FFBPType();
            if (vjoy.Ffb_h_DeviceID(data, ref devId) == 0 &&
                vjoy.Ffb_h_Type(data, ref someT) == 0)
            {
                if (someT == FFBPType.PT_EFOPREP)
                {
                    vJoy.FFB_EFF_OP a = new vJoy.FFB_EFF_OP();
                    vjoy.Ffb_h_EffOp(data, ref a);
                    if (devId == 1)
                    {
                        if (a.EffectOp == FFBOP.EFF_STOP)
                            Interlocked.Exchange(ref gcn1Ffb, 0);
                        else
                            Interlocked.Exchange(ref gcn1Ffb, gcn1FfbDur * a.LoopCount);
                    }
                    else if (devId == 2)
                    {
                        if (a.EffectOp == FFBOP.EFF_STOP)
                            Interlocked.Exchange(ref gcn2Ffb, 0);
                        else
                            Interlocked.Exchange(ref gcn2Ffb, gcn2FfbDur * a.LoopCount);
                    }
                    else if (devId == 3)
                    {
                        if (a.EffectOp == FFBOP.EFF_STOP)
                            Interlocked.Exchange(ref gcn3Ffb, 0);
                        else
                            Interlocked.Exchange(ref gcn3Ffb, gcn3FfbDur * a.LoopCount);
                    }
                    else if (devId == 4)
                    {
                        if (a.EffectOp == FFBOP.EFF_STOP)
                            Interlocked.Exchange(ref gcn4Ffb, 0);
                        else
                            Interlocked.Exchange(ref gcn4Ffb, gcn4FfbDur * a.LoopCount);
                    }
                }
                else if (someT == FFBPType.PT_EFFREP)
                {
                    vJoy.FFB_EFF_REPORT b = new vJoy.FFB_EFF_REPORT();
                    vjoy.Ffb_h_Eff_Report(data, ref b);
                    if (devId == 1)
                    {
                        gcn1FfbDur = b.Duration;
                        if (b.Duration == 0xFFFF)
                            gcn1FfbInf = true;
                        else
                            gcn1FfbInf = false;
                    }
                    else if (devId == 2)
                    {
                        gcn2FfbDur = b.Duration;
                        if (b.Duration == 0xFFFF)
                            gcn2FfbInf = true;
                        else
                            gcn2FfbInf = false;
                    }
                    else if (devId == 3)
                    {
                        gcn3FfbDur = b.Duration;
                        if (b.Duration == 0xFFFF)
                            gcn3FfbInf = true;
                        else
                            gcn3FfbInf = false;
                    }
                    else if (devId == 4)
                    {
                        gcn4FfbDur = b.Duration;
                        if (b.Duration == 0xFFFF)
                            gcn4FfbInf = true;
                        else
                            gcn4FfbInf = false;
                    }
                }
                else if(someT == FFBPType.PT_CTRLREP)
                {
                    FFB_CTRL ctrl = new FFB_CTRL();
                    vjoy.Ffb_h_DevCtrl(data, ref ctrl);
                    if(ctrl == FFB_CTRL.CTRL_DEVRST || ctrl == FFB_CTRL.CTRL_STOPALL)
                    {
                        if (devId == 1)
                            Interlocked.Exchange(ref gcn1Ffb, 0);
                        else if (devId == 2)
                            Interlocked.Exchange(ref gcn2Ffb, 0);
                        else if (devId == 3)
                            Interlocked.Exchange(ref gcn3Ffb, 0);
                        else if (devId == 4)
                            Interlocked.Exchange(ref gcn4Ffb, 0);
                    }
                    //else
                    //    Log(null, new LogEventArgs("Unimplemented CTRL: " + ctrl));
                }
                else if(someT == FFBPType.PT_PRIDREP)
                {
                    vJoy.FFB_EFF_PERIOD a = new vJoy.FFB_EFF_PERIOD();
                    if (vjoy.Ffb_h_Eff_Period(data, ref a) != 0)
                        Log(null, new LogEventArgs("Ffb_h_Eff_Period error"));
                    else
                    {
                        if (devId == 1)
                            gcn1FfbActive = (a.Period > 0 && a.Magnitude > 0);
                        else if (devId == 2)
                            gcn2FfbActive = (a.Period > 0 && a.Magnitude > 0);
                        else if (devId == 3)
                            gcn3FfbActive = (a.Period > 0 && a.Magnitude > 0);
                        else if (devId == 4)
                            gcn4FfbActive = (a.Period > 0 && a.Magnitude > 0);
                    }
                    //Log(null, new LogEventArgs(" " + a.Magnitude + " " + a.Period + " " + a.Phase));
                }
                else if(someT == FFBPType.PT_GAINREP)
                {
                    byte gain = new byte();
                    if (vjoy.Ffb_h_DevGain(data, ref gain) != 0)
                        Log(null, new LogEventArgs("Ffb_h_DevGain error"));
                }
                else if(someT == FFBPType.PT_NEWEFREP)
                {
                    FFBEType a = new FFBEType();
                    if (vjoy.Ffb_h_EffNew(data, ref a) != 0)
                        Log(null, new LogEventArgs("Ffb_h_EffNew error"));
                }
                else if(someT == FFBPType.PT_BLKFRREP || someT == FFBPType.PT_BLKLDREP)
                {
                    int index = 0;
                    if (vjoy.Ffb_h_EBI(data, ref index) != 0)
                        Log(null, new LogEventArgs("Ffb_h_EBI error"));
                }
                else if(someT == FFBPType.PT_CONSTREP)
                {
                    vJoy.FFB_EFF_CONSTANT a = new vJoy.FFB_EFF_CONSTANT();
                    if(vjoy.Ffb_h_Eff_Constant(data, ref a) != 0)
                        Log(null, new LogEventArgs("Ffb_h_Eff_Constant error"));
                    else
                    {
                        if (devId == 1)
                            gcn1FfbActive = (a.Magnitude != 0);
                        else if (devId == 2)
                            gcn2FfbActive = (a.Magnitude != 0);
                        else if (devId == 3)
                            gcn3FfbActive = (a.Magnitude != 0);
                        else if (devId == 4)
                            gcn4FfbActive = (a.Magnitude != 0);
                    }
                }
                else if(someT == FFBPType.PT_RAMPREP)
                {
                    vJoy.FFB_EFF_RAMP a = new vJoy.FFB_EFF_RAMP();
                    if (vjoy.Ffb_h_Eff_Ramp(data, ref a) != 0)
                        Log(null, new LogEventArgs("Ffb_h_Eff_Ramp error"));
                    else
                    {
                        if (devId == 1)
                            gcn1FfbActive = (a.Start != 0 || a.End != 0);
                        else if (devId == 2)
                            gcn2FfbActive = (a.Start != 0 || a.End != 0);
                        else if (devId == 3)
                            gcn3FfbActive = (a.Start != 0 || a.End != 0);
                        else if (devId == 4)
                            gcn4FfbActive = (a.Start != 0 || a.End != 0);
                    }
                }
                //else
                //    Log(null, new LogEventArgs("Unimplemented force feedback command: " + someT));
            }
        }
    }
}
