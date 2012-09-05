using SBC;
using System;
namespace SBC {
public class DynamicClass
{
SteelBattalionController controller;

const int numJoysticks = 1;
const int defaultAxisValue = 100;

//there are lots of ways to do this, enumeration would come to mind, but I"m keeping it simple for now.
const int x_axis = 0;
const int y_axis = 1;
const int z_axis = 2;
const int z_rotation = 3;
const int slider = 4;
const int x_rotation = 5;
const int y_rotation = 6;
const int dial = 7;


	public int getNumJoysticks()
	{
	    return numJoysticks;
	}

    public void Initialize()
    {

		controller = new SteelBattalionController();
		controller.Init(50);//50 is refresh rate in milliseconds

        controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon,                       VirtualKeyCode.VK_Q,                        false);
        controller.AddButtonKeyMapping(ButtonEnum.RightJoyFire,                             VirtualKeyCode.VK_W,                        true);
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
    // return number of buttons per joystick, program will then ask for values for each button (only using one joystick right now)
    public int getNumButtons(int joyNum)
    {
        return 13;
    }

    //return number of axis per joystick, program will then ask for values for each axis  (only using one joystick right now)
    public int getNumAxis(int joyNum)
    {
        return 8;//8 happens to be the maximum number of axes that PPJoy/Windows currently supports
    }
    public int getAxisValue(int joyNum, int axisNum)
    {
        //technically we should be accounting the joystickNumber as well, but only dealing with one joystick for the moment
        //we should handle all cases from 0 to getNumAxis()
        switch (axisNum)
        {
            //case 0 means we asked for what is the value to axis number 0
            case x_axis:
                return controller.AimingX;//we are assigning controller.AimingX to PPJoy axis 0
            case y_axis:
                    return controller.AimingY;
            case z_axis:
                return -1*(controller.RightPedal - controller.MiddlePedal);//throttle;
            case z_rotation:
                    return controller.RotationLever;
            case slider:
                return controller.SightChangeX;
            case x_rotation:
                return controller.SightChangeY;
            case y_rotation:
                return controller.LeftPedal;
            case dial:
                return controller.GearLever;

        }
        return defaultAxisValue;//we always have to return something
    }
    
    //we only want to use keymapping or buttonmapping for now, no reason you can't do both, but it might end up being confusing having
    //a button that both presses a joystick button and a key
    public bool useButtons()
    {
        return false;//using keymapping for now
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
    }
}
}