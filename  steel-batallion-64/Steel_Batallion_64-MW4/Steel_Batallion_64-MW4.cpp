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
bool jumpPressed = false;
bool stopPressed = false;//used in special handling of left pedal
int  pedalTriggerLevel = 50;
int zoomState = -2;//used in special handling of gear lever for zooming
bool performingAlphaStrike =false;
int thumbstickDeadZone = 20;

#include "joystick.h"//having issues with this, it won't find this in debug mode unless I use an explicit directory
int comShift = 0;

void updatePOVhat(SBC::SteelBattalionController^ controller)
{
	SBC::POVdirection lastDirection = controller->POVhat;

	if(( (::abs(controller->SightChangeX) > ::thumbstickDeadZone) || (::abs(controller->SightChangeY) > ::thumbstickDeadZone) ) && (controller->GearLever == -2) )//reverse)
	{
		if(::abs(controller->SightChangeX) > ::abs(controller->SightChangeY))
			if(controller->SightChangeX <0)
				controller->POVhat = SBC::POVdirection::LEFT;
			else
				controller->POVhat = SBC::POVdirection::RIGHT;
		else
			if(controller->SightChangeY <0)
				controller->POVhat = SBC::POVdirection::DOWN;
			else
				controller->POVhat = SBC::POVdirection::UP;

	}
	else
	{
		controller->POVhat = SBC::POVdirection::CENTER;	
	}

	if(lastDirection != controller->POVhat)
	{
		switch(lastDirection)
		{
			case SBC::POVdirection::LEFT:
				controller->sendKeyUp(SBC::VirtualKeyCode::LEFT);
				break;
			case SBC::POVdirection::RIGHT:
				controller->sendKeyUp(SBC::VirtualKeyCode::RIGHT);
				break;
			case SBC::POVdirection::DOWN:
				controller->sendKeyUp(SBC::VirtualKeyCode::VK_I);
				break;
			case SBC::POVdirection::UP:
				controller->sendKeyUp(SBC::VirtualKeyCode::VK_M);
				break;
		}
	}
	else
	{
		switch(controller->POVhat)
		{
			case SBC::POVdirection::LEFT:
				controller->sendKeyDown(SBC::VirtualKeyCode::LEFT);
				break;
			case SBC::POVdirection::RIGHT:
				controller->sendKeyDown(SBC::VirtualKeyCode::RIGHT);
				break;
			case SBC::POVdirection::DOWN:
				controller->sendKeyDown(SBC::VirtualKeyCode::VK_I);
				break;
			case SBC::POVdirection::UP:
				controller->sendKeyDown(SBC::VirtualKeyCode::VK_M);
				break;
		}
	}
}

void evaluateModifiableButton(SBC::SteelBattalionController^ controller,SBC::ButtonEnum toggleButton,SBC::ButtonEnum mainButton,SBC::VirtualKeyCode state1,SBC::VirtualKeyCode state2)
{
	//deal with RightJoyMainWeapon and firing groups 3/4
	if(controller->GetButtonState((int)toggleButton))//up position
	{
		if((int)controller->GetButtonKey(mainButton) == (int)state1)
			controller->AddButtonKeyMapping(mainButton,state2,false);//had to rename delete to delete_key because delete is reserved
	}
	else
	{
		if(controller->GetButtonKey(mainButton) == state2)
			controller->AddButtonKeyMapping(mainButton,state1,false);
	}
}

void evaluateDualLeftPedal(SBC::SteelBattalionController^ controller,SBC::VirtualKeyCode jumpKey,SBC::VirtualKeyCode stopKey)
{
	if(controller->LeftPedal > pedalTriggerLevel)
	{
		//take care of the button logic separately, to be less confusing
		if(!jumpPressed)//if not currently holding down jump key
		{
			if(controller->RightPedal > pedalTriggerLevel || controller->MiddlePedal > pedalTriggerLevel)
			{
				controller->sendKeyDown(jumpKey);
				jumpPressed = true;
			}
		}
		else//jump button was pressed
		{//adding these so that else if won't get optimized into one statement
			if(controller->RightPedal < pedalTriggerLevel && controller->MiddlePedal < pedalTriggerLevel)
			{
				controller->sendKeyUp(jumpKey);
				jumpPressed = false;
			}
		}

		if(!stopPressed)//if not currently holding down stop key
		{
			if(controller->RightPedal < pedalTriggerLevel && controller->MiddlePedal < pedalTriggerLevel)
			{
				controller->sendKeyDown(stopKey);//send fullstop command
				stopPressed = true;
			}
		}
		else//stop button was pressed
		{
			if(controller->RightPedal > pedalTriggerLevel || controller->MiddlePedal > pedalTriggerLevel)
			{
				controller->sendKeyUp(stopKey);
				stopPressed = false;
			}
		}
	}
	else
	{
		if(stopPressed)
		{
			controller->sendKeyUp(stopKey);
			stopPressed = false;
		}
		if(::jumpPressed)
		{
			controller->sendKeyUp(jumpKey);
			::jumpPressed = false;
		}
	}
}

