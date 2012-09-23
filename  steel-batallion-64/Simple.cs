//If you want a quick overview of how the configuration system works, take a look at SolExodus.cs
//This example was meant to recreate the functionality I displayed for the system in the original release
//however that also means that it is actually pretty complicated.

using SBC;
using myVJoyWrapper;
using System;
namespace SBC {
public class DynamicClass
{
SteelBattalionController controller;
vJoy joystick;
bool acquired;

const int refreshRate = 50;//number of milliseconds between call to mainLoop


	//this gets called once by main program
    public void Initialize()
    {
        int baseLineIntensity = 1;//just an average value for LED intensity
        int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

		controller = new SteelBattalionController();
		controller.Init(50);//50 is refresh rate in milliseconds
		//set all buttons by default to light up only when you press them down

		for(int i=4;i<4+30;i++)
		{
			if (i != (int)ButtonEnum.Eject)//excluding eject since we are going to flash that one
			controller.AddButtonLightMapping((ButtonEnum)(i-1),(ControllerLEDEnum)(i),true,baseLineIntensity);
		}
		 joystick = new vJoy();
		 acquired = joystick.acquireVJD(1);
		 joystick.resetAll();
	}
	
	//this is necessary, as main program calls this to know how often to call mainLoop
	public int getRefreshRate()
	{
		return refreshRate;
	}

	//this gets called once every refreshRate milliseconds by main program
	public void mainLoop()
	{
		joystick.setAxis(1,controller.AimingX,HID_USAGES.HID_USAGE_X);
		joystick.setAxis(1,controller.AimingY,HID_USAGES.HID_USAGE_Y);
		joystick.setAxis(1,(controller.RightPedal - controller.MiddlePedal),HID_USAGES.HID_USAGE_Z);//throttle
		joystick.setAxis(1,controller.RotationLever,HID_USAGES.HID_USAGE_RZ);
		joystick.setAxis(1,controller.SightChangeX,HID_USAGES.HID_USAGE_SL0);		
		joystick.setAxis(1,controller.SightChangeY,HID_USAGES.HID_USAGE_RX);				
		joystick.setAxis(1,controller.LeftPedal,HID_USAGES.HID_USAGE_RY);						
		joystick.setAxis(1,controller.GearLever,HID_USAGES.HID_USAGE_SL1);
		joystick.sendUpdate(1);

		//will create this appropriately later on.  Calling multiple times instead of storing it in buffer.
		for(int i=1;i<=32;i++)
		{
		joystick.setButton((bool)controller.GetButtonState(i-1),(uint)1,(char)i);
		}

	}
}
}