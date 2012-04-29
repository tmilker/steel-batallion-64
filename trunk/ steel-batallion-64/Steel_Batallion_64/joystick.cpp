#include "StdAfx.h"
#include "joystick.h"


joystick::joystick(void)
{
	JoyState = new JOYSTICK_STATE;
	Analog = JoyState->Analog;
	Digital = JoyState->Digital;
	Analog[0]= Analog[1]= Analog[2]= Analog[3]= Analog[4]= Analog[5]= Analog[6]= Analog[7]= (PPJOY_AXIS_MIN+PPJOY_AXIS_MAX)/2;
	for(int i=0;i<NUM_DIGITAL;i++)
		Digital[i] = 0;
}

int joystick::init(TCHAR* DevName)
{
	//DevName= PPJOY_IOCTL_DEVNAME;
		/* Open a handle to the control device for the first virtual joystick. */
	h= CreateFile(DevName,GENERIC_WRITE,FILE_SHARE_WRITE,NULL,OPEN_EXISTING,0,NULL);
	/* Make sure we could open the device! */
	if (h==INVALID_HANDLE_VALUE)
	{
	return -1;
	}

	/* Initialise the IOCTL data structure */
	JoyState->Signature= JOYSTICK_STATE_V1;
	JoyState->NumAnalog= NUM_ANALOG;	/* Number of analog values */
	Analog= JoyState->Analog;			/* Keep a pointer to the analog array for easy updating */
	JoyState->NumDigital= NUM_DIGITAL;	/* Number of digital values */
	Digital= JoyState->Digital;			/* Digital array */
	return 1;
}

void joystick::setAxis(int axisNumber,int value)
{
	Analog[axisNumber] = value;
}

void joystick::setButton(int buttonNumber,int value)
{
	Digital[buttonNumber] = value;
}

DWORD joystick::sendBuffer()
{
	DWORD test;
	if (!DeviceIoControl(h,IOCTL_PPORTJOY_SET_STATE,&(*JoyState),sizeof(*JoyState),NULL,0,&test,NULL))
	{
		rc= GetLastError();
		return rc;
	}
	return 1;
}