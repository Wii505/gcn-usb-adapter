This is a vjoy feeder application written in C# using LibUsbDotNet. 

It allows one to communicate with the WUP-028, Wii U to Gamecube USB adapter.

==================================================================

This driver has been confirmed to work on Windows 7 and Windows 8 so far.

Install Instructions:

-   (Windows 8 only) Restart into "disable driver signature enforcement" mode before proceeding.

-   Plug in your wii U adapter first (black USB end), and let Windows do nothing/fail to install it.

-   Run the Installer (as administrator).

-   You will be asked to install an unsigned driver (by something like this[4] ) click Install this software anyway.

-   You will be prompted to install vJoy. The default options are highly recommended.

-   After that, the installer will generate 4 controllers and populate them with the correct buttons/axes.

-   The provided application will start/stop access to your controllers, and it needs to be started to use them.

-   Analog sticks may need to be calibrated before first use, click the Windows Gamepad Info button for quick access from the application.

-   Dolphin input profile configurations are now available![5] Copy the contents of the zip to "Documents\Dolphin Emulator\Config\Profiles" and then go to GCPad in dolphin and select the profile and load it.

For reference:

*   Z-Axis is the L trigger

*   X-rotation is the C-stick X axis

*   Y-Rotation is the C-stick Y axis

*   Z-rotation is the R trigger

*   When calibrating the triggers, do not make them click (hit the button), it will cause you to go to the next screen and could mess up your calibration.

*   Rumble is not currently supported.

*   Input lag is variable as of the 2014-11-28.5 release. It defaults to 10ms (~2/3 a frame) and can go as low as 5ms (roughly 1/3 frame) or as high as 30ms (just under 2 frames). Faster refresh rates (lower values) will use more processing power but be more responsive.