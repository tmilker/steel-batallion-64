
```
//If you want a quick overview of how the configuration system works, take a look at SolExodus.cs
//This example was meant to recreate the functionality I displayed for the system in the original release
//however that also means that it is actually pretty complicated.
using SBC;
using myVJoyWrapper;
using System;
//using Microsoft.DirectX.DirectInput;
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

         controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch,            true, 3,    Microsoft.DirectX.DirectInput.Key.A, true);//last true means if you hold down the button,
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

  for(int i=1;i<=32;i++)
  {
   joystick.setButton((bool)controller.GetButtonState(i-1),(uint)1,(char)(i-1));
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
```

So here is the code from the example Simple.cs configuration file.  It is standard C# code.  To begin with the majority of changes you'll want to make will be within the Initialize function.  Currently as an example I added the line
```
controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch,            true, 3,    Microsoft.DirectX.DirectInput.Key.A, true);//last true means if you hold down the button,
```

First variable defines which key on the SteelBatallionController you want to affect, here is a list of available keys and their proper name.  (Capitalization does matter)

```
public enum ButtonEnum {
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
}
```


As you can see by the code, we're talking about the CockpitHatch key located under the eject button on the far right hand side of the controller.

The second parameter controls state change.  if you change the parameter from true to false, then the button becomes a statebutton, useful for modes such as nightvision where you press the button to toggle the state.

```
controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch,            false, 3,    Microsoft.DirectX.DirectInput.Key.A, true);//last true means if you hold down the button,
```
The third parameter controls the light intensity, any value from 1 - 15 is acceptable.

In order to define what key you want the button to press you need to know the DirectX.DirectInput key enumerations, they are available here:
[http://msdn.microsoft.com/en-us/library/windows/desktop/bb321074(v=vs.85).aspx](http://msdn.microsoft.com/en-us/library/windows/desktop/bb321074(v=vs.85).aspx)

If you make a mistake typing one in, the program will let you know inside the text box labeled Errors.  it'll tell you a line and column number of where the mistake was made.

The last variable for AddButtonKeyLightMapping controls whether or not the button is held down when you press it.


```
controller.AddButtonKeyLightMapping(ButtonEnum.CockpitHatch,            true, 3,    Microsoft.DirectX.DirectInput.Key.A, false);//last true means if you hold down the button,
```

I can't think of when you would want this to be the case, it means if you press and hold the cockpit hatch button you'll only get on letter a instead of aaaaaaaaaaaaaaaaaa

For configuring axes:
To be more specific, the axes are set within the mainloop section of the code:
```
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
 
 
for(int i=1;i<=32;i++)
{
joystick.setButton((bool)controller.GetButtonState(i-1),(uint)1,(char)(i-1));
}
 
joystick.sendUpdate(1);
 
}
```

the important part of this code is the setAxis function
```
joystick.setAxis(1,controller.AimingX,HID_USAGES.HID_USAGE_X);
```

the first parameter specifies the virtual joystick we're assigning. I can't think of a situation where you would want to use more than one virtual joystick, so leave it as 1

the second parameter corresponds to one of the axes available on the SB controller:
the available axes are:
```
/// Corresponds to the "Rotation Lever" joystick on the left.
RotationLever
 
/// Corresponds to the "Sight Change" analog stick on the "Rotation Lever" joystick.  X Axis value.
SightChangeX
 
/// Corresponds to the "Sight Change" analog stick on the "Rotation Lever" joystick.  Y Axis value.
SightChangeY
 
/// Corresponds to the "Aiming Lever" joystick on the right.  X Axis value.
AimingX
 
/// Corresponds to the "Aiming Lever" joystick on the right.  Y Axis value.
AimingY
 
/// Corresponds to the left pedal on the pedal block
LeftPedal
 
/// Corresponds to the middle pedal on the pedal block
MiddlePedal
 
/// Corresponds to the right pedal on the pedal block
RightPedal
 
/// Corresponds to the tuner dial position.  The 9 o'clock postion is 0, and the 6 o'clock position is 12.
/// The blank area between the 6 and 9 o'clock positions is 13, 14, and 15 clockwise.
TunerDial
 
/// Corresponds to the gear lever on the left block.
GearLever
```

So we have 10 axes available and only 8 places for them to be mapped to. The middle and right pedals naturally map to one axis in most games. And things such as the gear lever and the tunerdial are actually discrete axes that only have a few possible states.

the third parameter is the vJoy axes you want it mapped to, these names should correspond to the names you see on the calibration screen.

```
HID_USAGES.HID_USAGE_X//shows up as part of calibration square on first screen
HID_USAGES.HID_USAGE_Y//shows up as part of calibration square on first screen
HID_USAGES.HID_USAGE_Z//Z axis --third screen
HID_USAGES.HID_USAGE_RZ//Z Rotation - sixth screen
HID_USAGES.HID_USAGE_SL0//Slider - seventh screen
HID_USAGES.HID_USAGE_RX// X Rotation -- fourth screen
HID_USAGES.HID_USAGE_RY// Y Rotation -- fifth screen
HID_USAGES.HID_USAGE_SL1//Dial -- eighth screen
```

Thats its for the basics. Of course since the files are C# based, you can always do some slightly more complicated programming such as
```
joystick.setAxis(1,(controller.RightPedal - controller.MiddlePedal),HID_USAGES.HID_USAGE_Z);//throttle
```

Or you can use fancier things such as exponential. If you find your axes are reversed, you can always multiply by -1.
```
joystick.setAxis(1,-1*(controller.RightPedal - controller.MiddlePedal),HID_USAGES.HID_USAGE_Z);//throttle
```