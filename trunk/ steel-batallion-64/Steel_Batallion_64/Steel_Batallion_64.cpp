/*********************************************************************************
 *   SteelBattalion 64 - A program used to access the Steel Battalion XBox       *
 *   controller.  Written by Santiago Saldana based on the work by				 *
 *	 Joseph Coutcher.															 *
 *                                                                               *
  *                                                                              *
 *   SteelBattalion 64 is free software: you can redistribute it and/or modify   *
 *   it under the terms of the GNU General Public License as published by        *
 *   the Free Software Foundation, either version 3 of the License, or           *
 *   (at your option) any later version.                                         *
 *                                                                               *
 *   SteelBattalion 64 is distributed in the hope that it will be useful,        *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of              *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the               *
 *   GNU General Public License for more details.                                *
 *                                                                               *
 *   You should have received a copy of the GNU General Public License           *
 *   along with SteelBattalion.NET.  If not, see <http://www.gnu.org/licenses/>. *
 *                                                                               *
 *   I'm keeping this code under the LGPL as oriinally licensed by				 *
 *	 Joseph Coutcher.  If you have any suggestions for the library, or need help *
 *	 compiling please feel free to e-mail me.									 *
 *																				 * 
 *									PLEASE NOTE									 *
 *   This software was created using Visual Studio 2008 edition.  I have access  *
 *   to VS2010, however due to the issues with lack of intellisense with CLR/CLI *
 *   when dealing with managed C++, I decided to keep the project in 2008.       *
 *   While it will work with VS2010, please try the code with VS2008 before      *
 *   e-mailing me for help.                                                      *
 *                                                                               *
 *   EMail: saldsj3 at google mail		                                         *
 *                                                                               *
 *   4-29-2012: SJS - Initial commit                                             *
 *                                                                               * 
 *********************************************************************************/

#include "stdafx.h"
#include <stdio.h>
#include <conio.h>
#include <windows.h>
#include <tchar.h>

#include <winioctl.h>
using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::CodeDom::Compiler;
using namespace Microsoft::CSharp;
using namespace SBC;


int baseLineIntensity = 1;//just an average value for LED intensity
int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

public ref class Globals abstract sealed {
public:
static Object^ CSharpObject;
};


#include "joystick.h"

void setGearShiftLight(SBC::SteelBattalionController ^ controller,bool setNow,int intensity)
{
	/*	LED VALUES
	Gear5 = 41,
	Gear4 = 40,
	Gear3 = 39,
	Gear2 = 38,
	Gear1 = 37,
	GearN = 36,
	GearR = 35,*/
	
	for(int i=35;i<=41;i++)
		controller->SetLEDState((SBC::ControllerLEDEnum)(i),0,false);//turn all off

	int gearValue = controller->GearLever;//returns values -2,-1,1,2,3,4,5
	if (gearValue < 0)
		controller->SetLEDState((SBC::ControllerLEDEnum)((int)SBC::ControllerLEDEnum::Gear1+gearValue),intensity,setNow);//3 stands for intensity
	else
	controller->SetLEDState(SBC::ControllerLEDEnum::GearN + (SBC::ControllerLEDEnum)(gearValue),intensity,setNow);
}


//this seems to work slowly, I'm not sure why.
static void controller_ButtonStateChanged(SBC::SteelBattalionController ^ controller, array<SBC::ButtonState ^> ^ stateChangedArray) 
{
	// Typically, you want to check which buttons were triggered.  This array contains
	// a list of all buttons, their current values, and whether their state has changed.
	if (stateChangedArray[(int) SBC::ButtonEnum::FunctionLineColorChange]->changed) {
		if(controller->GetButtonState((int) SBC::ButtonEnum::FunctionLineColorChange))//only switch light on downpress
		{
			int currentLEDState = controller->GetLEDState(SBC::ControllerLEDEnum::LineColorChange);
			controller->SetLEDState(SBC::ControllerLEDEnum::LineColorChange,(!currentLEDState)*5);
		}
		controller->AddButtonLightMapping(SBC::ButtonEnum::Eject,false,15);
		controller->AddButtonLightMapping(SBC::ButtonEnum::Eject,false,15);
		// Do specific things when the "Line Color Change" button has a state change
	}

	// Use a for loop to examine each one of the states returned in the state change array
	/*for each(SBC::ButtonState^ state in stateChangedArray) 
	{
		if (state->changed) {
			// Write out the state of the button if it was changed
			printf("Button: %s  State: %s\n", state->button.ToString(), state->currentState.ToString());
		}
	}*/

	if (stateChangedArray[(int) SBC::ButtonEnum::GearLeverStateChange]->changed) 
	{
		setGearShiftLight(controller,true,baseLineIntensity);
	}


}




