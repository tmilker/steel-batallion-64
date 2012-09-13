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
            bool enabled = vJoy.vJoyEnabled();
            int version = vJoy.GetvJoyVersion();
            string productInfo = vJoy.getProductString();
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
