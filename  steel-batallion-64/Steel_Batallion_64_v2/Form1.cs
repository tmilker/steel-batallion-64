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


namespace SBC
{
	public partial class Form1 : Form
	{
		Object CSharpObject;
		Worker workerObject;
		Thread workerThread;
        CompilerParameters compilerParams;
		bool ProgramStarted = false;
        bool firstTime = true;
		
		public Form1(string[] args)
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
            fileString.Text = Properties.Settings.Default.storedFileName;
            SteelBattalionController controller;
            vJoy joystick;
            bool acquired;
            int baseLineIntensity = 3;//just an average value for LED intensity
            controller = new SteelBattalionController();
            controller.Init(50);//50 is refresh rate in milliseconds
            //set all SBC.Buttons by default to light up only when you press them down

            /*for (int i = 4; i < 4 + 30; i++)
            {
                if (i != (int)SBC.ButtonLights.Eject)//excluding eject since we are going to flash that one
                    controller.AddButtonLightMapping((SBC.ButtonLights)(i - 1), (ControllerLEDEnum)(i), true, baseLineIntensity);
            }*/

            controller.AddButtonKeyMapping(SBC.Buttons.RightJoyMainWeapon, Microsoft.DirectX.DirectInput.Key.D6, Microsoft.DirectX.DirectInput.Key.D5, true);
            controller.AddButtonKeyMapping(SBC.Buttons.RightJoyLockOn, Microsoft.DirectX.DirectInput.Key.R, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.Ignition, true, 3, Microsoft.DirectX.DirectInput.Key.P, true);
            //controller.AddButtonKeyLightMapping(Buttons.Start,				true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.MultiMonOpenClose, true, 3, Microsoft.DirectX.DirectInput.Key.B, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.MultiMonMapZoomInOut, true, 3, Microsoft.DirectX.DirectInput.Key.B, true);
            //controller.AddButtonKeyLightMapping(Buttons.MultiMonModeSelect,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(Buttons.MultiMonSubMonitor,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.MainMonZoomIn, true, 3, Microsoft.DirectX.DirectInput.Key.Z, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.MainMonZoomOut, true, 3, Microsoft.DirectX.DirectInput.Key.Z, true);
            //controller.AddButtonKeyLightMapping(Buttons.FunctionFSS,			true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(Buttons.FunctionManipulator,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.FunctionLineColorChange, true, 3, Microsoft.DirectX.DirectInput.Key.H, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.Washing, true, 3, Microsoft.DirectX.DirectInput.Key.C, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.Extinguisher, true, 3, Microsoft.DirectX.DirectInput.Key.O, true);
            //controller.AddButtonKeyLightMapping(Buttons.Chaff,				true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            //controller.AddButtonKeyLightMapping(Buttons.FunctionTankDetach,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.FunctionOverride, true, 3, Microsoft.DirectX.DirectInput.Key.O, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.FunctionNightScope, true, 3, Microsoft.DirectX.DirectInput.Key.N, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.FunctionF1, true, 3, Microsoft.DirectX.DirectInput.Key.Tab, true);
            //controller.AddButtonKeyLightMapping(Buttons.FunctionF2,			true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.FunctionF3, true, 3, Microsoft.DirectX.DirectInput.Key.LeftControl, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.WeaponConMain, true, 3, Microsoft.DirectX.DirectInput.Key.RightControl, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.WeaponConSub, true, 3, Microsoft.DirectX.DirectInput.Key.BackSpace, true);
            //controller.AddButtonKeyLightMapping(Buttons.WeaponConMagazine,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.Comm1, true, 3, Microsoft.DirectX.DirectInput.Key.F6, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.Comm2, true, 3, Microsoft.DirectX.DirectInput.Key.F8, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.Comm3, true, 3, Microsoft.DirectX.DirectInput.Key.F9, true);
            //controller.AddButtonKeyLightMapping(Buttons.Comm4,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
            controller.AddButtonKeyLightMapping(SBC.ButtonLights.Comm5, true, 3, Microsoft.DirectX.DirectInput.Key.RightBracket, true);
            controller.AddButtonKeyMapping(SBC.Buttons.LeftJoySightChange, Microsoft.DirectX.DirectInput.Key.Z, true);


		}

		private void timer1_Tick(object sender, EventArgs e)
		{
		   /* if (joystick.IsDeviceAccessible())
				joystick.UpdateJoystick();*/
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			//joystick.Dispose();
            if (ProgramStarted)
            {
                workerObject.RequestStop();
                // Use the Join method to block the current thread 
                // until the object's thread terminates.
                workerThread.Join();
                Status.Text = "Stopped";
                ProgramStarted = false;
            }
		}

		private void StartBtn_Click(object sender, EventArgs e)
        {
			if(!ProgramStarted)
			{
			    CSharpCodeProvider codeProvider = new CSharpCodeProvider();
			    codeProvider.CreateCompiler();
			    //add compiler parameters

                compilerParams = new CompilerParameters();//have to have this here everytime
                if (firstTime)//simple hack to fix issues with Microsoft.DirectX.DirectInput.dll not being able to be loaded multiple times
                {
                    compilerParams.ReferencedAssemblies.Add("Microsoft.DirectX.DirectInput.dll");
                }
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    compilerParams.ReferencedAssemblies.Add(assembly.Location);
                }


			    String[] fileNames = new String[1];


			    CompilerResults results = codeProvider.CompileAssemblyFromFile(compilerParams, fileString.Text);
			    if (results.Errors.Count > 0)
			    {
				    //MessageBox.Show("There were errors");
				    int i = 0;
				    string[] newLines = new string[results.Errors.Count * 2];
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
                    firstTime = false;//once we create a sucessful assembly, we don't need to keep referencing DirectX.Direcinput
                    errorBox.Lines  = null;
                    CSharpObject = results.CompiledAssembly.CreateInstance("SBC.DynamicClass");

				    // Create the thread object. This does not start the thread.
				    workerObject = new Worker(ref CSharpObject);
				    workerThread = new Thread(workerObject.DoWork);
                    Status.Text = "Running";
				    ProgramStarted = true;
				    // Start the worker thread.
				    workerThread.Start();
			    }
			}

		}

		private void StopBtn_Click(object sender, EventArgs e)
		{
			workerObject.RequestStop();
            // Use the Join method to block the current thread 
            // until the object's thread terminates.
            workerThread.Join();
            Status.Text = "Stopped";
            ProgramStarted = false;
		}

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "C# files|*.cs";
            openFileDialog1.Title = "Select a C# configuration File";

            // Show the Dialog.
            // If the user clicked OK in the dialog and
            // a .CUR file was selected, open it.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.
                Properties.Settings.Default.storedFileName = fileString.Text = openFileDialog1.FileName;
                Properties.Settings.Default.Save();
            }
        }
	}
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
				CSharpObject.GetType().InvokeMember("mainLoop", System.Reflection.BindingFlags.InvokeMethod, null, CSharpObject, null);
				Thread.Sleep(refreshRate);
			}
			CSharpObject.GetType().InvokeMember("shutDown", System.Reflection.BindingFlags.InvokeMethod, null, CSharpObject, null);
		}
		public void RequestStop()
		{
			_shouldStop = true;
		}
		// Volatile is used as hint to the compiler that this data
		// member will be accessed by multiple threads.
		private volatile bool _shouldStop;
	}
}
