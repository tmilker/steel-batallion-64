using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace myVJoyWrapper
{

    public enum VjdStat
    {
        //status can be one of the following values:
        VJD_STAT_OWN, // The vJoy Device is owned by this application.
        VJD_STAT_FREE, // The vJoy Device is NOT owned by any application (including this one).
        VJD_STAT_BUSY, // The vJoy Device is owned by another application.
        // It cannot be acquired by this application.
        VJD_STAT_MISS, // The vJoy Device is missing. It either does not exist or the driver is down.
        VJD_STAT_UNKN, // Unknown
    }

    public enum HID_USAGES
    {
         HID_USAGE_X       =   0x30,
         HID_USAGE_Y       =	0x31,
         HID_USAGE_Z       =   0x32,
         HID_USAGE_RX      =   0x33,
         HID_USAGE_RY      =   0x34,
         HID_USAGE_RZ      =   0x35,
         HID_USAGE_SL0     =   0x36,
         HID_USAGE_SL1     =   0x37,
         HID_USAGE_WHL     =   0x38,
         HID_USAGE_POV     =   0x39,
    }



// Usage example:
//	JOYSTICK_POSITION iReport;
//	:
//	DeviceIoControl (hDevice, 100, &iReport, sizeof(HID_INPUT_REPORT), NULL, 0, &bytes, NULL)
/*typedef struct _JOYSTICK_POSITION
{
	BYTE	bDevice;	// Index of device. 1-based.
	LONG	wThrottle;
	LONG	wRudder;
	LONG	wAileron;
	LONG	wAxisX;
	LONG	wAxisY;
	LONG	wAxisZ;
	LONG	wAxisXRot;
	LONG	wAxisYRot;
	LONG	wAxisZRot;
	LONG	wSlider;
	LONG	wDial;
	LONG	wWheel;
	LONG	wAxisVX;
	LONG	wAxisVY;
	LONG	wAxisVZ;
	LONG	wAxisVBRX;
	LONG	wAxisVBRY;
	LONG	wAxisVBRZ;
	LONG	lButtons;	// 32 buttons: 0x00000001 means button1 is pressed, 0x80000000 -> button32 is pressed
	DWORD	bHats;		// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
	DWORD	bHatsEx1;	// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
	DWORD	bHatsEx2;	// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
	DWORD	bHatsEx3;	// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
} JOYSTICK_POSITION, *PJOYSTICK_POSITION;*/

    public class vJoy
    {


        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct JoystickState
        {
            public Byte bDevice;
            public UInt32 Throttle;
            public UInt32 Rudder;
            public UInt32 Aileron;
            public UInt32 AxisX;
            public UInt32 AxisY;
            public UInt32 AxisZ;
            public UInt32 AxisXRot;
            public UInt32 AxisYRot;
            public UInt32 AxisZRot;
            public UInt32 Slider;
            public UInt32 Dial;
            public UInt32 Wheel;
            public UInt32 AxisVX;
            public UInt32 AxisVY;
            public UInt32 AxisVZ;
            public UInt32 AxisVBRX;
            public UInt32 AxisVBRY;
            public UInt32 AxisVBRZ;
            public UInt32 Buttons;
            public UInt32 bHats;		// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
            public UInt32 bHatsEx1;	// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
            public UInt32 bHatsEx2;	// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
            public UInt32 bHatsEx3;	// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
        } ;

        [DllImport("vJoyInterface.dll")]
        private static extern bool vJoyEnabled();

        [DllImport("vJoyInterface.dll")]
        private static extern short GetvJoyVersion();
        
        [DllImport("vJoyInterface.dll")]
        private static extern IntPtr GetvJoyProductString();

        [DllImport("vJoyInterface.dll")]
        private static extern IntPtr GetvJoyManufacturerString();

        [DllImport("vJoyInterface.dll")]
        private static extern IntPtr GetvJoySerialNumberString();

        [DllImport("vJoyInterface.dll")]
        private static extern int GetVJDStatus(UInt32 rID);

        [DllImport("vJoyInterface.dll")]
        private static extern bool AcquireVJD(UInt32 rID);

        [DllImport("vJoyInterface.dll")]
        private static extern void RelinquishVJD(UInt32 rID);

        [DllImport("vJoyInterface.dll")]
        private static extern bool UpdateVJD(UInt32 rID, IntPtr PVOID);

        [DllImport("vJoyInterface.dll")]
        private static extern int GetVJDButtonNumber(UInt32 rID);

        [DllImport("vJoyInterface.dll")]
        private static extern int GetVJDDiscPovNumber(UInt32 rID);

        [DllImport("vJoyInterface.dll")]
        private static extern int GetVJDContPovNumber(UInt32 rID);

        [DllImport("vJoyInterface.dll")]
        private static extern bool GetVJDAxisExist(UInt32 rID, UInt32 Axis);


        [DllImport("vJoyInterface.dll")]
        private static extern bool SetBtn(bool Value, UInt32 rID, char nBtn);


        
        [DllImport("vJoyInterface.dll")]
        private static extern bool SetAxis(UInt32 Value, UInt32 rID, UInt32 Axis);

        bool enabledCalled = false;

        private void checkEnabled()
        {
            if(!enabledCalled)
            {
                vJoyEnabled();
                enabledCalled = true;
            }
        }

        public bool isEnabled()
        {
            return vJoyEnabled();
        }

        public string getProductString()
        {
            checkEnabled();
            return Marshal.PtrToStringAuto(GetvJoyProductString());
        }

        public int getVersion()
        {
            checkEnabled();
            return (int)GetvJoyVersion();
        }

        public string getManufacturerString()
        {
            checkEnabled();
            return Marshal.PtrToStringAuto(GetvJoyManufacturerString());
        }

        public string getSerialNumberString()
        {
            checkEnabled();
            return Marshal.PtrToStringAuto(GetvJoySerialNumberString());
        }

        public VjdStat getVJDStatus(int rID)
        {
            return (VjdStat)GetVJDStatus((UInt32)rID);
        }

        public bool acquireVJD(int rID)
        {
            return AcquireVJD((UInt32)rID);
        }

        public void relinquishVJD(int rID)
        {
            if (GetVJDStatus((UInt32)rID) == (UInt32)VjdStat.VJD_STAT_OWN)
                RelinquishVJD((UInt32)rID);
        }

        public int joystickStateSize()
        {
            JoystickState example = new JoystickState();
            return System.Runtime.InteropServices.Marshal.SizeOf(example);
            
        }

        public bool setThrottle(int rID, int value)
        {
            JoystickState example = new JoystickState();
            example.bDevice = 1;
            example.AxisX = (uint) value;
            example.Buttons = 255;

            IntPtr unmanagedPointer = Marshal.AllocHGlobal(96);
            Marshal.StructureToPtr(example, unmanagedPointer, false);

            return UpdateVJD((UInt32)rID,unmanagedPointer);
        }

        public bool setAxis(int rID, int value, uint axis)
        {
            return SetAxis((uint) value, (uint) rID,(uint) axis);
        }

        public int getTotalButtons(int rID)
        {
            return GetVJDButtonNumber((uint)rID);
        }

        public int getTotalDiscretePOVs(int rID)
        {
            return GetVJDDiscPovNumber((uint)rID);
        }

        public int getTotalContinuousPOVs(int rID)
        {
            return GetVJDContPovNumber((uint)rID);
        }

        public bool hasAxis(int rID, HID_USAGES Axis)
        {
            return GetVJDAxisExist((uint)rID, (uint)Axis);
        }

        public bool setButton(bool value, int rID, int button)
        {
            return SetBtn(value, (uint)rID, (char)button);
        }







    }
}
