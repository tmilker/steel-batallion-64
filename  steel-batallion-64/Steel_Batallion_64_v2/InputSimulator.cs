using System;
using System.Runtime.InteropServices;
using Microsoft.DirectX.DirectInput;
//http://damiproductions.darkbb.com/t504-c-sending-keys-with-the-sendinput-api

namespace SBC
{
    internal class InputSimulator
    {
        #region Imports

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        #endregion

        public static int ModifiedKeyStrokeDelay = 5;//number of milliseconds combination keys will be held down.
                                              //this number was experimentally found out as the minimum necessary for MW4 to respond to modified
                                              //keystroke

        public static void SimulateKeyDown(Microsoft.DirectX.DirectInput.Key key)
        {
            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            //structure.ki.wVk = VkKeyScan((char)key);
            structure.ki.wScan = (ushort)key;
            structure.ki.dwFlags = (uint)((int)KEYEVENTF.KEYDOWN | (uint)KEYEVENTF.SCANCODE);
            structure.ki.time = 0;
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] pInputs = new INPUT[] { structure };

            SendInput(1, pInputs, Marshal.SizeOf(structure));
        }

        public static void SimulateKeyUp(Microsoft.DirectX.DirectInput.Key key)
        {
            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            //structure.ki.wVk = VkKeyScan((char)key);
            structure.ki.wScan = (ushort)key;
            structure.ki.dwFlags = (uint)((int)KEYEVENTF.KEYUP | (uint)KEYEVENTF.SCANCODE);
            structure.ki.time = 0;
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] pInputs = new INPUT[] { structure };

            SendInput(1, pInputs, Marshal.SizeOf(structure));
        }

        public static void SimulateKeyPress(Microsoft.DirectX.DirectInput.Key key)
        {
            SimulateKeyDown(key);
            SimulateKeyUp(key);
        }

        public static void SimulateModifiedKeyStroke(Microsoft.DirectX.DirectInput.Key modifier, Microsoft.DirectX.DirectInput.Key keycode)
        {
            SimulateKeyDown(modifier);
            SimulateKeyDown(keycode);
            System.Threading.Thread.Sleep(ModifiedKeyStrokeDelay);
            SimulateKeyUp(keycode);
            SimulateKeyUp(modifier);
            

            //I tried doing this using several input statements, but it seems that the OS likes it if you spread them out
            //if anyone knows more about this let me know, but this method works for now.
        }

        

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public int type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [Flags]
        public enum InputType
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }

        [Flags]
        public enum MOUSEEVENTF
        {
            MOVE = 0x0001, /* mouse move */
            LEFTDOWN = 0x0002, /* left button down */
            LEFTUP = 0x0004, /* left button up */
            RIGHTDOWN = 0x0008, /* right button down */
            RIGHTUP = 0x0010, /* right button up */
            MIDDLEDOWN = 0x0020, /* middle button down */
            MIDDLEUP = 0x0040, /* middle button up */
            XDOWN = 0x0080, /* x button down */
            XUP = 0x0100, /* x button down */
            WHEEL = 0x0800, /* wheel button rolled */
            MOVE_NOCOALESCE = 0x2000, /* do not coalesce mouse moves */
            VIRTUALDESK = 0x4000, /* map to entire virtual desktop */
            ABSOLUTE = 0x8000 /* absolute move */
        }

        [Flags]
        public enum KEYEVENTF
        {
            KEYDOWN = 0,
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            UNICODE = 0x0004,
            SCANCODE = 0x0008,
        }
    }
}