//this seems to work slowly, I'm not sure why.
static void controller_ButtonStateChanged(SBC::SteelBattalionController ^ controller, array<SBC::ButtonState ^> ^ stateChangedArray) 
{
	// Typically, you want to check which buttons were triggered.  This array contains
	// a list of all buttons, their current values, and whether their state has changed.
	if (stateChangedArray[(int) SBC::ButtonEnum::TunerDialStateChange]->changed) 
	{
		int previousCom = comShift;
		
		if(controller->TunerDial <= 4)
			comShift = 0;
		else
			comShift = 1;

		if((previousCom != comShift))//only switch light on downpress
		{
			int startingValue = (int)SBC::ButtonEnum::Comm1;
			int F1Key = (int)SBC::VirtualKeyCode::F1;
			if(comShift == 1)
			{
				for(int i = startingValue; i<startingValue+5;i++)
				{
					controller->SetLEDState((SBC::ControllerLEDEnum)(i+1),15,true);
					Sleep(10);
					controller->SetLEDState((SBC::ControllerLEDEnum)(i+1),0,true);
					SBC::VirtualKeyCode virtualKey = (SBC::VirtualKeyCode)((int)SBC::VirtualKeyCode::F1+(i-startingValue+5));
					SBC::ButtonEnum button = (SBC::ButtonEnum) i;
					controller->AddButtonKeyMapping(button,virtualKey,false);
				}
			}
			else
			{
				for(int i = startingValue+4; i>=startingValue;i--)
				{
					controller->SetLEDState((SBC::ControllerLEDEnum)(i+1),15,true);
					Sleep(10);
					controller->SetLEDState((SBC::ControllerLEDEnum)(i+1),0,true);//lights are shifted by 1 compared to buttons
					controller->AddButtonKeyMapping((SBC::ButtonEnum) i,(SBC::VirtualKeyCode)((int)SBC::VirtualKeyCode::F1+(i-startingValue)),false);
				}
			}
		}
	}
}


