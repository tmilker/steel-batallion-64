//If you want a quick overview of how the configuration system works, take a look at SolExodus.cs
//This example was meant to recreate the functionality I displayed for the system in the original release
//however that also means that it is actually pretty complicated.

//using myVJoyWrapper;
using System;
using Microsoft.DirectX.DirectInput;
namespace Steel_Batallion_64_v2 {
public class DynamicClass
{

const int refreshRate = 50;//number of milliseconds between call to mainLoop


	//this gets called once by main program
    public void Initialize()
    {

	}
	
	//this is necessary, as main program calls this to know how often to call mainLoop
	public int getRefreshRate()
	{
		return refreshRate;
	}
	
	//this gets called once every refreshRate milliseconds by main program
	public void mainLoop()
	{


	}
	
	//this gets called at the end of the program and must be present, as it cleans up resources
	public void shutDown()
	{

	}
	
}
}