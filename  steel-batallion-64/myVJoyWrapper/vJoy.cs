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

        private const int maxDevices = 16;
        
        private struct JoystickState
        {
            public byte bDevice;
            public Int32 Throttle;
            public Int32 Rudder;
            public Int32 Aileron;
            public Int32 AxisX;
            public Int32 AxisY;
            public Int32 AxisZ;
            public Int32 AxisXRot;
            public Int32 AxisYRot;
            public Int32 AxisZRot;
            public Int32 Slider;
            public Int32 Dial;
            public Int32 Wheel;
            public Int32 AxisVX;
            public Int32 AxisVY;
            public Int32 AxisVZ;
            public Int32 AxisVBRX;
            public Int32 AxisVBRY;
            public Int32 AxisVBRZ;
            public Int32 Buttons;
            public UInt32 bHats;		// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
            public UInt32 bHatsEx1;	// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
            public UInt32 bHatsEx2;	// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
            public UInt32 bHatsEx3;	// Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
        };

        JoystickState[] currentState = new JoystickState[maxDevices];

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

        /* ALL THESE ARE made public */
        [DllImport("vJoyInterface.dll")]
        public static extern bool ResetVJD(UInt32 rID);
        
        [DllImport("vJoyInterface.dll")]
        public static extern bool ResetAll();

        [DllImport("vJoyInterface.dll")]
        public static extern bool ResetButtons(UInt32 rID);

        [DllImport("vJoyInterface.dll")]
        public static extern bool ResetPovs(UInt32 rID);
       
        [DllImport("vJoyInterface.dll")]
        private static extern bool SetAxis(Int32 Value, UInt32 rID, UInt32 Axis);

        [DllImport("vJoyInterface.dll")]
        private static extern bool SetBtn(bool Value, UInt32 rID, char nBtn);

        [DllImport("vJoyInterface.dll")]
        private static extern bool SetDiscPov(Int32 Value, UInt32 rID, char nPov);

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

        public VjdStat getVJDStatus(uint rID)
        {
            return (VjdStat)GetVJDStatus(rID);
        }

        public bool acquireVJD(uint rID)
        {
            if (AcquireVJD(rID))
            {
                currentState[rID].bDevice = (byte) rID;
                return true;
            }
            else
                return false;
        }

        public void relinquishVJD(uint rID)
        {
            if (GetVJDStatus((UInt32)rID) == (UInt32)VjdStat.VJD_STAT_OWN)
                RelinquishVJD(rID);
        }

        public int joystickStateSize()
        {
            JoystickState example = new JoystickState();
            return System.Runtime.InteropServices.Marshal.SizeOf(example);
            
        }

        public int getTotalButtons(uint rID)
        {
            return GetVJDButtonNumber(rID);
        }

        public int getTotalDiscretePOVs(uint rID)
        {
            return GetVJDDiscPovNumber(rID);
        }

        public int getTotalContinuousPOVs(uint rID)
        {
            return GetVJDContPovNumber(rID);
        }

        public bool hasAxis(uint rID, HID_USAGES Axis)
        {
            return GetVJDAxisExist(rID, (uint)Axis);
        }

        public void setAxis(uint rID, int value, HID_USAGES axis)
        {
            switch(axis)
            {
                case(HID_USAGES.HID_USAGE_X):
                    currentState[rID].AxisX = value;
                    break;
                case(HID_USAGES.HID_USAGE_Y):
                    currentState[rID].AxisY = value;
                    break;
                case(HID_USAGES.HID_USAGE_Z):
                    currentState[rID].AxisZ = value;
                    break;
                case(HID_USAGES.HID_USAGE_RX):
                    currentState[rID].AxisXRot = value;
                    break;
                case(HID_USAGES.HID_USAGE_RY):
                    currentState[rID].AxisYRot = value;
                    break;
                case(HID_USAGES.HID_USAGE_RZ):
                    currentState[rID].AxisZRot = value;
                    break;
                case(HID_USAGES.HID_USAGE_SL0):
                    currentState[rID].Slider = value;
                    break;
                case(HID_USAGES.HID_USAGE_SL1):
                    currentState[rID].Dial = value;
                    break;
                case(HID_USAGES.HID_USAGE_WHL):
                    currentState[rID].Wheel = value;
                    break;
            }
        }

        public bool setButton(bool value, uint rID, char button)
        {
            return SetBtn(value, rID, button);
        }

        public bool setDiscPov(Int32 Value, uint rID, char nPov)
        {
            return SetDiscPov(Value, rID, nPov);
        }

        public bool setContPov(UInt32 Value, uint rID, char nPov)
        {
            return setContPov(Value, rID, nPov);
        }

        public bool resetAll()
        {
            return ResetAll();
        }

        public bool resetVJD(uint rID)
        {
            return ResetVJD(rID);
        }

        public bool resetButtons(uint rID)
        {
            return ResetButtons(rID);
        }

        public bool sendUpdate(UInt32 rID)
        {
            byte[] arr = new byte[Marshal.SizeOf(currentState[rID])];
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(Marshal.SizeOf(currentState[rID]));
            Marshal.StructureToPtr(currentState[rID], unmanagedPointer, false);
            Marshal.Copy(unmanagedPointer, arr, 0, Marshal.SizeOf(currentState[rID]));


            return UpdateVJD(rID, unmanagedPointer);
        }
    }
}
