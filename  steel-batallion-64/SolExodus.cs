using SBC;
using System;
namespace SBC {
public class DynamicClass
{
SteelBattalionController controller;//the object we actually use to get information from the Steel Batallion Controller

//this is where we define how many joysticks the program is expecting to update, for now we are only using one joystick
//to keep things simple.  Since Windows only supports 8 axis joysticks natively, if you wanted to support all 10 axes
//on the steel batallion you would have to spread it out over 2 joysticks.  I can't think of a reason you would want
//to do that though.  Its easier to mix and match as you need, and you can usually combine the middle and right pedals.
//as I do here.  In case you aren't familiar with C# anything following a // or between /* */ is commented out and
//does not affect actual code
const int numJoysticks = 1;
const int defaultAxisValue = 100;//just a default value to send back to the program if you somehow fail to provide
								 //a return value in the switch statement

//there are lots of ways to do this, enumeration would come to mind, but I"m keeping it simple for now.
//These are equivalent to the names of the axes you would see when calibrating the joystick
const int x_axis = 0;
const int y_axis = 1;
const int z_axis = 2;
const int z_rotation = 3;
const int slider = 4;
const int x_rotation = 5;
const int y_rotation = 6;
const int dial = 7;

int pedalTriggerLevel = 50;//used in special handlign of left pedal, the amount we have to press to get it to trigger the button
bool jumpPressed = false;//boolean used to store when the jump pedal was pressed.
VirtualKeyCode jumpKey = VirtualKeyCode.VK_J;//this is used in the extraCode section and is the key we want pressed when
											//the left pedal gets depressed passed the left pedal trigger level

	//This function must be defined
	public int getNumJoysticks()
	{
	    return numJoysticks;
	}
	
	//Here is a list of all buttons available for mapping
	/*
		RightJoyMainWeapon,
		RightJoyFire,
		RightJoyLockOn,
        Eject,
		CockpitHatch,
		Ignition,
		Start,
		MultiMonOpenClose,
		MultiMonMapZoomInOut,
		MultiMonModeSelect,
		MultiMonSubMonitor,
		MainMonZoomIn,
		MainMonZoomOut,
        FunctionFSS,
        FunctionManipulator,
        FunctionLineColorChange,
		Washing,
		Extinguisher,
		Chaff,
        FunctionTankDetach,
        FunctionOverride,
        FunctionNightScope,
        FunctionF1,
        FunctionF2,
        FunctionF3,
		WeaponConMain,
		WeaponConSub,
		WeaponConMagazine,
		Comm1,
		Comm2,
		Comm3,
		Comm4,
		Comm5,
		LeftJoySightChange,
		ToggleFilterControl,
		ToggleOxygenSupply,
		ToggleFuelFlowRate,
		ToggleBufferMaterial,
		ToggleVTLocation,
		TunerDialStateChange,
		GearLeverStateChange
	*/

