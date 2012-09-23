using System;
using System.Threading;
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
    public class Worker
    {
        Object CSharpObject;
        public Worker(ref Object anObject)
        {
            CSharpObject = anObject;
        }
        // This method will be called when the thread is started.
        public void DoWork()
        {
            CSharpObject.GetType().InvokeMember("Initialize", System.Reflection.BindingFlags.InvokeMethod, null, CSharpObject, null);

            int refreshRate = (int)CSharpObject.GetType().InvokeMember("getRefreshRate", System.Reflection.BindingFlags.InvokeMethod, null, CSharpObject, null);
            while (!_shouldStop)
            {
                //Console.WriteLine("worker thread: working...");
                CSharpObject.GetType().InvokeMember("mainLoop", System.Reflection.BindingFlags.InvokeMethod, null, CSharpObject, null);
                Thread.Sleep(refreshRate);
            }
            Console.WriteLine("worker thread: terminating gracefully.");
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        // Volatile is used as hint to the compiler that this data
        // member will be accessed by multiple threads.
        private volatile bool _shouldStop;
    }

    public partial class Form1 : Form
    {
        string[] inputArgs;
        Object CSharpObject;
        
        public Form1(string[] args)
        {
            InitializeComponent();
            inputArgs = args;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*vJoy joystick = new vJoy();

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
            bool hasWHL = joystick.hasAxis(1, HID_USAGES.HID_USAGE_WHL);*/




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
            compilerParams.ReferencedAssemblies.Add("SBC.dll");
            compilerParams.ReferencedAssemblies.Add("myVJoyWrapper.dll");
            String[] fileNames = new String[1];

            if (inputArgs.Length != 1)
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
            CompilerResults results = codeProvider.CompileAssemblyFromFile(compilerParams, @"..\..\..\..\..\Steel-Batallion-64\Simple.cs");
            if (results.Errors.Count > 0)
            {
                //MessageBox.Show("There were errors");
                int i =0;
                string[] newLines = new string[results.Errors.Count*2];
                foreach (CompilerError error in results.Errors)
                {
                    newLines[i] = error.ToString();
                    newLines[i + 1] = "\n";
                    i = i + 2;
                }
                errorBox.Lines = newLines;
            }
            else
            {
                CSharpObject = results.CompiledAssembly.CreateInstance("SBC.DynamicClass");
                // Create the thread object. This does not start the thread.
                Worker workerObject = new Worker(ref CSharpObject);
                Thread workerThread = new Thread(workerObject.DoWork);

                // Start the worker thread.
                workerThread.Start();
            }        

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
