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

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct JoystickState {
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
			public Byte Hat;
		} ;

// Usage example:
//	JOYSTICK_POSITION iReport;
//	:
//	DeviceIoControl (hDevice, 100, &iReport, sizeof(HID_INPUT_REPORT), NULL, 0, &bytes, NULL)
typedef struct _JOYSTICK_POSITION
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
} JOYSTICK_POSITION, *PJOYSTICK_POSITION;

    public class vJoy
    {
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
        private static extern void RelinquishVJD(UInt32 rID);

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
            if (GetVJDStatus((UInt32)rID) == VjdStat.VJD_STAT_OWN)
                RelinquishVJD((UInt32)rID);
        }


    }
}
