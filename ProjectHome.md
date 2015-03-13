This amazing controller has gotten little attention in the last few years, and with Mechwarrior Online coming up I wanted to take on the challenge of making it work in Windows 7 64 bit.  A driver already exists for 32 bit systems.  This project makes use of the following open source projects:
http://vjoystick.sourceforge.net/site/
http://steelbattalionnet.codeplex.com/

**WARNING WARNING WARNING**
You must use an xbox->usb connector adapter NO GREATER than 3 FEET.  They come in 3ft and 6ft versions, the 6 ft version **WILL NOT WORK!!!**


Currently the project consists of a very simple GUI that creates a glue between the managed C# libraries created on codeplex and the vJoy virtual joystick library.  The goal is to keep the program simple so people can easily modify it.