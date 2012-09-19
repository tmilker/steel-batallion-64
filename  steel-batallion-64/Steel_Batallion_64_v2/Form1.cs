using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using  System.IO;
using  System.Reflection;
using  System.CodeDom.Compiler;
using  Microsoft.CSharp;
using  SBC;
using myVJoyWrapper;


namespace Steel_Batallion_64_v2
{
    public partial class Form1 : Form
    {
        string[] inputArgs;
        
        public Form1(string[] args)
        {
            InitializeComponent();
            inputArgs = args;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            vJoy joystick = new vJoy();

            bool enabled = joystick.isEnabled();
            int version = joystick.getVersion();
            string productInfo = joystick.getProductString();
            VjdStat currentStatus = joystick.getVJDStatus(1);
            bool acuired = joystick.acquireVJD(1);
            VjdStat currentStatus2 = joystick.getVJDStatus(1);
            int byteSize = joystick.joystickStateSize();

            int totalButtons = joystick.getTotalButtons(1);
            int discretePOVnumber = joystick.getTotalDiscretePOVs(1);
            int continuousPOVnumber = joystick.getTotalContinuousPOVs(1);

            bool hasX = joystick.hasAxis(1, HID_USAGES.HID_USAGE_X);
            bool hasY = joystick.hasAxis(1, HID_USAGES.HID_USAGE_Y);
            bool hasZ = joystick.hasAxis(1, HID_USAGES.HID_USAGE_Z);
            bool hasRX = joystick.hasAxis(1, HID_USAGES.HID_USAGE_RX);
            bool hasRY = joystick.hasAxis(1, HID_USAGES.HID_USAGE_RY);
            bool hasRZ = joystick.hasAxis(1, HID_USAGES.HID_USAGE_RZ);
            bool hasSL0 = joystick.hasAxis(1, HID_USAGES.HID_USAGE_SL0);
            bool hasSL1 = joystick.hasAxis(1, HID_USAGES.HID_USAGE_SL1);
            bool hasWHL = joystick.hasAxis(1, HID_USAGES.HID_USAGE_WHL);



            int value = 0;
            joystick.resetAll();//needs to be called before modifying values
            joystick.setAxis(1, 256, HID_USAGES.HID_USAGE_X);
            joystick.sendUpdate(1);
            /*while (true)
            {
                for (int i = 0; i < 10; i++)
                    successful = joystick.setAxis(1,value, (uint)(HID_USAGES.HID_USAGE_X + i));

                System.Threading.Thread.Sleep(20);
                value += 10;
            }*/

            //successful = joystick.setAxis(1, value, (uint)(HID_USAGES.HID_USAGE_X));

           
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            codeProvider.CreateCompiler();
            //add compiler parameters
            CompilerParameters compilerParams = new CompilerParameters();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                compilerParams.ReferencedAssemblies.Add(assembly.Location);
            }
            String[] fileNames;
			if(inputArgs.Length !=1)
        	{
		        MessageBox.Show("Incorrect number of command line paramters, please input settings c# file");
                return;
        	}
            /*if (joystick.IsDeviceAccessible())
            {
                joystick.SetButton(0, true);
                joystick.SetAxis((int)VJoy.JoystickAxis.AxisX, (ushort)VJoy.AxisMax);
                joystick.UpdateJoystick();
            }*/
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           /* if (joystick.IsDeviceAccessible())
                joystick.UpdateJoystick();*/
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //joystick.Dispose();
        }
    }
}
