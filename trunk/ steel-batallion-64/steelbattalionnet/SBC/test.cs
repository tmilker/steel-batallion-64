namespace SBC {
public class DynamicClass
{
SteelBattalionController controller;
    public void Main(string temp)
    {
		controller = new SBC.SteelBattalionController();
		controller.Init(50);/*
		//set all buttons by default to light up only when you press them down
		for(int i=4;i<4+30;i++)
		{
			if (i != (int)SBC.ButtonEnum.Eject)//excluding eject since we are going to flash that one
			controller->AddButtonLightMapping((SBC.ButtonEnum)(i-1),(SBC.ControllerLEDEnum)(i),true,baseLineIntensity);
		}

		//add exceptions to intensity
		//controller->AddButtonLightMapping(SBC.ButtonEnum.Eject,SBC.ControllerLEDEnum.EmergencyEject,true,emergencyLightIntensity);
		controller->AddButtonLightMapping(SBC.ButtonEnum.Ignition,SBC.ControllerLEDEnum.Ignition,true,emergencyLightIntensity);
		controller->AddButtonLightMapping(SBC.ButtonEnum.Start,SBC.ControllerLEDEnum.Start,true,emergencyLightIntensity);

		//add exceptions to toggle state, lightOnHold = false means to toggle light state when pressed
		controller->AddButtonLightMapping(SBC.ButtonEnum.CockpitHatch,SBC.ControllerLEDEnum.CockpitHatch,false,emergencyLightIntensity);//false means toggle light state
		controller->AddButtonLightMapping(SBC.ButtonEnum.FunctionLineColorChange,SBC.ControllerLEDEnum.LineColorChange,false,baseLineIntensity);
		controller->AddButtonLightMapping(SBC.ButtonEnum.FunctionNightScope,SBC.ControllerLEDEnum.NightScope,false,emergencyLightIntensity);//changed intensity for fun


		controller->setGearLights(true,10);*/
	}
}
}