using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCNUSBFeeder
{
    public class GCNState
    {       
        public int analogX;
        public int analogY;

        public int cstickX;
        public int cstickY;

        public int analogL;
        public int analogR;

        public bool A;
        public bool B;
        public bool X;
        public bool Y;
        public bool Z;
        public bool R;
        public bool L;
        public bool start;

        public bool up;
        public bool left;
        public bool down;
        public bool right;

        public int POVstate;

        public GCNState() { }

        public static GCNState GetState(byte[] input)
        {
            //[0] upper end D-Pad, lower end a,b,x,y
            //[1] R button, L Button, z, start
            //[2] analog X
            //[3] analog Y
            //[4] c-stick X
            //[5] c-stick Y
            //[6] L axis
            //[7] R Axis

            //[0] [0]: A
            //    [1]: B
            //    [2]: X
            //    [3]: Y
            //    [4]: Left
            //    [5]: Right
            //    [6]: Down
            //    [7]: Up

            //[1] [0]: start
            //    [1]: z
            //    [2]: R button
            //    [3]: L Button
            //    [4]: not used
            //    [5]: not used
            //    [6]: not used
            //    [7]: not used

            GCNState pad = new GCNState();
            if (input.Length == 8)
            {
                byte b1 = input[0];
                pad.A     = (b1 & (1 << 0)) != 0;
                pad.B     = (b1 & (1 << 1)) != 0;
                pad.X     = (b1 & (1 << 2)) != 0;
                pad.Y     = (b1 & (1 << 3)) != 0;
                
                pad.left  = (b1 & (1 << 4)) != 0;
                pad.right = (b1 & (1 << 5)) != 0;
                pad.down  = (b1 & (1 << 6)) != 0;
                pad.up    = (b1 & (1 << 7)) != 0;

                //Generate POV state for vJoy.
                if      (pad.right) { pad.POVstate =  1; }
                else if (pad.down ) { pad.POVstate =  2; }
                else if (pad.left ) { pad.POVstate =  3; }
                else if (pad.up   ) { pad.POVstate =  0; }
                else                { pad.POVstate = -1; }

                byte b2 = input[1];
                pad.start =  (b2 & (1 << 0)) != 0;
                pad.Z     =  (b2 & (1 << 1)) != 0;
                pad.R     =  (b2 & (1 << 2)) != 0;
                pad.L     =  (b2 & (1 << 3)) != 0;

                pad.analogX = (int)input[2];
                pad.analogY = (int)input[3];
                pad.cstickX = (int)input[4];
                pad.cstickY = (int)input[5];
                pad.analogL = (int)input[6];
                pad.analogR = (int)input[7];
                return pad;
            }
            else
            {
                throw new Exception("Invalid byte array for input");
            }
        }
    }
}
