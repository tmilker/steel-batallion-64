# Installation #

**WARNING WARNING WARNING**
You must use an xbox->usb connector adapter NO GREATER than 3 FEET.  They come in 3ft and 6ft versions, the 6 ft version **WILL NOT WORK!!!**

Installation involves a few steps.
Downloading and installing prerequisites,
Installing LibUSB.NET
Installing VJoy Virtual Joystick software.
Plugging in the Steel Batallion controller and installing the drivers.
Running the Steel-Batallion-64-MW4 executable.


# Details #

Here is a synopsis of the video.  (In case it doesn't work)

Download the Steel-Batallion-64 zip file from this site:
http://steel-batallion-64.googlecode.com/files/Steel-Batallion-64.zip

Extract the zip file.

Download these two files:
http://vjoystick.sourceforge.net/redirect_download_x86x64.php
http://vjoystick.sourceforge.net/redirect_download_vJoy2Aps.php

The first file is the vJoy installation program, the second contains the configuration program and vJoy testing programs.

INSTALLATION ORDER MATTERS!  OR at least you'll get less errors that way.  Windows 64-bit versions require Kernel Mode Driver Certification (Expensive!).  They will complain if you don't have it.  Neither vJoy nor LibUSB.NET seem to have satisfying credentials as far as Windows is concerned.  In order to bypass this hurdle, you have to have Windows in Test Mode.  The vJoy installer takes this into account and the first time you open it (if you're not in test mode) it will give you a warning, and after clicking OK, it will restart your computer and place you into test Mode.  Upon restart it will allow you to continue the installation.



Continue the vJoy installation.  It will warn you saying that Windows can't verify the software, this is normal as it does not have Kernel Mode Driver Certification.  Depending on your machine the installation process can take up to 5 minutes.  Took about 2-3 on mine.

After installing vJoy go to the directory where you downloaded vJoy2.0.1Apps.zip and unzip it.  Since this program was written specifically for 64bit machines I'll assume you are using one and recommend you use the 64 bit configuration program.

Go on and run:
vJoy2.0.1Apps\vJoyConf\vJoyConfx64\vJoyConf

Check off all the axes as well as the additional axes.  Set the POVs to 0 and set the number of buttons to 32.

It should look like this:
![http://imageshack.us/a/img402/6518/vjoyconfsettings.png](http://imageshack.us/a/img402/6518/vjoyconfsettings.png)

Then click Apply.  You should hear the same noise you usually hear when you unplug and replug a USB device.  vJoy drive should now be ready.

To test it out, go to Game Controllers tab in Windows, you can do this by typing in "set up usb" in the Windows 7 Search bar.  Its called "Set up USB game controllers"

It should look like this:
![http://imageshack.us/a/img208/2446/setupusb.png](http://imageshack.us/a/img208/2446/setupusb.png)

Click on vJoy Device, and click on the Properties Button.
It'll bring up the vJoy Device Properties window.  Click on the Test tab as circled below:
![http://imageshack.us/a/img594/6850/vjoydeviceproperties.png](http://imageshack.us/a/img594/6850/vjoydeviceproperties.png)

If you configured everything correctly and added the extra axes and buttons it should look like this:
![http://imageshack.us/a/img6/9487/vjoydevicetesttab.png](http://imageshack.us/a/img6/9487/vjoydevicetesttab.png)




Close everything out and install LibUsbDotNet\_Setup.2.2.8.
http://sourceforge.net/projects/libusbdotnet/files/latest/download

Nothing special here, just SKIP the USB filter section at the end, its not necessary.  If you had tried to install this before installing PPJoy and were not in Windows Test Mode then an error would show up over your Steel Batallion controller when you plugged it in

![http://imageshack.us/a/img40/3258/sb64003.png](http://imageshack.us/a/img40/3258/sb64003.png)

Plug in your Steel Batallion Controller let it Install, Open up Device Manager and right click on the Steel Batallion Controller and select update drivers,

---

[UPDATE!!!!]
If you are running Windows 8, then I feel bad for you, however the modified inf file that is being used wit libUSB.NET is not signed.  In Windows 7 when you turn on Test Mode, you can install uncertified drivers, Windows 8 takes an extra step, you can find out how to do this here:
[since old site is now down](updated.md)
https://learn.sparkfun.com/tutorials/disabling-driver-signature-on-windows-8/disabling-signed-driver-enforcement-on-windows-8

After this is complete, you can finish installing the driver.  Otherwise Windows 8 will complain saying it can't install 3rd party unsigned inf, or something along those lines.

---



> select browse for driver and select the Drivers folder within the Steel Batallion Controller file.
![http://imageshack.us/a/img145/1747/updatedriversoftware.png](http://imageshack.us/a/img145/1747/updatedriversoftware.png)
![http://imageshack.us/a/img825/7083/updatedriverslocation.png](http://imageshack.us/a/img825/7083/updatedriverslocation.png)





You should now have everything you need to run the program, double click on the runSteelBatallion64 shortcut

![http://imageshack.us/a/img585/1166/programopening.png](http://imageshack.us/a/img585/1166/programopening.png)

Next, click File,Open and select Simple.cs from the same directory where runSteelBatallion64 shortcut is located

![http://imageshack.us/a/img28/1848/opencsfile.png](http://imageshack.us/a/img28/1848/opencsfile.png)

Click on Start, if all goes well, no Errors will show up inside the textbox, the Status label will change to Running and the lights on the controller will flash 5 times.

![http://imageshack.us/a/img99/2995/successs.png](http://imageshack.us/a/img99/2995/successs.png)

Calibration:
Calibration is required, check out the calibration page for details.
(Updates coming soon).

That should be it, e-mail me if you have any problems.