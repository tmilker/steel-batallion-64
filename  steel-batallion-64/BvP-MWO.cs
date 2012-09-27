// MWO Config File
// version 0.3
// by von Pilsner (thanks to HackNFly!@!!!)
//
// Uses default MWO Keybindings as of Sept 26, 2012


using System;
using Microsoft.DirectX.DirectInput;
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

const int refreshRate = 20;//number of milliseconds between call to mainLoop

	//this gets called once by main program
    public void Initialize()
    {
        int baseLineIntensity = 3;//just an average value for LED intensity
        int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

		controller = new SteelBattalionController();
		controller.Init(50);//50 is refresh rate in milliseconds
		//set all SBC.Buttons by default to light up only when you press them down

		for(int i=4;i<4+30;i++)
		{
			if (i != (int)SBC.ButtonLights.Eject)//excluding eject since we are going to flash that one
			controller.AddButtonLightMapping((SBC.ButtonLights)(i-1),(ControllerLEDEnum)(i),true,baseLineIntensity);
		}

		controller.AddButtonKeyMapping(SBC.Buttons.RightJoyMainWeapon,  Microsoft.DirectX.DirectInput.Key.D6, Microsoft.DirectX.DirectInput.Key.D5, true);
		controller.AddButtonKeyMapping(SBC.Buttons.RightJoyLockOn,  Microsoft.DirectX.DirectInput.Key.R, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.Ignition,			true, 3,    Microsoft.DirectX.DirectInput.Key.P, true);
		//controller.AddButtonKeyLightMapping(Buttons.Start,				true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.MultiMonOpenClose,	true, 3,    Microsoft.DirectX.DirectInput.Key.B, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.MultiMonMapZoomInOut,	true, 3,    Microsoft.DirectX.DirectInput.Key.B, true);
		//controller.AddButtonKeyLightMapping(Buttons.MultiMonModeSelect,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		//controller.AddButtonKeyLightMapping(Buttons.MultiMonSubMonitor,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.MainMonZoomIn,		true, 3,    Microsoft.DirectX.DirectInput.Key.Z, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.MainMonZoomOut,		true, 3,    Microsoft.DirectX.DirectInput.Key.Z, true);
		//controller.AddButtonKeyLightMapping(Buttons.FunctionFSS,			true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		//controller.AddButtonKeyLightMapping(Buttons.FunctionManipulator,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.FunctionLineColorChange,	true, 3,    Microsoft.DirectX.DirectInput.Key.H, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.Washing,				true, 3,    Microsoft.DirectX.DirectInput.Key.C, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.Extinguisher,		true, 3,    Microsoft.DirectX.DirectInput.Key.O, true);
		//controller.AddButtonKeyLightMapping(Buttons.Chaff,				true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		//controller.AddButtonKeyLightMapping(Buttons.FunctionTankDetach,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.FunctionOverride,	true, 3,    Microsoft.DirectX.DirectInput.Key.O, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.FunctionNightScope,	true, 3,    Microsoft.DirectX.DirectInput.Key.N, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.FunctionF1,			true, 3,    Microsoft.DirectX.DirectInput.Key.Tab, true);
		//controller.AddButtonKeyLightMapping(Buttons.FunctionF2,			true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.FunctionF3,			true, 3,    Microsoft.DirectX.DirectInput.Key.LeftControl, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.WeaponConMain,		true, 3,    Microsoft.DirectX.DirectInput.Key.RightControl, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.WeaponConSub,		true, 3,    Microsoft.DirectX.DirectInput.Key.BackSpace, true);
		//controller.AddButtonKeyLightMapping(Buttons.WeaponConMagazine,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.Comm1,	true, 3,    Microsoft.DirectX.DirectInput.Key.F6, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.Comm2,	true, 3,    Microsoft.DirectX.DirectInput.Key.F8, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.Comm3,	true, 3,    Microsoft.DirectX.DirectInput.Key.F9, true);
		//controller.AddButtonKeyLightMapping(Buttons.Comm4,	true, 3,    Microsoft.DirectX.DirectInput.Key.X, true);
		controller.AddButtonKeyLightMapping(SBC.ButtonLights.Comm4,	true, 3,    Microsoft.DirectX.DirectInput.Key.RightBracket, true);
		/*controller.AddButtonKeyMapping(SBC.Buttons.LeftJoySightChange,  Microsoft.DirectX.DirectInput.Key.Z, true);
*/
		joystick = new vJoy();
		acquired = joystick.acquireVJD(1);
		joystick.resetAll();//have to reset before we use it*/
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

	void updatePOVhat()
	{
	int thumbstickDeadZone = 10;
		POVdirection lastDirection = controller.POVhat;

		if(( (Math.Abs(controller.SightChangeX) > thumbstickDeadZone) || (Math.Abs(controller.SightChangeY) > thumbstickDeadZone) ) && (controller.GearLever == -2) )//reverse)
		{
			if(Math.Abs(controller.SightChangeX) > Math.Abs(controller.SightChangeY))
				if(controller.SightChangeX <0)
					controller.POVhat = POVdirection.LEFT;
				else
					controller.POVhat = POVdirection.RIGHT;
			else
				if(controller.SightChangeY <0)
					controller.POVhat = POVdirection.DOWN;
				else
					controller.POVhat = POVdirection.UP;

		}
		else
		{
			controller.POVhat = POVdirection.CENTER;	
		}

		if(lastDirection != controller.POVhat)
		{
			switch(lastDirection)
			{
				case POVdirection.LEFT:
					controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Left);
					break;
				case POVdirection.RIGHT:
					controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Right);
					break;
				case POVdirection.DOWN:
					controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Up);
					break;
				case POVdirection.UP:
					controller.sendKeyUp(Microsoft.DirectX.DirectInput.Key.Down);
					break;
			}
		}
		else
		{
			switch(controller.POVhat)
			{
				case POVdirection.LEFT:
					controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Left);
					break;
				case POVdirection.RIGHT:
					controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Right);
					break;
				case POVdirection.DOWN:
					controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Up);
					break;
				case POVdirection.UP:
					controller.sendKeyDown(Microsoft.DirectX.DirectInput.Key.Down);
					break;
			}
		}
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
		
		joystick.setContPov(1,getDegrees(controller.SightChangeX,controller.SightChangeY),1);

		// toggle tricks!!!
	
			if(controller.GetButtonState(34)) //FILT Toggle
			{
					controller.AddButtonKeyMapping(Buttons.RightJoyFire,  Microsoft.DirectX.DirectInput.Key.BackSlash, true);
					joystick.setButton((bool)controller.GetButtonState(1),(uint)1,(char)(15));
			}
			else
			{
					controller.AddButtonKeyMapping(Buttons.RightJoyFire,  Microsoft.DirectX.DirectInput.Key.NoConvert, true);
					joystick.setButton((bool)controller.GetButtonState(1),(uint)1,(char)(0));
			}
			
			if(controller.GetButtonState(35)) //O2 Supply Toggle
			{
					controller.AddButtonKeyMapping(Buttons.RightJoyMainWeapon,  Microsoft.DirectX.DirectInput.Key.D6, true);
			}
			else
			{
					controller.AddButtonKeyMapping(Buttons.RightJoyMainWeapon,  Microsoft.DirectX.DirectInput.Key.D5, true);
			}

			if(controller.GetButtonState(36)) //Fuel Flow Toggle
			{
					//controller.AddButtonKeyMapping(Buttons.RightJoyFire,  Microsoft.DirectX.DirectInput.Key.BackSlash, true);
			}
			else
			{
					//controller.AddButtonKeyMapping(Buttons.RightJoyFire,  Microsoft.DirectX.DirectInput.Key.Return, true); //I don't know how to bind mouse 1
			}

			if(controller.GetButtonState(37)) //Buffer Toggle
			{
					//controller.AddButtonKeyMapping(Buttons.RightJoyFire,  Microsoft.DirectX.DirectInput.Key.BackSlash, true);
			}
			else
			{
					//controller.AddButtonKeyMapping(Buttons.RightJoyFire,  Microsoft.DirectX.DirectInput.Key.Return, true); //I don't know how to bind mouse 1
			}

			if(controller.GetButtonState(38)) //VT-Location Toggle
			{
					//controller.AddButtonKeyMapping(Buttons.RightJoyFire,  Microsoft.DirectX.DirectInput.Key.BackSlash, true);
			}
			else
			{
					//controller.AddButtonKeyMapping(Buttons.RightJoyFire,  Microsoft.DirectX.DirectInput.Key.Return, true); //I don't know how to bind mouse 1
			}
			
		
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

		joystick.sendUpdate(1);
	}
	
	//this gets called at the end of the program and must be present, as it cleans up resources
	public void shutDown()
	{
		controller.UnInit();
		joystick.Release(1);
	}
}
}