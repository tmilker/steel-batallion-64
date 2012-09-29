// MWO Config File
// version 0.7
// by von Pilsner (thanks to HackNFly!@!!!)
//
// Uses default MWO keybindings as of Sept 26, 2012
// You must map the 'throttle' axis in-game though...
//
// For use with Steel-Batallion-64_v2_pre.zip 
// 64 bit driver code/glue by HackNFly  http://code.google.com/p/steel-batallion-64/
//
// add the folowing to user.cfg
//
// cl_joystick_gain = 1.35
// cl_joystick_invert_throttle = 0
// cl_joystick_invert_pitch = 1
// cl_joystick_invert_yaw = 0
// cl_joystick_invert_turn = 0
// cl_joystick_throttle_range = 0
//
// updated by Santiago Saldana Sept, 27, 2012

using SBC;
using System;
namespace SBC {
public class DynamicClass
{
SteelBattalionController controller;
vJoy joystick;
bool acquired;

bool jumpPressed = false;

bool stopPressed = false;//used in special handling of left pedal
int pedalTriggerLevel = 50;//used in special handlign of left pedal, 

Microsoft.DirectX.DirectInput.Key jumpKey = Microsoft.DirectX.DirectInput.Key.Space;
Microsoft.DirectX.DirectInput.Key stopKey = Microsoft.DirectX.DirectInput.Key.X;

const int refreshRate = 30;//number of milliseconds between call to mainLoop

	//this gets called once by main program
    public void Initialize()
    {
        int baseLineIntensity = 3;//just an average value for LED intensity
        int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

		controller = new SteelBattalionController();
		controller.Init(50);//50 is refresh rate in milliseconds
		//set all buttons by default to light up only when you press them down

		for(int i=4;i<4+30;i++)
		{
			if (i != (int)ButtonEnum.Eject)//excluding eject since we are going to flash that one
			controller.AddButtonLightMapping((ButtonEnum)(i-1),(ControllerLEDEnum)(i),true,baseLineIntensity);
		}

		controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon,  Microsoft.DirectX.DirectInput.Key.D6, Microsoft.DirectX.DirectInput.Key.D5, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.RightJoyLockOn,true,3,  Microsoft.DirectX.DirectInput.Key.R, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.Eject,					true, 3,    Microsoft.DirectX.DirectInput.Key.O, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.Ignition,				true, 3,    Microsoft.DirectX.DirectInput.Key.P, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.Start,					true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonOpenClose,		true, 3,    Microsoft.DirectX.DirectInput.Key.B, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonMapZoomInOut,	true, 3,    Microsoft.DirectX.DirectInput.Key.B, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonModeSelect,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonSubMonitor,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomIn,			true, 3,    Microsoft.DirectX.DirectInput.Key.Z, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomOut,			true, 3,    Microsoft.DirectX.DirectInput.Key.Z, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.FunctionFSS,			true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.FunctionManipulator,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.FunctionLineColorChange,	true, 3,    Microsoft.DirectX.DirectInput.Key.H, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.Washing,					true, 3,    Microsoft.DirectX.DirectInput.Key.C, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.Extinguisher,			true, 3,    Microsoft.DirectX.DirectInput.Key.O, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.Chaff,					true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.FunctionTankDetach,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.FunctionOverride,		true, 3,    Microsoft.DirectX.DirectInput.Key.O, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.FunctionNightScope,		true, 3,    Microsoft.DirectX.DirectInput.Key.N, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1,				true, 3,    Microsoft.DirectX.DirectInput.Key.Tab, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF2,			true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF3,				true, 3,    Microsoft.DirectX.DirectInput.Key.LeftControl, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMain,			true, 3,    Microsoft.DirectX.DirectInput.Key.RightControl, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConSub,			true, 3,    Microsoft.DirectX.DirectInput.Key.BackSpace, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMagazine,		true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.Comm1,	true, 3,    Microsoft.DirectX.DirectInput.Key.F6, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.Comm2,	true, 3,    Microsoft.DirectX.DirectInput.Key.F8, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.Comm3,	true, 3,    Microsoft.DirectX.DirectInput.Key.F9, true);
		//controller.AddButtonKeyLightMapping(ButtonEnum.Comm4,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(ButtonEnum.Comm5,	true, 3,    Microsoft.DirectX.DirectInput.Key.RightBracket, true);
		controller.AddButtonKeyMapping(ButtonEnum.LeftJoySightChange,  Microsoft.DirectX.DirectInput.Key.Z, true);

		joystick = new vJoy();
		acquired = joystick.acquireVJD(1);
		joystick.resetAll();//have to reset before we use it
	}
	
	//this is necessary, as main program calls this to know how often to call mainLoop
	public int getRefreshRate()
	{
		return refreshRate;
	}
	
	private uint getDegrees(double x,double y)
	{
		uint temp = (uint)(System.Math.Atan(y/x)* (180/Math.PI));
		if(x < 0)
			temp +=180;
		if(x > 0 && y < 0)
			temp += 360;
			
		temp += 90;//origin is vertical on POV not horizontal
			
		if(temp > 360)//by adding 90 we may have gone over 360
			temp -=360;
		
		temp*=100;
		
		if (temp > 35999)
			temp = 35999;
		if (temp < 0)
			temp = 0;
			
		return temp;
	}

