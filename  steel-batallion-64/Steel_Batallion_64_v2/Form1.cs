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
using Microsoft.DirectX.DirectInput;


namespace Steel_Batallion_64_v2
{
	public partial class Form1 : Form
	{
		string[] inputArgs;
		Object CSharpObject;
		Worker workerObject;
		Thread workerThread;
		bool ProgramStarted = false;
		
		public Form1(string[] args)
		{
			InitializeComponent();
			inputArgs = args;
		}

		private void Form1_Load(object sender, EventArgs e)
		{          

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

		private void StartBtn_Click(object sender, EventArgs e)
		{
			if(!ProgramStarted)
			{
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

			    CompilerResults results = codeProvider.CompileAssemblyFromFile(compilerParams, @"..\..\..\..\..\Steel-Batallion-64\Simple.cs");
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
				//Console.WriteLine("worker thread: working...");
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
