#include <windows.h>
#pragma once
#include "ppjioctl.h"
#define	NUM_ANALOG	8		/* Number of analog values which we will provide */
#define	NUM_DIGITAL	16		/* Number of digital values which we will provide */

#pragma pack(push,1)		/* All fields in structure must be byte aligned. */
typedef struct
{
 unsigned long	Signature;				/* Signature to identify packet to PPJoy IOCTL */
 char			NumAnalog;				/* Num of analog values we pass */
 long			Analog[NUM_ANALOG];		/* Analog values */
 char			NumDigital;				/* Num of digital values we pass */
 char			Digital[NUM_DIGITAL];	/* Digital values */
}	JOYSTICK_STATE;
#pragma pack(pop)

ref class joystick
{
public:
	joystick(void);
	int init(TCHAR* DevName);
	void setAxis(int axisNumber,int value);
	void setButton(int buttonNumber,int value);
	DWORD joystick::sendBuffer();
private:
	TCHAR				*DevName;
	HANDLE				h;
	char				ch;
	JOYSTICK_STATE*		JoyState;
	DWORD				RetSize;
	DWORD				rc;

	long				*Analog;
	char				*Digital;
};

