This is a vjoy feeder application written in C# using LibUsbDotNet. 

It allows for communication with the WUP-028 model of the Wii U to Gamecube USB adapter.

For reference:

*   Z-Axis is the L trigger

*   X-rotation is the C-stick X axis

*   Y-Rotation is the C-stick Y axis

*   Z-rotation is the R trigger

*   When calibrating the triggers, do not make them click (hit the button), it will cause you to go to the next screen and could mess up your calibration.

*   Rumble is not currently supported.

*   Input lag is variable as of the 2014-11-28.5 release. It defaults to 10ms (~2/3 a frame) and can go as low as 5ms (roughly 1/3 frame) or as high as 30ms (just under 2 frames). Faster refresh rates (lower values) will use more processing power but be more responsive.