	//this function must be defined and is called only once by the program.  This is where most people will do their
	//modifications specific to a game
    public void Initialize()
    {

		controller = new SteelBattalionController();//We have to first initialize the controller
		controller.Init(50);//50 is refresh rate in milliseconds

		//Now add all the keymapping you desire
		//AddButtonKeyMapping requires a Button, a keycode, and whether or not to send separate keydown/ key up presses
        controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon,                       VirtualKeyCode.VK_Q,                        false);//false is equivalent to quickly hitting button once and releasing
        controller.AddButtonKeyMapping(ButtonEnum.RightJoyFire,                             VirtualKeyCode.VK_W,                        true);//true is equal to holding down a key
        controller.AddButtonKeyMapping(ButtonEnum.RightJoyLockOn,                           VirtualKeyCode.VK_E,                        false);
        controller.AddButtonKeyLightMapping(ButtonEnum.Eject,                   true, 15,   VirtualKeyCode.VK_R,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch,            true, 3,    VirtualKeyCode.VK_T,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.Ignition,                true, 15,   VirtualKeyCode.VK_Y,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.Start,                   true, 15,   VirtualKeyCode.VK_U,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonOpenClose,       true, 3,    VirtualKeyCode.VK_I,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonMapZoomInOut,    true, 3,    VirtualKeyCode.VK_O,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonModeSelect,      true, 3,    VirtualKeyCode.VK_P,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonSubMonitor,      true, 7,    VirtualKeyCode.VK_A,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomIn,           true, 3,    VirtualKeyCode.VK_S,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomOut,          true, 3,    VirtualKeyCode.VK_D,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionFSS,             true, 7,    VirtualKeyCode.VK_F,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionManipulator,     true, 3,    VirtualKeyCode.VK_G,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionLineColorChange, true, 7,    VirtualKeyCode.VK_H,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.Washing,                 true, 3,    VirtualKeyCode.VK_J,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.Extinguisher,            true, 3,    VirtualKeyCode.VK_K,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.Chaff,                   true, 3,    VirtualKeyCode.VK_L,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionTankDetach,      true, 3,    VirtualKeyCode.VK_Z,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionOverride,        true, 3,    VirtualKeyCode.VK_X,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionNightScope,      false, 7,   VirtualKeyCode.VK_C,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1,              true, 3,    VirtualKeyCode.VK_V,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF2,              true, 3,    VirtualKeyCode.VK_B,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF3,              true, 3,    VirtualKeyCode.VK_N,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMain,           true, 3,    VirtualKeyCode.VK_M,                        true);
        controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConSub,            true, 3,    VirtualKeyCode.OEM_4,                       true);
        controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMagazine,       true, 7,    VirtualKeyCode.OEM_6,                       true);
	}
	
	//we only want to use keymapping or buttonmapping for now, no reason you can't do both, but it might end up being confusing having
    //a button that both presses a joystick button and a key
    public bool useButtons()
    {
        return false;//using keymapping for now
    }
	
	// Function must be defined, only important if useButtons returns True
    // return number of buttons per joystick, program will then ask for values for each button (only using one joystick right now)
    public int getNumButtons(int joyNum)
    {
        return 13;
    }

	//Function must be defined, to keep things simple, leave this at 8
    //return number of axis per joystick, program will then ask for values for each axis  (only using one joystick right now)
    public int getNumAxis(int joyNum)
    {
        return 8;//8 happens to be the maximum number of axes that PPJoy/Windows currently supports
    }
	
	//HERE is where we assign axis, each case corresponds to the name of an axis you would see under
	//the usb game controller calibration
	//only axis not shown here is the tuner dial axis, which can be accessed using
	//  controller.TunerDial
	
	//
	
	
    public int getAxisValue(int joyNum, int axisNum)
    {
        //technically we should be accounting the joystickNumber as well, but only dealing with one joystick for the moment
        //we should handle all cases from 0 to getNumAxis()
        switch (axisNum)
        {
            //case 0 means we asked for what is the value to axis number 0
            case x_axis:
                return controller.AimingX;//Corresponds to the "Aiming Lever" joystick on the right.  X Axis value.
            case y_axis:
                    return controller.AimingY;//Corresponds to the "Aiming Lever" joystick on the right.  Y Axis value.
            case z_axis:
                return -1*(controller.RightPedal - controller.MiddlePedal);//throttle, combining middle and right pedals
            case z_rotation:
                    return controller.RotationLever;//Corresponds to the "Rotation Lever" joystick on the left.
            case slider:
                return controller.SightChangeX;//Corresponds to the "Sight Change" analog stick on the "Rotation Lever" joystick.  X Axis value.
            case x_rotation:
                return controller.SightChangeY;//Corresponds to the "Sight Change" analog stick on the "Rotation Lever" joystick.  Y Axis value.
            case y_rotation:
                return controller.LeftPedal;
            case dial:
                return controller.GearLever;

        }
        return defaultAxisValue;//we always have to return something
    }
    


    //this function has to be here even if we do not use it
    public bool getButtonValue(int joyNum, int buttonNumber)
    {
        //technically we should be accounting the joystickNumber as well, but only dealing with one joystick for the moment
        return controller.GetButtonState(buttonNumber);
    }

    //this function has to be here even if we do not use it
    public void extraCode()//place any extra code you want executed during the loop here.
    {
		if(controller.LeftPedal > pedalTriggerLevel)
		{
	    	//take care of the button logic separately, to be less confusing
    		if(!jumpPressed)//if not currently holding down jump key
		    {
   				controller.sendKeyDown(jumpKey);
			    jumpPressed = true;
		    }
		}
		else//jump button was pressed
		{		
			if(jumpPressed)
			{
				controller.sendKeyUp(jumpKey);
				jumpPressed = false;
			}
		}	
    }
}
}