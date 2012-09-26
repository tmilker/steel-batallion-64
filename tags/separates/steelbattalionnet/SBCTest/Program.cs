/*********************************************************************************
 *   SteelBattalion.NET - A library to access the Steel Battalion XBox           *
 *   controller.  Written by Joseph Coutcher.                                    *
 *                                                                               *
 *   This file is part of SteelBattalion.NET                                     *
 *                                                                               *
 *   SteelBattalion.NET is free software: you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by        *
 *   the Free Software Foundation, either version 3 of the License, or           *
 *   (at your option) any later version.                                         *
 *                                                                               *
 *   SteelBattalion.NET is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of              *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the               *
 *   GNU General Public License for more details.                                *
 *                                                                               *
 *   You should have received a copy of the GNU General Public License           *
 *   along with SteelBattalion.NET.  If not, see <http://www.gnu.org/licenses/>. *
 *                                                                               *
 *   While this code is licensed under the LGPL, please let me know whether      *
 *   you're using it.  I'm always interested in hearing about new projects,      *
 *   especially ones that I was able to make a contribution to...in the form of  *
 *   this library.                                                               *
 *                                                                               *
 *   EMail: geekteligence at google mail                                         *
 *                                                                               *
 *   2010-11-26: JC - Initial commit                                             *
 *                                                                               * 
 *********************************************************************************/

using System;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;


namespace SBCTest {
    public class NotDynamicClass
    {
        private readonly List<string> values = new List<string>();

        public void AddValue(string value)
        {
            values.Add(value);
        }

        public void ProcessValues()
        {
            foreach (var item in values)
            {
                Console.WriteLine(item);
            }
        }
    }

	class Program {
		public static void Main(string[] args) {
			// Initialize the controller
        var provider = CSharpCodeProvider.CreateProvider("c#");
        CompilerParameters parameters = new CompilerParameters();
        parameters.GenerateExecutable = true;

        var assemblyContainingNotDynamicClass = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        parameters.ReferencedAssemblies.Add(assemblyContainingNotDynamicClass);
        // Add available assemblies - this should be enough for the simplest
        // applications.
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            parameters.ReferencedAssemblies.Add(asm.Location);
        }
        var results = provider.CompileAssemblyFromSource(parameters, new[] 
        { 
@"
namespace SBCTest {
public class DynamicClass
{
    public static void Main()
    {
        NotDynamicClass @class = new NotDynamicClass();
        @class.AddValue(""One"");
        @class.AddValue(""Two"");
        @class.ProcessValues();
    }
}
}
"
        });
        if (results.Errors.Count > 0)
        {
            foreach (var error in results.Errors)
            {
                Console.WriteLine(error);
            }
        }
        else
        {
            var t = results.CompiledAssembly.GetType("SBCTest.DynamicClass");
            t.GetMethod("Main").Invoke(null, null);
        }
    
            
			SBC.SteelBattalionController controller = new SBC.SteelBattalionController();
			controller.Init(50);
			
			// Uncomment if you want to monitor the raw data coming out of the controller
			//controller.RawData += new SBC.SteelBattalionController.RawDataDelegate(controller_RawData);
					//set all buttons by default to light up only when you press them down
			// Add the event handler to monitor button state changed events
			controller.ButtonStateChanged += new SBC.SteelBattalionController.ButtonStateChangedDelegate(controller_ButtonStateChanged);
			
			// Run in an infinite loop
			while(1 == 1) { System.Threading.Thread.Sleep(10); }
		}

		static void controller_ButtonStateChanged(SBC.SteelBattalionController controller,SBC.ButtonState[] stateChangedArray) {
			// Typically, you want to check which buttons were triggered.  This array contains
			// a list of all buttons, their current values, and whether their state has changed.
			if (stateChangedArray[(int) SBC.ButtonEnum.FunctionLineColorChange].changed) {
				// Do specific things when the "Line Color Change" button has a state change
			}
			
			// Use a for loop to examine each one of the states returned in the state change array
			foreach(SBC.ButtonState state in stateChangedArray) {
				if (state.changed) {
					// Write out the state of the button if it was changed
					Console.WriteLine("Button: {0,32}  State: {1}", state.button.ToString(), state.currentState.ToString());
				}
			}
		}

		static void controller_RawData(byte[] rawData) {
			Console.WriteLine(BitConverter.ToString(rawData));
		}
	}
}