int main(array<System::String ^> ^args)
{
joystick ^ joystick1 = gcnew joystick;
int lastGearValue;
bool lastResetValue;
bool currentResetValue;
Hashtable^ ButtonsHash	=	gcnew ::Hashtable();
Hashtable^ KeysHash		=	gcnew ::Hashtable();//hash table of all valid keys to map to
Hashtable^ HOH			=	gcnew ::Hashtable();//hash of hashes
SBC::SteelBattalionController^ controller;

// Initialize the controller
controller = gcnew SBC::SteelBattalionController();
controller->Init(50);
// Add the event handler to monitor button state changed events
controller->ButtonStateChanged += gcnew SBC::SteelBattalionController::ButtonStateChangedDelegate(controller_ButtonStateChanged);



if(!joystick1->init(L"\\\\.\\PPJoyIOCTL1") < 0)
{
	printf("Unable to open PPJoy Virtual Joystick 1, check the Game Controllers panel.");
	Sleep(2000);
	exit(1);
}


FILE * keyStream = fopen("ValidKeys.txt","w");
FILE * buttonStream = fopen("Buttons.txt","w");

fprintf(keyStream,"[Keys]\n");
for each(Object^ key in Enum::GetValues(SBC::VirtualKeyCode::typeid))
{
	//printf("%s = %i\n",o->ToString(), System::Convert::ToInt32(Enum::Format( SBC::VirtualKeyCode::typeid, o,  "D" )) );
	int value = System::Convert::ToInt32(Enum::Format( SBC::VirtualKeyCode::typeid, key,  "D" ));
	KeysHash[key->ToString()->ToUpper()] = value;
	fprintf(keyStream,"%s = %i\n",key->ToString(),  value);
}
fclose(keyStream);


fprintf(buttonStream,"[Buttons]\n");
for each(Object^ button in Enum::GetValues(SBC::ButtonEnum::typeid))
{
	int value = System::Convert::ToInt32(Enum::Format( SBC::ButtonEnum::typeid, button,  "D" ));
	ButtonsHash[button->ToString()->ToUpper()] = value;
	fprintf(buttonStream,"%s = %i \n",button->ToString()->ToUpper(),value);
}
fclose(buttonStream);


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

//SET button key mapping
if(HOH->Contains("BUTTONKEYMAPPINGS"))
for each(::Object ^ button in ((::Hashtable ^)HOH["BUTTONKEYMAPPINGS"])->Keys)
	{
		SBC::ButtonEnum buttonInput = (SBC::ButtonEnum)ButtonsHash[button];
		::Hashtable ^ buttonMap = (::Hashtable ^)HOH["BUTTONKEYMAPPINGS"];
		int temp = (int)KeysHash[(String ^)buttonMap[button]];
		SBC::VirtualKeyCode keyCode = (SBC::VirtualKeyCode)temp;

		controller->AddButtonKeyMapping(buttonInput,keyCode,true);//true means send separate keydown/keyup commands
	}

//SET modifier button key mapping
if(HOH->Contains("BUTTONKEYMAPPINGS") && HOH->Contains("BUTTONMODIFIERS"))
for each(::Object ^ button in ((::Hashtable ^)HOH["BUTTONMODIFIERS"])->Keys)
	{
		SBC::ButtonEnum buttonInput = (SBC::ButtonEnum)ButtonsHash[button];
		::Hashtable ^ buttonMap = (::Hashtable ^)HOH["BUTTONKEYMAPPINGS"];
		::Hashtable ^ modifierMap = (::Hashtable ^)HOH["BUTTONMODIFIERS"];
		if(buttonMap->Contains(button))
		{
			int originalKey = (int) KeysHash[(String ^)buttonMap[button]];
			int modifierKey = (int) KeysHash[(String ^)modifierMap[button]];
			controller->AddButtonKeyMapping(buttonInput,(SBC::VirtualKeyCode)modifierKey,(SBC::VirtualKeyCode)originalKey,false);//false means don't send separate keydown/keyup commands
		}
	}

//set this manually as it is manipulated within loop
controller->AddButtonKeyMapping(SBC::ButtonEnum::RightJoyMainWeapon,SBC::VirtualKeyCode::DELETE_key,false);//had to rename delete to delete_key because delete is reserved
controller->AddButtonKeyMapping(SBC::ButtonEnum::RightJoyFire,SBC::VirtualKeyCode::RETURN,false);//had to rename delete to delete_key because delete is reserved
   

 Console::WriteLine(L"Welcome to Steel Batallion 64");
 Console::WriteLine(L"Leave this running while you play");
 Console::WriteLine(L"This program will update PPJoy Virtual Joystick 1");

 while(1)
 {

	//printf("%s\n",controller->GetBinaryBuffer(4,6));
	joystick1->setAxis(0,controller->AimingX);


	updatePOVhat(controller);//updates POVhat, and sends appropriate keypress downs and ups depending on gear lever and thumbstick position
		

	if(controller->GearLever > -2)//reverse
	{
		joystick1->setAxis(1,controller->AimingY-controller->SightChangeY);
		joystick1->setAxis(3,controller->RotationLever+controller->SightChangeX);
	}
	else
	{
		joystick1->setAxis(1,controller->AimingY);//pitch up/down
		joystick1->setAxis(3,controller->RotationLever);//torso twist
	}


	joystick1->setAxis(2,-1*(controller->RightPedal - controller->MiddlePedal));//throttle
	
	
	evaluateDualLeftPedal(controller,SBC::VirtualKeyCode::VK_J,SBC::VirtualKeyCode::VK_1);
				

	currentResetValue = controller->GetButtonState((int)SBC::ButtonEnum::ToggleFuelFlowRate);
	if(currentResetValue != lastResetValue && currentResetValue)
	{
		controller->TestLEDs(1);//reset lights
	}
	lastResetValue = currentResetValue;


	evaluateModifiableButton(controller,SBC::ButtonEnum::ToggleFilterControl,SBC::ButtonEnum::RightJoyMainWeapon,SBC::VirtualKeyCode::PRIOR,SBC::VirtualKeyCode::DELETE_key);

	if(!controller->GetButtonState((int)SBC::ButtonEnum::RightJoyFire) && ::performingAlphaStrike)//we were performing alphastrike and now we're not
		::performingAlphaStrike = false;

	if(controller->GetButtonState((int)SBC::ButtonEnum::ToggleOxygenSupply))
		if(controller->GetButtonState((int)SBC::ButtonEnum::RightJoyFire) && !::performingAlphaStrike)
		{
			controller->sendKeyPress(SBC::VirtualKeyCode::INSERT);//group1
			controller->sendKeyPress(SBC::VirtualKeyCode::HOME);//group2
			controller->sendKeyPress(SBC::VirtualKeyCode::PRIOR);//group3
			controller->sendKeyPress(SBC::VirtualKeyCode::DELETE_key);//group4
			controller->sendKeyPress(SBC::VirtualKeyCode::END);//group5
			controller->sendKeyPress(SBC::VirtualKeyCode::NEXT);//group6
			::performingAlphaStrike = true;
		}

	if(joystick1->sendBuffer()<1)
	{
		printf("ERROR sending joystick1 values");
		Sleep(1000);
		break;
	}
	Sleep(50);
 }



    return 0;
}
