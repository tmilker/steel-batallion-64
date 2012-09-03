using SBC;
namespace SBC {
public class DynamicClass
{
SteelBattalionController controller;

const int numJoysticks = 1;
const int defaultAxisValue = 100;

//Variables used in extraCode section
bool currentResetValue;
bool lastResetValue;
bool jumpPressed = false;

bool stopPressed = false;//used in special handling of left pedal
int pedalTriggerLevel = 50;
bool performingAlphaStrike = false;
int thumbstickDeadZone = 25;
bool inverseThumbStick = false;
double maxZoomMultiplier = 1.5;//used for integrating thumbstick into zooming system


	public int getNumJoysticks()
	{
	return numJoysticks;
	}
    /*
    public void AddButtonKeyLightMapping(ButtonEnum button, bool lightOnHold, int intensity, VirtualKeyCode keyCode, bool holdDown)
    {
        AddButtonLightMapping(button, lightOnHold, intensity);
        AddButtonKeyMapping(button, keyCode, holdDown);
    }

    public void AddButtonKeyLightMapping(ButtonEnum button, bool lightOnHold, int intensity, VirtualKeyCode keyCode1, VirtualKeyCode keyCode2, bool holdDown)
    {
        AddButtonLightMapping(button, lightOnHold, intensity);
        AddButtonKeyMapping(button, keyCode1, keyCode2, holdDown);
    }
     */

    public void Initialize()
    {
        int baseLineIntensity = 1;//just an average value for LED intensity
        int emergencyLightIntensity = 15;//for stuff like eject,cockpit Hatch,Ignition, and Start

		controller = new SteelBattalionController();
		controller.Init(50);
		//set all buttons by default to light up only when you press them down
		for(int i=4;i<4+30;i++)
		{
			if (i != (int)ButtonEnum.Eject)//excluding eject since we are going to flash that one
			controller.AddButtonLightMapping((ButtonEnum)(i-1),(ControllerLEDEnum)(i),true,baseLineIntensity);
		}
        
		//add buttonlight mapping,false = keep light on, i.e. a button that changes states
        controller.AddButtonKeyLightMapping(ButtonEnum.Eject, true, 15,VirtualKeyCode.VK_Z,true);
        controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.Ignition, true, 15);
        controller.AddButtonKeyLightMapping(ButtonEnum.Start, true, 15);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonOpenClose, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonMapZoomInOut, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonModeSelect, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.MultiMonSubMonitor, true, 7);
        controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomIn, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.MainMonZoomOut, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionFSS, true, 7);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionManipulator, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionLineColorChange, false, 7);//keep light on between presses
        controller.AddButtonKeyLightMapping(ButtonEnum.Washing, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.Extinguisher, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.Chaff, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionTankDetach, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionOverride, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionNightScope, false, 7);//keep light on between presses
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF1, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF2, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.FunctionF3, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMain, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConSub, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.WeaponConMagazine, true, 7);
        controller.AddButtonKeyLightMapping(ButtonEnum.Comm1, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.Comm2, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.Comm3, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.Comm4, true, 3);
        controller.AddButtonKeyLightMapping(ButtonEnum.Comm5, true, 3);*/

        controller.AddButtonKeyMapping(ButtonEnum.RightJoyMainWeapon, VirtualKeyCode.VK_Q, true);//true means send separate keydown/keyup commands




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
            case 0:
                return controller.AimingX;//we are assigning controller.AimingX to PPJoy axis 0
            case 1:
                return controller.AimingY;
            case 2:
                return -1*(controller.RightPedal - controller.MiddlePedal);//throttle;
            case 3:
                return controller.RotationLever;
            case 4:
                return controller.SightChangeX;
            case 5:
                return controller.SightChangeY;
            case 6:
                return controller.LeftPedal;
            case 7:
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
    public bool getButtonValue(int joyNum, int buttonNumber)
    {
        //technically we should be accounting the joystickNumber as well, but only dealing with one joystick for the moment
        return controller.GetButtonState(buttonNumber);
    }

    public void extraCode()//place any extra code you want executed during the loop here.
    {
        currentResetValue = controller.GetButtonState((int)ButtonEnum.ToggleFuelFlowRate);
	    if(currentResetValue != lastResetValue && currentResetValue)
	    {
    		controller.TestLEDs(1);//reset lights
	    }
	    lastResetValue = currentResetValue;

        evaluateDualLeftPedal(controller,VirtualKeyCode.VK_J,VirtualKeyCode.VK_1);
	    evaluateModifiableButton(controller,ButtonEnum.ToggleFilterControl,ButtonEnum.RightJoyMainWeapon,VirtualKeyCode.PRIOR,VirtualKeyCode.DELETE_key);

	    if(!controller.GetButtonState((int)ButtonEnum.RightJoyFire) && performingAlphaStrike)//we were performing alphastrike and now we're not
		    performingAlphaStrike = false;

	    if(controller.GetButtonState((int)ButtonEnum.ToggleOxygenSupply))
		    if(controller.GetButtonState((int)ButtonEnum.RightJoyFire) && !performingAlphaStrike)
		    {
			    controller.sendKeyPress(VirtualKeyCode.INSERT);//group1
			    controller.sendKeyPress(VirtualKeyCode.HOME);//group2
			    controller.sendKeyPress(VirtualKeyCode.PRIOR);//group3
			    controller.sendKeyPress(VirtualKeyCode.DELETE_key);//group4
			    controller.sendKeyPress(VirtualKeyCode.END);//group5
			    controller.sendKeyPress(VirtualKeyCode.NEXT);//group6
			    performingAlphaStrike = true;
		    }
    }

    //extra function added for use in "extraCode" specific to MW4 file
    private void evaluateModifiableButton(SteelBattalionController controller,ButtonEnum toggleButton,ButtonEnum mainButton,VirtualKeyCode state1,VirtualKeyCode state2)
    {
	    //deal with RightJoyMainWeapon and firing groups 3/4
	    if(controller.GetButtonState((int)toggleButton))//up position
	    {
    		if((int)controller.GetButtonKey(mainButton) == (int)state1)
			    controller.AddButtonKeyMapping(mainButton,state2,false);//had to rename delete to delete_key because delete is reserved
	    }
	    else
	    {
    		if(controller.GetButtonKey(mainButton) == state2)
			   controller.AddButtonKeyMapping(mainButton,state1,false);
	    }
    }
    //extra function added for use in "extraCode" specific to MW4 file
    void evaluateDualLeftPedal(SteelBattalionController controller,VirtualKeyCode jumpKey,VirtualKeyCode stopKey)
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

}
}