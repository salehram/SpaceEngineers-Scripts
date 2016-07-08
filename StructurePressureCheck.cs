Color emergencyColor = new Color(255, 0, 0); 
Color normalColor = new Color(255, 255, 155); 
 
void Main(string argument) 
{ 
    checkVents("AirVent-ControlRoom", "Lights-Interior", "Alarm-Blocks", "Doors"); 
} 
 
void checkVents(string ventsGroupName, string lightsGroupName, string alarmGroupName, string doorGroupName) 
{ 
    var allGroups = new List<IMyBlockGroup>(); 
    GridTerminalSystem.GetBlockGroups(allGroups); 
    for (int i = 0; i < allGroups.Count; i++) 
    { 
        if (allGroups[i].Name == ventsGroupName) 
        { 
            List<IMyTerminalBlock> AirVents = allGroups[i].Blocks; 
            for (int ii = 0; ii < AirVents.Count; ii++) 
            { 
                IMyAirVent ventUnit = AirVents[ii] as IMyAirVent; 
                bool status = ventUnit.CanPressurize; 
                if (status == true) 
                { 
                    // normal status 
                    emergencyLights(false, lightsGroupName); 
                    activateAlarm(false, alarmGroupName); 
                } 
                else 
                { 
                    // before declaring emergency, we need to check if it is actually set to depressurize 
                    bool ventMode = ventUnit.IsDepressurizing; 
                    if (ventMode==false) 
                    { 
                        // emergency status, the vent is not set to depressurizing! 
                        emergencyLights(true, lightsGroupName);
                        activateAlarm(true, alarmGroupName);
                        lockDown(true, doorGroupName);
                    } 
                    else 
                    { 
                        // normal status, we have set the vent to actually depressurize 
                        emergencyLights(false, lightsGroupName); 
                        activateAlarm(false, alarmGroupName);
                        lockDown(false, doorGroupName);
                    } 
                } 
            } 
        } 
    } 
} 
 
void emergencyLights(bool Status, string lightGroup_Name) 
{ 
    var AllGroups = new List<IMyBlockGroup>(); 
    GridTerminalSystem.GetBlockGroups(AllGroups); 
    for (int i = 0; i < AllGroups.Count; i++) 
    { 
        if (AllGroups[i].Name == lightGroup_Name) 
        { 
            List<IMyTerminalBlock> Lights = AllGroups[i].Blocks; 
            for (int ii = 0; ii < Lights.Count; ii++) 
            { 
                IMyInteriorLight Light = Lights[ii] as IMyInteriorLight; 
                if (Light != null) 
                { 
                    Color CurrentColor = Light.GetValue<Color>("Color"); 
                    if (CurrentColor != null) 
                    { 
                        // checking to see if we have emergency status 
                        if (Status == true) 
                        { 
                            // emergency status 
                            if (CurrentColor != emergencyColor) 
                            { 
                                Light.SetValue("Color", emergencyColor); 
                            } 
                        } 
                        else 
                        { 
                            // normal status 
                            if (CurrentColor != normalColor) 
                            { 
                                Light.SetValue("Color", normalColor); 
                            } 
                        } 
                    } 
                } 
            } 
        } 
    } 
} 
 
void activateAlarm(bool Status, string alarmGroup_Name) 
{ 
    var allGroups = new List<IMyBlockGroup>(); 
    GridTerminalSystem.GetBlockGroups(allGroups); 
    for (int i = 0; i < allGroups.Count; i++) 
    { 
        if (allGroups[i].Name == alarmGroup_Name) 
        { 
            List<IMyTerminalBlock> AlarmBlocks = allGroups[i].Blocks; 
            for (int ii = 0; ii < AlarmBlocks.Count; ii++) 
            { 
                IMySoundBlock alarm = AlarmBlocks[ii] as IMySoundBlock; 
                if (Status == true) 
                { 
                    alarm.GetActionWithName("PlaySound").Apply(alarm); 
                } 
                else 
                { 
                    alarm.GetActionWithName("StopSound").Apply(alarm); 
                } 
            } 
        } 
    } 
}

void lockDown(bool Status, string doorGroup_Name)  
{  
    var allGroups = new List<IMyBlockGroup>();  
    GridTerminalSystem.GetBlockGroups(allGroups);  
    for (int i = 0; i < allGroups.Count; i++)  
    {  
        if (allGroups[i].Name == doorGroup_Name)  
        {  
            List<IMyTerminalBlock> DoorBlocks = allGroups[i].Blocks;  
            for (int ii = 0; ii < DoorBlocks.Count; ii++)  
            {  
                IMyDoor door = DoorBlocks[ii] as IMyDoor;  
                if (Status == true)  
                {
                    //closing all required doors
                    door.GetActionWithName("Open_Off").Apply(door);
                    //locking all closed doors and turn them off
                    /**bool doorStatus = door.Open;
                    if (doorStatus == false)
                    {
                        door.GetActionWithName("OnOff_Off").Apply(door);
                    }**/
                }
                else  
                {
                    //we need to restore the doors to "on" status in case we turned them off before
                    //door.GetActionWithName("OnOff_On").Apply(door);
                }  
            }  
        }  
    }  
}
