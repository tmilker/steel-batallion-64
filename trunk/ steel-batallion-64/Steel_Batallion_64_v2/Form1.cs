﻿using System;
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
//using Microsoft.DirectX.DirectInput;


namespace Steel_Batallion_64_v2
{
	public partial class Form1 : Form
	{
		Object CSharpObject;
		Worker workerObject;
		Thread workerThread;
		bool ProgramStarted = false;
		
		public Form1(string[] args)
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
            fileString.Text = Properties.Settings.Default.storedFileName;
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
			    CompilerParameters compilerParams = new CompilerParameters();
			    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			    {
				    compilerParams.ReferencedAssemblies.Add(assembly.Location);
			    }
			    compilerParams.ReferencedAssemblies.Add("SBC.dll");
			    compilerParams.ReferencedAssemblies.Add("myVJoyWrapper.dll");
                compilerParams.ReferencedAssemblies.Add("Microsoft.DirectX.DirectInput.dll");
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
