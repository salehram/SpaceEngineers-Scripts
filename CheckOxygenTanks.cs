//
// defining required variables
//
Color normalTextColor = new Color(0, 0, 255);
Color emergencyTextColor = new Color(255, 0, 0);
IMyTextPanel LCD_ControlRoom_OxygenLevel;
IMyOxygenTank oxygenTank;
IMyOxygenGenerator oxygenGenerator;
float oxygenLevel;
// this float will set the threshold where oxygen generators will turned on if oxygen levels go below it
// if oxygen level is above this threshold, oxygen generators will be turned off
float thresholdLevel = 70.0F;
// end of threshold definition
string screenText = "";
bool oxygenGenerators_Status;
// object names
// change the text in the lcdScreenName to the actual name of your own LCDScreen object that you want it to display the stats
// change/add items in the list oxygenTank_Names to add all the oxygen tanks by their name that you want to work with
// change/add items in the list oxygenGenerator_Names to add all oxygen generators by name that you want to work with
string lcdScreenName = "LCD-Oxygen-Levels";
List<string> oxygenTank_Names = new List<string>(new string[] { "Oxygen Tank 5", "Oxygen Tank 6", "Oxygen Tank 7", "Oxygen Tank 8" });
List<string> oxygenGenerator_Names = new List<string>(new string[] { "Oxygen Generator 2", "Oxygen Generator 3", "Oxygen Generator 4", "Oxygen Generator 5" });
// end of object names
//
// end of required variables definitions
//

void Main(string argument)
{
    // calling checkOxygenLevels method, this method will do:
    //      1. check the oxygen level in all the specified oxygen tanks in the list oxygenTank_Names
    //      2. if one tank is below the threshold level in var thresholdLevel, then it will switch oxygen generators online
    //      3. if all tanks are above the threshold, all oxygen generators will be turned off
    checkOxygenLevels();
}

void checkOxygenLevels()
{
    // setting the status of oxygen generators to false = offline 
    oxygenGenerators_Status = false;
    // setting the default screen text 
    screenText = "Oxygen level:\n";
    // finding the LCD screen we want 
    LCD_ControlRoom_OxygenLevel = GridTerminalSystem.GetBlockWithName(lcdScreenName) as IMyTextPanel;
    // printing the default text value on the screen 
    LCD_ControlRoom_OxygenLevel.WritePublicText(screenText, false);
    // setting the default font color to blue 
    LCD_ControlRoom_OxygenLevel.SetValue("FontColor", normalTextColor);
    for (int i = 0; i < oxygenTank_Names.Count; i++)
    {
        // setting the default oxygen level to 0 
        oxygenLevel = 0F;
        // finding the first item of the oxygen tanks 
        oxygenTank = GridTerminalSystem.GetBlockWithName(oxygenTank_Names[i]) as IMyOxygenTank;
        // getting the value of the current oxygen level 
        oxygenLevel = oxygenTank.GetOxygenLevel();
        // making the oxygen level in the proper format 
        oxygenLevel = oxygenLevel * 100;
        // adding new line in the lcd screen with the oxygen value of the current tank 
        screenText = screenText + oxygenTank_Names[i] + " : " + oxygenLevel.ToString("##.##") + "%\n";
        // checking the value of the oxygen level to take proper action based on it 
        if (oxygenLevel < thresholdLevel)
        {
            // here oxygen level is below threshold, we will print red text and turn on oxygen generators 
            LCD_ControlRoom_OxygenLevel.SetValue("FontColor", emergencyTextColor);
            // turning on the oxygen generators 
            // setting the status to true = online
            oxygenGenerators_Status = true;
            for (int ii = 0; ii < oxygenGenerator_Names.Count; ii++)
            {
                oxygenGenerator = GridTerminalSystem.GetBlockWithName(oxygenGenerator_Names[ii]) as IMyOxygenGenerator;
                oxygenGenerator.GetActionWithName("OnOff_On").Apply(oxygenGenerator);
            }
        }
        else
        {
            // oxygen levels are above threshold, we will turn off oxygen generators
            // setting the status to false = offline
            oxygenGenerators_Status = false;
            for (int ii = 0; ii < oxygenGenerator_Names.Count; ii++)
            {
                oxygenGenerator = GridTerminalSystem.GetBlockWithName(oxygenGenerator_Names[ii]) as IMyOxygenGenerator;
                oxygenGenerator.GetActionWithName("OnOff_Off").Apply(oxygenGenerator);
            }
        }
    }
    // adding text to indicate status of oxygen generators
    for (int iii = 0; iii < oxygenGenerator_Names.Count; iii++)
    {
        oxygenGenerator = GridTerminalSystem.GetBlockWithName(oxygenGenerator_Names[iii]) as IMyOxygenGenerator;
        if (oxygenGenerators_Status == true)
        {
            // generator is online
            screenText = screenText + oxygenGenerator_Names[iii] + ": Online\n";
        }
        else
        {
            // generator is offline
            screenText = screenText + oxygenGenerator_Names[iii] + ": Offline\n";
        }
    }
    LCD_ControlRoom_OxygenLevel.WritePublicText(screenText, false);
    LCD_ControlRoom_OxygenLevel.ShowPublicTextOnScreen();
}
