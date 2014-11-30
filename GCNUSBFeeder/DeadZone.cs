using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCNUSBFeeder
{

    public class ControllerDeadZones
    {
        public AnalogDeadZone analogStick;
        public AnalogDeadZone cStick;
        public LinearDeadZone LTrigger;
        public LinearDeadZone RTrigger;

        public ControllerDeadZones()
        {
            analogStick = new AnalogDeadZone();
            cStick = new AnalogDeadZone();
            LTrigger = new LinearDeadZone();
            RTrigger = new LinearDeadZone();
        }
    }

    public class AnalogDeadZone
    {
        public int xRadius;
        public int yRadius;

        public int centerX;
        public int centerY;

        public AnalogDeadZone(int x = 10, int y = 10, int cX = 127, int cY = 127)
        {
            xRadius = x;
            yRadius = y;

            centerX = cX;
            centerY = cY;
        }

        public bool inDeadZone(int x, int y)
        {
            // MATH <3
            double value = (Math.Pow(x - centerX, 2) / Math.Pow(xRadius, 2)) +
                           (Math.Pow(y - centerY, 2) / Math.Pow(yRadius, 2));

            if (value <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public class LinearDeadZone
    {
        public int radius;

        public LinearDeadZone(int r = 10)
        {
            radius = r;
        }

        // This is based on the assumption that linear sticks start at 0
        // and increase in value from there.
        public bool inDeadZone(int val)
        {
            if (val <= radius)
            {
                return true;
            }
            else
            { 
                return false; 
            }
        }
    }
}