	public void updatePOVhat()
	{
	int thumbstickDeadZone = 44;
		SBC.POVdirection lastDirection = controller.POVhat;

		if(( (Math.Abs(controller.SightChangeX) > thumbstickDeadZone) || (Math.Abs(controller.SightChangeY) > thumbstickDeadZone) ))
		{
			if(Math.Abs(controller.SightChangeX) > Math.Abs(controller.SightChangeY))
				if(controller.SightChangeX <0)
					controller.POVhat = SBC.POVdirection.LEFT;
				else
					controller.POVhat = SBC.POVdirection.RIGHT;
			else
				if(controller.SightChangeY <0)
					controller.POVhat = SBC.POVdirection.DOWN;
				else
					controller.POVhat = SBC.POVdirection.UP;

		}
		else
		{
			controller.POVhat = SBC.POVdirection.CENTER;	
		}

		if(lastDirection != controller.POVhat)
		{
			switch(lastDirection)
			{
				case SBC.POVdirection.LEFT:
					controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Left);
					break;
				case SBC.POVdirection.RIGHT:
					controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Right);
					break;
				case SBC.POVdirection.DOWN:
					controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Up);
					break;
				case SBC.POVdirection.UP:
					controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Down);
					break;
			}
		}
		else
		{
			switch(controller.POVhat)
			{
				case SBC.POVdirection.LEFT:
					controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Left);
					break;
				case SBC.POVdirection.RIGHT:
					controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Right);
					break;
				case SBC.POVdirection.DOWN:
					controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Up);
					break;
				case SBC.POVdirection.UP:
					controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Down);
					break;
			}
		}
	}
	
	public void evaluateLeftPedal()
	{
	    	if(controller.LeftPedal > pedalTriggerLevel)
    	{
	    	//take care of the button logic separately, to be less confusing
    		if(!jumpPressed)//if not currently holding down jump key
		    {
			    if(controller.RightPedal > pedalTriggerLevel || controller.MiddlePedal > pedalTriggerLevel)
			    {
    				controller.sendKeyDown(jumpKey);
				    jumpPressed = true;
			    }
		    }
		    else//jump button was pressed
		    {
                //adding these so that else if won't get optimized into one statement
    			if(controller.RightPedal < pedalTriggerLevel && controller.MiddlePedal < pedalTriggerLevel)
	    		{
				    controller.sendKeyUp(jumpKey);
				    jumpPressed = false;
			    }
		    }   

		    if(!stopPressed)//if not currently holding down stop key
		    {
			    if(controller.RightPedal < pedalTriggerLevel && controller.MiddlePedal < pedalTriggerLevel)
			    {
				    controller.sendKeyDown(stopKey);//send fullstop command
				    stopPressed = true;
			    }
		    }
		    else//stop button was pressed
		    {
			    if(controller.RightPedal > pedalTriggerLevel || controller.MiddlePedal > pedalTriggerLevel)
			    {
				    controller.sendKeyUp(stopKey);
				    stopPressed = false;
			    }
		    }
	    }
	    else
	    {
		    if(stopPressed)
		    {
			    controller.sendKeyUp(stopKey);
			    stopPressed = false;
		    }
		    if(jumpPressed)
		    {
			    controller.sendKeyUp(jumpKey);
			    jumpPressed = false;
		    }
	    }
	}

	//this gets called once every refreshRate milliseconds by main program
	public void mainLoop()
	{
	updatePOVhat();
		
		joystick.setAxis(1,controller.AimingX,HID_USAGES.HID_USAGE_X);
		joystick.setAxis(1,controller.AimingY,HID_USAGES.HID_USAGE_Y);
		joystick.setAxis(1,(controller.RightPedal - controller.MiddlePedal),HID_USAGES.HID_USAGE_Z);//throttle
		joystick.setAxis(1,controller.RotationLever,HID_USAGES.HID_USAGE_RZ);
		joystick.setAxis(1,controller.SightChangeX,HID_USAGES.HID_USAGE_SL0);		
		joystick.setAxis(1,controller.SightChangeY,HID_USAGES.HID_USAGE_RX);				
		joystick.setAxis(1,controller.LeftPedal,HID_USAGES.HID_USAGE_RY);						
		joystick.setAxis(1,controller.GearLever,HID_USAGES.HID_USAGE_SL1);
		joystick.setContPov(1,getDegrees(controller.SightChangeX,controller.SightChangeY),1);

		// toggle tricks!!!
			if(controller.GetButtonState(ButtonEnum.ToggleFilterControl)) //FILT Toggle
			{
					controller.AddButtonKeyMapping(ButtonEnum.RightJoyFire,  Microsoft.DirectX.DirectInput.Key.BackSlash, true);
					joystick.setButton(controller.GetButtonState(ButtonEnum.RightJoyFire),1,15);
			}
			else
			{
					controller.AddButtonKeyMapping(ButtonEnum.RightJoyFire,  Microsoft.DirectX.DirectInput.Key.NoConvert, true);
					joystick.setButton(controller.GetButtonState(ButtonEnum.RightJoyFire),1,0);
			}
			
			if(controller.GetButtonState(ButtonEnum.ToggleOxygenSupply)) // O2 Supply Toggle
			{
					controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon,  Microsoft.DirectX.DirectInput.Key.D6, true);
			}
			else
			{
					controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon,  Microsoft.DirectX.DirectInput.Key.D5, true);
			}

		
			evaluateLeftPedal();
			joystick.sendUpdate(1);
		}
		
		//this gets called at the end of the program and must be present, as it cleans up resources
		public void shutDown()
		{
			controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.LeftControl);
			controller.UnInit();
			joystick.Release(1);
		}
	}
}