int main(array<System::String ^> ^args)
{
array<joystick^> ^ joysticks;
int lastGearValue;



System::CodeDom::Compiler::CodeDomProvider^ provider = CSharpCodeProvider::CreateProvider("c#");
CompilerParameters^ parameters = gcnew CompilerParameters();
parameters->GenerateExecutable = false;
String^ executingLocation = Assembly::GetExecutingAssembly()->Location;
String^ assemblyContainingNotDynamicClass = Path::GetFileName(Assembly::GetExecutingAssembly()->Location);
//parameters->ReferencedAssemblies->Add(assemblyContainingNotDynamicClass);
parameters->ReferencedAssemblies->Add("..\\Executables\\SBC.dll");

for each (Assembly^ assembly in AppDomain::CurrentDomain->GetAssemblies())
        {
            parameters->ReferencedAssemblies->Add(assembly->Location);
        }

array<String^>^ fileNames = gcnew array<String^>(1);
//fileNames[0] = gcnew String("C:\\Users\\Santiago\\Documents\\Visual Studio 2008\\Projects\\Steel-Batallion-64\\steelbattalionnet\\SBC\\test.cs");
if(args->Length ==1)
	fileNames[0] = gcnew String(args[0]);
else
{
	Console::WriteLine(L"Incorrect number of command line paramters, please input settings c# file");
	Sleep(3000);
	exit(1);
}
	

System::CodeDom::Compiler::CompilerResults^ results = provider->CompileAssemblyFromFile(parameters, fileNames);


        if (results->Errors->Count > 0)
        {
			for each (CompilerError ^ error in results->Errors)
            {
				Console::WriteLine(error);
            }
			while(1){Sleep(10);}
        }
        else
        {
			array<Object^>^ parameters = gcnew array<Object^>(1);
			Globals::CSharpObject = results->CompiledAssembly->CreateInstance("SBC.DynamicClass");
			Globals::CSharpObject->GetType()->InvokeMember("Initialize",System::Reflection::BindingFlags::InvokeMethod,nullptr,Globals::CSharpObject,nullptr);
        }        

int numJoysticks = (int)Globals::CSharpObject->GetType()->InvokeMember("getNumJoysticks",System::Reflection::BindingFlags::InvokeMethod,nullptr,Globals::CSharpObject,nullptr);

 
joysticks  = gcnew array<joystick^>(numJoysticks);

for(int i=0;i<joysticks->Length;i++)
{
	joysticks[i] = gcnew joystick;


	char joystickName[50];

	sprintf(joystickName,"\\\\.\\PPJoyIOCTL%d",i+1);

    // Convert to a wchar_t*
    // You must first convert to a char * for this to work.
    size_t origsize = strlen(joystickName) + 1;
    size_t convertedChars = 0;
    wchar_t wcstring[50];
    mbstowcs_s(&convertedChars, wcstring, origsize, joystickName, _TRUNCATE);

	//if(!joysticks[0]->init(L"\\\\.\\PPJoyIOCTL1") < 0)
	if(!joysticks[i]->init(wcstring) < 0)
	{
		printf("Unable to open PPJoy Virtual Joystick 1, check the Game Controllers panel.");
		Sleep(2000);
		exit(1);
	}
			array<Object^>^ parameters = gcnew array<Object^>(1);
			parameters[0] = (int) 1;
int numButtons = (int)Globals::CSharpObject->GetType()->InvokeMember("getNumButtons",System::Reflection::BindingFlags::InvokeMethod,nullptr,Globals::CSharpObject,parameters);
joysticks[i]->totalButtons = numButtons;
}











// Initialize the controller
//controller = gcnew SBC::SteelBattalionController();
//controller->Init(50);




// Add the event handler to monitor button state changed events
//controller->ButtonStateChanged += gcnew SBC::SteelBattalionController::ButtonStateChangedDelegate(controller_ButtonStateChanged);

 Console::WriteLine(L"Welcome to Steel Batallion 64");
 /*
 Console::WriteLine(L"Leave this Running While you Play");
 Console::WriteLine(L"This program will update PPJoy Virtual Joysticks 1 - 3");
 Console::WriteLine(L"Run a one time calibration on each PPJoy Virtual Joystick");
 Console::WriteLine(L"so the Game Controllers panel updates");
*/
 while(1)
 {
	for(int i=0;i<numJoysticks;i++)
	{
		array<Object^>^ parameters = gcnew array<Object^>(1);
		parameters[0] = (int) i;
		int numAxis = (int)Globals::CSharpObject->GetType()->InvokeMember("getNumAxis",System::Reflection::BindingFlags::InvokeMethod,nullptr,Globals::CSharpObject,parameters);
		for(int j=0;j<numAxis;j++)
		{
			parameters = gcnew array<Object^>(2);
			parameters[0] = (int) i;
			parameters[1] = (int) j;

			int axisValue = (int)Globals::CSharpObject->GetType()->InvokeMember("getAxisValue",System::Reflection::BindingFlags::InvokeMethod,nullptr,Globals::CSharpObject,parameters);
			joysticks[i]->setAxis(j,axisValue);
		}
	}

	bool setButtons = (bool)Globals::CSharpObject->GetType()->InvokeMember("useButtons",System::Reflection::BindingFlags::InvokeMethod,nullptr,Globals::CSharpObject,nullptr);

	if(setButtons)
	{
		for(int i=0;i<joysticks->Length;i++)
		{
			for(int j=0;j<joysticks[i]->totalButtons;j++)
			{
				array<Object^>^ parameters = gcnew array<Object^>(2);
				parameters[0] = (int) i;
				parameters[1] = (int) j;
				bool buttonValue = (bool)Globals::CSharpObject->GetType()->InvokeMember("getButtonValue",System::Reflection::BindingFlags::InvokeMethod,nullptr,Globals::CSharpObject,parameters);

				joysticks[i]->setButton(j,buttonValue);
			}
		}
	}
	for(int i=0;i<numJoysticks;i++)
	{
		if(joysticks[i]->sendBuffer()<1)
		{
			printf("ERROR sending joystick values");
			Sleep(1000);
			break;
		}
	}
	Globals::CSharpObject->GetType()->InvokeMember("extraCode",System::Reflection::BindingFlags::InvokeMethod,nullptr,Globals::CSharpObject,nullptr);


	Sleep(50);
 }



    return 0;
}
