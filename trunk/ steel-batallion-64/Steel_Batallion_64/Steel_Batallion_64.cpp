// Console_clr.cpp : main project file.

#include "stdafx.h"
#include <stdio.h>
#include <conio.h>
#include <windows.h>

#include <winioctl.h>

using namespace System;

#include "joystick.h"


int main(array<System::String ^> ^args)
{
array<joystick^> ^ joysticks = gcnew array<joystick^>(3);

for(int i=0;i<joysticks->Length;i++)
{
	joysticks[i] = gcnew joystick;
}

joysticks[0]->init(L"\\\\.\\PPJoyIOCTL1");
//joysticks[1]->init(L"\\\\.\\PPJoyIOCTL2");
//joysticks[2]->init(L"\\\\.\\PPJoyIOCTL3");

SBC::SteelBattalionController^ controller;



// Initialize the controller
controller = gcnew SBC::SteelBattalionController();
controller->Init(50);

 Console::WriteLine(L"Hello World");

 while(1)
 {

	joysticks[0]->setAxis(0,controller->AimingX);
	joysticks[0]->setAxis(1,controller->AimingY);
	controller->GetButtonState(SBC::ButtonEnum::Chaff);
	joysticks[0]->setButton(0,controller->GetButtonState(SBC::ButtonEnum::RightJoyFire));
	joysticks[0]->setButton(1,controller->GetButtonState(SBC::ButtonEnum::RightJoyLockOn));
	joysticks[0]->setButton(2,controller->GetButtonState(SBC::ButtonEnum::RightJoyMainWeapon));




	for(int i=0;i<joysticks->Length;i++)
	{
		if(joysticks[i]->sendBuffer()<1)
		{
			printf("ERROR sending joystick $d values",i);
			Sleep(1000);
			break;
		}
	}
	Sleep(10);
 }



    return 0;
}
