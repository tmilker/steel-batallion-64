using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace myVJoyWrapper
{

    enum VjdStat
    {
        //status can be one of the following values:
        VJD_STAT_OWN, // The vJoy Device is owned by this application.
        VJD_STAT_FREE, // The vJoy Device is NOT owned by any application (including this one).
        VJD_STAT_BUSY, // The vJoy Device is owned by another application.
        // It cannot be acquired by this application.
        VJD_STAT_MISS, // The vJoy Device is missing. It either does not exist or the driver is down.
        VJD_STAT_UNKN, // Unknown
    }

    public static class vJoy
    {
        [DllImport("vJoyInterface.dll")]
        public static extern bool vJoyEnabled();

        [DllImport("vJoyInterface.dll")]
        public static extern short GetvJoyVersion();
        
        [DllImport("vJoyInterface.dll")]
        private static extern IntPtr GetvJoyProductString();

        [DllImport("vJoyInterface.dll")]
        private static extern IntPtr GetvJoyManufacturerString();

        [DllImport("vJoyInterface.dll")]
        private static extern IntPtr GetvJoySerialNumberString();

        public static string getProductString()
        {
            vJoyEnabled();
            return Marshal.PtrToStringAuto(GetvJoyProductString());
        }




    }
}
