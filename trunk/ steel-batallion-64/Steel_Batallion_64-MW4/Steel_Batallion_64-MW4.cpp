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
#include <atlstr.h>

#include <winioctl.h>

using namespace System;
using namespace System::Collections;
using namespace System::IO;
using namespace System::Text::RegularExpressions;

int baseLineIntensity = 1;//just an average value for LED intensity
int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

#include "joystick.h"//having issues with this, it won't find this in debug mode unless I use an explicit directory



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
joystick ^ joystick1 = gcnew joystick;
int lastGearValue;
bool lastResetValue;
bool currentResetValue;
Hashtable^ ButtonsHash	=	gcnew ::Hashtable();
Hashtable^ HOH			=	gcnew ::Hashtable();//hash of hashes
SBC::SteelBattalionController^ controller;

// Initialize the controller
controller = gcnew SBC::SteelBattalionController();
controller->Init(50);


if(!joystick1->init(L"\\\\.\\PPJoyIOCTL1") < 0)
{
	printf("Unable to open PPJoy Virtual Joystick 1, check the Game Controllers panel.");
	Sleep(2000);
	exit(1);
}


FILE * stream = fopen("buttons.txt","w");
int i =0;

//this works because I know that buttons are a normal enumeration

/*for each(Object^ key in Enum::GetNames(SBC::VirtualKeyCode::typeid))
{

}*/


//this works because I know that buttons are a normal enumeration
for each(Object^ button in Enum::GetNames(SBC::ButtonEnum::typeid))
{
	ButtonsHash->Add(button->ToString()->ToUpper(),i);
	printf("%s %i \n",button->ToString()->ToUpper(),i);
	i++;
}
//SBC::ButtonEnum test = (SBC::ButtonEnum)ButtonsHash["RIGHTJOYMAINWEAPON"];


try
{
    // Create an instance of StreamReader to read from a file.
    // The using statement also closes the StreamReader.
    StreamReader^ sr = gcnew StreamReader("MW4.ini");
    String^ line;
    // Read and display lines from the file until the end of
    // the file is reached.
	String ^ group;
    while ((line = sr->ReadLine()) != nullptr)
    {
		String^ category	= "\\[(.*)]"; 
		String^ entry		= "([a-zA-Z0-9_]*)[\f\n\r\t\v]*=[\f\n\r\t\v]*([a-zA-Z0-9_]*)";//need to figure out how to get standard system for this, i.e. \w \s working for visual c++
		String^ comments	= ";.*";//remove everything after semicolon

		line = Regex::Replace(line,comments,"");
		Match^ match = Regex::Match(line,category);
		if(match->Success)
		{
			group = match->Groups[1]->Value->ToUpper();
			if(!HOH->Contains(group))
				HOH[group] = gcnew ::Hashtable();
		}
		else
		{
			Match^ match = Regex::Match(line,entry,RegexOptions::ECMAScript);
			if(match->Success)
			{
				String ^ button = match->Groups[1]->Value->ToUpper();//make everything uppercase to ignore case
				String ^ keyValue  = match->Groups[2]->Value->ToUpper();
				String ^ mystery  = match->Groups[3]->Value->ToUpper();

				if(ButtonsHash->Contains(button))
					((Hashtable ^)HOH[group])[button] = keyValue;
				int blah = 1;
			}
		}
    }
    sr->Close();
}
catch (Exception^ e)
{
    // Let the user know what went wrong.
    Console::WriteLine("The file MW4.ini could not be read:");
    Console::WriteLine(e->Message);
	Sleep(2000);
	exit(-1);
}


/*
//set all buttons by default to light up only when you press them down
for(int i=3;i<33;i++)
{
		controller->AddButtonLightMapping((SBC::ButtonEnum)(i),true,baseLineIntensity);
}*/

//SET toggle state
if(HOH->Contains("TOGGLELIGHTS"))
for each(::Object ^ button in ((::Hashtable ^)HOH["TOGGLELIGHTS"])->Keys)
	{
		controller->AddButtonLightMapping((SBC::ButtonEnum)ButtonsHash[button],false,emergencyLightIntensity);
	}
//SET light intensity
if(HOH->Contains("LIGHTVALUES"))
for each(::Object ^ button in ((::Hashtable ^)HOH["LIGHTVALUES"])->Keys)
	{
		SBC::ButtonEnum buttonInput = (SBC::ButtonEnum)ButtonsHash[button];
		::Hashtable ^ lightShow = (::Hashtable ^)HOH["LIGHTVALUES"];
		int intensity = System::Convert::ToInt32(lightShow[button]);
		if(HOH->Contains("TOGGLELIGHTS") && ((::Hashtable ^)HOH["TOGGLELIGHTS"])->Contains(button))
		{
			controller->AddButtonLightMapping(buttonInput,false,intensity);
		}
		else
			controller->AddButtonLightMapping(buttonInput,true,intensity);
	}
/*
//add exceptions to intensity
controller->AddButtonLightMapping(SBC::ButtonEnum::Eject,true,emergencyLightIntensity);
controller->AddButtonLightMapping(SBC::ButtonEnum::Ignition,true,emergencyLightIntensity);
controller->AddButtonLightMapping(SBC::ButtonEnum::Start,true,emergencyLightIntensity);

//add exceptions to toggle state, lightOnHold = false means to toggle light state when pressed
controller->AddButtonLightMapping(SBC::ButtonEnum::CockpitHatch,false,emergencyLightIntensity);//false means toggle light state
controller->AddButtonLightMapping(SBC::ButtonEnum::FunctionLineColorChange,false,baseLineIntensity);
controller->AddButtonLightMapping(SBC::ButtonEnum::FunctionNightScope,false,emergencyLightIntensity);//changed intensity for fun


//add key mapping
controller->AddButtonKeyMapping(SBC::ButtonEnum::RightJoyFire,SBC::VirtualKeyCode::RETURN,false);
controller->AddButtonKeyMapping(SBC::ButtonEnum::Washing,SBC::VirtualKeyCode::SPACE,false);
controller->AddButtonKeyMapping(SBC::ButtonEnum::WeaponConMain,SBC::VirtualKeyCode::OEM_6,false);//OEM_6 = ]
controller->AddButtonKeyMapping(SBC::ButtonEnum::WeaponConSub,SBC::VirtualKeyCode::OEM_4,false);//OEM_4 = [
controller->AddButtonKeyLightMapping(SBC::ButtonEnum::WeaponConMagazine,false,baseLineIntensity,SBC::VirtualKeyCode::SHIFT,SBC::VirtualKeyCode::VK_C,false);
*/





bool check = controller->ButtonLights->ContainsKey(SBC::ButtonEnum::FunctionLineColorChange);
lastGearValue = controller->GearLever;
setGearShiftLight(controller,true,baseLineIntensity);


// Add the event handler to monitor button state changed events
//controller->ButtonStateChanged += gcnew SBC::SteelBattalionController::ButtonStateChangedDelegate(controller_ButtonStateChanged);

 Console::WriteLine(L"Welcome to Steel Batallion 64");
 Console::WriteLine(L"Leave this Running While you Play");
 Console::WriteLine(L"This program will update PPJoy Virtual Joysticks 1 - 3");
 Console::WriteLine(L"Run a one time calibration on each PPJoy Virtual Joystick");
 Console::WriteLine(L"so the Game Controllers panel updates");

 while(1)
 {

	//printf("%s\n",controller->GetBinaryBuffer(4,6));
	joystick1->setAxis(0,controller->AimingX);
	joystick1->setAxis(1,controller->AimingY+controller->SightChangeY);


	joystick1->setAxis(2,-1*(controller->RightPedal - controller->MiddlePedal));
	joystick1->setAxis(3,controller->RotationLever+controller->SightChangeX);
	
/*
	joysticks[1]->setAxis(3,controller->TunerDial);


	joysticks[2]->setAxis(0,controller->SightChangeX);
	joysticks[2]->setAxis(1,controller->SightChangeY);
	joysticks[2]->setAxis(2,controller->RotationLever);
	joysticks[2]->setAxis(3,controller->GearLever);*/

	int currentGearValue = controller->GearLever;

	//this seems to work ALOT faster than using the delegate call back, not sure just yet why.
	if (currentGearValue != lastGearValue)
		setGearShiftLight(controller,true,baseLineIntensity);


	//for(int j=0;j<joystick1->totalButtons;j++)
	//	joystick1->setButton(j,controller->GetButtonState(j));

	currentResetValue = controller->GetButtonState((int)SBC::ButtonEnum::ToggleFuelFlowRate);
	if(currentResetValue != lastResetValue && currentResetValue)
	{
		controller->TestLEDs(1);//reset lights
		setGearShiftLight(controller,true,baseLineIntensity);
	}
	lastResetValue = currentResetValue;
	if(joystick1->sendBuffer()<1)
	{
		printf("ERROR sending joystick1 values");
		Sleep(1000);
		break;
	}
	lastGearValue = currentGearValue;
	Sleep(50);
 }



    return 0;
}
